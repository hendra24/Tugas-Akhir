using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class BallBoundingBox
    {
        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public double Radius = 0;
        public int JointType1;
        public int JointType2;
        public BallBoundingBox(double X, double Y, double Z, double Radius, int JointType1, int JointType2)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Radius = Radius;
            this.JointType1 = JointType1;
            this.JointType2 = JointType2;
        }
        public bool Collide(BallBoundingBox ballBoundingBox)
        {
            double distance = ((X - ballBoundingBox.X) * (X - ballBoundingBox.X)) + ((Y - ballBoundingBox.Y) * (Y - ballBoundingBox.Y)) + ((Z - ballBoundingBox.Z) * (Z - ballBoundingBox.Z));
            if (distance < ((ballBoundingBox.Radius + Radius) * (ballBoundingBox.Radius + Radius)))
                return true;
            return false;
        }
    }
}
