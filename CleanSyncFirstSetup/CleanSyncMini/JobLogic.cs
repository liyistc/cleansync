using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;
using DirectoryInformation;

namespace cleanSyncMinimalVersion
{
    [Serializable]
    class JobLogic
    {
        Job job;
      
        List<Job> JobList;
        List<Job> IncompleteJobList;
        FolderMeta FM;
        JobUSB JUSB;
        //SyncLogic SL;
        //Comparison Com;
        int ID;
        public JobLogic()
        {
            job = null;
            JobList = new List<Job>();
            IncompleteJobList = new List<Job>();
            FM = null;
            JUSB = null;
            ID = -1;
        }
        internal Job CreateJob(string pathPC, string pathName, string JobName)
        {
            CheckNameConflict(JobName);
            job = new Job(pathPC, pathName, JobName);
            InsertIncompleteJob(job);
            return job;
        }
        internal Job CreateJob(JobUSB jobUSB, string pathPC)
        {
            job = new Job(jobUSB,pathPC);
            InsertCompleteJob(job);
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
                JUSB = new JobUSB(j.jobName,j.PCID,j.FM);
                j.JobUSB = JUSB;
                ReadAndWrite.ExportIncompleteJobList(IncompleteJobList);
                ReadAndWrite.ExportIncompleteToUSB(j);
                return true;
            }
        }
        /*internal void AcceptSetup(Job ContinuedJob)
        {
           ReadAndWrite.CopyFolder(ContinuedJob.pathPC,ContinuedJob.pathUSB);
           ContinuedJob.FM = ReadAndWrite.BuildTree(ContinuedJob.pathPC2);
           JobList.Add(ContinuedJob);
           IncompleteJobList.Remove(ContinuedJob);
           ReadAndWrite.ExportJobList(JobList);
           ContinuedJob.JobUSB.PCID = ID; // get ID first
           ReadAndWrite.ExportCompleteToUSB(ContinuedJob);
        }*/

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
