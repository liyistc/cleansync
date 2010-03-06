using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using CleanSyncMini;

namespace CleanSyncMini
{
    [Serializable]
    class JobLogic
    {
        List<string> StoredPCJobInfoPaths;
        List<string> StoredUSBJobInfoPaths;

        List<PCJob> PCJobs;
        internal List<USBJob> USBJobs;

        CompareLogic compareLogic;

        int num;
      
        public JobLogic(int n)
        {
         
            StoredPCJobInfoPaths = new List<string>();
            StoredUSBJobInfoPaths = new List<string>();
            PCJobs = new List<PCJob>();
            USBJobs = new List<USBJob>();
            compareLogic = new CompareLogic();

            num = n;
        }

        internal void InitializePCJobInfo()
        {
            StoredPCJobInfoPaths =  ReadAndWrite.ImportJobList(num);
            PCJobs = ReadAndWrite.GetPCJobs(StoredPCJobInfoPaths);
        }

        internal PCJob CreateJob(string jobName, string PCPath, string USBPath)
        {
            //CheckNameConflict(jobName);
            
            PCJob pcJob = new PCJob(jobName, PCPath, USBPath);
            
            InsertJob(pcJob);
            
            return pcJob;
        }
        internal PCJob CreateJob(USBJob jobUSB, string PCPath)
        {
            jobUSB.PCTwoPath = PCPath;
            PCJob pcJob = new PCJob(jobUSB,PCPath);
            pcJob.JobState = JobStatus.Complete;
            
            InsertJob(pcJob);            
            
            InsertJob(jobUSB);
            
            ReadAndWrite.RemoveIncompleteUSBJob(jobUSB);
            
            return pcJob;
        }
        internal void CheckNameConflict(string JobName)
        {

        }
        internal void InsertJob(PCJob pcJob)
        {
            PCJobs.Add(pcJob);
            StoredPCJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnPC(pcJob));
            
            //Modified
            ReadAndWrite.ExportPCJobPathsList(StoredPCJobInfoPaths,num);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        internal void InsertJob(USBJob usbJob)
        {
            USBJobs.Add(usbJob);
            StoredUSBJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnUSB(usbJob));
            ReadAndWrite.ExportUSBJobPathsList(StoredUSBJobInfoPaths,usbJob);
            ReadAndWrite.ExportUSBJob(usbJob);
        }
       
        internal bool setupJob(PCJob pcJob)
        {
            FolderMeta folderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            if (folderInfo.Equals(null))
            {
                Console.WriteLine("Cannot build folder meta tree from path:"+pcJob.PCPath);
                return false;
            }

            pcJob.FolderInfo = folderInfo;
                
            ReadAndWrite.ExportPCJob(pcJob);

            USBJob usbJob = new USBJob(pcJob);

            //pcJob.SetUsbJob(usbJob);
       
            usbJob.diff = compareLogic.ConvertFolderMetaToDifferences(pcJob.FolderInfo);

            SyncLogic.SyncPCToUSB(usbJob.diff, pcJob);
            
            WriteIncompleteFileInfoOnUSB(usbJob);
                        
            return true;
            
        }

        private string GetFullFolderPath(PCJob pcJob, FolderMeta folderInfo)
        {
            return pcJob.PCPath + folderInfo.Path;
        }

        private void WriteIncompleteFileInfoOnUSB(USBJob usbJob)
        {
            ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }
        internal void AcceptSetup(PCJob ContinuedJob)
        {
            ContinuedJob.FolderInfo = ReadAndWrite.BuildTree(ContinuedJob.PCPath);
            //Console.WriteLine(ContinuedJob.FolderInfo.getString());
            //FolderMeta folder = new FolderMeta("C:\\stub", "C:\\");
            
            USBJob usbJob = null;

            foreach (USBJob usb in USBJobs)
            {
                if (ContinuedJob.JobName.Equals(usb.JobName))
                {
                    usbJob = usb;
                    ContinuedJob.SetUsbJob(usbJob);
                }
            }
            //if (usbJob==null) Console.WriteLine("null!!!!");
            //Console.WriteLine(usbJob.JobName + "\n" + usbJob.PCTwoPath);

            Differences newDiff = compareLogic.ConvertFolderMetaToDifferences(ContinuedJob.FolderInfo);
            //Differences newDiff = compareLogic.CompareDirectories(ContinuedJob.FolderInfo, folder);
            //Console.WriteLine("CompareTestStart");
            //compareLogic.compareTest(newDiff);
            //Console.WriteLine("CompareTestEnd");
            List<Conflicts> firstConflict = compareLogic.ComparePCwithUSB(usbJob.diff, newDiff);
          //  Console.WriteLine("CompareResult");
           // compareLogic.ComaprePCwithUSBTest(usbJob.diff, newDiff, firstConflict);
          //  Console.WriteLine("CompareResult");
            if (firstConflict.Count != 0)
            {
                Console.WriteLine("There is conflicts.");
                return;
            }

            ComparisonResult result = new ComparisonResult(usbJob.diff,newDiff,firstConflict);
            ContinuedJob.SetUsbJob(usbJob);
            SyncLogic.CleanSync(result,ContinuedJob);
            //ContinuedJob.FolderInfo = ReadAndWrite.BuildTree(ContinuedJob.PCPath);
            //ReadAndWrite.ExportPCJob(ContinuedJob);
            ReadAndWrite.ExportUSBJob(usbJob);
        }




        internal void CleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
 
            foreach (USBJob usb in USBJobs)
            {
                if (pcJob.JobName.Equals(usb.JobName))
                {
                    pcJob.SetUsbJob(usb);
                }
            }

            SyncLogic.CleanSync(comparisonResult, pcJob);
            ReadAndWrite.ExportPCJob(pcJob);
            
        }

        internal ComparisonResult Compare(PCJob pcJob)
        {
            FolderMeta currentFolderMeta = ReadAndWrite.BuildTree(pcJob.PCPath);
            FolderMeta storedFolderMeta = pcJob.FolderInfo;

            Differences pcDiff = compareLogic.CompareDirectories(currentFolderMeta, storedFolderMeta);
            
            USBJob usbJob = null;

            foreach (USBJob usb in USBJobs)
            {
                if (pcJob.JobName==usb.JobName)
                {
                    usbJob = usb;
                }
            }

            pcJob.SetUsbJob(usbJob);

            List<Conflicts> conflict = compareLogic.ComparePCwithUSB(pcJob.GetUsbJob().diff, pcDiff);

            ComparisonResult comparisonResult = new ComparisonResult(pcJob.GetUsbJob().diff, pcDiff, conflict);

            return comparisonResult;
        }

        internal void USBPlugIn(string usbPath)
        {
            StoredUSBJobInfoPaths = ReadAndWrite.GetUSBJobListPath(usbPath);
            USBJobs = ReadAndWrite.GetUSBJobs(StoredUSBJobInfoPaths);
            foreach (PCJob pcJob in PCJobs)
            {
                foreach (USBJob usb in USBJobs)
                {
                    if (pcJob.JobName.Equals(usb.JobName)) pcJob.SetUsbJob(usb);
                }
            }
        }
    }
}
