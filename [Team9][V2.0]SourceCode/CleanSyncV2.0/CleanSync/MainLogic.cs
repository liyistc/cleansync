/***********************************************************************
 * 
 * ******************CleanSync Version 2.0 MainLogic********************
 * 
 * Written By : Li Yi & Li Zichen
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;
using DirectoryInformation;
using System.Management;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace CleanSync
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

        /// <summary>
        /// Return the Current connected Drives
        /// </summary>
        public List<string> GetCurrentDrives
        {
            get { return CurrentDrives; }
            set { CurrentDrives = value; }
        }

        /// <summary>
        /// Call the jobLogic to initialize the stored jobs on computer
        /// </summary>
        internal void InitializePCJobInfo()
        {
            try
            {
                jobLogic.InitializePCJobInfo(thisPCID);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// Create a new job.
        /// </summary>
        /// <param name="JobName"></param>
        /// <param name="PCPath"></param>
        /// <param name="pathName"></param>
        /// <param name="config"></param>
        /// <returns>return the created job, null if there already exists a job with the same name</returns> 
        internal PCJob CreateJob(string JobName,string PCPath, string pathName, JobConfig config)
         {
             return jobLogic.CreateJob(JobName,PCPath, pathName+@"_CleanSync_Data_\"+JobName, thisPCID, config);
         }
         internal void FirstTimeSync(PCJob pcJob, System.ComponentModel.BackgroundWorker worker,System.ComponentModel.DoWorkEventArgs eArg)
         {
             jobLogic.setupJob(pcJob, worker,eArg);
         }
         
        /// <summary>
        /// Help method called by USBPlugIn to check whether an incomplete job has been deleted by the computer who creates it.If yes, delete the 
        /// incomplete job and its folders.
        /// </summary>
        /// <param name="usbJob"></param>
        /// <param name="pcID"></param>
        /// <param name="drive"></param>
         private void CheckIncompleteDeletion(USBJob usbJob,string pcID,string drive)
         {
             if (!pcID.Equals(usbJob.PCOneID)) return;
             foreach (PCJob pcJob in GetPCJobs())
             {
                 if (usbJob.JobName.Equals(pcJob.JobName)) return;
             }
             usbJob.AbsoluteUSBPath = drive + usbJob.RelativeUSBPath;
             ReadAndWrite.DeleteFile(ReadAndWrite.GetIncompleteUSBFilePath(usbJob));
             ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);
             if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(usbJob)))
                 ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(usbJob));
             if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(usbJob)))
                 ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(usbJob));
             if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob)))
                 ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob));
         }

        /// <summary>
        /// Accept a Job from USB device and
        /// create a Job on Local disk
        /// </summary>
        /// <param name="jobUSB"></param>
        /// <param name="PCPath"></param>
        /// <returns>PCJob</returns>
         internal PCJob CreateJob(USBJob jobUSB, string PCPath)
         {
             
             PCJob jobPC = jobLogic.CreateJob(jobUSB,PCPath,thisPCID);
             IncompleteList.Remove(jobUSB);
             return jobPC;
         }


        /// <summary>
        /// Compare old metadata with current metadata
        /// </summary>
        /// <param name="pcJob"></param>
        /// <returns>ComparisonResult</returns>
         internal ComparisonResult Compare(PCJob pcJob)
         {
             ComparisonResult compResult = jobLogic.Compare(pcJob);
             return compResult;
         }

        /// <summary>
        /// Carry out normal clean synchronization
        /// </summary>
        /// <param name="comparisonResult"></param>
        /// <param name="pcJob"></param>
        /// <param name="worker"></param>
         internal void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker,System.ComponentModel.DoWorkEventArgs eArg)
         {
             jobLogic.CleanSync(comparisonResult, pcJob, worker,eArg);
         }


        internal void setUSBJobList(List<USBJob> usbJobs)
        {
            jobLogic.USBJobs = usbJobs;
        }

        /// <summary>
        /// This method is invoked when a removable device is plugged-in.
        /// </summary>
        /// <param name="drives"></param>
        /// <returns>return the incomplete jobs stored on this removable device and not created by the current computer</returns>
        internal List<USBJob> USBPlugIn(List<string> drives)
        {
       
            string drive;
            string pcID = GetPCID();
            List<USBJob> incomplete = new List<USBJob>();
            for (int i = 0; i < drives.Count; i++)
            {
                drive = drives[i];
                if (CurrentDrives.Contains(drive)) //CurrentDrives contains the drives that has already been loaded.
                    continue;
                else
                {
                    try
                    {
                        jobLogic.USBPlugIn(drive, pcID);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    // the following code gets the incomplete job on the drive
                    if (Directory.Exists(ReadAndWrite.GetIncompleteUSBFolderPath(drive)))
                    {
                 
                        List<USBJob> temp = ReadAndWrite.GetIncompleteUSBJobList(drive);
                        foreach (USBJob usbJob in temp)
                        {
                            if (usbJob.PCOneID.Equals(pcID))
                            {
                                CheckIncompleteDeletion(usbJob, pcID, drive);
                                foreach (PCJob pcJob in jobLogic.PCJobs)
                                {
                                    if (usbJob.JobName.Equals(pcJob.JobName) && usbJob.PCOnePath.Equals(pcJob.PCPath))
                                    {
                                        usbJob.AbsoluteUSBPath = drive + usbJob.RelativeUSBPath;
                                        pcJob.AbsoluteUSBPath = usbJob.AbsoluteUSBPath;
                                        pcJob.SetUsbJob(usbJob);
                                        pcJob.JobState = JobStatus.Complete;
                                        //pcJob.ToggleStatus(pcJob.JobState);
                                    }
                                }
                                continue;
                            }

                            if (!jobLogic.CheckPCNameConflict(usbJob.JobName))
                            {
                                incomplete.Add(usbJob);
                                usbJob.AbsoluteUSBPath = drive + usbJob.RelativeUSBPath;

                            }
                        }
                    }
                    CurrentDrives.Add(drive);
                }
            }

            IncompleteList.AddRange(incomplete);
            return IncompleteList;

        }

        /// <summary>
        /// The method is invoked when a removable device is removed. Update the incomplete jobs list and the CurrentDrives list.
        /// </summary>
        /// <param name="drives"></param>
        internal void USBRemoved(string[] drives)
        {
            jobLogic.USBRemoved();
            for (int i = 0; i < IncompleteList.Count; i++)
            {
                if (!File.Exists(IncompleteList.ElementAt(i).AbsoluteUSBPath))
                {
                    IncompleteList.RemoveAt(i);
                    i--;
                }
            }
            CurrentDrives.Clear();
            foreach (string drive in drives)
                CurrentDrives.Add(drive);
        }

        /// <summary>
        /// Get all the jobs stored on the computer
        /// </summary>
        /// <returns>return the list contains all the loaded computer jobs</returns>
        public List<PCJob> GetPCJobs()
        {
            return jobLogic.PCJobs;
        }

        /// <summary>
        /// Get all the jobs stored on the removable device
        /// </summary>
        /// <returns>return the list contains all the loaded removable device jobs</returns>
        public List<USBJob> GetUSBJobs()
        {
            return jobLogic.USBJobs;
        }

        public ComparisonResult HandleConflicts(ComparisonResult comparisonResult)
        {
            return jobLogic.HandleConflicts(comparisonResult);
        }

        public void DeleteJob(PCJob pcJob)
        {
            jobLogic.DeleteJob(pcJob,GetPCID());
        }

        /// <summary>
        /// The method returns the unique string which identifies the current computer.
        /// If this is the first time the CleanSync runs on the computer, the string is created by System.Environment.TickCount and stored into a file
        /// under CleanSync data folder.
        /// If the file exists, return the string contained in the file.
        /// </summary>
        /// <returns>return a unique string which identifies the current computer</returns>
        private string GetPCID()
        {

            if (!File.Exists(ReadAndWrite.GetPCIDFile()))
            {
                string time = System.Environment.TickCount.ToString() ;
                ReadAndWrite.CreatePCIDFile(time);
                return time;
            }
            else 
            {
                string[] pcID = File.ReadAllLines(ReadAndWrite.GetPCIDFile());

                return pcID[0];
            }
        }

        public void CheckJobStatus()
        {
            jobLogic.CheckJobStatus();
        }

        /// <summary>
        /// Validate a folder path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>bool</returns>
        public bool ValidatePath(string path)
        {
            Regex fileRegex = new Regex(@"^[a-zA-Z]:(\\[^\""\?\\\/\:\*\<\>\|]+)*\\?$");
            Match match = fileRegex.Match(path);
            if (match.Success && path.Length <= 255)
                return true;
            else return false;
        }

        /// <summary>
        /// Validate a job name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValidateJobName(string name)
        {
            //Regex nameRegex = new Regex(@"^[^\""\?\\\/\:\*\<\>\|]+$");
            Regex nameRegex = new Regex(@"^[A-Za-z0-9_\ ]+$");
            Match match = nameRegex.Match(name);
            if (match.Success && name.Length <= 20 && !name.Trim().Equals(string.Empty))
                return true;
            else return false;
        }

        #region SpaceCalculation
        [DllImport("kernel32.dll", EntryPoint = "GetDiskFreeSpaceExA")]
        private static extern long GetDiskFreeSpaceEx(string lpDirectoryName,
          out long lpFreeBytesAvailableToCaller,
          out long lpTotalNumberOfBytes,
          out long lpTotalNumberOfFreeBytes);
        internal long GetFreeDiskSpace(string driveName)
        {
            long totalBytes, freeBytes, freeBytesAvail, result;

            if (!Directory.Exists(driveName))
                throw new ArgumentException("Invalid Drive " + driveName);

            result = GetDiskFreeSpaceEx(driveName,
                out freeBytesAvail,
                out totalBytes,
                out freeBytes);

            return freeBytesAvail;

        }

        public string GetDiskSpaceMB(long freespace)
        {
            try
            {
                double dResult = (double)freespace;
                dResult = (dResult / 1024) / 1024;

                return dResult.ToString("###,###,##0.00MB");
            }
            catch (ArgumentException)
            {
                return "";
            }
        }

        public bool CheckPCDiskSpace(ComparisonResult result, PCJob pcJob)
        {
            long pcFreeSpace = GetFreeDiskSpace(ReadAndWrite.GetRootPath(pcJob.PCPath));
            if (pcFreeSpace < result.GetPCRequiredSpace())
                return false;
            return true;
        }

        public bool CheckUSBDiskSpace(ComparisonResult result, PCJob pcJob)
        {
            long usbFreeSpace = GetFreeDiskSpace(ReadAndWrite.GetRootPath(pcJob.AbsoluteUSBPath));
            if (usbFreeSpace < result.GetUSBRequiredSpace())
                return false;
            return true;
        }

        public bool CheckUSBDiskSpace(string pcPath, string usbPath)
        {            
            CompareLogic compLogic = new CompareLogic();
            Differences pcToUSBDiff = compLogic.ConvertFolderMetaToDifferences(ReadAndWrite.BuildTree(pcPath));
            long usbFreeSpace = GetFreeDiskSpace(ReadAndWrite.GetRootPath(usbPath));
            if (usbFreeSpace < pcToUSBDiff.GetRequireSpace())
                return false;
            return true;
        }

        public string GetPCFreeSpace(PCJob pcJob)
        {
            long pcFreeSpace = GetFreeDiskSpace(ReadAndWrite.GetRootPath(pcJob.PCPath));
            return GetDiskSpaceMB(pcFreeSpace);
        }

        public string GetUSBFreeSpace(PCJob pcJob)
        {
            long usbFreeSpace = GetFreeDiskSpace(ReadAndWrite.GetRootPath(pcJob.AbsoluteUSBPath));
            return GetDiskSpaceMB(usbFreeSpace);
        }

        public string GetPCRequiredSpace(ComparisonResult result)
        {
            long pcRequiredSpace = result.GetPCRequiredSpace();
            return GetDiskSpaceMB(pcRequiredSpace);
        }

        public string GetUSBRequiredSpace(ComparisonResult result)
        {
            long usbRequiredSpace = result.GetUSBRequiredSpace();
            return GetDiskSpaceMB(usbRequiredSpace);
        }

        #endregion

        public bool Resync(PCJob pcJob)
        {
            if (pcJob.GetUsbJob().MostRecentPCID.Equals(GetPCID()))
                return true;
            return false;
        }

        public ComparisonResult AutoConflictResolve(PCJob pcJob, ComparisonResult result)
        {
            return jobLogic.AutoConflictResolve(pcJob, result);
        }
    }
}
