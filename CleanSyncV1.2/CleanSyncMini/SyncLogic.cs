using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CleanSync;
using Exceptions;

namespace DirectoryInformation
{

    public class SyncLogic
    {
        private System.ComponentModel.BackgroundWorker bgWorker;

        private double onePercentSize;
        private long totalSize;
        private string backupDirectory;
        private string ReSyncTempUsb;
        private string tempPC;
        private string tempUSB;
        private string reSyncTempUsbBackup;

        private DifferenceToTreeConvertor convertor;

        public SyncLogic()
        {
            totalSize = 0;
            onePercentSize = 0;
            backupDirectory = "";
            ReSyncTempUsb = "";
            tempPC = "";
            tempUSB = "";
            reSyncTempUsbBackup = "";
            convertor = new DifferenceToTreeConvertor();
        }

        #region Entry Methods
        /// <summary>
        /// Do a clean synchronization
        /// </summary>
        /// <param name="comparisonResult">The comparison result after comparison</param>
        /// <param name="pcJob">pcJob of the computer to be synchronized</param>
        /// <param name="worker">worker to update the progress bar of the GUI</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            bgWorker = worker;
            try
            {
                //CreateTempFolders(pcJob);
                SetTempFolderPaths(pcJob);

              /*  if (pcJob.GetUsbJob().Synchronizing)
                {
                    RestoreInterruptedUSB(pcJob.GetUsbJob());
                }
                if (pcJob.GetUsbJob().ReSynchronizing)
                {
                    RestoreReSyncUSB(pcJob);
                }
                else
                {*/
                    ReadAndWrite.DeleteFolderContent(tempPC);
                    ReadAndWrite.DeleteFolderContent(tempUSB);
                    ReadAndWrite.DeleteFolderContent(ReSyncTempUsb);
               // }

                Differences PCToUSBDone = new Differences();
                Differences USBToPCDone = new Differences();

                if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID) //this is the most recent PC to sync, so do a re-synchronization
                {
                    initializeTotalSize(comparisonResult, true);
                    CleanSyncReSync(comparisonResult, pcJob);
                }
                else
                {
                    initializeTotalSize(comparisonResult, false);
                    SetBackupFolder(pcJob);
                    NormalCleanSync(comparisonResult, pcJob);  //the other PC was the most recent PC to sync, so do a normal synchronization
                }
                this.bgWorker = null;
                totalSize = 0;
                onePercentSize = 0;
                backupDirectory = "";
            }
            catch (Exception)
            {
                this.bgWorker = null;
                totalSize = 0;
                onePercentSize = 0;
                backupDirectory = "";
                throw;
            }
        }



        /// <summary>
        /// Do a fist time synchronization
        /// </summary>
        /// <param name="PCToUSB">The difference to use to copy</param>
        /// <param name="pcJob">pcJob of the computer to be synchronized</param>
        /// <param name="worker">worker to update the progress bar of the GUI</param>
        public void SyncPCToUSB(Differences PCToUSB, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            /*TestingCode*/

            /*   Differences PCToUSBDone = new Differences();
               Differences USBToPCDone = new Differences();
         
             * */
            //end test
            bgWorker = worker;
            InitializeTotalSizeFirstTimeSynchronization(PCToUSB);

            try
            {
                SetTempFolderPaths(pcJob);

                Differences pcToUSBDone = new Differences();
                pcJob.FolderInfo = new FolderMeta(pcJob.PCPath, pcJob.PCPath); //create an empty tree structure, next time sync will be equivalent to 1st time sync.
                ReadAndWrite.ExportPCJob(pcJob);

                pcJob.GetUsbJob().Synchronizing = true;
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
                pcJob.GetUsbJob().diff = new Differences();
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                ReadAndWrite.CreateDirectory(pcJob.AbsoluteUSBPath);
                FolderMeta root = convertor.ConvertDifferencesToTreeStructure(PCToUSB);

                NormalCleanSyncFolderPcToUsb(root, pcJob.PCPath, tempUSB, pcToUSBDone);

                ReadAndWrite.MoveFolderContents(tempUSB, pcJob.AbsoluteUSBPath);
                pcJob.GetUsbJob().diff = PCToUSB;
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
                ReadAndWrite.ExportPCJob(pcJob);
            }
            catch (Exception)
            {
                this.bgWorker = null;
                backupDirectory = "";
                throw;
            }
            this.bgWorker = null;
            backupDirectory = "";
        }

        #endregion
        
        #region restoration
        private void RestoreIncompletePCChanges(FolderMeta changes, string pcPath)
        {
            foreach (FileMeta file in changes.files)
            {
                switch (file.FileType)
                {
                    case ComponentMeta.Type.New: ReadAndWrite.DeleteFile(pcPath + file.Path + file.Name);
                        break;
                    case ComponentMeta.Type.Modified: ReadAndWrite.DeleteFile(pcPath + file.Path + file.Name);
                        ReadAndWrite.MoveFile(backupDirectory + file.Path + file.Name, pcPath + file.Path + file.Name);
                        break;
                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(backupDirectory + file.Path + file.Name, pcPath + file.Path + file.Name);
                        break;
                }
            }
            foreach (FolderMeta folder in changes.folders)
            {
                switch (folder.FolderType)
                {
                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(folder, backupDirectory, pcPath);
                        break;
                    case ComponentMeta.Type.New: ReadAndWrite.DeleteFolder(folder, pcPath);
                        break;
                    case ComponentMeta.Type.Modified: RestoreIncompletePCChanges(folder, pcPath);
                        break;
                }
            }
        }

        private void RestoreInterruptedPCJobPCChanges(PCJob pcJob)
        {
            ReadAndWrite.MoveFolderContentWithReplace(backupDirectory, pcJob.PCPath);
        }

        public void RestoreInterruptedUSB(USBJob usbJob)
        {
            if (usbJob.SynchronizingPcToUSB)
            {
                ReadAndWrite.DeleteFolderContent(tempUSB);
                usbJob.SynchronizingPcToUSB = false;
            }
            else if (usbJob.SynchronizingUSBToPC)
            {
                usbJob.SynchronizingUSBToPC = false;
            }
            else if (usbJob.MovingOldDiffToTemp) //sync PCtoUSB done, USBtoPC done, have not move folder over
            {
                ReadAndWrite.MoveFolderContentWithReplace(ReSyncTempUsb, usbJob.AbsoluteUSBPath);
            }
            else // usbJob.MovingTemptoOldDiff
            {
                ReadAndWrite.MoveFolderContentWithReplace(usbJob.AbsoluteUSBPath, tempUSB);
                ReadAndWrite.MoveFolderContents(ReSyncTempUsb, usbJob.AbsoluteUSBPath);
            }
            usbJob.Synchronizing = false;
            ReadAndWrite.ExportUSBJob(usbJob);
        }

        public void RestoreReSyncUSB(PCJob pcJob)
        {
            USBJob usbJob = pcJob.GetUsbJob();
            if (!usbJob.RecoveryPossible) throw new RecoveryNotPossibleException();

            ReadAndWrite.DeleteFolderContent(tempUSB);
            usbJob.ReSynchronizing = false;
            if (!usbJob.JobState.Equals(JobStatus.Incomplete))
                ReadAndWrite.ExportUSBJob(usbJob);
            else
                ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }

        //private void UndoUSBResyncFolder(FolderMeta oldDifferencesOriginal, FolderMeta changedDifferences, string pathUSB)
        //{
        //    RestoreUSBResyncFiles(oldDifferencesOriginal, changedDifferences, pathUSB);

        //    List<FolderMeta> originalFolders = oldDifferencesOriginal.folders;
        //    List<FolderMeta> changedFolders = changedDifferences.folders;

        //    int origIndex = 0;
        //    int changedIndex = 0;
        //    int origSize = originalFolders.Count;
        //    int changedSize = changedFolders.Count;

        //    while (origIndex < origSize && changedIndex < changedSize)
        //    {
        //        FolderMeta origFolder = originalFolders[origIndex];
        //        FolderMeta changedFolder = changedFolders[changedIndex];

        //        if (origFolder < changedFolder)  //originally modified/new, now deleted
        //        {
        //            ReadAndWrite.MoveFileOrFolder(ReSyncTempUsb + origFolder.Path + origFolder.Name, pathUSB + origFolder.Path + origFolder.Name);
        //            origIndex++;
        //        }
        //        else if (changedFolder < origFolder)   //new difference, just undo it.
        //        {
        //            if(changedFolder.FolderType != ComponentMeta.Type.Deleted)
        //            {
                        

        //        }
        //        else
        //        {
                    
        //        }
        //    }
        //}

        //private void RestoreUSBResyncFiles(FolderMeta oldDifferencesOriginal, FolderMeta changedDifferences, string pathUSB)
        //{
        // List<FileMeta> originalFiles = oldDifferencesOriginal.files;
        // List<FileMeta> changedFiles = changedDifferences.files;

        // int origIndex = 0;
        // int changedIndex = 0;
        // int origSize = originalFiles.Count;
        // int changedSize = changedFiles.Count;

        // while (origIndex < origSize && changedIndex < changedSize)
        // {
        //     FileMeta origFile = originalFiles[origIndex];
        //     FileMeta changedFile = changedFiles[changedIndex];

        //     if (origFile < changedFile) //originall modified/new, now deleted
        //     {
        //         ReadAndWrite.MoveFileOrFolder(ReSyncTempUsb + origFile.Path + origFile.Name, pathUSB + origFile.Path + origFile.Name);
        //         origIndex++;
        //     }
        //     else if (changedFile < origFile)  //changed file is new file difference added
        //     {
        //         if (changedFile.FileType != ComponentMeta.Type.Deleted)
        //         {
        //             ReadAndWrite.MoveFileOrFolder(pathUSB + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //             ReadAndWrite.MoveFileOrFolder(ReSyncTempUsb + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //         }
        //         changedIndex++;
        //     }
        //     else
        //     {
        //         switch (origFile.FileType)
        //         {
        //             case ComponentMeta.Type.Deleted: //originally deleted, now created a new file with that name, and set to modified
        //                 if (changedFile.FileType != ComponentMeta.Type.Deleted)
        //                     ReadAndWrite.MoveFileOrFolder(pathUSB + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //                 break;
        //             case ComponentMeta.Type.New:
        //                 if (changedFile.LastModifiedTime > origFile.LastModifiedTime)  //file was replaced
        //                     ReadAndWrite.MoveFileOrFolder(pathUSB + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //                 ReadAndWrite.MoveFileOrFolder(backupDirectory + changedFile.Path + changedFile.Name, pathUSB + changedFile.Path + changedFile.Name);
        //                 break;
        //             case ComponentMeta.Type.New:
        //                 if (changedFile.LastModifiedTime > origFile.LastModifiedTime)  //file was replaced
        //                     ReadAndWrite.MoveFileOrFolder(pathUSB + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //                 ReadAndWrite.MoveFileOrFolder(backupDirectory + changedFile.Path + changedFile.Name, pathUSB + changedFile.Path + changedFile.Name);
        //                 break;
        //         }
        //         changedIndex++;
        //         origIndex++;
        //     }
        // }
        // while (changedIndex < changedSize)
        // {
        //     FileMeta changedFile = changedFiles[changedIndex++];
        //     if (changedFile.FileType != ComponentMeta.Type.Deleted)
        //     {
        //         ReadAndWrite.MoveFileOrFolder(pathUSB + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //         ReadAndWrite.MoveFileOrFolder(ReSyncTempUsb + changedFile.Path + changedFile.Name, tempUSB + changedFile.Path + changedFile.Name);
        //     }
        // }
        // while (origIndex < origSize)
        // {
        //     FileMeta origFile = originalFiles[origIndex];
        //     ReadAndWrite.MoveFileOrFolder(ReSyncTempUsb + origFile.Path + origFile.Name, pathUSB + origFile.Path + origFile.Name);
        // }
        //}
        #endregion
        
        #region creatingBackupFolder

        private void SetBackupFolder(PCJob pcJob)
        {
            backupDirectory = ReadAndWrite.GetPCBackUpFolder(pcJob);
            ReadAndWrite.DeleteFolderContent(backupDirectory);
        }

        private void SetTempFolderPaths(PCJob pcJob)
        {
            tempPC = ReadAndWrite.GetPCTempFolder(pcJob);
            tempUSB = ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob());
            ReSyncTempUsb = ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob());
            reSyncTempUsbBackup = ReadAndWrite.GetUSBRootPath(pcJob.AbsoluteUSBPath) + @"\_CleanSync_Data_\_cs_job_data\reSyncTempUsbBackup" + @"\" + pcJob.JobName;
        }

    /*    private void CreateTempFolders(PCJob pcJob)
        {
            tempPC = ReadAndWrite.GetMyDocumentsDirectory() + @"\CleanSync";
            ReadAndWrite.CreateDirectory(tempPC);

            tempUSB = ReadAndWrite.GetUSBRootPath(pcJob.AbsoluteUSBPath) + @"\_CleanSync_Data_\_cs_job_data";
            ReSyncTempUsb = tempUSB;
            reSyncTempUsbBackup = tempUSB;
            tempPC = tempPC + @"\" + "temp";
            tempUSB = tempUSB + @"\" + "temp";
            ReSyncTempUsb += @"\" + "resync temp";
            reSyncTempUsbBackup += @"\" + "reSyncTempUsbBackup";

            ReadAndWrite.CreateDirectory(tempPC);
            ReadAndWrite.CreateDirectory(tempUSB);
            ReadAndWrite.CreateDirectory(ReSyncTempUsb);
            ReadAndWrite.CreateDirectory(reSyncTempUsbBackup);

            tempPC = tempPC + @"\" + pcJob.JobName;
            tempUSB = tempUSB + @"\" + pcJob.JobName;
            ReSyncTempUsb += @"\" + pcJob.JobName;
            reSyncTempUsbBackup += @"\" + pcJob.JobName;
            ReadAndWrite.CreateDirectory(tempPC);
            ReadAndWrite.CreateDirectory(tempUSB);
            ReadAndWrite.CreateDirectory(ReSyncTempUsb);
            ReadAndWrite.CreateDirectory(reSyncTempUsbBackup);
        }
     * */
        #endregion
        
        #region normalSync

        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta pcToUsb = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
            FolderMeta usbToPc = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
            Differences usbToPcDone = new Differences();
            Differences pcToUSBDone = new Differences();

            try
            {
                //Sync PC -> USB temp folder
                pcJob.GetUsbJob().SynchronizingPcToUSB = true;
                pcJob.GetUsbJob().Synchronizing = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                NormalCleanSyncFolderPcToUsb(pcToUsb, pcJob.PCPath, tempUSB, pcToUSBDone);

                //Sync USB -> PC 
                pcJob.GetUsbJob().SynchronizingPcToUSB = false;
                pcJob.GetUsbJob().SynchronizingUSBToPC = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());

                ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath, tempPC, bgWorker, onePercentSize);
                NormalCleanSyncFolderUsbToPC(usbToPc, tempPC, pcJob.PCPath, usbToPcDone);
                pcJob.GetUsbJob().SynchronizingUSBToPC = false;
				pcJob.GetUsbJob().MovingOldDiffToTemp = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (SyncCancelledException)
            {
                if (pcJob.GetUsbJob().SynchronizingUSBToPC)
                {
                    FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                    RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);
                    ReadAndWrite.DeleteFolderContent(backupDirectory);
                }
                ReadAndWrite.DeleteFolderContent(tempUSB);
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
				pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);
                return;
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                //move USB temp folder to the correct folder
                ReadAndWrite.MoveFolderContents(pcJob.AbsoluteUSBPath, ReSyncTempUsb); 
                pcJob.GetUsbJob().MovingOldDiffToTemp = false;
                pcJob.GetUsbJob().MovingTempToOldDiff = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (SyncCancelledException)
            {
                FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);
                ReadAndWrite.DeleteFolderContent(backupDirectory);
                ReadAndWrite.MoveFolderContentWithReplace(ReSyncTempUsb, pcJob.AbsoluteUSBPath);
                ReadAndWrite.DeleteFolderContent(tempUSB);
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);
                return;
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                ReadAndWrite.MoveFolderContents(tempUSB, pcJob.AbsoluteUSBPath);

                pcJob.GetUsbJob().MovingTempToOldDiff = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (SyncCancelledException)
            {
                FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);

                ReadAndWrite.DeleteFolderContent(backupDirectory);

                ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
                ReadAndWrite.MoveFolderContentWithReplace(ReSyncTempUsb, pcJob.AbsoluteUSBPath);
                ReadAndWrite.DeleteFolderContent(tempUSB);

                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);
                return;
            }
            catch (Exception)
            {
                throw;
            }
            

            pcJob.GetUsbJob().diff = comparisonResult.PCDifferences;     //set the new difference on usbJob
            //backup 

            //backup 
            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC
            pcJob.GetUsbJob().Synchronizing = false;
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());

            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            pcJob.Synchronizing = false;
            ReadAndWrite.ExportPCJob(pcJob); 

            ReadAndWrite.DeleteFolderContent(ReSyncTempUsb);
            ReadAndWrite.DeleteFolderContent(tempUSB);
        }
        private void NormalCleanSyncFolderPcToUsb(FolderMeta pcToUsb, string originDirectoryRoot, string destinationDirectoryRoot, Differences pcToUsbDone)
        {
            try
            {
                foreach (FileMeta file in pcToUsb.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New:
                            ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                            pcToUsbDone.AddNewFileDifference(file);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                            pcToUsbDone.AddModifiedFileDifference(file);
                            break;
                    }
                }
                foreach (FolderMeta folder in pcToUsb.folders)
                {
                    switch (folder.FolderType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.CopyFolder(folder, originDirectoryRoot, destinationDirectoryRoot , bgWorker, onePercentSize);
                            pcToUsbDone.AddNewFolderDifference(folder);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(destinationDirectoryRoot + folder.Path + folder.Name);
                            NormalCleanSyncFolderPcToUsb(folder, originDirectoryRoot, destinationDirectoryRoot, pcToUsbDone);
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {

                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void NormalCleanSyncFolderUsbToPC(FolderMeta usbToPC, string originDirectoryRoot, string destinationDirectoryRoot, Differences usbToPCDone)
        {
            if (usbToPC.files.Count == 0 && usbToPC.folders.Count == 0) ReadAndWrite.DeleteFolder(backupDirectory + usbToPC.Path + usbToPC.Name);
            try
            {
                foreach (FileMeta file in usbToPC.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New:
                            ReadAndWrite.createParentFolders(file, destinationDirectoryRoot);
                            ReadAndWrite.MoveFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name);
                            usbToPCDone.AddNewFileDifference(file);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.MoveFile(destinationDirectoryRoot + file.Path + file.Name, backupDirectory + file.Path + file.Name);
                            ReadAndWrite.MoveFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name);
                            usbToPCDone.AddModifiedFileDifference(file);
                            break;
                        case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(destinationDirectoryRoot + file.Path + file.Name, backupDirectory + file.Path + file.Name);
                            usbToPCDone.AddDeletedFileDifference(file);
                            break;
                    }

                }

                foreach (FolderMeta folder in usbToPC.folders)
                {
                    switch (folder.FolderType)
                    {
                        case ComponentMeta.Type.New:
                            ReadAndWrite.createParentFolders(folder, destinationDirectoryRoot);
                            ReadAndWrite.MoveFolder(folder, originDirectoryRoot, destinationDirectoryRoot);
                            break;
                        case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(folder, destinationDirectoryRoot, backupDirectory);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(backupDirectory + folder.Path + folder.Name);
                            NormalCleanSyncFolderUsbToPC(folder, originDirectoryRoot, destinationDirectoryRoot, usbToPCDone);
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {

                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
     
        #region ReSync
        private void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {

            FolderMeta oldDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
            FolderMeta newDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
            FolderMeta oldDifferencesOriginal = new FolderMeta(oldDifferencesRoot);
            Differences pcToUSBDone = new Differences();
            try
            {
                pcJob.GetUsbJob().ReSynchronizing = true;
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                //Copy files to USB
                NormalCleanSyncFolderPcToUsb(newDifferencesRoot, pcJob.PCPath, tempUSB, pcToUSBDone);

                //Files copied to US
                pcJob.GetUsbJob().RecoveryPossible = false;
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                ReSynchronizeFolders(oldDifferencesRoot, newDifferencesRoot, tempUSB, pcJob.AbsoluteUSBPath, pcToUSBDone);
                pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);

                ReadAndWrite.ExportPCJob(pcJob);
                Differences oldDifferences = convertor.ConvertTreeStructureToDifferences(oldDifferencesRoot);
                pcJob.GetUsbJob().diff = oldDifferences;
                pcJob.GetUsbJob().RecoveryPossible = true;
                pcJob.GetUsbJob().ReSynchronizing = false;
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
            }
            catch (SyncCancelledException)
            {
                oldDifferencesRoot.sortComponents();
                //   UndoUSBResyncFolder(oldDifferencesOriginal, oldDifferencesRoot, pcJob.AbsoluteUSBPath);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ReSynchronizeFolders(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string sourceDirectory, string destinationDirectory, Differences pcToUSBDone)
        {
            try
            {
               // ReadAndWrite.CreateDirectory(ReSyncTempUsb + oldDifferencesRoot.Path);
                ReSynchronizeFiles(oldDifferencesRoot, newDifferencesRoot, sourceDirectory, destinationDirectory);
                List<FolderMeta> foldersOld = oldDifferencesRoot.folders;
                List<FolderMeta> foldersNew = newDifferencesRoot.folders;
                int oldIndex = 0;
                int newIndex = 0;
                int oldCount = foldersOld.Count;
                int newCount = foldersNew.Count;

                while (oldIndex < oldCount && newIndex < newCount)
                {
                    FolderMeta folderOld = foldersOld[oldIndex];
                    FolderMeta folderNew = foldersNew[newIndex];

                    if (folderOld < folderNew) //folderOld difference is not changed
                    {
                        oldIndex++;
                    }
                    else if (folderNew < folderOld)
                    {
                        switch (folderNew.FolderType)
                        {
                            case ComponentMeta.Type.New:
                                ReadAndWrite.MoveFolder(sourceDirectory + folderNew.Path + folderNew.Name, destinationDirectory + folderNew.Path + folderNew.Name);
                                foldersOld.Add(folderNew);
                                break;
                            case ComponentMeta.Type.Deleted: foldersOld.Add(folderNew);
                                break;
                            case ComponentMeta.Type.Modified: foldersOld.Add(folderNew);
                                ReadAndWrite.MoveFolder(sourceDirectory + folderNew.Path + folderNew.Name, destinationDirectory + folderNew.Path + folderNew.Name);
                                break;
                        }
                        newIndex++;
                    }
                    else
                    {
                        switch (folderOld.FolderType)
                        {
                            case ComponentMeta.Type.New:
                                switch (folderNew.FolderType)
                                {
                                    //folderOld: new, folderNew: deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(destinationDirectory + folderOld.Path + folderOld.Name, ReSyncTempUsb + folderOld.Path + folderOld.Name);
                                        foldersOld[oldIndex] = folderNew;
                                        break;
                                    //folderOld: new, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(ReSyncTempUsb + folderOld.Path + folderOld.Name);
                                     ReSynchronizeToNewFolder(folderOld, folderNew, sourceDirectory, destinationDirectory);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Modified:
                                switch (folderNew.FolderType)
                                {
                                    //folderOld: modified, folderNew: deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(destinationDirectory + folderOld.Path + folderOld.Name, ReSyncTempUsb + folderOld.Path + folderOld.Name);
                                        RemoveSubDeletedOriginallyNew(folderNew);
                                        foldersOld[oldIndex] = folderNew;
                                        break;
                                    //folderOld: modified, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(ReSyncTempUsb + folderOld.Path + folderOld.Name);
                                        ReSynchronizeFolders(folderOld, folderNew, sourceDirectory, destinationDirectory, pcToUSBDone);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Deleted: //originally was deleted, now re-created a folder with the same name
                                folderOld.FolderType = ComponentMeta.Type.Modified;
                                ReadAndWrite.CreateDirectory(ReSyncTempUsb + folderOld.Path + folderOld.Name);
                                ReSynchronizeFolders(folderOld, folderNew, sourceDirectory, destinationDirectory, pcToUSBDone);
                                break;
                        }
                        newIndex++;
                        oldIndex++;
                    }
                }
                while (newIndex < newCount)
                {
                    FolderMeta folderNew = foldersNew[newIndex];
                    switch (folderNew.FolderType)
                    {
                        case ComponentMeta.Type.Modified: ReadAndWrite.MoveFolder(sourceDirectory + folderNew.Path + folderNew.Name, destinationDirectory + folderNew.Path + folderNew.Name);
                            foldersOld.Add(folderNew);
                            break;
                        case ComponentMeta.Type.Deleted: foldersOld.Add(folderNew);
                            break;
                        default: foldersOld.Add(folderNew);
                            ReadAndWrite.MoveFolder(sourceDirectory + folderNew.Path + folderNew.Name, destinationDirectory + folderNew.Path + folderNew.Name);
                            break;
                    }
                    newIndex++;
                }
                FolderMeta.ClearFolderList(foldersOld);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void RemoveSubDeletedOriginallyNew(FolderMeta folderNew)
        {
            for (int i = 0; i < folderNew.folders.Count; i++)
            {
                FolderMeta subfolder = folderNew.folders[i];
                if (subfolder.FolderType == ComponentMeta.Type.New)
                {
                    folderNew.folders[i] = null;
                }
                else
                {
                    subfolder.FolderType = ComponentMeta.Type.Deleted;
                    RemoveSubDeletedOriginallyNew(subfolder);
                }
            }
            for (int i = 0; i < folderNew.files.Count; i++)
            {
                FileMeta subfile = folderNew.files[i];
                if (subfile.FileType == ComponentMeta.Type.New)
                {
                    folderNew.files[i] = null;
                }
                else
                {
                    subfile.FileType = ComponentMeta.Type.Deleted;
                }
            }
            FolderMeta.ClearFolderList(folderNew.folders);
            FolderMeta.ClearFileList(folderNew.files);
        }

        private bool ReSynchronizeToNewFolder(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
        {
            try
            {
                bool changeType = false;
            //    ReadAndWrite.CreateDirectory(ReSyncTempUsb + oldDifferencesRoot.Path);
                changeType = ReSynchronizeToNewFolderFiles(oldDifferencesRoot, newDifferencesRoot, pathPC, pathUSB);

                List<FolderMeta> foldersOld = oldDifferencesRoot.folders;
                List<FolderMeta> foldersNew = newDifferencesRoot.folders;

                int indexOld = 0;
                int indexNew = 0;
                int foldersOldSize = foldersOld.Count;
                int foldersNewSize = foldersNew.Count;

                while (indexOld < foldersOldSize && indexNew < foldersNewSize)
                {
                    FolderMeta folderOld = foldersOld[indexOld];
                    FolderMeta folderNew = foldersNew[indexNew];

                    if (folderOld < folderNew)
                    {
                        indexOld++;
                    }
                    else if (folderNew < folderOld)
                    {
                        if(folderNew.FolderType != ComponentMeta.Type.New) changeType = true;
                        foldersOld.Add(folderNew);
                        ReadAndWrite.MoveFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (folderNew.FolderType)
                        {
                            case ComponentMeta.Type.Deleted: foldersOld[indexOld] = null;
                                ReadAndWrite.MoveFolder(pathUSB + folderNew.Path + folderNew.Name, ReSyncTempUsb + folderNew.Path + folderOld.Name);
                                break;
                            case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(ReSyncTempUsb + folderNew.Path + folderNew.Name);
                                if (ReSynchronizeToNewFolder(folderOld, folderNew, pathPC, pathUSB)) changeType = true;
                                break;
                        }
                        indexOld++;
                        indexNew++;
                    }
                }
                while (indexNew < foldersNewSize)
                {
                    FolderMeta folderNew = foldersNew[indexNew++];
                    foldersOld.Add(folderNew);
                    ReadAndWrite.MoveFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                }
                FolderMeta.ClearFolderList(foldersOld);
                if (changeType) oldDifferencesRoot.FolderType = ComponentMeta.Type.Modified;
                return changeType;
            }
            catch (System.IO.FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private bool ReSynchronizeToNewFolderFiles(FolderMeta folderOld, FolderMeta folderNew, string pathPC, string pathUSB)
        {
            try
            {
                bool changeType = false;
                List<FileMeta> filesOld = folderOld.files;
                List<FileMeta> filesNew = folderNew.files;

                int indexOld = 0;
                int indexNew = 0;
                int filesOldSize = filesOld.Count;
                int filesNewSize = filesNew.Count;

                while (indexOld < filesOldSize && indexNew < filesNewSize)
                {
                    FileMeta fileOld = filesOld[indexOld];
                    FileMeta fileNew = filesNew[indexNew];

                    if (fileOld < fileNew)
                    {
                        indexOld++;
                    }
                    else if (fileNew < fileOld) //the fileNew is a new file.
                    {
                        if (fileNew.FileType != ComponentMeta.Type.New) changeType = true;
                        filesOld.Add(fileNew);
                        ReadAndWrite.MoveFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (fileNew.FileType)
                        {
                            case ComponentMeta.Type.Modified:
                                ReadAndWrite.MoveFile(pathUSB + fileNew.Path + fileNew.Name, ReSyncTempUsb + fileNew.Path + fileNew.Name);
                                ReadAndWrite.MoveFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name);
                                filesOld[indexOld] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.New;
                                break;
                            case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(pathUSB + fileOld.Path + fileOld.Name, ReSyncTempUsb + fileNew.Path + fileNew.Name);
                                filesOld[indexOld] = null;
                                break;
                        }
                        indexOld++;
                        indexNew++;
                    }
                }
                while (indexNew < filesNewSize) // rest of the new files
                {
                    if (filesNew[indexNew].FileType != ComponentMeta.Type.New) changeType = true;
                    filesOld.Add(filesNew[indexNew]);
                    filesNew[indexNew].FileType = ComponentMeta.Type.New;
                    ReadAndWrite.MoveFile(pathPC + filesNew[indexNew].Path + filesNew[indexNew].Name, pathUSB + filesNew[indexNew].Path + filesNew[indexNew].Name);
                    indexNew++;
                }
                FolderMeta.ClearFileList(filesOld);
                return changeType;
            }
            catch (System.IO.FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ReSynchronizeFiles(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
        {
            try
            {
                List<FileMeta> filesOld = oldDifferencesRoot.files;
                List<FileMeta> filesNew = newDifferencesRoot.files;

                int oldIndex = 0;
                int newIndex = 0;
                int oldLength = filesOld.Count;
                int newLength = filesNew.Count;

                FileMeta fileOld;
                FileMeta fileNew;

                while (oldIndex < oldLength && newIndex < newLength)
                {
                    fileOld = filesOld[oldIndex];
                    fileNew = filesNew[newIndex];

                    if (fileOld < fileNew) //the fileOld difference is not changed
                    {
                        oldIndex++;
                    }
                    else if (fileNew < fileOld) //fileNew is a new file difference
                    {
                        filesOld.Add(fileNew);
                        switch (fileNew.FileType)
                        {
                            case ComponentMeta.Type.Deleted: break;   //just add the difference
                            default: ReadAndWrite.MoveFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name);
                                break;

                        }
                        newIndex++;
                    }
                    else  //fileOld need to be updated with fileNew
                    {
                        switch (fileOld.FileType)
                        {
                            case ComponentMeta.Type.Deleted: //previously deleted, now created new file with the same name
                                ReadAndWrite.MoveFile(pathPC, pathUSB);
                                filesOld[oldIndex] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.Modified;
                                break;
                            case ComponentMeta.Type.New: //previously modified or new, now either deleted or further modified
                                switch (fileNew.FileType)
                                {
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(pathUSB + fileOld.Path + fileOld.Name, ReSyncTempUsb + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = null;
                                        break;
                                    case ComponentMeta.Type.Modified: ReadAndWrite.MoveFile(pathUSB + fileOld.Path + fileOld.Name, ReSyncTempUsb + fileOld.Path + fileOld.Name);
                                        ReadAndWrite.MoveFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = fileNew;
                                        if (fileOld.FileType == ComponentMeta.Type.New) fileNew.FileType = ComponentMeta.Type.New;
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Modified: //previously modified or new, now either deleted or further modified
                                switch (fileNew.FileType)
                                {
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(pathUSB + fileOld.Path + fileOld.Name, ReSyncTempUsb + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = fileNew;
                                        break;
                                    case ComponentMeta.Type.Modified: ReadAndWrite.MoveFile(pathUSB + fileOld.Path + fileOld.Name, ReSyncTempUsb + fileOld.Path + fileOld.Name);
                                        ReadAndWrite.MoveFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = fileNew;
                                        if (fileOld.FileType == ComponentMeta.Type.New) fileNew.FileType = ComponentMeta.Type.New;
                                        break;
                                }
                                break;

                        }
                        oldIndex++;
                        newIndex++;
                    }
                }
                while (newIndex < newLength)
                {
                    FileMeta file = filesNew[newIndex++];
                    filesOld.Add(file);
                    if (file.FileType != ComponentMeta.Type.Deleted) //if it's new or modified
                        ReadAndWrite.CopyFile(pathPC + file.Path + file.Name, pathUSB + file.Path + file.Name, bgWorker, onePercentSize);
                }
                FolderMeta.ClearFileList(filesOld);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        
        #region Check size to copy
        private void initializeTotalSize(ComparisonResult comparisonResult, bool resync)
        {
            totalSize = 0;   //reset totalSize

            if (!resync)
            {
                updateFileSize(comparisonResult.USBDifferences.getNewFileList());
                updateFileSize(comparisonResult.USBDifferences.getModifiedFileList());
                updateFolderSize(comparisonResult.USBDifferences.getNewFolderList());
            }
            updateFileSize(comparisonResult.PCDifferences.getNewFileList());
            updateFileSize(comparisonResult.PCDifferences.getModifiedFileList());
            updateFolderSize(comparisonResult.PCDifferences.getNewFolderList());

            onePercentSize = totalSize / 100;
        }
        private void InitializeTotalSizeFirstTimeSynchronization(Differences PCToUSB)
        {

            this.totalSize = 0;
            this.onePercentSize = 0;

            updateFileSize(PCToUSB.getNewFileList());
            updateFileSize(PCToUSB.getModifiedFileList());

            updateFolderSize(PCToUSB.getNewFolderList());

            onePercentSize = totalSize / 100;
        }
        private void updateFolderSize(List<FolderMeta> folders)
        {
            foreach (FolderMeta folder in folders)
            {
                if (folder != null)
                {
                    if (folder.files != null)
                        updateFileSize(folder.files);
                    if (folder.folders != null)
                        updateFolderSize(folder.folders);
                }
            }
        }
        private void updateFileSize(List<FileMeta> files)
        {
            foreach (FileMeta file in files)
            {
                if (file != null)
                    totalSize += file.Size;
            }
        }
        #endregion
    }
}
