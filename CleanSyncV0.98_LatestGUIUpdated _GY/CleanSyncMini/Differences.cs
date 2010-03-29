using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace DirectoryInformation
{
    [Serializable]
    public class Differences
    {
        private List<FolderMeta> deletedFolderDifference =new List<FolderMeta>();
        private List<FolderMeta> newFolderDifference = new List<FolderMeta>();
        private List<FileMeta> deletedFileDifference = new List<FileMeta>();
        private List<FileMeta> newFileDifference = new List<FileMeta>();
        private List<FileMeta> modifiedFileDifference = new List<FileMeta>();

        public Differences()
        {
        }
        public Differences(Differences differences)
        {
            DuplicateFolderList(this.deletedFolderDifference, differences.getDeletedFolderList());
            DuplicateFolderList(this.newFolderDifference, differences.getNewFolderList());
            DuplicateFileList(this.newFileDifference, differences.getNewFileList());
            DuplicateFileList(this.modifiedFileDifference, differences.getModifiedFileList());
            DuplicateFileList(this.deletedFileDifference, differences.getDeletedFileList());
        }

        private void DuplicateFolderList(List<FolderMeta> newList, List<FolderMeta> baseList)
        {
            foreach (FolderMeta folder in baseList)
            {
                newList.Add(new FolderMeta(folder));
            }
        }
        private void DuplicateFileList(List<FileMeta> newList, List<FileMeta> baseList)
        {
            foreach (FileMeta file in baseList)
            {
                newList.Add(new FileMeta(file));
            }
        }
        private void LableSubFoldersAndFiles(FolderMeta rootFolder, ComponentMeta.Type type)
        {
            List<FileMeta> fileList = rootFolder.files;
            List<FolderMeta> folderList = rootFolder.folders;
            foreach (FileMeta file in fileList)
            {
                file.FileType = type;
            }
            foreach (FolderMeta folder in folderList)
            {
                folder.FolderType = type;
                LableSubFoldersAndFiles(folder, type);
            }
        }
        public void AddDeletedFolderDifference(FolderMeta deletedFolder)
        {
            deletedFolder.FolderType = FolderMeta.Type.Deleted;
            LableSubFoldersAndFiles(deletedFolder, FolderMeta.Type.Deleted);
            deletedFolderDifference.Add(deletedFolder);
        }

        public void AddNewFolderDifference(FolderMeta newFolder)
        {
            newFolder.FolderType = FolderMeta.Type.New;
            LableSubFoldersAndFiles(newFolder, ComponentMeta.Type.New);
            newFolderDifference.Add(newFolder);
        }
        public void AddNewFileDifference(FileMeta newFile)
        {
            newFile.FileType = FileMeta.Type.New;
            newFileDifference.Add(newFile);
        }
        public void AddDeletedFileDifference(FileMeta newFile)
        {
            newFile.FileType = FileMeta.Type.Deleted;
            deletedFileDifference.Add(newFile);
        }
        public void AddModifiedFileDifference(FileMeta newFile)
        {
            newFile.FileType = FileMeta.Type.Modified;
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
        public bool removeFolderFromDeletedFolderList(FolderMeta folder)
        {
           return RemoveFolder(folder, deletedFolderDifference, "deletedFolderList");
        }
        public bool removeFolderFromNewFolderList(FolderMeta folder)
        {
            return RemoveFolder(folder, newFolderDifference ,"newFolderList");
        }
        public bool removeFileFromNewFileList(FileMeta file)
        {
            return RemoveFile(file, newFileDifference,"newFileList");
        }
        public bool removeFileFromModifiedFileList(FileMeta file)
        {
           return RemoveFile(file, modifiedFileDifference, "modifiedFileList");
        }
        public bool removeFileFromDeletedFileList(FileMeta file)
        {
           return RemoveFile(file, deletedFileDifference, "deletedFileList"); 
        }

        private bool RemoveFile(FileMeta fileToBeRemoved, List<FileMeta> fileList,string info)
        {
            bool deleted = false;
            string fileToBeRemovedInfo = fileToBeRemoved.Path + fileToBeRemoved.Name;
            foreach (FileMeta file in fileList)
            {
                if (fileToBeRemovedInfo.Equals(file.Path + file.Name))
                {
                    fileList.Remove(file);
                    deleted = true;
                    Console.WriteLine("DDDDDDDDdeletingFile: " +info+":  " + file.AbsolutePath);
                    break;
                }
            }
            return deleted;
        }
        private bool RemoveFolder(FolderMeta folderToBeRemoved, List<FolderMeta> folderList,string info)
        {
            bool deleted = false;
            string folderToBeRemovedInfo = folderToBeRemoved.Path + folderToBeRemoved.Name;
            foreach (FolderMeta folder in folderList)
            {
                if (folderToBeRemovedInfo.Equals(folder.Path + folder.Name))
                {
                    folderList.Remove(folder);
                    deleted = true;
                    Console.WriteLine("DDDDDDDDdeletingFolder: " +info+": " + folder.AbsolutePath);
                    break;
                }
            }
            return deleted;
        }

        public override string ToString()
        {
            string result = "";
            result +=   "New Folders:\n";
            foreach (FolderMeta newFolder in newFolderDifference)
                result += "  "+newFolder.Name+"\n";
            result += "Deleted Folders:\n";
            foreach (FolderMeta deletedFolder in deletedFolderDifference)
                result += "  "+deletedFolder.Name+"\n";
            result += "New Files:\n";
            foreach (FileMeta newFile in newFileDifference)
                result += "  "+newFile.Name+"\n";
            result += "Deleted Files:\n";
            foreach (FileMeta deletedFile in deletedFileDifference)
                result += "  "+deletedFile.Name + "\n";
            result += "Modified Files:\n";
            foreach (FileMeta modifiedFile in modifiedFileDifference)
                result += "  "+ modifiedFile.Name + "\n";
            return result;
        }

        internal long GetRequireSpace()
        {
            long totalSize = 0;

            totalSize += GetFileSize(this.getNewFileList());
            totalSize += GetFileSize(this.getModifiedFileList());
            totalSize += GetFolderSize(this.getNewFolderList());
            return totalSize;
        }

        private long GetFolderSize(List<FolderMeta> folders)
        {
            long size = 0;
            foreach (FolderMeta folder in folders)
            {
                if (folder.files != null)
                    size += GetFileSize(folder.files);
                if (folder.folders != null)
                    size += GetFolderSize(folder.folders);
            }
            return size;
        }

        private long GetFileSize(List<FileMeta> files)
        {
            long size = 0;
            foreach (FileMeta file in files)
            {
                size += file.Size;
            }
            return size;
        }

    }
}
