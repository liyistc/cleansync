using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using CleanSyncMini;

namespace CleanSyncMini
{
    [Serializable]
    class JobLogic
    {
        List<string> StoredPCJobInfoPaths;
        List<string> StoredUSBJobInfoPaths;
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
         
            StoredPCJobInfoPaths = new List<string>();
            StoredUSBJobInfoPaths = new List<string>();
            PCJobs = new List<PCJob>();
            USBJobs = new List<USBJob>();
            compareLogic = new CompareLogic();
            sync = new SyncLogic();
            //num = n;
        }

        internal void InitializePCJobInfo()
        {
            StoredPCJobInfoPaths = ReadAndWrite.ImportJobList();

            if (StoredPCJobInfoPaths == null)
            {
                StoredPCJobInfoPaths = new List<string>();
                PCJobs = new List<PCJob>();
                return;
            }

            PCJobs = ReadAndWrite.GetPCJobs(StoredPCJobInfoPaths);
        }

        internal void InitializeUSBJobInfo(string usbRoot,string pcID)
        {
            List<string> relativePaths = ReadAndWrite.GetUSBJobListPath(usbRoot);

            if (relativePaths == null)
            {
                relativePaths = new List<string>();
                USBJobs = new List<USBJob>();
                return;
            }

            foreach (string relativePath in relativePaths)
                StoredUSBJobInfoPaths.Add(usbRoot + relativePath);

            //StoredUSBJobInfoPaths = ReadAndWrite.GetUSBJobListPath(usbRoot);

            USBJobs = ReadAndWrite.GetUSBJobs(StoredUSBJobInfoPaths);

            foreach (USBJob usbJob in USBJobs)
                usbJob.AbsoluteUSBPath = usbRoot + usbJob.RelativeUSBPath;
            
            for (int i = 0; i < USBJobs.Count; i++)
            {
                USBJob usbJob = USBJobs.ElementAt(i);
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

        internal PCJob CreateJob(string jobName, string PCPath, string AbsoluteUSBPath, string PCID)
        {
            if (CheckNameConflict(jobName)) return null;

            if (!Directory.Exists(AbsoluteUSBPath))
                Directory.CreateDirectory(AbsoluteUSBPath);

            PCJob pcJob = new PCJob(jobName, PCPath, AbsoluteUSBPath, PCID);
            
            InsertJob(pcJob);
            
            return pcJob;
        }
        internal PCJob CreateJob(USBJob jobUSB, string PCPath, string PCID)
        {
            jobUSB.PCTwoPath = PCPath;
            PCJob pcJob = new PCJob(jobUSB,PCPath,PCID);
            pcJob.JobState = JobStatus.Complete;
            jobUSB.JobState = JobStatus.Complete;
            jobUSB.PCTwoID = PCID;
            //jobUSB.MostRecentPCID = PCID;
            InsertJob(pcJob);            
            
            InsertJob(jobUSB);
            
            ReadAndWrite.RemoveIncompleteUSBJob(jobUSB);
            
            return pcJob;
        }
        internal bool CheckNameConflict(string JobName)
        {
            foreach (PCJob pcJob in PCJobs)
                if (pcJob.JobName.Equals(JobName)) return true;
            foreach (USBJob usbJob in USBJobs)
                if (usbJob.JobName.Equals(JobName)) return true;
            
            return false;
        }
        internal void InsertJob(PCJob pcJob)
        {
            if (CheckNameConflict(pcJob.JobName)) return;
            PCJobs.Add(pcJob);
            StoredPCJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnPC(pcJob));
            
            if(!Directory.Exists(pcJob.PCPath)){
                Directory.CreateDirectory(pcJob.PCPath);
            }

            //Modified
            ReadAndWrite.ExportPCJobPathsList(StoredPCJobInfoPaths);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        internal void InsertJob(USBJob usbJob)
        {
       
            USBJobs.Add(usbJob);
            StoredUSBJobInfoPaths.Add(ReadAndWrite.GetStoredPathOnUSB(usbJob));
            ReadAndWrite.ExportUSBJobPathsList(StoredUSBJobInfoPaths,usbJob);
            ReadAndWrite.ExportUSBJob(usbJob);
        }

        internal bool setupJob(PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            FolderMeta folderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            if (folderInfo.Equals(null))
            {
                Console.WriteLine("Cannot build folder meta tree from path:"+pcJob.PCPath);
                return false;
            }

            pcJob.FolderInfo = folderInfo;
                
            ReadAndWrite.ExportPCJob(pcJob);

            USBJob usbJob = new USBJob(pcJob);

            pcJob.SetUsbJob(usbJob);
       
            usbJob.diff = compareLogic.ConvertFolderMetaToDifferences(pcJob.FolderInfo);

            sync.SyncPCToUSB(usbJob.diff, pcJob, worker);
            
            WriteIncompleteFileInfoOnUSB(usbJob);
                        
            return true;
            
        }

        private string GetFullFolderPath(PCJob pcJob, FolderMeta folderInfo)
        {
            return pcJob.PCPath + folderInfo.Path;
        }

        private void WriteIncompleteFileInfoOnUSB(USBJob usbJob)
        {
            ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }
        /*internal ComparisonResult AcceptSetupCompare(PCJob ContinuedJob)
        {
            ContinuedJob.FolderInfo = ReadAndWrite.BuildTree(ContinuedJob.PCPath);

            Differences newDiff = compareLogic.ConvertFolderMetaToDifferences(ContinuedJob.FolderInfo);
 
            List<Conflicts> firstConflict = compareLogic.ComparePCwithUSB(ContinuedJob.GetUsbJob().diff, newDiff);

            ComparisonResult result = new ComparisonResult(ContinuedJob.GetUsbJob().diff, newDiff, firstConflict);
         
            if (firstConflict.Count != 0)
            {
                Console.WriteLine("There is conflicts.");
                return result;
            }

            return result;
            
            //SyncLogic.CleanSync(result,ContinuedJob);
 
            //ReadAndWrite.ExportUSBJob(ContinuedJob.GetUsbJob());
        }*/




        internal void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {

            sync.CleanSync(comparisonResult, pcJob, worker);
            //ReadAndWrite.ExportPCJob(pcJob);

        }

        internal ComparisonResult Compare(PCJob pcJob)
        {
            FolderMeta currentFolderMeta = ReadAndWrite.BuildTree(pcJob.PCPath);
            FolderMeta storedFolderMeta = pcJob.FolderInfo;

            Differences pcDiff = compareLogic.CompareDirectories(currentFolderMeta, storedFolderMeta);

            List<Conflicts> conflict = compareLogic.ComparePCwithUSB(pcJob.GetUsbJob().diff, pcDiff);

            ComparisonResult comparisonResult = new ComparisonResult(pcJob.GetUsbJob().diff, pcDiff, conflict);

            return comparisonResult;
        }

        internal void USBPlugIn(string drive, string pcID)
        {
            if (Directory.Exists(ReadAndWrite.GetStoredFolderOnUSB(drive)))
            {
                InitializeUSBJobInfo(drive,pcID);

                /*if (Directory.Exists(ReadAndWrite.GetDeleteJobPCFolder()) &&
                    Directory.GetFiles(ReadAndWrite.GetDeleteJobPCFolder()).Length != 0)
                {
                    DeleteCorrespondingUSBJob(drive);
                }
                if (Directory.Exists(ReadAndWrite.GetDeleteJobUSBFolder(drive)) &&
                    Directory.GetFiles(ReadAndWrite.GetDeleteJobUSBFolder(drive)).Length != 0)

                    DeleteCorrespondingPCJob(drive, pcID);*/

                //ConnectPCJobwithUSBJob();
            }

        }

        internal void USBRemoved()
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
        }

        internal ComparisonResult handleConflicts(ComparisonResult comparisonResult, int[] userChoice)
        {
            ConflictHandler conflictHandler = new ConflictHandler();
            return conflictHandler.handleConflicts(comparisonResult, userChoice);
        }

        internal PCJob ConnectUSBJobwithPCJob(USBJob usbJob)
        {
            foreach (PCJob pcJob in PCJobs)
            {
                if (pcJob.JobName.Equals(usbJob.JobName))
                {
                    //pcJob.ToggleStatus(pcJob.JobState);
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
                    
                if (pcJob.JobState.Equals(JobStatus.Incomplete))
                {
                    ReadAndWrite.DeleteFile(ReadAndWrite.GetIncompleteUSBFilePath(pcJob.GetUsbJob()));
                    ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath);
                    return;
                }
                USBJob usbJob = pcJob.GetUsbJob();
                if (pcID.Equals(usbJob.PCOneID))
                    usbJob.PCOneDeleted = true;
                else
                    usbJob.PCTwoDeleted = true;
                ReadAndWrite.ExportUSBJob(usbJob);
            }

        }

        private void RemovePCJob(PCJob pcJob)
        {
            StoredPCJobInfoPaths.Remove(ReadAndWrite.GetStoredPathOnPC(pcJob));
            ReadAndWrite.ExportPCJobPathsList(StoredPCJobInfoPaths);

            PCJobs.Remove(pcJob);
            ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnPC(pcJob));
        }

