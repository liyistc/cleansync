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
        private string tempPC;
        private string tempUSB;

        private DifferenceToTreeConvertor convertor;

        public SyncLogic()
        {
            totalSize = 0;
            onePercentSize = 0;
            backupDirectory = "";
            tempPC = "";
            tempUSB = "";
            convertor = new DifferenceToTreeConvertor();
        
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
            try
            {
                CreateBackupFolder(pcJob);
                SetTempFolderPaths(pcJob);

                if (pcJob.GetUsbJob().Synchronizing)
                {
                    RestoreInterruptedUSB(pcJob.GetUsbJob());
                }
                if (pcJob.GetUsbJob().ReSynchronizing)
                {
                    RestoreReSyncUSB(pcJob);
                }
                else
                {
                    ReadAndWrite.DeleteFolderContent(tempPC);
                    ReadAndWrite.DeleteFolderContent(tempUSB);
                }

                Differences PCToUSBDone = new Differences();
                Differences USBToPCDone = new Differences();
                this.bgWorker = worker;

                if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID) //this is the most recent PC to sync, so do a re-synchronization
                {
                    initializeTotalSize(comparisonResult, true);
                    CleanSyncReSync(comparisonResult, pcJob);
                }
                else
                {
                    initializeTotalSize(comparisonResult, false);
                    NormalCleanSync(comparisonResult, pcJob);  //the other PC was the most recent PC to sync, so do a normal synchronization
                }
                this.bgWorker = null;
                totalSize = 0;
                onePercentSize = 0;
                backupDirectory = "";
            }
            catch (SyncInterruptedException e)
            {
                throw e;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void SetTempFolderPaths(PCJob pcJob)
        {          
            tempPC = ReadAndWrite.GetPCRootPath() + @"\" + "temp folders" + @"\" + pcJob.JobName;
            tempUSB = ReadAndWrite.GetStoredFolderOnUSB(pcJob.GetUsbJob().AbsoluteUSBPath) + @"\" + "temp folders" + @"\" + pcJob.JobName;
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
            
            this.bgWorker = worker;
            InitializeTotalSizeFirstTimeSynchronization(PCToUSB);
            
            try
            {
                CreateTempFolders(pcJob);
                CreateBackupFolder(pcJob);

                pcJob.FolderInfo = new FolderMeta(pcJob.PCPath, pcJob.PCPath); //create an empty tree structure, next time sync will be equivalent to 1st time sync.
                ReadAndWrite.ExportPCJob(pcJob);
                pcJob.GetUsbJob().Synchronizing = true;
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
                pcJob.GetUsbJob().diff = new Differences();
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                
                ReadAndWrite.CreateDirectory(pcJob.AbsoluteUSBPath);
                FolderMeta root = convertor.ConvertDifferencesToTreeStructure(PCToUSB);
                NormalCleanSyncFolderPcToUsb(root, pcJob.PCPath, tempUSB);
                ReadAndWrite.MoveFileOrFolderContents(tempUSB, pcJob.AbsoluteUSBPath);

                pcJob.GetUsbJob().diff = PCToUSB;
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

            }
            catch (System.IO.FileNotFoundException)
            {
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
                        ReadAndWrite.MoveFileOrFolder(backupDirectory + file.Path + file.Name, pcPath + file.Path + file.Name);
                        break;
                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFileOrFolder(backupDirectory + file.Path + file.Name, pcPath + file.Path + file.Name);
                        break;
                }
            }
            foreach (FolderMeta folder in changes.folders)
            {
                switch (folder.FolderType)
                {
                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFileOrFolder(backupDirectory + folder.Path + folder.Name, pcPath + folder.Path + folder.Name);
                        break;
                    case ComponentMeta.Type.New: ReadAndWrite.DeleteFolder(pcPath + folder.Path + folder.Name);
                        break;
                    case ComponentMeta.Type.Modified: RestoreIncompletePCChanges(folder, pcPath);
                        break;
                }
            }
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
                ReadAndWrite.DeleteFolderContent(tempPC);
                usbJob.SynchronizingUSBToPC = false;
            }
            else //sync PCtoUSB done, USBtoPC done, have not move folder over
            {
                ReadAndWrite.DeleteFolderContent(usbJob.AbsoluteUSBPath);
                ReadAndWrite.MoveFileOrFolderContents(tempUSB, usbJob.AbsoluteUSBPath);
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

        #endregion
        #region creatingBackupFolder
        
        private void CreateBackupFolder(PCJob pcJob)
        {
            backupDirectory = ReadAndWrite.GetMyDocumentsDirectory() + @"\CleanSync";
            backupDirectory = backupDirectory + @"\backup";
            ReadAndWrite.CreateDirectory(backupDirectory);
            backupDirectory = backupDirectory + @"\" + pcJob.JobName;
            ReadAndWrite.CreateDirectory(backupDirectory);
            string currentDate = DateTime.Now.ToString();
            currentDate = currentDate.Replace(":", "-");
            currentDate = currentDate.Replace("/", "-");
            backupDirectory = backupDirectory + @"\" + currentDate;
            ReadAndWrite.CreateDirectory(backupDirectory);
        }

        private void CreateTempFolders(PCJob pcJob)
        {
            tempPC = ReadAndWrite.GetMyDocumentsDirectory() +@"\CleanSync";
            ReadAndWrite.CreateDirectory(tempPC);

            tempUSB = ReadAndWrite.GetStoredFolderOnUSB(pcJob.GetUsbJob().AbsoluteUSBPath);

            tempPC = tempPC + @"\" + "temp";
            tempUSB = tempUSB + @"\" + "temp";
            ReadAndWrite.CreateDirectory(tempPC);
            ReadAndWrite.CreateDirectory(tempUSB);
            tempPC = tempPC + @"\" + pcJob.JobName;
            tempUSB = tempUSB + @"\" + pcJob.JobName;
            ReadAndWrite.CreateDirectory(tempPC);
            ReadAndWrite.CreateDirectory(tempUSB);
        }
#endregion
        #region normalSync

        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta pcToUsb = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
            FolderMeta usbToPc = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);

            try
            {
                //Sync PC -> USB temp folder
                pcJob.GetUsbJob().SynchronizingPcToUSB = true;
                pcJob.GetUsbJob().Synchronizing = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                NormalCleanSyncFolderPcToUsb(pcToUsb, tempUSB, pcJob.AbsoluteUSBPath);
                
                //Sync USB -> PC 
                pcJob.GetUsbJob().SynchronizingPcToUSB = false;
                pcJob.GetUsbJob().SynchronizingUSBToPC = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());

                ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath, tempPC, bgWorker, onePercentSize);
                NormalCleanSyncFolderUsbToPC(usbToPc, tempPC, pcJob.PCPath);
                pcJob.GetUsbJob().SynchronizingUSBToPC = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                
                //move USB temp folder to the correct folder
                ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
                ReadAndWrite.MoveFileOrFolderContents(tempUSB, pcJob.AbsoluteUSBPath);
              
                pcJob.GetUsbJob().diff = comparisonResult.PCDifferences;     //set the new difference on usbJob
                           
                //backup 
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (SyncInterruptedException)
            {
              /*  FolderMeta doneChannges = convertor.ConvertDifferencesToTreeStructure(PCToUSBDone);
                //RestoreIncompletePCChanges(doneChannges, pcJob.PCPath);
                ReadAndWrite.DeleteFolder(backupDirectory);  */
                throw;
            }
            catch (System.IO.FileNotFoundException)
            {
           /*     FolderMeta doneChannges = convertor.ConvertDifferencesToTreeStructure(PCToUSBDone);
                //RestoreIncompletePCChanges(doneChannges, pcJob.PCPath);
                ReadAndWrite.DeleteFolder(backupDirectory);   */
                throw;
            }
            catch (Exception)
            {
        /*        FolderMeta doneChannges = convertor.ConvertDifferencesToTreeStructure(PCToUSBDone);
                //RestoreIncompletePCChanges(doneChannges, pcJob.PCPath);
                ReadAndWrite.DeleteFolder(backupDirectory);*/
                throw new SyncInterruptedException();
            }

            ReadAndWrite.DeleteFolderContent(tempUSB);
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        private void NormalCleanSyncFolderPcToUsb(FolderMeta pcToUsb, string originDirectoryRoot, string destinationDirectoryRoot)
        {
            try
            {
                foreach (FileMeta file in pcToUsb.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                            break;
                    }
                }
                foreach (FolderMeta folder in pcToUsb.folders)
                {
                    switch (folder.FolderType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.CopyFolder(originDirectoryRoot + folder.Path + folder.Name, destinationDirectoryRoot + folder.Path + folder.Name, bgWorker, onePercentSize);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(destinationDirectoryRoot + folder.Path + folder.Name);
                            NormalCleanSyncFolderPcToUsb(folder, originDirectoryRoot, destinationDirectoryRoot);
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(originDirectoryRoot) == 0) throw e;  //PC file name changed
                if (path.IndexOf(destinationDirectoryRoot) == 0) throw new SyncInterruptedException(); //USB folder changed or USB removed
            }
            catch (Exception e)
            {
                throw new SyncInterruptedException();
            }
        }

        private void NormalCleanSyncFolderUsbToPC(FolderMeta usbToPC, string originDirectoryRoot, string destinationDirectoryRoot)
        {
            try
            {
                foreach (FileMeta file in usbToPC.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.MoveFileOrFolder(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name);
                           
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.MoveFileOrFolder(destinationDirectoryRoot + file.Path + file.Name, backupDirectory + file.Path + file.Name);
                            ReadAndWrite.MoveFileOrFolder(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name);
                           
                            break;
                        case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFileOrFolder(destinationDirectoryRoot + file.Path + file.Name, backupDirectory + file.Path + file.Name);
                          
                            break;
                    }
                    
                }

                foreach (FolderMeta folder in usbToPC.folders)
                {
                    switch (folder.FolderType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.MoveFileOrFolder(originDirectoryRoot + folder.Path + folder.Name, destinationDirectoryRoot + folder.Path + folder.Name);
                          
                            break;
                        case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFileOrFolder(destinationDirectoryRoot + folder.Path + folder.Name, backupDirectory + folder.Path + folder.Name);
                          
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.MoveFileOrFolder(destinationDirectoryRoot + folder.Path + folder.Name, backupDirectory + folder.Path + folder.Name);
                            NormalCleanSyncFolderUsbToPC(usbToPC, originDirectoryRoot, destinationDirectoryRoot);
                            break;
                    }
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(originDirectoryRoot) == 0) throw new SyncInterruptedException(); //USB folder changed or USB removed
                if (path.IndexOf(destinationDirectoryRoot) == 0) throw e;  //PC file name changed
            }
            catch (Exception)
            {
                throw new SyncInterruptedException();
            }
        }
        #endregion

        #region ReSync
        private void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            try
            {

                DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
                FolderMeta oldDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
                FolderMeta newDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);

                pcJob.GetUsbJob().ReSynchronizing = true;
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                //Copy files to USB
                NormalCleanSyncFolderPcToUsb(newDifferencesRoot, pcJob.PCPath, tempUSB);

                //Files copied to US
                pcJob.GetUsbJob().RecoveryPossible = false;
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                
                ReSynchronizeFolders(oldDifferencesRoot, newDifferencesRoot, tempUSB, pcJob.AbsoluteUSBPath);


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
            catch (Exception)
            {
                throw new SyncInterruptedException();
            } 
        }

        private void ReSynchronizeFolders(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
        {
            try
            {
                ReSynchronizeFiles(oldDifferencesRoot, newDifferencesRoot, pathPC, pathUSB);
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
                                ReadAndWrite.MoveFileOrFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                                foldersOld.Add(folderNew);
                                break;
                            case ComponentMeta.Type.Deleted: foldersOld.Add(folderNew);
                                break;
                            case ComponentMeta.Type.Modified: foldersOld.Add(folderNew);
                                NormalCleanSyncFolderPcToUsb(folderNew, pathPC, pathUSB);
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
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFolder(pathUSB + folderOld.Path + folderOld.Name);
                                        foldersOld[oldIndex] = null;
                                        break;
                                    //folderOld: new, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReSynchronizeToNewFolder(folderOld, folderNew, pathPC, pathUSB);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Modified:
                                switch (folderNew.FolderType)
                                {
                                    //folderOld: modified, folderNew: deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFolder(pathUSB + folderOld.Path + folderOld.Name);
                                        foldersOld[oldIndex] = null;
                                        break;
                                    //folderOld: modified, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReSynchronizeFolders(folderOld, folderNew, pathPC, pathUSB);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Deleted: //originally was deleted, now re-created a folder with the same name
                                folderOld.FolderType = ComponentMeta.Type.Modified;
                                ReSynchronizeFolders(folderOld, folderNew, pathPC, pathUSB);
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
                        case ComponentMeta.Type.Modified: NormalCleanSyncFolderPcToUsb(folderNew, pathPC, pathUSB);
                            foldersOld.Add(folderNew);
                            break;
                        case ComponentMeta.Type.Deleted: foldersOld.Add(folderNew);
                            break;
                        default: foldersOld.Add(folderNew);
                            ReadAndWrite.MoveFileOrFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                            break;
                    }
                    newIndex++;
                }
                ClearFolderList(foldersOld);
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(pathPC) == 0) throw e;
                if (path.IndexOf(pathUSB) == 0) throw new SyncInterruptedException();
            }
            catch (SyncInterruptedException)
            {
                throw; 
            }
            catch (Exception)
            {
                throw new SyncInterruptedException();
            }
        }

        private void ReSynchronizeToNewFolder(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
        {
            try
            {
                ReSynchronizeToNewFolderFiles(oldDifferencesRoot, newDifferencesRoot, pathPC, pathUSB);

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
                        foldersOld.Add(folderNew);
                        folderNew.FolderType = ComponentMeta.Type.NotTouched;
                        ReadAndWrite.MoveFileOrFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (folderNew.FolderType)
                        {
                            case ComponentMeta.Type.Deleted: foldersOld[indexOld] = null;
                                ReadAndWrite.DeleteFolder(pathUSB + folderNew.Path + folderNew.Name);
                                break;
                            case ComponentMeta.Type.Modified: ReSynchronizeToNewFolder(folderOld, folderNew, pathPC, pathUSB);
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
                    folderNew.FolderType = ComponentMeta.Type.NotTouched;
                    ReadAndWrite.MoveFileOrFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
                }
                ClearFolderList(foldersOld);
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(pathPC) == 0) throw e;
                if (path.IndexOf(pathUSB) == 0) throw new SyncInterruptedException();
            }
            catch (Exception)
            {
                throw new SyncInterruptedException();
            }
        }

        private void ReSynchronizeToNewFolderFiles(FolderMeta folderOld, FolderMeta folderNew, string pathPC, string pathUSB)
        {
            try
            {
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
                        filesOld.Add(fileNew);
                        fileNew.FileType = ComponentMeta.Type.NotTouched;
                        ReadAndWrite.MoveFileOrFolder(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (fileNew.FileType)
                        {
                            case ComponentMeta.Type.Modified:
                                ReadAndWrite.DeleteFile(pathUSB + fileNew.Path + fileNew.Name);
                                ReadAndWrite.MoveFileOrFolder(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name);
                                filesOld[indexOld] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.NotTouched;
                                break;
                            case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFile(pathUSB + fileOld.Path + fileOld.Name);
                                filesOld[indexOld] = null;
                                break;
                        }
                        indexOld++;
                        indexNew++;
                    }
                }
                while (indexNew < filesNewSize) // rest of the new files
                {
                    filesOld.Add(filesNew[indexNew]);
                    filesNew[indexNew].FileType = ComponentMeta.Type.NotTouched;
                    ReadAndWrite.DeleteFile(pathUSB + filesNew[indexNew].Path + filesNew[indexNew].Name);
                    ReadAndWrite.MoveFileOrFolder(pathPC + filesNew[indexNew].Path + filesNew[indexNew].Name, pathUSB + filesNew[indexNew].Path + filesNew[indexNew].Name);
                    indexNew++;
                }
                ClearFileList(filesOld);
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(pathPC) == 0) throw e;
                if (path.IndexOf(pathUSB) == 0) throw new SyncInterruptedException();
            }
            catch (Exception)
            {
                throw new SyncInterruptedException();
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
                        filesOld.Add(fileOld);
                        switch (fileOld.FileType)
                        {
                            case ComponentMeta.Type.Deleted: break;   //just add the difference
                            default: ReadAndWrite.MoveFileOrFolder(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name);
                                break;
      
                        }
                        newIndex++;
                    }
                    else  //fileOld need to be updated with fileNew
                    {
                        switch(fileOld.FileType)
                        {
                            case ComponentMeta.Type.Deleted: //previously deleted, now created new file with the same name
                                ReadAndWrite.MoveFileOrFolder(pathPC, pathUSB);
                                filesOld[oldIndex] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.Modified;
                                break;
                            default: //previously modified or new, now either deleted or further modified
                                switch (fileNew.FileType)
                                {
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFile(pathUSB + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = null;
                                        break;
                                    case ComponentMeta.Type.Modified:
                                        ReadAndWrite.DeleteFile(pathUSB + fileOld.Path + fileOld.Name);
                                        ReadAndWrite.MoveFileOrFolder(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name);
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
                ClearFileList(filesOld);
            }
            catch (System.IO.FileNotFoundException e)
            {
                string path = e.FileName;
                if (path.IndexOf(pathPC) == 0) throw e;
                if (path.IndexOf(pathUSB) == 0) throw new SyncInterruptedException();
            }
            catch (Exception)
            {
                throw new SyncInterruptedException();
            }
        }

        private void ClearFolderList(List<FolderMeta> folderList)
        {
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta folder = folderList[i];
                if (folder == null)
                {
                    folderList.RemoveAt(i);
                    i--;
                }
            }
        }
        private void ClearFileList(List<FileMeta> fileList)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList[i];
                if (file == null)
                {
                    fileList.RemoveAt(i);
                    i--;
                }
            }
        }
#endregion

        #region Check size to copy

        private void initializeTotalSize(ComparisonResult comparisonResult,bool resync)
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
