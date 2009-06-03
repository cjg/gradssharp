using System;
using System.Data;
using System.Xml.Linq;

namespace GradsSharp
{
    /// <summary>
    /// Summary description for Dim
    /// </summary>
    public class Dims
    {
        private int defaultFileNumber;
        private int x1,x2;
        private bool xVarying=false;
        private int y1,y2;
        private bool yVarying=false;
        private int z1,z2;
        private bool zVarying=false;
        private int t1,t2;
        private bool tVarying=false;
        private int e1,e2;

        private double lon1,lon2;
        public double getMinLon() { return lon1; }
        public double getMaxLon() { return lon2; }

        private double lat1,lat2;
        public double getMinLat() { return lat1; }
        public double getMaxLat() { return lat2; }

        private double lev1,lev2;
        public double getMinLev() { return lev1; }
        public double getMaxLev() { return lev2; }

        private DateTime time1,time2;
        public DateTime getMinTime() { return time1; }
        public DateTime getMaxTime() { return time2; }


        override public String ToString()
        {
            return defaultFileNumber+";"+
            lon1+"-"+lon2+","+x1+"-"+x2+":"+xVarying+";"+
            lat1+"-"+lat2+","+y1+"-"+y2+":"+yVarying+";"+
            lev1+"-"+lev2+","+z1+"-"+z2+":"+zVarying+";";
        }

        public Dims(Grads grads)
        {
            Result gr=grads.Query("dims");
            init(gr.Output);
        }

        public Dims(Result gr)
        {
            init(gr.Output);
        }

        private void init(String[] lines)
        {
            String line;
            String[] parts;

            defaultFileNumber=int.Parse(lines[0].Trim().Replace("Default file number is: ",""));

            line = lines[1];
            while (line.Contains("  "))
                line = line.Replace("  ", " ");

            parts=line.Split(' ');
            lon1=Double.Parse(parts[5]);
            if (parts[2].Equals("varying"))
            {
                xVarying=true;

                double dx1=Double.Parse(parts[10]);
                x1=int.Parse(parts[10]);

                lon2=Double.Parse(parts[7]);
                double dx2=Double.Parse(parts[12]);
                x2=int.Parse(parts[12]);

                if ((dx1-x1)!=0) x1=x1-1;
                if ((dx2-x2)!=0) x2=x2+1;
            }
            else
            {
                lon2=lon1;
                x1=int.Parse(parts[8]);
                x2=x1;
            }
        
            line = lines[2];
            while (line.Contains("  "))
                line = line.Replace("  ", " ");

            parts=line.Split(' ');
            lat1=Double.Parse(parts[5]);
            if (parts[2].Equals("varying"))
            {
                yVarying=true;

                double dy1=Double.Parse(parts[10]);
                y1=int.Parse(parts[10]);

                lat2=Double.Parse(parts[7]);
                double dy2=Double.Parse(parts[12]);
                y2=int.Parse(parts[12]);

                if ((dy1-y1)!=0) y1=y1-1;
                if ((dy2-y2)!=0) y2=y2+1;
            }
            else
            {
                lat2=lat1;
                y1=int.Parse(parts[8]);
                y2=y1;
            }

            line = lines[3];
            while (line.Contains("  "))
                line = line.Replace("  ", " ");

            parts=line.Split(' ');
            lev1=Double.Parse(parts[5]);
            if (parts[2].Equals("varying"))
            {
                zVarying=true;

                double dz1=Double.Parse(parts[10]);
                z1=int.Parse(parts[10]);

                lev2=Double.Parse(parts[7]);
                double dz2=Double.Parse(parts[12]);
                z2=int.Parse(parts[12]);

                if ((dz1-z1)!=0) z1=z1-1;
                if ((dz2-z2)!=0) z2=z2+1;
            }
            else
            {
            lev2=lev1;
            z1=int.Parse(parts[8]);
            z2=z1;
            }
    /*
        line=Utils.removeSpaces(lines[4]);
        parts=line.split(" ");
        lon1=Double.parseDouble(parts[5]);
        int t1=Integer.parseInt(parts[10]);
        if (parts[2].equals("varying")) {
          tVarying=true;
          lon2=Double.parseDouble(parts[7]);
          double dx2=Double.parseDouble(parts[12]);
          int t2=Integer.parseInt(parts[12]);
        } else {
          lon2=lon1;
          t2=t1;
        }

        line=Utils.removeSpaces(lines[5]);
        parts=line.split(" ");
        lon1=Double.parseDouble(parts[5]);
        double dx1=Double.parseDouble(parts[10]);
        int x1=Integer.parseInt(parts[10]);
        if (parts[2].equals("varying")) {
          xVarying=true;
          lon2=Double.parseDouble(parts[7]);
          double dx2=Double.parseDouble(parts[12]);
          int x2=Integer.parseInt(parts[12]);
          if ((dx1-x1)!=0) x1=x1-1;
          if ((dx2-x2)!=0) x2=x2+1;


    */
        }
    }
}
