using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;

namespace CleanSyncMini
{
    public class ComparisonResultNullException : Exception
    {
        public ComparisonResultNullException(string message)
            : base(message)
        {
        }
        
    }


    class ConflictHandler
    {



        public ComparisonResult handleConflicts(ComparisonResult comparisonResult, int[] userChoice)
        {
            //question: do we need to create our own exception class?

            if (comparisonResult == null)
            {
                throw new ComparisonResultNullException("ComparisonResult is null");

            }
            if (userChoice == null)
            {
                return comparisonResult;
            }
            Differences PCDifferences = comparisonResult.PCDifferences;
            Differences USBDifferencs = comparisonResult.USBDifferences;
            Differences differencesToBeUpdated;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            Debug.Assert(userChoice.Length == conflictList.Count);
            
            for (int i = 0; i < userChoice.Length; i++)
            {
                Debug.Assert(userChoice[i] == 0 || userChoice[i] == 1);
                if (userChoice[i] == 0)
                {//user chooose to keep the USBDifferences, hence we should delete the difference in PC which causes the conflict
                    differencesToBeUpdated = PCDifferences;
                }
                else
                {
                    differencesToBeUpdated = USBDifferencs;
                }

                if (conflictList[i].FolderOrFileConflictType == Conflicts.FolderFileType.FolderConflict)
                {
                    
                    handleFolderConflict(differencesToBeUpdated,conflictList[i], userChoice[i]);
                }
                else if (conflictList[i].FolderOrFileConflictType == Conflicts.FolderFileType.FileConflict)
                {
                    handleFileConflict(differencesToBeUpdated, conflictList[i], userChoice[i]);
                }
            }
            return comparisonResult;
        }


            private void handleFolderConflict(Differences differencesToBeUpdated , Conflicts folderConflict, int USBorPCIndex)
            {
                FolderMeta FolderToBeUpdated;
                Conflicts.ConflictType conflictType;
                if (USBorPCIndex == 0)
                {
                    FolderToBeUpdated = folderConflict.CurrentPCFolder;
                    conflictType = folderConflict.PCFolderFileType;
                }
                else
                {
                    FolderToBeUpdated = folderConflict.USBFolder;
                    conflictType = folderConflict.USBFolderFileType;
                }

                switch(conflictType){
                    case Conflicts.ConflictType.Deleted:
                        differencesToBeUpdated.removeFolderFromDeletedFolderList(FolderToBeUpdated);
                        break;
                    case Conflicts.ConflictType.New:
                        differencesToBeUpdated.removeFolderFromNewFolderList(FolderToBeUpdated);
                        break;
                }
            }
            private void handleFileConflict(Differences differencesToBeUpdated, Conflicts fileConflict, int USBorPCIndex)
            {
                FileMeta FileToBeUpdated;
                Conflicts.ConflictType conflictType;
                if (USBorPCIndex == 0)
                {
                    FileToBeUpdated = fileConflict.CurrentPCFile;
                    conflictType = fileConflict.PCFolderFileType;
                }
                else
                {
                    FileToBeUpdated = fileConflict.USBFile;
                    conflictType = fileConflict.USBFolderFileType;
                }

                switch (conflictType)
                {
                    case Conflicts.ConflictType.Deleted:
                        differencesToBeUpdated.removeFileFromDeletedFileList(FileToBeUpdated);
                        break;
                    case Conflicts.ConflictType.New:
                        differencesToBeUpdated.removeFileFromNewFileList(FileToBeUpdated);
                        break;
                    case Conflicts.ConflictType.Modified:
                        differencesToBeUpdated.removeFileFromModifiedFileList(FileToBeUpdated);
                        break;
                }
            }


    }
}
