﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CleanSync;
using Exceptions;

namespace DirectoryInformation
{
    /// <summary>
    /// SyncLogic handles all synchronization processes. 
    /// </summary>
    public class SyncLogic
    {
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.ComponentModel.DoWorkEventArgs eArg;

        private double onePercentSize;
        private long totalSize;
        private string backupDirectory;
        private string tempUSBForUSBContent;
        private string tempPC;
        private string tempUSBForPCContent;
        private string USBTempReSync;

        private DifferenceToTreeConvertor convertor;

        public SyncLogic()
        {
            totalSize = 0;
            onePercentSize = 0;
            backupDirectory = "";
            tempUSBForUSBContent = "";
            tempPC = "";
            tempUSBForPCContent = "";
            USBTempReSync = "";
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
            public void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker, System.ComponentModel.DoWorkEventArgs e)
            {
                this.bgWorker = worker;
                this.eArg = e;
                try
                {
                    SetTempFolderPaths(pcJob);
                    SetBackupFolder(pcJob);

                    //flush
                    ReadAndWrite.DeleteFolderContent(tempPC);
                    ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                    ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
                    ReadAndWrite.DeleteFolderContent(USBTempReSync);
                

                    pcJob.Synchronizing = true;
                    ReadAndWrite.ExportPCJob(pcJob);

                    Differences PCToUSBDone = new Differences();
                    Differences USBToPCDone = new Differences();

                    if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID) //this is the most recent PC to sync, so do a re-synchronization
                    {
                        initializeTotalSize(comparisonResult);
                        CleanSyncReSync(comparisonResult, pcJob);
                    }
                    else
                    {
                        ClearBackupFolder(pcJob);
                        initializeTotalSize(comparisonResult);
                        NormalCleanSync(comparisonResult, pcJob);  //the other PC was the most recent PC to sync, so do a normal synchronization
                    }
                    this.bgWorker = null;
                    this.eArg = null;
                    totalSize = 0;
                    onePercentSize = 0;
                    backupDirectory = "";
                }
                catch (Exception)
                {
                    this.bgWorker = null;
                    this.eArg = null;
                    totalSize = 0;
                    onePercentSize = 0;
                    backupDirectory = "";
                    throw;
                }
            }

        /// <summary>
        /// Do a synchronization for the first time
        /// </summary>
        /// <param name="PCToUSB">File structure of the folder to be copied over to USB</param>
        /// <param name="pcJob">pcJob of the computer to be synchronized</param>
        /// <param name="worker">worker to update the progress bar of the GUI</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InitializationSynchronize(Differences PCToUSB, PCJob pcJob, System.ComponentModel.BackgroundWorker worker,System.ComponentModel.DoWorkEventArgs e)
        {
            this.bgWorker = worker;
            this.eArg = e;

            InitializeTotalSizeFirstTimeSynchronization(PCToUSB);

            try
            {
                SetTempFolderPaths(pcJob);

                Differences pcToUSBDone = new Differences();
                pcJob.FolderInfo = new FolderMeta(pcJob.PCPath, pcJob.PCPath); //create an empty tree structure, next time sync will be equivalent to 1st time sync.
                pcJob.Synchronizing = true;
                ReadAndWrite.ExportPCJob(pcJob);

                pcJob.GetUsbJob().Synchronizing = true;
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
                pcJob.GetUsbJob().diff = new Differences();
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                ReadAndWrite.CreateDirectory(pcJob.AbsoluteUSBPath);
                FolderMeta root = convertor.ConvertDifferencesToTreeStructure(PCToUSB);

                NormalCleanSyncFolderPcToUsb(root, pcJob.PCPath, tempUSBForPCContent, pcToUSBDone);

                ReadAndWrite.MoveFolderContents(tempUSBForPCContent, pcJob.AbsoluteUSBPath);
                pcJob.GetUsbJob().diff = PCToUSB;
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                pcJob.Synchronizing = false;
                pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
                ReadAndWrite.ExportPCJob(pcJob);
            }
            catch (Exception)
            {
                Differences pcToUSBDone = new Differences();
                pcJob.Synchronizing = false;
                pcJob.FolderInfo = new FolderMeta(pcJob.PCPath, pcJob.PCPath); //create an empty tree structure, next time sync will be equivalent to 1st time sync.
                ReadAndWrite.ExportPCJob(pcJob);

                try
                {
                    pcJob.GetUsbJob().Synchronizing = false;
                    pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
                    pcJob.GetUsbJob().diff = new Differences();
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                }
                catch (Exception)
                {
                    throw new USBUnpluggedException();
                }
                this.bgWorker = null;
                this.eArg = null;
                backupDirectory = "";
                throw;
            }
            this.bgWorker = null;
            this.eArg = null;
            backupDirectory = "";
        }

