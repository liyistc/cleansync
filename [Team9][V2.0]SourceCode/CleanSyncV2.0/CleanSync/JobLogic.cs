/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 JobLogic********************
 * 
 * Written By : Li Zichen
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
using DirectoryInformation;
using CleanSync;

namespace CleanSync
{
    [Serializable]
    class JobLogic
    {
       
        SyncLogic sync;
        const int COMPUTERONE = 1;
        const int COMPUTERTWO = 2;
        const int OTHERCOMPUTER = -1;

        internal List<PCJob> PCJobs
        {
            get;
            set;
        }
        internal List<USBJob> USBJobs
        {
            get;
            set;
        }

        CompareLogic compareLogic;

      
        public JobLogic()
        {
            PCJobs = new List<PCJob>();
            USBJobs = new List<USBJob>();
            compareLogic = new CompareLogic();
            sync = new SyncLogic();
        }

        /// <summary>
        /// Initialize previously stored synchronization jobs on the computer, update the PCJobs list.
        /// </summary>
        /// <param name="pcID">string that identifies the current computer</param>
        internal void InitializePCJobInfo(string pcID)
        {
            if (!Directory.Exists(ReadAndWrite.GetStoredFolderOnPC(pcID)))
            {
                PCJobs = new List<PCJob>();
                return;
            }
            
            string[] storedPCJobs = Directory.GetFiles(ReadAndWrite.GetStoredFolderOnPC(pcID));
            
            if (storedPCJobs.Length == 0) return;
            try
            {
                PCJobs = ReadAndWrite.GetPCJobs(storedPCJobs);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("path for stored computer job should not be null.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Initialize stored synchronized jobs on the plugged-in removable device, update the USBJobs list
        /// </summary>
        /// <param name="usbRoot">specify the root of the removable device drive</param>
        /// <param name="pcID">string that identifies the current computer</param>
        internal void InitializeUSBJobInfo(string usbRoot,string pcID)
        {
      
            string[] storedUSBJobs = Directory.GetFiles(ReadAndWrite.GetStoredFolderOnUSB(usbRoot));
            
            if (storedUSBJobs.Length == 0)
            {
                return;
            }
            

            int originCount = USBJobs.Count; // count of the previous loaded jobs stored on other removable device, these jobs info should not be modified

            foreach (string usbJobPath in storedUSBJobs)
            {
                try
                {
                    USBJobs.Add(ReadAndWrite.GetUSBJob(usbJobPath));
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentNullException("path for stored removable device job should not be null.");
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

      
            USBJob usbJob;
            for (int i = originCount; i < USBJobs.Count; i++)
            {
                usbJob = USBJobs.ElementAt(i);
                try
                {
                    usbJob.AbsoluteUSBPath = usbRoot + usbJob.RelativeUSBPath;
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentNullException("loaded removable device job should not be null.");
                }
                catch (Exception)
                {
                    throw new Exception("Set absolute path failed.");
                }

                // the following code checks whether the job is deleted on this computer.If yes, update the information to the respective job
                int pcNo = GetPCNumber(usbJob,pcID);

                if (pcNo == OTHERCOMPUTER) continue;

                else if (pcNo == COMPUTERONE)
                    i = CheckPCTwoDelete(i, usbJob);

                else if (pcNo == COMPUTERTWO)
                    i = CheckPCOneDelete(i, usbJob);
            }

        }

        /// <summary>
        /// Helper method to check job deletion, update the information when necessary.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="usbJob"></param>
        /// <returns></returns>
        private int CheckPCTwoDelete(int i, USBJob usbJob)
        {
            PCJob pcJob = ConnectUSBJobwithPCJob(usbJob);
            if (pcJob != null && usbJob.PCTwoDeleted)
            {
                RemoveUSBJob(usbJob);
                RemovePCJob(pcJob);
                i--;
            }
            else if (pcJob == null && usbJob.PCTwoDeleted)
            {
                RemoveUSBJob(usbJob);
                i--;
            }
            else if (pcJob == null)
            {
                usbJob.PCOneDeleted = true;
                ReadAndWrite.ExportUSBJob(usbJob);
                DeleteUSBFolders(usbJob);
            }
            return i;
        }

        

        /// <summary>
        /// Helper method to check job deletion, update the information when necessary.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="usbJob"></param>
        /// <returns></returns>
        private int CheckPCOneDelete(int i, USBJob usbJob)
        {
            PCJob pcJob = ConnectUSBJobwithPCJob(usbJob);
            if (pcJob != null && usbJob.PCOneDeleted)
            {
                RemoveUSBJob(usbJob);
                RemovePCJob(pcJob);
                i--;
            }
            else if (pcJob == null && usbJob.PCOneDeleted)
            {
                RemoveUSBJob(usbJob);
                i--;
            }
            else if (pcJob == null)
            {
                usbJob.PCTwoDeleted = true;
                ReadAndWrite.ExportUSBJob(usbJob);
                DeleteUSBFolders(usbJob);
            }
            return i;
        }

        /// <summary>
        /// Helper method called by CheckPCJobDelete. Connected a job on removable device with its corresponding job on computer
        /// </summary>
        /// <param name="usbJob"></param>
        /// <returns>returns the corresponding computer job, null if the computer job is deleted</returns>
        internal PCJob ConnectUSBJobwithPCJob(USBJob usbJob)
        {
            foreach (PCJob pcJob in PCJobs)
            {
                if (pcJob.JobName.Equals(usbJob.JobName))
                {
                    if (pcJob.PCID.Equals(usbJob.PCOneID) && !pcJob.PCPath.Equals(usbJob.PCOnePath))
                        continue;
                    if (pcJob.PCID.Equals(usbJob.PCTwoID) && !pcJob.PCPath.Equals(usbJob.PCTwoPath))
                        continue;

                    pcJob.JobState = JobStatus.Complete;
                    pcJob.SetUsbJob(usbJob);
                    pcJob.AbsoluteUSBPath = usbJob.AbsoluteUSBPath;
                    return pcJob;
                }
            }
            return null;
        }

        /// <summary>
        /// Helper method called by InitializeUSBJobInfo. returns the value indicates which computer the removable device is plugged-in.
        /// </summary>
        /// <param name="usbJob"></param>
        /// <param name="pcID">string that identifies the current computer</param>
        /// <returns></returns>
        private int GetPCNumber(USBJob usbJob,string pcID)
        {
            if (pcID.Equals(usbJob.PCOneID))
                return COMPUTERONE;
            else if (pcID.Equals(usbJob.PCTwoID))
                return COMPUTERTWO;
            else return OTHERCOMPUTER;
        }

        /// <summary>
        /// Create a new synchronization job and sets the PCJob properties
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="PCPath"></param>
        /// <param name="AbsoluteUSBPath"></param>
        /// <param name="PCID"></param>
        /// <param name="config"></param>
        /// <returns>return created synchronization job, null if there already exists a job having same name with the passed in jobName</returns>
        internal PCJob CreateJob(string jobName, string PCPath, string AbsoluteUSBPath, string PCID, JobConfig config)
        {
            if (CheckNameConflict(jobName,AbsoluteUSBPath)) return null;

            
            try
            {
                PCJob pcJob = new PCJob(jobName, PCPath, AbsoluteUSBPath, PCID,config);

                ReadAndWrite.CreatePCBackUpDirectory(pcJob);
                ReadAndWrite.CreatePCTempDirectory(pcJob);

                pcJob.JobState = JobStatus.Complete;
                
                InsertJob(pcJob);
                
                return pcJob;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Parameters for creating computer job should not be null.");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Parameter failed for creating new computer job.");
            }
            catch (Exception)
            {
                throw new Exception("Failed to create new computer job.");
            }
            
        }
        
        /// <summary>
        /// Create a synchronization job when an incomplete job is accepted, update corresponding properties and delete the incomplete job
        /// </summary>
        /// <param name="jobUSB"></param>
        /// <param name="PCPath"></param>
        /// <param name="PCID"></param>
        /// <returns>created synchronization job</returns>
        internal PCJob CreateJob(USBJob jobUSB, string PCPath, string PCID)
        {
            jobUSB.PCTwoPath = PCPath;
            try
            {
                PCJob pcJob = new PCJob(jobUSB, PCPath, PCID);
                pcJob.JobState = JobStatus.Complete;
                jobUSB.JobState = JobStatus.Complete;
                jobUSB.PCTwoID = PCID;
                //jobUSB.MostRecentPCID = PCID;

                ReadAndWrite.CreatePCBackUpDirectory(pcJob);
                ReadAndWrite.CreatePCTempDirectory(pcJob);

                InsertJob(pcJob);

                InsertJob(jobUSB);
                ReadAndWrite.RemoveIncompleteUSBJob(jobUSB);

                

                return pcJob;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Parameter for creating computer job from removable job should not be null.");
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Parameter failed for creating computer job from removable job.");
            }
            catch (Exception)
            {
                throw new Exception();
            }
                       
        }

        /// <summary>
        /// Helper method called by CreatePCJob. update PCJobs list and create the folder and file to store the job information.
        /// </summary>
        /// <param name="pcJob"></param>
        internal void InsertJob(PCJob pcJob)
        {

            PCJobs.Add(pcJob);


            if (!Directory.Exists(pcJob.PCPath))
            {
                ReadAndWrite.CreateDirectory(pcJob.PCPath);
            }

            ReadAndWrite.ExportPCJob(pcJob);
        }

        /// <summary>
        /// Helper method called by CreatePCJob. update USBJobs list and create the file to store the job information
        /// </summary>
        /// <param name="usbJob"></param>
        internal void InsertJob(USBJob usbJob)
        {

            USBJobs.Add(usbJob);

            ReadAndWrite.ExportUSBJob(usbJob);
        }

        /// <summary>
        /// Helper method called by CreatePCJob, check whether a job with the same name has already been created.
        /// Jobs stored on the computer and the removable device are all checked.
        /// </summary>
        /// <param name="JobName"></param>
        /// <param name="usbPath"></param>
        /// <returns></returns>
        internal bool CheckNameConflict(string JobName,string usbPath)
        {
            try
            {
                foreach (PCJob pcJob in PCJobs)
                    if (pcJob.JobName.Equals(JobName,StringComparison.OrdinalIgnoreCase)) return true;
                foreach (USBJob usbJob in USBJobs)
                    if (usbJob.JobName.Equals(JobName, StringComparison.OrdinalIgnoreCase)) return true;
                string[] incomplete = ReadAndWrite.GetDirectoryFiles(ReadAndWrite.GetIncompleteUSBFolderPath(usbPath));
                foreach (string incompleteName in incomplete)
                    if (JobName.Equals(Path.GetFileNameWithoutExtension(incompleteName),StringComparison.OrdinalIgnoreCase)) return true;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Name Conflict checking : job should not be null.");
            }
            return false;
        }

        internal bool CheckPCNameConflict(string JobName)
        {
            try
            {
                foreach (PCJob pcJob in PCJobs)
                    if (pcJob.JobName.Equals(JobName)) return true;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Name Conflict checking : job should not be null.");
            }
            return false;
        }

        
        /// <summary>
        /// Action performs after a synchronization job is created. Do the initialization synchronization of the job and write an incomplete job to
        /// the removable device
        /// </summary>
        /// <param name="pcJob"></param>
        /// <param name="worker"></param>
        /// <param name="eArg"></param>
        internal void setupJob(PCJob pcJob, System.ComponentModel.BackgroundWorker worker,System.ComponentModel.DoWorkEventArgs eArg)
        {
            try
            {
                FolderMeta folderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);

                pcJob.FolderInfo = folderInfo;

                ReadAndWrite.ExportPCJob(pcJob);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("computer job for first sync should not be null.");
            }

            USBJob usbJob = new USBJob(pcJob);

            ReadAndWrite.CreateUSBTempDirectory(usbJob);
            ReadAndWrite.CreateUSBResyncDirectory(usbJob);
            ReadAndWrite.CreateUSBResyncBackUpDirectory(usbJob);

            pcJob.SetUsbJob(usbJob);
       
            usbJob.diff = compareLogic.ConvertFolderMetaToDifferences(pcJob.FolderInfo);

            try
            {
                sync.InitializationSynchronize(usbJob.diff, pcJob, worker,eArg);
            }
            catch (UnauthorizedAccessException e)
            {
                throw e;
            }
            
            WriteIncompleteFileInfoOnUSB(usbJob);
            
        }

        
        /// <summary>
        /// Helper method called by setupJob. It creates an incomplete job on removable device.
        /// </summary>
        /// <param name="usbJob"></param>
        private void WriteIncompleteFileInfoOnUSB(USBJob usbJob)
        {
            ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }
        


        /// <summary>
        /// Call SyncLogic to perform CleanSync.
        /// </summary>
        /// <param name="comparisonResult"></param>
        /// <param name="pcJob"></param>
        /// <param name="worker"></param>
        /// <param name="eArg"></param>
        internal void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker,System.ComponentModel.DoWorkEventArgs eArg)
        {
            try
            {
                sync.CleanSync(comparisonResult, pcJob, worker,eArg);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Parameter for clean sync should not be null.");
            }

        }

        /// <summary>
        /// Call CompareLogic to compare two folders. Check whether the job needs restoration.
        /// </summary>
        /// <param name="pcJob"></param>
        /// <returns>return a Comparison result object which consists of three list: changes made on computer, changes made on removable device,
        /// conflicts</returns>
        internal ComparisonResult Compare(PCJob pcJob)
        {
            try
            {
                if (pcJob.Synchronizing) JobsRestoreLogic.RestoreInterruptedPCJobPCChanges(pcJob);
                if (pcJob.GetUsbJob().Synchronizing) JobsRestoreLogic.RestoreInterruptedUSB(pcJob);
                if (pcJob.GetUsbJob().ReSynchronizing) JobsRestoreLogic.RestoreReSyncUSB(pcJob);

                FolderMeta currentFolderMeta = ReadAndWrite.BuildTree(pcJob.PCPath);
                FolderMeta storedFolderMeta = pcJob.FolderInfo;

                Differences pcDiff = compareLogic.CompareDirectories(currentFolderMeta, storedFolderMeta);
                Differences usbDifferences = new Differences(pcJob.GetUsbJob().diff);
                List<Conflicts> conflict = compareLogic.DetectConflicts(usbDifferences, pcDiff);
                ComparisonResult comparisonResult = new ComparisonResult(usbDifferences, pcDiff, conflict);
                return comparisonResult;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Parameter for compare should not be null.");
            }
        }

        /// <summary>
        /// Method handling removable device plug-in, check whether it contains synchronization jobs. If yes, initialize these jobs.
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="pcID"></param>
        internal void USBPlugIn(string drive, string pcID)
        {
            if (Directory.Exists(ReadAndWrite.GetStoredFolderOnUSB(drive)))
                try
                {
                    InitializeUSBJobInfo(drive, pcID);
                }
                catch (Exception e)
                {
                    throw e;
                }
        }

        /// <summary>
        /// Method handling removable device plug-out. reset all jobs on computer with their corresponding job plugged-out, update USBJobs list.
        /// </summary>
        internal void USBRemoved()
        {
            try
            {
                foreach (PCJob pcJob in PCJobs)
                {
                    if (pcJob.GetUsbJob() != null && !Directory.Exists(pcJob.GetUsbJob().AbsoluteUSBPath))
                    {
                        pcJob.SetUsbJob(null);
                        pcJob.AbsoluteUSBPath = "Removable disk not plugged in.";
                        pcJob.ToggleStatus(pcJob.JobState);
                        ReadAndWrite.ExportPCJob(pcJob);
                    }
                }
				for (int i = 0; i < USBJobs.Count; i++)
                {
                    if (!Directory.Exists(USBJobs[i].AbsoluteUSBPath))
                    {
                        USBJobs.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("On Removable device remove.Job stored on computer should not be null.");
            }
        }

        
        /// <summary>
        /// Call ConflictHandler class to handle conflicts detected by Compare.
        /// </summary>
        /// <param name="comparisonResult"></param>
        /// <returns></returns>
        internal ComparisonResult HandleConflicts(ComparisonResult comparisonResult)
        {
            ConflictHandler conflictHandler = new ConflictHandler();
            return conflictHandler.HandleConflicts(comparisonResult);
        }

        
        /// <summary>
        /// Delete a selected computer synchronization job.
        /// </summary>
        /// <param name="pcJob"></param>
        /// <param name="pcID"></param>
        internal void DeleteJob(PCJob pcJob,string pcID)
        {
            RemovePCJob(pcJob);

            if (pcJob.GetUsbJob() != null) // if removable device is present
            {
                    
                // if job is a incomplete, delete the incomplete job file and folders
                if (pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                {
                    if (File.Exists(ReadAndWrite.GetIncompleteUSBFilePath(pcJob.GetUsbJob()))) 
                        ReadAndWrite.DeleteFile(ReadAndWrite.GetIncompleteUSBFilePath(pcJob.GetUsbJob()));
                    DeleteUSBFolders(pcJob.GetUsbJob());                  
                    return;
                }
                
                // if job is not incomplete, update the delete information in the removable device job file, delete all the folders related with that job
                USBJob usbJob = pcJob.GetUsbJob();
                if (pcID.Equals(usbJob.PCOneID))
                    usbJob.PCOneDeleted = true;
                else
                    usbJob.PCTwoDeleted = true;
                ReadAndWrite.ExportUSBJob(usbJob);
                DeleteUSBFolders(usbJob);
            }

        }

        /// <summary>
        /// Helper method called by DeleteJob. update PCJobs list, Delete all the information related to that job. 
        /// </summary>
        /// <param name="pcJob"></param>
        private void RemovePCJob(PCJob pcJob)
        {
            
            PCJobs.Remove(pcJob);
            if (File.Exists(ReadAndWrite.GetStoredPathOnPC(pcJob)))
                ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnPC(pcJob));
            if (Directory.Exists(ReadAndWrite.GetPCTempFolder(pcJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetPCTempFolder(pcJob));
            
        }

        /// <summary>
        /// Helper method called by CheckPCJobDelete.Update USBJobs list, delete all the information related to that job. 
        /// </summary>
        /// <param name="usbJob"></param>
        private void RemoveUSBJob(USBJob usbJob)
        {
            
            USBJobs.Remove(usbJob);

            ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnUSB(usbJob));
            DeleteUSBFolders(usbJob);
        }

        /// <summary>
        /// CheckJobStatus when job is loaded. Toggle the status when necessary.
        /// </summary>
        public void CheckJobStatus()
        {
            foreach (PCJob pcJob in PCJobs)
            {
                if (pcJob.JobState.Equals(JobStatus.Complete) && pcJob.GetUsbJob() == null)
                    pcJob.ToggleStatus(pcJob.JobState);
            }
        }

        /// <summary>
        /// Codes dealing with auto conflict resolve.
        /// </summary>
        /// <param name="pcJob"></param>
        /// <param name="result"></param>
        /// <returns>returns a comparison result with all conflicts automatically handled</returns>
        internal ComparisonResult AutoConflictResolve(PCJob pcJob, ComparisonResult result)
        {
            switch (pcJob.JobSetting.ConflictConfig)
            {
                //case AutoConflictOption.IgnoreBoth:
                //    SetConflictUserChoice(result.conflictList, false, false);
                //    break;
                case AutoConflictOption.KeepPCItems:
                    SetConflictUserChoice(result.conflictList, true, false);
                    break;
                case AutoConflictOption.KeepUSBItems:
                    SetConflictUserChoice(result.conflictList, false, true);
                    break;
                //case AutoConflictOption.KeepBoth:
                //    // To be implemented.
                //    // conflict rename
                //    SetConflictUserChoice(result.conflictList, false, false);
                //    break;
            }
            return HandleConflicts(result);
        }
        
        /// <summary>
        /// Sets conflict choice of a job.
        /// </summary>
        /// <param name="conflictList"></param>
        /// <param name="pc"></param>
        /// <param name="usb"></param>
        private void SetConflictUserChoice(List<Conflicts> conflictList, bool pc, bool usb)
        {
            foreach (Conflicts conflict in conflictList)
            {
                conflict.PCSelected = pc;
                conflict.USBSelected = usb;
            }
        }

        /// <summary>
        /// Helper method to delete all folders corresponding to a job on removable device.
        /// </summary>
        /// <param name="usbJob"></param>
        private void DeleteUSBFolders(USBJob usbJob)
        {
            if (Directory.Exists(usbJob.AbsoluteUSBPath))
                ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);
            if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(usbJob));
            if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(usbJob));
            if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob));
        }
    }
}
