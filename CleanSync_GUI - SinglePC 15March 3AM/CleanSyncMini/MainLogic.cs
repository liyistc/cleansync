using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;
using DirectoryInformation;
using System.Management;

namespace CleanSyncMini
{
    public class MainLogic
    {
        private JobLogic jobLogic;
        private string thisPCID;
        private List<string> CurrentDrives;
        
        public List<USBJob> IncompleteList;


        public MainLogic()
        {
            jobLogic = new JobLogic();
            thisPCID = GetPCID();
            CurrentDrives = new List<string>();
            IncompleteList = new List<USBJob>();
        }

        public List<string> GetCurrentDrives
        {
            get { return CurrentDrives; }
            set { CurrentDrives = value; }
        }

        internal void InitializePCJobInfo()
        {
            jobLogic.InitializePCJobInfo();
        }
        internal PCJob CreateJob(string JobName,string PCPath, string pathName )
         {
             return jobLogic.CreateJob(JobName,PCPath, pathName, thisPCID);
         }
        internal bool FirstTimeSync(PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
         {
             return jobLogic.setupJob(pcJob, worker);
         }
        internal List<USBJob> AcceptJob(List<string> drives)
         {
             List<USBJob> incomplete = new List<USBJob>();
             string pcID = GetPCID();
             foreach (string drive in drives)
             {
        
                 if (Directory.Exists(ReadAndWrite.GetIncompleteUSBFolderLocation(drive)))
                 {
                     List<USBJob> temp = ReadAndWrite.GetIncompleteUSBJobList(drive) ;
                     foreach (USBJob usbJob in temp)
                     {
                         if (usbJob.PCOneID.Equals(pcID)) continue;
                         bool flag = true;
                         foreach (USBJob original in incomplete)
                         {
                             if (usbJob.JobName.Equals(original.JobName))
                             {
                                 flag = false;
                                 break;
                             }
                         }
                         if (flag) incomplete.Add(usbJob);
                     }
            
                     foreach (USBJob usbJob in incomplete)
                     {
                         usbJob.AbsoluteUSBPath = drive + usbJob.RelativeUSBPath;
                     }
                 }
             }
             IncompleteList = incomplete;
             return incomplete; 
         }
        internal PCJob CreateJob(USBJob jobUSB, string PCPath)
         {
             
             PCJob jobPC = jobLogic.CreateJob(jobUSB,PCPath,thisPCID);
             IncompleteList.Remove(jobUSB);
             return jobPC;
         }

        /*internal ComparisonResult AcceptJobCompare(PCJob ContinuedJob)
         {
             ComparisonResult result =  jobLogic.Compare(ContinuedJob);
             return result;
         }*/

        internal ComparisonResult Compare(PCJob pcJob)
         {
             ComparisonResult compResult = jobLogic.Compare(pcJob);
             return compResult;
         }

        internal void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
         {
             jobLogic.CleanSync(comparisonResult, pcJob, worker);
         }


        internal void setUSBJobList(List<USBJob> usbJobs)
        {
            jobLogic.USBJobs = usbJobs;
        }

        internal void USBPlugIn(List<string> drives)
        {
            foreach (string drive in drives)
            {
                if (CurrentDrives.Contains(drive))
                    continue;
                else
                {
                    string pcID = GetPCID();
                    jobLogic.USBPlugIn(drive, pcID);
                    CurrentDrives.Add(drive);
                }
            }
            
        }

        internal void USBRemoved(string[] drives)
        {
            jobLogic.USBRemoved();
            CurrentDrives.Clear();
            foreach (string drive in drives)
                CurrentDrives.Add(drive);
        }

        public List<PCJob> GetPCJobs()
        {
            return jobLogic.PCJobs;
        }

        public List<USBJob> GetUSBJobs()
        {
            return jobLogic.USBJobs;
        }

        public ComparisonResult handleConflicts(ComparisonResult comparisonResult, int[] userChoice)
        {
            return jobLogic.handleConflicts(comparisonResult, userChoice);
        }

        public void DeleteJob(PCJob pcJob)
        {
            jobLogic.DeleteJob(pcJob,GetPCID());
        }

        private string GetPCID()
        {
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT MacAddress FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection queryCollection = query.Get();

            foreach (ManagementBaseObject managementObj in queryCollection)
            {
                string pcID = Convert.ToString(managementObj["MacAddress"]);
                if (!pcID.Equals(""))
                {
                    return pcID;
                }
            }

            return null;
        }
    }
}
