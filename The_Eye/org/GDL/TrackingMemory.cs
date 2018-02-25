using System;
using System.Collections.Generic;
using System.Text;

namespace org.GDL
{
    class TrackingMemory
    {
        public int Line = 0;
        public double TimePeriod = 0;
        public Point3D[] BodyParts = null;
        public String[] Conclusions = null;
        public int User = -1;
        public Dictionary<string, object> Features = null;
        //new Dictionary<string, object>();
    }
}
