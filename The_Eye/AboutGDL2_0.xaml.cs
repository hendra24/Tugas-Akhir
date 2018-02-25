using Microsoft.Samples.Kinect.BodyBasics;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class AboutGDL_20 : Window
    {
        public AboutGDL_20()
        {
            InitializeComponent();
            Title = "About " + HendlerHolder.ApplicationName;

            TextBoxAppName.Text = ApplicationInfo.ProductName + " v. " + ApplicationInfo.Version;
            TextBoxCredits.Text =
                ApplicationInfo.CopyrightHolder + "\r\n"
                + ApplicationInfo.Description + "\r\n"
                + ApplicationInfo.CompanyName;

        }
    }
}
