//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using org.GDL;
    using org.TKinect;
    using System.Collections;
    using ICSharpCode.AvalonEdit.Highlighting;
    using System.Xml;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using System.Threading;
    using System.Windows.Input;
    using org.GDLStudio;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Dictionary<string, int> dictionary;
        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup skeletonDrawingGroup;
        private DrawingGroup depthDrawingGroup;
        private DrawingGroup rgbDrawingGroup;
        private DrawingGroup infraredDrawingGroup;
        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage skeletonImageSource;
        private DrawingImage depthImageSource;
        private DrawingImage rgbImageSource;
        private DrawingImage infraredImageSource;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// Width of display (depth space)
        /// </summary>
        private int displayWidth;

        /// <summary>
        /// Height of display (depth space)
        /// </summary>
        private int displayHeight;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader = null;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthBitmap = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap colorBitmap = null;

        /// <summary>
        /// Reader for color frames
        /// </summary>
        private ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] depthPixels = null;

        /// <summary>
        /// Intermediate storage for the depth data converted to color
        /// </summary>
        private byte[] colorPixelsDepth;
        byte[] RLUT = null;
        byte[] GLUT = null;
        byte[] BLUT = null;
        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;

        /// <summary>
        /// Reader for infrared frames
        /// </summary>
        private InfraredFrameReader infraredFrameReader = null;

        /// <summary>
        /// Description (width, height, etc) of the infrared frame data
        /// </summary>
        private FrameDescription infraredFrameDescription = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap infraredBitmap = null;

        /// <summary>
        /// Maximum value (as a float) that can be returned by the InfraredFrame
        /// </summary>
        private const float InfraredSourceValueMaximum = (float)ushort.MaxValue;

        /// <summary>
        /// The value by which the infrared source data will be scaled
        /// </summary>
        private const float InfraredSourceScale = 0.75f;

        /// <summary>
        /// Smallest value to display when the infrared data is normalized
        /// </summary>
        private const float InfraredOutputValueMinimum = 0.01f;

        /// <summary>
        /// Largest value to display when the infrared data is normalized
        /// </summary>
        private const float InfraredOutputValueMaximum = 1.0f;

        /// <summary>
        /// GDL
        /// </summary>
        Stopwatch GDLWatch = new Stopwatch();
        Stopwatch FPSStopwatch = new Stopwatch();
        public GDLInterpreter inter = null;
        ////////
        private static double AcquisitionFrequencyTreshold = 0.01;
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // one sensor is currently supported
            HendlerHolder.kinectSensor = KinectSensor.GetDefault();

            // get the coordinate mapper
            HendlerHolder.coordinateMapper = HendlerHolder.kinectSensor.CoordinateMapper;
            CameraIntrinsics ci = HendlerHolder.coordinateMapper.GetDepthCameraIntrinsics();
            // get the depth (display) extents
            FrameDescription frameDescription = HendlerHolder.kinectSensor.DepthFrameSource.FrameDescription;

            // get size of joint space
            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            // open the reader for the body frames
            this.bodyFrameReader = HendlerHolder.kinectSensor.BodyFrameSource.OpenReader();
            // open the reader for the depth frames
            this.depthFrameReader = HendlerHolder.kinectSensor.DepthFrameSource.OpenReader();
            // get FrameDescription from DepthFrameSource
            this.depthFrameDescription = HendlerHolder.kinectSensor.DepthFrameSource.FrameDescription;

            // allocate space to put the pixels being received and converted
            this.depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];

            // create the bitmap to display
            this.depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            // Allocate space to put the color pixels we'll create
            this.colorPixelsDepth = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height * sizeof(int)];

            // open the reader for the color frames
            this.colorFrameReader = HendlerHolder.kinectSensor.ColorFrameSource.OpenReader();
            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = HendlerHolder.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);


            // open the reader for the depth frames
            this.infraredFrameReader = HendlerHolder.kinectSensor.InfraredFrameSource.OpenReader();

            

            // get FrameDescription from InfraredFrameSource
            this.infraredFrameDescription = HendlerHolder.kinectSensor.InfraredFrameSource.FrameDescription;

            // create the bitmap to display
            this.infraredBitmap = new WriteableBitmap(this.infraredFrameDescription.Width, this.infraredFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray32Float, null);


            // Set the image we display to point to the bitmap where we'll put the image data
            

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));

            

            // set IsAvailableChanged event notifier
            HendlerHolder.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            HendlerHolder.kinectSensor.Open();

            // set the status text
            this.StatusText = HendlerHolder.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // Create the drawing group we'll use for drawing
            this.skeletonDrawingGroup = new DrawingGroup();
            this.rgbDrawingGroup = new DrawingGroup();
            this.depthDrawingGroup = new DrawingGroup();
            this.infraredDrawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.skeletonImageSource = new DrawingImage(this.skeletonDrawingGroup);
            this.rgbImageSource = new DrawingImage(this.rgbDrawingGroup);
            this.depthImageSource = new DrawingImage(this.depthDrawingGroup);
            this.infraredImageSource = new DrawingImage(this.infraredDrawingGroup);

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            HendlerHolder.MainWindow = this;
            //HendlerHolder.OpenNewPlayerWindow();
            HendlerHolder.DisplayWidth = frameDescription.Width;
            HendlerHolder.DisplayHeight = frameDescription.Height;
            // populate body colors, one for each BodyIndex
            HendlerHolder.BodyColors = new List<Pen>();

            HendlerHolder.BodyColors.Add(new Pen(Brushes.Red, 6));
            HendlerHolder.BodyColors.Add(new Pen(Brushes.Orange, 6));
            HendlerHolder.BodyColors.Add(new Pen(Brushes.Green, 6));
            HendlerHolder.BodyColors.Add(new Pen(Brushes.Blue, 6));
            HendlerHolder.BodyColors.Add(new Pen(Brushes.Indigo, 6));
            HendlerHolder.BodyColors.Add(new Pen(Brushes.Violet, 6));
            HendlerHolder.FillJointsConnectivity();

            //text editor setup
            //textEditor.SyntaxHighlighting = LoadHighlightingDefinition("GDLscript.xshd");
            //textEditor.TextArea.Caret.PositionChanged += this.CaretChangedEvent;
            //textEditor.TextChanged += this.TextChangedEventHandler;

            //Title = getApplicationName();
            //textEditor.ShowLineNumbers = true;
            FPSStopwatch.Start();

            //this.Title = HendlerHolder.ApplicationName;
        }

        public static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            var type = typeof(MainWindow);
            var fullName = type.Namespace + "." + resourceName;
            using (var stream = type.Assembly.GetManifestResourceStream(fullName))
            using (var reader = new XmlTextReader(stream))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }


        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.ReaderBody_FrameArrived;
                GDLWatch.Start();
                // wire handler for frame arrival
                //this.depthFrameReader.FrameArrived += this.ReaderDepth_FrameArrived;

                RLUT = new byte[6000];
                GLUT = new byte[6000];
                BLUT = new byte[6000];
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(System.AppDomain.CurrentDomain.BaseDirectory + "alut.tif");
                System.Drawing.Color color;
                for (int a = 0; a < RLUT.Length; a++)
                {
                    int index = (int)((double)a * ((double)bmp.Height / (double)RLUT.Length));
                    color = bmp.GetPixel(0, index);
                    RLUT[a] = color.R;
                    GLUT[a] = color.G;
                    BLUT[a] = color.B;
                }
            }
            this.DepthImage.Source = this.depthBitmap;
            this.RGBImage.Source = this.colorBitmap;
            this.InfraredImage.Source = this.infraredBitmap;
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            int count = HendlerHolder.PlayerWindows.Count -1;
            for (int a = count; a >= 0; a--)
            {
                PlayerWindow playerWindow = (PlayerWindow)HendlerHolder.PlayerWindows[a];
                playerWindow.Close();
            }

            count = HendlerHolder.PlayerWindows3D.Count - 1;
            for (int a = count; a >= 0; a--)
            {
                PlayerWindow3D playerWindow = (PlayerWindow3D)HendlerHolder.PlayerWindows3D[a];
                playerWindow.Close();
            }
            
            if (this.bodyFrameReader != null)
            {
                // BodyFrameReader is IDisposable
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (HendlerHolder.kinectSensor != null)
            {
                HendlerHolder.kinectSensor.Close();
                HendlerHolder.kinectSensor = null;
            }
            if (HendlerHolder.RecognitionCounter != null)
            {
                HendlerHolder.RecognitionCounter.Close();
                HendlerHolder.RecognitionCounter = null;
            }
        }

        

        public void UpdateRGBImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.RGBImage.Source = source;
                }));
        }

        public void UpdateSkeletonImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.SkeletonImage.Source = source;
                }));
        }

        public void UpdateDepthImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.DepthImage.Source = source;
                }));
        }

        public void UpdateInfraredImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.InfraredImage.Source = source;
                }));
        }

        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ReaderDepth_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            bool depthFrameProcessed = false;

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    // the fastest way to process the body index data is to directly access 
                    // the underlying buffer
                    using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        // verify data and write the color data to the display bitmap
                        if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / this.depthFrameDescription.BytesPerPixel)) &&
                            (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                        {
                            // Note: In order to see the full range of depth (including the less reliable far field depth)
                            // we are setting maxDepth to the extreme potential depth threshold
                            ushort maxDepth = ushort.MaxValue;

                            // If you wish to filter by reliable depth distance, uncomment the following line:
                            //// maxDepth = depthFrame.DepthMaxReliableDistance

                            this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, depthFrame.DepthMinReliableDistance, maxDepth);
                            depthFrameProcessed = true;
                        }
                    }
                }
            }

            if (depthFrameProcessed)
            {
                this.RenderDepthPixels();
            }
            counterDepth++;
            if (FPSStopwatch.ElapsedMilliseconds - startTimeDepth > 1000)
            {
                fpsDepth = counterDepth;
                counterDepth = 0;
                startTimeDepth = FPSStopwatch.ElapsedMilliseconds;
                DepthFPS.Text = fpsDepth.ToString() + " fps";
            }
        }

        int counterDepth = 0;
        long startTimeDepth = 0;
        int fpsDepth = 0;

        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MapDepthToByte = 8000 / 256;
        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;
            int colorPixelIndex = 0;
            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort depth = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);

                int depthHelp = depth;
                int r = 0, g = 0, b = 0;
                if (depthHelp >= RLUT.Length)
                    depthHelp = (short)(RLUT.Length - 1);
                if (depthHelp < 0)
                    depthHelp = 0;
                b = RLUT[depthHelp];
                g = GLUT[depthHelp];
                r = BLUT[depthHelp];
                this.colorPixelsDepth[colorPixelIndex++] = (byte)b;
                this.colorPixelsDepth[colorPixelIndex++] = (byte)g;
                this.colorPixelsDepth[colorPixelIndex++] = (byte)r;
                ++colorPixelIndex;
            }
        }


        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.colorPixelsDepth,
                this.depthBitmap.PixelWidth * sizeof(int),
                0);
        }

        /// <summary>
        /// Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // ColorFrame is IDisposable
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the infrared frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_InfraredFrameArrived(object sender, InfraredFrameArrivedEventArgs e)
        {
            // InfraredFrame is IDisposable
            using (InfraredFrame infraredFrame = e.FrameReference.AcquireFrame())
            {
                if (infraredFrame != null)
                {
                    // the fastest way to process the infrared frame data is to directly access 
                    // the underlying buffer
                    using (Microsoft.Kinect.KinectBuffer infraredBuffer = infraredFrame.LockImageBuffer())
                    {
                        // verify data and write the new infrared frame data to the display bitmap
                        if (((this.infraredFrameDescription.Width * this.infraredFrameDescription.Height) == (infraredBuffer.Size / this.infraredFrameDescription.BytesPerPixel)) &&
                            (this.infraredFrameDescription.Width == this.infraredBitmap.PixelWidth) && (this.infraredFrameDescription.Height == this.infraredBitmap.PixelHeight))
                        {
                            this.ProcessInfraredFrameData(infraredBuffer.UnderlyingBuffer, infraredBuffer.Size);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the InfraredFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the infraredFrameData pointer.
        /// </summary>
        /// <param name="infraredFrameData">Pointer to the InfraredFrame image data</param>
        /// <param name="infraredFrameDataSize">Size of the InfraredFrame image data</param>
        private unsafe void ProcessInfraredFrameData(IntPtr infraredFrameData, uint infraredFrameDataSize)
        {
            // infrared frame data is a 16 bit value
            ushort* frameData = (ushort*)infraredFrameData;

            // lock the target bitmap
            this.infraredBitmap.Lock();

            // get the pointer to the bitmap's back buffer
            float* backBuffer = (float*)this.infraredBitmap.BackBuffer;

            // process the infrared data
            for (int i = 0; i < (int)(infraredFrameDataSize / this.infraredFrameDescription.BytesPerPixel); ++i)
            {
                // since we are displaying the image as a normalized grey scale image, we need to convert from
                // the ushort data (as provided by the InfraredFrame) to a value from [InfraredOutputValueMinimum, InfraredOutputValueMaximum]
                backBuffer[i] = Math.Min(InfraredOutputValueMaximum, (((float)frameData[i] / InfraredSourceValueMaximum * InfraredSourceScale) * (1.0f - InfraredOutputValueMinimum)) + InfraredOutputValueMinimum);
            }

            // mark the entire bitmap as needing to be drawn
            this.infraredBitmap.AddDirtyRect(new Int32Rect(0, 0, this.infraredBitmap.PixelWidth, this.infraredBitmap.PixelHeight));

            // unlock the bitmap
            this.infraredBitmap.Unlock();
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ReaderBody_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            HendlerHolder.OpenRecognitionCounter();
            Vector4 FloorClipPlane = new Vector4();
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    FloorClipPlane = bodyFrame.FloorClipPlane;
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived && inter != null && GDLWatch.Elapsed.TotalSeconds > AcquisitionFrequencyTreshold)
            {
                Body firstTracked = null;
                for (int a = 0; a < this.bodies.Length && firstTracked == null; a++)
                {
                    if (((Body)bodies[a]).IsTracked)
                        firstTracked = (Body)bodies[a];
                }
                if (firstTracked != null)
                {
                    Point3D[] bodyParts = HendlerHolder.GenerateBodyPartArray(firstTracked, 0);
                    GDLWatch.Stop();
                    double TimeHelp = GDLWatch.Elapsed.TotalSeconds;
                    //double TimeHelp = (double)GDLWatch.ElapsedMilliseconds / 1000.0;
                    GDLWatch.Reset();
                    GDLWatch.Start();
                    //String[] con = inter.ReturnConclusions(firstTracked, 0, TimeHelp);
                    //String[] con = inter.ReturnConclusions(bodyParts, 0, TimeHelp);
                    String[] con = inter.ReturnConclusions(bodyParts, TimeHelp);
                    //if (HendlerHolder.SkeletonPlayerWindow != null)
                      //  HendlerHolder.SkeletonPlayerWindow.Up
                    //this.ConclusionsLabel.Content = "";
                    String allConclusions = "";
                    String conclusionsWithExclamation = "";
                    for (int a = 0; a < con.Length; a++)
                    {
                        allConclusions += con[a] + "\r\n";
                        if (con[a].Contains("!"))
                        {
                            conclusionsWithExclamation += con[a] + "\r\n";
                        }
                        HendlerHolder.OpenRecognitionCounter().AddToDictionary(con[a]);
                    }
                    
                    if (HendlerHolder.SkeletonPlayerWindow != null)
                    {
                        HendlerHolder.SkeletonPlayerWindow.UpdateGDLOutput(allConclusions, conclusionsWithExclamation);
                    }
                    if (HendlerHolder.DepthPlayerWindow != null)
                    {
                        HendlerHolder.DepthPlayerWindow.UpdateGDLOutput(allConclusions, conclusionsWithExclamation);
                    }
                    if (HendlerHolder.InfraredPlayerWindow != null)
                    {
                        HendlerHolder.InfraredPlayerWindow.UpdateGDLOutput(allConclusions, conclusionsWithExclamation);
                    }
                    if (HendlerHolder.RGBPlayerWindow != null)
                    {
                        HendlerHolder.RGBPlayerWindow.UpdateGDLOutput(allConclusions, conclusionsWithExclamation);
                    }
                }
            }

            if (dataReceived)
            {
                using (DrawingContext dc = this.skeletonDrawingGroup.Open())
                {
                    // Draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    int penIndex = 0;
                    foreach (Body body in this.bodies)
                    {
                        Pen drawPen = HendlerHolder.BodyColors[penIndex++];

                        if (body.IsTracked)
                        {
                            this.DrawClippedEdges(body, dc);

                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            // convert the joint points to depth (display) space
                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            foreach (JointType jointType in joints.Keys)
                            {
                                // sometimes the depth(Z) of an inferred joint may show as negative
                                // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                CameraSpacePoint position = joints[jointType].Position;
                                if (position.Z < 0)
                                {
                                    position.Z = HendlerHolder.InferredZPositionClamp;
                                }

                                DepthSpacePoint depthSpacePoint = HendlerHolder.coordinateMapper.MapCameraPointToDepthSpace(position);
                                jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                            }

                            this.DrawBody(joints, jointPoints, dc, drawPen);

                            this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                            this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                        }
                    }

                    // prevent drawing outside of our render area
                    this.skeletonDrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                    //UpdateRGBImage(imageSource);
                    UpdateSkeletonImage(skeletonImageSource);
                    if (HendlerHolder.SkeletonPlayerWindow != null)
                    {
                        HendlerHolder.SkeletonPlayerWindow.UpdateImage(skeletonImageSource);
                    }
                }
                if (HendlerHolder.DepthPlayerWindow != null)
                {
                    using (DrawingContext dc = this.depthDrawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        //if (Background.SelectedIndex == 0)
                        //    dc.DrawImage(this.colorBitmap, new Rect(0, 0, 640, 480));
                        //if (Background.SelectedIndex == 1)
                        dc.DrawImage(this.depthBitmap, new Rect(0, 0, 512, 424));
                        int penIndex = 0;
                        foreach (Body body in this.bodies)
                        {
                            Pen drawPen = HendlerHolder.BodyColors[penIndex++];

                            if (body.IsTracked)
                            {
                                this.DrawClippedEdges(body, dc);

                                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                                // convert the joint points to depth (display) space
                                Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                                foreach (JointType jointType in joints.Keys)
                                {
                                    // sometimes the depth(Z) of an inferred joint may show as negative
                                    // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                                    CameraSpacePoint position = joints[jointType].Position;
                                    if (position.Z < 0)
                                    {
                                        position.Z = HendlerHolder.InferredZPositionClamp;
                                    }

                                    DepthSpacePoint depthSpacePoint = HendlerHolder.coordinateMapper.MapCameraPointToDepthSpace(position);
                                    jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                                }

                                this.DrawBody(joints, jointPoints, dc, drawPen);

                                this.DrawHand(body.HandLeftState, jointPoints[JointType.HandLeft], dc);
                                this.DrawHand(body.HandRightState, jointPoints[JointType.HandRight], dc);
                            }
                        }

                        // prevent drawing outside of our render area
                        this.depthDrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        //UpdateRGBImage(imageSource);
                        UpdateDepthImage(depthImageSource);
                        if (HendlerHolder.DepthPlayerWindow != null)
                        {
                            HendlerHolder.DepthPlayerWindow.UpdateImage(depthImageSource);
                        }
                    }
                    //HendlerHolder.SkeletonPlayerWindow.UpdateImage(imageSource);
                }

                if (HendlerHolder.RGBPlayerWindow != null)
                {
                    using (DrawingContext dc = this.rgbDrawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        //if (Background.SelectedIndex == 0)
                        //    dc.DrawImage(this.colorBitmap, new Rect(0, 0, 640, 480));
                        //if (Background.SelectedIndex == 1)
                        dc.DrawImage(this.colorBitmap, new Rect(0, 0, 512, 424));
                        this.rgbDrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        UpdateRGBImage(rgbImageSource);
                        //UpdateDepthImage(rgbImageSource);
                        if (HendlerHolder.RGBPlayerWindow != null)
                        {
                            HendlerHolder.RGBPlayerWindow.UpdateImage(rgbImageSource);
                        }
                    }
                    //HendlerHolder.SkeletonPlayerWindow.UpdateImage(imageSource);*/
                }
                if (HendlerHolder.InfraredPlayerWindow != null)
                {
                    using (DrawingContext dc = this.infraredDrawingGroup.Open())
                    {
                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        //if (Background.SelectedIndex == 0)
                        //    dc.DrawImage(this.colorBitmap, new Rect(0, 0, 640, 480));
                        //if (Background.SelectedIndex == 1)
                        dc.DrawImage(this.infraredBitmap, new Rect(0, 0, 512, 424));
                       
                        this.infraredDrawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));
                        UpdateInfraredImage(infraredImageSource);
                        //UpdateDepthImage(rgbImageSource);
                        if (HendlerHolder.InfraredPlayerWindow != null)
                        {
                            HendlerHolder.InfraredPlayerWindow.UpdateImage(infraredImageSource);
                        }
                    }
           
                }
            }
            if (dataReceived)
            {
                foreach (PlayerWindow playerWindow in HendlerHolder.PlayerWindows)
                {
                    if (playerWindow.Recording && HendlerHolder.IsKinectPlayerWindow(playerWindow))
                    {
                        if (playerWindow.SKLFileStream != null)
                        {
                            TSkeleton ts = null;
                            foreach (Body body in this.bodies)
                            {
                                if (body.IsTracked )
                                {
                                    long elapsed = playerWindow.CaptureStopwatch.ElapsedMilliseconds;
                                    
                                    if (playerWindow.TimeFromStartOfCapture < 0)
                                        playerWindow.TimeFromStartOfCapture = elapsed - 10;
                                    long time = elapsed - playerWindow.TimeFromStartOfCapture;
                                    if (time > AcquisitionFrequencyTreshold * 1000)
                                    {
                                    ts = TSkeletonHelper.SkeletonToTSkeleton(body, time, FloorClipPlane);
                                    //playerWindow.TimeFromStartOfCapture = elapsed;
                                    TSkeletonHelper.SaveTSkeletonToFile(ts, playerWindow.SKLFileStream, playerWindow.FrameCount);
                                    }
                                }
                            }
                            if (ts != null)
                            {
                                playerWindow.FrameCount++;
                                playerWindow.TimeFromStartOfCapture = playerWindow.CaptureStopwatch.ElapsedMilliseconds;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="drawingPen">specifies color to draw a specific body</param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext, drawingPen);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

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
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], HendlerHolder.JointThickness, HendlerHolder.JointThickness);
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

        //private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext, Pen drawingPen)
        private void DrawBody(TSkeleton tSkeleton, DrawingContext drawingContext, Pen drawingPen)    
        {
            this.DrawClippedEdges(tSkeleton, drawingContext);

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

            this.DrawBody(tSkeleton, updatedPositions, drawingContext, drawingPen);

            this.DrawHand(tSkeleton.HandLeftState, updatedPositions[(int)JointType.HandLeft], drawingContext);
            this.DrawHand(tSkeleton.HandRightState, updatedPositions[(int)JointType.HandRight], drawingContext);
        }

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

        private void DrawBone(TSkeleton tSkeleton, Point[] jointPoints, int jointType0, int jointType1, DrawingContext drawingContext, Pen drawingPen)
        {

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
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints">joints to draw</param>
        /// <param name="jointPoints">translated positions of joints to draw</param>
        /// <param name="jointType0">first joint of bone to draw</param>
        /// <param name="jointType1">second joint of bone to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// /// <param name="drawingPen">specifies color to draw a specific bone</param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext, Pen drawingPen)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = HendlerHolder.InferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
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

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - HendlerHolder.ClipBoundsThickness, this.displayWidth, HendlerHolder.ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, HendlerHolder.ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, HendlerHolder.ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - HendlerHolder.ClipBoundsThickness, 0, HendlerHolder.ClipBoundsThickness, this.displayHeight));
            }
        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = HendlerHolder.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }



        private void ShowRulesF()
        {
            if (inter != null)
            {
                RuleViewer rw = new RuleViewer(inter.FeatureTable, inter.RuleTable);
                rw.Show();
            }
        }


        private void RunRGDL()
        {
            RGDLWindow rgdl = new RGDLWindow();
            int windowCount = HendlerHolder.PlayerWindows.Count;
            String SKLFileLoaded = null;
            int a = 0;
            while (a < windowCount)
            {
                if (!HendlerHolder.IsKinectPlayerWindow((PlayerWindow)HendlerHolder.PlayerWindows[a]))
                {
                    if (((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLFileName != null)
                    {
                        SKLFileLoaded = ((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLFileName;
                        a = windowCount;
                    }
                }
            }
            if (SKLFileLoaded != null)
                rgdl.setSKLFile(SKLFileLoaded);
            if (this.GDLFileLoaded != null)
            {
                rgdl.setInputGDLFile(this.GDLFileLoaded);
                rgdl.setOutputGDLFile(this.GDLFileLoaded + "2.gdl");
            }
            rgdl.ShowDialog();
            if (rgdl.getOutputGDLFile() != null)
            {
                LoadFile(rgdl.getOutputGDLFile());
                RunParser(rgdl.getOutputGDLFile());
            }
            rgdl.Close();
            rgdl = null;
        }

        private void ComputeRecognitionSummaryF(bool showSubrules, bool showMainRules)
        {
            if (inter == null)
            {
                MessageBox.Show(this, "No rules loaded.", "Compute recognition summary error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (inter.RuleTable == null)
            {
                MessageBox.Show(this, "No rules loaded.", "Compute recognition summary error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int a = 0;
            int windowCount = HendlerHolder.PlayerWindows.Count;
            ArrayList recordingArraySKL = null;
            while (a < windowCount)
            {
                if (!HendlerHolder.IsKinectPlayerWindow((PlayerWindow)HendlerHolder.PlayerWindows[a]))
                {
                    if (((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLFileName != null)
                    {
                        recordingArraySKL = ((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLRecording;
                        a = windowCount;
                    }
                }
            }
            if (recordingArraySKL == null)
            {
                MessageBox.Show(this, "No SKL file loaded.", "Compute recognition summary error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (recordingArraySKL.Count < 1)
            {
                MessageBox.Show(this, "No SKL file loaded.", "Compute recognition summary error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GDLInterpreter gdlI2 = new GDLInterpreter(this.inter.FeatureTable, this.inter.RuleTable, this.inter.HeapSize);
            //Dictionary<string, int> dictionary = new Dictionary<string, int>();
            //RecognitionCounter recognitionCounter = new RecognitionCounter();
            //recognitionCounter.Show();
            RecognitionCounter recognitionCounter = HendlerHolder.OpenRecognitionCounter();
            
            for (a = 0; a < recordingArraySKL.Count; a++)
            {
                TSkeleton firstTracked = null;
                TSkeleton[] tm = (TSkeleton[])recordingArraySKL[a];
                for (int b = 0; b < tm.Length; b++)
                {
                    if (firstTracked == null)
                        firstTracked = tm[b];


                    double seconds = (double)firstTracked.TimePeriod / 1000.0;
                    //TSkeleton fT = TSkeletonHelper.TSkeletonToSkeleton(firstTracked);
                    Point3D[] bodyParts = HendlerHolder.GenerateBodyPartArray(firstTracked, 0);
                    String[] con = gdlI2.ReturnConclusions(bodyParts, seconds);
                    for (int c = 0; c < con.Length; c++)
                    {
                        if (!con[c].Contains("!") && showSubrules)
                        {
                            Dispatcher.Invoke(
                                new Action(
                                () =>
                                {
                                    recognitionCounter.AddToDictionary(con[c]);
                                }));
                        }
                        if (con[c].Contains("!") && showMainRules)
                        {
                            Dispatcher.Invoke(
                                new Action(
                                () =>
                                {
                                    recognitionCounter.AddToDictionary(con[c]);
                                }));
                        }
                    }
                }
            }
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            GDLFileLoaded = null;
            this.Title = HendlerHolder.ApplicationName;
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            LoadRulesF();
        }
        
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void RunParser_Click(object sender, RoutedEventArgs e)
        {
            RunParser();
        }
        private void ShowRules_Click(object sender, RoutedEventArgs e)
        {
            ShowRulesF();
        }
        private void ShowKeyFrames_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyFramesF(true, true);
        }
        private void ComputeRecognitionSummary_Click(object sender, RoutedEventArgs e)
        {
            ComputeRecognitionSummaryF(false, true);
        }
        private void RGDL_Click(object sender, RoutedEventArgs e)
        {
            RunRGDL();
        }
        private void GDL12TO10_Click(object sender, RoutedEventArgs e)
        {
            //string textHelp = textEditor.Text;
            //textEditor.Text = GDL12TO10AlphaConverter(textHelp);
            string GDLFileLoaded10 = this.GDLFileLoaded + " 1.0.gdl";
            ///System.IO.File.WriteAllText(GDLFileLoaded10, textEditor.Text);
            LoadFile(GDLFileLoaded10);
            RunParser();
        }

        public String GDL12TO10AlphaConverter(String file12Content)
        {
            
            String fileContent = file12Content;

            bool allfeaturesFound = false;
            int position = 0;
            String featureContent = null;
            String featureName = null;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            while (!allfeaturesFound)
            {
                int startF = fileContent.IndexOf("FEATURE", position, StringComparison.OrdinalIgnoreCase);
                if (startF >= 0)
                {
                    int endF = fileContent.IndexOf(" AS ", startF, StringComparison.OrdinalIgnoreCase) + 1;
                    int startLength = startF + "FEATURE".Length + 1;
                    try
                    {
                        featureContent = fileContent.Substring(startLength, endF - startLength);

                        featureContent = featureContent.Trim();

                        startF = endF + "AS".Length;
                        endF = fileContent.IndexOf("\r\n", startF, StringComparison.OrdinalIgnoreCase);
                        featureName = fileContent.Substring(startF, endF - startF);
                        featureName = featureName.Trim();

                        dictionary.Add(featureName, featureContent);

                        position = endF;
                    }
                    catch
                    {
                        allfeaturesFound = true;
                    }
                }
                else
                    allfeaturesFound = true;
            }

            String rulesContent;
            int ruleStartIndex = fileContent.IndexOf("RULE", 0, StringComparison.OrdinalIgnoreCase);
            rulesContent = fileContent.Substring(ruleStartIndex);
            rulesContent = rulesContent.ToLower();

            //foreach (KeyValuePair<string, string> pair in dictionary)

            List<string> list = new List<string>(dictionary.Keys);
            foreach (string k in list)
            {
                if (k.ToLower().Contains("_mean_"))
                {
                    rulesContent = rulesContent.Replace(k.ToLower(), dictionary[k]);
                    dictionary.Remove(k);
                }
            }

            foreach (string k in list)
            {
                if (k.ToLower().Contains("_dev_"))
                {
                    rulesContent = rulesContent.Replace(k.ToLower(), dictionary[k]);
                    dictionary.Remove(k);
                }
            }

            foreach (string k in list)
            {
                if (k.ToLower().Contains("_eps"))
                {
                    rulesContent = rulesContent.Replace(k.ToLower(), dictionary[k]);
                    dictionary.Remove(k);
                }
            }

            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                rulesContent = rulesContent.Replace(pair.Key.ToLower(), pair.Value);
            }
            return rulesContent;
        }

        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutGDL_20 about = new AboutGDL_20();
            about.ShowDialog();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!HendlerHolder.FileDroppingEnabled)
            {
                string msg = "File dropping has been disabled either by internal processing (like parsing) or by the user in main menu.";
                HendlerHolder.DisplayMessage(this, msg);
                return;
            }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                for (int a = 0; a < files.Length; a++)
                {
                    if (Path.GetExtension(files[0]).ToLower() == ".gdl")
                    {
                        LoadFile(files[a]);
                        RunParser(files[a]);
                    }
                    else if (Path.GetExtension(files[0]).ToLower() == ".skl")
                    {
                        HendlerHolder.OpenNewPlayerWindow(true, files[a]);
                    }
                }
            }

        }

        bool isDataDirty = false;

        
        String GDLFileLoaded = null;
        
        private void LoadFile(String fileName)
        {
            try
            {
                String appllicationName = HendlerHolder.ApplicationName;
                if (appllicationName == null)
                    appllicationName = "";
                this.Title = appllicationName + "-[" + fileName + "]";
                this.GDLFileLoaded = fileName;
                isDataDirty = false;
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "The file: \"" + fileName + "\" couldn't be loaded.", "GDL script editor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunParser(String FileName)
        {
            Thread t = new Thread(() => LoaderGDLThread(FileName));
            t.IsBackground = true;
            t.Start();
        }

        private void LoaderGDLThread(String fileName)
        {
            // Open document 
            GDLInterpreter oldInter = inter;
            bool oldFileDroppingEnabled = HendlerHolder.FileDroppingEnabled;
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.LoadRules.IsEnabled = false;
                    this.ShowRules.IsEnabled = false;
                    this.menu1.IsEnabled = false;
                    this.ProcessingLabel.Visibility = System.Windows.Visibility.Visible;
                    this.ProcessingLabel.Content = "Parsing...";
                    HendlerHolder.FileDroppingEnabled = false;
                }));
            try
            {
                ParserToken[] AllFeatures = null;
                ParserToken[] AllRules = null;
                GDLParser.ParseFile(fileName, ref AllFeatures, ref AllRules);
                inter = new GDLInterpreter(AllFeatures, AllRules);
                Dispatcher.Invoke(
                        new Action(
                            () =>
                            {
                                MessageBox.Show(this, "The file has been correctly parsed.", "GDL script editor", MessageBoxButton.OK, MessageBoxImage.Information);
                            }));
            }
            catch (ParserException pe)
            {

                Dispatcher.Invoke(
                new Action(
                    () =>
                    {
                        MessageBox.Show(this, pe.Message, "GDL script editor", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                    }));
                inter = oldInter;
            }
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.LoadRules.IsEnabled = true;
                    this.ShowRules.IsEnabled = true;
                    this.menu1.IsEnabled = true;
                    this.GDLFileLoaded = fileName;
                    this.ProcessingLabel.Visibility = System.Windows.Visibility.Hidden;
                    HendlerHolder.FileDroppingEnabled = oldFileDroppingEnabled;
                }));
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            if (e.Key == Key.F3)
            {
                LoadRulesF();
            }
            if (e.Key == Key.F5)
            {
                RunParser();
            }
            if (e.Key == Key.F6)
            {
                RunRGDL();
            }
            if (e.Key == Key.F7)
            {
                ShowKeyFramesF(true, true);
            }
            if (e.Key == Key.F1)
            {
                //HelpSyntax();
            }
        }

        private void ShowKeyFramesF(bool showSubrules, bool showMainRules)
        {
            if (inter == null)
            {
                MessageBox.Show(this, "No rules loaded.", "Show key frames error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (inter.RuleTable == null)
            {
                MessageBox.Show(this, "No rules loaded.", "Show key frames error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int a = 0;
            int windowCount = HendlerHolder.PlayerWindows.Count;
            ArrayList recordingArraySKL = null;
            while (a < windowCount)
            {
                if (!HendlerHolder.IsKinectPlayerWindow((PlayerWindow)HendlerHolder.PlayerWindows[a]))
                {
                    if (((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLFileName != null)
                    {
                        recordingArraySKL = ((PlayerWindow)HendlerHolder.PlayerWindows[a]).SKLRecording;
                        a = windowCount;
                    }
                }
            }
            if (recordingArraySKL == null)
            {
                MessageBox.Show(this, "No SKL file loaded.", "Show key frames error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (recordingArraySKL.Count < 1)
            {
                MessageBox.Show(this, "No SKL file loaded.", "Show key frames error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            GDLInterpreter gdlI2 = new GDLInterpreter(this.inter.FeatureTable, this.inter.RuleTable, this.inter.HeapSize);
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (a = 0; a < recordingArraySKL.Count; a++)
            {
                TSkeleton firstTracked = null;
                TSkeleton[] tm = (TSkeleton[])recordingArraySKL[a];
                for (int b = 0; b < tm.Length; b++)
                {
                    if (firstTracked == null)
                        firstTracked = tm[b];


                    double seconds = (double)firstTracked.TimePeriod / 1000.0;
                    //Skeleton fT = TSkeletonHelper.TSkeletonToSkeleton(firstTracked);
                    Point3D[] bodyParts = HendlerHolder.GenerateBodyPartArray(firstTracked, 0);
                    //String[] con = inter.ReturnConclusions(fT, 0, seconds);
                    //String[] con = inter.ReturnConclusions(bodyParts, 0, seconds);
                    String[] con = gdlI2.ReturnConclusions(bodyParts, seconds);
                    for (int c = 0; c < con.Length; c++)
                    {
                        if (!con[c].Contains("!") && showSubrules)
                            dictionary[con[c]] = a;
                        if (con[c].Contains("!") && showMainRules)
                            dictionary[con[c]] = a;
                    }
                }
            }

            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                //Console.WriteLine("{0}, {1}",
                //pair.Key,
                //pair.Value);
                TSkeleton firstTracked = null;
                TSkeleton[] tm = (TSkeleton[])recordingArraySKL[pair.Value];
                for (int b = 0; b < tm.Length; b++)
                {
                    if (firstTracked == null)
                        firstTracked = tm[b];
                    ShowFrameWindow sfw = new ShowFrameWindow();
                    using (DrawingContext dc = sfw.drawingVisual.RenderOpen())
                    {
                        dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, 512, 424));
                        for (int c = 0; c < tm.Length; c++)
                        {
                            Pen drawPen = HendlerHolder.BodyColors[0];
                            this.DrawBody(firstTracked, dc, drawPen);
                        }
                            //this.DrawBonesAndJoints(tm[c], dc);
                    }
                    String textHelp = "Rule: " + pair.Key + ", Frame no.: " + pair.Value;
                    sfw.Title = textHelp;
                    sfw.InfoTextBox.Text = textHelp;
                    sfw.SkeletonRTB.Render(sfw.drawingVisual);
                    sfw.Show();
                    sfw.UpdateImage(sfw.SkeletonRTB);
                }



            }
        }

        private void LoadRulesF()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gdl";
            dlg.Filter = "GDL Files (*.gdl)|*.gdl";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                LoadFile(dlg.FileName);
                RunParser(dlg.FileName);
            }
        }

        private void RunParser()
        {
            if (GDLFileLoaded == null || isDataDirty)
            {
                MessageBox.Show(this, "You must save your file first before parsing.", "GDL script editor", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            RunParser(GDLFileLoaded);
        }

       

        private void SkeletonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (HendlerHolder.SkeletonPlayerWindow == null)
                {
                    HendlerHolder.SkeletonPlayerWindow = new PlayerWindow();
                    HendlerHolder.SkeletonPlayerWindow.Show();
                }
            }
        }

        private void EnableSkeleton_Checked(object sender, RoutedEventArgs e)
        {
            if (EnableSkeleton.IsChecked == true)
                this.bodyFrameReader.FrameArrived += this.ReaderBody_FrameArrived;
            else
                this.bodyFrameReader.FrameArrived -= this.ReaderBody_FrameArrived;
            // wire handler for frame arrival
            //this.depthFrameReader.FrameArrived += this.ReaderDepth_FrameArrived;
        }

        private void EnableDepth_Checked(object sender, RoutedEventArgs e)
        {
            if (EnableDepth.IsChecked == true)
                this.depthFrameReader.FrameArrived += this.ReaderDepth_FrameArrived;
            else
                this.depthFrameReader.FrameArrived -= this.ReaderDepth_FrameArrived;
        }

        private void DepthImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (HendlerHolder.DepthPlayerWindow == null)
                {
                    HendlerHolder.DepthPlayerWindow = new PlayerWindow();
                    HendlerHolder.DepthPlayerWindow.Show();
                }
            }
        }

        private void EnableDepth_Unchecked(object sender, RoutedEventArgs e)
        {
            if (EnableDepth.IsChecked == true)
                this.depthFrameReader.FrameArrived += this.ReaderDepth_FrameArrived;
            else
                this.depthFrameReader.FrameArrived -= this.ReaderDepth_FrameArrived;
        }

        private void EnableRGB_Click(object sender, RoutedEventArgs e)
        {
            if (EnableRGB.IsChecked == true)
                // wire handler for frame arrival
                this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;
            else
                this.colorFrameReader.FrameArrived -= this.Reader_ColorFrameArrived;
        }

        private void RGBImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (HendlerHolder.RGBPlayerWindow == null)
                {
                    HendlerHolder.RGBPlayerWindow = new PlayerWindow();
                    HendlerHolder.RGBPlayerWindow.Show();
                }
            }
        }

        private void InfraredImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (HendlerHolder.InfraredPlayerWindow == null)
                {
                    HendlerHolder.InfraredPlayerWindow = new PlayerWindow();
                    HendlerHolder.InfraredPlayerWindow.Show();
                }
            }
        }

        private void EnableInfrared_Click(object sender, RoutedEventArgs e)
        {
            if (EnableInfrared.IsChecked == true)
                // wire handler for frame arrival
                this.infraredFrameReader.FrameArrived += this.Reader_InfraredFrameArrived;
            else
                this.infraredFrameReader.FrameArrived -= this.Reader_InfraredFrameArrived;

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        
    }
}
