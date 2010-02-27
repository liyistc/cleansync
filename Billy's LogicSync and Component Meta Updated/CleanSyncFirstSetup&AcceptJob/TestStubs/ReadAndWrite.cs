using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestStubs
{
    public static class ReadAndWrite
    {
        static SyncLogicUnitTestResults results = new SyncLogicUnitTestResults();

        public static void CopyFile(string sourcePath, string targetPath)
        {
          
        }

        public static void CopyFolder(string sourcePath, string targetPath)
        {
        }

        public static void DeleteFile(string Path)
        {
        }

        public static void DeleteFolder(string sourcePath)
        {
        }

    }

    public class ComparsionResult
    {

        public Differences USBDifferences
        {
            get;
            set;
        }
        public Differences PCDifferences
        {
            get;
            set;
        }

        public ComparsionResult(Differences USBDifferences, Differences PCDifferences)
        {
            this.USBDifferences = USBDifferences;
            this.PCDifferences = PCDifferences;
        }
    }

    public class FolderMeta
    {
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public FolderMeta(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
    public class FileMeta
    {
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public FileMeta(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    public class Job
    {
        public string pathUSB
        {
            get;
            set;
        }
        public string pathPC
        {
            get;
            set;
        }

        public Job(string pathPc, string pathUSB)
        {
            this.pathPC = pathPc;
            this.pathUSB = pathUSB;
        }
    }

    public class Differences
    {

        private List<FolderMeta> deletedFolderDifference = new List<FolderMeta>();
        private List<FolderMeta> newFolderDifference = new List<FolderMeta>();
        private List<FileMeta> deletedFileDifference = new List<FileMeta>();
        private List<FileMeta> newFileDifference = new List<FileMeta>();
        private List<FileMeta> modifiedFileDifference = new List<FileMeta>();

        public void AddDeletedFolderDifference(FolderMeta deletedFolder)
        {
            deletedFolderDifference.Add(deletedFolder);
        }

        public void AddNewFolderDifference(FolderMeta newFolder)
        {
            newFolderDifference.Add(newFolder);
        }
        public void AddNewFileDifference(FileMeta newFile)
        {
            newFileDifference.Add(newFile);
        }
        public void AddDeletedFileDifference(FileMeta newFile)
        {
            deletedFileDifference.Add(newFile);
        }
        public void AddModifiedFileDifference(FileMeta newFile)
        {
            modifiedFileDifference.Add(newFile);
        }



        public List<FolderMeta> getNewFolderList()
        {
            return this.newFolderDifference;
        }
        public List<FolderMeta> getDeletedFolderList()
        {
            return this.deletedFolderDifference;
        }
        public List<FileMeta> getDeletedFileList()
        {
            return this.deletedFileDifference;
        }
        public List<FileMeta> getNewFileList()
        {
            return this.newFileDifference;
        }
        public List<FileMeta> getModifiedFileList()
        {
            return this.modifiedFileDifference;
        }
    }
}
