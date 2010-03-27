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
        private MainLogic Control;
        private BackgroundWorker backgroundWorker2;
        private BackgroundWorker backgroundWorker1;
        ComparisonResult result;
        PCJob cmpJob;
        private USBDetection usbDetector;
        private List<PCJob> GUIJobList;
        private BackgroundWorker bgWorker;
        private PCJob selectedPCJob;

        private System.Threading.Semaphore detectionSemaphore;

        #region float window
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

            List<USBJob> incompleteJobList = Control.USBPlugIn(drives);
            UpdateIncompletedJobList(incompleteJobList);
            Control.CheckJobStatus();
            
            this.usbDetector = new USBDetection();
            usbDetectionThread();
            
            backgroundWorker2 = new BackgroundWorker();
            InitializeBackgroundWorker();
            detectionSemaphore = new System.Threading.Semaphore(1, 1);

        }


        #region USB Detection Background Thread
        private void usbDetectionThread()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += (s2, e2) =>
            {
                //this.usbDetector = new USBDetection();
                this.usbDetector.addBackgroundWorker(bgWorker);
                this.usbDetector.runDetection(this.usbDetector);

            };
            bgWorker.ProgressChanged += (s2, e2) =>
            {
                detectionSemaphore.WaitOne();
                usbDetector.SetDrives();

                if (e2.ProgressPercentage == 0)
                {
                    List<string> drives = usbDetector.GetDrives();

                    List<USBJob> incomplete = Control.USBPlugIn(drives);

                    UpdateIncompletedJobList(incomplete);
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

                    if (AcceptFrameInfor.IsVisible)
                    {
                        ShowMainFrame();
                    }
                    
                }
                detectionSemaphore.Release();
            };

            bgWorker.RunWorkerAsync();

        }
        #endregion

        #region Analyse TreeView Display
        private void DisplayDifferencesTree(bool resync)
        {
            Differences usbDifferences = result.USBDifferences;
            Differences pcDifferences = result.PCDifferences;
            CompareLogic compareLogic = new CompareLogic();
            FolderMeta root = cmpJob.FolderInfo;
            backgroundWorker1 = new BackgroundWorker();
            InitializeBackgroundWorker1();

            if (root == null)
            {
                root = new FolderMeta(cmpJob.PCPath, cmpJob.PCPath);
            }
            FolderMeta rootCopy1 = new FolderMeta(root);
            DifferenceToTreeConvertor convertOne = new DifferenceToTreeConvertor();
            rootCopy1 = convertOne.ConvertDifferencesToTreeStructure(pcDifferences);
            TreeViewItem jobTreeView2 = new TreeViewItem();
            jobTreeView2.Header = "Current Job: " + cmpJob.JobName + " (Changes Made On Computer)";
            try
            {
                DrawTree(rootCopy1, jobTreeView2);
            }
            catch (UnauthorizedAccessException e)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Message + "\nPlease Make Sure You Have Write Control Over The Disks.";
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException e)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Message);
                return;
            }

            catch (ArgumentOutOfRangeException e)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Message);
                return;
            }
            catch (ArgumentException e)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Message);
                return;
            }
            catch (Exception e)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Message);
                this.Close();
            }
            CompareResultLeft.Items.Add(jobTreeView2);

            if (!resync)
            {
                FolderMeta rootCopy = new FolderMeta(root);
                DifferenceToTreeConvertor convertTwo = new DifferenceToTreeConvertor();
                rootCopy = convertTwo.ConvertDifferencesToTreeStructure(usbDifferences);
                TreeViewItem jobTreeView = new TreeViewItem();
                jobTreeView.Header = "Current Job: " + cmpJob.JobName + " (Changes Made On Removable Device)";
                DrawTree(rootCopy, jobTreeView);
                CompareResultRight.Items.Add(jobTreeView);
            }

            ShowAnalyseFrame();
        }

        #endregion

        #region Analyse Sync Background Thread
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
            try
            {
                Control.CleanSync(result, cmpJob, worker);
            }
            catch (UnauthorizedAccessException error)
            {
                //Balloon InfoBalloon = new Balloon();
                //InfoBalloon.BallonContent.Text = error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.";
                //InfoBalloon.BalloonText = "CleanSync";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                //Balloon InfoBalloon = new Balloon();
                //InfoBalloon.BallonContent.Text = error.Message;
                //InfoBalloon.BalloonText = "CleanSync";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException error)
            {
                //Balloon InfoBalloon = new Balloon();
                //InfoBalloon.BallonContent.Text = error.Message;
                //InfoBalloon.BalloonText = "CleanSync";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                //Balloon InfoBalloon = new Balloon();
                //InfoBalloon.BallonContent.Text = error.Message;
                //InfoBalloon.BalloonText = "CleanSync";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                //Balloon InfoBalloon = new Balloon();
                //InfoBalloon.BallonContent.Text = error.Message;
                //InfoBalloon.BalloonText = "CleanSync";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                MessageBox.Show(error.Message);
                //this.Close();
                return;
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            SyncProgressBar.Value += (double)e.ProgressPercentage / 100000;
            SyncBarLabel.Content = (string)e.UserState;
        }

        private void backgroundWorker1_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {

            }
            else
            {
                SyncProgressBar.Value = 100;
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BalloonText = "CleanSync";
                InfoBalloon.BallonContent.Text = "Clean Synchroniztion Succeeds.";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                AnalyseStartSync.IsEnabled = true;
                AnalyseCancel.IsEnabled = true;
                ShowMainFrame();
            }
        }
        #endregion

        #region First Time Sync Background Thread
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
            try
            {
                Control.FirstTimeSync(selectedPCJob, worker);
            }
            catch (UnauthorizedAccessException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                //this.Close();
                return;
            }
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
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = e.Error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(e.Error.Message);
            }

            else if (e.Cancelled)
            {

            }
            else
            {
                FirstSyncProgressBar.Value = 100;
                //MessageBox.Show("First Time Sync Finished.");
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = "First Time Synchroniztion Succeeds.";
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                FirstSync.IsEnabled = true;
                CancelCreat.IsEnabled = true;
                ShowMainFrame();
                AttachDropBox1.Clear();
                AttachDropBox2.Clear();
                NewJobName.Clear();
                FirstSyncProgressBar.Visibility = Visibility.Hidden;
                BarLabel.Visibility = Visibility.Hidden;
                JobList.SelectedIndex = JobList.Items.Count - 1;
            }
        }
        #endregion

        #region DrawTree Helper Method
        private void DrawTree(FolderMeta root, TreeViewItem parent)
        {
            if (root.folders != null)
            {
                foreach (FolderMeta folder in root.folders)
                {
                    TreeViewItem folderTreeView = new TreeViewItem();

                    Image iconImage = new Image();
                    iconImage.Source = IconExtractor.GetFolderIconImage();
                    iconImage.Height = 20;
                    iconImage.Width = 20;

                    StackPanel pic = new StackPanel();
                    pic.Height = 25;
                    //pic.Width = 202;
                    pic.VerticalAlignment = VerticalAlignment.Stretch;
                    pic.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    name.Height = 25;
                    //name.Width = 200;
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
                        Balloon InfoBalloon = new Balloon();
                        InfoBalloon.BallonContent.Text = error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.";
                        InfoBalloon.BalloonText = "CleanSync";
                        CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                        //MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                        return;
                    }
                    catch (ArgumentNullException error)
                    {
                        Balloon InfoBalloon = new Balloon();
                        InfoBalloon.BallonContent.Text = error.Message;
                        InfoBalloon.BalloonText = "CleanSync";
                        CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                        //MessageBox.Show(error.Message);
                        return;
                    }

                    catch (ArgumentOutOfRangeException error)
                    {
                        Balloon InfoBalloon = new Balloon();
                        InfoBalloon.BallonContent.Text = error.Message;
                        InfoBalloon.BalloonText = "CleanSync";
                        CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                        //MessageBox.Show(error.Message);
                        return;
                    }
                    catch (ArgumentException error)
                    {
                        Balloon InfoBalloon = new Balloon();
                        InfoBalloon.BallonContent.Text = error.Message;
                        InfoBalloon.BalloonText = "CleanSync";
                        CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                        //MessageBox.Show(error.Message);
                        return;
                    }
                    catch (Exception error)
                    {
                        Balloon InfoBalloon = new Balloon();
                        InfoBalloon.BallonContent.Text = error.Message;
                        InfoBalloon.BalloonText = "CleanSync";
                        CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                        //MessageBox.Show(error.Message);
                        this.Close();
                    }
                }
            }
            if (root.files != null)
            {
                foreach (FileMeta file in root.files)
                {
                    TreeViewItem fileTreeView = new TreeViewItem();

                    Image iconImage = new Image();
                    iconImage.Source = IconExtractor.GetFileIconImage(file.Name);
                    iconImage.Height = 18;
                    iconImage.Width = 18;

                    StackPanel pic = new StackPanel();
                    pic.Height = 25;
                    //pic.Width = 210;
                    pic.Orientation = Orientation.Horizontal;
                    Label name = new Label();
                    name.Height = 25;
                    //name.Width = 200;
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
                        //indicateImage.Source = new BitmapImage(new Uri(@"pic/Modify.png", UriKind.Relative));
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

        #region Frame Control Helper Methods
        private void ShowAcceptFrame()
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Visible;
            AcceptFrameInfor.Visibility = Visibility.Visible;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            Analyser.Visibility = Visibility.Hidden;
            AnalyseButtons.Visibility = Visibility.Hidden;
            AnalyseProgressBar.Visibility = Visibility.Hidden;
            Conflict.Visibility = Visibility.Hidden;
            ConflictButtons.Visibility = Visibility.Hidden;
            JobListHost.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;
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
            AnalyseButtons.Visibility = Visibility.Hidden;
            AnalyseProgressBar.Visibility = Visibility.Hidden;
            Conflict.Visibility = Visibility.Hidden;
            ConflictButtons.Visibility = Visibility.Hidden;
            JobListHost.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;
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
            AnalyseButtons.Visibility = Visibility.Hidden;
            AnalyseProgressBar.Visibility = Visibility.Hidden;
            Conflict.Visibility = Visibility.Hidden;
            ConflictButtons.Visibility = Visibility.Hidden;
            JobListHost.Visibility = Visibility.Visible;
            JobStateIndicator.Visibility = Visibility.Visible;

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
            AnalyseButtons.Visibility = Visibility.Visible;
            AnalyseProgressBar.Visibility = Visibility.Hidden;
            Conflict.Visibility = Visibility.Hidden;
            ConflictButtons.Visibility = Visibility.Hidden;
            JobListHost.Visibility = Visibility.Hidden;
            JobStateIndicator.Visibility = Visibility.Hidden;
        }

        private void ShowConflictFrame()
        {
            MainFrameInfor.Visibility = Visibility.Hidden;
            MainFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameGridButtons.Visibility = Visibility.Hidden;
            AcceptFrameInfor.Visibility = Visibility.Hidden;
            NewJobFrameGridButtons.Visibility = Visibility.Hidden;
            NewJobFrameInfor.Visibility = Visibility.Hidden;
            Analyser.Visibility = Visibility.Hidden;
            AnalyseButtons.Visibility = Visibility.Hidden;
            AnalyseProgressBar.Visibility = Visibility.Hidden;
            Conflict.Visibility = Visibility.Visible;
            ConflictButtons.Visibility = Visibility.Visible;
            JobListHost.Visibility = Visibility.Hidden;
            JobStateIndicator.Visibility = Visibility.Hidden;
        }
        #endregion

        #region Update JobList Helper Method
        private void UpdateJobList()
        {
            JobList.Items.Clear();
            foreach (PCJob Job in Control.GetPCJobs())
            {
                System.Windows.Controls.Image tag = new System.Windows.Controls.Image();
                tag.Width = 20;
                tag.Height = 20;
                StackPanel pic = new StackPanel();
                pic.Height = 30;
                pic.Width = 180;
                pic.Orientation = Orientation.Horizontal;
                Label name = new Label();
                name.Height = 30;
                name.Width = 150;
                name.Content = Job.JobName;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                name.VerticalContentAlignment = VerticalAlignment.Center;
                name.HorizontalContentAlignment = HorizontalAlignment.Left;
                if (Job.JobState.Equals(JobStatus.Complete))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/green.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (Job.JobState.Equals(JobStatus.Incomplete))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/red.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (Job.JobState.Equals(JobStatus.NotReady))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/yellow.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }
            }

            foreach (USBJob usbJob in Control.IncompleteList)
            {
                Image tag = new Image();
                tag.Width = 20;
                tag.Height = 20;
                StackPanel pic = new StackPanel();
                pic.Height = 30;
                pic.Width = 180;
                pic.Orientation = Orientation.Horizontal;
                Label name = new Label();
                name.Height = 30;
                name.Width = 150;
                name.Content = usbJob.JobName;
                //name.FontSize = 12;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                name.VerticalContentAlignment = VerticalAlignment.Center;
                name.HorizontalContentAlignment = HorizontalAlignment.Left;
                if (usbJob.JobState.Equals(JobStatus.Complete))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/green.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (usbJob.JobState.Equals(JobStatus.Incomplete))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/red.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }

                if (usbJob.JobState.Equals(JobStatus.NotReady))
                {
                    tag.Source = new BitmapImage(new Uri(@"Pic/yellow.png", UriKind.Relative));
                    pic.Children.Add(name);
                    pic.Children.Add(tag);
                    JobList.Items.Add(pic);
                }
            }
        }
        #endregion

        #region Updaet Incomplete JobList Helper Method
        private void UpdateIncompletedJobList(List<USBJob> incomplete)
        {
            if (incomplete.Count != 0)
            {
                UpdateJobList();
                //ShowAcceptFrame();
                JobList.SelectedIndex = JobList.Items.Count - 1;

            }
        }
        #endregion

        #region GUI Component Event Handelers
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
            if (JobList.SelectedIndex != -1)
                cmpJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
            else
            {
                //Status.Content = "No Job Selected";
                return;
            }
            if (cmpJob.JobState.Equals(JobStatus.NotReady)||cmpJob.JobState.Equals(JobStatus.Incomplete))
            {
                //Status.Content = "Job Not Ready";
                return;
            }


            ShowAnalyseFrame();
            CompareResultLeft.Items.Clear();
            CompareResultRight.Items.Clear();
            //modify
            
           
            
            result = Control.Compare(cmpJob);
            PCFreeSpace.Content = "Free : " + Control.GetPCFreeSpace(cmpJob);
            RDFreeSpace.Content = "Free : " + Control.GetUSBFreeSpace(cmpJob);
            PCRequiredSpace.Content = "Required : " + Control.GetPCRequiredSpace(result);
            RDRequiredSpace.Content = "Required : " + Control.GetUSBRequiredSpace(result);


            if (result.conflictList.Count != 0 && !Control.Resync(cmpJob))
            {
                //ConflictPanel.Visibility = Visibility.Visible;
                ShowConflictFrame();

                ConflictList.Items.Clear();
                for (int i = 0; i < result.conflictList.Count; i++)
                {
                     ConflictList.Items.Add(result.conflictList.ElementAt(i));
                }
            }
            else
            {
                //ShowAnalyseFrame();
                DisplayDifferencesTree(Control.Resync(cmpJob));
            }
        }

        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Control.CheckJobStatus();
            UpdateJobList();
            ShowMainFrame();
            JobList.SelectedIndex = JobList.Items.Count - 1;
            FolderSelection2.DataContext = usbDetector.usbDriveList;
            
        }

        private void JobList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (JobList.SelectedIndex != -1)
            {
                PCJob pcJob;
                USBJob usbJob;
                if (JobList.SelectedIndex < Control.GetPCJobs().Count)
                {

                    //ShowMainFrame();
                    pcJob = (PCJob)Control.GetPCJobs().ElementAt(JobList.SelectedIndex);
                    if (pcJob.JobState.Equals(JobStatus.Complete))
                    {
                        if (pcJob.PCPath.Equals(pcJob.GetUsbJob().PCOnePath))
                            RemotePathDisplay.Content = pcJob.GetUsbJob().PCTwoPath;
                        else
                            RemotePathDisplay.Content = pcJob.GetUsbJob().PCOnePath;
                        USBPathDisplay.Content = pcJob.AbsoluteUSBPath;
                    }

                    else if (pcJob.JobState.Equals(JobStatus.NotReady))
                    {
                        RemotePathDisplay.Content = "Removable Device Not Ready";
                        USBPathDisplay.Content = "Removable Device Not Ready";
                    }

                    else
                    {
                        RemotePathDisplay.Content = "Not Fully Setup";
                        USBPathDisplay.Content = "Not Fully Setup";
                    }

                    JobNameDisplay.Content = pcJob.JobName;
                    LocalPathDisplay.Content = pcJob.PCPath;

                }
                else
                {
                    if (MainFrameInfor.IsVisible)
                    {
                        ShowAcceptFrame();
                    }
                    usbJob = (USBJob)Control.IncompleteList.ElementAt(JobList.SelectedIndex - Control.GetPCJobs().Count);
                    AcceptJobNameDisplay.Content = usbJob.JobName;
                    AcceptUSBPathDisplay.Content = usbJob.AbsoluteUSBPath;
                    AcceptRemotePathDisplay.Content = usbJob.PCOnePath;
                }
            }
            else
            {
                RemotePathDisplay.Content = "";
                USBPathDisplay.Content = "";
                JobNameDisplay.Content = "";
                LocalPathDisplay.Content = "";
            }       
        }


        private void FirstSync_Click(object sender, RoutedEventArgs e)
        {
            if (Control.validate_path(AttachDropBox1.Text) && Control.validate_path(AttachDropBox2.Text) && !NewJobName.Text.Equals(string.Empty))
            {
                Control.CreateJob(NewJobName.Text, AttachDropBox1.Text, System.IO.Path.GetPathRoot(AttachDropBox2.Text));
                UpdateJobList();
                AttachDropBox.Text = string.Empty;

                selectedPCJob = (PCJob)Control.GetPCJobs().ElementAt(Control.GetPCJobs().Count - 1);
                if (!Control.CheckUSBDiskSpace(selectedPCJob))
                {
                    Balloon InfoBalloon = new Balloon();
                    InfoBalloon.BallonContent.Text = "Not enough space on Removable Device. First Time Synchronization Fails";
                    InfoBalloon.BalloonText = "CleanSync";
                    CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                    //MessageBox.Show("Not enough space on Removable Device.First Sync failed.");
                    return;
                }
                FirstSyncProgressBar.Visibility = Visibility.Visible;
                BarLabel.Visibility = Visibility.Visible;
                FirstSyncProgressBar.Value = 0;
                //Balloon InfoBalloonS = new Balloon();
                //InfoBalloonS.BalloonText = "CleanSync";
                //InfoBalloonS.BallonContent.Text = "First Time Synchronization Starts.";
                //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloonS, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                
                //Disable buttons
                FirstSync.IsEnabled = false;
                CancelCreat.IsEnabled = false;
                //JobList.IsEnabled = false;
                backgroundWorker2.RunWorkerAsync();
            }
            else
            {
                //Message
                return;
            }
        }

     

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (!Control.validate_path(AttachDropBox.Text))
            {
                //Show Message
                return;
            }
            try
            {
                Control.CreateJob((USBJob)(Control.IncompleteList.ElementAt(JobList.SelectedIndex - Control.GetPCJobs().Count)), AttachDropBox.Text);
            }
            catch (UnauthorizedAccessException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message+"\nPlease Make Sure You Have Write Control Over The Disks.";
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            
            catch (ArgumentOutOfRangeException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                this.Close();
            }
            Balloon InfoBalloonS = new Balloon();
            InfoBalloonS.BallonContent.Text = "Accept Job Succeed"; ;
            InfoBalloonS.BalloonText = "CleanSync";
            CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloonS, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
            //MessageBox.Show("Accept Job Finished");
            ShowMainFrame();
            UpdateJobList();
            AttachDropBox.Clear();
            JobList.SelectedIndex = JobList.Items.Count - 1;

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Close();
        }

        private void minmize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            Hide();
        }

        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowMainFrame();
        }

        private void RemoveJob_Click(object sender, RoutedEventArgs e)
        {
            if (JobList.SelectedIndex == -1)
            {
                //Status.Content = "No Job Selected";
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
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message + "/nPlease Make Sure You Have Write Control Over The Disks.";
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message + "\nPlease Make Sure You Have Write Control Over The Disks.");
                return;
            }
            catch (ArgumentNullException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }

            catch (ArgumentOutOfRangeException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (ArgumentException error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                return;
            }
            catch (Exception error)
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = error.Message;
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show(error.Message);
                this.Close();
            }
        }



        private void AnalyseStartSync_Click(object sender, RoutedEventArgs e)
        {
            selectedPCJob = (PCJob)Control.GetPCJobs()[JobList.SelectedIndex];
            if (!Control.CheckUSBDiskSpace(result, selectedPCJob) || !Control.CheckPCDiskSpace(result, selectedPCJob))
            {
                Balloon InfoBalloon = new Balloon();
                InfoBalloon.BallonContent.Text = "Not Enough Space On Disk. No Synchronization Is Done.";
                InfoBalloon.BalloonText = "CleanSync";
                CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloon, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
                //MessageBox.Show("Not Enough Space on Disk.No Sync is done.");
                return;
            }
            AnalyseProgressBar.Visibility = Visibility.Visible;
            AnalyseStartSync.IsEnabled = false;
            AnalyseCancel.IsEnabled = false;
            //Balloon InfoBalloonS = new Balloon();
            //InfoBalloonS.BallonContent.Text = "Clean Synchronization Starts";
            //InfoBalloonS.BalloonText = "CleanSync";
            //CleanSyncNotifyIcon.ShowCustomBalloon(InfoBalloonS, System.Windows.Controls.Primitives.PopupAnimation.Slide, 4000);
            backgroundWorker1.RunWorkerAsync();
        }

        private void ConflictConfirm_Click(object sender, RoutedEventArgs e)
        {
            result = Control.HandleConflicts(result);

            DisplayDifferencesTree(false);
        }

        private void ConflictCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowMainFrame();
        }

        private void CleanSyncNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                Show();
                this.WindowState = WindowState.Normal;
            }

        }

        private void FolderSelection2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AttachDropBox2.Text = (string)FolderSelection2.SelectedItem + @"CleanSync\" + NewJobName.Text;
        }

        private void NewJobName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FolderSelection2.SelectedIndex != -1)
            {
                AttachDropBox2.Text = (string)FolderSelection2.SelectedItem + @"CleanSync\" + NewJobName.Text;
            }
        }

        private void ConflictLeftSelectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                result.conflictList.ElementAt(i).PCSelected = true;
            }
        }

        private void ConflictRightSelectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                result.conflictList.ElementAt(i).USBSelected = true;
            }
        }

        private void ConflictLeftUnselecteAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                result.conflictList.ElementAt(i).PCSelected = false;
            }
        }

        private void ConflictRightUnselectAll(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.ConflictList.Items.Count; i++)
            {
                result.conflictList.ElementAt(i).USBSelected = false;
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
    }
}
