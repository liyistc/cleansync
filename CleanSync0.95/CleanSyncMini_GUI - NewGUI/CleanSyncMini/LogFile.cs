using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanSyncMini
{
    public static class LogFile
    {
        static string LogFilePath = Directory.GetCurrentDirectory()+@"\CleanSyncLog.txt";

        public static void FileDeletion(string path)
        {
            File.AppendAllText(LogFilePath, "Delete File " + path + "\n");
        }

        public static void FolderDeletion(string path)
        {
            File.AppendAllText(LogFilePath, "Delete Folder " + path + "\n");
        }

        public static void FileCreation(string path)
        {
            File.AppendAllText(LogFilePath, "Create File " + path + "\n");
        }

        public static void FolderCreation(string path)
        {
            File.AppendAllText(LogFilePath, "Create Folder " + path + "\n");
        }

        public static void FileCopy(string source, string destination)
        {
            File.AppendAllText(LogFilePath, "Copy File " + source + " to " + destination + "\n");
        }

        public static void FolderCopy(string source, string destination)
        {
            File.AppendAllText(LogFilePath, "Copy Folder " + source + " to " + destination + "\n");
        }

        public static void NewLine()
        {
            File.AppendAllText(LogFilePath, "\n");
        }

        public static void ExportToPC(string path)
        {
            File.AppendAllText(LogFilePath, "Export PC Job to File " + Path.GetFileName(path)+"\n");
        }

        public static void ExportToUSB(string path)
        {
            File.AppendAllText(LogFilePath, "Export USB Job to File "  + Path.GetFileName(path)+"\n");
        }
        public static void ReportSolvedConflicts(string type, string PCpath, string AbsoluteUSBPath, string userChoicePath)
        {
            File.AppendAllText(LogFilePath, type + " " + PCpath + " conflicts with " + AbsoluteUSBPath + ". " + userChoicePath + " is updated" + "\n");
        }
    }
}
