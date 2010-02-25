using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;
using DirectoryInformation;

namespace CleanSyncMini
{
    public static class SyncLogic
    {
        


        internal static void SyncPCtoUSB(PCJob pcJob)
        {
            IEnumerator<FileMeta> files = pcJob.FolderInfo.GetFiles();
            IEnumerator<FolderMeta> folders = pcJob.FolderInfo.GetFolders();

            Directory.CreateDirectory(pcJob.USBPath);
            while (files.MoveNext())
            {
                ReadAndWrite.CopyFile(files.Current.Path, pcJob.USBPath + "\\" + files.Current.Name);
            }

            while (folders.MoveNext())
            {
                ReadAndWrite.CopyFolder(folders.Current.Path, pcJob.USBPath + "\\" + folders.Current.Name);
            }
        }
    }
}
