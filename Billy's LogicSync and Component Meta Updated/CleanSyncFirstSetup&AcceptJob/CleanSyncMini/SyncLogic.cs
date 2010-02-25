using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMinimalVersion;
using CleanSync;

namespace DirectoryInformation
{
    public static class SyncLogic
    {
        public static void cleanSync(ComparsionResult comparsionResult, Job job)
        {
            Differences USBToPC = comparsionResult.USBDifferences;
            Differences PCToUSB = comparsionResult.PCDifferences;

            SyncUSBToPC(USBToPC,job);
            SyncPCToUSB(PCToUSB, job);
        }

        private static void SyncPCToUSB(Differences PCToUSB, Job job)
        {
            LinkedList<FolderMeta> newFolderList = PCToUSB.getNewFolderList();
            LinkedList<FileMeta> newFileList = PCToUSB.getNewFileList();
            LinkedList<FileMeta> modifiedFileList = PCToUSB.getModifiedFileList();

            SyncPCToUSBNewFolder(job, newFolderList);
            SyncPCToUSBNewFile(job, newFileList);

            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                ReadAndWrite.CopyFile(job.pathPC + modifiedFile.Path + modifiedFile.Name, job.pathUSB + "modified" + i + ".temp");
                i++;
            }
        }

        private static void SyncPCToUSBNewFile(Job job, LinkedList<FileMeta> newFileList)
        {

            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                ReadAndWrite.CopyFile(job.pathPC + newFile.Path + newFile.Name, job.pathUSB + "new" + i + ".temp");
                i++;
            }
        }

        private static void SyncPCToUSBNewFolder(Job job, LinkedList<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach(FolderMeta newFolder in newFolderList)
            {
                ReadAndWrite.CopyFolder(job.pathPC + newFolder.Path + newFolder.Name, job.pathUSB + "new" + i);
                i++;
            }
        }


        private static void SyncUSBToPC(Differences USBToPC, Job job)
        {
            LinkedList<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            LinkedList<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            LinkedList<FileMeta> newFileList = USBToPC.getNewFileList();
            LinkedList<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            LinkedList<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();
            SyncUSBToPCNewFolder(job, newFolderList);
            SyncUSBtoPCDeleteFolder(job, deletedFolderList);
            SyncUSbToPCNewFile(job, newFileList);
            SyncUSBToPCModifiedFile(job, modifiedFileList);
            SyncUSBToPCDeleteFile(job, deletedFileList);
        }

        private static void SyncUSBToPCDeleteFile(Job job, LinkedList<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {
                ReadAndWrite.DeleteFile(job.pathPC + deletedFile.Path + deletedFile.Name);
            }
        }

        private static void SyncUSBToPCModifiedFile(Job job, LinkedList<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                ReadAndWrite.CopyFile(job.pathUSB + "modified" + i + ".temp", job.pathPC + modifiedFile.Path + modifiedFile.Name);
                i++;
            }
        }
        
        private static void SyncUSbToPCNewFile(Job job, LinkedList<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                ReadAndWrite.CopyFile(job.pathUSB + "new" + i + ".temp", job.pathPC + newFile.Path + newFile.Name);
                i++;
            }
        }

        private static void SyncUSBtoPCDeleteFolder(Job job, LinkedList<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                ReadAndWrite.DeleteFolder(job.pathPC + deletedFolder.Path + deletedFolder.Name);
            }
        }

        private static void SyncUSBToPCNewFolder(Job job, LinkedList<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                ReadAndWrite.CopyFolder(job.pathUSB + "new" + i, job.pathPC + newFolder.Path + newFolder.Name);
                i++;
            }
        }

        internal static void SyncPCToUSB(Job job)
        {
            IEnumerator<FileMeta> files = job.FM.GetFiles();
            IEnumerator<FolderMeta> folders = job.FM.GetFolders();

            Directory.CreateDirectory(job.pathUSB);
            while (files.MoveNext())
            {
                ReadAndWrite.CopyFile(files.Current.Path, job.pathUSB + "\\" + files.Current.Name);
            }

            while (folders.MoveNext())
            {
                ReadAndWrite.CopyFolder(folders.Current.Path, job.pathUSB + "\\" + folders.Current.Name);
            }
        }       
    }
}
