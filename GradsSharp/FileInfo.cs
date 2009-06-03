using System;

namespace GradsSharp
{

    /// <summary>
    /// Summary description for FileInfo
    /// </summary>
    public class FileInfo
    {
        private Files files=null;
        public Files getFiles()
        {
            return files;
        }

        private Dims dims=null;
        public Dims getDims()
        {
            return dims;
        }

        public FileInfo(Grads grads)
        {
            files=new Files(grads);
        }

        override public String ToString()
        {
            return dims+";"+files;
        }

    }
}