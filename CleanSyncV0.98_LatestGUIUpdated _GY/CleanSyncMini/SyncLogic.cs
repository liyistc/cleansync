using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using CleanSync;

namespace DirectoryInformation
{
    public class SyncLogic
    {
        private System.ComponentModel.BackgroundWorker bgWorker;

        private double onePercentSize;
        private long totalSize;
        //private int operationCount;

        public SyncLogic()
        {
            totalSize = 0;
            onePercentSize = 0;
        }

        private void initializeTotalSize(ComparisonResult comparisonResult)
        {
            totalSize = 0;

            updateFileSize(comparisonResult.PCDifferences.getNewFileList());
            //updateFileSize(comparisonResult.PCDifferences.getDeletedFileList());
            updateFileSize(comparisonResult.PCDifferences.getModifiedFileList());
           
            updateFileSize(comparisonResult.USBDifferences.getNewFileList());
            //updateFileSize(comparisonResult.USBDifferences.getDeletedFileList());
            updateFileSize(comparisonResult.USBDifferences.getModifiedFileList());

            updateFolderSize(comparisonResult.PCDifferences.getNewFolderList());
            //updateFolderSize(comparisonResult.PCDifferences.getDeletedFolderList());
            updateFolderSize(comparisonResult.USBDifferences.getNewFolderList());
            //updateFolderSize(comparisonResult.USBDifferences.getDeletedFolderList());

            onePercentSize = totalSize / 100;
        }

        private void updateFolderSize(List<FolderMeta> folders)
        {
            foreach (FolderMeta folder in folders)
            {
                if (folder.files != null)
                    updateFileSize(folder.files);
                if (folder.folders != null)
                    updateFolderSize(folder.folders);
            }
        }

        private void updateFileSize(List<FileMeta> files)
        {
            foreach (FileMeta file in files)
            {
                totalSize += file.Size;
            }
        }

        public void CleanSync(ComparisonResult comparisonResult, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            this.bgWorker = worker;
            initializeTotalSize(comparisonResult);

            if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID)
            {
               CleanSyncReSync(comparisonResult, pcJob);
            }
            else NormalCleanSync(comparisonResult, pcJob);

            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
            this.bgWorker = null;
        }

