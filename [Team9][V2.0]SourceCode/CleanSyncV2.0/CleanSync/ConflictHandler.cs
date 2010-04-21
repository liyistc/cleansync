/***********************************************************************
 * 
 * ****************CleanSync Version 2.0 ConflictHandler****************
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
using System.Text;
using DirectoryInformation;
using System.Diagnostics;

namespace CleanSync
{
    
   public class ConflictHandler
    {

        #region Constructor
        public ConflictHandler()
        {
        }
        #endregion

        #region Enumerator
        public enum PCorUSBFlag
        {
            PC,
            USB
        }
        #endregion

        #region Conflict Handling Methods
       /// <summary>
       /// handle conflicts based on user's choices.
       /// </summary>
       /// <param name="comparisonResult">a ComparisonResult Object which contains the conflictsList, PCDifferences and USBDifferences</param>
       /// <returns>a modified ComparisonResult Object</returns>
        public ComparisonResult HandleConflicts(ComparisonResult comparisonResult)
        {
            Debug.Assert(comparisonResult != null);
            Differences PCDifferences = comparisonResult.PCDifferences;
            Differences USBDifferences = comparisonResult.USBDifferences;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            for (int i = 0; i < conflictList.Count; i++)
            {
                Conflicts conflict = conflictList.ElementAt(i);
                switch (conflict.FolderOrFileConflictType)
                {
                    case Conflicts.FolderFileType.FileConflict: HandleFileConflict(PCDifferences, USBDifferences, conflict, conflict.getUserChoice()); break;
                    case Conflicts.FolderFileType.FileVSFolderConflict: HandleFileVSFolderConflict(PCDifferences, USBDifferences, conflict, conflict.getUserChoice()); break;
                    case Conflicts.FolderFileType.FolderVSFileConflict: HandleFolderVSFileConflict(PCDifferences, USBDifferences, conflict, conflict.getUserChoice()); break;
                }
                conflictList.Remove(conflict);
                i--;
            }
            Debug.Assert(conflictList.Count == 0);
            return new ComparisonResult(USBDifferences, PCDifferences, conflictList);
        }
        private void HandleFileConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
        {
            switch (userChoice)
            {
                case Conflicts.UserChoice.KeepPCUpdates:
                    UpdateEntriesForFileConflict(PCDifferences, USBDifferences, conflict, PCorUSBFlag.PC);
                    break;
                case Conflicts.UserChoice.KeepUSBUpdates:
                    UpdateEntriesForFileConflict(PCDifferences, USBDifferences, conflict, PCorUSBFlag.USB);
                    break;
            }
        }
        private void HandleFileVSFolderConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
        {
            switch (userChoice)
            {
                case Conflicts.UserChoice.KeepUSBUpdates: PCDifferences.removeFileFromModifiedFileList(conflict.CurrentPCFile); break;
                case Conflicts.UserChoice.KeepPCUpdates:
                    //only ModifiedFile VS Deleted Folder Conflict exists, change the type of file to New.
                    DeleteFileFromRootFolder(conflict.CurrentPCFile, conflict.USBFolder, USBDifferences.getDeletedFolderList());
                    PCDifferences.removeFileFromModifiedFileList(conflict.CurrentPCFile);
                    PCDifferences.AddNewFileDifference(conflict.CurrentPCFile); break;

            }
        }
        private void HandleFolderVSFileConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
        {
            switch (userChoice)
            {
                case Conflicts.UserChoice.KeepPCUpdates: USBDifferences.removeFileFromModifiedFileList(conflict.USBFile); break;
                case Conflicts.UserChoice.KeepUSBUpdates:
                    //only ModifiedFile VS Deleted Folder Conflict exists, change the type of file to New.
                    DeleteFileFromRootFolder(conflict.USBFile, conflict.CurrentPCFolder, PCDifferences.getDeletedFolderList());
                    USBDifferences.removeFileFromModifiedFileList(conflict.USBFile);
                    USBDifferences.AddNewFileDifference(conflict.USBFile);
                    break;
            }
        }
        #endregion

        #region Remove File and Folder Methods
        private void UpdateEntriesForFileConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, PCorUSBFlag flag)
        {
            //this is a helper method for HandleFileConflict. 
            bool deleted = false;

            //initialize all the variables
            //if user choose to keep PCchanges, set 'differences'  to 'PCDifferences', 'pairedDifferences' tp 'USBDifferences'.
            //else set 'differences' to 'USBDifferences', 'pairedDifferences' to 'PCDifferences'
            //do the same for file, type. etc.
            Conflicts.ConflictType type = conflict.PCFolderFileType;
            Conflicts.ConflictType pairedType = conflict.USBFolderFileType;
            FileMeta file = conflict.CurrentPCFile;
            FileMeta pairedFile = conflict.USBFile;
            Differences differences = PCDifferences;
            Differences pairedDifferences = USBDifferences;
            if (flag == PCorUSBFlag.USB)
            {
                type = conflict.USBFolderFileType;
                pairedType = conflict.PCFolderFileType;
                file = conflict.USBFile;
                pairedFile = conflict.CurrentPCFile;
                differences = USBDifferences;
                pairedDifferences = PCDifferences;
            }

            if (type != Conflicts.ConflictType.New || pairedType != Conflicts.ConflictType.New)
            {
                //delete the file entry in pairedDifference. i.e. If PC changes is chosen, PCDifferences is kept while Entry in USBDifferences should be deleted
                deleted = DeleteDifferencesFileEntry(pairedType, pairedFile, pairedDifferences);
                if (type == Conflicts.ConflictType.Modified && pairedType == Conflicts.ConflictType.Deleted)
                {
                    //special case, set the modified file type to New, if the remote pc has deleted the file.
                    differences.removeFileFromModifiedFileList(file);
                    differences.AddNewFileDifference(file);
                }
            }
            else if (type == Conflicts.ConflictType.New && pairedType == Conflicts.ConflictType.New)
            {
                //update the entry
                bool pcDeleted = false;
                deleted = pairedDifferences.removeFileFromNewFileList(pairedFile);
                if (deleted)
                {
                    //change the new file type to modified if the remote pc also contains a same new file
                    pcDeleted = differences.removeFileFromNewFileList(file);
                    Debug.Assert(pcDeleted);
                    differences.AddModifiedFileDifference(file);
                }
            }
            if (!deleted)
            {
                //if not deleted, if means it is a NewFile VS NewFile conflict which is intially a NewFolder VS NewFolder conflict
                //delete the entry in that foldermeta
                FolderMeta pairedFolder = FindFileRootFolder(pairedFile, pairedDifferences.getNewFolderList());
                Debug.Assert(pairedFolder != null);
                DeleteFileFromRootFolder(pairedFile, pairedFolder);
                FolderMeta folder = FindFileRootFolder(file, differences.getNewFolderList());
                Debug.Assert(folder != null);
                DeleteFileFromRootFolder(file, folder);
                //remove the folder if it is empty
                RemoveEmptyFolderFromList(folder, differences.getNewFolderList());
                RemoveEmptyFolderFromList(pairedFolder, pairedDifferences.getNewFolderList());
                differences.AddModifiedFileDifference(file);
            }
        }
        private void RemoveEmptyFolderFromList(FolderMeta folder, List<FolderMeta> folderList)
        {
            if (CheckFolderEmpty(folder))
            {
                //remove the folder from the FolderList
                folderList[folderList.IndexOf(folder, 0, folderList.Count)] = null;
            }
        }
        private void DeleteFileFromRootFolder(FileMeta fileToBeDeleted, FolderMeta baseFolder, List<FolderMeta> baseList)
        {
            baseFolder = CheckFolderInList(baseFolder, baseList);
            //find the root folder in the baseList
            Debug.Assert(baseFolder != null);
            DeleteFileFromRootFolder(fileToBeDeleted, baseFolder);
        }
        private bool DeleteFileFromRootFolder(FileMeta fileToBeDeleted, FolderMeta baseFolder)
        {
            List<FolderMeta> folderList = baseFolder.folders;
            List<FileMeta> fileList = baseFolder.files;
            string fileToBeDeletedInfo = fileToBeDeleted.Path + fileToBeDeleted.Name;
            for (int i = 0; i < fileList.Count; i++)
            {
                //check if file exists in the subfileList
                FileMeta file = fileList[i];
                if (file == null) continue;
                string fileInfo = file.Path + file.Name;
                if (fileToBeDeletedInfo.ToLower().Equals(fileInfo.ToLower()))
                {
                    fileList[i] = null;
                    return true;
                }
            }
            //if not, recursively check if the file exists in the subFolders
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta subFolder = folderList[i];
                if (subFolder == null || !fileToBeDeletedInfo.Contains(subFolder.Path + subFolder.Name)) continue;//find the correct root folder
                if (DeleteFileFromRootFolder(fileToBeDeleted, subFolder))
                    return true;
            }
            return false;
        }
        private bool DeleteDifferencesFileEntry(Conflicts.ConflictType conflictType, FileMeta fileToBeDeleted, Differences differencesToBeUpdated)
        {
            //delete the file entry based on the given type.
            bool deleted = false;
            switch (conflictType)
            {
                case Conflicts.ConflictType.Deleted:
                    deleted = differencesToBeUpdated.removeFileFromDeletedFileList(fileToBeDeleted);
                    break;
                case Conflicts.ConflictType.New:
                    deleted = differencesToBeUpdated.removeFileFromNewFileList(fileToBeDeleted);
                    break;
                case Conflicts.ConflictType.Modified:
                    deleted = differencesToBeUpdated.removeFileFromModifiedFileList(fileToBeDeleted);
                    break;
            }
            return deleted;
        }
        #endregion

        #region Check File and Folder Existence and Count List Size(Helper Methods)

        private bool CheckFolderEmpty(FolderMeta folder)
        {
            List<FolderMeta> folders = folder.folders;
            List<FileMeta> files = folder.files;
            if (CountFileListSize(files) != 0) return false;
            for (int i = 0; i < folders.Count; i++)
            {
                FolderMeta subFolder = folders[i];
                if (subFolder == null) continue;
                //recursively check the subFolders
                if (!CheckFolderEmpty(subFolder))
                {
                    return false;
                }
                else
                {    //if the subFolder is empty, remove it from the entry
                    folders[folders.IndexOf(subFolder, 0, folders.Count)] = null;
                }
            }
            return true;
        }
        private FileMeta CheckFileInList(FileMeta fileToBeChecked, List<FileMeta> fileList)
        {
            FileMeta fileDetected = null;
            string fileToBeCheckedInfo = fileToBeChecked.Path + fileToBeChecked.Name;
            foreach (FileMeta file in fileList)
            {
                if (file == null) continue;
                if (fileToBeCheckedInfo.ToLower().Equals((file.Path + file.Name).ToLower()))
                {
                    fileDetected = file;
                    break;
                }
            }
            return fileDetected;
        }
        private FolderMeta CheckFolderInList(FolderMeta folderToBeChecked, List<FolderMeta> folderList)
        {
            FolderMeta folderDetected = null;
            string folderToBeCheckedInfo = folderToBeChecked.Path + folderToBeChecked.Name;
            foreach (FolderMeta folder in folderList)
            {
                if (folder == null) continue;
                if (folderToBeCheckedInfo.ToLower().Equals((folder.Path + folder.Name).ToLower()))
                {
                    folderDetected = folder;
                    break;
                }
            }
            return folderDetected;
        }
        private int CountFileListSize(List<FileMeta> fileList)
        {
            int i = 0;
            foreach (FileMeta file in fileList)
            {
                if (file != null)
                    i++;
            }
            return i;
        }
        private FolderMeta FindFileRootFolder(FileMeta file, List<FolderMeta> folderList)
        {
            FolderMeta resultFolder = null;
            string fileInfo = file.Path + file.Name;
            foreach (FolderMeta folder in folderList)
            {
                if (folder == null) continue;
                if ((fileInfo + @"\").ToLower().Contains((folder.Path + folder.Name + @"\").ToLower()))
                {
                    resultFolder = folder;
                    break;
                }
            }
            return resultFolder;
        }
        #endregion
         
        }


           


    
}
