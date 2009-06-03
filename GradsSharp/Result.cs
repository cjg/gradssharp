using System.Drawing;
using System.IO;

namespace GradsSharp
{
    public class Result
    {
        private string output;
        private int result_code;
        private Image image;

        public Result(StreamReader sr)
        {
            bool ipc_found = false;
            string line;
            output = "";
            result_code = -1;

            while ((line = sr.ReadLine()) != null)
            {
                if (ipc_found && line.StartsWith("<RC>"))
                {
                    string s = line.Substring(line.IndexOf(' '));
                    s = s.Substring(0, s.IndexOf('<'));
                    result_code = int.Parse(s.Trim());
                    break;
                }
                if (ipc_found)
                {
                    output += line + "\n";
                }
                if (line.StartsWith("<IPC>"))
                    ipc_found = true;
            }
        }

        public int Status
        {
            get
            {
                return result_code;
            }
        }

        public string[] Output
        {
            get
            {
                return output.Split('\n');
            }
        }

        public string CompactOutput
        {
            get
            {
                return output;
            }
        }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }
    }
}