        private void RemoveUSBJob(USBJob usbJob)
        {
            StoredUSBJobInfoPaths.Remove(ReadAndWrite.GetStoredPathOnUSB(usbJob));
            ReadAndWrite.ExportUSBJobPathsList(StoredUSBJobInfoPaths, usbJob);

            USBJobs.Remove(usbJob);

            ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnUSB(usbJob));

            ReadAndWrite.DeleteFolder(usbJob.AbsoluteUSBPath);

        }
       
        /*private void DeleteCorrespondingPCJob(string usbRoot,string pcID)
        {
            List<USBJob> deletedUSBJobs = ReadAndWrite.GetUSBJobList(ReadAndWrite.GetDeleteJobUSBFolder(usbRoot));

            foreach (USBJob usbJob in deletedUSBJobs)
            {
                if (!usbJob.DeletePCID.Equals(pcID)) 
                    continue;
                for (int i = 0; i < PCJobs.Count; i++)
                {
                    if (PCJobs[i].JobName.Equals(usbJob.JobName))
                    {
                        usbJob.AbsoluteUSBPath = usbRoot + usbJob.RelativeUSBPath; 
                        StoredPCJobInfoPaths.Remove(ReadAndWrite.GetStoredPathOnPC(PCJobs[i]));
                        ReadAndWrite.ExportPCJobPathsList(StoredPCJobInfoPaths);
                        ReadAndWrite.DeleteFile(ReadAndWrite.GetStoredPathOnPC(PCJobs[i]));
                        PCJobs.RemoveAt(i);
                        ReadAndWrite.DeleteFile(ReadAndWrite.GetDeletedUSBJobPath(usbJob));
                        break;
                    }
                }
         
            }
        }*/

        /*private void DeleteCorrespondingUSBJob(string drive)
        {
            List<PCJob> deletedPCJobs = ReadAndWrite.GetPCJobList(ReadAndWrite.GetDeleteJobPCFolder());
            List<USBJob> incomplete;
            
            for (int i = 0; i < deletedPCJobs.Count; i++)
            {
                bool flag = false;
                if (deletedPCJobs.ElementAt(i).JobState.Equals(JobStatus.Incomplete))
                {
                    incomplete = ReadAndWrite.GetIncompleteUSBJobList(drive);
                    for (int j=0;j<incomplete.Count; j++)
                    {
                        if (incomplete[j].JobName.Equals(deletedPCJobs.ElementAt(i).JobName) &&
                            (incomplete[j].PCOneID.Equals(deletedPCJobs.ElementAt(i).PCID) || 
                            incomplete[j].PCTwoID.Equals(deletedPCJobs.ElementAt(i).PCID)))
                        {
                            incomplete[j].AbsoluteUSBPath = drive + incomplete[j].RelativeUSBPath;                           
                            ReadAndWrite.DeleteFile(ReadAndWrite.GetIncompleteUSBFilePath(incomplete[j]));
                            ReadAndWrite.DeleteFile(ReadAndWrite.GetDeletedPCJobPath(deletedPCJobs.ElementAt(i)));
                            incomplete.RemoveAt(j);
                            //ReadAndWrite.DeleteFile(ReadAndWrite.GetDeletedPCJobPath(deletedPCJobs.ElementAt(i)));
                            j--;
                            flag = true;
                            break;
                        }
                    }
                }

                if (flag) continue;
                for (int k=0;k<USBJobs.Count;k++)
                {
                    if (USBJobs[k].JobName.Equals(deletedPCJobs.ElementAt(i).JobName) &&
                        (USBJobs[k].PCOneID.Equals(deletedPCJobs.ElementAt(i).PCID) || 
                        USBJobs[k].PCTwoID.Equals(deletedPCJobs.ElementAt(i).PCID)))
                    {
                        RemoveUSBJob(USBJobs[k],deletedPCJobs.ElementAt(i).PCID);
                        k--;
                        ReadAndWrite.DeleteFile(ReadAndWrite.GetDeletedPCJobPath(deletedPCJobs.ElementAt(i)));
                    }
                }
            }
        }*/
    }
}
