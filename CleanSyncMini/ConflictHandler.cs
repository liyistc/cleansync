using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;

namespace CleanSync
{
    public class ComparisonResultNullException : Exception
    {
        public ComparisonResultNullException(string message)
            : base(message)
        {
        }        
    }

   public class ConflictHandler
    {
        public ConflictHandler()
        {
        }

        public ComparisonResult HandleConflicts(ComparisonResult comparisonResult)
        {
            Debug.Assert(comparisonResult != null);
            Differences PCDifferences  = comparisonResult.PCDifferences;
            Differences USBDifferences = comparisonResult.USBDifferences;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            for(int i =0; i <conflictList.Count; i++)
            {
                Conflicts conflict =  conflictList.ElementAt(i);
                switch(conflict.FolderOrFileConflictType)
                {
                    case Conflicts.FolderFileType.FileConflict: HandleFileConflict(PCDifferences, USBDifferences, conflict, conflict.getUserChoice()); break;
                    case Conflicts.FolderFileType.FolderVSSubFolderConflict: HandleFolderVSSubFolderConflict(PCDifferences, USBDifferences,conflict, conflict.getUserChoice());break;
                    case Conflicts.FolderFileType.SubFolderVSFolderConflict: HandleSubFolderVSFolderConflict(PCDifferences, USBDifferences,conflict, conflict.getUserChoice());break;
                    case Conflicts.FolderFileType.FileVSFolderConflict:  HandleFileVSFolderConflict(PCDifferences,USBDifferences,conflict, conflict.getUserChoice()); break;
                    case Conflicts.FolderFileType.FolderVSFileConflict:   HandleFolderVSFileConflict(PCDifferences,USBDifferences,conflict, conflict.getUserChoice()); break;
                }
                conflictList.Remove(conflict);
                i--;
            }
            Debug.Assert(conflictList.Count == 0);
            return new ComparisonResult(USBDifferences, PCDifferences,conflictList);
        }
       private void HandleFileConflict(Differences PCDifferences,Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
       {
           bool deleted = false;
           switch(userChoice)
           {
               case Conflicts.UserChoice.KeepPCUpdates:  
                   deleted = DeleteDifferencesFileEntry(conflict.USBFolderFileType, conflict.USBFile, USBDifferences);
                   if(!deleted) DeleteFileFromRootFolder( conflict.USBFile,FindFileRootFolder(conflict.USBFile, USBDifferences.getNewFolderList()));
                   break;
               case Conflicts.UserChoice.KeepUSBUpdates:
                   deleted = DeleteDifferencesFileEntry(conflict.PCFolderFileType, conflict.CurrentPCFile, PCDifferences);
                   if (!deleted) DeleteFileFromRootFolder(conflict.CurrentPCFile, FindFileRootFolder(conflict.CurrentPCFile, PCDifferences.getNewFolderList()));
                   break;
               case Conflicts.UserChoice.Untouched:
                   deleted = DeleteDifferencesFileEntry(conflict.USBFolderFileType, conflict.USBFile, USBDifferences);
                   if (!deleted) DeleteFileFromRootFolder(conflict.USBFile, FindFileRootFolder(conflict.USBFile, USBDifferences.getNewFolderList()));
                   deleted = DeleteDifferencesFileEntry(conflict.PCFolderFileType, conflict.CurrentPCFile, PCDifferences);
                   if (!deleted) DeleteFileFromRootFolder(conflict.CurrentPCFile, FindFileRootFolder(conflict.CurrentPCFile, PCDifferences.getNewFolderList()));
                   break;
           }
           
       }

       private FolderMeta FindFileRootFolder(FileMeta file, List<FolderMeta> folderList)
       {
           FolderMeta resultFolder=  null;
           string fileInfo = file.Path + file.Name;
           foreach (FolderMeta folder in folderList)
           {
               if (fileInfo.Contains(folder.Path + folder.Name))
               {
                   resultFolder = folder;
                   break;
               }
            }
           return resultFolder;
       }
       private void HandleFolderVSSubFolderConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
       {
           switch(userChoice)
           {
               case Conflicts.UserChoice.KeepPCUpdates: DeleteDifferencesFolderEntry(conflict.USBFolderFileType, conflict.USBFolder, USBDifferences); break;
               case Conflicts.UserChoice.KeepUSBUpdates: DeleteFolderFromRootFolder(conflict.USBFolder, conflict.CurrentPCFolder,PCDifferences.getDeletedFolderList(), PCDifferences); break;
           }
       }
       private void HandleSubFolderVSFolderConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
       {
           switch(userChoice)
           {
               case Conflicts.UserChoice.KeepUSBUpdates: DeleteDifferencesFolderEntry(conflict.PCFolderFileType, conflict.CurrentPCFolder, PCDifferences); break;
               case Conflicts.UserChoice.KeepPCUpdates: DeleteFolderFromRootFolder(conflict.CurrentPCFolder, conflict.USBFolder,USBDifferences.getDeletedFolderList(), USBDifferences); break;
           }
       }
       private void HandleFileVSFolderConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
       {
           switch (userChoice)
           {
               case Conflicts.UserChoice.KeepUSBUpdates: DeleteDifferencesFileEntry(conflict.PCFolderFileType, conflict.CurrentPCFile, PCDifferences); break;
               case Conflicts.UserChoice.KeepPCUpdates: DeleteFileFromRootFolder(conflict.CurrentPCFile, conflict.USBFolder, USBDifferences.getDeletedFolderList()); break;
           }
       }
       private void HandleFolderVSFileConflict(Differences PCDifferences, Differences USBDifferences, Conflicts conflict, Conflicts.UserChoice userChoice)
       {
           switch (userChoice)
           {
               case Conflicts.UserChoice.KeepPCUpdates: DeleteDifferencesFileEntry(conflict.USBFolderFileType, conflict.USBFile, USBDifferences); break;
               case Conflicts.UserChoice.KeepUSBUpdates: DeleteFileFromRootFolder(conflict.USBFile, conflict.CurrentPCFolder, PCDifferences.getDeletedFolderList()); break;
           }
       }
       private void DeleteFolderFromRootFolder(FolderMeta folderToBeDeleted, FolderMeta baseFolder, List<FolderMeta> baseList, Differences differences)
       {
           
           if((folderToBeDeleted.Path+folderToBeDeleted.Name).Equals(baseFolder.Path+ baseFolder.Name))
           {
               DeleteDifferencesFolderEntry(Conflicts.ConflictType.Deleted, baseFolder, differences);
               return;
           }
           baseFolder = CheckFolderInList(baseFolder, baseList);
           Debug.Assert(baseFolder != null);
           DeleteFolderFromRootFolder(folderToBeDeleted, baseFolder);
       }
       private void DeleteFolderFromRootFolder(FolderMeta folderToBeDeleted, FolderMeta baseFolder)
       {
           List<FolderMeta> folderList = baseFolder.folders;
           string folderToBeDeletedInfo = folderToBeDeleted.Path + folderToBeDeleted.Name;
           for (int i =0; i <folderList.Count; i++)
           {
               FolderMeta folder = folderList[i];
               string folderInfo = folder.Path+ folder.Name;
               if (folderToBeDeletedInfo.Equals(folderInfo))
               {
                   folderList.Remove(folder);
                   Console.WriteLine("SSSSSDDDDDDDDdeletingFolder: "+folder.AbsolutePath );
                   i--;
                   break;
               }
               else if (folderToBeDeletedInfo.Contains(folderInfo))
               {
                   DeleteFolderFromRootFolder(folderToBeDeleted, folder);
                   break;
               }
           }
       }
       private void DeleteFileFromRootFolder(FileMeta fileToBeDeleted, FolderMeta baseFolder, List<FolderMeta> baseList)
       {
           baseFolder = CheckFolderInList(baseFolder, baseList);
           Debug.Assert(baseFolder != null);
           DeleteFileFromRootFolder(fileToBeDeleted, baseFolder);
       }
       private void DeleteFileFromRootFolder(FileMeta fileToBeDeleted, FolderMeta baseFolder)
       {
           
           List<FolderMeta> folderList = baseFolder.folders;
           List<FileMeta> fileList = baseFolder.files;
           string fileToBeDeletedInfo = fileToBeDeleted.Path + fileToBeDeleted.Name;
           for (int i = 0; i < fileList.Count; i++)
           {
               FileMeta file = fileList[i];
               string fileInfo = file.Path+ file.Name;
               if (fileToBeDeletedInfo.Equals(fileInfo))
               {
                   fileList.Remove(file);
                   Console.WriteLine("SSSSSSDDDDDDDDdeletingFile: "+ file.AbsolutePath);
                   i--;
                   return;
               }
           }
           for (int i = 0; i < folderList.Count; i++)
           {
               FolderMeta folder = folderList[i];
               string folderInfo = folder.Path + folder.Name;
               if (fileToBeDeletedInfo.Contains(folderInfo))
               {
                   DeleteFileFromRootFolder(fileToBeDeleted, folder);
                   break;
               }
           }
       }
       private bool DeleteDifferencesFileEntry(Conflicts.ConflictType conflictType,FileMeta FileToBeUpdated, Differences differencesToBeUpdated)
       {
           bool deleted = false;
           switch (conflictType)
           {
               case Conflicts.ConflictType.Deleted:
                   deleted = differencesToBeUpdated.removeFileFromDeletedFileList(FileToBeUpdated);
                   break;
               case Conflicts.ConflictType.New:
                   deleted = differencesToBeUpdated.removeFileFromNewFileList(FileToBeUpdated);
                   break;
               case Conflicts.ConflictType.Modified:
                   deleted = differencesToBeUpdated.removeFileFromModifiedFileList(FileToBeUpdated);
                   break;
            }
           return deleted;
       }
        private void DeleteDifferencesFolderEntry(Conflicts.ConflictType conflictType,FolderMeta FolderToBeUpdated, Differences differencesToBeUpdated)
       {
           switch (conflictType)
           {
               case Conflicts.ConflictType.Deleted:
                   differencesToBeUpdated.removeFolderFromDeletedFolderList(FolderToBeUpdated);
                   break;
               case Conflicts.ConflictType.New:
                   differencesToBeUpdated.removeFolderFromNewFolderList(FolderToBeUpdated);
                   break;
            }
       }
        private FileMeta CheckFileInList(FileMeta fileToBeChecked, List<FileMeta> fileList)
        {
            FileMeta fileDetected = null;
            string fileToBeCheckedInfo = fileToBeChecked.Path + fileToBeChecked.Name;
            foreach (FileMeta file in fileList)
            {
                if (fileToBeCheckedInfo.Equals(file.Path + file.Name))
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
                // MessageBox.Show(folderToBeChecked.Path+folderToBeChecked.Name+"\n"+ folder.Path+folder.Name);
                if (folderToBeCheckedInfo.Equals(folder.Path + folder.Name))
                {
                    folderDetected = folder;
                    break;
                }
            }
            return folderDetected;
        }

 
           
         
        }


           


    
}
