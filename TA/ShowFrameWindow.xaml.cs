using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace org.GDLStudio
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class ShowFrameWindow : Window
    {
        public ShowFrameWindow()
        {
            InitializeComponent();
            SkeletonRTB = new RenderTargetBitmap(512, 424, 96.0, 96.0, PixelFormats.Default);
            drawingVisual = new DrawingVisual();
        }
        
        public RenderTargetBitmap SkeletonRTB = null;
        public DrawingVisual drawingVisual = null;

        public void UpdateImage(ImageSource source)
        {
            Dispatcher.Invoke(
            new Action(
                () =>
                {
                    this.FrameImage.Source = source;
                }));
        }
    }
}
