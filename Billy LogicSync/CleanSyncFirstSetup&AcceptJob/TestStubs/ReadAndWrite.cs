using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace TestStubs
{
    public static class ReadAndWrite
    {
        public static List<string> paths = new List<string>();
        public static void CopyFile(string sourcePath, string targetPath)
        {
            paths.Add("Copy File " + sourcePath + " " + targetPath);
        }

        public static void CopyFolder(string sourcePath, string targetPath)
        {
            paths.Add("Copy Folder " + sourcePath + " " + targetPath);
        }

        public static void DeleteFile(string Path)
        {
            paths.Add("Delete File " + Path );
        }

        public static void DeleteFolder(string sourcePath)
        {
            paths.Add("Delete Folder " + sourcePath);
        }


        public static void ExportPCJob(PCJob job)
        {
            throw new NotImplementedException();
        }

        public static void ExportUSBJob(object p)
        {
            throw new NotImplementedException();
        }

        public static FolderMeta BuildTree(string p)
        {
            throw new NotImplementedException();
        }
    }

    public class ComparisonResult
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

        public ComparisonResult(Differences USBDifferences, Differences PCDifferences)
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

    public class PCJob
    {
        public int PCID
        {
            get;
            set;
        }

        public string PCPath
        {
            get;
            set;
        }
        public string USBPath
        {
            get;
            set;
        }

        public PCJob(string pathPc, string USBPath)
        {
            this.PCPath = pathPc;
            this.USBPath = USBPath;
        }

        public USBJob GetUsbJob()
        {
            throw new NotImplementedException();
        }

        public FolderMeta FolderInfo
        {
            get;
            set;
        }

    }

    public class USBJob
    {
        public int MostRecentPCID { get; set; }
        public Differences diff { get; set; }
    }

    public class Differences
    {

        public List<FolderMeta> deletedFolderDifference = new List<FolderMeta>();
        public List<FolderMeta> newFolderDifference = new List<FolderMeta>();
        public List<FileMeta> deletedFileDifference = new List<FileMeta>();
        public List<FileMeta> newFileDifference = new List<FileMeta>();
        public List<FileMeta> modifiedFileDifference = new List<FileMeta>();

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
