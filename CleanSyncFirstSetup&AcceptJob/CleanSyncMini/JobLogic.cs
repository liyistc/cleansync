using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;
using DirectoryInformation;

namespace CleanSyncMinimalVersion
{
    [Serializable]
    class JobLogic
    {
        Job job;
      
        List<Job> JobList;
        List<Job> IncompleteJobList;
        FolderMeta FM;
        JobUSB JUSB;
        List<JobUSB> incompleteUSBJob;
        List<JobUSB> completeUSBJob;
        //SyncLogic SL;
        //Comparison Com;
        int ID;
        public JobLogic()
        {
            job = null;
            JobList = new List<Job>();
            IncompleteJobList = new List<Job>();
            JUSB = null;
            incompleteUSBJob = new List<JobUSB>();
            completeUSBJob = new List<JobUSB>();
            FM = null;
            JUSB = null;
            ID = -1;
        }
        internal Job CreateJob(string pathPC, string pathName, string JobName)
        {
            CheckNameConflict(JobName);
            job = new Job(pathPC, pathName, JobName);           
            InsertIncompleteJob(job);
            ReadAndWrite.ExportIncompleteJobList(IncompleteJobList);
            return job;
        }
        internal Job CreateJob(JobUSB jobUSB, string pathPC)
        {
            job = new Job(jobUSB,pathPC);
            InsertCompleteJob(job);
            for (int i = 0; i < incompleteUSBJob.Count; i++)
            {
                if (incompleteUSBJob.ElementAt(i).Equals(jobUSB))
                {
                    incompleteUSBJob.RemoveAt(i);
                    break;
                }
            }

            ReadAndWrite.ExportJobList(JobList);
            ReadAndWrite.ExportIncompleteToUSB(incompleteUSBJob,Path.GetPathRoot(jobUSB.pathUSB));
            return job;
        }
        internal void CheckNameConflict(string JobName)
        {

        }
        internal void InsertIncompleteJob(Job j)
        {
            IncompleteJobList.Add(j);
        }
        internal void InsertCompleteJob(Job j)
        {
            JobList.Add(j);
        }
        internal bool setupJob(Job j)
        {
            FM = ReadAndWrite.BuildTree(j.pathPC);
            if (FM.Equals(null))
                return false;
            else
            {
                j.FM = FM;
                ReadAndWrite.CopyFolder(FM.Path, j.pathUSB);
                JUSB = new JobUSB(j.jobName, j.pathPC, j.pathUSB,j.PCID, j.FM);
                //j.JobUSB = JUSB;
                incompleteUSBJob.Add(JUSB);
                Console.WriteLine(Path.GetPathRoot(JUSB.pathUSB));
                ReadAndWrite.ExportIncompleteToUSB(incompleteUSBJob,Path.GetPathRoot(JUSB.pathUSB));
                return true;
            }
        }
        internal void AcceptSetup(Job ContinuedJob)
        {
           ContinuedJob.FM = ReadAndWrite.BuildTree(ContinuedJob.pathPC);
           Console.WriteLine(ContinuedJob.FM.getString());
           // compare with differences
           // get conflictions
           ReadAndWrite.CopyFolder(ContinuedJob.pathUSB, ContinuedJob.pathPC);
           ReadAndWrite.DeleteFolder(ContinuedJob.pathUSB);

           SyncLogic.SyncPCtoUSB(ContinuedJob);
           ContinuedJob.FM = ReadAndWrite.BuildTree(ContinuedJob.pathPC);
           ReadAndWrite.ExportJobList(JobList);

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

        internal List<JobUSB> GetIncompleteJobList(string root)
        {
            return ReadAndWrite.ImportIncompleteFromUSB(root);
        }
    }
}
