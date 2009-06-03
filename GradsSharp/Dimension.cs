using System;
using System.Globalization;

namespace GradsSharp
{
    public class Dimension
    {
        private string name;
        private bool varying;
        private double start;
        private double end;
        private double limit;
        private Grads grads;

        public delegate void DimensionEventHandler(Dimension source);
        public event DimensionEventHandler DimensionChanged;

        public Dimension(string name, Grads grads)
        {
            this.grads = grads;
            this.name = name;
            Result co = grads.Tell("q dims");
            if (co.Status != 0)
                throw new Exception("Cannot query grads!");
            parse_output(co.Output);
            co = grads.Tell("q file");
            limit = -1;
            foreach (string s in co.Output)
            {
                if (!s.Contains(name + "size"))
                    continue;
                string tmp = s.Substring(s.IndexOf(name));
                tmp = tmp.Substring(tmp.IndexOf("= ") + 2);
                tmp = tmp.Substring(0, tmp.IndexOf(" "));
                limit = double.Parse(tmp);
            }
        }

        public void OnDimensionChanged(Dimension source)
        {
            Result co = grads.Tell("q dims");
            if (co.Status != 0)
                throw new Exception("Cannot query grads!");
            parse_output(co.Output);
        }

        private void parse_output(string[] output)
        {
            foreach (string line in output)
            {
                string s = line.Replace("  ", "\t");
                while (s.Contains("  "))
                    s = s.Replace("  ", " ");
                while (s.Contains("\t\t"))
                    s = s.Replace("\t\t", "\t");
                s = s.Replace("\t ", "\t").Replace(" \t", "\t");
                string[] splitted = s.Split('\t');
                if (splitted.Length != 3)
                    continue;
                varying = splitted[0].Substring(splitted[0].LastIndexOf(' ') + 1).Equals("varying");
                if (splitted[1].StartsWith(name + " "))
                    s = splitted[1];
                else if (splitted[2].StartsWith(name + " "))
                    s = splitted[2];
                else continue;
                s = s.Substring(s.IndexOf('=') + 2);
                if (!varying)
                {
                    start = double.Parse(s);
                    end = start;
                }
                else
                {
                    start = double.Parse(s.Substring(0, s.IndexOf(' ')));
                    s = s.Substring(s.LastIndexOf(' ') + 1);
                    end = double.Parse(s);
                }
                return;
            }
            throw new Exception("Dimension " + name + " not found!");
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool Varying
        {
            get
            {
                return varying;
            }
        }

        public double Start
        {
            get { return start; }
            set
            {
                if (limit > 0 && (value <= 0 || value > limit))
                    throw new Exception("Cannot set Start value " +  value + ": beyond the limit!");
                Result co = grads.Tell("set " + name + " " + value + " " + End);
                if (co.Status != 0)
                    throw new Exception("Cannot set " + name + " to " + value + " " + End);
                start = value;
                DimensionChanged(this);
            }
        }

        public double End
        {
            get { return end; }
            set
            {
                if (limit > 0 && (value <= 0 || value > limit))
                    throw new Exception("Cannot set End value " +  value + ": beyond the limit!");
                Result co = grads.Tell("set " + name + " " + Start + " " + value);
                if (co.Status != 0)
                    throw new Exception("Cannot set " + name + " to " + Start + " " + value);
                end = value;
                DimensionChanged(this);
            }
        }

        public double Value
        {
            get { return start; }
            set
            {
                if(limit > 0 && (value <= 0 || value > limit))
                    throw new Exception("Cannot set value " +  value + ": beyond the limit!");
                Result co = grads.Tell("set " + name + " " + value);
                if (co.Status != 0)
                    throw new Exception("Cannot set " + name + " to " + value);
                start = value;
                end = value;
                if (DimensionChanged != null)
                    DimensionChanged(this);
            }
        }

        public override string ToString()
        {
            string s = "Dimension: " + name;
            if (varying)
                s += " varying from " + start + " to " + end;
            else
                s += " fixed at " + start;
            return s;
        }
    }
}