using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace GradsSharp
{

    /// <summary>
    /// Summary description for Grads
    /// </summary>
    public class Grads 
    {
        public enum FileType { CTL, SDF, XDF };
        public enum GxOutType { Vector, Shaded, Contour, Print, Custom};
        private Process process;

        private String scratchDir="";
        private String cmd;// = "c:\\ProgramFile\\grads\\bin\\grads.exe";//
        private String tempPrefix;// = "\\tmp";
        private String gadDir;
        private String scriptsDir;
        private string info;
        private object mutex;
        private Dimension x;
        private Dimension y;
        private Dimension z;
        private Dimension t;
        private Dimension lon;
        private Dimension lat;
        private Dimension lev;
        private FileInfo fileinfo;
        private GxOutType gxouttype;
        private string gxoutcustom;

        public static String RandFilename(String prefix, String suffix) 
        {
            String result=null;

            do 
            {
                Random random = new Random();
                String randName = random.Next(1000000).ToString();
                result = prefix + "000000".Substring(6 - randName.Length) + randName + suffix;
            } while (result == null);
            return result;
        }

        public Grads()
	    {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            NameValueCollection appSettings = System.Web.Configuration.WebConfigurationManager.AppSettings;

            tempPrefix = appSettings.Get("TempPrefix");
            cmd = appSettings.Get("GradsCommand");
            if (cmd == null)
                cmd = "C:\\grads-2.0.a5\\bin\\grads.exe";
            gadDir = appSettings.Get("GradsEnvironment");
            scriptsDir = appSettings.Get("GradsScripts");


            scratchDir = RandFilename(tempPrefix , "");
            Directory.CreateDirectory(scratchDir);

            process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = "-b -l -u";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.EnvironmentVariables.Add("GADDIR", gadDir);
            process.Start();

            Result co = new Result(process.StandardOutput);
            if (co.Status != 0)
                throw new Exception("Cannot start GrADS!");
            info = co.CompactOutput;
            mutex = new object();
            this.GxOut = GxOutType.Print;
        }

        public string Info
        {
            get
            {
                return info;
            }
        }

        public Dimension X
        {
            get { return x; }
        }

        public Dimension Y
        {
            get { return y; }
        }

        public Dimension Z
        {
            get { return z; }
        }

        public Dimension T
        {
            get { return t; }
        }

        public Dimension Lon
        {
            get { return lon; }
        }

        public Dimension Lat
        {
            get { return lat; }
        }

        public Dimension Lev
        {
            get { return lev; }
        }

        public FileInfo FileInfo
        {
            get {
                if(fileinfo == null)
                    fileinfo = new FileInfo(this);
                return fileinfo;
            }
        }

        public DateTime Time
        {
            get 
            {
                double tset = T.Value;
                Result r = Tell("set T 1");
                string tmp = r.Output[0].Replace("\n", "").Trim();
                string[] s = tmp.Substring(tmp.LastIndexOf(' ') + 1).Split(':');
                Tell("set T " + tset);
                DateTime d = new DateTime(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), 0, 0);
                d.AddHours(tset - 1);
                return d;
            }
            set 
            {
                DateTime start = this.Time;
                System.TimeSpan t = value.Subtract(this.Time);
                this.T.Value = t.Days * 24 + t.Hours + 1;
            }
        }

        public GxOutType GxOut {
            get {
                return gxouttype;
            }
            set {
                Result co = null;
                switch(value)
                {
                    case GxOutType.Contour:
                        co = Set("gxout", "contour");
                        break;
                    case GxOutType.Custom:
                        throw new Exception("Cannot explicitly set GxOutType to Custom, set the GxOutCustom property instead");
                    case GxOutType.Print:
                        co = Set("gxout", "print");
                        break;
                    case GxOutType.Shaded:
                        co = Set("gxout", "shaded");
                        break;
                    case GxOutType.Vector:
                        co = Set("gxout", "vector");
                        break;
                }
                if(co.Status != 0)
                    throw new Exception("Cannot set GxOutType to " + value);
                gxouttype = value;
            }
        }

        public string GxOutCustom 
        {
            get 
            {
                if(gxouttype == GxOutType.Custom)
                    return gxoutcustom;
                return gxouttype.ToString();
            }
            set
            {
                Result co = Tell("set gxout " + value);
                if(co.Status != 0)
                    throw new Exception("Cannot set custom gxout " + value);
                gxouttype = GxOutType.Custom;
                gxoutcustom = value;
            }
        }

        public Result Tell(string command)
        {
            lock (mutex)
            {
                // FIXME: fix grads to handle \r\n 
                process.StandardInput.Write(command + "\n");
                Result co = new Result(process.StandardOutput);
                // TODO: add log
                return co;
            }
        }

        public void Open(String filename, FileType filetype)
        {
            Result c = Tell("close 1");
            Result co;
            switch (filetype)
            {
                case FileType.CTL:
                    co = Tell("open " + filename);
                    break;
                case FileType.SDF:
                    co = Tell("sdfopen " + filename);
                    break;
                default:
                    co = Tell("xdfopen " + filename);
                    break;
            }
            if (co.Status != 0)
                throw new Exception("Cannot open " + filename);
            
            x = new Dimension("X", this);
            lon = new Dimension("Lon", this);
            x.DimensionChanged += lon.OnDimensionChanged;
            lon.DimensionChanged += x.OnDimensionChanged;

            y = new Dimension("Y", this);
            lat = new Dimension("Lat", this);
            y.DimensionChanged += lat.OnDimensionChanged;
            lat.DimensionChanged += y.OnDimensionChanged;

            z = new Dimension("Z", this);
            lev = new Dimension("Lev", this);
            z.DimensionChanged += lev.OnDimensionChanged;
            lev.DimensionChanged += z.OnDimensionChanged;

            t = new Dimension("T", this);
            fileinfo = null;
        }

        public void Open(string ctl_directory, string ctl_prefix, DateTime time)
        {
            DirectoryInfo root = new DirectoryInfo(ctl_directory);
            string found = null;
            foreach (System.IO.FileInfo f in root.GetFiles())
            {
                if (!f.Extension.Equals(".ctl") || !f.Name.StartsWith(ctl_prefix))
                    continue;
                string[] d = f.Name.Replace(".ctl", "").Replace(ctl_prefix, "").Replace("_", "-").Split('-');
                DateTime t = new DateTime(int.Parse(d[0]), int.Parse(d[1]), int.Parse(d[2]), int.Parse(d[3]), 
                    int.Parse(d[4]), int.Parse(d[5]));
                if (t.CompareTo(time) > 0)
                    break;
                found = f.Name;
            }
            if (found == null)
                throw new Exception("Cannot find Ctl file for " + time.ToString());
            Open(ctl_directory + "\\" + found, FileType.CTL);
        }

        public double Amean(string var)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Result co = Tell("display amean(" + var 
                + ", lon=" + lon.Start
                + ", lon=" + lon.End
                + ", lat=" + lat.Start
                + ", lat=" + lat.End + ")");
            if (co.Status != 0)
                throw new Exception("Cannot compute the amean!");
            foreach (string s in co.Output)
            {
                try
                {
                    string tmp = s.Trim();
                    return double.Parse(tmp);
                }
                catch (FormatException e)
                {
                }
            }
            throw new Exception("Cannot compute the amean!");
        }
        
        public Result Eval(string var, string expr)
        {
            return Tell(var + " = " + expr);
        }

        public Result Set(String what, String how)
        {
            return Tell("set " + what + " " + how);
        }

        public Result Display(String expr)
        {
            return Tell("display " + expr);
        }
        
        public Result DrawMap()
        {
            Set("mpdset", "hires");
            Set("mpt", "0 1 1 6");
            Set("mpt", "1 15 1 1");
            return Tell("draw map");
        }

        public Result MapOff()
        {
            return Set("map","* off");
        }

        public Result DrawColorBar()
        {
            return Tell("run " + scriptsDir + "\\" + "cbarn.gs");
        }

        public void Clear()
        {
            Result co = Tell("clear");
            if(co.Status != 0)
                throw new Exception("Cannot clear!");
        }

        public void Draw(String opt)
        {
            Result co = Tell("draw " + opt);
            if(co.Status != 0)
                throw new Exception("Cannot draw " + opt);
        }

        public Result Query(String opt)
        {
            return Tell("query " + opt);
        }

        public Result PrintImage(int xsize, int ysize, bool white)
        {
            string color = "black";
            if (white) 
                color = "white";
            String tempname = RandFilename(scratchDir + "\\", ".bmp");
              
            Result co = Tell("printim " + tempname + " x" + xsize + " y" + ysize + " " + color);
            if(co.Status != 0)
                throw new Exception("Cannot print image!");
            System.Drawing.Image image = System.Drawing.Bitmap.FromFile(tempname);
            co.Image = image; 
   		    return co;
        }

        public void Quit() 
        {
            Result co = Tell("quit");
            if(co.Status != 0)
                throw new Exception("Cannot quit!");
        }
    }
}
