using System;
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
        public static void cleanSync(ComparsionResult comparsionResult, Job job)
        {
            Debug.Assert(comparsionResult != null);
            Debug.Assert(job != null);
            Differences USBToPC = comparsionResult.USBDifferences;
            Differences PCToUSB = comparsionResult.PCDifferences;
            Debug.Assert(USBToPC != null);
            Debug.Assert(PCToUSB != null);

            SyncUSBToPC(USBToPC,job);
            SyncPCToUSB(PCToUSB, job);
        }

        private static void SyncPCToUSB(Differences PCToUSB, Job job)
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

        private static void SyncPCToUSBModifiedFile(Job job, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                Debug.Assert(modifiedFile != null);
                Debug.Assert(job.pathPC != null && job.pathUSB != null);
                ReadAndWrite.CopyFile(job.pathPC + modifiedFile.Path + modifiedFile.Name, job.pathUSB + "modified" + i + ".temp");
                i++;
            }
        }

        private static void SyncPCToUSBNewFile(Job job, List<FileMeta> newFileList)
        {

            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                ReadAndWrite.CopyFile(job.pathPC + newFile.Path + newFile.Name, job.pathUSB + "new" + i + ".temp");
                i++;
            }
        }

        private static void SyncPCToUSBNewFolder(Job job, List<FolderMeta> newFolderList)
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
            List<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            List<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            List<FileMeta> newFileList = USBToPC.getNewFileList();
            List<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            List<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();
            SyncUSBToPCNewFolder(job, newFolderList);
            SyncUSBtoPCDeleteFolder(job, deletedFolderList);
            SyncUSbToPCNewFile(job, newFileList);
            SyncUSBToPCModifiedFile(job, modifiedFileList);
            SyncUSBToPCDeleteFile(job, deletedFileList);
        }

        private static void SyncUSBToPCDeleteFile(Job job, List<FileMeta> deletedFileList)
        {
            foreach (FileMeta deletedFile in deletedFileList)
            {
                ReadAndWrite.DeleteFile(job.pathPC + deletedFile.Path + deletedFile.Name);
            }
        }

        private static void SyncUSBToPCModifiedFile(Job job, List<FileMeta> modifiedFileList)
        {
            int i = 0;
            foreach (FileMeta modifiedFile in modifiedFileList)
            {
                ReadAndWrite.CopyFile(job.pathUSB + "modified" + i + ".temp", job.pathPC + modifiedFile.Path + modifiedFile.Name);
                i++;
            }
        }
        
        private static void SyncUSbToPCNewFile(Job job, List<FileMeta> newFileList)
        {
            int i = 0;
            foreach (FileMeta newFile in newFileList)
            {
                ReadAndWrite.CopyFile(job.pathUSB + "new" + i + ".temp", job.pathPC + newFile.Path + newFile.Name);
                i++;
            }
        }

        private static void SyncUSBtoPCDeleteFolder(Job job, List<FolderMeta> deleteFolderList)
        {
            foreach (FolderMeta deletedFolder in deleteFolderList)
            {
                ReadAndWrite.DeleteFolder(job.pathPC + deletedFolder.Path + deletedFolder.Name);
            }
        }

        private static void SyncUSBToPCNewFolder(Job job, List<FolderMeta> newFolderList)
        {
            int i = 0;
            foreach (FolderMeta newFolder in newFolderList)
            {
                ReadAndWrite.CopyFolder(job.pathUSB + "new" + i, job.pathPC + newFolder.Path + newFolder.Name);
                i++;
            }
        }
        /*
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
         * */
    }
}
