﻿using System;
using System.IO;
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
            Debug.Assert(comparisonResult != null);
            Debug.Assert(job != null);
            Debug.Assert(job.pathPC != null && job.pathUSB != null);
            Differences USBToPC = comparisonResult.USBDifferences;
            Differences PCToUSB = comparisonResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);

            SyncUSBToPC(USBToPC,job);
            SyncPCToUSB(PCToUSB, job);
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
                Debug.Assert(job.pathPC != null && job.pathUSB != null);
                ReadAndWrite.CopyFile(job.pathPC + modifiedFile.Path + modifiedFile.Name, job.pathUSB + "\\modified" + i + ".temp");
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
                ReadAndWrite.CopyFile(job.pathPC + newFile.Path + newFile.Name, job.pathUSB + "\\new" + i + ".temp");
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
                ReadAndWrite.CopyFolder(job.pathPC + newFolder.Path + newFolder.Name, job.pathUSB + "\\new" + i);
                i++;
            }
        }


        public static void SyncUSBToPC(Differences USBToPC, PCJob job)
        {
            Debug.Assert(USBToPC != null);
            Debug.Assert(job != null);
            Debug.Assert(job.pathPC != null && job.pathUSB != null);
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
                ReadAndWrite.DeleteFile(job.pathPC + deletedFile.Path + deletedFile.Name);
            }
        }

        public static void SyncUSBToPCModifiedFile(PCJob job, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(modifiedFile.Name != null && modifiedFile.Path != null);
                ReadAndWrite.CopyFile(job.pathUSB + "\\modified" + i + ".temp", job.pathPC + modifiedFile.Path + modifiedFile.Name);
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
                ReadAndWrite.CopyFile(job.pathUSB + "\\new" + i + ".temp", job.pathPC + newFile.Path + newFile.Name);
                i++;
            }
        }

        public static void SyncUSBtoPCDeleteFolder(PCJob job, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                Debug.Assert(deletedFolder != null);
                Debug.Assert(deletedFolder.Name != null && deletedFolder.Path != null);
                ReadAndWrite.DeleteFolder(job.pathPC + deletedFolder.Path + deletedFolder.Name);
            }
        }

        public static void SyncUSBToPCNewFolder(PCJob job, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                Debug.Assert(newFolder != null);
                Debug.Assert(newFolder.Name != null && newFolder.Path != null);
                ReadAndWrite.CopyFolder(job.pathUSB + "\\new" + i, job.pathPC + newFolder.Path + newFolder.Name);
                i++;
            }
        }
    }
}
