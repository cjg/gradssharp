using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

namespace GradsSharp
{
    class Kml
    {
        private Coordinate sw = null;
        private Coordinate ne = null;
        private string name = null;
        private string desc = null;

        public Coordinate SouthWestCorner
        {
            get { return sw; }
            set { sw = value; }
        }

        public Coordinate NorthEastCorner
        {
            get { return ne; }
            set { ne = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Desc
        {
            get { return desc; }
            set { desc = value; }
        }
        
        public Kml()
        {
        
        }

        

        public string Get(string url)
        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            String kmlString =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<kml xmlns=\"http://www.opengis.net/kml/2.2\">\n" +
                "  <Folder>\n";
            
            if (name != "") kmlString +=
                  "    <name>" + name + "</name>\n";

            if (desc != "") kmlString +=
                "    <description>" + desc + "</description>\n";

            kmlString +=
                "    <GroundOverlay>\n" +
                "      <Icon>\n" +
                "        <href>" + url + "</href>\n" +
                "      </Icon>\n" +
                "      <LatLonBox>\n" +
                "        <north>" + ne.Latitude + "</north>\n" +
                "        <south>" + sw.Latitude + "</south>\n" +
                "        <east>" + ne.Longitude + "</east>\n" +
                "        <west>" + sw.Longitude + "</west>\n" +
                "        <rotation>0.0</rotation>\n" +
                "      </LatLonBox>\n" +
                "    </GroundOverlay>\n" +
                "  </Folder>\n" +
                "</kml>\n";
            
            return kmlString;
        }
    }
}
