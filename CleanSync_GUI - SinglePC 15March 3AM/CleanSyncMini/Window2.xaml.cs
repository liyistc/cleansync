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
using DirectoryInformation;
using System.ComponentModel;


namespace CleanSyncMini
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Analyser : Window
    {
        PCJob ComparedJob;
        MainLogic MainLog;
        ComparisonResult Result;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;

        public Analyser(MainLogic control,PCJob pcJob, ComparisonResult result)
        {
            InitializeComponent();
            //string display="";
        /*    foreach (string part in result.ConvertComparisonResultToListOfString(result))
                display+=part;
        */
            Differences usbDifferences = result.USBDifferences;
            Differences pcDifferences = result.PCDifferences;
            CompareLogic compareLogic = new CompareLogic();
            FolderMeta root = pcJob.FolderInfo;
            if (root == null)
            {
                root = ReadAndWrite.BuildTree(pcJob.PCPath);
                //MessageBox.Show(root.getString());
            }
            FolderMeta rootCopy1 = new FolderMeta(root);
            compareLogic.ConvertDifferencesToTreeStructure(rootCopy1, pcDifferences);
            //MessageBox.Show(rootCopy1.getString());
            TreeViewItem jobTreeView2 = new TreeViewItem();
            jobTreeView2.Header = pcJob.JobName + " PC Differences";
            //treeView1.Items.Add(jobTreeView2);
            DrawTree(rootCopy1, jobTreeView2);

            FolderMeta rootCopy = new FolderMeta(root);
            //MessageBox.Show(rootCopy.getString());
            compareLogic.ConvertDifferencesToTreeStructure(rootCopy, usbDifferences);
            //MessageBox.Show(rootCopy.getString());
            //string usbDiff = root.getString();
            //ResultDisplay.Text = usbDiff + "********************\n"+pcDiff;
            // ResultDisplay.Text += display;
            TreeViewItem jobTreeView = new TreeViewItem();
            jobTreeView.Header = pcJob.JobName + " USB Differences";
            //treeView2.Items.Add(jobTreeView);

            DrawTree(rootCopy, jobTreeView);
           
            ComparedJob = pcJob;
            MainLog = control;
            Result = result;
            backgroundWorker1 = new BackgroundWorker();
            InitializeBackgroundWorker();
        }

        

        private void DrawTree(FolderMeta root,TreeViewItem parent)
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            //MainLog.CleanSync(Result, ComparedJob);
            //MessageBox.Show("Clean Sync Finished.");
            backgroundWorker1.RunWorkerAsync();
            
        }

        private void InitializeBackgroundWorker()
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
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            
            MainLog.CleanSync(Result, ComparedJob, worker);
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            //this.progressBar1.Value += (double)e.ProgressPercentage/100000;
            //this.label1.Content = (string)e.UserState;
        }

        private void backgroundWorker1_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                
            }
            else
            {
                
                MessageBox.Show("Clean Sync Finished.");
            }

            
        }

    }
}
