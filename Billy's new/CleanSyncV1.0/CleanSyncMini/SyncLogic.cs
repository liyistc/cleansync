using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CleanSync;

namespace DirectoryInformation
{
    public class SyncLogic
    {
        private System.ComponentModel.BackgroundWorker bgWorker;

        private double onePercentSize;
        private long totalSize;

        public SyncLogic()
        {
            totalSize = 0;
            onePercentSize = 0;
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

            this.bgWorker = worker;

            if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID) //this is the most recent PC to sync, so do a re-synchronization
            {
                initializeTotalSize(comparisonResult, true);
                CleanSyncReSync(comparisonResult, pcJob);
            }
            else
            {
                initializeTotalSize(comparisonResult,false);
                NormalCleanSync(comparisonResult, pcJob);  //the other PC was the most recent PC to sync, so do a normal synchronization
            }
            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;  //update most recent PC 
            this.bgWorker = null;
        }

        /// <summary>
        /// Do a fist time synchronization
        /// </summary>
        /// <param name="PCToUSB">The difference to use to copy</param>
        /// <param name="pcJob">pcJob of the computer to be synchronized</param>
        /// <param name="worker">worker to update the progress bar of the GUI</param>
        public void SyncPCToUSB(Differences PCToUSB, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            this.bgWorker = worker;
            InitializeTotalSizeFirstTimeSynchronization(PCToUSB);
            ReadAndWrite.CreateDirectory(pcJob.AbsoluteUSBPath);
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta root = convertor.ConvertDifferencesToTreeStructure(PCToUSB);
            NormalCleanSyncFolderPcToUsb(root, pcJob.PCPath, pcJob.AbsoluteUSBPath);
            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
            this.bgWorker = null;
        }

