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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;



namespace CleanSyncMini
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GUI : Window
    {
        private MainLogic Control;
        private List<PCJob> GUIJobList;
        
        public GUI()
        {
            InitializeComponent();
            this.Control = new MainLogic(0);
        }

               
        private void FolderSelection_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            FolderBrowser.ShowDialog();
            AttachDropBox.Text = FolderBrowser.SelectedPath;
            
        }

        private void FolderSelection_Click1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            FolderBrowser.ShowDialog();
            AttachDropBox1.Text = FolderBrowser.SelectedPath;

        }

        private void FolderSelection_Click2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            FolderBrowser.ShowDialog();
            AttachDropBox2.Text = FolderBrowser.SelectedPath;

        }

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Visible;
            NewJobFrameGridButtons.Visibility = Visibility.Visible;

        }

        private void CancelCreat_Click(object sender, RoutedEventArgs e)
        {
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Visible;
            MainFrameInfor.Visibility = Visibility.Visible;
        }

        private void CancelAccept_Click(object sender, RoutedEventArgs e)
        {
            AcceptFrameInfor.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Visible;
            MainFrameInfor.Visibility = Visibility.Visible;
        }

        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            Analyser Result = new Analyser();
            Result.Owner = this;
            Result.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (PCJob Job in Control.GetPCJobList())
                JobList.Items.Add(Job);
            JobList.DisplayMemberPath = "JobName"; 
            
        }

        private void CreatJob_Click(object sender, RoutedEventArgs e)
        {
            if (AttachDropBox1.Text != null && AttachDropBox2.Text != null &&NewJobName.Text!=null)
            {
                Control.CreateJob(NewJobName.Text, AttachDropBox1.Text, AttachDropBox2.Text);
                foreach (PCJob Job in Control.GetPCJobList())
                    JobList.Items.Add(Job);
            }
        }
    }
}
