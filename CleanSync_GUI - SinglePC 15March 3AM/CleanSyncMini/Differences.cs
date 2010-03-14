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
        public void removeFolderFromDeletedFolderList(FolderMeta folder)
        {
            this.deletedFolderDifference.Remove(folder);
        }
        public void removeFolderFromNewFolderList(FolderMeta folder)
        {
            this.newFolderDifference.Remove(folder);
        }
        public void removeFileFromNewFileList(FileMeta file)
        {
            this.newFileDifference.Remove(file);
        }
        public void removeFileFromModifiedFileList(FileMeta file)
        {
            this.modifiedFileDifference.Remove(file);
        }
        public void removeFileFromDeletedFileList(FileMeta file)
        {
            this.deletedFileDifference.Remove(file);
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

    }
}
