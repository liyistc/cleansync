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
    }
}
