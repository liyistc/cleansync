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
        private LinkedList<FolderMeta> deletedFolderDifference =new LinkedList<FolderMeta>();
        private LinkedList<FolderMeta> newFolderDifference = new LinkedList<FolderMeta>();
        private LinkedList<FileMeta> deletedFileDifference = new LinkedList<FileMeta>();
        private LinkedList<FileMeta> newFileDifference = new LinkedList<FileMeta>();
        private LinkedList<FileMeta> modifiedFileDifference = new LinkedList<FileMeta>();

        public void AddDeletedFolderDifference(FolderMeta deletedFolder) 
        {
            deletedFolderDifference.AddLast(deletedFolder);
        }

        public void AddNewFolderDifference(FolderMeta newFolder) 
        {
            newFolderDifference.AddLast(newFolder);
        }
        public void AddNewFileDifference(FileMeta newFile)
        {
            newFileDifference.AddLast(newFile);
        }
        public void AddDeletedFileDifference(FileMeta newFile)
        {
           deletedFileDifference.AddLast(newFile);
        }
        public void AddModifiedFileDifference(FileMeta newFile)
        {
            modifiedFileDifference.AddLast(newFile);
        }

      
        
        public LinkedList<FolderMeta> getNewFolderList()
        {
            return this.newFolderDifference;
        }
        public LinkedList<FolderMeta> getDeletedFolderList()
        {
            return this.deletedFolderDifference;
        }
        public LinkedList<FileMeta> getDeletedFileList()
        {
            return this.deletedFileDifference;
        }
        public LinkedList<FileMeta> getNewFileList()
        {
            return this.newFileDifference;
        }
        public LinkedList<FileMeta> getModifiedFileList()
        {
            return this.modifiedFileDifference;
        }
    }
}
