using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;

namespace CleanSyncMini
{
    class MainLogic
    {
        JobLogic jobLogic;

        public MainLogic()
        {
            jobLogic = new JobLogic();
        }
         internal PCJob CreateJob(string JobName,string pathPC, string pathName )
         {
             return jobLogic.CreateJob(JobName,pathPC, pathName);
         }
         internal bool FirstTimeSync(PCJob job)
         {
             return jobLogic.setupJob(job);
         }
         internal List<USBJob> AcceptJob(string root)
         {
             return ReadAndWrite.GetIncompleteUSBJobList(root); 
         }
         internal PCJob CreateJob(USBJob jobUSB, string pathPC)
         {
             PCJob jobPC = jobLogic.CreateJob(jobUSB,pathPC);
             
             return jobPC;
         }

         internal bool AcceptJobSync(PCJob ContinuedJob)
         {
             try
             {
                 jobLogic.AcceptSetup(ContinuedJob);
                 return true;
             }
             catch 
             {
                 return false;
             }
         }

        /* internal bool CleanSync()
         {
         }*/
    }
}
