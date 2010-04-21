/***********************************************************************
 * 
 * ******************CleanSync Version 2.0 Differences******************
 * 
 * Written By : Yu Qiqi
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
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
        #region Variables
        private List<FolderMeta> deletedFolderDifference = new List<FolderMeta>();
        private List<FolderMeta> newFolderDifference = new List<FolderMeta>();
        private List<FileMeta> deletedFileDifference = new List<FileMeta>();
        private List<FileMeta> newFileDifference = new List<FileMeta>();
        private List<FileMeta> modifiedFileDifference = new List<FileMeta>();
        #endregion


        #region Constructors
        public Differences()
        {
        }
        public Differences(Differences differences)
        {
            //Constructor for cloning a same copy of Differences
            DuplicateFolderList(this.deletedFolderDifference, differences.getDeletedFolderList());
            DuplicateFolderList(this.newFolderDifference, differences.getNewFolderList());
            DuplicateFileList(this.newFileDifference, differences.getNewFileList());
            DuplicateFileList(this.modifiedFileDifference, differences.getModifiedFileList());
            DuplicateFileList(this.deletedFileDifference, differences.getDeletedFileList());
        }
        #endregion


        #region Label File and Folder Type Method
        private void LableSubFoldersAndFiles(FolderMeta rootFolder, ComponentMeta.Type type)
        {
            List<FileMeta> fileList = rootFolder.files;
            List<FolderMeta> folderList = rootFolder.folders;
            foreach (FileMeta file in fileList)
            {
                if (file == null) continue;
                file.FileType = type;
            }
            foreach (FolderMeta folder in folderList)
            {
                //recursively label all the subFolders
                if (folder == null) continue;
                folder.FolderType = type;
                LableSubFoldersAndFiles(folder, type);
            }
        }
        #endregion


        #region Add Entry to the Differences Object Methods

        /// <summary>
        /// add deleted folder to deletedFolderList
        /// </summary>
        /// <param name="deletedFolder">the deleted folder to be added</param>
        public void AddDeletedFolderDifference(FolderMeta deletedFolder)
        {
            //label the Folder's type when add it into the list
            deletedFolder.FolderType = FolderMeta.Type.Deleted;
            LableSubFoldersAndFiles(deletedFolder, FolderMeta.Type.Deleted);
            deletedFolderDifference.Add(deletedFolder);
        }

        /// <summary>
        /// add new folder to newFolderList
        /// </summary>
        /// <param name="newFolder">the new folder to be added</param>
        public void AddNewFolderDifference(FolderMeta newFolder)
        {
            //label the Folder's type when add it into the list
            newFolder.FolderType = FolderMeta.Type.New;
            LableSubFoldersAndFiles(newFolder, ComponentMeta.Type.New);
            newFolderDifference.Add(newFolder);
        }

        /// <summary>
        /// add new file to newFileList
        /// </summary>
        /// <param name="newFile">the new file to be added</param>
        public void AddNewFileDifference(FileMeta newFile)
        {
            //label the File's type when add it into the list
            newFile.FileType = FileMeta.Type.New;
            newFileDifference.Add(newFile);
        }

        /// <summary>
        /// add deleted file to the deletedFileList
        /// </summary>
        /// <param name="deletedFile">the deleted file to be added</param>
        public void AddDeletedFileDifference(FileMeta deletedFile)
        {
            //label the File's type when add it into the list
            deletedFile.FileType = FileMeta.Type.Deleted;
            deletedFileDifference.Add(deletedFile);
        }

        /// <summary>
        /// add modified file to modifiedFileList
        /// </summary>
        /// <param name="modifiedFile">the modified file to be added</param>
        public void AddModifiedFileDifference(FileMeta modifiedFile)
        {
            modifiedFile.FileType = FileMeta.Type.Modified;
            modifiedFileDifference.Add(modifiedFile);
        }
        #endregion


        #region Retrieve File and Folder List Methods

        /// <summary>
        /// retrieve newFolderList
        /// </summary>
        /// <returns>newFolderList</returns>
        public List<FolderMeta> getNewFolderList()
        {
            return this.newFolderDifference;
        }

        /// <summary>
        /// retrieve deletedFolderList
        /// </summary>
        /// <returns>deletedFolderList</returns>
        public List<FolderMeta> getDeletedFolderList()
        {
            return this.deletedFolderDifference;
        }

        /// <summary>
        /// retrieve deletedFileList
        /// </summary>
        /// <returns>deletedFileList</returns>
        public List<FileMeta> getDeletedFileList()
        {
            return this.deletedFileDifference;
        }

        /// <summary>
        /// retrieve newFileList
        /// </summary>
        /// <returns>newFileList</returns>
        public List<FileMeta> getNewFileList()
        {
            return this.newFileDifference;
        }

        /// <summary>
        /// retrieve modifiedFileList
        /// </summary>
        /// <returns>modifiedFileList</returns>
        public List<FileMeta> getModifiedFileList()
        {
            return this.modifiedFileDifference;
        }
        #endregion


        #region Remove Entry From the Differences Object Methods

        /// <summary>
        /// remove deleted folder from deletedfolderList
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>bool</returns>
        public bool removeFolderFromDeletedFolderList(FolderMeta folder)
        {
            return RemoveFolder(folder, deletedFolderDifference);
        }

        /// <summary>
        /// remove new folder from newFolderList
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>bool</returns>
        public bool removeFolderFromNewFolderList(FolderMeta folder)
        {
            return RemoveFolder(folder, newFolderDifference);
        }

        /// <summary>
        /// remove new file from newFileList
        /// </summary>
        /// <param name="file"></param>
        /// <returns>bool</returns>
        public bool removeFileFromNewFileList(FileMeta file)
        {
            return RemoveFile(file, newFileDifference);
        }

        /// <summary>
        /// remove modified file from modifiedList
        /// </summary>
        /// <param name="file"></param>
        /// <returns>bool</returns>
        public bool removeFileFromModifiedFileList(FileMeta file)
        {
            return RemoveFile(file, modifiedFileDifference);
        }

        /// <summary>
        /// remove deleted file from deletedFileList
        /// </summary>
        /// <param name="file"></param>
        /// <returns>bool</returns>
        public bool removeFileFromDeletedFileList(FileMeta file)
        {
            return RemoveFile(file, deletedFileDifference);
        }

        private bool RemoveFile(FileMeta fileToBeRemoved, List<FileMeta> fileList)
        {
            bool deleted = false;
            string fileToBeRemovedInfo = fileToBeRemoved.Path + fileToBeRemoved.Name;
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList[i];
                if (file == null)
                    continue;
                if (fileToBeRemovedInfo.ToLower().Equals((file.Path + file.Name).ToLower()))
                {
                    fileList[i] = null;
                    deleted = true;
                    break;
                }
            }
            return deleted;
        }
        private bool RemoveFolder(FolderMeta folderToBeRemoved, List<FolderMeta> folderList)
        {
            bool deleted = false;
            string folderToBeRemovedInfo = folderToBeRemoved.Path + folderToBeRemoved.Name;
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta folder = folderList[i];
                if (folderToBeRemovedInfo.ToLower().Equals((folder.Path + folder.Name).ToLower()))
                {
                    folderList[i] = null;
                    deleted = true;
                    break;
                }
            }
            return deleted;
        }
        #endregion


        #region Space Prediction based on Differences Object
        /// <summary>
        /// get the required space of a given differences. 
        /// </summary>
        /// <returns>total space required represented as long</returns>
        public long GetRequireSpace()
        {
            long totalSize = 0;
            //calculate the required space for newFiles, modifiedFiles, newFolders
            totalSize += GetFileSize(this.getNewFileList());
            totalSize += GetFileSize(this.getModifiedFileList());
            totalSize += GetFolderSize(this.getNewFolderList());
            return totalSize;
        }

        private long GetFolderSize(List<FolderMeta> folders)
        {
            //sum up all the subFile sizes and subfolder sizes
            long size = 0;
            if (folders != null)
            {
                foreach (FolderMeta folder in folders)
                {
                    if (folder != null)
                    {
                        if (folder.files != null)
                            size += GetFileSize(folder.files);
                        if (folder.folders != null)
                            size += GetFolderSize(folder.folders);
                    }
                }
            }
            return size;
        }

        private long GetFileSize(List<FileMeta> files)
        {
            //sum up all the subFile sizes
            long size = 0;
            if (files != null)
            {
                foreach (FileMeta file in files)
                {
                    if (file != null)
                        size += file.Size;
                }
            }
            return size;
        }

        #endregion


        #region Duplicate List Methods
        private void DuplicateFolderList(List<FolderMeta> newList, List<FolderMeta> baseList)
        {
            foreach (FolderMeta folder in baseList)
            {
                if (folder != null)
                    newList.Add(new FolderMeta(folder));
                else
                    newList.Add(null);
            }
        }
        private void DuplicateFileList(List<FileMeta> newList, List<FileMeta> baseList)
        {
            foreach (FileMeta file in baseList)
            {
                if (file != null)
                    newList.Add(new FileMeta(file));
                else
                    newList.Add(null);
            }
        }
        #endregion

    }
}
