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
    /// Interaction logic for RecognitionCounter.xaml
    /// </summary>
    public partial class RecognitionCounter : Window
    {
        private Dictionary<string, int> dictionary;
        public RecognitionCounter()
        {
            InitializeComponent();
            dictionary = new Dictionary<string, int>();
        }

        public void AddToDictionary(String key)
        {
            if (dictionary.ContainsKey(key))
            {
                int countHelp = dictionary[key];
                countHelp++;
                dictionary[key] = countHelp;
            }
            else
            {
                dictionary.Add(key, 1);
            }
            SetText(dictionary);
        }


        public void SetText(Dictionary<string, int>dictionary)
        {
            string text = "";
            foreach (KeyValuePair<string, int> pair in dictionary)
            {
                if (ShowSubrulesCheckBox.IsChecked == true)
                    text += pair.Key + ": " + pair.Value + "\r\n";
                else
                {
                    if (pair.Key.Contains("!"))
                        text += pair.Key + ": " + pair.Value + "\r\n";
                }
            }
            this.counterTextBox.Text = text;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            this.counterTextBox.Text = "";
            dictionary.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HendlerHolder.RecognitionCounter = null;
        }

        private void ShowSubrulesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void ShowSubrulesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            SetText(dictionary);
        }
    }
}