        #endregion
        
        #region normalSync

        /// <summary>
        /// This method does a normal synchronization, taking care of backups too.
        /// </summary>
        /// <param name="comparisonResult"></param>
        /// <param name="pcJob"></param>
        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta pcToUsb = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
            FolderMeta usbToPc = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
            Differences usbToPcDone = new Differences();
            Differences pcToUSBDone = new Differences();

            FolderMeta olderInfoBackup = pcJob.FolderInfo;
            string prevPCID = pcJob.GetUsbJob().MostRecentPCID;

            try
            {
                // PC to USB temp folder
                pcJob.GetUsbJob().SynchronizingPcToUSB = true;
                pcJob.GetUsbJob().Synchronizing = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                NormalCleanSyncFolderPcToUsb(pcToUsb, pcJob.PCPath, tempUSBForPCContent, pcToUSBDone); //Extract changes from the PC to a temporary USB folder
                
                pcJob.GetUsbJob().SynchronizingPcToUSB = false;
                pcJob.GetUsbJob().SynchronizingUSBToPC = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                
                // USB to PC 
                ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath, tempPC, bgWorker, onePercentSize,eArg);  //copy changes from USB to temporary folder in PC
                NormalCleanSyncFolderUsbToPC(usbToPc, tempPC, pcJob.PCPath, usbToPcDone);  //Synchronize to PC

