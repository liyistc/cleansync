/***********************************************************************
 * 
 * ******************CleanSync Version 2.0 CompareLogic*****************
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
    public class CompareLogic
    {
        #region Constructor
        public CompareLogic()
        {
        }
        #endregion

        #region FolderMetaToDifferenceConvertor
        /// <summary>
        /// convert a given FolderMeta object to a Difference object.
        /// </summary>
        /// <param name="folderMeta">FolderMeta to be converted </param>
        /// <returns>A Differences object</returns>
        public Differences ConvertFolderMetaToDifferences(FolderMeta folderMeta)
        {
            Differences differences  = new Differences();
            CompareDirectories(folderMeta, null, differences);
            return differences;
        }
        #endregion


        #region Directories Comparison Methods
        /// <summary>
        /// Compare the current version direcotry metadata with its previous version and stores the change as a Differences object
        /// </summary>
        /// <param name="newTree">The current metadata of the directory</param>
        /// <param name="oldTree">Previous metadata of the directory</param>
        /// <returns>a Differences objects</returns>
        public Differences CompareDirectories(FolderMeta newTree, FolderMeta oldTree)
        {    
            Differences differences = new Differences();
            CompareDirectories(newTree, oldTree, differences);
            return differences;
        }

        private void CompareDirectories(FolderMeta newTree, FolderMeta oldTree, Differences differences)
        {  
            //recursively compares the subFiles and subFolders
            this.CompareFiles(newTree, oldTree, differences);
            this.CompareFolders(newTree, oldTree, differences);
        }

        private void CompareFiles(FolderMeta newFolder, FolderMeta oldFolder, Differences differences)
        {
            bool newSubFilesNext   = false;
            bool oldSubFilesNext   = false;
            //subFiles Enumerators
            IEnumerator<FileMeta> newSubFiles = null;
            IEnumerator<FileMeta> oldSubFiles = null;
            if (newFolder != null)
            {
                newSubFiles = newFolder.GetFiles();
                newSubFilesNext = newSubFiles.MoveNext();
            }
            if (oldFolder != null)
            {
                oldSubFiles = oldFolder.GetFiles();
                oldSubFilesNext = oldSubFiles.MoveNext();
            }
            
            while (newSubFilesNext && oldSubFilesNext)
            {
                FileMeta newSubFile = newSubFiles.Current;
                FileMeta oldSubFile = oldSubFiles.Current;
                
                if (newSubFile  <oldSubFile)
                {
                    //operator '<' compares the file name. 
                    //If newSubFile < oldSubFile, newSubFile is a newFile(It does not exist in the old enumerator) 
                    //add it into the Differences and move to the next.
                    differences.AddNewFileDifference(newSubFile);
                    newSubFilesNext = newSubFiles.MoveNext();       
                }
                else if (newSubFile >oldSubFile)
                {
                    //newSubFile is a deleted file in this case
                    differences.AddDeletedFileDifference(oldSubFile);
                    oldSubFilesNext = oldSubFiles.MoveNext();
                }
                else
                { 
                    //files with the same name, check whether it is a modified file
                    if (newSubFile.LastModifiedTime.CompareTo(oldSubFile.LastModifiedTime)!=0 ||newSubFile.Size != oldSubFile.Size)
                    {
                        differences.AddModifiedFileDifference(newSubFile);
                    }
                    newSubFilesNext = newSubFiles.MoveNext();
                    oldSubFilesNext = oldSubFiles.MoveNext();
                }
            }
            while (newSubFilesNext)
            {
                //add all remaining newSubFiles to the newFile list.
                differences.AddNewFileDifference(newSubFiles.Current);
                newSubFilesNext = newSubFiles.MoveNext();
            }
            while (oldSubFilesNext)
            {
                //add all remaining oldSubFiles to the deletedFile list
                differences.AddDeletedFileDifference(oldSubFiles.Current);
                oldSubFilesNext=oldSubFiles.MoveNext();
            }
        }//end of compareFiles

        private void CompareFolders(FolderMeta newFolder, FolderMeta oldFolder, Differences differences)
        {
            bool newSubFoldersNext = false;
            bool oldSubFoldersNext = false;
            IEnumerator<FolderMeta> newSubFolders = null;
            IEnumerator<FolderMeta> oldSubFolders = null;
            if (newFolder != null)
            {
                newSubFolders = newFolder.GetFolders();
                newSubFoldersNext = newSubFolders.MoveNext();
            }
            if (oldFolder != null)
            {
                oldSubFolders = oldFolder.GetFolders();
                oldSubFoldersNext = oldSubFolders.MoveNext();
            }
            while (newSubFoldersNext && oldSubFoldersNext)
            {
                
                FolderMeta newSubFolder = newSubFolders.Current;
                FolderMeta oldSubFolder = oldSubFolders.Current;
               // bool flag = newSubFolder.Name.ToLower().CompareTo(oldSubFolder.Name.ToLower()) < 0;
                if (newSubFolder <oldSubFolder)
                {
                    //newSubFolder is a new Folder, add it into the newFolderList and move next
                    differences.AddNewFolderDifference(newSubFolder);
                    newSubFoldersNext = newSubFolders.MoveNext();
                }
                else if (oldSubFolder <newSubFolder)
                {
                    //oldSubFolder is a deleted Folder
                    differences.AddDeletedFolderDifference(oldSubFolder);
                    oldSubFoldersNext = oldSubFolders.MoveNext();
                }
                else
                {
                    //FolderName the same, call CompareDirectories method recursively.
                    CompareDirectories(newSubFolder, oldSubFolder, differences);
                    newSubFoldersNext = newSubFolders.MoveNext();
                    oldSubFoldersNext = oldSubFolders.MoveNext();
                }
            }
            while (newSubFoldersNext)
            {  
                //add all remaining newSubFolders to the newFolder list
                differences.AddNewFolderDifference(newSubFolders.Current);
                newSubFoldersNext = newSubFolders.MoveNext();
            }
            while (oldSubFoldersNext)
            {
                //add all remaining oldSubFolders to the deletedFolderList
                differences.AddDeletedFolderDifference(oldSubFolders.Current);
                oldSubFoldersNext = oldSubFolders.MoveNext();   
            }
        }
        #endregion


        #region Conflicts Detection Methods
        /// <summary>
        /// given two Differences objects, detects the conflicts between them and returns a list of Conflicts object
        /// </summary>
        /// <param name="USBFoldersAndFiles">Differences from the remote PC, store in the USB</param>
        /// <param name="PCFoldersAndFiles">Current PC Differences</param>
        /// <returns>a list of Conflicts object</returns>
        public List<Conflicts> DetectConflicts(Differences USBDifferences, Differences PCDifferences)
        {
            List<Conflicts> conflicts = new List<Conflicts>();
            DetectFolderConflict(conflicts, PCDifferences, USBDifferences);
            DetectFileConflict(conflicts, PCDifferences, USBDifferences);
            return conflicts;
        }

        private void DetectFolderConflict(List<Conflicts> conflicts, Differences PCDifferences, Differences USBDifferences)
        {
            //Only 2 possible FolderConflicts types. Note NewFolder VS DeletedRootFolder is not considered as a confict for our implementation.
            DetectDeletedFolderConflictWithDeletedFolderList(PCDifferences.getDeletedFolderList(), USBDifferences.getDeletedFolderList());
            DetectDeletedFolderConflictWithDeletedFolderList(USBDifferences.getDeletedFolderList(), PCDifferences.getDeletedFolderList());
            DetectNewFolderConflictWithNewFolderList(conflicts, PCDifferences.getNewFolderList(), USBDifferences.getNewFolderList());
        }

        private void DetectDeletedFolderConflictWithDeletedFolderList(List<FolderMeta> folderList, List<FolderMeta> baseFolderList)
        {
            FolderMeta folder = null;
            for (int i = 0; i < folderList.Count; i++)
            {
                folder = folderList.ElementAt(i);
                if (folder == null) continue;
                FolderMeta folderInUSB = CheckFolderContainedInRootFolder(folder, baseFolderList);
                //if folderInUSB is not null, it is a Deleted VS Deleted Conflict
                if (folderInUSB != null && (folderInUSB.Path + folderInUSB.Name).ToLower().Equals((folder.Path + folder.Name).ToLower()))
                {
                    //Deleted Folders with Same name, remove both of them from the list entry.
                    folderList[i] = null;
                    baseFolderList[baseFolderList.IndexOf(folderInUSB, 0, baseFolderList.Count)] = null; 
                }
                else if (folderInUSB != null && (folderInUSB.Path + folderInUSB.Name+@"\").ToLower().CompareTo((folder.Path + folder.Name+@"\").ToLower()) < 0)
                {
                    //folder is a subFolder of folderInUSB, remove folder from folderInUSB
                    RemoveFolderFromRootFolder(folderList, folder, folderInUSB);
                }
            }
        }

        private void DetectFileConflict(List<Conflicts> conflicts, Differences PCDifferences, Differences USBDifferences)
        {
            Conflicts.ConflictType Modified = Conflicts.ConflictType.Modified;
            Conflicts.ConflictType New = Conflicts.ConflictType.New;
            Conflicts.ConflictType Deleted = Conflicts.ConflictType.Deleted;
            List<FileMeta> USBModifiedFiles = USBDifferences.getModifiedFileList();
            List<FileMeta> USBNewFiles = USBDifferences.getNewFileList();
            List<FileMeta> USBDeletedFiles = USBDifferences.getDeletedFileList();
            List<FileMeta> PCModifiedFiles = PCDifferences.getModifiedFileList();
            List<FileMeta> PCNewFiles = PCDifferences.getNewFileList();
            List<FileMeta> PCDeletedFiles = PCDifferences.getDeletedFileList();
            List<FolderMeta> USBDeletedFolders = USBDifferences.getDeletedFolderList();
            List<FolderMeta> PCDeletedFolders = PCDifferences.getDeletedFolderList();

            //Detect Modified file conflict with Deleted folder
            DetectFileConflictWithFolderList(conflicts,Modified, USBModifiedFiles,PCDeletedFolders, Conflicts.FolderFileType.FolderVSFileConflict);
            DetectFileConflictWithFolderList(conflicts,Modified, PCModifiedFiles, USBDeletedFolders,Conflicts.FolderFileType.FileVSFolderConflict);

            //Detect Deleted file conflict with Deleted folder
            DetectFileConflictWithFolderList(conflicts,Deleted, USBDeletedFiles,PCDeletedFolders, Conflicts.FolderFileType.FolderVSFileConflict);
            DetectFileConflictWithFolderList(conflicts,Deleted, PCDeletedFiles, USBDeletedFolders,Conflicts.FolderFileType.FileVSFolderConflict);
            
            //Detect File VS File Conflicts
            DetectFileConflictInList(conflicts, PCDeletedFiles, USBModifiedFiles,Deleted, Modified);
            DetectFileConflictInList(conflicts, PCModifiedFiles, USBDeletedFiles,Modified, Deleted);
            DetectFileConflictInList(conflicts, PCModifiedFiles, USBModifiedFiles, Modified,Modified);
            DetectFileConflictInList(conflicts, PCNewFiles, USBNewFiles, New,New);
            DetectFileConflictInList(conflicts, PCDeletedFiles, USBDeletedFiles, Deleted, Deleted);
        }

        private void DetectFileConflictInList(List<Conflicts> conflicts, List<FileMeta> fileList, List<FileMeta> baseList, Conflicts.ConflictType fileType, Conflicts.ConflictType baseFileType)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList.ElementAt(i);
                if (file == null) continue;
                FileMeta fileInBaseList = CheckFileInList(file, baseList);
                if (fileInBaseList != null)
                {
                    //if fileInBaseList != null, it is a file vs file conflict
                    int index = baseList.IndexOf(fileInBaseList, 0, baseList.Count);
                    if (fileType == Conflicts.ConflictType.Deleted && baseFileType == Conflicts.ConflictType.Deleted)
                    {
                        //if both are of Deleted type, remove them from the list entry(Both agree to delete result in no conflict)
                        fileList[i] = null;
                        baseList[index] = null;
                    }
                    else if (fileType == Conflicts.ConflictType.New && baseFileType == Conflicts.ConflictType.New)
                    {
                        //if both are of new type,
                        //check whether the two files are identical(One may copy the files to the remote PC, which should not be reported as conflicts)
                        if (file.Size != fileInBaseList.Size || file.LastModifiedTime.CompareTo(fileInBaseList.LastModifiedTime) != 0)
                        {
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.FileConflict, file, fileInBaseList, fileType, baseFileType));
                        }
                        else
                        { 
                            //remove from the entry if the two files are identical
                            baseList[index] = null;
                            fileList[i] = null;
                        }
                    }
                    else
                    {
                        //else it is a normal file conflict, add it to the conflict list
                        conflicts.Add(new Conflicts(Conflicts.FolderFileType.FileConflict, file, fileInBaseList, fileType, baseFileType));
                    }
                }
            }
        }

        private void DetectFileConflictWithFolderList(List<Conflicts> conflicts, Conflicts.ConflictType type, List<FileMeta> fileList, List<FolderMeta> deletedFolderList, Conflicts.FolderFileType folderFileType)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList.ElementAt(i);
                if (file == null) continue;
                string fileKey = file.Path + file.Name;
                FolderMeta folder = null;
                foreach (FolderMeta deletedFolder in deletedFolderList)
                {
                    if (deletedFolder == null) continue;
                    string deletedFolderKey = deletedFolder.Path + deletedFolder.Name;
                    if ((fileKey + @"\").ToLower().Contains((deletedFolderKey + @"\").ToLower()))
                    {
                        //if fileKey Contains deletedFolderKey, then file is a subFile of deletedFolder, it is a File VS Folder Conflict
                        folder = deletedFolder;
                        break;
                    }
                }
                if (folder != null && type == Conflicts.ConflictType.Modified)
                {
                    if (!CheckFileInFolder(file, folder)) return;//special case, a modified file may not be in a deletedFolder entry as a result of a previous conflict handling.
                    switch (folderFileType)
                    {
                        case Conflicts.FolderFileType.FileVSFolderConflict:
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.FileVSFolderConflict, file, folder, type, Conflicts.ConflictType.Deleted));
                            break;
                        case Conflicts.FolderFileType.FolderVSFileConflict:
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.FolderVSFileConflict, folder, file, Conflicts.ConflictType.Deleted, type));
                            break;
                    }
                }
                else if (folder != null && type == Conflicts.ConflictType.Deleted)
                {
                    //if file is of Deleted type, remove the entries.
                    RemoveFileFromDeletedListAndDeletedFolder(fileList, file, folder);
                }
            }
        }

       
        private void DetectNewFolderConflictWithNewFolderList(List<Conflicts> conflicts, List<FolderMeta> PCFolderList,  List<FolderMeta> USBFolderList)
        { 
            for (int i = 0; i < PCFolderList.Count; i++)
            {
                FolderMeta PCFolder = PCFolderList[i];
                if (PCFolder == null) continue;
                FolderMeta USBFolder = CheckFolderInList(PCFolder, USBFolderList);
                if (USBFolder != null)
                {
                    //if USBFOlder != null, it is a NewFolder VS NewFolder Conflict
                    USBFolder.FolderType = ComponentMeta.Type.Modified;
                    PCFolder.FolderType = ComponentMeta.Type.Modified;
                    List<FileMeta> PCFiles = PCFolder.files;
                    List<FileMeta> USBFiles = USBFolder.files;
                    List<FolderMeta> PCFolders = PCFolder.folders;
                    List<FolderMeta> USBFolders = USBFolder.folders;
                    
                    //recursively detect the conflicts in subfiles and subfolders, remove the identical entries
                    DetectFileConflictInList(conflicts, PCFiles, USBFiles, Conflicts.ConflictType.New, Conflicts.ConflictType.New);
                    DetectNewFolderConflictWithNewFolderList(conflicts, PCFolders, USBFolders);
                    //clean up the folders
                    if (CountFileListSize(PCFiles) == 0 && CountFolderListSize(PCFolders) == 0)
                    {
                        PCFolderList[i] =null; 
                    } 
                    if(CountFileListSize(USBFiles) == 0 && CountFolderListSize(USBFolders) == 0)
                    {   
                        USBFolderList[USBFolderList.IndexOf(USBFolder,0,USBFolderList.Count)] =null;
                    }
                }
            }
        }

        #endregion


        #region Remove File and Folder Methods
        private void RemoveFileFromDeletedListAndDeletedFolder(List<FileMeta> fileList, FileMeta file, FolderMeta folder)
        {
            FileMeta fileInList = CheckFileInList(file, folder.files);
            if (fileInList != null)
            {
                //if the file is in the current file list, remove it from the entry
                folder.files[folder.files.IndexOf(fileInList, 0, folder.files.Count)] = null;
                fileList[fileList.IndexOf(file, 0, fileList.Count)] = null;
                return;
            }
            foreach (FolderMeta folderInList in folder.folders)
            {
                //else remove recursively call the subFolders
                if (folderInList == null) continue;
                RemoveFileFromDeletedListAndDeletedFolder(fileList, file, folderInList);
            }
        }
        private void RemoveFolderFromRootFolder(List<FolderMeta> folderList, FolderMeta folder, FolderMeta baseFolder)
        {
            FolderMeta folderInList = CheckFolderInList(folder, baseFolder.folders);
            if (folderInList != null)
            {
                //if the folder is in the current folders list, remove it from the entry
                baseFolder.folders[baseFolder.folders.IndexOf(folderInList, 0, baseFolder.folders.Count)] = null;
                folderList[folderList.IndexOf(folder, 0, folderList.Count)] = null;
                return;
            }
            foreach (FolderMeta subFolderInList in baseFolder.folders)
            {
                //else remove it from the subFolders
                if (subFolderInList == null) continue;
                RemoveFolderFromRootFolder(folderList, folder, subFolderInList);
            }
        }
        #endregion


        #region Count File and Folder Size Methods
        private int CountFolderListSize(List<FolderMeta> folderList)
        {
            int count = 0;
            foreach (FolderMeta folder in folderList)
            {
                if (folder != null)
                    count++;
            }
            return count;
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
        #endregion

        #region Check File and Folder Exitence Methods
        private static FolderMeta CheckFolderContainedInRootFolder(FolderMeta folderIn, List<FolderMeta> folderList)
        {
            string folderKey = folderIn.Path + folderIn.Name;
            FolderMeta folder = null;
            foreach (FolderMeta folderInList in folderList)
            {
                if (folderInList == null) continue;
                string folderInListKey = folderInList.Path + folderInList.Name;
                if ((folderKey + @"\").ToLower().Contains((folderInListKey + @"\").ToLower()) || folderKey.ToLower().Equals(folderInListKey.ToLower()))
                {
                    folder = folderInList;
                    break;
                }
            }
            return folder;
        }

        private bool CheckFileInFolder(FileMeta file, FolderMeta folder)
        {
            string key = file.Path + file.Name;
            foreach (FileMeta fileToBeChecked in folder.files)
            {
                if (fileToBeChecked == null) continue;
                if (key.ToLower().Equals((fileToBeChecked.Path + fileToBeChecked.Name).ToLower()))
                {
                    return true;
                }
            }
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder == null) continue;
                if (CheckFileInFolder(file, subFolder))
                {
                    return true;
                }
            }
            return false;
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
        #endregion
     
    }
}
       