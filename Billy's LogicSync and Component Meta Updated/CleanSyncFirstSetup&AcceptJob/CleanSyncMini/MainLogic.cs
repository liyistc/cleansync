using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;

namespace CleanSyncMinimalVersion
{
    class MainLogic
    {
        JobLogic JL;

        public MainLogic()
        {
            JL = new JobLogic();
        }
         internal Job CreateJob(string pathPC, string pathName, string JobName)
         {
             return JL.CreateJob(pathPC, pathName, JobName);
         }
         internal bool FirstTimeSync(Job job)
         {
             return JL.setupJob(job);
         }
         internal List<JobUSB> AcceptJob(string root)
         {
             return JL.GetIncompleteJobList(root); 
         }
         internal Job CreateJob(JobUSB jobUSB, string pathPC)
         {
             Job j = JL.CreateJob(jobUSB,pathPC);
             //RW.RemoveCompletedJob(j);
             return j;
         }

         internal bool AcceptJobSync(Job ContinuedJob)
         {
             try
             {
                 JL.AcceptSetup(ContinuedJob);
                 return true;
             }
             catch 
             {
                 return false;
             }
         }

         /*internal bool CleanSync(Comparison Com, Job job)
         {
            return JL.CleanSync(Com, job);
         }*/

         internal List<Job> GetJobList()
         {
             return ReadAndWrite.ImportJobList();
         }

         /*internal List<string> Compare(Job job)
         {
            return JL.Compare(job);
         }*/
    }
}
