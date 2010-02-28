using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;
using DirectoryInformation;

namespace CleanSyncMini
{
    class MainLogic
    {
        JobLogic jobLogic;

        public MainLogic(int n)
        {
            jobLogic = new JobLogic(n);
        }
         internal PCJob CreateJob(string JobName,string PCPath, string pathName )
         {
             return jobLogic.CreateJob(JobName,PCPath, pathName);
         }
         internal bool FirstTimeSync(PCJob job)
         {
             return jobLogic.setupJob(job);
         }
         internal List<USBJob> AcceptJob(string root)
         {
             return ReadAndWrite.GetIncompleteUSBJobList(root); 
         }
         internal PCJob CreateJob(USBJob jobUSB, string PCPath)
         {
             jobUSB.PCTwoPath = PCPath;
             PCJob jobPC = jobLogic.CreateJob(jobUSB,PCPath);
             
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

         internal ComparisonResult Compare(PCJob pcJob)
         {
             ComparisonResult compResult = jobLogic.Compare(pcJob);
             return compResult;
         }
        
        internal void CleanSync(ComparisonResult comparisonResult,PCJob pcJob)
        {
            jobLogic.CleanSync(comparisonResult,pcJob);
        }

        internal void setUSBJobList(List<USBJob> usbJobs)
        {
            jobLogic.USBJobs = usbJobs;
        }

        internal void USBPlugIn(string usbPath)
        {
            jobLogic.USBPlugIn(usbPath);
        }
    }
}
