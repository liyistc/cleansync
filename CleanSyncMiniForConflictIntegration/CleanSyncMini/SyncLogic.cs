using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using CleanSyncMini;

namespace DirectoryInformation
{
    public static class SyncLogic
    {
        public static void CleanSync(ComparisonResult comparisonResult, PCJob pcJob)
        {
            Debug.Assert(comparisonResult != null);
            Debug.Assert(pcJob != null);
            Debug.Assert(pcJob.PCPath != null && pcJob.USBPath != null);
            Differences USBToPC = comparisonResult.USBDifferences;
            Differences PCToUSB = comparisonResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);
            
            SyncUSBToPC(USBToPC,pcJob);
            SyncPCToUSB(PCToUSB, pcJob);
            pcJob.GetUsbJob().diff = PCToUSB;
            ReadAndWrite.ExportUSBJob(pcJob.GetUsbJob());
            pcJob.FolderInfo = ReadAndWrite.BuildTree(pcJob.PCPath);
            ReadAndWrite.ExportPCJob(pcJob);
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
            foreach(FolderMeta newFolder in newFolderList)
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
