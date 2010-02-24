using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;
namespace CleanSyncCompare
{
    public class CompareLogic
    {



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

            newSubFiles.MoveNext();
            oldSubFiles.MoveNext();

            while (newSubFiles.Current != null && oldSubFiles.Current != null)
            {
                FileMeta newSubFile = newSubFiles.Current;
                FileMeta oldSubFile = oldSubFiles.Current;

                if (newSubFile < oldSubFile)
                {
                    differences.AddNewFileDifference(newSubFile);
                    newSubFiles.MoveNext();
                }
                else if (newSubFile > oldSubFile)
                {
                    differences.AddDeletedFileDifference(oldSubFile);
                    oldSubFiles.MoveNext();
                }
                else
                {   //newSubfile is the later version, hence it has a later modifed time 
                    Debug.Assert(newSubFile.LastModifiedTime >= oldSubFile.LastModifiedTime);
                    if (newSubFile.LastModifiedTime > oldSubFile.LastModifiedTime)//modified
                    {
                        differences.AddModifiedFileDifference(newSubFile);
                    }
                    newSubFiles.MoveNext();
                    oldSubFiles.MoveNext();
                }
            }
        }

        private void CompareFolders(FolderMeta newFolder, FolderMeta oldFolder, Differences differences)
        {
            IEnumerator<FolderMeta> newSubFolders = newFolder.GetFolders();
            IEnumerator<FolderMeta> oldSubFolders = oldFolder.GetFolders();

            newSubFolders.MoveNext();
            oldSubFolders.MoveNext();

            while (newSubFolders.Current != null && oldSubFolders.Current != null)
            {
                FolderMeta newSubFolder = newSubFolders.Current;
                FolderMeta oldSubFolder = oldSubFolders.Current;

                if (newSubFolder < oldSubFolder)
                {
                    differences.AddNewFolderDifference(newFolder);
                    newSubFolders.MoveNext();
                }

                else if (oldSubFolder < newSubFolder)
                {
                    differences.AddDeletedFolderDifference(oldFolder);
                    oldSubFolders.MoveNext();
                }

                else
                {
                    CompareDirectories(newSubFolder, oldSubFolder, differences);
                    newSubFolders.MoveNext();
                    oldSubFolders.MoveNext();
                }
            }
            while (newSubFolders.Current != null)
            {
                differences.AddNewFolderDifferences(newSubFolders.Current);
                newSubFolders.MoveNext();
            }
            while (oldSubFolders.Current != null)
            {
                differences.AddDeletedFolderDifferences(oldSubFolders.Current);
                oldSubFolders.MoveNext();
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
    }
}
