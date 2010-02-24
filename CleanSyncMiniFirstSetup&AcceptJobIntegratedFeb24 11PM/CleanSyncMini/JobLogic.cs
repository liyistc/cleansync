using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;
using DirectoryInformation;
using CleanSyncMini;

namespace CleanSyncMinimalVersion
{
    [Serializable]
    class JobLogic
    {
        List<string> StoredPCJobInfoPaths;
        List<string> StoredUSBJobInfoPaths;
        
  
        
        
        /*Job job;
      
        List<Job> JobList;
        List<Job> IncompleteJobList;
        FolderMeta FM;
        JobUSB JUSB;
        List<JobUSB> incompleteUSBJob;
        List<JobUSB> completeUSBJob;
        //SyncLogic SL;
        //Comparison Com;
        int ID;*/


        public JobLogic()
        {
            /*job = null;
            JobList = new List<Job>();
            IncompleteJobList = new List<Job>();
            JUSB = null;
            incompleteUSBJob = new List<JobUSB>();
            completeUSBJob = new List<JobUSB>();
            FM = null;
            JUSB = null;
            ID = -1;*/
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
            //InsertCompleteJob(job);
            /*
            for (int i = 0; i < incompleteUSBJob.Count; i++)
            {
                if (incompleteUSBJob.ElementAt(i).Equals(jobUSB))
                {
                    incompleteUSBJob.RemoveAt(i);
                    break;
                }
            }
             */
            ReadAndWrite.RemoveIncompleteUSBJob(jobUSB);

            //ReadAndWrite.ExportJobList(JobList);
            //ReadAndWrite.ExportIncompleteToUSB(incompleteUSBJob,Path.GetPathRoot(jobUSB.pathUSB));
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
        /*
        internal void InsertCompleteJob(Job j)
        {
            JobList.Add(j);
        }*/
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
                //ReadAndWrite.ExportIncompleteToUSB(incompleteUSBJob,Path.GetPathRoot(JUSB.pathUSB));
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

        /*internal bool CleanSync(Comparison Com, Job job)
        {
           //bool temp = SL.CleanSync(Com, job);
           job.FM=ReadAndWrite.BuildTree(job.pathPC);
           job.JobUSB.SetDifferenceMeta();
           job.JobUSB.PCID = ID;
           ReadAndWrite.ExportJobList(JobList);
           ReadAndWrite.ExportCompleteToUSB(job);
           //return temp;
           return true;
        }*/

        /*internal List<string> Compare(Job job)
        {
            FM = ReadAndWrite.BuildTree(job.pathPC);
            //List<string> Differences = Com.CompareDirectories(FM, job.FM);
            //Com.ComparisionResult = Com.ComparePCWithUSB(job.JobUSB.Differences, Differences);
            //return Com.ComparisionResult;
        }*/

        
    }
}
