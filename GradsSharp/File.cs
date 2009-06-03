using System;
using System.Collections.Generic;

namespace GradsSharp
{

    /// <summary>
    /// Summary description for File
    /// </summary>
    public class File
    {
        private int index;
        private String title;
        private String descriptor;
        private String binary;
        private int type = 0;
        private int xSize;
        private int ySize;
        private int zSize;
        private int tSize;
        private int eSize;

        private List<Var> vars = new List<Var>();

        public File(Result gr)
        {
            String[] lines=gr.Output;
            String left;
            int pos;

            pos=lines[0].Trim().IndexOf(":");
            left=lines[0].Trim().Substring(0,pos-1).Trim();
            title=lines[0].Trim().Substring(pos+1).Trim();

            pos=left.LastIndexOf(" ");
            index=int.Parse(left.Substring(pos).Trim());
            descriptor=lines[1].Trim().Replace("Descriptor: ","").Trim();
            binary=lines[2].Trim().Replace("Binary: ","").Trim();

            left=lines[3].Trim().Replace("Type = ","");
            if (left.Equals("Type"))
                type=1;

            left=lines[4].Trim().Replace(" = ","=");
            String[] parts = left.Split(' ');

            xSize = int.Parse(parts[0].Split('=')[1]);
            ySize = int.Parse(parts[2].Split('=')[1]);
            zSize = int.Parse(parts[4].Split('=')[1]);
            tSize = int.Parse(parts[6].Split('=')[1]);
            eSize = int.Parse(parts[8].Split('=')[1]);

            left=lines[5].Trim().Replace("Number of Variables = ","").Trim();
        
            int nVars = int.Parse(left);
    
            for (int i=0;i<nVars;i++)
            {
                String line=lines[6+i].Trim();
                vars.Add(new Var(line));
            }

        }

        override public String ToString() {
            String result="";
            result+=index+";";
            result+=title+";";
            result+=descriptor+";";
            result+=binary+";";
            result+=type+";";
            result+=xSize+";"+ySize+";"+zSize+";"+tSize+";"+eSize+";";
            result+=vars.Count+";";
        
            foreach(Var var in vars)
                result+=var+";";
            return result;
        }
    }
}