                pcJob.GetUsbJob().SynchronizingUSBToPC = false;
                pcJob.GetUsbJob().MovingOldDiffToTemp = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (Exception)
            {

                if (pcJob.GetUsbJob().SynchronizingUSBToPC)
                {
                    FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                    RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);  //restore changes on the PC
                    ReadAndWrite.DeleteFolderContent(backupDirectory);
                }
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);
                try
                {
                    ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                    pcJob.GetUsbJob().Synchronizing = false;
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                }
                catch (Exception)
                { throw new USBUnpluggedException(); }
                throw;
            }
            try
            {
                //move the original USB path to a temporary folder
                ReadAndWrite.MoveFolderContents(pcJob.AbsoluteUSBPath, tempUSBForUSBContent); 
                pcJob.GetUsbJob().MovingOldDiffToTemp = false;
                pcJob.GetUsbJob().MovingTempToOldDiff = true;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            }
            catch (Exception)
            {
                FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);
                ReadAndWrite.DeleteFolderContent(backupDirectory);;
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);
                try
                {
                    ReadAndWrite.MoveFolderContentWithReplace(tempUSBForUSBContent, pcJob.AbsoluteUSBPath);
                    ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                    pcJob.GetUsbJob().Synchronizing = false;
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                }
                catch (Exception)
                { throw new USBUnpluggedException(); }
                throw;
            }
            try
            {
                //move the content from the temp folder containing the PC's copied files to the correct path
                ReadAndWrite.MoveFolderContents(tempUSBForPCContent, pcJob.AbsoluteUSBPath);

                pcJob.GetUsbJob().MovingTempToOldDiff = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());

                pcJob.GetUsbJob().diff = comparisonResult.PCDifferences;     //set the new difference on usbJob
               
                //backup 
                pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC
                pcJob.GetUsbJob().Synchronizing = false;
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());

                pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob); 

            }
            catch (Exception)
            {
                FolderMeta doneChanges = convertor.ConvertDifferencesToTreeStructure(usbToPcDone);
                RestoreIncompletePCChanges(doneChanges, pcJob.PCPath);

                ReadAndWrite.DeleteFolderContent(backupDirectory);
                pcJob.Synchronizing = false;
                pcJob.FolderInfo = olderInfoBackup;
                ReadAndWrite.ExportPCJob(pcJob);

                try
                {
                    ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
                    ReadAndWrite.MoveFolderContentWithReplace(tempUSBForUSBContent, pcJob.AbsoluteUSBPath);

                    pcJob.GetUsbJob().diff = comparisonResult.PCDifferences;
                    pcJob.GetUsbJob().Synchronizing = false;
                    pcJob.GetUsbJob().MostRecentPCID = prevPCID;
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                    ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
                    ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                }
                catch (Exception)
                { throw new USBUnpluggedException(); }
                throw;
            }
            try
            {
                ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
                ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                ReadAndWrite.DeleteFolderContent(tempPC);
            }
            catch (Exception)
            {
                return; 
            }
        }

        /// <summary>
        /// Extracts required files and folders on the PC over to the USB
        /// </summary>
        /// <param name="pcToUsb"></param>
        /// <param name="originDirectoryRoot"></param>
        /// <param name="destinationDirectoryRoot"></param>
        /// <param name="pcToUsbDone"></param>
        private void NormalCleanSyncFolderPcToUsb(FolderMeta pcToUsb, string originDirectoryRoot, string destinationDirectoryRoot, Differences pcToUsbDone)
        {
            try
            {
                foreach (FileMeta file in pcToUsb.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New:
                            ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize,eArg);
                            pcToUsbDone.AddNewFileDifference(file);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize,eArg);
                            pcToUsbDone.AddModifiedFileDifference(file);
                            break;
                    }
                }
                foreach (FolderMeta folder in pcToUsb.folders)
                {
                    switch (folder.FolderType)
                    {
                        case ComponentMeta.Type.New: ReadAndWrite.CopyFolder(folder, originDirectoryRoot, destinationDirectoryRoot , bgWorker, onePercentSize,eArg);
                            pcToUsbDone.AddNewFolderDifference(folder);
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(destinationDirectoryRoot + folder.Path + folder.Name);
                            NormalCleanSyncFolderPcToUsb(folder, originDirectoryRoot, destinationDirectoryRoot, pcToUsbDone);
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Synchronizes the PC with changes from the USB
        /// </summary>
        /// <param name="usbToPC"></param>
        /// <param name="originDirectoryRoot"></param>
        /// <param name="destinationDirectoryRoot"></param>
        /// <param name="usbToPCDone"></param>
        private void NormalCleanSyncFolderUsbToPC(FolderMeta usbToPC, string originDirectoryRoot, string destinationDirectoryRoot, Differences usbToPCDone)
        {
            try
            {
                foreach (FileMeta file in usbToPC.files)
                {
                    switch (file.FileType)
                    {
                        case ComponentMeta.Type.New:
                            ReadAndWrite.createParentFolders(file, destinationDirectoryRoot);  //create the folders to this file if path does not exist yet
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
                            ReadAndWrite.createParentFolders(folder, destinationDirectoryRoot);  //create the folders to this file if path does not exist yet
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
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region ReSync
        /// <summary>
        /// Does a resynchronization. The old changes made by this PC will be merged and updated with the new changes.
        /// </summary>
        /// <param name="comparisonResult"></param>
        /// <param name="pcJob"></param>
        private void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {

            FolderMeta oldDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
            FolderMeta newDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
           
            if(newDifferencesRoot.files.Count != 0 || newDifferencesRoot.folders.Count != 0) ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath, USBTempReSync, bgWorker, onePercentSize,eArg); // Backup

            //for restoration, work on a new copy of oldDifferencesRoot, so that can just delete to restore
            FolderMeta oldDifferencesOriginal = new FolderMeta(oldDifferencesRoot);
            FolderMeta prevFolderInfo = pcJob.FolderInfo;

            try
            {
                pcJob.GetUsbJob().ReSynchronizing = true;
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());

                //Copy files to USB
                Differences pcToUSBDone = new Differences();  // dummy difference, needed for NormalCleanSyncFolderPcToUsb.
                NormalCleanSyncFolderPcToUsb(newDifferencesRoot, pcJob.PCPath, tempUSBForPCContent, pcToUSBDone);

                ReSynchronizeFolders(oldDifferencesOriginal, newDifferencesRoot, tempUSBForPCContent, pcJob.AbsoluteUSBPath, pcToUSBDone);

            }
            catch (Exception)
            {

                ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
                ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
                ReadAndWrite.MoveFolderContents(USBTempReSync, pcJob.AbsoluteUSBPath);
                try
                {
                    pcJob.GetUsbJob().ReSynchronizing = false;
                    if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                        ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                    else
                        ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                }
                catch (Exception)
                {
                    throw new USBUnpluggedException();
                }
                throw;
            }
            try
            {
                oldDifferencesOriginal.sortComponents();
                pcJob.GetUsbJob().diff = convertor.ConvertTreeStructureToDifferences(oldDifferencesOriginal); //set the new tree.
                pcJob.GetUsbJob().ReSynchronizing = false;
                if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                    ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                else
                    ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);

            }
            catch (Exception)
            {
                pcJob.FolderInfo = prevFolderInfo;
                pcJob.Synchronizing = false;
                ReadAndWrite.ExportPCJob(pcJob);

                ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
                ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
                ReadAndWrite.MoveFolderContents(USBTempReSync, pcJob.AbsoluteUSBPath); 

                try
                {
                    pcJob.GetUsbJob().diff = convertor.ConvertTreeStructureToDifferences(oldDifferencesRoot);
                    pcJob.GetUsbJob().ReSynchronizing = false;
                    if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                        ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
                    else
                        ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
                }
                catch (Exception)
                { throw new USBUnpluggedException(); }
                throw;
            }
            ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
            ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
            ReadAndWrite.DeleteFolderContent(USBTempReSync);
        }

        /// <summary>
        /// Resynchronize 2 folders that are modified.
        /// </summary>
        /// <param name="oldDifferencesRoot"></param>
        /// <param name="newDifferencesRoot"></param>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="pcToUSBDone"></param>
        private void ReSynchronizeFolders(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string sourceDirectory, string destinationDirectory, Differences pcToUSBDone)
        {
            try
            {
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
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(destinationDirectory + folderOld.Path + folderOld.Name, tempUSBForUSBContent + folderOld.Path + folderOld.Name);
                                        foldersOld[oldIndex] = null;
                                        break;
                                    //folderOld: new, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(tempUSBForUSBContent + folderOld.Path + folderOld.Name);
                                        ReSynchronizeToNewFolder(folderOld, folderNew, sourceDirectory, destinationDirectory);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Modified:
                                switch (folderNew.FolderType)
                                {
                                    //folderOld: modified, folderNew: deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFolder(destinationDirectory + folderOld.Path + folderOld.Name, tempUSBForUSBContent + folderOld.Path + folderOld.Name);
                                        CompareNewDeletedWithOldModifiedFolders(folderNew, folderOld); //go through deleted folder and get rid of previously new files/folders, and add previously deleted files/folders
                                       // folderNew.sortComponents();
                                        foldersOld[oldIndex] = folderNew;
                                        break;

                                    //folderOld: modified, folderNew: modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(tempUSBForUSBContent + folderOld.Path + folderOld.Name);
                                        ReSynchronizeFolders(folderOld, folderNew, sourceDirectory, destinationDirectory, pcToUSBDone);
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Deleted: //originally was deleted, now re-created a folder with the same name
                                folderOld.FolderType = ComponentMeta.Type.Modified; //no longer deleted, now it's just modified.
                                ReadAndWrite.CreateDirectory(destinationDirectory + folderNew.Path + folderNew.Name);
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
                //FolderMeta.ClearFolderList(foldersOld);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks the inner contents when a previously modified folder is now deleted. Finds new files and folders and removes them from the list as they do not exist in the other computer.
        /// </summary>
        /// <param name="folderNew"></param>
        /// <param name="folderOld"></param>
        private void CompareNewDeletedWithOldModifiedFolders(FolderMeta folderNew, FolderMeta folderOld)
        {
            try
            {
                int oldInt = 0;
                int newInt = 0;
                int oldCount = folderOld.folders.Count;
                int newCount = folderNew.folders.Count;

                while (oldInt < oldCount && newInt < newCount)
                {
                    FolderMeta subFolderNew = folderNew.folders[newInt];
                    FolderMeta subFolderOld = folderOld.folders[oldInt];

                    if (subFolderNew < subFolderOld) //not found in old, this folder is just deleted
                    {
                        newInt++;
                    }
                    else if (subFolderOld < subFolderNew)//originally already to be deleted, now add to the new tree.
                    {
                        folderNew.folders.Add(subFolderOld);
                        oldInt++;
                    }
                    else
                    {
                        //subFolderOld: new, SubFolderNew: deleted
                        if (subFolderOld.FolderType == ComponentMeta.Type.New)
                        {
                            folderNew.folders[newInt] = null;
                        }
                        //subFolderOld: modified, subFolderNew: deleted
                        else if (subFolderOld.FolderType == ComponentMeta.Type.Modified)
                        {
                            CompareNewDeletedWithOldModifiedFolders(subFolderNew, subFolderOld);
                        }
                        oldInt++;
                        newInt++;
                    }
                }
                while(oldInt < oldCount)//originally deleted, now delete also
                {
                    folderNew.folders.Add(folderOld.folders[oldInt++]);
                }

                CompareNewDeletedWithOldModifiedFiles(folderNew, folderOld);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks the inner contents when a previously modified folder is now deleted. 
        /// Finds new files and removes them from the list as they do not exist in the other computer.
        /// </summary>
        /// <param name="folderNew"></param>
        /// <param name="folderOld"></param>
        private void CompareNewDeletedWithOldModifiedFiles(FolderMeta folderNew, FolderMeta folderOld)
        {
            int oldInt = 0;
            int newInt = 0;
            int oldCount = folderOld.files.Count;
            int newCount = folderNew.files.Count;

            while (oldInt < oldCount && newInt < newCount)
            {
                FileMeta subFileOld = folderOld.files[oldInt];
                FileMeta subFileNew = folderNew.files[newInt];

                if (subFileNew < subFileOld)  //originally not found in old, this file is just deleted
                {
                    newInt++;
                }
                else if (subFileOld < subFileNew) //originally deleted, now added to the new tree
                {
                    folderNew.files.Add(subFileOld);
                    oldInt++;
                }
                else
                {
                    //Originally new, now deleted
                    if (subFileOld.FileType == ComponentMeta.Type.New) folderNew.files[newInt] = null;

                    //if originally modified, now deleted, no changes need to be made
                    newInt++;
                    oldInt++;
                }
            }
            while(oldInt < oldCount)
            {
                folderNew.files.Add(folderOld.files[oldInt++]);
            }
        }
        
        /// <summary>
        /// Resyncs a modified folder against a previously new folder
        /// </summary>
        /// <param name="oldDifferencesRoot"></param>
        /// <param name="newDifferencesRoot"></param>
        /// <param name="sourceRoot"></param>
        /// <param name="destinationRoot"></param>
        private void ReSynchronizeToNewFolder(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string sourceRoot, string destinationRoot)
        {
            try
            {
                ReSynchronizeToNewFolderFiles(oldDifferencesRoot, newDifferencesRoot, sourceRoot, destinationRoot);

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

                    if (folderOld < folderNew) //no change
                    {
                        indexOld++;
                    }
                    else if (folderNew < folderOld) //new folder
                    {
                        foldersOld.Add(folderNew);
                        ReadAndWrite.MoveFolder(sourceRoot + folderNew.Path + folderNew.Name, destinationRoot + folderNew.Path + folderNew.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (folderNew.FolderType)
                        {
                            // folderOld: New, folderNew: deleted
                            case ComponentMeta.Type.Deleted: foldersOld[indexOld] = null;
                                ReadAndWrite.MoveFolder(destinationRoot + folderNew.Path + folderNew.Name, tempUSBForUSBContent + folderNew.Path + folderOld.Name);
                                break;
                            // folderOld: New, folderNew: modified
                            case ComponentMeta.Type.Modified: ReadAndWrite.CreateDirectory(tempUSBForUSBContent + folderNew.Path + folderNew.Name);
                                ReSynchronizeToNewFolder(folderOld, folderNew, sourceRoot, destinationRoot);
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
                    ReadAndWrite.MoveFolder(sourceRoot + folderNew.Path + folderNew.Name, destinationRoot + folderNew.Path + folderNew.Name);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Resyncs the files in a modified folder against a previously new folder
        /// </summary>
        /// <param name="sourceRoot"></param>
        /// <param name="destinationRoot"></param>
        /// <param name="pathPC"></param>
        /// <param name="pathUSB"></param>
        private void ReSynchronizeToNewFolderFiles(FolderMeta folderOld, FolderMeta folderNew, string sourceRoot, string destinationRoot)
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
                        ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileNew.Path + fileNew.Name);
                        indexNew++;
                    }
                    else
                    {
                        switch (fileNew.FileType)
                        {
                            //fileOld: new, fileNew: modified
                            case ComponentMeta.Type.Modified:
                                ReadAndWrite.MoveFile(destinationRoot + fileNew.Path + fileNew.Name, tempUSBForUSBContent + fileNew.Path + fileNew.Name);
                                ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileNew.Path + fileNew.Name);
                                filesOld[indexOld] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.New;
                                break;
                            //fileOld: new, fileNew: deleted
                            case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(destinationRoot + fileOld.Path + fileOld.Name, tempUSBForUSBContent + fileNew.Path + fileNew.Name);
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
                    filesNew[indexNew].FileType = ComponentMeta.Type.New;
                    ReadAndWrite.MoveFile(sourceRoot + filesNew[indexNew].Path + filesNew[indexNew].Name, destinationRoot + filesNew[indexNew].Path + filesNew[indexNew].Name);
                    indexNew++;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Resynchronizes the files in a folder
        /// </summary>
        /// <param name="oldDifferencesRoot"></param>
        /// <param name="newDifferencesRoot"></param>
        /// <param name="sourceRoot"></param>
        /// <param name="destinationRoot"></param>
        private void ReSynchronizeFiles(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string sourceRoot, string destinationRoot)
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
                            default: ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileNew.Path + fileNew.Name); //new or modified
                                break;

                        }
                        newIndex++;
                    }
                    else  //fileOld need to be updated with fileNew
                    {
                        switch (fileOld.FileType)
                        {
                            //fileOld: deleted, fileNew: new
                            case ComponentMeta.Type.Deleted: //created a file of the same name. 
                                ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileNew.Path + fileNew.Name);
                                filesOld[oldIndex] = fileNew;
                                fileNew.FileType = ComponentMeta.Type.Modified;
                                break;
                            case ComponentMeta.Type.New: 
                                switch (fileNew.FileType)
                                {
                                    //fileOld:New, fileNew: Deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(destinationRoot + fileOld.Path + fileOld.Name, tempUSBForUSBContent + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = null;
                                        break;
                                    //fileOld: New, fileNew: Modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.MoveFile(destinationRoot + fileOld.Path + fileOld.Name, tempUSBForUSBContent + fileOld.Path + fileOld.Name);
                                        ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = fileNew;
                                        if (fileOld.FileType == ComponentMeta.Type.New) fileNew.FileType = ComponentMeta.Type.New;
                                        break;
                                }
                                break;
                            case ComponentMeta.Type.Modified: 
                                switch (fileNew.FileType)
                                {
                                    //fileOld: Modified, fileNew: Deleted
                                    case ComponentMeta.Type.Deleted: ReadAndWrite.MoveFile(destinationRoot + fileOld.Path + fileOld.Name, tempUSBForUSBContent + fileOld.Path + fileOld.Name);
                                        filesOld[oldIndex] = fileNew;
                                        break;

                                    //fileOld: Modified, fileNew: Modified
                                    case ComponentMeta.Type.Modified: ReadAndWrite.MoveFile(destinationRoot + fileOld.Path + fileOld.Name, tempUSBForUSBContent + fileOld.Path + fileOld.Name);
                                        ReadAndWrite.MoveFile(sourceRoot + fileNew.Path + fileNew.Name, destinationRoot + fileOld.Path + fileOld.Name);
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
                        ReadAndWrite.CopyFile(sourceRoot + file.Path + file.Name, destinationRoot + file.Path + file.Name, bgWorker, onePercentSize,eArg);
                }
                FolderMeta.ClearFileList(filesOld);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        #endregion*/

        #region restoration
        /// <summary>
        /// Restore the changes made to the folder in the PC during a normal synchronization
        /// </summary>
        /// <param name="changes">Tree representation of the changes made </param>
        /// <param name="pcPath">Path to restore</param>
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
        #endregion

        #region setting backup and temp folders' directories

        private void SetBackupFolder(PCJob pcJob)
        {
            backupDirectory = ReadAndWrite.GetPCBackUpFolder(pcJob);
        }
        private void ClearBackupFolder(PCJob pcJob)
        {
            ReadAndWrite.DeleteFolderContent(backupDirectory);
        }
        private void SetTempFolderPaths(PCJob pcJob)
        {
            tempPC = ReadAndWrite.GetPCTempFolder(pcJob);
            tempUSBForPCContent = ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob());
            tempUSBForUSBContent = ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob());
            USBTempReSync = ReadAndWrite.GetUSBResyncBackUpDirectory(pcJob.GetUsbJob());
        }

        #endregion
        
        #region Check size to copy
        /// <summary>
        /// Calculates the total size that will be copied to and fro during a synchronization process
        /// </summary>
        /// <param name="comparisonResult"></param>
        private void initializeTotalSize(ComparisonResult comparisonResult)
        {
            totalSize = 0;   //reset totalSize

           
            updateFileSize(comparisonResult.USBDifferences.getNewFileList());
            updateFileSize(comparisonResult.USBDifferences.getModifiedFileList());
            updateFolderSize(comparisonResult.USBDifferences.getNewFolderList());
       
            updateFileSize(comparisonResult.PCDifferences.getNewFileList());
            updateFileSize(comparisonResult.PCDifferences.getModifiedFileList());
            updateFolderSize(comparisonResult.PCDifferences.getNewFolderList());

            onePercentSize = totalSize / 100;
        }

        /// <summary>
        /// Calculates the total size that will be copied during synchronization for the the first time
        /// </summary>
        /// <param name="PCToUSB"></param>
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