        private void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.AbsoluteUSBPath != null);
            Differences USBToPC = comparisonResult.USBDifferences;
            Differences PCToUSB = comparisonResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);

            SyncUSBToPC(USBToPC, pcJob);
            
            SyncPCToUSB(PCToUSB, pcJob);

            pcJob.GetUsbJob().diff = PCToUSB;
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);

        }

        private void AcceptJobSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
        }

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
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
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
                oldDifferences.AddNewFileDifference(newFile);
                ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp",bgWorker,onePercentSize);
                i++;
            }
        }

        private void ReSyncModifiedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> newFilesOld, List<FileMeta> modifiedFilesOld, List<FileMeta> modifiedFilesNew)
        {
            foreach (FileMeta modifiedFile in modifiedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        newFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");
                        ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp",bgWorker,onePercentSize);
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        modifiedFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp");
                        ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp",bgWorker,onePercentSize);
                        found = true;
                    }
                }
                if (!found)
                {
                    ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + modifiedFilesOld.Count + ".temp",bgWorker,onePercentSize);
                    oldDifferences.AddModifiedFileDifference(modifiedFile);
                }
            }
        }

        private void ReSyncDeletedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> newFilesOld, List<FileMeta> deletedFilesNew, List<FileMeta> modifiedFilesOld)
        {

            foreach (FileMeta deletedFile in deletedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");
                        newFilesOld[i] = null;
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName +"m" + i + ".temp");
                        modifiedFilesOld[i] = null;
                        oldDifferences.AddDeletedFileDifference(deletedFile);
                        found = true;
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
                oldDifferences.AddNewFolderDifference(newFolder);
                ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + j,bgWorker,onePercentSize);
                j++;
            }
        }

        private void ReSyncDeletedFolders(Differences oldDifferences, PCJob pcJob, List<FolderMeta> newFoldersOld, List<FolderMeta> deletedFoldersNew)
        {

            foreach (FolderMeta deletedFolder in deletedFoldersNew)
            {
                bool found = false;
                for (int i = 0; i < newFoldersOld.Count && !found; i++)
                {
                    if ((deletedFolder.Path + deletedFolder.Name).Equals(newFoldersOld[i].Path + newFoldersOld[i].Name))
                    {
                        ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);
                        newFoldersOld[i] = null;
                        found = true;
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
              //  if(files[lastFreeIndex] != null) Console.wr
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
            while(lastFreeIndex< folders.Count && folders[lastFreeIndex] != null) lastFreeIndex++;
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }

        public void SyncPCToUSB(Differences PCToUSB, PCJob pcJob)
        {
            List<FolderMeta> newFolderList = PCToUSB.getNewFolderList();
            List<FileMeta> newFileList = PCToUSB.getNewFileList();
            List<FileMeta> modifiedFileList = PCToUSB.getModifiedFileList();
            Debug.Assert(newFolderList != null);
            Debug.Assert(newFileList != null);
            Debug.Assert(modifiedFileList != null);
            SyncPCToUSBNewFolder(pcJob, newFolderList);
            SyncPCToUSBNewFile(pcJob, newFileList);
            SyncPCToUSBModifiedFile(pcJob, modifiedFileList);

            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
        }

        public void SyncPCToUSB(Differences PCToUSB, PCJob pcJob, System.ComponentModel.BackgroundWorker worker)
        {
            this.bgWorker = worker;

            this.totalSize = 0;
            this.onePercentSize = 0;

            //updateFileSize(PCToUSB.getDeletedFileList());
            updateFileSize(PCToUSB.getNewFileList());
            updateFileSize(PCToUSB.getModifiedFileList());
           
            updateFolderSize(PCToUSB.getNewFolderList());
            //updateFolderSize(PCToUSB.getDeletedFolderList());

            onePercentSize = totalSize / 100;

            SyncPCToUSB(PCToUSB, pcJob);

            this.bgWorker = null;
        }

        public void SyncPCToUSBModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(pcJob.PCPath != null && pcJob.AbsoluteUSBPath != null);
                ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp",bgWorker,onePercentSize);

                i++;
            }
        }

        public void SyncPCToUSBNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp",bgWorker,onePercentSize);

                i++;
            }
        }

        public void SyncPCToUSBNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Path != null & newFolder.Name != null);
                ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i,bgWorker,onePercentSize);

                i++;
            }
        }


        public void SyncUSBToPC(Differences USBToPC, PCJob pcJob)
        {
            Debug.Assert(USBToPC != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.AbsoluteUSBPath != null);
            List<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            List<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            List<FileMeta> newFileList = USBToPC.getNewFileList();
            List<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            List<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();
            Debug.Assert(newFolderList != null);
            Debug.Assert(newFileList != null);
            Debug.Assert(modifiedFileList != null);
            Debug.Assert(deletedFileList != null);
            Debug.Assert(deletedFolderList != null);

            SyncUSBToPCNewFolder(pcJob, newFolderList);
            SyncUSBtoPCDeleteFolder(pcJob, deletedFolderList);
            SyncUSbToPCNewFile(pcJob, newFileList);
            SyncUSBToPCModifiedFile(pcJob, modifiedFileList);
            SyncUSBToPCDeleteFile(pcJob, deletedFileList);

            //Delete usb temp folders and files
           // ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
        }

        public void SyncUSBToPCDeleteFile(PCJob pcJob, List<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {
                Debug.Assert(deletedFile != null);
                Debug.Assert(deletedFile.Name != null);
                Debug.Assert(deletedFile.Path != null);
                ReadAndWrite.DeleteFile(pcJob.PCPath + deletedFile.Path + deletedFile.Name);

            }
        }

        public void SyncUSBToPCModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(modifiedFile.Name != null && modifiedFile.Path != null);
                ReadAndWrite.CopyFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp", pcJob.PCPath + modifiedFile.Path + modifiedFile.Name,bgWorker,onePercentSize);
                ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "m" + i + ".temp");

                i++;
            }
        }

        public void SyncUSbToPCNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp", pcJob.PCPath + newFile.Path + newFile.Name,bgWorker,onePercentSize);
                ReadAndWrite.DeleteFile(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i + ".temp");

                i++;
            }
        }

        public void SyncUSBtoPCDeleteFolder(PCJob pcJob, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                Debug.Assert(deletedFolder != null);
                Debug.Assert(deletedFolder.Name != null && deletedFolder.Path != null);
                ReadAndWrite.DeleteFolder(pcJob,deletedFolder);

            }
        }

        public void SyncUSBToPCNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Name != null && newFolder.Path != null);
                ReadAndWrite.CopyFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName +"n" + i, pcJob.PCPath + newFolder.Path + newFolder.Name,bgWorker,onePercentSize);
                ReadAndWrite.DeleteFolder(pcJob.AbsoluteUSBPath + "\\" + pcJob.JobName + "n" + i);

                i++;
            }
        }
    }
}
