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
        const int ONE = 1;
        const int TWO = 2;
        const int OTHER = -1;

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

        //int num;
      
        public JobLogic()
        {
            PCJobs = new List<PCJob>();
            USBJobs = new List<USBJob>();
            compareLogic = new CompareLogic();
            sync = new SyncLogic();
            //num = n;
        }

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
            catch (Exception)
            {
                throw new Exception();
            }
        }

        internal void InitializeUSBJobInfo(string usbRoot,string pcID)
        {
      
            string[] storedUSBJobs = Directory.GetFiles(ReadAndWrite.GetStoredFolderOnUSB(usbRoot));
            
            if (storedUSBJobs.Length == 0)
            {
                return;
            }
            

            int originCount = USBJobs.Count;

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
                catch (Exception)
                {
                    throw new Exception();
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

                int pcNo = GetPCNumber(usbJob,pcID);

                if (pcNo == OTHER) continue;

                else if (pcNo == ONE)
                    i = CheckPCTwoDelete(i, usbJob);

                else if (pcNo == TWO)
                    i = CheckPCOneDelete(i, usbJob);
            }

        }

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
				if (Directory.Exists(usbJob.AbsoluteUSBPath))
                    ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);
                if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(usbJob));
                if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(usbJob));
                if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob));
            }
            return i;
        }

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
				if (Directory.Exists(usbJob.AbsoluteUSBPath))
                    ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);
                if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(usbJob));
                if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(usbJob));
                if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob)))
                    ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob));
            }
            return i;
        }

        private int GetPCNumber(USBJob usbJob,string pcID)
        {
            if (pcID.Equals(usbJob.PCOneID))
                return ONE;
            else if (pcID.Equals(usbJob.PCTwoID))
                return TWO;
            else return OTHER;
        }

        internal PCJob CreateJob(string jobName, string PCPath, string AbsoluteUSBPath, string PCID, JobConfig config)
        {
            if (CheckNameConflict(jobName,AbsoluteUSBPath)) return null;

            /*if (!Directory.Exists(AbsoluteUSBPath))
                ReadAndWrite.CreateDirectory(AbsoluteUSBPath);*/
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

                //ReadAndWrite.CreateTempStorageFolder(pcJob);

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
        internal void InsertJob(PCJob pcJob)
        {
            //if (CheckNameConflict(pcJob.JobName)) return;
            
            PCJobs.Add(pcJob);
   
            
            if(!Directory.Exists(pcJob.PCPath)){
                ReadAndWrite.CreateDirectory(pcJob.PCPath);
            }

            //Modified
   
            ReadAndWrite.ExportPCJob(pcJob);
        }

        internal void InsertJob(USBJob usbJob)
        {
       
            USBJobs.Add(usbJob);
    
            ReadAndWrite.ExportUSBJob(usbJob);
        }

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

            pcJob.GetUsbJob().diff = compareLogic.ConvertFolderMetaToDifferences(pcJob.FolderInfo);

            //ReadAndWrite.CreateTempStorageFolder(pcJob);

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

        private string GetFullFolderPath(PCJob pcJob, FolderMeta folderInfo)
        {
            return pcJob.PCPath + folderInfo.Path;
        }

        private void WriteIncompleteFileInfoOnUSB(USBJob usbJob)
        {
            ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }
        



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

        internal void USBPlugIn(string drive, string pcID)
        {
            if (Directory.Exists(ReadAndWrite.GetStoredFolderOnUSB(drive)))
                InitializeUSBJobInfo(drive,pcID);

        }

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

        

        

        internal ComparisonResult HandleConflicts(ComparisonResult comparisonResult)
        {
            ConflictHandler conflictHandler = new ConflictHandler();
            return conflictHandler.HandleConflicts(comparisonResult);
        }

        internal PCJob ConnectUSBJobwithPCJob(USBJob usbJob)
        {
            foreach (PCJob pcJob in PCJobs)
            {
                if (pcJob.JobName.Equals(usbJob.JobName))
                {
                    //pcJob.ToggleStatus(pcJob.JobState);
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

        internal void DeleteJob(PCJob pcJob,string pcID)
        {
            RemovePCJob(pcJob);

            if (pcJob.GetUsbJob() != null)
            {
                    
                if (pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                {
                    if (File.Exists(ReadAndWrite.GetIncompleteUSBFilePath(pcJob.GetUsbJob()))) 
                        ReadAndWrite.DeleteFile(ReadAndWrite.GetIncompleteUSBFilePath(pcJob.GetUsbJob()));
                    if (Directory.Exists(pcJob.AbsoluteUSBPath))
                        ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath);
                    if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob())))
                        ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob()));
                    if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob())))
                        ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob()));
                    if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(pcJob.GetUsbJob())))
                        ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(pcJob.GetUsbJob()));
                   
                    return;
                }
                USBJob usbJob = pcJob.GetUsbJob();
                if (pcID.Equals(usbJob.PCOneID))
                    usbJob.PCOneDeleted = true;
                else
                    usbJob.PCTwoDeleted = true;
                ReadAndWrite.ExportUSBJob(usbJob);
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

        private void RemovePCJob(PCJob pcJob)
        {
            
            PCJobs.Remove(pcJob);
            if (File.Exists(ReadAndWrite.GetStoredPathOnPC(pcJob)))
                ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnPC(pcJob));
            if (Directory.Exists(ReadAndWrite.GetPCTempFolder(pcJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetPCTempFolder(pcJob));
            /*if (Directory.Exists(ReadAndWrite.GetPCBackUpFolder(pcJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetPCBackUpFolder(pcJob));*/
            //ReadAndWrite.DeleteFolder(ReadAndWrite.GetTempStorageFolder(pcJob));
        }

        private void RemoveUSBJob(USBJob usbJob)
        {
            

            USBJobs.Remove(usbJob);

            ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnUSB(usbJob));
			
			if (Directory.Exists(usbJob.AbsoluteUSBPath))
            	ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);
            if (Directory.Exists(ReadAndWrite.GetUSBResyncDirectory(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncDirectory(usbJob));
            if (Directory.Exists(ReadAndWrite.GetUSBTempFolder(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBTempFolder(usbJob));
            if (Directory.Exists(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob)))
                ReadAndWrite.DeleteFolder(ReadAndWrite.GetUSBResyncBackUpDirectory(usbJob));
        }

        public void CheckJobStatus()
        {
            foreach (PCJob pcJob in PCJobs)
            {
                if (pcJob.JobState.Equals(JobStatus.Complete) && pcJob.GetUsbJob() == null)
                    pcJob.ToggleStatus(pcJob.JobState);
            }
        }


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

        private void SetConflictUserChoice(List<Conflicts> conflictList, bool pc, bool usb)
        {
            foreach (Conflicts conflict in conflictList)
            {
                conflict.PCSelected = pc;
                conflict.USBSelected = usb;
            }
        }
    }
}
