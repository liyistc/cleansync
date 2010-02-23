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
       /* public static void cleanSync(ComparsionResult comparsionResult, Job job)
        {
            Differences USBToPC = comparsionResult.USBDifferences;
            Differences PCT0USB = comparsionResult.PCDifferences;

            syncUSBToPC(USBToPC,job);
            syncPCToUSB(PCTOUSB, job);
        }


        private static void syncUSBToPC(Differences USBToPC, Job job)
        {
            LinkedList<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            LinkedList<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            LinkedList<FileMeta> newFileList = USBToPC.getNewFileList();
            LinkedList<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            LinkedList<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();

            foreach( FolderMeta newFolder in newFolderList)
            {
                ReadAndWrite.copyFolder(job.pathUSB + newFolder.Path + newFolder.Name, job.PCPath + newFolder.Path);
            }
            foreach (FolderMeta deletedFolder in newFolderList)
            {
                ReadAndWrite.DeleteFile(job.pathPC + deletedFolder.Path + deletedFolder.Name);
            }
            foreach (FileMeta newFile in newFolderList)
            {
                ReadAndWrite.copyFile(job.pathUSB + newFile.Path + newFile.Name, job.PCPath + newFile.Path);
            }
            foreach (FileMeta modifiedFile in newFolderList)
            {
                ReadAndWrite.copyFile(job.pathUSB + modifiedFile.Path + modifiedFile.Name, job.PCPath + modifiedFile.Path);
            }
            foreach (FileMeta deletedFile in newFolderList)
            {
                ReadAndWrite.deleteFile(job.USBPath + deletedFile.Path + deletedFile.Name, job.PCPath + deletedFile.Path);
            }
        }*/
        

        internal static void SyncPCtoUSB(Job job)
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
