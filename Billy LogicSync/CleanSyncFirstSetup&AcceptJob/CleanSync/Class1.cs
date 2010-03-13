using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using TestStubs;

namespace DirectoryInformation
{
    public static class SyncLogic
    {

        public static void CleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            if (pcJob.PCID == pcJob.GetUsbJob().MostRecentPCID)
            {
               CleanSyncReSync(comparisonResult, pcJob);
            }
            else NormalCleanSync(comparisonResult, pcJob);

            pcJob.GetUsbJob().MostRecentPCID = pcJob.PCID;
        }

        private static void NormalCleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.USBPath != null);
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

        private static void AcceptJobSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
        }

        private static void CleanSyncReSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.USBPath != null);
            Differences oldDifferences = comparisonResult.USBDifferences;
            Differences newDifferences = comparisonResult.PCDifferences;
            Debug.Assert(oldDifferences != null);
            Debug.Assert(newDifferences != null);

            ReSyncFolders(oldDifferences, newDifferences, pcJob);
            ReSyncFiles(oldDifferences, newDifferences, pcJob);

            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
        }

        private static void ReSyncFiles(Differences oldDifferences, Differences newDifferences, PCJob pcJob)
        {
            List<FileMeta> newFilesOld = oldDifferences.getNewFileList();
            List<FileMeta> newFilesNew = newDifferences.getNewFileList();
            List<FileMeta> deletedFilesNew = newDifferences.getDeletedFileList();
            List<FileMeta> modifiedFilesOld = oldDifferences.getModifiedFileList();
            List<FileMeta> modifiedFilesNew = newDifferences.getModifiedFileList();

            ReSyncDeletedFiles(oldDifferences, pcJob, newFilesOld, deletedFilesNew, modifiedFilesOld);
            RemoveNullComponentsFiles(pcJob, newFilesOld, "new");
            RemoveNullComponentsFiles(pcJob, modifiedFilesOld, "modified");
            ReSyncModifiedFiles(oldDifferences, pcJob, modifiedFilesOld, modifiedFilesNew);
            ReSyncNewFiles(pcJob, oldDifferences, newFilesNew);
        }

        private static void ReSyncFolders(Differences oldDifferences, Differences newDifferences, PCJob pcJob)
        {
            List<FolderMeta> newFoldersOld = oldDifferences.getNewFolderList();
            List<FolderMeta> newFoldersNew = newDifferences.getNewFolderList();
            List<FolderMeta> deletedFoldersNew = newDifferences.getDeletedFolderList();
            ReSyncDeletedFolders(oldDifferences, pcJob, newFoldersOld, deletedFoldersNew);
            RemoveNullComponentsFolders(pcJob, newFoldersOld, "new");
            ReSyncNewFolders(oldDifferences, pcJob, newFoldersNew);
        }

        private static void ReSyncNewFiles(PCJob pcJob, Differences oldDifferences, List<FileMeta> newFilesNew)
        {

            int i = oldDifferences.getNewFileList().Count;
            foreach (FileMeta newFile in newFilesNew)
            {
                oldDifferences.AddNewFileDifference(newFile);
                ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.USBPath + "\\new" + i + ".temp");
                i++;
            }
        }

        private static void ReSyncModifiedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> modifiedFilesOld, List<FileMeta> modifiedFilesNew)
        {
            List<FileMeta> newFilesOld = oldDifferences.getNewFileList();
            foreach (FileMeta modifiedFile in modifiedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        newFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(pcJob.USBPath + "\\new" + i + ".temp");
                        ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.USBPath + "\\new" + i + ".temp");
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        modifiedFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(pcJob.USBPath + "\\modified" + i + ".temp");
                        ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.USBPath + "\\modified" + i + ".temp");
                        found = true;
                    }
                }
                if (!found)
                {
                    ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.USBPath + "\\modified" + modifiedFilesOld.Count + ".temp");
                    oldDifferences.AddModifiedFileDifference(modifiedFile);
                }
            }
        }

        private static void ReSyncDeletedFiles(Differences oldDifferences, PCJob pcJob, List<FileMeta> newFilesOld, List<FileMeta> deletedFilesNew, List<FileMeta> modifiedFilesOld)
        {
            foreach (FileMeta deletedFile in deletedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(pcJob.USBPath + "\\new" + i + ".temp");
                        newFilesOld[i] = null;
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(pcJob.USBPath + "\\modified" + i + ".temp");
                        modifiedFilesOld[i] = null;
                        oldDifferences.AddDeletedFileDifference(deletedFile);
                        found = true;
                    }
                }
                if (!found) oldDifferences.AddDeletedFileDifference(deletedFile);
            }
        }

        private static void ReSyncNewFolders(Differences oldDifferences, PCJob pcJob, List<FolderMeta> newFoldersNew)
        {

            int j = oldDifferences.getNewFolderList().Count;
            foreach (FolderMeta newFolder in newFoldersNew)
            {
                oldDifferences.AddNewFolderDifference(newFolder);
                ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.USBPath + "\\new" + j);
                j++;
            }
        }

        private static void ReSyncDeletedFolders(Differences oldDifferences, PCJob pcJob, List<FolderMeta> newFoldersOld, List<FolderMeta> deletedFoldersNew)
        {

            foreach (FolderMeta deletedFolder in deletedFoldersNew)
            {
                bool found = false;
                for (int i = 0; i < newFoldersOld.Count && !found; i++)
                {
                    if ((deletedFolder.Path + deletedFolder.Name).Equals(newFoldersOld[i].Path + newFoldersOld[i].Name))
                    {
                        ReadAndWrite.DeleteFolder(pcJob.USBPath + "\\new" + i);
                        newFoldersOld[i] = null;
                        found = true;
                    }
                }
                if (!found) oldDifferences.AddDeletedFolderDifference(deletedFolder);
            }
        }

        private static void RemoveNullComponentsFiles(PCJob pcJob, List<FileMeta> files, string listType)
        {
            int lastFreeIndex = 0;
            int lastFileIndex = files.Count;
            bool rearranging = true;

            while(rearranging)
            {
                while(lastFreeIndex < lastFileIndex && files[lastFreeIndex] != null) lastFreeIndex++;
                while(lastFreeIndex < lastFileIndex && files[lastFileIndex] == null) lastFileIndex--;
                if (lastFreeIndex < lastFileIndex)
                {
                    ReadAndWrite.RenameFile(pcJob.USBPath + "\\" + listType + lastFileIndex + ".temp", pcJob.USBPAth + "\\" + listType + lastFreeIndex + ".temp");
                    files[lastFreeIndex] = files[lastFileIndex];
                    files[lastFileIndex] = null;
                }
                else rearranging = false;
            }
            while (lastFreeIndex < files.Count)
            {
                files.RemoveAt(lastFreeIndex);
            }
        }

        public static void RemoveNullComponentsFolders(PCJob pcJob, List<FolderMeta> folders, string listType)
        {
            int lastFreeIndex = 0;
            int lastFileIndex = folders.Count;
            bool rearranging = true;

            while (rearranging)
            {
                while (lastFreeIndex < lastFileIndex && files[lastFreeIndex] != null) lastFreeIndex++;
                while (lastFreeIndex < lastFileIndex && files[lastFileIndex] == null) lastFileIndex--;
                if (lastFreeIndex < lastFileIndex)
                {
                    ReadAndWrite.RenameFolder(pcJob.USBPath + "\\" + listType + lastFileIndex, pcJob.USBPAth + "\\" + listType + lastFreeIndex);
                    folders[lastFreeIndex] = folders[lastFileIndex];
                    folders[lastFileIndex] = null;
                }
                else rearranging = false;
            }
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }

        public static void SyncPCToUSB(Differences PCToUSB, PCJob pcJob)
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
        }

        public static void SyncPCToUSBModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(pcJob.PCPath != null && pcJob.USBPath != null);
                ReadAndWrite.CopyFile(pcJob.PCPath + modifiedFile.Path + modifiedFile.Name, pcJob.USBPath + "\\modified" + i + ".temp");
                i++;
            }
        }

        public static void SyncPCToUSBNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(pcJob.PCPath + newFile.Path + newFile.Name, pcJob.USBPath + "\\new" + i + ".temp");
                i++;
            }
        }

        public static void SyncPCToUSBNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Path != null & newFolder.Name != null);
                ReadAndWrite.CopyFolder(pcJob.PCPath + newFolder.Path + newFolder.Name, pcJob.USBPath + "\\new" + i);
                i++;
            }
        }


        public static void SyncUSBToPC(Differences USBToPC, PCJob pcJob)
        {
            Debug.Assert(USBToPC != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.USBPath != null);
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
            ReadAndWrite.DeleteFolderContent(pcJob.USBPath);
        }

        public static void SyncUSBToPCDeleteFile(PCJob pcJob, List<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {
                Debug.Assert(deletedFile != null);
                Debug.Assert(deletedFile.Name != null);
                Debug.Assert(deletedFile.Path != null);
                ReadAndWrite.DeleteFile(pcJob.PCPath + deletedFile.Path + deletedFile.Name);
            }
        }

        public static void SyncUSBToPCModifiedFile(PCJob pcJob, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(modifiedFile.Name != null && modifiedFile.Path != null);
                ReadAndWrite.CopyFile(pcJob.USBPath + "\\modified" + i + ".temp", pcJob.PCPath + modifiedFile.Path + modifiedFile.Name);
                i++;
            }
        }

        public static void SyncUSbToPCNewFile(PCJob pcJob, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(pcJob.USBPath + "\\new" + i + ".temp", pcJob.PCPath + newFile.Path + newFile.Name);
                i++;
            }
        }

        public static void SyncUSBtoPCDeleteFolder(PCJob pcJob, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                Debug.Assert(deletedFolder != null);
                Debug.Assert(deletedFolder.Name != null && deletedFolder.Path != null);
                ReadAndWrite.DeleteFolder(pcJob.PCPath + deletedFolder.Path + deletedFolder.Name);
            }
        }

        public static void SyncUSBToPCNewFolder(PCJob pcJob, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Name != null && newFolder.Path != null);
                ReadAndWrite.CopyFolder(pcJob.USBPath + "\\new" + i, pcJob.PCPath + newFolder.Path + newFolder.Name);
                i++;
            }
        }
    }
}
