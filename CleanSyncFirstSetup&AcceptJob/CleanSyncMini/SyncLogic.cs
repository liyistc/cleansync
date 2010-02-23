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
        /*public static void cleanSync(ComparsionResult comparsionResult, Job job)
        {
            Differences USBToPC = comparsionResult.USBDifferences;
            Differences PCT0USB = comparsionResult.PCDifferences;

            //syncUSBToPC(USBToPC,job);
            //syncPCToUSB(PCTOUSB, job);
        }*/


        /*private void syncUSBToPC(Differences USBToPC, Job job)
        {
            LinkedList<FolderMeta> newFolderList = USBToPC.getNewFolderList();
            LinkedList<FolderMeta> deletedFolderList = USBToPC.getDeletedFolderList();
            LinkedList<FileMeta> newFileList = USBToPC.getNewFileList();
            LinkedList<FileMeta> deletedFileList = USBToPC.getDeletedFileList();
            LinkedList<FileMeta> modifiedFileList = USBToPC.getModifiedFileList();

            foreach( FolderMeta newFolder in newFolderList)
            {
                //ReadAndWrite.copyFolder(newFolder.USBPath, job.PCPath);
            }
            foreach (FolderMeta deletedFolder in newFolderList)
            {
                //ReadAndWrite.DeleteFile(job.PCPath);
            }
            //foreach (FileMeta newFile in newFolderList)
            {
                //ReadAndWrite.copyFile(job.USBPath, job.PCPath);
            }
            //foreach (FileMeta modifiedFile in newFolderList)
            {
                //ReadAndWrite.copyFile(job.USBPath, job.PCPath);
            }
            //foreach (FileMeta deletedFile in newFolderList)
            {
                //ReadAndWrite.deleteFile(job.USBPath, job.PCPath);
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
