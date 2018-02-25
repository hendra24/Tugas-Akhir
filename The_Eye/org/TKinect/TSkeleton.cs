using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.TKinect
{
    public class TSkeleton
    {
        // Summary:
        //     Gets the edges of the field of view that clip the body.
        public FrameEdges ClippedEdges { get; set; }
       
        //
        // Summary:
        //     Gets the confidence of the body's left hand state.
        public TrackingConfidence HandLeftConfidence { get; set; }
        //
        // Summary:
        //     Gets the status of the body's left hand state.
        public HandState HandLeftState { get; set; }
        //
        // Summary:
        //     Gets the confidence of the body's right hand state.
        public TrackingConfidence HandRightConfidence { get; set; }
        //
        // Summary:
        //     Gets the status of the body's right hand state.
        public HandState HandRightState { get; set; }
        //
        // Summary:
        //     Gets whether or not the body is restricted.
        public bool IsRestricted { get; set; }
        //
        // Summary:
        //     Gets whether or not the body is tracked.
        public bool IsTracked { get; set; }
        //
        // Summary:
        //     Gets the number of joints in a body.
        //public static int JointCount { get; set; }
        //
        // Summary:
        //     Gets the joint orientations of the body.
        //public Dictionary<JointType, JointOrientation> JointOrientations { get; }
        // Summary:
        //     The type of joint.
        //public JointType[] JointType;
        //
        // Summary:
        //     The orientation of the joint.
        public Vector4[] Orientation;
        //
        // Summary:
        //     Gets the joint positions of the body.
        //public Dictionary<JointType, Joint> Joints { get; }
        // Summary:
        //     The type of joint.
        //public JointType JointType;
        //
        // Summary:
        //     The position of the joint in camera space.
        public CameraSpacePoint[] Position;
        //
        // Summary:
        //     The tracking state of the joint.
        public TrackingState[] TrackingState;
        //
        // Summary:
        //     Gets the lean vector of the body.
        public PointF Lean;
        //
        // Summary:
        //     Gets the tracking state for the body lean.
        public TrackingState LeanTrackingState { get; set; }
        //
        // Summary:
        //     Gets the tracking ID for the body.
        public ulong TrackingId { get; set; }
        public long TimePeriod;
        public long AbsoluteTime;
        public Vector4 FloorClipPlane;
        public int ElevationAngle = 0;
        /*
        //bone rotation
        //Quaternion - x y z w
        public float[][] AbsoluteRotation;
        public float[][] HierarchicalRotation;
        //None = 0,
        //Right = 1,
        //Left = 2,
        //Top = 4,
        //Bottom = 8,
        public int ClippedEdges = 0;
        //x y z
        public float[] Position;
        //joint - x y z
        public float[][] JointsPositions;
        //NotTracked = 0,
        //Inferred = 1,
        //Tracked = 2,
        public int[] JointsTrackingState;

        //
        public ulong TrackingId;

        //NotTracked = 0,
        //PositionOnly = 1,
        //Tracked = 2,
        public int TrackingState;
        public long TimePeriod;
        public long AbsoluteTime;

        public System.Tuple<float, float, float, float> FloorClipPlane = new Tuple<float, float, float, float>(0, 0, 0, 0);
        public int ElevationAngle = 0;
        public int SeatedMode = 0;

        public long SyncTime = 0;*/

        public TSkeleton Clone()
        {
            TSkeleton ts = new TSkeleton();
            ts.ClippedEdges = this.ClippedEdges;
            ts.HandLeftConfidence = this.HandLeftConfidence;
            ts.HandLeftState = this.HandLeftState;
            ts.HandRightConfidence = this.HandRightConfidence;
            ts.HandRightState = this.HandRightState;
            ts.IsRestricted = this.IsRestricted;
            ts.IsTracked = this.IsTracked;
            ts.Orientation = new Vector4[this.Orientation.Length];
            for (int a = 0; a < ts.Orientation.Length; a++)
            {
                ts.Orientation[a].W = this.Orientation[a].W;
                ts.Orientation[a].X = this.Orientation[a].X;
                ts.Orientation[a].Y = this.Orientation[a].Y;
                ts.Orientation[a].Z = this.Orientation[a].Z;
            }

            ts.Position = new CameraSpacePoint[this.Position.Length];
            for (int a = 0; a < ts.Orientation.Length; a++)
            {
                ts.Position[a].X = this.Position[a].X;
                ts.Position[a].Y = this.Position[a].Y;
                ts.Position[a].Z = this.Position[a].Z;
            }

            ts.TrackingState = new TrackingState[this.TrackingState.Length];
            for (int a = 0; a < ts.Orientation.Length; a++)
            {
                ts.TrackingState[a] = this.TrackingState[a];
            }
            ts.Lean = this.Lean;
            ts.LeanTrackingState = this.LeanTrackingState;
            ts.TrackingId = this.TrackingId;
            ts.TimePeriod = this.TimePeriod;
            ts.AbsoluteTime = this.AbsoluteTime;
            ts.FloorClipPlane = this.FloorClipPlane;
            ts.ElevationAngle = this.ElevationAngle;
            return ts;
        }
    }
}
