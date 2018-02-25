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

namespace org.GDLStudio
{
    /// <summary>
    /// Interaction logic for RuleViewer.xaml
    /// </summary>
    public partial class RuleViewer : Window
    {
        private static TreeViewItem GenerateNodeForRule(ParserToken rule)
        {
            TreeViewItem ruleNode = new TreeViewItem();
            ruleNode.Header = rule.TokenString;
            for (int a = 0; a < rule.Children.Count; a++)
            {
                TreeViewItem tn = GenerateNodeForRule((ParserToken)rule.Children[a]);
                ruleNode.Items.Add(tn);
            }
            return ruleNode;
        }

        private static void MakeTree(ParserToken[] AllRules, TreeView tv)
        {
            for (int a = 0; a < AllRules.Length; a++)
            {
                TreeViewItem rootConclusion = new TreeViewItem();
                rootConclusion.Header = AllRules[a].Conclusion;
                TreeViewItem tn = GenerateNodeForRule(AllRules[a]);
                rootConclusion.Items.Add(tn);
                tv.Items.Add(rootConclusion);
            }
        }

        public RuleViewer(org.GDL.ParserToken[] AllFeatures, org.GDL.ParserToken[] AllRules)
        {
            InitializeComponent();
            if (AllFeatures != null)
                MakeTree(AllFeatures, this.treeView0);
            MakeTree(AllRules, this.treeView1);
        }

    }
}
