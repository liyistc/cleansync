using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;
namespace CleanSyncCompare
{
    class CompareLogic
    {

        public CompareLogic()
        {
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
            IEnumerator<FileMeta> newSubFiles = newFolder.GetFiles();
            IEnumerator<FileMeta> oldSubFiles = oldFolder.GetFiles();
            bool newSubFilesNext  =newSubFiles.MoveNext();
            bool oldSubFilesNext  = oldSubFiles.MoveNext();
            
           
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
                 //  Console.WriteLine(newSubFile.LastModifiedTime + " "+ oldSubFile.LastModifiedTime);
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
            IEnumerator<FolderMeta> newSubFolders = newFolder.GetFolders();
            IEnumerator<FolderMeta> oldSubFolders = oldFolder.GetFolders();

            bool newSubFoldersNext = newSubFolders.MoveNext();
            bool oldSubFoldersNext = oldSubFolders.MoveNext();
            //Console.WriteLine(newSubFolders.Current.Name);
            while (newSubFoldersNext && oldSubFoldersNext)
            {
                
                FolderMeta newSubFolder = newSubFolders.Current;
                FolderMeta oldSubFolder = oldSubFolders.Current;

                if (newSubFolder.Name.CompareTo(oldSubFolder.Name) <0)
                {
                    
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
                newSubFoldersNext = newSubFolders.MoveNext();
                differences.AddNewFolderDifference(newSubFolders.Current);
                
            }
            while (oldSubFoldersNext)
            {
                oldSubFoldersNext = oldSubFolders.MoveNext();
                differences.AddDeletedFolderDifference(oldSubFolders.Current);
                
            }
        }



        public LinkedList<Conflicts> comparePCwithUSB(Differences USBFoldersAndFiles, Differences PCFoldersAndFiles)
        {
            LinkedList<Conflicts> conflicts = new LinkedList<Conflicts>();
            
            LinkedList<FolderMeta> USBDeletedFolders = USBFoldersAndFiles.getDeletedFolderList();
            LinkedList<FolderMeta> PCDeletedFolders = PCFoldersAndFiles.getDeletedFolderList();
            LinkedList<FolderMeta> PCNewFolders = PCFoldersAndFiles.getNewFolderList();
            LinkedList<FolderMeta> USBNewFolders = USBFoldersAndFiles.getNewFolderList();
            
            LinkedList<FileMeta> USBNewFiles = USBFoldersAndFiles.getNewFileList();
            LinkedList<FileMeta> USBDeletedFiles = USBFoldersAndFiles.getDeletedFileList();
            LinkedList<FileMeta> PCNewFiles =   PCFoldersAndFiles.getNewFileList();
            LinkedList<FileMeta> PCDeletedFiles = PCFoldersAndFiles.getDeletedFileList();
            LinkedList<FileMeta> USBModifiedFiles = USBFoldersAndFiles.getModifiedFileList();
            LinkedList<FileMeta> PCModifiedFiles = PCFoldersAndFiles.getModifiedFileList();
            for (int i = 0; i < USBDeletedFolders.Count; i++)
            {
                FolderMeta tempUSB = USBDeletedFolders.ElementAt(i);
                FolderMeta tempPCDeleted = PCDeletedFolders.ElementAt(i);
                FolderMeta tempPCNew     = PCNewFolders.ElementAt(i);
                if (tempUSB.Name.CompareTo(tempPCDeleted.Name) == 0)
                {
                    USBDeletedFolders.Remove(tempUSB);
                }
                if (tempUSB.Name.CompareTo(tempPCNew.Name) == 0)
                {

                    conflicts.AddLast(new Conflicts(tempPCNew, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted));
                }
            }
            for (int i = 0; i < USBNewFolders.Count; i++)
            {
                FolderMeta tempUSB = USBNewFolders.ElementAt(i);
                FolderMeta tempPCDeleted = PCDeletedFolders.ElementAt(i);
                FolderMeta tempPCNew = PCNewFolders.ElementAt(i);
                if (tempUSB.Name.CompareTo(tempPCDeleted.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCDeleted, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New));
                }
                if (tempUSB.Name.CompareTo(tempPCNew.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCNew, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.New));
                }
            }
            for (int i = 0; i < USBNewFiles.Count; i++)
            {
                FileMeta tempUSB = USBNewFiles.ElementAt(i);
                FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
                FileMeta tempPCNew = PCNewFiles.ElementAt(i);
                FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                if (tempUSB.Name.CompareTo(tempPCDeleted.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCDeleted, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New));
                }
                if (tempUSB.Name.CompareTo(tempPCNew.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCNew, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.New));
                }
                if (tempUSB.Name.CompareTo(tempPCModified.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCModified, tempUSB,Conflicts.ConflictType.Modified, Conflicts.ConflictType.New));
                }
            }
            for (int i = 0; i < USBModifiedFiles.Count; i++)
            {
                FileMeta tempUSB = USBModifiedFiles.ElementAt(i);
                FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
                FileMeta tempPCNew = PCNewFiles.ElementAt(i);
                FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                if (tempUSB.Name.CompareTo(tempPCDeleted.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCDeleted, tempUSB, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.Modified));
                }
                if (tempUSB.Name.CompareTo(tempPCNew.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCNew, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Modified));
                }
                if (tempUSB.Name.CompareTo(tempPCModified.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCModified, tempUSB, Conflicts.ConflictType.Modified, Conflicts.ConflictType.Modified));
                }
            }
            for (int i = 0; i < USBDeletedFiles.Count; i++)
            {
                FileMeta tempUSB = USBDeletedFiles.ElementAt(i);
                FileMeta tempPCDeleted = PCDeletedFiles.ElementAt(i);
                FileMeta tempPCNew = PCNewFiles.ElementAt(i);
                FileMeta tempPCModified = PCModifiedFiles.ElementAt(i);
                if (tempUSB.Name.CompareTo(tempPCDeleted.Name) == 0)
                {
                    USBDeletedFiles.Remove(tempUSB);
                }
                if (tempUSB.Name.CompareTo(tempPCNew.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCNew, tempUSB, Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted));
                }
                if (tempUSB.Name.CompareTo(tempPCModified.Name) == 0)
                {
                    conflicts.AddLast(new Conflicts(tempPCModified, tempUSB,Conflicts.ConflictType.Modified, Conflicts.ConflictType.Deleted));
                }
            }
               
            return conflicts;
        }


        //test code for compareDirectories
        public void compareTest(Differences diff)
        {
            LinkedList<FolderMeta> deletedFolderDifference = diff.getDeletedFolderList();
            LinkedList<FolderMeta> newFolderDifference = diff.getNewFolderList();
            LinkedList<FileMeta> deletedFileDifference = diff.getDeletedFileList();
            LinkedList<FileMeta> newFileDifference = diff.getNewFileList();
            LinkedList<FileMeta> modifiedFileDifference = diff.getModifiedFileList();

            Console.WriteLine("DeletedFolders");
            printResult(deletedFolderDifference);
            Console.WriteLine("NewFolders");
            printResult( newFolderDifference);
            Console.WriteLine("DeletedFiles");
            printResult(deletedFileDifference);
            Console.WriteLine("NewFiles");
            printResult(newFileDifference);
            Console.WriteLine("ModifiedFiles");
            printResult(modifiedFileDifference);

        }


        public static void printResult(LinkedList<FolderMeta> folderList)
        {
            foreach( FolderMeta folder in folderList ){
                Console.WriteLine("   "+folder.Name);
            }

        }
        public void printResult(LinkedList<FileMeta> fileList)
        {

            foreach (FileMeta file in fileList)
            {
                
                Console.WriteLine("   "+file.Name);
            }

        }

        //test code for comparePCwithUSB

    }
}
