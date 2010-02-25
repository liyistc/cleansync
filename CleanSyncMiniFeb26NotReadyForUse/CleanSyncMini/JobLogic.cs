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
      
        public JobLogic()
        {
         
            StoredPCJobInfoPaths = new List<string>();
            StoredUSBJobInfoPaths = new List<string>();
        }
        internal PCJob CreateJob(string jobName, string pathPC, string pathUSB)
        {
            CheckNameConflict(jobName);
            PCJob pcJob = new PCJob(jobName, pathPC, pathUSB);           
            InsertJob(pcJob);
            
            
            return pcJob;
        }
        internal PCJob CreateJob(USBJob jobUSB, string pathPC)
        {
            PCJob pcJob = new PCJob(jobUSB,pathPC);
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
            StoredPCJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnPC(pcJob));
            ReadAndWrite.ExportPCJobPathsList(StoredPCJobInfoPaths);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        internal void InsertJob(USBJob usbJob)
        {
            StoredUSBJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnUSB(usbJob));
            ReadAndWrite.ExportUSBJobPathsList(StoredUSBJobInfoPaths,usbJob);
            ReadAndWrite.ExportUSBJob(usbJob);
        }
       
        internal bool setupJob(PCJob pcJob)
        {
            FolderMeta folderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            if (folderInfo.Equals(null))
                return false;
            else
            {
                pcJob.FolderInfo = folderInfo;
                string fullFolderPath = GetFullFolderPath(pcJob, folderInfo);
                ReadAndWrite.CopyFolder(fullFolderPath, pcJob.USBPath);
                ReadAndWrite.ExportPCJob(pcJob);
                USBJob usbJob = new USBJob(pcJob);
                WriteIncompleteFileInfoOnUSB(usbJob);
                Console.WriteLine(Path.GetPathRoot(usbJob.USBPath));
                
                return true;
            }
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
           Console.WriteLine(ContinuedJob.FolderInfo.getString());
           // compare with differences
           // get conflictions
           ReadAndWrite.CopyFolder(ContinuedJob.USBPath, ContinuedJob.PCPath);
           ReadAndWrite.DeleteFolder(ContinuedJob.USBPath);

           SyncLogic.SyncPCtoUSB(ContinuedJob);
           ContinuedJob.FolderInfo = ReadAndWrite.BuildTree(ContinuedJob.PCPath);
           ReadAndWrite.ExportPCJob(ContinuedJob);

           /*ReadAndWrite.CopyFolder(ContinuedJob.pathPC,ContinuedJob.pathUSB);
           ContinuedJob.FM = ReadAndWrite.BuildTree(ContinuedJob.pathPC2);
           JobList.Add(ContinuedJob);
           IncompleteJobList.Remove(ContinuedJob);
           ReadAndWrite.ExportJobList(JobList);
           ContinuedJob.JobUSB.PCID = ID; // get ID first
           ReadAndWrite.ExportCompleteToUSB(ContinuedJob);*/
        }

       
        
    }
}
