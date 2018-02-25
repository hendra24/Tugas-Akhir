using Microsoft.Kinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.TKinect
{
    class TSkeletonHelper
    {
        public static ArrayList CloneRecording(ArrayList recording)
        {
            if (recording == null)
                return null;
            ArrayList clone = new ArrayList();
            for (int a = 0; a < recording.Count; a++)
            {
                TSkeleton[] ts = (TSkeleton[])recording[a];
                TSkeleton[] tsClone = new TSkeleton[ts.Length];
                for (int b = 0; b < ts.Length; b++)
                {
                    tsClone[b] = ts[b].Clone();
                }
                clone.Add(tsClone);
            }
            return clone;
        }

        private static void WriteToFile(StreamWriter sw, PointF pf)
        {
            sw.Write(pf.X.ToString(CultureInfo.InvariantCulture) + " ");
            sw.Write(pf.Y.ToString(CultureInfo.InvariantCulture) + " ");
        }
        private static void WriteToFile(StreamWriter sw, Vector4[] array)
        {
            for (int a = 0; a < array.Length; a++)
            {
                sw.Write(array[a].W.ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(array[a].X.ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(array[a].Y.ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(array[a].Z.ToString(CultureInfo.InvariantCulture) + " ");
            }
        }
        private static void WriteToFile(StreamWriter sw, CameraSpacePoint[] array)
        {
            for (int a = 0; a < array.Length; a++)
            {
                sw.Write(array[a].X.ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(array[a].Y.ToString(CultureInfo.InvariantCulture) + " ");
                sw.Write(array[a].Z.ToString(CultureInfo.InvariantCulture) + " ");
            }
        }
        private static void WriteToFile(StreamWriter sw, TrackingState[] array)
        {
            for (int a = 0; a < array.Length; a++)
            {
                sw.Write((int)array[a] + " ");
            }
        }
        public static void SaveTSkeletonToFile(TSkeleton tSkeleton, StreamWriter sw, long currentFrameNumber)
        {
            sw.Write(currentFrameNumber + " ");
            sw.Write(tSkeleton.TimePeriod + " ");
            sw.Write((int)tSkeleton.ClippedEdges + " ");
            sw.Write((int)tSkeleton.HandLeftConfidence + " ");
            sw.Write((int)tSkeleton.HandLeftState + " ");
            sw.Write((int)tSkeleton.HandRightConfidence + " ");
            sw.Write((int)tSkeleton.HandRightState + " ");
            sw.Write(tSkeleton.IsRestricted + " ");
            sw.Write(tSkeleton.IsTracked + " ");
            WriteToFile(sw, tSkeleton.Lean);
            sw.Write((int)tSkeleton.LeanTrackingState + " ");
            WriteToFile(sw, tSkeleton.Orientation);
            WriteToFile(sw, tSkeleton.Position);
            WriteToFile(sw, tSkeleton.TrackingState);
            sw.WriteLine();
        }
        /*
        public static Skeleton TSkeletonToSkeleton(TSkeleton tSkeleton)
        {
            Skeleton skeleton = new Skeleton();
            for (int a = 0; a < 20; a++)
            {
                Microsoft.Kinect.Vector4 v = new Microsoft.Kinect.Vector4();
                v.X = tSkeleton.AbsoluteRotation[a][0];
                v.Y = tSkeleton.AbsoluteRotation[a][1];
                v.Z = tSkeleton.AbsoluteRotation[a][2];
                v.W = tSkeleton.AbsoluteRotation[a][3];
                Quaternion q = Quaternion.Identity;
                q.X = v.X;
                q.Y = v.Y;
                q.Z = v.Z;
                q.W = v.W;
                //Matrix m = Matrix.CreateFromQuaternion(q);
                //Matrix4 m4 = XNAMatrixToMatrix4(m);
                skeleton.BoneOrientations[(JointType)a].AbsoluteRotation.Quaternion = v;
                //skeleton.BoneOrientations[(JointType)a].AbsoluteRotation.Matrix = m4;


                v.X = tSkeleton.HierarchicalRotation[a][0];
                v.Y = tSkeleton.HierarchicalRotation[a][1];
                v.Z = tSkeleton.HierarchicalRotation[a][2];
                v.W = tSkeleton.HierarchicalRotation[a][3];
                q = Quaternion.Identity;
                q.X = v.X;
                q.Y = v.Y;
                q.Z = v.Z;
                q.W = v.W;
                //m = Matrix.CreateFromQuaternion(q);
                //m4 = XNAMatrixToMatrix4(m);

                skeleton.BoneOrientations[(JointType)a].HierarchicalRotation.Quaternion = v;
                //skeleton.BoneOrientations[(JointType)a].HierarchicalRotation.Matrix = m4;

                SkeletonPoint sp = new SkeletonPoint();
                sp.X = tSkeleton.JointsPositions[a][0];
                sp.Y = tSkeleton.JointsPositions[a][1];
                sp.Z = tSkeleton.JointsPositions[a][2];

                //Joint j = new Joint();
                //j.JointType = skeleton.Joints[(JointType)a].JointType;
                //j.Position = sp;

                Joint j = (skeleton.Joints[(JointType)a]);
                j.Position = sp;
                j.TrackingState = (JointTrackingState)tSkeleton.JointsTrackingState[a];
                skeleton.Joints[(JointType)a] = j;
                //skeleton.Joints[(JointType)a].Position = sp;

                skeleton.TrackingId = tSkeleton.TrackingId;
                skeleton.TrackingState = (SkeletonTrackingState)tSkeleton.TrackingState;

                sp = new SkeletonPoint();
                sp.X = tSkeleton.Position[0];
                sp.Y = tSkeleton.Position[1];
                sp.Z = tSkeleton.Position[2];
                skeleton.Position = sp;


                //skeleton.Joints[(JointType)a].Position = sp;
                //skeleton.Joints[(JointType)a].TrackingState = (JointTrackingState)skeletonHelper.JointsTrackingState[a];
                //skeleton.BoneOrientations[(JointType)a].HierarchicalRotation.
            }
            return skeleton;
        }
        */

        private static void ReadFromFile(ref PointF pf, String[] fileData, ref int startIndex)
        {
            pf.X = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture);
            startIndex++;
            pf.Y = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture);
            startIndex++;
        }

        private static void ReadFromFile(ref Vector4[] v4array, String[] fileData, ref int startIndex)
        {
            for (int a = 0; a < v4array.Length; a++)
            {
                v4array[a] = new Vector4();
                v4array[a].W = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
                v4array[a].X = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
                v4array[a].Y = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
                v4array[a].Z = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
            }
        }

        private static void ReadFromFile(ref CameraSpacePoint[] cspArray, String[] fileData, ref int startIndex)
        {
            for (int a = 0; a < cspArray.Length; a++)
            {
                cspArray[a] = new CameraSpacePoint();
                cspArray[a].X = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
                cspArray[a].Y = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
                cspArray[a].Z = float.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
            }
        }
        private static void ReadFromFile(ref TrackingState[] tsArray, String[] fileData, ref int startIndex)
        {
            for (int a = 0; a < tsArray.Length; a++)
            {
                tsArray[a] = new TrackingState();
                tsArray[a] = (TrackingState)int.Parse(fileData[startIndex], CultureInfo.InvariantCulture); 
                startIndex++;
            }
        }
        public static ArrayList ReadRecordingFromFile(String fileToPlay)
        {
            ArrayList recording = new ArrayList();
            StreamReader sr = new StreamReader(fileToPlay);
            String line;
            ArrayList arHelp = new ArrayList();
            long prevFrameNumber = -1;
            long currentFrameNumber = -1;

            long absoluteTime = 0;
            try
            {
                do
                {
                    TSkeleton tSkeleton = new TSkeleton();
                    prevFrameNumber = currentFrameNumber;
                    //do
                    {
                        
                        line = sr.ReadLine();
                        line = line.Replace(',', '.');
                        char[] delimiterChars = { ' ' };
                        int index = 0;
                        String[] array = line.Split(delimiterChars);
                        
                        currentFrameNumber = long.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.TimePeriod = long.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.ClippedEdges = (FrameEdges)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.HandLeftConfidence = (TrackingConfidence)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.HandLeftState = (HandState)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.HandRightConfidence = (TrackingConfidence)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.HandRightState = (HandState)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.IsRestricted = bool.Parse(array[index]);
                        index++;
                        tSkeleton.IsTracked = bool.Parse(array[index]);
                        index++;
                        ReadFromFile(ref tSkeleton.Lean, array, ref index);
                        tSkeleton.LeanTrackingState = (TrackingState)int.Parse(array[index], CultureInfo.InvariantCulture);
                        index++;
                        tSkeleton.Orientation = new Vector4[25];
                        ReadFromFile(ref tSkeleton.Orientation, array, ref index);
                        tSkeleton.Position = new CameraSpacePoint[25];
                        ReadFromFile(ref tSkeleton.Position, array, ref index);
                        tSkeleton.TrackingState = new TrackingState[25];
                        ReadFromFile(ref tSkeleton.TrackingState, array, ref index);
                    }

                    if (currentFrameNumber != prevFrameNumber && prevFrameNumber >= 0)
                    {
                        TSkeleton[] skeletons = new TSkeleton[arHelp.Count];
                        for (int b = 0; b < skeletons.Length; b++)
                        {
                            skeletons[b] = (TSkeleton)arHelp[b];
                        }
                        arHelp.Clear();
                        //STARY BUG ;-) GDY ŁAPAŁO WIĘCEJ NIŻ JEDNĄ KLATKĘ W 30 HZ
                        if (skeletons[0].TimePeriod > 10)
                        {
                            recording.Add(skeletons);
                        }
                        arHelp.Add(tSkeleton);
                        absoluteTime += skeletons[0].TimePeriod;
                    }
                    else
                    {
                        arHelp.Add(tSkeleton);
                        absoluteTime += tSkeleton.TimePeriod;
                    }

                }
                while (line != null && sr != null);
            }
            catch { };
            if (sr != null)
            {
                sr.Close();
                sr = null;
            }
            if (arHelp.Count > 0)
            {
                TSkeleton[] skeletons = new TSkeleton[arHelp.Count];
                for (int b = 0; b < skeletons.Length; b++)
                    skeletons[b] = (TSkeleton)arHelp[b];
                arHelp.Clear();
                //STARY BUG ;-) GDY ŁAPAŁO WIĘCEJ NIŻ JEDNĄ KLATKĘ W 30 HZ
                if (skeletons[0].TimePeriod > 10)
                {
                    recording.Add(skeletons);
                }
            }

            return recording;
        }

        public static TSkeleton SkeletonToTSkeleton(Body skeleton, long TimePeriod, Vector4 FloorClipPlane)
        {
            TSkeleton tSkeleton = new TSkeleton();
            tSkeleton.ClippedEdges = skeleton.ClippedEdges;
            tSkeleton.HandLeftConfidence = skeleton.HandLeftConfidence;
            tSkeleton.HandLeftState = skeleton.HandLeftState;
            tSkeleton.HandRightConfidence = skeleton.HandRightConfidence;
            tSkeleton.HandRightState = skeleton.HandRightState;
            tSkeleton.IsRestricted = skeleton.IsRestricted;
            tSkeleton.IsTracked = skeleton.IsTracked;
            tSkeleton.Lean = skeleton.Lean;
            tSkeleton.LeanTrackingState = skeleton.LeanTrackingState;
            tSkeleton.FloorClipPlane = FloorClipPlane;
            tSkeleton.Orientation = new Vector4[25];
            tSkeleton.Position = new CameraSpacePoint[25];
            tSkeleton.TrackingState = new TrackingState[25];
            for (int a = 0; a < tSkeleton.Orientation.Length; a++)
            {
                tSkeleton.Orientation[a] = skeleton.JointOrientations[(JointType)a].Orientation;
                tSkeleton.Position[a] = skeleton.Joints[(JointType)a].Position;
                tSkeleton.TrackingState[a] = skeleton.Joints[(JointType)a].TrackingState;
            }
            tSkeleton.TimePeriod = TimePeriod;
            //tSkeleton.AbsoluteTime = AbsoluteTime;
            return tSkeleton;
        }
    }
}
