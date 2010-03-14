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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;
using System.Threading;
using DirectoryInformation;


namespace CleanSyncMini
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GUI : Window
    {
        private MainLogic Control;
        private BackgroundWorker backgroundWorker2;
        private BackgroundWorker backgroundWorker1;
        ComparisonResult result;
        PCJob cmpJob; 
        private USBDetection usbDetector;
        private List<PCJob> GUIJobList;
        private BackgroundWorker bgWorker;
        private PCJob selectedPCJob;
        
        
        public GUI()
        {            
            InitializeComponent();
            this.Control = new MainLogic();
            Control.InitializePCJobInfo();
            GUIJobList = Control.GetPCJobs();

            string[] logicDrives = System.Environment.GetLogicalDrives();
            List<string> drives = new List<string>();
            foreach (string drive in logicDrives)
            {
                drives.Add(drive);
            }
            
            Control.USBPlugIn(drives);
            getIncompletedJobList(drives);
            usbDetectionThread();
            backgroundWorker2 = new BackgroundWorker();
            InitializeBackgroundWorker();
            
        }

        
        private void usbDetectionThread()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += (s2, e2) =>
                {
                    this.usbDetector = new USBDetection();
                    this.usbDetector.addBackgroundWorker(bgWorker);
                    this.usbDetector.runDetection(this.usbDetector);
                    
                };
            bgWorker.ProgressChanged += (s2, e2) =>
                {
                    if (e2.ProgressPercentage == 0)
                    {
                        List<string> drives = usbDetector.GetDrives();
                        
                        Control.USBPlugIn(drives);

                        getIncompletedJobList(drives);
                        UpdateJobList();
                        
                    }
                    else
                    {
                        string[] logicDrives = System.Environment.GetLogicalDrives();
                        List<string> drives = new List<string>();
                        foreach (string drive in logicDrives)
                            drives.Add(drive);
                        Control.USBRemoved(logicDrives);
                        getIncompletedJobList(drives);
                        UpdateJobList();
                    }
                };

            bgWorker.RunWorkerAsync();
            
        }

        private void getIncompletedJobList(List<string> drives)
        {
            List<USBJob> incompleteJobList = Control.AcceptJob(drives);
            if (incompleteJobList.Count != 0)
            {
                UpdateJobList();
                JobList.SelectedIndex = JobList.Items.Count - 1;
                MainFrameInfor.Visibility = Visibility.Hidden;
                MainFrameGridButtons.Visibility = Visibility.Hidden;
                AcceptFrameInfor.Visibility = Visibility.Visible;
                AcceptFrameGridButtons.Visibility = Visibility.Visible;

            }
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
            Analyser.Visibility = Visibility.Visible;
            AnalyseFrameGridButtons.Visibility = Visibility.Visible;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            MainFrameInfor.Visibility = Visibility.Hidden;
            CompareResultLeft.Items.Clear();
            CompareResultRight.Items.Clear();
           
            if(JobList.SelectedIndex!=-1)
                cmpJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
            result = Control.Compare(cmpJob);
            Differences usbDifferences = result.USBDifferences;
            Differences pcDifferences = result.PCDifferences;
            CompareLogic compareLogic = new CompareLogic();
            FolderMeta root = cmpJob.FolderInfo;
            backgroundWorker1 = new BackgroundWorker();
            InitializeBackgroundWorker1();

            if (root == null)
            {
                root = ReadAndWrite.BuildTree(cmpJob.PCPath);
            }
            FolderMeta rootCopy1 = new FolderMeta(root);
            compareLogic.ConvertDifferencesToTreeStructure(rootCopy1, pcDifferences);
            TreeViewItem jobTreeView2 = new TreeViewItem();
            jobTreeView2.Header = cmpJob.JobName + " PC Differences";
            DrawTree(rootCopy1, jobTreeView2);
            CompareResultLeft.Items.Add(jobTreeView2);
            FolderMeta rootCopy = new FolderMeta(root);
            compareLogic.ConvertDifferencesToTreeStructure(rootCopy, usbDifferences);
            TreeViewItem jobTreeView = new TreeViewItem();
            jobTreeView.Header = cmpJob.JobName + " USB Differences";
            DrawTree(rootCopy, jobTreeView);
            CompareResultRight.Items.Add(jobTreeView);
            

            


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateJobList();
            ShowMainFrame();
        }
        
        private void JobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JobList.SelectedIndex != -1)
            {   
                PCJob pcJob;
                USBJob usbJob;
                if (JobList.SelectedIndex<Control.GetPCJobs().Count)
                {
                    ShowMainFrame();
                    pcJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
                    JobNameDisplay.Content = pcJob.JobName;
                    LocalPathDisplay.Content = pcJob.PCPath;
                    USBPathDisplay.Content = pcJob.AbsoluteUSBPath;
                }
                else
                {
                    ShowAcceptFrame();
                    usbJob = (USBJob)Control.IncompleteList.ElementAt(JobList.SelectedIndex-Control.GetPCJobs().Count);
                    AcceptJobNameDisplay.Content = usbJob.JobName;
                    AcceptUSBPathDisplay.Content = usbJob.AbsoluteUSBPath;
                    AcceptRemotePathDisplay.Content = usbJob.PCOnePath;
                }
            }
        }

        private void ShowAcceptFrame()
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Visible;
            AcceptFrameInfor.Visibility = Visibility.Visible;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            Analyser.Visibility = Visibility.Hidden;
            AnalyseFrameGridButtons.Visibility = Visibility.Hidden;
        }

        private void ShowMainFrame()
        {
            MainFrameInfor.Visibility = Visibility.Visible;
            MainFrameGridButtons.Visibility = Visibility.Visible;
            AcceptFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameInfor.Visibility = Visibility.Hidden;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            Analyser.Visibility = Visibility.Hidden;
            AnalyseFrameGridButtons.Visibility = Visibility.Hidden;
        }

        private void ShowNewJobFrame()
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameInfor.Visibility = Visibility.Hidden;
            NewJobFrameGridButtons.Visibility = Visibility.Visible;
            NewJobFrameInfor.Visibility = Visibility.Visible;
            Analyser.Visibility = Visibility.Hidden;
            AnalyseFrameGridButtons.Visibility = Visibility.Hidden;
        }

        private void ShowAnalyseFrame()
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameInfor.Visibility = Visibility.Hidden;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            Analyser.Visibility = Visibility.Visible;
            AnalyseFrameGridButtons.Visibility = Visibility.Visible;
        }

        private void FirstSync_Click(object sender, RoutedEventArgs e)
        {
            if (AttachDropBox1.Text != null && AttachDropBox2.Text != null && NewJobName.Text != null)
            {
                Control.CreateJob(NewJobName.Text, AttachDropBox1.Text, AttachDropBox2.Text);
                UpdateJobList();
            }

           selectedPCJob = (PCJob)Control.GetPCJobs().ElementAt(Control.GetPCJobs().Count-1);
           FirstSyncProgressBar.Visibility = Visibility.Visible;
           BarLabel.Visibility = Visibility.Visible;
           FirstSyncProgressBar.Value = 0;
           backgroundWorker2.RunWorkerAsync();
           
           
        }

        private void UpdateJobList()
        {
            JobList.Items.Clear();
            foreach (PCJob Job in Control.GetPCJobs())
            {
                Canvas tag = new Canvas();
                tag.Width = 15;
                tag.Height = 15;
                StackPanel pic = new StackPanel();
                pic.Height = 30;
                pic.Width = 230;
                pic.Orientation = Orientation.Horizontal;
                Label name = new Label();
                name.Height = 30;
                name.Width = 200;
                name.Content = Job.JobName;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                name.VerticalContentAlignment = VerticalAlignment.Center;
                name.HorizontalContentAlignment = HorizontalAlignment.Left;
                if (Job.JobState.Equals(JobStatus.Complete))
                {
                    tag.Background = Brushes.Green;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (Job.JobState.Equals(JobStatus.Incomplete))
                {
                    tag.Background = Brushes.Red;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (Job.JobState.Equals(JobStatus.NotReady))
                {
                    tag.Background = Brushes.Yellow;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }
            }

            foreach (USBJob usbJob in Control.IncompleteList)
            {
                Canvas tag = new Canvas();
                tag.Width = 15;
                tag.Height = 15;
                StackPanel pic = new StackPanel();
                pic.Height = 30;
                pic.Width = 230;
                pic.Orientation = Orientation.Horizontal;
                Label name = new Label();
                name.Height = 30;
                name.Width = 200;
                name.Content = usbJob.JobName;
                //name.FontSize = 12;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                name.VerticalContentAlignment = VerticalAlignment.Center;
                name.HorizontalContentAlignment = HorizontalAlignment.Left;
                if (usbJob.JobState.Equals(JobStatus.Complete))
                {
                    tag.Background = Brushes.Green;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (usbJob.JobState.Equals(JobStatus.Incomplete))
                {
                    tag.Background = Brushes.Red;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (usbJob.JobState.Equals(JobStatus.NotReady))
                {
                    tag.Background = Brushes.Yellow;
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }
            }
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Control.CreateJob((USBJob)(Control.IncompleteList.ElementAt(JobList.SelectedIndex-Control.GetPCJobs().Count)), AttachDropBox.Text);
            MessageBox.Show("Accept Job Finished");
            ShowMainFrame();
            UpdateJobList();
            AttachDropBox.Clear();
            JobList.SelectedIndex = JobList.Items.Count - 1;
            
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minmize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        
        private void DeleteJob_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.DoWork +=
                new DoWorkEventHandler(backgroundWorker2_DoWork);
            backgroundWorker2.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker2_RunWorkerCompleted);
            backgroundWorker2.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker2_ProgressChanged);
        }

        private void backgroundWorker2_DoWork(object sender,
           DoWorkEventArgs e)
        {
    
            BackgroundWorker worker = sender as BackgroundWorker;
            Control.FirstTimeSync(selectedPCJob, worker);
        }
        private void backgroundWorker2_ProgressChanged(object sender,
           ProgressChangedEventArgs e)
        {
            this.FirstSyncProgressBar.Value += (double)e.ProgressPercentage / 100000;
            this.BarLabel.Content = (string)e.UserState;
        }

         private void backgroundWorker2_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            else if (e.Cancelled)
            {
                
            }
            else
            {                
                FirstSyncProgressBar.Value = 100;
                MessageBox.Show("First Time Sync Finished.");
                NewJobFrameInfor.Visibility = Visibility.Hidden;
                NewJobFrameGridButtons.Visibility = Visibility.Hidden;
                MainFrameGridButtons.Visibility = Visibility.Visible;
                MainFrameInfor.Visibility = Visibility.Visible;
                AttachDropBox1.Clear();
                AttachDropBox2.Clear();
                NewJobName.Clear();
                FirstSyncProgressBar.Visibility = Visibility.Hidden;
                BarLabel.Visibility = Visibility.Hidden;
                JobList.SelectedIndex = JobList.Items.Count - 1;
            }

            
        }

         private void Cancel_Click(object sender, RoutedEventArgs e)
         {
             MainFrameGridButtons.Visibility = Visibility.Visible;
             MainFrameInfor.Visibility = Visibility.Visible;
             Analyser.Visibility = Visibility.Hidden;
             AnalyseFrameGridButtons.Visibility = Visibility.Hidden;
         }

         private void RemoveJob_Click(object sender, RoutedEventArgs e)
         {
             Control.DeleteJob((PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex));
             JobList.Items.RemoveAt(JobList.SelectedIndex);
         }

         private void InitializeBackgroundWorker1()
         {
             backgroundWorker1.WorkerReportsProgress = true;
             backgroundWorker1.DoWork +=
                 new DoWorkEventHandler(backgroundWorker1_DoWork);
             backgroundWorker1.RunWorkerCompleted +=
                 new RunWorkerCompletedEventHandler(
             backgroundWorker1_RunWorkerCompleted);
             backgroundWorker1.ProgressChanged +=
                 new ProgressChangedEventHandler(
             backgroundWorker1_ProgressChanged);
         }

         private void backgroundWorker1_DoWork(object sender,
             DoWorkEventArgs e)
         {
             BackgroundWorker worker = sender as BackgroundWorker;
             Control.CleanSync(result, cmpJob, worker);
         }

         private void backgroundWorker1_ProgressChanged(object sender,
             ProgressChangedEventArgs e)
         {
             SyncProgressBar.Value += (double)e.ProgressPercentage/100000;
             SyncBarLabel.Content = (string)e.UserState;
         }

         private void backgroundWorker1_RunWorkerCompleted(
             object sender, RunWorkerCompletedEventArgs e)
         {
             if (e.Error != null)
             {
                 MessageBox.Show(e.Error.Message);
             }
             else if (e.Cancelled)
             {

             }
             else
             {
                 SyncProgressBar.Value = 100;
                 MessageBox.Show("Clean Sync Finished.");
                 SyncBarLabel.Visibility = Visibility.Hidden;
                 SyncProgressBar.Visibility = Visibility.Hidden;
                 MainFrameGridButtons.Visibility = Visibility.Visible;
                 MainFrameInfor.Visibility = Visibility.Visible;
                 AnalyseFrameGridButtons.Visibility = Visibility.Hidden;
                 Analyser.Visibility = Visibility.Hidden;
             }


         }

         private void DrawTree(FolderMeta root, TreeViewItem parent)
         {
             if (root.folders != null)
             {
                 foreach (FolderMeta folder in root.folders)
                 {
                     TreeViewItem folderTreeView = new TreeViewItem();
                     folderTreeView.Header = "[" + folder.FolderType + "]" + folder.Name;
                     parent.Items.Add(folderTreeView);
                     DrawTree(folder, folderTreeView);
                 }
             }
             if (root.files != null)
             {
                 foreach (FileMeta file in root.files)
                 {
                     TreeViewItem fileTreeView = new TreeViewItem();
                     fileTreeView.Header = "[" + file.FileType + "]" + file.Name;
                     parent.Items.Add(fileTreeView);
                 }
             }
         }

         private void AnalyseStartSync_Click(object sender, RoutedEventArgs e)
         {
             SyncProgressBar.Visibility = Visibility.Visible;
             SyncBarLabel.Visibility = Visibility.Visible;             
             backgroundWorker1.RunWorkerAsync();
         }

    }
}
