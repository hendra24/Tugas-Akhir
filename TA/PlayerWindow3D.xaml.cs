using org.TKinect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    /// <summary>
    /// Interaction logic for PlayerWindow3D.xaml
    /// </summary>
    public partial class PlayerWindow3D : Window
    {
        ArrayList SKLRecording = new ArrayList();


        //ArrayList jointIndexAray = new ArrayList();
        Point3D floorPoint;
        double cameraDistance = 5;
        double cameraHeight = 0.25;
        double cameraMinDistane = 0.1;
        float angleX = 0;
        float angleY = 180;

        BallModel []myBallModels = null;

        private void PrepareAnimation()
        {
            floorPoint = new Point3D(0, ((TSkeleton[])SKLRecording[0])[0].Position[15].Y, 0);
            myBallModels = new BallModel[1];
            for (int a = 0; a < myBallModels.Length; a++)
            {
                if (a == 0)
                    myBallModels[a] = new BallModel(_models, Colors.Red, Colors.Green);
                else
                    myBallModels[a] = new BallModel(_models, Colors.Orange, Colors.BlueViolet);
            }
            
             _models.ForEach(x => mainViewport.Children.Add(x));
            
            
            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource =
                new BitmapImage(new Uri("Wall3.png", UriKind.Relative));
            
            //colors_brush.TileMode = TileMode.Tile;
            //colors_brush.Stretch = Stretch.None;
            DiffuseMaterial colors_material =
        new DiffuseMaterial(colors_brush);
            
            double eps = 0.1;
            double roomHeight = 4;
            double roomWidth = 10;
            
            //front
            mainViewport.Children.Add(GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X, floorPoint.Y + (roomHeight / 2.0) - eps, floorPoint.Z + (roomWidth / 2.0)), new Size3D(roomWidth, roomHeight, 0.01)));
            //right
            mainViewport.Children.Add(GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X - (roomWidth / 2.0), floorPoint.Y + (roomHeight / 2.0) - eps, floorPoint.Z), new Size3D(0.01, roomHeight, roomWidth)));
            //left
            mainViewport.Children.Add(GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X + (roomWidth / 2.0), floorPoint.Y + (roomHeight / 2.0) - eps, floorPoint.Z), new Size3D(0.01, roomHeight, roomWidth)));
            //rear
            mainViewport.Children.Add(GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X, floorPoint.Y + (roomHeight / 2.0) - eps, floorPoint.Z - (roomWidth / 2.0)), new Size3D(roomWidth, roomHeight, 0.01)));
            
            colors_brush = new ImageBrush();
            colors_brush.ImageSource =
                new BitmapImage(new Uri("Floor.png", UriKind.Relative));
                //new BitmapImage(new Uri("Floor3.png", UriKind.Relative));
            colors_material =
                new DiffuseMaterial(colors_brush);
            
            //ceiling
            ceiling = GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X, floorPoint.Y + roomHeight - eps, floorPoint.Z), new Size3D(roomWidth, 0.01, roomWidth));
            mainViewport.Children.Add(ceiling);
            //floor
            mainViewport.Children.Add(GraphicPrimitiveGenerator.GetTexturedCube(colors_material, new Point3D(floorPoint.X, floorPoint.Y, floorPoint.Z), new Size3D(roomWidth, 0.01, roomWidth)));
            mainViewport.ClipToBounds = false;
            mainViewport.IsHitTestVisible = false;
            
        }
        ModelVisual3D ceiling = null;
        public MaterialGroup GetSurfaceTexture(Color colour)
        {
            var materialGroup = new MaterialGroup();
            ImageBrush colors_brush = new ImageBrush();
            colors_brush.ImageSource =
                new BitmapImage(new Uri("howto_wpf_textures3.png", UriKind.Relative));
            colors_brush.TileMode = TileMode.Tile;
            colors_brush.Stretch = Stretch.None;
            return materialGroup;
        }

        private void ChangeContentWhileRecordingOrPlaying(bool playing)
        {
            StopButton.IsEnabled = playing;
            PlayButton.IsEnabled = !playing;
        }

        bool IsPlaying = false;

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeContentWhileRecordingOrPlaying(true);
            
            Thread ReaderThread = new Thread(PlayerThread);
            ReaderThread.IsBackground = true;
            ReaderThread.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            ChangeContentWhileRecordingOrPlaying(false);
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((int)ProgressSlider.Value < SKLRecording.Count)
                ShowFrame((TSkeleton[])SKLRecording[(int)ProgressSlider.Value]);
        }

        public PlayerWindow3D(ArrayList SKLRecording)
        {
            if (SKLRecording == null)
                return;
            if (SKLRecording.Count <= 0)
                return;
            InitializeComponent();
            //SKLRecording = TSkeletonHelper.ReadRecordingFromFile("SkeletonRecord18.skl");
            this.SKLRecording = SKLRecording;
            if (SKLRecording == null)
                this.Close();
            ProgressSlider.Maximum = SKLRecording.Count - 1;
            ProgressSlider.Value = 0;
            //SKLRecording = TSkeletonHelper.ReadRecordingFromFile("c:\\publikacje\\Silownia karate full\\Nagranie 1\\Kinect 2\\segmented\\skl1\\mae-geri\\middling\\SkeletonRecord8.skl");
            //ShowFrame((TSkeleton[])SKLRecording[0]);

            PrepareAnimation();
            if (SKLRecording.Count > 0)
            {
                for (int a = 0; a < myBallModels.Length; a++)
                    ShowFrame((TSkeleton[])SKLRecording[0]);
            }
        }

        

        private void PlayerThread()
        {
            TSkeleton[] TSkeletons = null;
            int line = 0;
            Dispatcher.Invoke(new Action(
                        () =>
                        {
                            line = (int)ProgressSlider.Value;
                        }));
            bool doOnce = true;
            IsPlaying = true;
            try
            {
                while (line < SKLRecording.Count && IsPlaying)
                {

                    TSkeletons = (TSkeleton[])SKLRecording[line];
                    //ShowFrame((TSkeleton[])TSkeletons);
                    
                    /*
                    if (doOnce)
                    Dispatcher.Invoke(new Action(
                        () =>
                        {
                            String filler = "";
                            if (SKLRecording.Count < 100)
                            {   
                                if (line < 10)
                                    filler += "0";
                            }
                            else if (SKLRecording.Count < 1000)
                            {
                                if (line < 10)
                                    filler += "00";
                                else if (line < 100)
                                    filler += "0";
                            }

                            takeScreenshot(filler + line + ".png");
                        }));*/
                    Thread.Sleep((int)TSkeletons[0].TimePeriod);
                    Dispatcher.Invoke(new Action(
                        () =>
                        {
                            line++;
                            if (line < SKLRecording.Count)
                                ProgressSlider.Value += 1;
                            else
                            {
                                if (CheckBoxLoop.IsChecked == true)
                                {
                                    line = 0;
                                    Dispatcher.Invoke(new Action(
                                    () =>
                                    {
                                        ProgressSlider.Value = 0;
                                    }));
                                }
                            }
                        }));
                    if (line >= SKLRecording.Count)
                    {
                        doOnce = false;
                    }
                }
            
            IsPlaying = false;
            Dispatcher.Invoke(new Action(
                        () =>
                        {
                            ChangeContentWhileRecordingOrPlaying(false);
                        }));
            }
            catch { }
        }



        //private System.Timers.Timer _timer;
        private readonly List<ModelVisual3D> _models = new List<ModelVisual3D>();
        //private double _angle;

        public void ShowFrame(TSkeleton[] skeletons)
        {
            Dispatcher.Invoke(new Action(
                        () =>
                        {
                            Point3D cp = camera.Position;
                            Matrix3D matrix = new Matrix3D();

                            //matrix.Translate(new Vector3D(0, 0, 10));
                            Quaternion qq1 = new Quaternion(new Vector3D(0, 1, 0), angleY);
                            Quaternion qq2 = new Quaternion(new Vector3D(1, 0, 0), angleX);
                            qq1 *= qq2;
                            //matrix.Rotate(new Quaternion(new Vector3D(0, 1, 0), angleY));
                            //angleY += 0.5f;
                            //matrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), angleX));
                            //angleX += 0.5f;
                            matrix.Rotate(qq1);
                            Point3D results = matrix.Transform(new Point3D(0, cameraHeight, cameraDistance));
                            Vector3D look = new Vector3D(-results.X, -results.Y, -results.Z);
                            look.Normalize();
                            camera.LookDirection = look;
                            //camera.Position = new Point3D(camera.Position.X + 0.01f, camera.Position.Y, camera.Position.Z);
                            camera.Position = results;
                            camera.LookDirection = look;
                            Point3D results2 = matrix.Transform(new Point3D(0, cameraHeight + 0, cameraDistance + 0));
                            Vector3D look2 = new Vector3D(-results2.X, -results2.Y, -results2.Z);
                            look2.Normalize();

                            mySpotLight.Direction = look2;
                            mySpotLight.Position = results2;
                        }));
            
                                               Dispatcher.Invoke(new Action(
                        () =>
                        {
                            //for (int a = 0; a < _models.Count; a++)
                            if (_models.Count <= 0) return;
                            for (int a = 0; a < myBallModels.Length; a++)
                                myBallModels[a].RenderModel((bool)CheckBoxRelativeMove.IsChecked, skeletons[0]);
                            //ArrayList al = new ArrayList();
                            //myBallModels[0].DetectCollision(myBallModels[0], al);
                        }));

        }
        


        
        

        

        private void mainViewport_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void mainViewport_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ButtonState == Pressed 
        }

        Point pp = new Point(-1, -1);


        private void PlayerWindow3D1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
        }

        private void PlayerWindow3D1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released &&
                e.RightButton == MouseButtonState.Released)
                Mouse.Capture(null);
        }

        Point prevPosition = new Point(-1, -1);
        private void PlayerWindow3D1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.Captured == this && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentLocation = Mouse.GetPosition(this);
                Point knobCenter = new Point(this.ActualHeight / 2, this.ActualWidth / 2);
                angleY += (float)(currentLocation.X - prevPosition.X) / 10f;
                angleX += (float)(currentLocation.Y - prevPosition.Y) / 10f;
            }

            if (Mouse.Captured == this && e.RightButton == MouseButtonState.Pressed)
            {
                Point currentLocation = Mouse.GetPosition(this);
                cameraDistance += (float)(currentLocation.Y - prevPosition.Y) / 10f;
                if (cameraDistance < cameraMinDistane)
                    cameraDistance = cameraMinDistane;
            }
            /*if (Mouse.Captured == this && e.MiddleButton == MouseButtonState.Pressed)
            {
                Point currentLocation = Mouse.GetPosition(this);
                cameraHeight += (float)(currentLocation.Y - prevPosition.Y) / 10f;
            }*/

            prevPosition = Mouse.GetPosition(this);
            if ((int)ProgressSlider.Value < SKLRecording.Count)
                ShowFrame((TSkeleton[])SKLRecording[(int)ProgressSlider.Value]);
        }

        private void PlayerWindow3D1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
        }

        private void PlayerWindow3D1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*if(e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                Mouse.Capture(this);
            }*/
        }

        private void takeScreenshot(String filepath)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)mainViewportWithBorder.ActualWidth,
                        (int)mainViewportWithBorder.ActualHeight, 96, 96, PixelFormats.Default);
            bmp.Render(this.mainViewportWithBorder);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            using (Stream s = File.Create(filepath))
            {
                encoder.Save(s);
            }
        }

        private void PlayerWindow3D1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsPlaying = false;
            HendlerHolder.PlayerWindows3D.Remove(this);
        }

        private void CheckBoxRelativeMove_Checked(object sender, RoutedEventArgs e)
        {
            if ((int)ProgressSlider.Value < SKLRecording.Count)
                ShowFrame((TSkeleton[])SKLRecording[(int)ProgressSlider.Value]);
        }

        private void CheckBoxShowCeiling_Checked(object sender, RoutedEventArgs e)
        {
            if (ceiling == null) return;
            if (CheckBoxShowCeiling.IsChecked == true)
            {
                if (!mainViewport.Children.Contains(ceiling))
                    mainViewport.Children.Add(ceiling);
            }
            else
            {
                if (mainViewport.Children.Contains(ceiling))
                    mainViewport.Children.Remove(ceiling);
            }
            if ((int)ProgressSlider.Value < SKLRecording.Count)
                ShowFrame((TSkeleton[])SKLRecording[(int)ProgressSlider.Value]);
        }

        private void CheckBoxShowCeiling_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ceiling == null) return;
            if (CheckBoxShowCeiling.IsChecked == true)
            {
                if (!mainViewport.Children.Contains(ceiling))
                    mainViewport.Children.Add(ceiling);
            }
            else
            {
                if (mainViewport.Children.Contains(ceiling))
                    mainViewport.Children.Remove(ceiling);
            }
            if ((int)ProgressSlider.Value < SKLRecording.Count)
                ShowFrame((TSkeleton[])SKLRecording[(int)ProgressSlider.Value]);
        }


    }

    public class CircleAssitor
    {
        public CircleAssitor()
        {
            CurrentTriangle = new Triangle();
        }

        public Point3D FirstPoint { get; set; }
        public Point3D LastPoint { get; set; }
        public Triangle CurrentTriangle { get; set; }
    }

    public class Triangle
    {
        public Point3D P0 { get; set; }
        public Point3D P1 { get; set; }
        public Point3D P2 { get; set; }

        public Triangle Clone(double z, bool switchP1andP2)
        {
            var newTriangle = new Triangle();
            newTriangle.P0 = GetPointAdjustedBy(this.P0, new Point3D(0, 0, z));

            var point1 = GetPointAdjustedBy(this.P1, new Point3D(0, 0, z));
            var point2 = GetPointAdjustedBy(this.P2, new Point3D(0, 0, z));

            if (!switchP1andP2)
            {
                newTriangle.P1 = point1;
                newTriangle.P2 = point2;
            }
            else
            {
                newTriangle.P1 = point2;
                newTriangle.P2 = point1;
            }
            return newTriangle;
        }

        private Point3D GetPointAdjustedBy(Point3D point, Point3D adjustBy)
        {
            var newPoint = new Point3D { X = point.X, Y = point.Y, Z = point.Z };
            newPoint.Offset(adjustBy.X, adjustBy.Y, adjustBy.Z);
            return newPoint;
        }
    }


}
