/***********************************************************************
 * 
 * ******************CleanSync Version 2.0 CleanSyncGUI*****************
 * 
 * Written By : Gu Yang & Li Yi
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
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
using System.Windows.Interop;
using System.Windows.Media.Animation;


namespace CleanSync
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GUI : Window
    {
        #region Paremeters Declaration
        private MainLogic Control;
        private BackgroundWorker NewJobSyncProc; 
        private BackgroundWorker AnalyseSyncProc;
        private PCJob cmpJob;
        private USBDetection usbDetector;
        private List<PCJob> GUIJobList;
        private BackgroundWorker RemovableDiskDetect;
        private List<PCJob> autosyncedJob;
        private System.Threading.Semaphore detectionSemaphore;
        private System.Threading.Semaphore syncSemaphore;
        private bool conflictFlag;
        private delegate void NoArgDelegate();        
        private struct SyncJobInfo
        {
            public ComparisonResult Result
            {
                set;
                get;
            }
            public PCJob CmpJob
            {
                set;
                get;
            }
        }
        private SyncJobInfo syncJobInfo;
        
        
        #endregion

        #region Window Floating
        public enum HitTest : int
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }

        private const int WM_NCHITTEST = 0x0084;
        private readonly int agWidth = 12; // corner width
        private readonly int bThickness = 4; // border width
        private Point mousePoint = new Point(); //mouse cordinates   

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }

        
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCHITTEST:
                    this.mousePoint.X = (lParam.ToInt32() & 0xFFFF);
                    this.mousePoint.Y = (lParam.ToInt32() >> 16);



                    // top left corner
                    if (this.mousePoint.Y - this.Top <= this.agWidth
                       && this.mousePoint.X - this.Left <= this.agWidth)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOPLEFT);
                    }
                    // bottom left corner   
                    else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth
                       && this.mousePoint.X - this.Left <= this.agWidth)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOMLEFT);
                    }
                    // top right corner
                    else if (this.mousePoint.Y - this.Top <= this.agWidth
                       && this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOPRIGHT);
                    }
                    // bottom right corner
                    else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth
                       && this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
                    }
                    // window left
                    else if (this.mousePoint.X - this.Left <= this.bThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTLEFT);
                    }
                    // window right
                    else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.bThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTRIGHT);
                    }
                    // window top
                    else if (this.mousePoint.Y - this.Top <= this.bThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOP);
                    }
                    // window bottom 
                    else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.bThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOM);
                    }
                    else if(this.mousePoint.Y>this.Top && this.mousePoint.Y < this.Top + 40 && this.mousePoint.X > this.Left && this.mousePoint.X < this.Left + 700)// 窗口移动   
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTCAPTION);
                    }
                    break;
            }
            return IntPtr.Zero;
        }
        #endregion

        #region Initializaion

        public GUI()
        {
            
            
            InitializeComponent();
            
            Control = new MainLogic();
            try
            {
                Control.InitializePCJobInfo();
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to Load Computer Job "+System.IO.Path.GetFileNameWithoutExtension(e.Message)+". The Job File is Deleted.Please Restart CleanSync. We Are Sorry About the Inconvenience.");
            }
            GUIJobList = Control.GetPCJobs();
            autosyncedJob = new List<PCJob>();

            string[] logicDrives = System.Environment.GetLogicalDrives();
            List<string> drives = new List<string>();
            foreach (string drive in logicDrives)
            {
                drives.Add(drive);
            }
            try
            {
                List<USBJob> incompleteJobList = Control.USBPlugIn(drives);
                UpdateIncompletedJobList(incompleteJobList);
            }           
            catch(Exception e)
            {
                MessageBox.Show("Failed to Load Computer Job " + System.IO.Path.GetFileNameWithoutExtension(e.Message) + ". The Job File is Deleted.Please Restart CleanSync. We Are Sorry About the Inconvenience.");
                
            }
            Control.CheckJobStatus();            
            this.usbDetector = new USBDetection();
            usbDetectionThread();
            syncJobInfo = new SyncJobInfo();            
            NewJobSyncProc = new BackgroundWorker();
            InitializeBackgroundWorker();
            detectionSemaphore = new Semaphore(1, 1);
            syncSemaphore = new Semaphore(1, 1);
            conflictFlag = false;
        }
        #endregion

        #region USB Detection Background Thread
        
        private void usbDetectionThread()
        {
            RemovableDiskDetect = new BackgroundWorker();
            RemovableDiskDetect.WorkerReportsProgress = true;
            RemovableDiskDetect.WorkerSupportsCancellation = true;
            RemovableDiskDetect.DoWork += (s2, e2) =>
            {
                usbDetector.addBackgroundWorker(RemovableDiskDetect);
                usbDetector.runDetection(usbDetector);
            };
            RemovableDiskDetect.ProgressChanged += (s2, e2) =>
            {
                detectionSemaphore.WaitOne();
                usbDetector.SetDrives();
                if (e2.ProgressPercentage == 0)
                {
                    List<string> drives = usbDetector.GetDrives();
                    try
                    {
                        List<USBJob> incomplete = Control.USBPlugIn(drives);
                        AutoSync();

                        UpdateIncompletedJobList(incomplete);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Failed to Load Computer Job " + System.IO.Path.GetFileNameWithoutExtension(e.Message) + ". The Job File is Deleted.Please Restart CleanSync. We Are Sorry About the Inconvenience.");
                        this.Close();
                    }

                    UpdateJobList();
                    JobList.SelectedIndex = JobList.Items.Count - 1;
                }
                else
                {
                    string[] logicDrives = System.Environment.GetLogicalDrives();
                    List<string> drives = new List<string>();
                    foreach (string drive in logicDrives)
                        drives.Add(drive);
                    Control.USBRemoved(logicDrives);
                    UpdateIncompletedJobList(Control.USBPlugIn(drives));
                    UpdateJobList();
                    JobList.SelectedIndex = JobList.Items.Count - 1;
                    for (int i = 0; i < autosyncedJob.Count; i++)
                    {
                        if (!autosyncedJob[i].JobState.Equals(JobStatus.Complete))
                        {
                            autosyncedJob.RemoveAt(i);
                            i--;
                        }
                    }
                    if (AcceptFrame.IsVisible)
                    {
                        ShowMainFrame();
                    }
                    
                }
                detectionSemaphore.Release();
            };
            RemovableDiskDetect.RunWorkerAsync();
        }
        #endregion

        #region Analyse TreeView Display
        private void DisplayDifferencesTree(bool resync)
        {
            Differences usbDifferences = syncJobInfo.Result.USBDifferences;
            Differences pcDifferences = syncJobInfo.Result.PCDifferences;
            CompareLogic compareLogic = new CompareLogic();
            FolderMeta root = syncJobInfo.CmpJob.FolderInfo;
            if (root == null)
            {
                root = new FolderMeta(syncJobInfo.CmpJob.PCPath, syncJobInfo.CmpJob.PCPath);
            }
            FolderMeta LeftRoot = new FolderMeta(root);
            DifferenceToTreeConvertor convertOne = new DifferenceToTreeConvertor();
            LeftRoot = convertOne.ConvertDifferencesToTreeStructure(pcDifferences);
            TreeViewItem LeftTreeRoot = new TreeViewItem();
            LeftTreeRoot.Foreground = Brushes.WhiteSmoke;
            Label LeftTreeRootHeader = new Label();
            LeftTreeRootHeader.Height = 30;
            LeftTreeRootHeader.Foreground = Brushes.WhiteSmoke;
            LeftTreeRootHeader.Content = "Current Job: " + syncJobInfo.CmpJob.JobName + " (Changes Made On Computer)";
            LeftTreeRoot.Header = LeftTreeRootHeader;
            try
            {
                DrawTree(LeftRoot, LeftTreeRoot);
            }
            catch (UnauthorizedAccessException e)
            {
               MessageBox.Show(e.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.Message);
                return;
            }

            catch (ArgumentOutOfRangeException e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.Close();
            }
            CompareResultLeft.Items.Add(LeftTreeRoot);
            if (!resync)
            {
                FolderMeta RightRoot = new FolderMeta(root);
                DifferenceToTreeConvertor convertTwo = new DifferenceToTreeConvertor();
                RightRoot = convertTwo.ConvertDifferencesToTreeStructure(usbDifferences);
                TreeViewItem RightTreeRoot = new TreeViewItem();
                Label RightTreeRootHeader = new Label();
                RightTreeRootHeader.Height = 30;
                RightTreeRootHeader.Foreground = Brushes.WhiteSmoke;
                RightTreeRootHeader.Content = "Current Job: " + syncJobInfo.CmpJob.JobName + " (Changes Made On Remote Path)";
                RightTreeRoot.Foreground = Brushes.WhiteSmoke;
                RightTreeRoot.Header = RightTreeRootHeader;
                DrawTree(RightRoot, RightTreeRoot);
                CompareResultRight.Items.Add(RightTreeRoot);
            }
            ShowAnalyseFrame();
        }
        #endregion

        #region Analyse Synchronization Background Thread
        private void InitializeCleanSyncProc()
        {
            AnalyseSyncProc.WorkerReportsProgress = true;
            AnalyseSyncProc.WorkerSupportsCancellation = true;
            AnalyseSyncProc.DoWork +=
                new DoWorkEventHandler(AnalyseSyncProc_DoWork);
            AnalyseSyncProc.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            AnalyseSyncProc_RunWorkerCompleted);
            AnalyseSyncProc.ProgressChanged +=
                new ProgressChangedEventHandler(
            AnalyseSyncProc_ProgressChanged);
        }

        private void AnalyseSyncProc_DoWork(object sender,
            DoWorkEventArgs e)
        {
            syncSemaphore.WaitOne();
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                SyncJobInfo jobInfo = (SyncJobInfo)e.Argument;
                Control.CleanSync(jobInfo.Result, jobInfo.CmpJob, worker,e);
                e.Result = jobInfo.CmpJob;
            }
            catch (UnauthorizedAccessException error)
            {
                MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
           
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            finally
            {
                
            }
        }

        private void AnalyseSyncProc_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            AnalyseProgressBar.Value += (double)e.ProgressPercentage / 100000;
            AnalysePBLabel.Content = (string)e.UserState;
        }

        private void AnalyseSyncProc_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                ShowBalloon("Synchronization Cancelled");
            }
            else
            {
                AnalyseProgressBar.Value = 100;
                //AnalyseStartSync.IsEnabled = true;
                //AnalyseCancel.IsEnabled = true;
                if ((ConflictFrame.IsVisible || AnalyseFrame.IsVisible)&& !conflictFlag)
                {
                    ShowMainFrame();
                }
                AnalyseProgressBar.Value = 0;
                AnalyseProgressBarGrid.Visibility = Visibility.Hidden;
                PCJob cmpJob = e.Result as PCJob;
                if (cmpJob != null)
                    cmpJob.Synchronized = Visibility.Visible;
            }
            AnalyseStartSync.IsEnabled = true;
            AnalyseProgressBar.Value = 0;
            AnalyseProgressBarGrid.Visibility = Visibility.Hidden;
            EnableMainButtons();
            syncSemaphore.Release();
        }

        
        #endregion

        #region Create New Job Background Thread
        private void InitializeBackgroundWorker()
        {
            NewJobSyncProc.WorkerReportsProgress = true;
            NewJobSyncProc.WorkerSupportsCancellation = true;
            NewJobSyncProc.DoWork +=
                new DoWorkEventHandler(NewJobSyncProc_DoWork);
            NewJobSyncProc.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            NewJobSyncProc_RunWorkerCompleted);
            NewJobSyncProc.ProgressChanged +=
                new ProgressChangedEventHandler(
            NewJobSyncProc_ProgressChanged);
        }

        private void NewJobSyncProc_DoWork(object sender,
           DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                Control.FirstTimeSync(cmpJob, worker, e);
            }
            catch (UnauthorizedAccessException error)
            {
                
                MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                return;
            }
        }
        private void NewJobSyncProc_ProgressChanged(object sender,
           ProgressChangedEventArgs e)
        {
            NewJobProgressBar.Value += (double)e.ProgressPercentage / 100000;
            NewJobPBLabel.Content = (string)e.UserState;
        }

        private void NewJobSyncProc_RunWorkerCompleted(
           object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                ShowBalloon("First Time Synchronization Cancelled");
            }
            else
            {
                NewJobProgressBar.Value = 100;
                ShowBalloon("First Time Synchroniztion Successful");
            }
            NewJobCreate.IsEnabled = true;
            ShowMainFrame();
            NewJobLocalPath.Clear();
            NewJobRemovablePath.Text = "";
            NewJobName.Clear();
            NewJobProgressBar.Visibility = Visibility.Hidden;
            NewJobPBLabel.Visibility = Visibility.Hidden;
            JobList.SelectedIndex = JobList.Items.Count - 1;
            NewJobProgressBar.Value = 0;
            NewJobName.Clear();
            NewJobLocalPath.Clear();
            RemovableDeviceList.SelectedIndex = -1;
            NewJobConflictRes.SelectedIndex = 0;
            NewJobAutomation.SelectedIndex = 0;

        }
        #endregion

        #region DrawTree Method
        
        private void DrawTree(FolderMeta root, TreeViewItem parent)
        {
            if (root.folders != null)
            {
                foreach (FolderMeta folder in root.folders)
                {
                    if (folder == null)
                        continue;
                    TreeViewItem folderTreeView = new TreeViewItem();
                    
                    Image iconImage = new Image();
                    iconImage.Source = IconExtractor.GetFolderIconImage();
                    iconImage.Height = 20;
                    iconImage.Width = 20;

                    StackPanel pic = new StackPanel();
                    pic.Height = 25;
                    pic.VerticalAlignment = VerticalAlignment.Stretch;
                    pic.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    name.Height = 30;
                    name.Foreground = Brushes.WhiteSmoke;
                    name.Content = folder.Name;

                    Image indicateImage = new Image();
                    indicateImage.Height = 18;
                    indicateImage.Width = 18;
                    BitmapImage temp = new BitmapImage();
                    if (folder.FolderType.Equals(ComponentMeta.Type.New))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Create.png", UriKind.Relative));
                    }
                    else if (folder.FolderType.Equals(ComponentMeta.Type.Modified))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Modify.png", UriKind.Relative));
                    }
                    else if (folder.FolderType.Equals(ComponentMeta.Type.Deleted))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Delete.png", UriKind.Relative));
                    }

                    name.VerticalAlignment = VerticalAlignment.Center;
                    name.HorizontalAlignment = HorizontalAlignment.Left;
                    name.VerticalContentAlignment = VerticalAlignment.Center;
                    name.HorizontalContentAlignment = HorizontalAlignment.Left;

                    pic.Children.Add(indicateImage);
                    pic.Children.Add(iconImage);
                    pic.Children.Add(name);
                    folderTreeView.Header = pic;
                    parent.Items.Add(folderTreeView);
                    try
                    {
                        DrawTree(folder, folderTreeView);
                    }
                    catch (UnauthorizedAccessException error)
                    {
                        MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                        return;
                    }
                    catch (ArgumentNullException error)
                    {
                        MessageBox.Show(error.Message);
                        return;
                    }

                    catch (ArgumentOutOfRangeException error)
                    {
                        MessageBox.Show(error.Message);
                        return;
                    }
                    catch (ArgumentException error)
                    {
                        MessageBox.Show(error.Message);
                        return;
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                        this.Close();
                    }
                }
            }
            if (root.files != null)
            {
                foreach (FileMeta file in root.files)
                {
                    if (file == null)
                        continue;
                    TreeViewItem fileTreeView = new TreeViewItem();

                    Image iconImage = new Image();
                    iconImage.Source = IconExtractor.GetFileIconImage(file.Name);
                    iconImage.Height = 18;
                    iconImage.Width = 18;

                    StackPanel pic = new StackPanel();
                    pic.Height = 25;
                    pic.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    name.Height = 30;
                    name.Foreground = Brushes.WhiteSmoke;
                    name.Content = file.Name;

                    Image indicateImage = new Image();
                    indicateImage.Height = 18;
                    indicateImage.Width = 18;
                    BitmapImage temp = new BitmapImage();
                    if (file.FileType.Equals(ComponentMeta.Type.New))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Create.png", UriKind.Relative));
                    }
                    else if (file.FileType.Equals(ComponentMeta.Type.Modified))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Modify.png", UriKind.Relative));
                    }
                    else if (file.FileType.Equals(ComponentMeta.Type.Deleted))
                    {
                        indicateImage.Source = new BitmapImage(new Uri(@"Pic/Delete.png", UriKind.Relative));
                    }

                    name.VerticalAlignment = VerticalAlignment.Center;
                    name.HorizontalAlignment = HorizontalAlignment.Left;
                    name.VerticalContentAlignment = VerticalAlignment.Center;
                    name.HorizontalContentAlignment = HorizontalAlignment.Left;

                    pic.Children.Add(indicateImage);
                    pic.Children.Add(iconImage);
                    pic.Children.Add(name);

                    fileTreeView.Header = pic;
                    parent.Items.Add(fileTreeView);
                }
            }
        }
        #endregion

        #region Update JobList Methods

        private void UpdateJobList()
        {
            JobList.Items.Clear();
            foreach (PCJob Job in Control.GetPCJobs())
            {
                JobList.Items.Add(Job);
            }

            foreach (USBJob usbJob in Control.IncompleteList)
            {
                JobList.Items.Add(usbJob);
            }
        }
        
        private void UpdateIncompletedJobList(List<USBJob> incomplete)
        {
            if (incomplete.Count != 0)
            {
                UpdateJobList();
                if(!NewJobFrame.IsVisible)
                    JobList.SelectedIndex = JobList.Items.Count - 1;

            }
        }

        #endregion

        #region Window Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Control.CheckJobStatus();
            UpdateJobList();
            JobList.SelectedIndex = JobList.Items.Count - 1;
            RemovableDeviceList.DataContext = usbDetector.usbDriveList;
            AutoSync();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            foreach (PCJob job in Control.GetPCJobs())
            {
                ReadAndWrite.ExportPCJob(job);
            }
            Hide();
            Close();

        }

        private void Minmize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Hide();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process HelpManual = new System.Diagnostics.Process();
                HelpManual.StartInfo.FileName = "CleanSync User Guide.chm";
                HelpManual.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                HelpManual.Start();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot find user guide!");
                return;
            }
            catch (Exception)
            {
                MessageBox.Show("Error loading user guide!");
                return;
            }
        }

        #endregion
        
        #region NotifyIcon Event Handelers
        
        private void CleanSyncNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                Show();
                this.WindowState = WindowState.Normal;
            }

        }

        private void ShowBalloon(string s)
        {
            Balloon InfoBalloon = new Balloon();
            InfoBalloon.BallonContent.Text = s;
            InfoBalloon.BalloonText = "CleanSync";
            CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 2000);
        }

        #endregion

        #region Job List Event Handlers

        private void JobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JobList.SelectedIndex != -1)
            {
                PCJob pcJob;
                USBJob usbJob;

                if (JobList.SelectedIndex < Control.GetPCJobs().Count)
                {
                    if(!NewJobFrame.IsVisible && !ConflictFrame.IsVisible)
                        ShowMainFrame();
                    pcJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
                    ConflictRes.DataContext = pcJob.JobSetting;
                    Automation.DataContext = pcJob.JobSetting;

                    if (pcJob.JobState.Equals(JobStatus.Complete))
                    {
                        if (pcJob.PCPath.Equals(pcJob.GetUsbJob().PCOnePath))
                            MainRemotePath.Text = pcJob.GetUsbJob().PCTwoPath;
                        else
                            MainRemotePath.Text = pcJob.GetUsbJob().PCOnePath;
                        MainRemovablePath.Text = pcJob.AbsoluteUSBPath;
                    }

                    else if (pcJob.JobState.Equals(JobStatus.NotReady))
                    {
                        MainRemotePath.Text = "Removable Device Not Ready";
                        MainRemovablePath.Text = "Removable Device Not Ready";
                    }

                    else
                    {
                        MainRemotePath.Text = "Not Fully Setup";
                        MainRemovablePath.Text = "Not Fully Setup";
                    }

                    MainJobName.Text = pcJob.JobName;
                    MainLocalPath.Text = pcJob.PCPath;

                }
                else
                {
                    if (!NewJobFrame.IsVisible)
                    {
                        ShowAcceptFrame();
                    }
                    usbJob = (USBJob)Control.IncompleteList.ElementAt(JobList.SelectedIndex - Control.GetPCJobs().Count);
                    AcceptJobName.Text = usbJob.JobName;
                    AcceptRemovablePath.Text = usbJob.AbsoluteUSBPath;
                    AcceptRemotePath.Text = usbJob.PCOnePath;
                }
            }
            else
            {
                if (Control.GetPCJobs().Count == 0)
                {
                    ConflictRes.SelectedIndex = -1;
                    Automation.SelectedIndex = -1;
                }
                
                MainRemotePath.Text = "";
                MainRemovablePath.Text = "";
                MainJobName.Text = "";
                MainLocalPath.Text = "";
            }
        }

        private void JobList_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (JobList.SelectedIndex != -1 && JobList.SelectedIndex < Control.GetPCJobs().Count)
            {
                ShowMainFrame();
            }
            else if (JobList.SelectedIndex >= Control.GetPCJobs().Count)
            {
                ShowAcceptFrame();
            }
        }

        #endregion        

        #region Main Frame Event Handlers

        private void NewJob_Click(object sender, RoutedEventArgs e)
        {
            NewJobRemovablePath.Text = (string)RemovableDeviceList.SelectedItem + @"_CleanSync_Data_\" + NewJobName.Text;
            ShowNewJobFrame();
        }

        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            if (JobList.SelectedIndex != -1)
            {
                syncJobInfo.CmpJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
            }
            else
            {
                ShowBalloon("Please select a job");
                return;
            }
            if (syncJobInfo.CmpJob.JobState.Equals(JobStatus.NotReady)||syncJobInfo.CmpJob.JobState.Equals(JobStatus.Incomplete))
            {
                ShowBalloon("Job Not Ready");
                return;
            }
            if (!Directory.Exists(syncJobInfo.CmpJob.PCPath)||!Directory.Exists(syncJobInfo.CmpJob.AbsoluteUSBPath))
            {
                MessageBox.Show("Could Not Find the Specified Path of the Current Job. The Job Will Be Removed.");
                Control.DeleteJob(syncJobInfo.CmpJob);
                UpdateJobList();
                return;
            }
            Cursor = Cursors.Wait;
            CompareResultLeft.Items.Clear();
            CompareResultRight.Items.Clear();
          
            syncJobInfo.Result = Control.Compare(syncJobInfo.CmpJob);
            syncJobInfo.CmpJob.Synchronized = Visibility.Hidden;

            //Auto Conflict Handle
            if (!syncJobInfo.CmpJob.JobSetting.ConflictConfig.Equals(AutoConflictOption.Off))
            {
                syncJobInfo.Result = Control.AutoConflictResolve(syncJobInfo.CmpJob, syncJobInfo.Result);
            }
            try
            {
                PCFreeSpace.Content = "Free : " + Control.GetPCFreeSpace(syncJobInfo.CmpJob);
                RDFreeSpace.Content = "Free : " + Control.GetUSBFreeSpace(syncJobInfo.CmpJob);
                PCRequiredSpace.Content = "Required : " + Control.GetPCRequiredSpace(syncJobInfo.Result);
                RDRequiredSpace.Content = "Required : " + Control.GetUSBRequiredSpace(syncJobInfo.Result);
            }
            catch (Exception err)
            {
                MessageBox.Show("Excpetion When " + err.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }


            if (syncJobInfo.Result.conflictList.Count != 0 && !Control.Resync(syncJobInfo.CmpJob))
            {
                ConflictList.Items.Clear();
                ShowConflictFrame();
                for (int i = 0; i < syncJobInfo.Result.conflictList.Count; i++)
                {
                    ConflictList.Items.Add(syncJobInfo.Result.conflictList.ElementAt(i));
                }
            }
            else
            {

                //NewJob.IsEnabled = false;
                //Refresh(NewJob);
                //RemoveJob.IsEnabled = false;
                //Refresh(RemoveJob);
                //Synchronize.IsEnabled = false;
                //Refresh(Synchronize);
                //Analyse.IsEnabled = false;
                //Refresh(Analyse);
                DisableMainButtons();
                //Refresh(MainFrame);
                Refresh(AnalyseFrame);

                DisplayDifferencesTree(Control.Resync(syncJobInfo.CmpJob));
                ShowAnalyseFrame();
                
            }
            Cursor = Cursors.Arrow;
            EnableMainButtons();
        }

        private void RemoveJob_Click(object sender, RoutedEventArgs e)
        {
            if (JobList.SelectedIndex == -1)
            {
                ShowBalloon("Please select a job");
                return;
            }
            try
            {
                if (MessageBox.Show("About to delete " + Control.GetPCJobs()[JobList.SelectedIndex].JobName + ".", "Deletion Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    Control.DeleteJob((PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex));
                    JobList.Items.RemoveAt(JobList.SelectedIndex);
                }

                else return;

            }
            catch (UnauthorizedAccessException error)
            {
                MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Could not delete an incomplete job");
                return;
            }
            catch (ArgumentException error)
            {
                MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
                this.Close();
            }
        }

        private void Synchronize_Click(object sender, RoutedEventArgs e)
        {
            if (JobList.SelectedIndex != -1)
                syncJobInfo.CmpJob = Control.GetPCJobs()[JobList.SelectedIndex];
            else
            {
                ShowBalloon("Please select a job");
                return;
            }
            if (syncJobInfo.CmpJob.JobState.Equals(JobStatus.NotReady) || syncJobInfo.CmpJob.JobState.Equals(JobStatus.Incomplete))
            {
                ShowBalloon("Job Not Ready");
                return;
            }
            if (!Directory.Exists(syncJobInfo.CmpJob.PCPath) || !Directory.Exists(syncJobInfo.CmpJob.AbsoluteUSBPath))
            {
                MessageBox.Show("Could Not Find the Specified Path of the Current Job. The Job Will Be Removed.");
                Control.DeleteJob(syncJobInfo.CmpJob);
                UpdateJobList();
                return;
            }
            DirectSync();
        }

        #endregion

        #region New Job Frame Event Handelers

        private void NewJobFolderSelection_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            FolderBrowser.ShowDialog();
            NewJobLocalPath.Text = FolderBrowser.SelectedPath;
        }

        private void NewJobCancel_Click(object sender, RoutedEventArgs e)
        {
            if (NewJobSyncProc!=null && NewJobSyncProc.IsBusy)
            {
                if (MessageBox.Show("Cancel Current Job?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (NewJobSyncProc != null && NewJobSyncProc.IsBusy)
                        NewJobSyncProc.CancelAsync();
                }
                return;
            }
            NewJobName.Foreground = Brushes.Black;
            NewJobLocalPath.Foreground = Brushes.Black;
            RemovableInfor.Foreground = Brushes.WhiteSmoke;
            NewJobName.Clear();
            NewJobLocalPath.Clear();
            RemovableDeviceList.SelectedIndex = -1;
            NewJobConflictRes.SelectedIndex = 0;
            NewJobAutomation.SelectedIndex = 0;
            ShowMainFrame();
        }

        private void NewJobCreate_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush myBrusherror = new SolidColorBrush();
            myBrusherror = SetErrorEffect();

            if (Control.ValidatePath(NewJobLocalPath.Text) && RemovableDeviceList.SelectedIndex != -1 && !NewJobName.Text.Equals(string.Empty))
            {
                if (!Control.ValidateJobName(NewJobName.Text))
                {
                    ShowBalloon("Invalid Job Name");
                    return;
                }
                if (usbDetector.GetDrives().Contains(System.IO.Path.GetPathRoot(NewJobLocalPath.Text)))
                {
                    ShowBalloon("Please Select Target Folder on a Local Hard Disk");
                    return;
                }
                if (!Directory.Exists(NewJobLocalPath.Text))
                {
                    ShowBalloon("The Folder Selected Does Not Exist, Please Validate the Path");
                    return;
                }


                NewJobName.Foreground = Brushes.Black;
                NewJobLocalPath.Foreground = Brushes.Black;
                RemovableInfor.Foreground = Brushes.Black;
                MessageBoxResult conf = MessageBox.Show(this, "CleanSync will now synchronize the folder.\n\n" + "Job Name: " + NewJobName.Text + "\nLocal Path: " + NewJobLocalPath.Text + "\nRemovable Path: " + NewJobRemovablePath.Text, "Confirmation", MessageBoxButton.OKCancel);
                if (conf == MessageBoxResult.OK)
                {
                    try
                    {
                        if (!Control.CheckUSBDiskSpace(NewJobLocalPath.Text, (string)RemovableDeviceList.SelectedItem))
                        {
                            ShowBalloon("Not enough space on Removable Device. First Time Synchronization Failed");
                            return;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("First Sync Failed. Please Make Sure You Have Write Control Over The Selected Folder Path.");
                        return;
                    }

                    if (Control.CreateJob(NewJobName.Text, NewJobLocalPath.Text, (string)RemovableDeviceList.SelectedItem, new JobConfig((AutoConflictOption)NewJobConflictRes.SelectedValue, (AutoSyncOption)NewJobAutomation.SelectedValue)) == null)
                    {
                        ShowBalloon(@"There is already a job named """ + NewJobName.Text + @""" existing. Please choose another name.");

                        // Warning Message: Same Job Name
                        return;
                    }

                    UpdateJobList();
                    AcceptLocalPath.Text = string.Empty;

                    cmpJob = (PCJob)Control.GetPCJobs().ElementAt(Control.GetPCJobs().Count - 1);
                    NewJobProgressBar.Visibility = Visibility.Visible;
                    NewJobPBLabel.Visibility = Visibility.Visible;
                    NewJobProgressBar.Value = 0;


                    //Disable buttons
                    NewJobCreate.IsEnabled = false;

                    NewJobSyncProc.RunWorkerAsync(syncJobInfo);

                }
                else
                {
                    return;
                }
            }
            else
            {

                if (NewJobName.Text.Equals(string.Empty))
                    NewJobName.Foreground = myBrusherror;
                else
                    NewJobName.Foreground = Brushes.Black;
                if (!Control.ValidatePath(NewJobLocalPath.Text))
                    NewJobLocalPath.Foreground = myBrusherror;
                else
                    NewJobLocalPath.Foreground = Brushes.Black;
                if (RemovableDeviceList.SelectedIndex == -1)
                    RemovableInfor.Foreground = myBrusherror;
                else
                    NewJobRemovablePath.Foreground = Brushes.Black;
            }
        }

        private void NewJobName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!NewJobName.Text.Equals(string.Empty))
            {
                NewJobName.Foreground = Brushes.Black;
                NewJobRemovablePath.Text = (string)RemovableDeviceList.SelectedItem + @"_CleanSync_Data_\" + NewJobName.Text;
            }
            else
            {
                NewJobRemovablePath.Text = (string)RemovableDeviceList.SelectedItem + @"_CleanSync_Data_\";
            }
        }

        private void NewJobLocalPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!NewJobLocalPath.Text.Equals(string.Empty))
            {
                NewJobLocalPath.Foreground = Brushes.Black;
                NewJobLocalPath.Focus();
                NewJobLocalPath.SelectionStart = NewJobLocalPath.Text.Length;
            }
        }

        private void RemovableDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemovableInfor.Foreground = Brushes.WhiteSmoke;
            NewJobRemovablePath.Text = (string)RemovableDeviceList.SelectedItem + @"_CleanSync_Data_\" + NewJobName.Text;
        }

        private void RemovableDeviceList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (RemovableDeviceList.Items.IsEmpty)
            {
                NoRemDev.Visibility = Visibility.Visible;
            }

        }

        #endregion

        #region Accept Frame Event Handelers

        private void AcceptFolderSelection_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            FolderBrowser.ShowDialog();
            AcceptLocalPath.Text = FolderBrowser.SelectedPath;
        }

        private void AcceptCancel_Click(object sender, RoutedEventArgs e)
        {
            AcceptLocalPath.Foreground = Brushes.Black;
            JobList.SelectedIndex = Control.GetPCJobs().Count - 1;
            AcceptLocalPath.Clear();
            ShowMainFrame();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush myBrusherror = new SolidColorBrush();
            myBrusherror = SetErrorEffect();

            if (!Control.ValidatePath(AcceptLocalPath.Text))
            {
                AcceptLocalPath.Foreground = myBrusherror;
                return;
            }
            else if (usbDetector.GetDrives().Contains(System.IO.Path.GetPathRoot(AcceptLocalPath.Text)))
            {
                ShowBalloon("Please Select Folder on a Local Hard Disk");
                return;
            }
            else
            {
                try
                {
                    AcceptLocalPath.Foreground = Brushes.Black;
                    Control.CreateJob((USBJob)(Control.IncompleteList.ElementAt(JobList.SelectedIndex - Control.GetPCJobs().Count)), AcceptLocalPath.Text);
                }
                catch (UnauthorizedAccessException error)
                {
                    MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                    return;
                }
                catch (ArgumentNullException error)
                {
                    MessageBox.Show(error.Message);
                    return;
                }

                catch (ArgumentOutOfRangeException error)
                {
                    MessageBox.Show(error.Message);
                    return;
                }
                catch (ArgumentException error)
                {
                    MessageBox.Show(error.Message);
                    return;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                    this.Close();
                }
                ShowBalloon("Accept Job Succeed");
                UpdateJobList();
                AcceptLocalPath.Clear();
                JobList.SelectedIndex = JobList.Items.Count - 1;
            }
        }

        private void AcceptLocalPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AcceptLocalPath.Text.Equals(string.Empty))
            {
                AcceptLocalPath.Foreground = Brushes.Black;
                AcceptLocalPath.Focus();
                AcceptLocalPath.SelectionStart = AcceptLocalPath.Text.Length;
            }
        }

        #endregion
        
        #region Analyse Frame Event Handlers

        private void AnalyseStartSync_Click(object sender, RoutedEventArgs e)
        {
            syncJobInfo.CmpJob = (PCJob)Control.GetPCJobs()[JobList.SelectedIndex];

            if (!Directory.Exists(syncJobInfo.CmpJob.AbsoluteUSBPath))
            {
                MessageBox.Show("Removable path cannot be found!");
                ShowMainFrame();
                return;
            }
            else if (!Directory.Exists(syncJobInfo.CmpJob.PCPath))
            {
                MessageBox.Show("Could Not Find the Specified Path of the Current Job. The Job Will Be Removed.");
                Control.DeleteJob(syncJobInfo.CmpJob);
                UpdateJobList();
                return;
            }
            if (!Control.CheckUSBDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob) || !Control.CheckPCDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob))
            {
                ShowBalloon("Not Enough Space On Disk. No Synchronization Is Done.");
                return;
            }
            
            AnalyseProgressBarGrid.Visibility = Visibility.Visible;
            AnalyseStartSync.IsEnabled = false;
            //AnalyseCancel.IsEnabled = false;
            AnalyseSyncProc = new BackgroundWorker();
            InitializeCleanSyncProc();
            AnalyseSyncProc.RunWorkerAsync(syncJobInfo);
        }

        private void AnalyseCancel_Click(object sender, RoutedEventArgs e)
        {
            if (AnalyseSyncProc!=null && AnalyseSyncProc.IsBusy)
            {
                if (MessageBox.Show("Cancel Current Job?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (AnalyseSyncProc != null && AnalyseSyncProc.IsBusy)
                        AnalyseSyncProc.CancelAsync();
                }
                else
                {
                    return;
                }
            }
            CompareResultLeft.Items.Clear();
            CompareResultRight.Items.Clear();
            ShowMainFrame();
        }

        #endregion

        #region Conflict Frame Event Handlers 

        private void ConflictConfirm_Click(object sender, RoutedEventArgs e)
        {
            ConflictCanel.IsEnabled = false;
            Refresh(ConflictCanel);
            Confirm.IsEnabled = false;
            Refresh(Confirm);

            syncJobInfo.Result = Control.HandleConflicts(syncJobInfo.Result);
            if (!conflictFlag)
            {
                DisplayDifferencesTree(false);
            }
            else
            {
                if (!Control.CheckUSBDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob) || !Control.CheckPCDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob))
                {
                    ShowBalloon("Not Enough Space On Disk. No Synchronization Is Done");
                    Synchronize.IsEnabled = true;
                    Analyse.IsEnabled = true;
                    NewJob.IsEnabled = true;
                    RemoveJob.IsEnabled = true;
                    ShowMainFrame();

                    conflictFlag = false;
                    AutoSync();

                    return;
                }

                AnalyseSyncProc = new BackgroundWorker();
                InitializeCleanSyncProc();
                AnalyseSyncProc.RunWorkerAsync(syncJobInfo);
                if (conflictFlag)
                {
                    conflictFlag = false;
                    AutoSync();
                }
            }
        }

        private void ConflictCancel_Click(object sender, RoutedEventArgs e)
        {
            Synchronize.IsEnabled = true;
            Analyse.IsEnabled = true;
            NewJob.IsEnabled = true;
            RemoveJob.IsEnabled = true;
            ShowMainFrame();
            conflictFlag = false;
            AutoSync();
        }

        private void ConflictLeftSelectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                syncJobInfo.Result.conflictList.ElementAt(i).PCSelected = true;
            }
        }

        private void ConflictRightSelectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                syncJobInfo.Result.conflictList.ElementAt(i).USBSelected = true;
            }
        }

        private void ConflictLeftUnselecteAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                syncJobInfo.Result.conflictList.ElementAt(i).PCSelected = false;
            }
        }

        private void ConflictRightUnselectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                syncJobInfo.Result.conflictList.ElementAt(i).USBSelected = false;
            }
        }

        #endregion
        
        #region Synchronization Methods

        private void DirectSync()
        {
            
            if (syncJobInfo.CmpJob.JobState.Equals(JobStatus.NotReady) || syncJobInfo.CmpJob.JobState.Equals(JobStatus.Incomplete))
            {
                ShowBalloon("Job Not Ready");
                return;
            }
            DisableMainButtons();

            syncJobInfo.Result = Control.Compare(syncJobInfo.CmpJob);

            if (syncJobInfo.Result.conflictList.Count != 0 && !Control.Resync(syncJobInfo.CmpJob))
            {
                
                if (syncJobInfo.CmpJob.JobSetting.ConflictConfig.Equals(AutoConflictOption.Off))
                {
                    this.ConflictFrame.Visibility = Visibility.Visible;
                    ShowConflictFrame();
                    conflictFlag = true;

                    ConflictList.Items.Clear();
                    for (int i = 0; i < syncJobInfo.Result.conflictList.Count; i++)
                    {
                        ConflictList.Items.Add(syncJobInfo.Result.conflictList.ElementAt(i));
                    }
                    return;
                }
                else
                {
                    syncJobInfo.Result = Control.AutoConflictResolve(syncJobInfo.CmpJob, syncJobInfo.Result);
                }
            }

            if (!Control.CheckUSBDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob) || !Control.CheckPCDiskSpace(syncJobInfo.Result, syncJobInfo.CmpJob))
            {
                ShowBalloon("Not Enough Space On Disk. No Synchronization Is Done");
                return;
            }

            
            ShowBalloon("Synchronization Job: " + syncJobInfo.CmpJob.JobName + " is working in background");
            AnalyseSyncProc = new BackgroundWorker();
            InitializeCleanSyncProc();
            AnalyseSyncProc.RunWorkerAsync(syncJobInfo);
        }
        
        private void AutoSync()
                {
                    for (int i = 0; i < Control.GetPCJobs().Count; i++ )
                    {
                        PCJob pcJob = Control.GetPCJobs()[i];
                        if (pcJob.JobState.Equals(JobStatus.Complete) && pcJob.JobSetting.SyncConfig.Equals(AutoSyncOption.On))
                        {
                            if (conflictFlag)
                                break;

                            if (autosyncedJob.Contains(pcJob)) continue;
                            syncJobInfo.CmpJob = pcJob;
                            if (!Directory.Exists(syncJobInfo.CmpJob.PCPath) || !Directory.Exists(syncJobInfo.CmpJob.AbsoluteUSBPath))
                            {
                                MessageBox.Show("Could Not Find the Specified Path of the Current Job. The Job Will Be Removed.");
                                Control.DeleteJob(syncJobInfo.CmpJob);
                                UpdateJobList();
                                i--; 
                                continue;
                            }
                            DirectSync();
                            autosyncedJob.Add(pcJob);
                        }
                    }
                }

        #endregion

        #region Frame Switching Methods

        private void ShowAcceptFrame()
        {
            MainFrame.Visibility = Visibility.Hidden;
            NewJobFrame.Visibility = Visibility.Hidden;
            AnalyseFrame.Visibility = Visibility.Hidden;
            ConflictFrame.Visibility = Visibility.Hidden;
            AcceptFrame.Visibility = Visibility.Visible;
            JobListGrid.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;
        }

        private void ShowMainFrame()
        {
            AcceptFrame.Visibility = Visibility.Hidden;
            NewJobFrame.Visibility = Visibility.Hidden;
            AnalyseFrame.Visibility = Visibility.Hidden;
            ConflictFrame.Visibility = Visibility.Hidden;
            MainFrame.Visibility = Visibility.Visible;
            JobListGrid.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;
        }

        private void ShowNewJobFrame()
        {
            MainFrame.Visibility = Visibility.Hidden;
            AcceptFrame.Visibility = Visibility.Hidden;  
            AnalyseFrame.Visibility = Visibility.Hidden;
            ConflictFrame.Visibility = Visibility.Hidden;
            NewJobFrame.Visibility = Visibility.Visible;
            JobListGrid.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;

        }

        private void ShowAnalyseFrame()
        {
            MainFrame.Visibility = Visibility.Hidden;
            AcceptFrame.Visibility = Visibility.Hidden;
            NewJobFrame.Visibility = Visibility.Hidden;
            ConflictFrame.Visibility = Visibility.Hidden;
            JobListGrid.Visibility = Visibility.Hidden;
            JobStateIndicator.Visibility = Visibility.Hidden;
            AnalyseFrame.Visibility = Visibility.Visible;
        }

        private void ShowConflictFrame()
        {
            ConflictCanel.IsEnabled = true;
            Confirm.IsEnabled = true;
            MainFrame.Visibility = Visibility.Hidden;
            AcceptFrame.Visibility = Visibility.Hidden;
            NewJobFrame.Visibility = Visibility.Hidden;
            AnalyseFrame.Visibility = Visibility.Hidden;     
            JobListGrid.Visibility = Visibility.Hidden;
            JobStateIndicator.Visibility = Visibility.Hidden;
            ConflictFrame.Visibility = Visibility.Visible;
        }
        #endregion

        #region General Control Helper Methods

        public static void Refresh(DependencyObject obj)
        {
            obj.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                (NoArgDelegate)delegate { });
        }

        private void EnableMainButtons()
        {
            Synchronize.IsEnabled = true;
            Analyse.IsEnabled = true;
            NewJob.IsEnabled = true;
            RemoveJob.IsEnabled = true;
        }
        private void DisableMainButtons()
        {
            Synchronize.IsEnabled = false;
            Analyse.IsEnabled = false;
            NewJob.IsEnabled = false;
            RemoveJob.IsEnabled = false;
        }

        private SolidColorBrush SetErrorEffect()
        {
            SolidColorBrush myBrush = new SolidColorBrush();
            myBrush.Color = Colors.Blue;

            ColorAnimation myColorAnimation = new ColorAnimation();
            myColorAnimation.From = Colors.White;
            myColorAnimation.To = Colors.Red;
            myColorAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            myColorAnimation.AccelerationRatio = 1;
            myColorAnimation.AutoReverse = true;
            myColorAnimation.RepeatBehavior = new RepeatBehavior(TimeSpan.FromMilliseconds(2500));
            // Apply the animation to the brush's Color property.
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, myColorAnimation);
            return myBrush;
        }

        #endregion

    }
}
