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
using org.GDL;
using System.Collections;
using org.TKinect;
using System.Globalization;
using org.RGDL;

namespace org.GDLStudio
{
    /// <summary>
    /// Interaction logic for RGDLWindow.xaml
    /// </summary>
    public partial class RGDLWindow : Window
    {
        public RGDLWindow()
        {
            InitializeComponent();
        }

        public void setSKLFile(String sklFile)
        {
            this.SKLFileTextBox.Text = sklFile;
        }

        public void setInputGDLFile(String inputGDLFile)
        {
            this.InputGDLtextBox.Text = inputGDLFile;
        }

        public void setOutputGDLFile(String outputGDLFile)
        {
            this.OutputGDLtextBox.Text = outputGDLFile;
        }

        public String getOutputGDLFile()
        {
            return this.OutputGDLFile;
        }

        private String OutputGDLFile = null;

        public int[] keyFrames = null;
        
        private void ComputeButton_Click(object sender, RoutedEventArgs e)
        {
            ArrayList skelRecording = TSkeletonHelper.ReadRecordingFromFile(SKLFileTextBox.Text);
            int clastersCount = int.Parse(ClasterCountBox.Text);
            double minimaTimeDistanceBox = double.Parse(MinimaTimeDistanceBox.Text, CultureInfo.InvariantCulture);
            double epsilon = double.Parse(textBoxEpsilon.Text, CultureInfo.InvariantCulture);
            int maxIterations = int.Parse(textBoxMaxIterations.Text, CultureInfo.InvariantCulture);
            try
            {
                String GDLVersion = "1.1";
                if (radioButtonGDL1_0.IsChecked == true)
                    GDLVersion = "1.0";
                if (radioButtonGDL1_1.IsChecked == true)
                    GDLVersion = "1.1";
                //if (radioButtonGDL1_1.IsChecked == true)
                //!!
                GDLVersion = "1.1";
                keyFrames = RGDLTrainer(skelRecording,
                    InputGDLtextBox.Text,
                    OutputGDLtextBox.Text,
                    clastersCount, minimaTimeDistanceBox,
                    textBoxRuleName.Text,
                    GDLVersion,
                    epsilon,
                    maxIterations);
                OutputGDLFile = OutputGDLtextBox.Text;
                //this.Close();
                this.Visibility = Visibility.Hidden;
            }
            catch (RGDLException ex)
            {
                MessageBox.Show(ex.Message, "R-GDL Exception", MessageBoxButton.OK);
            }
        }


        private static int[] RGDLTrainer(ArrayList skelRecording, String oldFileName, String newFileName, int clastersCount, double minimalTimeDistance,
            String ruleName, String GDLVersion, double epislon, int maxIterations)
        {
            int[] keyframes = null;
            ParserToken[] AllFeatures = null;
            ParserToken[] AllRules = null;
            GDLParser.ParseFile(oldFileName, ref AllFeatures, ref AllRules);
            string oldGDLFileContent = System.IO.File.ReadAllText(oldFileName);

            int seed = 0;
            if (RandomInit)
            {
                Random r = new Random();
                seed = r.Next();
            }

            String gdlFileContent = org.RGDL.GDLGenerator.GenerateRules(skelRecording, AllFeatures,
                oldGDLFileContent,
                clastersCount, minimalTimeDistance,
                maxIterations,//
                ruleName, 
                GDLVersion,
                epislon,
                seed,
                ref keyframes);
            System.IO.File.WriteAllText(newFileName, gdlFileContent);
            return keyframes;
            //MessageBox.Show("Done.", "OK", MessageBoxButton.OK);
        }

        private void SKLFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".skl";
            dlg.Filter = "SKL Files (*.skl)|*.skl";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                SKLFileTextBox.Text = dlg.FileName;
            }
        }

        private void InputGDLButton_Click(object sender, RoutedEventArgs e)
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
                InputGDLtextBox.Text = dlg.FileName;
            }
        }

        private void OutputButton_Click(object sender, RoutedEventArgs e)
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
                OutputGDLtextBox.Text = dlg.FileName;
            }
        }

        private static bool RandomInit = true;

        private void checkBoxRandomInitialization_Checked(object sender, RoutedEventArgs e)
        {
            RandomInit = checkBoxRandomInitialization.IsChecked.Value;
        }


    }
}
