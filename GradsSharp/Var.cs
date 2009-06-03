using System;

namespace GradsSharp
{

    /// <summary>
    /// Summary description for Var
    /// </summary>
    public class Var
    {
        private String name;
        private int nLevels;
        private int gribCode;
        private String desc;
        private String unit;

        public Var(String name, int nLevels, int gribCode, String desc, String unit)
        {
            this.name = name;
            this.nLevels = nLevels;
            this.gribCode = gribCode;
            this.desc = desc;
            this.unit = unit;
        }

        public Var(String line)
        {
            line = line.Trim();
            String[] parts = line.Split(' ');
            name = parts[0];
            nLevels = int.Parse(parts[1]);
            gribCode = int.Parse(parts[2]);
            String temp = "";
            for (int i = 3; i < parts.Length; i++)
                temp += parts[i] + " ";
            temp = temp.Trim();

            int pos = temp.IndexOf("(");
            int diff = temp.Length - pos - 1;
            unit = temp.Substring(pos + 1, diff - 1);
            desc = temp.Substring(0, pos - 1);
        }

        override public String ToString()
        {
            return name + " " + nLevels + " " + gribCode + " " + desc + " (" + unit + ")";
        }
    }
}
