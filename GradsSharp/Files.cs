using System;

namespace GradsSharp
{

    /// <summary>
    /// Summary description for Files
    /// </summary>
    public class Files : System.Collections.Generic.List<File>
    {
        public Files()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public Files(Grads grads)
        {
            Result gr=grads.Query("files");
            int nFiles=0;
            foreach (String line in gr.Output)
            {
                if (line.StartsWith("File")==true) {
                    nFiles++;
                    Result grQueryFile=grads.Query("file "+nFiles);
                    File file=new File(grQueryFile);
                    Add(file);
                }
            }
        }
    }
}