using Microsoft.Kinect;
using org.GDL;
using org.GDLStudio;
using org.TKinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {
        public PlayerWindow(bool IsKinectPlayerWindow = true)
        {
            InitializeComponent();
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);
            if (IsKinectPlayerWindow)
            {
                PlayButton.Visibility = Visibility.Hidden;
                StartTextBox.Visibility = Visibility.Hidden;
                StopTextBox.Visibility = Visibility.Hidden;
                SaveFragButton.Visibility = Visibility.Hidden;
                Show3D.Visibility = Visibility.Hidden;
            }
        }

        public void UpdateGDLOutput(String allConclusiosn, String conclusionsWithExclamation)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    if (this.ShowSubrulesCheckbox.IsChecked == true)
                        this.GDLOutput.Text = allConclusiosn;
                    else
                        this.GDLOutput.Text = conclusionsWithExclamation;
                }));
        }

        public PlayerWindow(String SKLFileName) : this(false)
        {
            if (SKLFileName != null)
            {
                LoadSKLRecording(SKLFileName);
                ChangeContentWhileRecordingOrPlaying(false);

                RecordButton.Visibility = Visibility.Hidden;

            }
        }

        private bool recording = false;
        private String sKLFileName = null;
        private StreamWriter sKLFileStream = null;
        public long TimeFromStartOfCapture = -1;
        public ArrayList SKLRecording = null;
        public Stopwatch CaptureStopwatch = new Stopwatch();
        
        public int FrameCount = 0;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        public bool IsPlaying = false;

        void ChangeContentWhileRecordingOrPlaying(bool playOrRecord)
        {
            RecordButton.IsEnabled = !playOrRecord;
            PlayButton.IsEnabled = !playOrRecord;
            StopButton.IsEnabled = playOrRecord;
            ProgressSlider.IsEnabled = !playOrRecord;
        }

        private void SkeletonOutput_Drop(object sender, DragEventArgs e)
        {
            if (IsPlaying == true || recording == true || HendlerHolder.FileDroppingEnabled == false)
            {
                string msg = "File dropping has been disabled either by internal processing (like recording or playing) or by the user in main menu.";
                HendlerHolder.DisplayMessage(this, msg);
                return;
            }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int a = 0; a < files.Length; a++)
                {
                    if (System.IO.Path.GetExtension(files[a]).ToLower() == ".skl")
                    {
                        if (HendlerHolder.IsKinectPlayerWindow(this))
                        {
                            HendlerHolder.OpenNewPlayerWindow(true, files[a]);
                        }
                        else
                        {
                            LoadSKLRecording(files[0]);
                            Title = files[0];
                        }
                    }
                }
                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                
                /*Dispatcher.Invoke(
                    new Action(
                    () =>
                    {
                        this.parentMainWindow.LoadSkeletonFile(files[0]);
                    }));*/
            }

        }

        public void PlayRecording()
        {
            if (SKLRecording == null)
                return;
            if (SKLRecording.Count == 0)
                return;
            RecognitionCounter recognitionCounter = HendlerHolder.OpenRecognitionCounter();
            IsPlaying = true;
            ChangeContentWhileRecordingOrPlaying(true);
            Thread ReaderThread = new Thread(PlayerThread);
            ReaderThread.IsBackground = true;
            //ReadFile(fileToPlay);
            //line = 0;
            ReaderThread.Start();
        }

        private void PlayerThread()
        {
            int line = 0;
            GDLInterpreter inter = null;
            RecognitionCounter recognitionCounter = HendlerHolder.OpenRecognitionCounter();
            Dispatcher.Invoke(
                    new Action(
                    () =>
                    {
                        line = (int)ProgressSlider.Value;
                        if (HendlerHolder.MainWindow.inter != null)
                            inter = new GDLInterpreter(HendlerHolder.MainWindow.inter.FeatureTable, HendlerHolder.MainWindow.inter.RuleTable);
                    }));
                        TSkeleton[] TSkeletons = null;
                        while (line < SKLRecording.Count && IsPlaying)
                        {
                            TSkeletons = (TSkeleton[])SKLRecording[line];

                            if (inter != null)
                            {
                                double seconds = (double)TSkeletons[0].TimePeriod / 1000.0;
                                Point3D[] bodyParts = HendlerHolder.GenerateBodyPartArray(TSkeletons[0], 0);
                                //String[] con = inter.ReturnConclusions(fT, 0, seconds);
                                //String[] con = inter.ReturnConclusions(bodyParts, 0, seconds);
                                String[] con = inter.ReturnConclusions(bodyParts, seconds);
                                String allConclusions = "";
                                String conclusionsWithExclamation = "";
                                for (int a = 0; a < con.Length; a++)
                                {
                                    allConclusions += con[a] + "\r\n";
                                    if (con[a].Contains("!"))
                                    {
                                        conclusionsWithExclamation += con[a] + "\r\n";
                                    }
                                    Dispatcher.Invoke(
                                    new Action(
                                    () =>
                                    {
                                        recognitionCounter.AddToDictionary(con[a]);
                                    }));
                                }
                                UpdateGDLOutput(allConclusions, conclusionsWithExclamation);
                            }


                            Dispatcher.Invoke(new Action(
                    () =>
                    {
                            ProgressSlider.Value += 1;
                            //DrawImage(TSkeletons);
                        }));
                            Thread.Sleep((int)TSkeletons[0].TimePeriod);
                            line++;
                        }
                        Dispatcher.Invoke(new Action(
                                () =>
                                {
                                    StopPlaying();
                                }));
        }
        public void BeginCapture()
        {
            String dirName = System.AppDomain.CurrentDomain.BaseDirectory;
            //String dirName = Path.GetDirectoryName(Application.Current.ExecutablePath);
            String fileNameSkeleton = "SkeletonRecord";
            int imageIndex = 0;
            CaptureStopwatch.Reset();
            CaptureStopwatch.Start();
            while (File.Exists(dirName + "/" + fileNameSkeleton + imageIndex + ".skl"))
            {
                imageIndex++;
            }
            TimeFromStartOfCapture = -1;
            sKLFileName = dirName + "/" + fileNameSkeleton + imageIndex + ".skl";
            sKLFileStream = new StreamWriter(sKLFileName);
            RecordButton.IsEnabled = false;
            ChangeContentWhileRecordingOrPlaying(true);
            recording = true;
        }

        public void StopPlaying()
        {
            ChangeContentWhileRecordingOrPlaying(false);
            IsPlaying = false;
        }

        public void StopCapture()
        {
            recording = false;
            if (sKLFileStream == null)
                return;
            CaptureStopwatch.Stop();
            if (sKLFileStream != null)
                sKLFileStream.Close();
            sKLFileStream = null;
            if (SKLRecording != null)
                SKLRecording.Clear();
            HendlerHolder.OpenNewPlayerWindow(true, SKLFileName);
            SKLFileName = null;
            ChangeContentWhileRecordingOrPlaying(false);
            //SKLRecording = TSkeletonHelper.ReadRecordingFromFile(SKLFileName);
            //LoadSKLRecording(SKLFileName);
            //ChangeContentWhileRecordingOrPlaying(false);
            /*if (SKLRecording != null)
            {
                ProgressSlider.Maximum = SKLRecording.Count -1;
                ProgressSlider.Value = ProgressSlider.Maximum;
                ProgressSlider.Value = 0;
                if (SKLRecording.Count > 0)
                    DrawImage((TSkeleton[])SKLRecording[0]);
            }
            else
            {
                ProgressSlider.Maximum = 0;
                ProgressSlider.Value = 0;
                DrawImage(null);
            }*/
        }

        public void LoadSKLRecording(String SKLFile)
        {
            ArrayList SKLRecordingHelp = null;
            try
            {
                SKLRecordingHelp = TSkeletonHelper.ReadRecordingFromFile(SKLFile);
            }
            catch (Exception e)
            {
                String msg = "SKL file " + SKLFile + " loading error: " + e.Message;
                HendlerHolder.DisplayMessage(this, msg);
                return;
            }
            if (SKLRecordingHelp == null)
            {
                String msg = "SKL file " + SKLFile + " seems to be empty.";
                HendlerHolder.DisplayMessage(this, msg);
                return;
            }
            sKLFileName = SKLFile;
            SKLRecording = SKLRecordingHelp;
            if (SKLRecording != null)
            {
                ProgressSlider.Maximum = SKLRecording.Count - 1;
                ProgressSlider.Value = ProgressSlider.Maximum;
                ProgressSlider.Value = 0;
                if (SKLRecording.Count > 0)
                    DrawImage((TSkeleton[])SKLRecording[0]);
            }
            else
            {
                ProgressSlider.Maximum = 0;
                ProgressSlider.Value = 0;
                DrawImage(null);
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SKLRecording != null)
            {
                if (ProgressSlider.Value < SKLRecording.Count)
                {
                    TSkeleton[] tSkeletons = (TSkeleton[])SKLRecording[(int)ProgressSlider.Value];
                    DrawImage(tSkeletons);
                    UpdateImage(this.imageSource);
                    StopTextBox.Text = ((int)e.NewValue).ToString();
                }
            }
        }

        private void DrawClippedEdges(TSkeleton body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, HendlerHolder.DisplayHeight - HendlerHolder.ClipBoundsThickness, HendlerHolder.DisplayWidth, HendlerHolder.ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, HendlerHolder.DisplayWidth, HendlerHolder.ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, HendlerHolder.ClipBoundsThickness, HendlerHolder.DisplayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(HendlerHolder.DisplayWidth - HendlerHolder.ClipBoundsThickness, 0, HendlerHolder.ClipBoundsThickness, HendlerHolder.DisplayHeight));
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(TSkeleton tSkeleton, Point[] updatedPositions, DrawingContext drawingContext, Pen drawingPen)
        //private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            //foreach (var bone in this.bones)
            for (int a = 0; a < HendlerHolder.ArrayJointsConnectivity.Length; a++)
            {
                this.DrawBone(tSkeleton, updatedPositions, HendlerHolder.ArrayJointsConnectivity[a][0], HendlerHolder.ArrayJointsConnectivity[a][1], drawingContext, drawingPen);
            }

            // Draw the joints
            for (int a = 0; a < tSkeleton.Position.Length; a++)
            {
                Brush drawBrush = null;

                TrackingState trackingState = tSkeleton.TrackingState[a];

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = HendlerHolder.TrackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = HendlerHolder.InferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, updatedPositions[a], HendlerHolder.JointThickness, HendlerHolder.JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        //private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        private void DrawBone(TSkeleton tSkeleton, Point[] jointPoints, int jointType0, int jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            //Joint joint0 = joints[jointType0];
            //Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (tSkeleton.TrackingState[jointType0] == TrackingState.NotTracked ||
                tSkeleton.TrackingState[jointType1] == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = HendlerHolder.InferredBonePen;
            if ((tSkeleton.TrackingState[jointType0] == TrackingState.Tracked) && (tSkeleton.TrackingState[jointType1] == TrackingState.Tracked))
            {
                drawPen = drawingPen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(HendlerHolder.HandClosedBrush, null, handPosition, HendlerHolder.HandSize, HendlerHolder.HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(HendlerHolder.HandOpenBrush, null, handPosition, HendlerHolder.HandSize, HendlerHolder.HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(HendlerHolder.HandLassoBrush, null, handPosition, HendlerHolder.HandSize, HendlerHolder.HandSize);
                    break;
            }
        }

        private void DrawImage(TSkeleton[]tSkeletons)
        {
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, HendlerHolder.DisplayWidth, HendlerHolder.DisplayHeight));
                if (tSkeletons == null)
                    return;
                int penIndex = 0;
                //foreach (Body body in this.bodies)
                for (int a = 0; a < tSkeletons.Length; a++)
                {
                    TSkeleton tSkeleton = tSkeletons[a];
                    Pen drawPen = HendlerHolder.BodyColors[penIndex++];

                    if (tSkeleton.IsTracked)
                    {
                        this.DrawClippedEdges(tSkeleton, dc);

                        //IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                        // convert the joint points to depth (display) space
                        //Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                        //foreach (JointType jointType in joints.Keys)
                        Point[] updatedPositions = new Point[tSkeleton.Position.Length];
                        for (int b = 0; b < tSkeleton.Position.Length; b++)
                        {
                            // sometimes the depth(Z) of an inferred joint may show as negative
                            // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                            updatedPositions[b] = new Point();
                            CameraSpacePoint updatedCSP = new CameraSpacePoint();
                            updatedCSP.X = tSkeleton.Position[b].X;
                            updatedCSP.Y = tSkeleton.Position[b].Y;
                            updatedCSP.Z = tSkeleton.Position[b].Z;
                            if (updatedCSP.Z < 0)
                            {
                               updatedCSP.Z = HendlerHolder.InferredZPositionClamp;
                            }

                            DepthSpacePoint depthSpacePoint = HendlerHolder.MapSkeletonPointToDepthPointOffline(updatedCSP);
                            //DepthSpacePoint depthSpacePoint = HendlerHolder.coordinateMapper.MapCameraPointToDepthSpace(updatedCSP);
                            updatedPositions[b] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            //jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                        }

                        this.DrawBody(tSkeleton, updatedPositions, dc, drawPen);

                        this.DrawHand(tSkeleton.HandLeftState, updatedPositions[(int)JointType.HandLeft], dc);
                        this.DrawHand(tSkeleton.HandRightState, updatedPositions[(int)JointType.HandRight], dc);
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, HendlerHolder.DisplayWidth, HendlerHolder.DisplayHeight));
            }
        }

        public void UpdateImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.SkeletonImage.Source = source;
                }));
        }
        /*
        public void UpdateGDLOutput(String text)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.GDLOutput.Text = text;
                }));
        }*/

        public StreamWriter SKLFileStream
        {
            get
            {
                return sKLFileStream;
            }
            set
            {
                sKLFileStream = value;
            }
        }
        public String SKLFileName
        {
            get
            {
                return sKLFileName;
            }
            set
            {
                sKLFileName = value;
            }
        }
        public bool Recording
        {
            get
                {
                    return recording;
                }
            set
                {
                    recording = value;
                    if (recording)
                    {
                        //if ()
                    }
                }
        }
        void PlayerMainWindow_Closing(object sender, CancelEventArgs e)
        {
            StopPlaying();
            StopCapture();
            //MessageBox.Show("Closing called");

            // If data is dirty, notify user and ask for a response 
            /*if (this.isDataDirty)
            {
                string msg = "Data is dirty. Close without saving?";
                MessageBoxResult result = 
                  MessageBox.Show(
                    msg, 
                    "Data App", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // If user doesn't want to close, cancel closure
                    e.Cancel = true;
                }
            }*/
            HendlerHolder.PlayerWindows.Remove(this);
            if (this == HendlerHolder.SkeletonPlayerWindow)
                HendlerHolder.SkeletonPlayerWindow = null;
            if (this == HendlerHolder.DepthPlayerWindow)
                HendlerHolder.DepthPlayerWindow = null;
            if (this == HendlerHolder.RGBPlayerWindow)
                HendlerHolder.RGBPlayerWindow = null;
            if (this == HendlerHolder.InfraredPlayerWindow)
                HendlerHolder.InfraredPlayerWindow = null;
        }
        private void RecordButtn_Click(object sender, RoutedEventArgs e)
        {
            if (HendlerHolder.IsKinectPlayerWindow(this))
                BeginCapture();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (recording)
            {
                StopCapture();
                return;
            }
            if (IsPlaying)
            {
                StopPlaying();
            }
        }

        private void PlayButton_Click_1(object sender, RoutedEventArgs e)
        {
            PlayRecording();
        }

        private void Show3D_Click(object sender, RoutedEventArgs e)
        {
            if (SKLRecording == null)
                return;
            if (SKLRecording.Count < 1)
                return;
            //PlayerWindow3D pw3D = new PlayerWindow3D(TSkeletonHelper.CloneRecording(SKLRecording));
            PlayerWindow3D pw3D = HendlerHolder.OpenNewPlayerWindow3D(TSkeletonHelper.CloneRecording(SKLRecording));
            pw3D.Title = this.Title;
            //pw3D.Show();
        }

        private void SaveFragButton_Click(object sender, RoutedEventArgs e)
        {
            if (HendlerHolder.IsKinectPlayerWindow(this))
                return;
            if (SKLRecording == null)
            {
                MessageBox.Show(this, "No recording was loaded.", "Error saving recording", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SKLRecording.Count == 0)
            {
                MessageBox.Show(this, "No recording was loaded.", "Error saving recording", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int start = 0;
            int stop = 0;
            try
            {
                start = int.Parse(StartTextBox.Text);
                stop = int.Parse(StopTextBox.Text);
            }
            catch
            {
                MessageBox.Show(this, "Error in parsing range.", "Error saving recording", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (start < 0 || start >= SKLRecording.Count ||
                stop < 0 || stop >= SKLRecording.Count ||
                start > stop)
            {
                MessageBox.Show(this, "Recording ranges out of range.", "Error saving recording", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            String dirName = System.AppDomain.CurrentDomain.BaseDirectory;
            String fileNameSkeleton = "SkeletonRecord";
            int imageIndex = 0;
            while (File.Exists(dirName + "/" + fileNameSkeleton + imageIndex + ".skl"))
            {
                imageIndex++;
            }
            StreamWriter sw = new StreamWriter(dirName + "/" + fileNameSkeleton + imageIndex + ".skl");
            long currentFrame = 0;
            for (int a = start; a <= stop; a++)
            {
                TSkeleton[] ts = (TSkeleton[])SKLRecording[a];
                for (int b = 0; b < ts.Length; b++)
                {
                    TSkeletonHelper.SaveTSkeletonToFile(ts[b], sw, currentFrame);
                }
                currentFrame++;
            }
            sw.Close();
            MessageBox.Show(this, "File saved successfully under path:\r\n" + dirName + "/" + fileNameSkeleton + imageIndex + ".skl", "Saving", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}
