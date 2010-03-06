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

        public static void CleanSync(ComparisonResult comparisonResult, PCJob job)
        {
            if (job.PCID == job.GetUsbJob().MostRecentPCID)
            {
                CleanSyncReSync(comparisonResult, job);
            }
            else NormalCleanSync(comparisonResult, job);

            job.GetUsbJob().MostRecentPCID = job.PCID;
        }

        private static void NormalCleanSync(ComparisonResult comparisonResult, PCJob job)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(job != null);
            Debug.Assert(job.PCPath != null && job.USBPath != null);
            Differences USBToPC = comparisonResult.USBDifferences;
            Differences PCToUSB = comparisonResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);

            SyncUSBToPC(USBToPC, job);
            SyncPCToUSB(PCToUSB, job);
            job.GetUsbJob().diff = PCToUSB;
            ReadAndWrite.ExportUSBJob(job.GetUsbJob());
            job.FolderInfo = ReadAndWrite.BuildTree(job.PCPath);
            ReadAndWrite.ExportPCJob(job);

        }

        private static void AcceptJobSync(ComparisonResult comparisonResult, PCJob job)
        {
        }

        private static void CleanSyncReSync(ComparisonResult comparisonResult, PCJob job)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(job != null);
            Debug.Assert(job.PCPath != null && job.USBPath != null);
            Differences oldDifferences = comparisonResult.USBDifferences;
            Differences newDifferences = comparisonResult.PCDifferences;
            Debug.Assert(oldDifferences != null);
            Debug.Assert(newDifferences != null);

            ReSyncFolders(oldDifferences, newDifferences, job);
            ReSyncFiles(oldDifferences, newDifferences, job);

            ReadAndWrite.ExportUSBJob(job.GetUsbJob());
            job.FolderInfo = ReadAndWrite.BuildTree(job.PCPath);
            ReadAndWrite.ExportPCJob(job);
            
        }

        private static void ReSyncFiles(Differences oldDifferences, Differences newDifferences, PCJob job)
        {
            List<FileMeta> newFilesOld = oldDifferences.getNewFileList();
            List<FileMeta> newFilesNew = newDifferences.getNewFileList();
            List<FileMeta> deletedFilesNew = newDifferences.getDeletedFileList();
            List<FileMeta> modifiedFilesOld = oldDifferences.getModifiedFileList();
            List<FileMeta> modifiedFilesNew = newDifferences.getModifiedFileList();

            ReSyncDeletedFiles(oldDifferences, job, newFilesOld, deletedFilesNew, modifiedFilesOld);
            RemoveNullComponentsFiles(job, newFilesOld, "new");
            RemoveNullComponentsFiles(job, modifiedFilesOld, "modified");
            ReSyncModifiedFiles(oldDifferences, job, newFilesOld, modifiedFilesOld, modifiedFilesNew);
            ReSyncNewFiles(job, oldDifferences, newFilesNew);
        }

        private static void ReSyncFolders(Differences oldDifferences, Differences newDifferences, PCJob job)
        {
            List<FolderMeta> newFoldersOld = oldDifferences.getNewFolderList();
            List<FolderMeta> newFoldersNew = newDifferences.getNewFolderList();
            List<FolderMeta> deletedFoldersNew = newDifferences.getDeletedFolderList();
            ReSyncDeletedFolders(oldDifferences, job, newFoldersOld, deletedFoldersNew);

            RemoveNullComponentsFolders(job, newFoldersOld, "new");
            ReSyncNewFolders(oldDifferences, job, newFoldersNew);
        }

        private static void ReSyncNewFiles(PCJob job, Differences oldDifferences, List<FileMeta> newFilesNew)
        {
            int i = 0;
            foreach (FileMeta newFile in newFilesNew)
            {
                oldDifferences.AddNewFileDifference(newFile);
                ReadAndWrite.CopyFile(job.PCPath + newFile.Path + newFile.Name, job.USBPath + "\\new" + i + ".temp");
                i++;
            }
        }

        private static void ReSyncModifiedFiles(Differences oldDifferences, PCJob job, List<FileMeta> newFilesOld, List<FileMeta> modifiedFilesOld, List<FileMeta> modifiedFilesNew)
        {
            foreach (FileMeta modifiedFile in modifiedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        newFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(job.USBPath + "\\new" + i + ".temp");
                        ReadAndWrite.CopyFile(job.PCPath + modifiedFile.Path + modifiedFile.Name, job.USBPath + "\\new" + i + ".temp");
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((modifiedFile.Path + modifiedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        modifiedFilesOld[i] = modifiedFile;
                        ReadAndWrite.DeleteFile(job.USBPath + "\\modified" + i + ".temp");
                        ReadAndWrite.CopyFile(job.PCPath + modifiedFile.Path + modifiedFile.Name, job.USBPath + "\\modified" + i + ".temp");
                        found = true;
                    }
                }
                if (!found)
                {
                    ReadAndWrite.CopyFile(job.PCPath + modifiedFile.Path + modifiedFile.Name, job.USBPath + "\\modified" + modifiedFilesOld.Count + ".temp");
                    oldDifferences.AddModifiedFileDifference(modifiedFile);
                }
            }
        }

        private static void ReSyncDeletedFiles(Differences oldDifferences, PCJob job, List<FileMeta> newFilesOld, List<FileMeta> deletedFilesNew, List<FileMeta> modifiedFilesOld)
        {

            foreach (FileMeta deletedFile in deletedFilesNew)
            {
                bool found = false;
                for (int i = 0; i < newFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(newFilesOld[i].Path + newFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(job.USBPath + "\\new" + i + ".temp");
                        newFilesOld[i] = null;
                        found = true;
                    }
                }
                for (int i = 0; i < modifiedFilesOld.Count && !found; i++)
                {
                    if ((deletedFile.Path + deletedFile.Name).Equals(modifiedFilesOld[i].Path + modifiedFilesOld[i].Name))
                    {
                        ReadAndWrite.DeleteFile(job.USBPath + "\\modified" + i + ".temp");
                        modifiedFilesOld[i] = null;
                        oldDifferences.AddDeletedFileDifference(deletedFile);
                        found = true;
                    }
                }
                if (!found) modifiedFilesOld.Add(deletedFile);
            }
        }

        private static void ReSyncNewFolders(Differences oldDifferences, PCJob job, List<FolderMeta> newFoldersNew)
        {

            int j = newFoldersNew.Count;
            foreach (FolderMeta newFolder in newFoldersNew)
            {
                oldDifferences.AddNewFolderDifference(newFolder);
                ReadAndWrite.CopyFolder(job.PCPath + newFolder.Path + newFolder.Name, job.USBPath + "\\new" + j);
                j++;
            }
        }

        private static void ReSyncDeletedFolders(Differences oldDifferences, PCJob job, List<FolderMeta> newFoldersOld, List<FolderMeta> deletedFoldersNew)
        {

            foreach (FolderMeta deletedFolder in deletedFoldersNew)
            {
                bool found = false;
                for (int i = 0; i < newFoldersOld.Count && !found; i++)
                {
                    if ((deletedFolder.Path + deletedFolder.Name).Equals(newFoldersOld[i].Path + newFoldersOld[i].Name))
                    {
                        ReadAndWrite.DeleteFolder(job.USBPath + "\\new" + i);
                        newFoldersOld[i] = null;
                        found = true;
                    }
                }
                if (!found) oldDifferences.AddDeletedFolderDifference(deletedFolder);
            }
        }
        
        private static void RemoveNullComponentsFiles(PCJob job, List<FileMeta> files, string listType)
        {
            int lastFreeIndex = 0;
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i] != null)
                {
                    if (lastFreeIndex < i)
                    {
                        //ReadAndWrite.RenameFolder(job.USBPath + "\\" + listType + i, job.USBPAth + "\\" + listType + lastFreeIndex + ".temp");
                        files[lastFreeIndex] = files[i];
                        files[i] = null;
                    }
                    lastFreeIndex++;
                }
            }
            while (lastFreeIndex < files.Count)
            {
                files.RemoveAt(lastFreeIndex);
            }
        }

        public static void RemoveNullComponentsFolders(PCJob job, List<FolderMeta> folders, string listType)
        {
            int lastFreeIndex = 0;
            for (int i = 0; i < folders.Count; i++)
            {
                if (folders[i] != null)
                {
                    if (lastFreeIndex < i)
                    {
                        //ReadAndWrite.RenameFolder(job.USBPath + "\\" + listType + i, job.USBPAth + "\\" + listType + lastFreeIndex);
                        folders[lastFreeIndex] = folders[i];
                        folders[i] = null;
                    }
                    lastFreeIndex++;
                }
            }
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }

        public static void SyncPCToUSB(Differences PCToUSB, PCJob job)
        {
            List<FolderMeta> newFolderList = PCToUSB.getNewFolderList();
            List<FileMeta> newFileList = PCToUSB.getNewFileList();
            List<FileMeta> modifiedFileList = PCToUSB.getModifiedFileList();
            Debug.Assert(newFolderList != null);
            Debug.Assert(newFileList != null);
            Debug.Assert(modifiedFileList != null);
            SyncPCToUSBNewFolder(job, newFolderList);
            SyncPCToUSBNewFile(job, newFileList);
            SyncPCToUSBModifiedFile(job, modifiedFileList);
        }
        
        public static void SyncPCToUSBModifiedFile(PCJob job, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(job.PCPath != null && job.USBPath != null);
                ReadAndWrite.CopyFile(job.PCPath + modifiedFile.Path + modifiedFile.Name, job.USBPath + "\\modified" + i + ".temp");
                i++;
            }
        }

        public static void SyncPCToUSBNewFile(PCJob job, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(job.PCPath + newFile.Path + newFile.Name, job.USBPath + "\\new" + i + ".temp");
                i++;
            }
        }

        public static void SyncPCToUSBNewFolder(PCJob job, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach(FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Path != null & newFolder.Name != null);
                ReadAndWrite.CopyFolder(job.PCPath + newFolder.Path + newFolder.Name, job.USBPath + "\\new" + i);
                i++;
            }
        }


        public static void SyncUSBToPC(Differences USBToPC, PCJob job)
        {
            Debug.Assert(USBToPC != null);
            Debug.Assert(job != null);
            Debug.Assert(job.PCPath != null && job.USBPath != null);
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
            
            SyncUSBToPCNewFolder(job, newFolderList);
            SyncUSBtoPCDeleteFolder(job, deletedFolderList);
            SyncUSbToPCNewFile(job, newFileList);
            SyncUSBToPCModifiedFile(job, modifiedFileList);
            SyncUSBToPCDeleteFile(job, deletedFileList);
        }

        public static void SyncUSBToPCDeleteFile(PCJob job, List<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {
                Debug.Assert(deletedFile != null);
                Debug.Assert(deletedFile.Name != null);
                Debug.Assert(deletedFile.Path != null);
                ReadAndWrite.DeleteFile(job.PCPath + deletedFile.Path + deletedFile.Name);
            }
        }

        public static void SyncUSBToPCModifiedFile(PCJob job, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(modifiedFile.Name != null && modifiedFile.Path != null);
                ReadAndWrite.CopyFile(job.USBPath + "\\modified" + i + ".temp", job.PCPath + modifiedFile.Path + modifiedFile.Name);
                i++;
            }
        }
        
        public static void SyncUSbToPCNewFile(PCJob job, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                Debug.Assert(newFile != null);
                Debug.Assert(newFile.Path != null && newFile.Name != null);
                ReadAndWrite.CopyFile(job.USBPath + "\\new" + i + ".temp", job.PCPath + newFile.Path + newFile.Name);
                i++;
            }
        }

        public static void SyncUSBtoPCDeleteFolder(PCJob job, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                Debug.Assert(deletedFolder != null);
                Debug.Assert(deletedFolder.Name != null && deletedFolder.Path != null);
                ReadAndWrite.DeleteFolder(job.PCPath + deletedFolder.Path + deletedFolder.Name);
            }
        }

        public static void SyncUSBToPCNewFolder(PCJob job, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Name != null && newFolder.Path != null);
                ReadAndWrite.CopyFolder(job.USBPath + "\\new" + i, job.PCPath + newFolder.Path + newFolder.Name);
                i++;
            }
        }
    }
}
