using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class CollisionExemplar
    {
        public int Player1JointType1 = 0;
        public int Player1JointType2 = 0;
        public int Player2JointType1 = 0;
        public int Player2JointType2 = 0;
        public CollisionExemplar(int Player1JointType1, int Player1JointType2, int Player2JointType1, int Player2JointType2)
        {
            this.Player1JointType1 = Player1JointType1;
            this.Player1JointType2 = Player1JointType2;
            this.Player2JointType1 = Player2JointType1;
            this.Player2JointType2 = Player2JointType2;
        }
        public override bool Equals(Object obj)
        {
            CollisionExemplar bodyBasics2 = obj as CollisionExemplar;
            if (bodyBasics2 == null)
                return false;
            else
            {
                if (Player1JointType1 == bodyBasics2.Player1JointType1
                    && Player1JointType2 == bodyBasics2.Player1JointType2
                    && Player2JointType1 == bodyBasics2.Player2JointType1
                    && Player2JointType2 == bodyBasics2.Player2JointType2)
                return true;
            }
            return false;
        }
    }
}
