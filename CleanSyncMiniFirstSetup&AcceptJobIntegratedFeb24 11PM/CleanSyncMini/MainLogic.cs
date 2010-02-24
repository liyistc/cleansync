using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;
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
         internal PCJob CreateJob(string JobName,string pathPC, string pathName )
         {
             return JL.CreateJob(JobName,pathPC, pathName);
         }
         internal bool FirstTimeSync(PCJob job)
         {
             return JL.setupJob(job);
         }
         internal List<USBJob> AcceptJob(string root)
         {
             return ReadAndWrite.GetIncompleteUSBJobList(root); 
         }
         internal PCJob CreateJob(USBJob jobUSB, string pathPC)
         {
             PCJob jobPC = JL.CreateJob(jobUSB,pathPC);
             //RW.RemoveCompletedJob(j);
             return jobPC;
         }

         internal bool AcceptJobSync(PCJob ContinuedJob)
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

         /*internal List<PCJob> GetJobList()
         {
             return ReadAndWrite.ImportJobList();
         }*/

         /*internal List<string> Compare(Job job)
         {
            return JL.Compare(job);
         }*/
    }
}
