using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;

namespace CleanSyncMini
{
    class CompareLogic
    {
       
        public CompareLogic()
        {
        }

        public Differences ConvertFolderMetaToDifferences(FolderMeta folderMeta)
        {
            Differences differences  = new Differences();
            CompareDirectories(folderMeta, null, differences);
            return differences;
        }
        
        public Differences CompareDirectories(FolderMeta newTree, FolderMeta oldTree)
        {
            
            Differences differences = new Differences();
            CompareDirectories(newTree, oldTree, differences);
            return differences;
        }

        private void CompareDirectories(FolderMeta newTree, FolderMeta oldTree, Differences differences)
        {
           
            this.CompareFiles(newTree, oldTree, differences);
            this.CompareFolders(newTree, oldTree, differences);
        }

        private void CompareFiles(FolderMeta newFolder, FolderMeta oldFolder, Differences differences)
        {
            bool newSubFilesNext   = false;
            bool oldSubFilesNext   = false;
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
                
                if (newSubFile.Name.CompareTo(oldSubFile.Name)< 0 )
                {
                   // Console.WriteLine("I am adding a new File to the list: "+ newSubFile);
                    differences.AddNewFileDifference(newSubFile);
                    newSubFilesNext = newSubFiles.MoveNext();
                   
                }
                else if (newSubFile.Name.CompareTo(oldSubFile.Name)>0)
                {
                   // Console.WriteLine("ponit2");
                    differences.AddDeletedFileDifference(oldSubFile);
                    oldSubFilesNext = oldSubFiles.MoveNext();
                }
                else

                {   //newSubfile is the later version, hence it has a later modifed time 
                    
                    Debug.Assert(newSubFile.LastModifiedTime >= oldSubFile.LastModifiedTime);
             //       Console.WriteLine(newSubFile.Name + " " + newSubFile.LastModifiedTime + " " + oldSubFile.Name + " " + oldSubFile.LastModifiedTime);
                    if (newSubFile.LastModifiedTime > oldSubFile.LastModifiedTime)//modified
                    {
                        
                        differences.AddModifiedFileDifference(newSubFile);
                    }
                    newSubFilesNext = newSubFiles.MoveNext();
                    oldSubFilesNext = oldSubFiles.MoveNext();
                }
            }
            while (newSubFilesNext)
            {
                differences.AddNewFileDifference(newSubFiles.Current);
                newSubFilesNext = newSubFiles.MoveNext();
            }
            while (oldSubFilesNext)
            {
                differences.AddDeletedFileDifference(oldSubFiles.Current);
                oldSubFilesNext=oldSubFiles.MoveNext();
            }
        }

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
         /*   IEnumerator<FolderMeta> newSubFolders = newFolder.GetFolders();
            IEnumerator<FolderMeta> oldSubFolders = oldFolder.GetFolders();

            bool newSubFoldersNext = newSubFolders.MoveNext();
            bool oldSubFoldersNext = oldSubFolders.MoveNext();
           */ //Console.WriteLine(newSubFolders.Current.Name);
            while (newSubFoldersNext && oldSubFoldersNext)
            {
                
                FolderMeta newSubFolder = newSubFolders.Current;
                FolderMeta oldSubFolder = oldSubFolders.Current;
                bool flag = newSubFolder.Name.ToLower().CompareTo(oldSubFolder.Name.ToLower()) < 0;
              //  Console.WriteLine("iteration: "+newSubFolder.Name+ " "+oldSubFolder.Name+ " "+flag);
                if (newSubFolder.Name.CompareTo(oldSubFolder.Name) <0)
                {

                 //   Console.WriteLine("this is a newFolder: " + newSubFolder.Name);
                    differences.AddNewFolderDifference(newSubFolder);
                    newSubFoldersNext = newSubFolders.MoveNext();
                }

                else if (oldSubFolder.Name.CompareTo(newSubFolder.Name)<0)
                {
                    differences.AddDeletedFolderDifference(oldSubFolder);
                    oldSubFoldersNext = oldSubFolders.MoveNext();
                }

                else
                {
                    CompareDirectories(newSubFolder, oldSubFolder, differences);
                    newSubFoldersNext = newSubFolders.MoveNext();
                    oldSubFoldersNext = oldSubFolders.MoveNext();
                }
            }
            
            while (newSubFoldersNext)
            {
               
                differences.AddNewFolderDifference(newSubFolders.Current);
                newSubFoldersNext = newSubFolders.MoveNext();
                
            }
           
