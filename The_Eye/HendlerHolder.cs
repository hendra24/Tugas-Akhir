using Microsoft.Kinect;
using org.GDL;
using org.GDLStudio;
using org.TKinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class HendlerHolder
    {
        public static MainWindow MainWindow = null;
        public static ArrayList PlayerWindows = new ArrayList();
        public static ArrayList PlayerWindows3D = new ArrayList();

        public static int DisplayWidth = 0;
        public static int DisplayHeight = 0;
        public static bool FileDroppingEnabled = true;
        public static String ApplicationName = "GDL Studio for Kinect 2";
        public static RecognitionCounter RecognitionCounter = null;


        public static void DisplayMessage(Window owner, String message)
        {
            MessageBoxResult result =
                  MessageBox.Show(owner,
                    message,
                    HendlerHolder.ApplicationName,
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }

        private static PlayerWindow skeletonPlayerWindow = null;
        public static PlayerWindow SkeletonPlayerWindow
        {
            get { return skeletonPlayerWindow; }
            set
            {
                if (skeletonPlayerWindow != null)
                    PlayerWindows.Remove(skeletonPlayerWindow);
                if (value != null)
                {
                    PlayerWindows.Add(value);
                    value.Title = "Skeleton stream output";
                }
                skeletonPlayerWindow = value;
            }
        }
        private static PlayerWindow depthPlayerWindow = null;
        public static PlayerWindow DepthPlayerWindow
        {
            get { return depthPlayerWindow; }
            set
            {
                if (depthPlayerWindow != null)
                    PlayerWindows.Remove(depthPlayerWindow);
                if (value != null)
                {
                    PlayerWindows.Add(value);
                    value.Title = "Depth stream output";
                }
                depthPlayerWindow = value;
            }
        }
        private static PlayerWindow rgbPlayerWindow = null;
        public static PlayerWindow RGBPlayerWindow
        {
            get { return rgbPlayerWindow; }
            set
            {
                if (rgbPlayerWindow != null)
                    PlayerWindows.Remove(rgbPlayerWindow);
                if (value != null)
                {
                    PlayerWindows.Add(value);
                    value.Title = "RGB stream output";
                }
                rgbPlayerWindow = value;
            }
        }

        private static PlayerWindow infraredPlayerWindow = null;
        public static PlayerWindow InfraredPlayerWindow
        {
            get { return infraredPlayerWindow; }
            set
            {
                if (infraredPlayerWindow != null)
                    PlayerWindows.Remove(infraredPlayerWindow);
                if (value != null)
                {
                    PlayerWindows.Add(value);
                    value.Title = "Infrared stream output";
                }
                infraredPlayerWindow = value;
            }
        }

        public static Point3D[] GenerateBodyPartArray(TSkeleton skeleton, int user)
        {
            Point3D[] returnArray = new Point3D[skeleton.Position.Length];
            if (skeleton == null) return null;
            for (int a = 0; a < returnArray.Length; a++)
            {
                returnArray[a] = new Point3D(0, 0, 0);
                try
                {
                    returnArray[a].X = skeleton.Position[a].X * 1000.0f;
                    returnArray[a].Y = skeleton.Position[a].Y * 1000.0f;
                    returnArray[a].Z = skeleton.Position[a].Z * 1000.0f;
                }
                catch
                {

                }
            }

            return returnArray;
        }

        public static Point3D[] GenerateBodyPartArray(Body skeleton, int user)
        {
            Point3D[] returnArray = new Point3D[skeleton.Joints.Count];
            if (skeleton == null) return null;
            for (int a = 0; a < returnArray.Length; a++)
            {
                returnArray[a] = new Point3D(0, 0, 0);
                try
                {
                    returnArray[a].X = skeleton.Joints[(JointType)a].Position.X * 1000.0f;
                    returnArray[a].Y = skeleton.Joints[(JointType)a].Position.Y * 1000.0f;
                    returnArray[a].Z = skeleton.Joints[(JointType)a].Position.Z * 1000.0f;
                }
                catch
                {

                }
            }

            return returnArray;
        }

        public static bool IsKinectPlayerWindow(PlayerWindow Window)
        {
            if (Window == HendlerHolder.DepthPlayerWindow || Window == HendlerHolder.RGBPlayerWindow || Window == HendlerHolder.SkeletonPlayerWindow || Window == HendlerHolder.InfraredPlayerWindow)
                return true;
            return false;
        }
        /*
        public static PlayerWindow OpenNewPlayerWindow(bool Show = true)
        {
            PlayerWindow playerWindow = new PlayerWindow();
            PlayerWindows.Add(playerWindow);
            if (Show)
            {
                playerWindow.Show();
            }
            return playerWindow;
        }*/
        public static RecognitionCounter OpenRecognitionCounter()
        {
            if (RecognitionCounter == null)
            {
                RecognitionCounter = new RecognitionCounter();
                RecognitionCounter.Show();
            }
            return RecognitionCounter;


        }


        public static PlayerWindow3D OpenNewPlayerWindow3D(ArrayList recording, bool Show = true)
        {
            PlayerWindow3D playerWindow = new PlayerWindow3D(recording);
            
            PlayerWindows3D.Add(playerWindow);
            if (Show)
            {
                playerWindow.Show();
            }
            return playerWindow;
        }

        public static PlayerWindow OpenNewPlayerWindow(bool Show = true, String SKLFileName = null)
        {
            PlayerWindow playerWindow = new PlayerWindow(SKLFileName);
            if (SKLFileName != null)
            {
                playerWindow.Title = SKLFileName;
            }
            PlayerWindows.Add(playerWindow);
            if (Show)
            {
                playerWindow.Show();
            }
            return playerWindow;
        }
        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        public static List<Pen> BodyColors;
        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        public static double ClipBoundsThickness = 10;
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        public static double HandSize = 30;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        public static double JointThickness = 3;

        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        public static float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as closed
        /// </summary>
        public static readonly Brush HandClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as opened
        /// </summary>
        public static readonly Brush HandOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        /// <summary>
        /// Brush used for drawing hands that are currently tracked as in lasso (pointer) position
        /// </summary>
        public static readonly Brush HandLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        public static readonly Brush TrackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        public static readonly Brush InferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        public static readonly Pen InferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public static KinectSensor kinectSensor = null;
        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        public static CoordinateMapper coordinateMapper = null;

        public static int[][]ArrayJointsConnectivity = null;
        public static void FillJointsConnectivity()
        {
            ArrayJointsConnectivity = new int[25][];
            for (int a = 0; a < ArrayJointsConnectivity.Length; a++)
            {
                ArrayJointsConnectivity[a] = new int[2];
            }
            //RIGHT FOOT
            ArrayJointsConnectivity[0][0] = (int)JointType.SpineBase;
            ArrayJointsConnectivity[0][1] = (int)JointType.HipRight;

            ArrayJointsConnectivity[1][0] = (int)JointType.HipRight;
            ArrayJointsConnectivity[1][1] = (int)JointType.KneeRight;

            ArrayJointsConnectivity[2][0] = (int)JointType.KneeRight;
            ArrayJointsConnectivity[2][1] = (int)JointType.AnkleRight;

            ArrayJointsConnectivity[3][0] = (int)JointType.AnkleRight;
            ArrayJointsConnectivity[3][1] = (int)JointType.FootRight;
            //LEFT FOOT
            ArrayJointsConnectivity[4][0] = (int)JointType.SpineBase;
            ArrayJointsConnectivity[4][1] = (int)JointType.HipLeft;

            ArrayJointsConnectivity[5][0] = (int)JointType.HipLeft;
            ArrayJointsConnectivity[5][1] = (int)JointType.KneeLeft;

            ArrayJointsConnectivity[6][0] = (int)JointType.KneeLeft;
            ArrayJointsConnectivity[6][1] = (int)JointType.AnkleLeft;

            ArrayJointsConnectivity[7][0] = (int)JointType.AnkleLeft;
            ArrayJointsConnectivity[7][1] = (int)JointType.FootLeft;
            //SPINE
            ArrayJointsConnectivity[8][0] = (int)JointType.SpineBase;
            ArrayJointsConnectivity[8][1] = (int)JointType.SpineMid;

            ArrayJointsConnectivity[9][0] = (int)JointType.SpineMid;
            ArrayJointsConnectivity[9][1] = (int)JointType.SpineShoulder;

            ArrayJointsConnectivity[10][0] = (int)JointType.SpineShoulder;
            ArrayJointsConnectivity[10][1] = (int)JointType.Neck;

            ArrayJointsConnectivity[11][0] = (int)JointType.Neck;
            ArrayJointsConnectivity[11][1] = (int)JointType.Head;
            //RIGHT HAND
            ArrayJointsConnectivity[12][0] = (int)JointType.SpineShoulder;
            ArrayJointsConnectivity[12][1] = (int)JointType.ShoulderRight;

            ArrayJointsConnectivity[13][0] = (int)JointType.ShoulderRight;
            ArrayJointsConnectivity[13][1] = (int)JointType.ElbowRight;

            ArrayJointsConnectivity[14][0] = (int)JointType.ElbowRight;
            ArrayJointsConnectivity[14][1] = (int)JointType.WristRight;

            ArrayJointsConnectivity[15][0] = (int)JointType.WristRight;
            ArrayJointsConnectivity[15][1] = (int)JointType.HandRight;

            ArrayJointsConnectivity[16][0] = (int)JointType.HandRight;
            ArrayJointsConnectivity[16][1] = (int)JointType.ThumbRight;

            ArrayJointsConnectivity[17][0] = (int)JointType.HandRight;
            ArrayJointsConnectivity[17][1] = (int)JointType.HandTipRight;
            
            //LEFT HAND
            ArrayJointsConnectivity[18][0] = (int)JointType.SpineShoulder;
            ArrayJointsConnectivity[18][1] = (int)JointType.ShoulderLeft;

            ArrayJointsConnectivity[19][0] = (int)JointType.ShoulderLeft;
            ArrayJointsConnectivity[19][1] = (int)JointType.ElbowLeft;

            ArrayJointsConnectivity[20][0] = (int)JointType.ElbowLeft;
            ArrayJointsConnectivity[20][1] = (int)JointType.WristLeft;

            ArrayJointsConnectivity[21][0] = (int)JointType.WristLeft;
            ArrayJointsConnectivity[21][1] = (int)JointType.HandLeft;

            ArrayJointsConnectivity[22][0] = (int)JointType.HandLeft;
            ArrayJointsConnectivity[22][1] = (int)JointType.ThumbLeft;

            ArrayJointsConnectivity[23][0] = (int)JointType.HandLeft;
            ArrayJointsConnectivity[23][1] = (int)JointType.HandTipLeft;
        }
       
        //An RGB color camera – 640×480 in version 1, 1920×1080 in version 2
        //A depth sensor – 320×240 in v1, 512×424 in v2
        //An infrared sensor – 512×424 in v2
        public static DepthSpacePoint MapSkeletonPointToDepthPointOffline(CameraSpacePoint skeletonPoint)
        {
            DepthSpacePoint depthImagePoint = new DepthSpacePoint();
            double pfDepthX = 0;
            double pfDepthY = 0;
            if ((double)skeletonPoint.Z > 1.40129846432482E-45)
            {
                pfDepthX = (float)(0.5 + (double)skeletonPoint.X * 285.630004882813 / ((double)skeletonPoint.Z * 512.0));
                pfDepthY = (float)(0.5 - (double)skeletonPoint.Y * 285.630004882813 / ((double)skeletonPoint.Z * 424.0));
            }
            depthImagePoint.X = (int)(pfDepthX * 512.0 + 0.5);
            depthImagePoint.Y = (int)(pfDepthY * 424.0 + 0.5);
            return depthImagePoint;
        }
    }
}