        #endregion

        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta pcToUsb = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);
            FolderMeta usbToPc = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);

            NormalCleanSyncFolderPcToUsb(pcToUsb, pcJob.PCPath, pcJob.AbsoluteUSBPath);
            ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
            NormalCleanSyncFolderUsbToPC(usbToPc, pcJob.AbsoluteUSBPath, pcJob.PCPath);

            pcJob.GetUsbJob().diff = comparisonResult.PCDifferences;     //set the new difference on usbJob
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        private void NormalCleanSyncFolderPcToUsb(FolderMeta pcToUsb, string originDirectoryRoot, string destinationDirectoryRoot)
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

        private void NormalCleanSyncFolderUsbToPC(FolderMeta usbToPC, string originDirectoryRoot, string destinationDirectoryRoot)
        {
            foreach (FileMeta file in usbToPC.files)
            {
                switch (file.FileType)
                {
                    case ComponentMeta.Type.New: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                                                 break;
                    case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(originDirectoryRoot + file.Path + file.Name, destinationDirectoryRoot + file.Path + file.Name, bgWorker, onePercentSize);
                                                      break;
                    case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFile(destinationDirectoryRoot + file.Path + file.Name);
                                                     break;
                }
            }

            foreach (FolderMeta folder in usbToPC.folders)
            {
                switch (folder.FolderType)
                {
                    case ComponentMeta.Type.New: ReadAndWrite.CopyFolder(originDirectoryRoot + folder.Path + folder.Name, destinationDirectoryRoot + folder.Path + folder.Name, bgWorker, onePercentSize);
                                                 break;
                    case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFolder(destinationDirectoryRoot + folder.Path + folder.Name);
                                                     break;
                    case ComponentMeta.Type.Modified: NormalCleanSyncFolderUsbToPC(usbToPC, originDirectoryRoot, destinationDirectoryRoot);
                                                     break;
                }
            }
        }

        private void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            DifferenceToTreeConvertor convertor = new DifferenceToTreeConvertor();
            FolderMeta oldDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.USBDifferences);
            FolderMeta newDifferencesRoot = convertor.ConvertDifferencesToTreeStructure(comparisonResult.PCDifferences);

            ReSynchronizeFolders(oldDifferencesRoot, newDifferencesRoot, pcJob.PCPath, pcJob.AbsoluteUSBPath);
           
            Differences oldDifferences = convertor.ConvertTreeStructureToDifferences(oldDifferencesRoot);
            pcJob.GetUsbJob().diff = oldDifferences;
            if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            else
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        private void ReSynchronizeFolders(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
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
                        case ComponentMeta.Type.New: ReadAndWrite.CopyFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name);
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
                                case ComponentMeta.Type.Modified: ReSynchronizeFolders(oldDifferencesRoot, newDifferencesRoot, pathPC, pathUSB);
                                    break;
                            }
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
                        ReadAndWrite.CopyFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name, bgWorker, onePercentSize);
                        break;
                }
                newIndex++;
            }
            ClearFolderList(foldersOld);
        }

        private void ReSynchronizeToNewFolder(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
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
                    ReadAndWrite.CopyFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name, bgWorker, onePercentSize);
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
                ReadAndWrite.CopyFolder(pathPC + folderNew.Path + folderNew.Name, pathUSB + folderNew.Path + folderNew.Name, bgWorker, onePercentSize);
            }
            ClearFolderList(foldersOld);
        }

        private void ReSynchronizeToNewFolderFiles(FolderMeta folderOld, FolderMeta folderNew, string pathPC, string pathUSB)
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
                    ReadAndWrite.CopyFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name, bgWorker, onePercentSize);
                    indexNew++;
                }
                else
                {
                    switch (fileNew.FileType)
                    {
                        case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name, bgWorker, onePercentSize);
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
                ReadAndWrite.CopyFile(pathPC + filesNew[indexNew].Path + filesNew[indexNew].Name, pathUSB + filesNew[indexNew].Path + filesNew[indexNew].Name, bgWorker, onePercentSize);
                indexNew++;
            }
            ClearFileList(filesOld);
        }

        private void ReSynchronizeFiles(FolderMeta oldDifferencesRoot, FolderMeta newDifferencesRoot, string pathPC, string pathUSB)
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
                        default: ReadAndWrite.CopyFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileNew.Path + fileNew.Name, bgWorker, onePercentSize);
                            break;
                    }
                    newIndex++;
                }
                else  //fileOld need to be updated with fileNew
                {
                    switch (fileNew.FileType)
                    {
                        case ComponentMeta.Type.Deleted: ReadAndWrite.DeleteFile(pathUSB + fileOld.Path + fileOld.Name);
                            filesOld[oldIndex] = null;
                            break;
                        case ComponentMeta.Type.Modified: ReadAndWrite.CopyFile(pathPC + fileNew.Path + fileNew.Name, pathUSB + fileOld.Path + fileOld.Name, bgWorker, onePercentSize);
                            filesOld[oldIndex] = fileNew;
                            if(fileOld.FileType == ComponentMeta.Type.New) fileNew.FileType = ComponentMeta.Type.New;
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
/*

        #region Normal Synchronization methods
        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.AbsoluteUSBPath != null);
            Differences USBToPC = comparisonResult.USBDifferences;
            Differences PCToUSB = comparisonResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);

            ClearNullEntry(PCToUSB);

            SyncUSBToPC(USBToPC, pcJob);
            SyncPCToUSB(PCToUSB, pcJob);

            pcJob.GetUsbJob().diff = PCToUSB;     //set the new difference on usbJob
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);

        }

        private void SyncPCToUSB(Differences PCToUSB, PCJob pcJob)
        {
            List<FolderMeta> newFolderList = PCToUSB.getNewFolderList();
            List<FileMeta> newFileList = PCToUSB.getNewFileList();
            List<FileMeta> modifiedFileList = PCToUSB.getModifiedFileList();
            SyncPCToUSBNewFolder(pcJob, newFolderList);
            SyncPCToUSBNewFile(pcJob, newFileList);
            SyncPCToUSBModifiedFile(pcJob, modifiedFileList);

            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
        }

        private void SyncPCToUSBModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                if (modifiedFile != null) 
                    ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp", bgWorker, onePercentSize);

                i++;
            }
        }

        private void SyncPCToUSBNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                if (newFile != null) 
                    ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp", bgWorker, onePercentSize);

                i++;
            }
        }

        private void SyncPCToUSBNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                //ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i, bgWorker, onePercentSize);
                if (newFolder != null) 
                    ReadAndWrite.CopyFolder(pcJob, newFolder, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i,bgWorker,onePercentSize);
                i++;
            }
        }


        private void SyncUSBToPC(Differences USBToPC, PCJob pcJob)
        {
            List<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            List<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            List<FileMeta> newFileList = USBToPC.getNewFileList();
            List<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            List<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();
            SyncUSBToPCNewFolder(pcJob, newFolderList);
            SyncUSBtoPCDeleteFolder(pcJob, deletedFolderList);
            SyncUSbToPCNewFile(pcJob, newFileList);
            SyncUSBToPCModifiedFile(pcJob, modifiedFileList);
            SyncUSBToPCDeleteFile(pcJob, deletedFileList);
        }

        private void SyncUSBToPCDeleteFile(PCJob pcJob, List<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {

                if (deletedFile != null) 
                    ReadAndWrite.DeleteFile(pcJob.PCPath + deletedFile.Path + deletedFile.Name);
            }
        }

        private void SyncUSBToPCModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                if (modifiedFile != null) 
                    ReadAndWrite.CopyFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp", pcJob.PCPath + modifiedFile.Path + modifiedFile.Name,bgWorker,onePercentSize);
                ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp");

                i++;
            }
        }

        private void SyncUSbToPCNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {

                if(newFile != null) ReadAndWrite.CopyFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp", pcJob.PCPath + newFile.Path + newFile.Name, bgWorker, onePercentSize);
                ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");

                i++;
            }
        }

        private void SyncUSBtoPCDeleteFolder(PCJob pcJob, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                if (deletedFolder != null) 
                    ReadAndWrite.DeleteFolder(pcJob, deletedFolder);

            }
        }

        private void SyncUSBToPCNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                if (newFolder != null) //ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i, pcJob.PCPath + newFolder.Path + newFolder.Name, bgWorker, onePercentSize);
                    ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i, pcJob, newFolder,bgWorker,onePercentSize);
                ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);

                i++;
            }
        }
        #endregion

        #region Re-Synchronization methods
        private void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.AbsoluteUSBPath != null);
            Differences oldDifferences = comparisonResult.USBDifferences;
            Differences newDifferences = comparisonResult.PCDifferences;
            Debug.Assert(oldDifferences != null);
            Debug.Assert(newDifferences != null);

            ReSyncFolders(oldDifferences, newDifferences, pcJob);

            ReSyncFiles(oldDifferences, newDifferences, pcJob);

            pcJob.GetUsbJob().diff = oldDifferences;
            if (!pcJob.GetUsbJob().JobState.Equals(JobStatus.Incomplete))
                ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            else
                ReadAndWrite.ExportIncompleteJobToUSB(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        private void ReSyncFiles(Differences oldDifferences, Differences newDifferences, PCJob pcJob)
        {
            List<FileMeta> newFilesOld = oldDifferences.getNewFileList();
            List<FileMeta> newFilesNew = newDifferences.getNewFileList();
            List<FileMeta> deletedFilesNew = newDifferences.getDeletedFileList();
            List<FileMeta> modifiedFilesOld = oldDifferences.getModifiedFileList();
            List<FileMeta> modifiedFilesNew = newDifferences.getModifiedFileList();

            ReSyncDeletedFiles(oldDifferences, pcJob, newFilesOld, deletedFilesNew, modifiedFilesOld);
            RemoveNullComponentsFiles(pcJob, newFilesOld, "n");
            RemoveNullComponentsFiles(pcJob, modifiedFilesOld, "m");
            ReSyncModifiedFiles(oldDifferences, pcJob, newFilesOld, modifiedFilesOld, modifiedFilesNew);
            ReSyncNewFiles(pcJob, oldDifferences, newFilesNew);
        }

        private void ReSyncFolders(Differences oldDifferences, Differences newDifferences, PCJob pcJob)
        {
            List<FolderMeta> newFoldersOld = oldDifferences.getNewFolderList();
            List<FolderMeta> newFoldersNew = newDifferences.getNewFolderList();
            List<FolderMeta> deletedFoldersNew = newDifferences.getDeletedFolderList();
            ReSyncDeletedFolders(oldDifferences, pcJob, newFoldersOld, deletedFoldersNew);

            RemoveNullComponentsFolders(pcJob, newFoldersOld, "n");
            ReSyncNewFolders(oldDifferences, pcJob, newFoldersNew);
        }

        private void ReSyncNewFiles(PCJob pcJob, Differences oldDifferences, List<FileMeta> newFilesNew)
        {

            int i = oldDifferences.getNewFileList().Count;

            foreach (FileMeta newFile in newFilesNew)
            {
                bool found = false;
                int n = 0;
                foreach (FolderMeta newFolderOld in oldDifferences.getNewFolderList())
                {
                    if (newFolderOld != null)
                    {
                        if (newFile.Path.IndexOf(newFolderOld.Path + newFolderOld.Name) == 0)
                        {
                            found = true;
                            ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + newFile.Path.Substring((newFolderOld.Path + newFolderOld.Name).Length) + newFile.Name,bgWorker,onePercentSize);
                        }
                    }
                    n++;
                    if (found) break;
                }
                if (!found)
                {
                    oldDifferences.AddNewFileDifference(newFile);
                    ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp", bgWorker, onePercentSize);
                    i++;
                }
            }
        }

        private void ReSyncModifiedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> newFilesOld, List<FileMeta> modifiedFilesOld, List<FileMeta> modifiedFilesNew)
        {
            foreach (FileMeta modifiedFile in modifiedFilesNew)
            {
                bool found = false;

                int n = 0;
                foreach (FolderMeta newFolderOld in oldDifferences.getNewFolderList())
                {
                    if (newFolderOld != null)
                    {
                        if (modifiedFile.Path.IndexOf(newFolderOld.Path + newFolderOld.Name) == 0)
                        {
                            ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + modifiedFile.Path.Substring((newFolderOld.Path + newFolderOld.Name).Length) + modifiedFile.Name,bgWorker,onePercentSize);
                            found = true;
                        }
                    }
                    n++;
                    if (found) break;
                }
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if (newFilesOld[i] != null)
                    {
                        if ((modifiedFile.Path + modifiedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                        {
                            newFilesOld[i] = modifiedFile;
                            ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");
                            ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp", bgWorker, onePercentSize);
                            found = true;
                        }
                    }
                }

                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if (modifiedFilesOld[i] != null)
                    {
                        if ((modifiedFile.Path + modifiedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                        {
                            modifiedFilesOld[i] = modifiedFile;
                            ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp");
                            ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp", bgWorker, onePercentSize);
                            found = true;
                        }
                    }
                }
                if (!found)
                {
                    ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + modifiedFilesOld.Count + ".temp", bgWorker, onePercentSize);
                    oldDifferences.AddModifiedFileDifference(modifiedFile);
                }
            }
        }

        private void ReSyncDeletedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> newFilesOld, List<FileMeta> deletedFilesNew, List<FileMeta> modifiedFilesOld)
        {

            foreach (FileMeta deletedFile in deletedFilesNew)
            {
                bool found = false; int n = 0;
                foreach (FolderMeta newFolderOld in oldDifferences.getNewFolderList())
                {
                    if (newFolderOld != null)
                    {
                        if (deletedFile.Path.IndexOf(newFolderOld.Path + newFolderOld.Name) == 0)
                        {
                            found = true;
                            //string test =(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + deletedFile.Path.Substring(newFolderOld.Path.Length) + deletedFile.Name);
                            ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + deletedFile.Path.Substring((newFolderOld.Path + newFolderOld.Name).Length) + deletedFile.Name);

                        }
                    }
                    n++;
                    if (found) break;
                }
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if (newFilesOld[i] != null)
                    {
                        if ((deletedFile.Path + deletedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                        {
                            ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");
                            newFilesOld[i] = null;
                            found = true;
                        }
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if (modifiedFilesOld[i] != null)
                    {
                        if ((deletedFile.Path + deletedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                        {
                            ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp");
                            modifiedFilesOld[i] = null;
                            oldDifferences.AddDeletedFileDifference(deletedFile);
                            found = true;
                        }
                    }
                }
                if (!found) oldDifferences.AddDeletedFileDifference(deletedFile);
            }
        }

        private void ReSyncNewFolders(Differences oldDifferences, PCJob pcJob, List<FolderMeta> newFoldersNew)
        {

            int j = oldDifferences.getNewFolderList().Count;
            foreach (FolderMeta newFolder in newFoldersNew)
            {
                bool found = false;
                int n = 0;
                foreach (FolderMeta newFolderOld in oldDifferences.getNewFolderList())
                {
                    if (newFolderOld != null)
                    {
                        if (newFolder.Path.IndexOf(newFolderOld.Path + newFolderOld.Name) == 0)
                        {
                            found = true;
                            string test = pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + newFolder.Path.Substring((newFolderOld.Path + newFolderOld.Name).Length) + newFolder.Name;
                            ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + n + "\\" + newFolder.Path.Substring((newFolderOld.Path + newFolderOld.Name).Length) + newFolder.Name);
                        }
                    }
                    n++;
                    if (found) break;
                }
                if (!found)
                {
                    oldDifferences.AddNewFolderDifference(newFolder);
                    ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + j, bgWorker, onePercentSize);
                    j++;
                }
            }
        }

        private void ReSyncDeletedFolders(Differences oldDifferences, PCJob pcJob, List<FolderMeta> newFoldersOld, List<FolderMeta> deletedFoldersNew)
        {

            foreach (FolderMeta deletedFolder in deletedFoldersNew)
            {
                bool found = false;
               
                for (int i = 0; i < newFoldersOld.Count && !found; i++)
                {
                    if (newFoldersOld[i] != null)
                    {
                        if ((deletedFolder.Path + deletedFolder.Name).Equals(newFoldersOld[i].Path + newFoldersOld[i].Name))
                        {
                            ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);
                            newFoldersOld[i] = null;
                            found = true;
                        }
                    }
                }
                for (int i = 0; i < newFoldersOld.Count && !found; i++)
                {
                    if (newFoldersOld[i] != null)
                    {
                        if ((deletedFolder.Path + deletedFolder.Name).IndexOf(newFoldersOld[i].Path + newFoldersOld[i].Name) == 0 &&
                            (deletedFolder.Path + deletedFolder.Name).Length > (newFoldersOld[i].Path + newFoldersOld[i].Name).Length)
                        {
                            ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + deletedFolder.Path.Substring((newFoldersOld[i].Path + newFoldersOld[i].Name).Length) + deletedFolder.Name);
                            found = true;
                        }
                        else if ((deletedFolder.Path + deletedFolder.Name).IndexOf(newFoldersOld[i].Path + newFoldersOld[i].Name) == 0 &&
                            (deletedFolder.Path + deletedFolder.Name).Length < (newFoldersOld[i].Path + newFoldersOld[i].Name).Length)
                        {
                            ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);
                            newFoldersOld[i] = null;
                        }
                        else if ((deletedFolder.Path + deletedFolder.Name).Equals(newFoldersOld[i].Path + newFoldersOld[i].Name))
                        {
                            ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);
                            newFoldersOld[i] = null;
                            found = true;
                        }
                    }
                }
                if (!found) oldDifferences.AddDeletedFolderDifference(deletedFolder);
            }
        }



        private void RemoveNullComponentsFiles(PCJob pcJob, List<FileMeta> files, string listType)
        {
            int lastFreeIndex = 0;
            int lastFileIndex = files.Count - 1;
            bool rearranging = true;

            while (rearranging)
            {
                while (lastFreeIndex < lastFileIndex && files[lastFreeIndex] != null) lastFreeIndex++;
                while (lastFreeIndex < lastFileIndex && files[lastFileIndex] == null) lastFileIndex--;
                if (lastFreeIndex < lastFileIndex)
                {
                    ReadAndWrite.RenameFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + listType + lastFileIndex + ".temp", pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + listType + lastFreeIndex + ".temp");
                    files[lastFreeIndex] = files[lastFileIndex];
                    files[lastFileIndex] = null;
                }
                else rearranging = false;
            }
            while (lastFreeIndex < files.Count && files[lastFreeIndex] != null) lastFreeIndex++;
            while (lastFreeIndex < files.Count)
            {
                files.RemoveAt(lastFreeIndex);
            }
        }

        public void RemoveNullComponentsFolders(PCJob pcJob, List<FolderMeta> folders, string listType)
        {
            int lastFreeIndex = 0;
            int lastFileIndex = folders.Count;
            bool rearranging = true;

            while (rearranging)
            {
                while (lastFreeIndex < lastFileIndex && folders[lastFreeIndex] != null) lastFreeIndex++;
                while (lastFreeIndex < lastFileIndex && folders[lastFileIndex - 1] == null) lastFileIndex--;
                if (lastFreeIndex < lastFileIndex)
                {
                    ReadAndWrite.RenameFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + listType + lastFileIndex, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + listType + lastFreeIndex);
                    folders[lastFreeIndex] = folders[lastFileIndex];
                    folders[lastFileIndex] = null;
                }
                else rearranging = false;
            }
            while (lastFreeIndex < folders.Count && folders[lastFreeIndex] != null) lastFreeIndex++;
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }
        #endregion
        */
    }
}