            while (oldSubFoldersNext)
            {
                
                differences.AddDeletedFolderDifference(oldSubFolders.Current);
                oldSubFoldersNext = oldSubFolders.MoveNext();
                
            }
        }



        public List<Conflicts> ComparePCwithUSB(Differences USBFoldersAndFiles, Differences PCFoldersAndFiles)
        {
            List<Conflicts> conflicts = new List<Conflicts>();

            List<FolderMeta> USBDeletedFolders = USBFoldersAndFiles.getDeletedFolderList();
           // USBDeletedFolders.Sort();
            List<FolderMeta> PCDeletedFolders = PCFoldersAndFiles.getDeletedFolderList();
            //PCDeletedFolders.Sort();
            List<FolderMeta> PCNewFolders = PCFoldersAndFiles.getNewFolderList();
           // PCNewFolders.Sort()
            List<FolderMeta> USBNewFolders = USBFoldersAndFiles.getNewFolderList();
           // USBNewFolders.Sort();
            List<FileMeta> USBNewFiles = USBFoldersAndFiles.getNewFileList();
            //USBNewFiles.Sort();
            List<FileMeta> USBDeletedFiles = USBFoldersAndFiles.getDeletedFileList();
           // USBDeletedFiles.Sort();
            List<FileMeta> PCNewFiles =   PCFoldersAndFiles.getNewFileList();
            //PCNewFiles.Sort();
            List<FileMeta> PCDeletedFiles = PCFoldersAndFiles.getDeletedFileList();
            //PCDeletedFiles.Sort();
            List<FileMeta> USBModifiedFiles = USBFoldersAndFiles.getModifiedFileList();
            //USBModifiedFiles.Sort();
            List<FileMeta> PCModifiedFiles = PCFoldersAndFiles.getModifiedFileList();
            //PCModifiedFiles.Sort();

            for (int i = 0; i < USBDeletedFolders.Count; i++ )
            {
                
                FolderMeta tempUSB = USBDeletedFolders.ElementAt(i);
                // FolderMeta tempUSB = USBDeletedFolders.ElementAt(i);
               // Console.WriteLine(i+ ": deletedFolder  " + tempUSB.Name);
                // FolderMeta tempPCDeleted = PCDeletedFolders.ElementAt(i);
                //FolderMeta tempPCNew     = PCNewFolders.ElementAt(i);
                FolderMeta folderDeletedConflict = checkList(tempUSB, PCDeletedFolders);
                if (folderDeletedConflict != null)
                {
                   
                    USBDeletedFolders.Remove(tempUSB);
                    i--;
                }
                FolderMeta folderNewConflict = checkList(tempUSB, PCNewFolders);
                if (folderNewConflict != null)
                {

                    conflicts.Add(new Conflicts(folderNewConflict, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted));
                }
            }
            for (int i = 0; i < USBNewFolders.Count; i++)
            {
                FolderMeta tempUSB = USBNewFolders.ElementAt(i);
                //FolderMeta tempPCDeleted = PCDeletedFolders.ElementAt(i);
                //FolderMeta tempPCNew = PCNewFolders.ElementAt(i);
                FolderMeta folderDeletedConflict = checkList(tempUSB, PCDeletedFolders);
                if (folderDeletedConflict != null)
                {
                    conflicts.Add(new Conflicts(folderDeletedConflict, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New));
                }
                FolderMeta folderNewConflict = checkList(tempUSB, PCNewFolders);
                if (folderNewConflict != null)
                {
                    conflicts.Add(new Conflicts(folderNewConflict, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.New));
                }
            }
            for (int i = 0; i < USBNewFiles.Count; i++)
            {
                FileMeta tempUSB = USBNewFiles.ElementAt(i);
                //FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
                //FileMeta tempPCNew = PCNewFiles.ElementAt(i);
                //FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                FileMeta fileDeletedConflict = checkList(tempUSB, PCDeletedFiles);
                if (fileDeletedConflict != null)
                {
                    conflicts.Add(new Conflicts(fileDeletedConflict, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New));
                }
                FileMeta fileNewConflict = checkList(tempUSB, PCNewFiles);
                if (fileNewConflict != null)
                {
                    conflicts.Add(new Conflicts(fileNewConflict, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.New));
                }
                FileMeta fileModifiedConflict = checkList(tempUSB, PCModifiedFiles);
                if (fileModifiedConflict != null)
                {
                    conflicts.Add(new Conflicts(fileModifiedConflict, tempUSB,Conflicts.ConflictType.Modified, Conflicts.ConflictType.New));
                }
            }
            for (int i = 0; i < USBModifiedFiles.Count; i++)
            {
                FileMeta tempUSB = USBModifiedFiles.ElementAt(i);
               // FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
               // FileMeta tempPCNew = PCNewFiles.ElementAt(i);
               // FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                FileMeta fileDeletedConflict = checkList(tempUSB, PCDeletedFiles);
                if (fileDeletedConflict != null)
                {
                    conflicts.Add(new Conflicts(fileDeletedConflict, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.Modified));
                }
                FileMeta fileNewConflict = checkList(tempUSB, PCNewFiles);
                if (fileNewConflict != null)
                {
                    conflicts.Add(new Conflicts(fileNewConflict, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Modified));
                }
                FileMeta fileModifiedConflict = checkList(tempUSB, PCModifiedFiles);
                if (fileModifiedConflict != null)
                {
                    conflicts.Add(new Conflicts(fileModifiedConflict, tempUSB, Conflicts.ConflictType.Modified, Conflicts.ConflictType.Modified));
                }
            }
            for (int i = 0; i < USBDeletedFiles.Count; i++)
            {
                FileMeta tempUSB = USBDeletedFiles.ElementAt(i);
                //FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
                //FileMeta tempPCNew = PCNewFiles.ElementAt(i);
                //FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                FileMeta fileDeletedConflict = checkList(tempUSB, PCDeletedFiles);
                if (fileDeletedConflict != null)
                {
                    USBDeletedFiles.Remove(tempUSB);
                    i--;
                } 
                FileMeta fileNewConflict = checkList(tempUSB, PCNewFiles);
                if (fileNewConflict != null)
                {
                    conflicts.Add(new Conflicts(fileNewConflict, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted));
                } 
                FileMeta fileModifiedConflict = checkList(tempUSB, PCModifiedFiles);
                if (fileModifiedConflict != null)
                {
                    conflicts.Add(new Conflicts(fileModifiedConflict, tempUSB,Conflicts.ConflictType.Modified, Conflicts.ConflictType.Deleted));
                }
            }
               
            return conflicts;
        }
        private FolderMeta checkList(FolderMeta folderMeta, List<FolderMeta> folderList)
        {
            FolderMeta folderDetected = null;
            foreach (FolderMeta folder in folderList)
            {
                if (folderMeta.Name.CompareTo(folder.Name) ==0)
                {
                    folderDetected = folder;
                    break; 
                }
            }
            return folderDetected;
        }
        private FileMeta checkList(FileMeta fileMeta, List<FileMeta> fileList)
        {
            FileMeta fileDetected = null;
            foreach (FileMeta file in fileList)
            {
                if (fileMeta.Name.CompareTo(file.Name) ==0)
                {
                    fileDetected = file;
                    break; 
                }
            }
            return fileDetected;
        }


        //test code for compareDirectories
        public void compareTest(Differences diff)
        {
            List<FolderMeta> deletedFolderDifference = diff.getDeletedFolderList();
            List<FolderMeta> newFolderDifference = diff.getNewFolderList();
            List<FileMeta> deletedFileDifference = diff.getDeletedFileList();
            List<FileMeta> newFileDifference = diff.getNewFileList();
            List<FileMeta> modifiedFileDifference = diff.getModifiedFileList();

            Console.WriteLine("DeletedFolders");
            printResultFolder(deletedFolderDifference);
            Console.WriteLine("NewFolders");
            printResultFolder( newFolderDifference);
            Console.WriteLine("DeletedFiles");
            printResultFile(deletedFileDifference);
            Console.WriteLine("NewFiles");
            printResultFile(newFileDifference);
            Console.WriteLine("ModifiedFiles");
            printResultFile(modifiedFileDifference);

        }


        public static void printResultFolder(List<FolderMeta> folderList)
        {
            foreach( FolderMeta folder in folderList ){
                Console.WriteLine("   "+folder.Name);
            }

        }
        public static void printResultFile(List<FileMeta> fileList)
        {

            foreach (FileMeta file in fileList)
            {
                
                Console.WriteLine("   "+file.Name);
            }

        }
        public static void printResultConflicts(List<Conflicts> conflictList)
        {
            foreach (Conflicts conflict in conflictList)
            {
                if(conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FolderConflict)
                {
                    Console.WriteLine("PCFodler: "+conflict.CurrentPCFolder.Name +" conflicts with USBFolder: "+ conflict.USBFolder.Name);

                }
                else if (conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FileConflict)
                {
                    Console.WriteLine("PCFile: " + conflict.CurrentPCFile.Name + " conflicts with USBFile: " + conflict.USBFile.Name);

                }
               
            }

        }

        //test code for comparePCwithUSB
        public void ComaprePCwithUSBTest(ComparisonResult comparisonResult)
        {   Differences USBDifferences = comparisonResult.USBDifferences;
            Differences PCDifferences = comparisonResult.PCDifferences;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            Console.WriteLine("USBDifferences:");
            Console.WriteLine("DeletedFolder");
            printResultFolder(USBDifferences.getDeletedFolderList());
            Console.WriteLine("NewFolder");
            printResultFolder(USBDifferences.getNewFolderList());
            Console.WriteLine("DeletedFiles");
            printResultFile(USBDifferences.getDeletedFileList());
            Console.WriteLine("NewFiles");
            printResultFile(USBDifferences.getNewFileList());
            Console.WriteLine("ModifiedFiles");
            printResultFile(USBDifferences.getModifiedFileList());

            Console.WriteLine();
            Console.WriteLine("PCDifferences:");
            Console.WriteLine("DeletedFolder");
            printResultFolder(PCDifferences.getDeletedFolderList());
            Console.WriteLine("NewFolder");
            printResultFolder(PCDifferences.getNewFolderList());
            Console.WriteLine("DeletedFiles");
            printResultFile(PCDifferences.getDeletedFileList());
            Console.WriteLine("NewFiles");
            printResultFile(PCDifferences.getNewFileList());
            Console.WriteLine("ModifiedFiles");
            printResultFile(PCDifferences.getModifiedFileList());


            Console.WriteLine("Conflictions Detected:");
            printResultConflicts(conflictList);
        }

    }


}
