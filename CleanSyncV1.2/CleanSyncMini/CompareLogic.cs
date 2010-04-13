using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Diagnostics;
namespace CleanSync
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

                { 
                    if (newSubFile.LastModifiedTime.CompareTo(oldSubFile.LastModifiedTime)!=0 ||newSubFile.Size != oldSubFile.Size)//modified
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
            while (newSubFoldersNext && oldSubFoldersNext)
            {
                
                FolderMeta newSubFolder = newSubFolders.Current;
                FolderMeta oldSubFolder = oldSubFolders.Current;
                bool flag = newSubFolder.Name.ToLower().CompareTo(oldSubFolder.Name.ToLower()) < 0;
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
               
                differences.AddNewFolderDifference(newSubFolders.Current);
                newSubFoldersNext = newSubFolders.MoveNext();
                
            }
           
            while (oldSubFoldersNext)
            {
                
                differences.AddDeletedFolderDifference(oldSubFolders.Current);
                oldSubFoldersNext = oldSubFolders.MoveNext();
                
            }
        }
//************************************************ConflictDetect***************************************************************

        public List<Conflicts> DetectConflicts(Differences USBFoldersAndFiles, Differences PCFoldersAndFiles)
        {
            List<Conflicts> conflicts = new List<Conflicts>();
            Differences USBDifferences = USBFoldersAndFiles; //create a new copy of Differences to support Cancel.
            Differences PCDifferences  = PCFoldersAndFiles;
            DetectFolderConflict(conflicts, PCDifferences, USBDifferences);
            DetectFileConflict(conflicts, PCDifferences, USBDifferences);
            return conflicts;
        }

        private void DetectFolderConflict(List<Conflicts> conflicts, Differences PCDifferences, Differences USBDifferences)
        {
            DetectDeletedFolderConflictWithDeletedFolderList(PCDifferences.getDeletedFolderList(), USBDifferences.getDeletedFolderList());
            DetectDeletedFolderConflictWithDeletedFolderList(USBDifferences.getDeletedFolderList(), PCDifferences.getDeletedFolderList());
            DetectNewFolderConflictWithNewFolderList(conflicts, PCDifferences.getNewFolderList(), USBDifferences.getNewFolderList());
        }
        private void DetectDeletedFolderConflictWithDeletedFolderList(List<FolderMeta> folderList, List<FolderMeta> baseFolderList)
        {
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta folder = folderList.ElementAt(i);
                if (folder == null) continue;
                FolderMeta folderInUSB = CheckFolderContainedInRootFolder(folder, baseFolderList);
                if (folderInUSB != null && (folderInUSB.Path+ folderInUSB.Name).Equals(folder.Path+ folder.Name))
                {
                    folderList[i] = null;
                    baseFolderList[baseFolderList.IndexOf(folderInUSB, 0, baseFolderList.Count)] = null; 
                    //i--;
                }
                else if (folderInUSB != null && (folderInUSB.Path + folderInUSB.Name).CompareTo(folder.Path + folder.Name) < 0)
                {
                    RemoveFolderFromRootFolder(folderList, folder, folderInUSB);
                }
            }
        }
        private void RemoveFolderFromRootFolder(List<FolderMeta> folderList, FolderMeta folder,FolderMeta baseFolder)
        {
            FolderMeta folderInList = CheckFolderInList(folder, baseFolder.folders);
            if (folderInList != null)
            {
                int index1 = baseFolder.folders.IndexOf(folderInList, 0, baseFolder.folders.Count);
                baseFolder.folders[index1] = null;
                int index2 = folderList.IndexOf(folder, 0, folderList.Count);
                folderList[index2] = null;
                return;
            }
            foreach (FolderMeta subFolderInList in folder.folders)
            {
                if (subFolderInList == null) continue;
                RemoveFolderFromRootFolder(folderList, folder, subFolderInList);
            }
        }
        private void DetectFileConflict(List<Conflicts> conflicts, Differences PCDifferences, Differences USBDifferences)
        {
            //DetectFileConflictWithDeletedFolderList(conflicts,Conflicts.ConflictType.New,PCDifferences.getNewFileList(), USBDifferences.getDeletedFolderList(), Conflicts.FolderFileType.FileVSFolderConflict);
            DetectFileConflictWithFolderList(conflicts, Conflicts.ConflictType.Modified,USBDifferences.getModifiedFileList(), PCDifferences.getDeletedFolderList(), Conflicts.FolderFileType.FolderVSFileConflict);
            DetectFileConflictWithFolderList(conflicts, Conflicts.ConflictType.Modified,PCDifferences.getModifiedFileList(), USBDifferences.getDeletedFolderList(),Conflicts.FolderFileType.FileVSFolderConflict);
            DetectFileConflictWithFolderList(conflicts, Conflicts.ConflictType.Deleted, USBDifferences.getDeletedFileList(), PCDifferences.getDeletedFolderList(), Conflicts.FolderFileType.FolderVSFileConflict);
            DetectFileConflictWithFolderList(conflicts, Conflicts.ConflictType.Deleted, PCDifferences.getDeletedFileList(), USBDifferences.getDeletedFolderList(), Conflicts.FolderFileType.FileVSFolderConflict);
            //DetectFileConflictWithDeletedFolderList(conflicts, Conflicts.ConflictType.New,USBDifferences.getNewFileList(), PCDifferences.getDeletedFolderList(), Conflicts.FolderFileType.FolderVSFileConflict);
            DetectFileConflictInList(conflicts, PCDifferences.getDeletedFileList(), USBDifferences.getModifiedFileList(), Conflicts.ConflictType.Deleted, Conflicts.ConflictType.Modified);
            DetectFileConflictInList(conflicts, PCDifferences.getModifiedFileList(), USBDifferences.getDeletedFileList(), Conflicts.ConflictType.Modified, Conflicts.ConflictType.Deleted);
            DetectFileConflictInList(conflicts, PCDifferences.getModifiedFileList(), USBDifferences.getModifiedFileList(), Conflicts.ConflictType.Modified, Conflicts.ConflictType.Modified);
            DetectFileConflictInList(conflicts, PCDifferences.getNewFileList(), USBDifferences.getNewFileList(), Conflicts.ConflictType.New, Conflicts.ConflictType.New);
            //DetectFileConflictInList(conflicts, PCDifferences.getNewFileList(), USBDifferences.getDeletedFileList(), Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted);
            //DetectFileConflictInList(conflicts, PCDifferences.getDeletedFileList(), USBDifferences.getNewFileList(), Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New);
           //DetectFileConflictInList(conflicts, PCDifferences.getNewFileList(), USBDifferences.getModifiedFileList(), Conflicts.ConflictType.New, Conflicts.ConflictType.Modified);
            //DetectFileConflictInList(conflicts, PCDifferences.getModifiedFileList(), USBDifferences.getNewFileList(), Conflicts.ConflictType.Modified, Conflicts.ConflictType.New);
            DetectFileConflictInList(conflicts, PCDifferences.getDeletedFileList(), USBDifferences.getDeletedFileList(), Conflicts.ConflictType.Deleted, Conflicts.ConflictType.Deleted);
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
                    int index = baseList.IndexOf(fileInBaseList, 0, baseList.Count);
                    if (fileType == Conflicts.ConflictType.Deleted && baseFileType == Conflicts.ConflictType.Deleted)
                    {
                        fileList[i] = null;
                        baseList[index] = null;
                    }
                    else if (fileType == Conflicts.ConflictType.New && baseFileType == Conflicts.ConflictType.New)
                    {
                        if (file.Size != fileInBaseList.Size || file.LastModifiedTime.CompareTo(fileInBaseList.LastModifiedTime) != 0)
                        {
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.FileConflict, file, fileInBaseList, fileType, baseFileType));
                        }
                        else
                        { 
                            baseList[index] = null;
                            fileList[i] = null;
                        }
                    }
                    else
                    {
                        conflicts.Add(new Conflicts(Conflicts.FolderFileType.FileConflict, file, fileInBaseList, fileType, baseFileType));
                    }
                }
            }
        }
  /*     
        private void DetectNewFolderConflictWithDeletedFolderList(List<Conflicts> conflicts, List<FolderMeta> newFolderList, List<FolderMeta> deletedFolderList, Conflicts.FolderFileType type)
        {
            for (int i = 0; i < newFolderList.Count; i++)
            {
                FolderMeta newFolder  = newFolderList.ElementAt(i);
                FolderMeta folder = CheckFolderContainedInRootFolder(newFolder, deletedFolderList);
                if (folder != null){
                    switch (type)
                    {
                        case Conflicts.FolderFileType.SubFolderVSFolderConflict:
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.SubFolderVSFolderConflict, newFolder, folder, Conflicts.ConflictType.New, Conflicts.ConflictType.Deleted));
                            break;
                        case Conflicts.FolderFileType.FolderVSSubFolderConflict:
                            conflicts.Add(new Conflicts(Conflicts.FolderFileType.FolderVSSubFolderConflict, folder, newFolder, Conflicts.ConflictType.Deleted, Conflicts.ConflictType.New));
                            break;
                    }
                }
            }
        }*/

        private static FolderMeta CheckFolderContainedInRootFolder(FolderMeta folderIn, List<FolderMeta> folderList)
        {
            string folderKey = folderIn.Path + folderIn.Name;
            FolderMeta folder = null;
            foreach (FolderMeta folderInList in folderList)
            {
                if (folderInList == null) continue;
                string folderInListKey = folderInList.Path + folderInList.Name;
                if (folderKey.Contains(folderInListKey+@"\") ||folderKey.Equals(folderInListKey) ) //conflict---- creating new folder inside a deleted folder
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
                if (key.Equals(fileToBeChecked.Path + fileToBeChecked.Name))
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
        }//end of new method
        //xiaoQ modified method
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
                    string deletedFolderKey = deletedFolder.Path + deletedFolder.Name + @"\";
                    if (fileKey.Contains(deletedFolderKey)) //conflict---- creating new folder inside a deleted folder
                    {
                        folder = deletedFolder;
                        break;
                    }
                }
                if (folder != null && type == Conflicts.ConflictType.Modified)
                {
                    if (!CheckFileInFolder(file, folder)) return;
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
                    RemoveFileFromDeletedListAndDeletedFolder(fileList, file, folder);
                }
            }
        }//end of modified method
        private void RemoveFileFromDeletedListAndDeletedFolder(List<FileMeta>fileList, FileMeta file, FolderMeta folder)
        {
           // bool removed = false;
            FileMeta fileInList = CheckFileInList(file, folder.files);
            if (fileInList != null)
            {
                folder.files[folder.files.IndexOf(fileInList,0,folder.files.Count)]=null;
                fileList[fileList.IndexOf(file,0,fileList.Count)]=null;
                return;
            }
            foreach (FolderMeta folderInList in folder.folders)
            {
                if(folderInList ==null) continue;
                RemoveFileFromDeletedListAndDeletedFolder(fileList, file, folderInList);
            }
           // return removed;
        }
        private void DetectNewFolderConflictWithNewFolderList(List<Conflicts> conflicts, List<FolderMeta> PCFolderList,  List<FolderMeta> USBFolderList)
        { 
            for (int i = 0; i < PCFolderList.Count; i++)
            {
                FolderMeta PCFolder = PCFolderList.ElementAt(i);
                if (PCFolder == null) continue;
                FolderMeta USBFolder = CheckFolderInList(PCFolder, USBFolderList);
                if (USBFolder != null)
                {
                    List<FileMeta> PCFiles = PCFolder.files;
                    List<FileMeta> USBFiles = USBFolder.files;
                    List<FolderMeta> PCFolders = PCFolder.folders;
                    List<FolderMeta> USBFolders = USBFolder.folders;
                    DetectFileConflictInList(conflicts, PCFiles, USBFiles, Conflicts.ConflictType.New, Conflicts.ConflictType.New);
                    DetectNewFolderConflictWithNewFolderList(conflicts, PCFolders, USBFolders);
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


        /*
        private void DetectNewFolderConflictWithNewFolderList(List<Conflicts> conflicts, Differences PCDifferences, Differences USBDifferences)
        {
            List<FolderMeta> PCFolderList = PCDifferences.getNewFolderList();
            List<FolderMeta> USBFolderList = USBDifferences.getNewFolderList();
            for (int i = 0; i < PCFolderList.Count; i++)
            {
                FolderMeta PCFolder = PCFolderList.ElementAt(i);
                FolderMeta USBFolder = CheckFolderInList(PCFolder, USBFolderList);
                if (USBFolder != null)
                {
                    List<FileMeta> PCFiles = PCFolder.files;
                    List<FolderMeta> PCFolders = PCFolder.folders;
                    if (PCFiles.Count != 0) AppendFileListToFileList(PCFiles, PCDifferences.getNewFileList());
                    if (PCFolders.Count != 0) AppendFolderListToFolderList(PCFolders, PCFolderList);
                    List<FileMeta> USBFiles = USBFolder.files;
                    List<FolderMeta> USBFolders = USBFolder.folders;
                    if (USBFiles.Count != 0) AppendFileListToFileList(USBFiles, USBDifferences.getNewFileList());
                    if (USBFolders.Count != 0) AppendFolderListToFolderList(USBFolders, USBFolderList);
                }
            }
        }

        private void AppendFileListToFileList(List<FileMeta> fileList, List<FileMeta> baseList)
        {
            foreach (FileMeta file in fileList)
            {
                baseList.Add(file);
            }
        }

        private void AppendFolderListToFolderList(List<FolderMeta> folderList, List<FolderMeta> baseList)
        {
            foreach (FolderMeta folder in folderList)
            {
                baseList.Add(folder);
            }
        }*/
        private FileMeta CheckFileInList(FileMeta fileToBeChecked, List<FileMeta> fileList)
        {
            FileMeta fileDetected = null;
            string fileToBeCheckedInfo = fileToBeChecked.Path + fileToBeChecked.Name;
            foreach (FileMeta file in fileList)
            {
                if (file == null) continue;               
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
                if (folder == null) continue;
                // MessageBox.Show(folderToBeChecked.Path+folderToBeChecked.Name+"\n"+ folder.Path+folder.Name);
                if (folderToBeCheckedInfo.Equals(folder.Path + folder.Name))
                {
                    folderDetected = folder;
                    break;
                }
                
            }
            return folderDetected;
        }
       
   /**********************************************************end of conflict detection************************************************/     

       
        
        
   /*
       
        public FolderMeta ConvertDifferencesToTreeStructure(FolderMeta root, Differences diff)
        {
            FolderMeta diffRoot = new FolderMeta(root.AbsolutePath, root.rootDir);
            foreach (FolderMeta folder in diff.getDeletedFolderList())
                diffRoot = ConstructFolderPath(diffRoot, folder, folder.Path);
            foreach (FolderMeta folder in diff.getNewFolderList())
                diffRoot = ConstructFolderPath(diffRoot, folder, folder.Path);
            foreach (FileMeta file in diff.getNewFileList())
                diffRoot = ConstructFilePath(diffRoot, file, file.Path);
            foreach (FileMeta file in diff.getDeletedFileList())
                diffRoot = ConstructFilePath(diffRoot, file, file.Path);
            foreach (FileMeta file in diff.getModifiedFileList())
                diffRoot = ConstructFilePath(diffRoot, file, file.Path);
            return diffRoot;
        }
        
        private FolderMeta ConstructFolderPath(FolderMeta diffRoot, FolderMeta folder, string relativePath)
        {
            string nextLevel = relativePath.Substring(relativePath.IndexOf(@"\") + 1);
            if (nextLevel.Length == 0)
            {
                diffRoot.AddFolder(folder);
            }
            else
            {
                string folderName = nextLevel.Substring(0, nextLevel.IndexOf(@"\"));
                FolderMeta newFolder = new FolderMeta(folder.AbsolutePath.Replace(relativePath + folder.Name, "") + @"\" + folderName, folder.rootDir);
                newFolder.FolderType = ComponentMeta.Type.Modified;
                newFolder = ConstructFolderPath(newFolder, folder, nextLevel.Substring(nextLevel.IndexOf(@"\")));
                diffRoot.AddFolder(newFolder);

            }

            return diffRoot;
        }

        private FolderMeta ConstructFilePath(FolderMeta diffRoot, FileMeta file, string relativePath)
        {
            string nextLevel = relativePath.Substring(relativePath.IndexOf(@"\") + 1);
            if (nextLevel.Length == 0)
            {
                diffRoot.AddFile(file);
            }
            else
            {
                string folderName = nextLevel.Substring(0, nextLevel.IndexOf(@"\"));
                FolderMeta newFolder = new FolderMeta(file.AbsolutePath.Replace(relativePath + file.Name, "") + @"\" + folderName, file.rootDir);
                newFolder.FolderType = ComponentMeta.Type.Modified;
                newFolder = ConstructFilePath(newFolder, file, nextLevel.Substring(nextLevel.IndexOf(@"\")));
                diffRoot.AddFolder(newFolder);
            }
            return diffRoot;
        }
*/


      
        
        
        
        /*       public FolderMeta ConvertDifferencesToTreeStructure(FolderMeta root, Differences differencesIn)
               {
                   Differences differences = new Differences(differencesIn);
                   List<FileMeta>   subFiles   = root.files;
                   List<FolderMeta> subFolders = root.folders;
                   List<FileMeta> deletedFileList = differences.getDeletedFileList();
                   List<FileMeta> newFileList = differences.getNewFileList();
                   List<FileMeta> modifiedFileList = differences.getModifiedFileList();
                   List<FolderMeta> deletedFolderList = differences.getDeletedFolderList();
                   List<FolderMeta> newFolderList = differences.getNewFolderList();
           
                   //RemoveUntouchedFilesFromRootFileList(root, differences, subFiles);

                   ClearFileList(root, newFileList, ComponentMeta.Type.New);
                   ClearFileList(root, modifiedFileList, ComponentMeta.Type.Modified);
                   ClearFileList(root, deletedFileList, ComponentMeta.Type.Deleted);
                   ClearFolderList(root, deletedFolderList, ComponentMeta.Type.Deleted);
                   ClearFolderList(root, newFolderList, ComponentMeta.Type.New);
                   ConvertSubFolders(root, differences, subFolders);
                   LabelFolderList(subFolders);
                   ClearRootUnTouchedFiles(root);
                   ClearRootUnTouchedFolders(root);
                   return root;
               }
       
        private void ConvertSubFolders(FolderMeta root, Differences differences, List<FolderMeta> subFolders)
        {
            foreach (FolderMeta folder in subFolders)
            {
                if (folder.FolderType == ComponentMeta.Type.NotTouched)
                {
                    ConvertDifferencesToTreeStructure(folder, differences);
                    if (folder.FolderType != ComponentMeta.Type.NotTouched)
                        root.FolderType = ComponentMeta.Type.Modified;
                }
                else
                {
                    if (root.FolderType != ComponentMeta.Type.Modified)
                        root.FolderType = ComponentMeta.Type.Modified;
                }

            }
        }
      
        private void AddDeletedFilesToRootFileList(FolderMeta root, List<FileMeta> deletedFileList)
        {
            for (int i = 0; i < deletedFileList.Count; i++)
            {
                FileMeta file = deletedFileList.ElementAt(i);
                //Efficiency ready to be improved
                if (file.Path.Equals(root.Path) || file.Path.Equals(root.Path + root.Name + @"\"))
                {
                    file.FileType = ComponentMeta.Type.Deleted;
                    FileMeta fileDeleted = CheckFileInList(file, root.files);
                    if (fileDeleted == null) 
                    {
                        root.AddFile(file); 
                    }
                    else
                    {
                        fileDeleted.FileType = ComponentMeta.Type.Deleted;
                    }
                    if (root.FolderType != ComponentMeta.Type.Modified)
                        root.FolderType = ComponentMeta.Type.Modified;
                }

            }
        }

       private void RemoveUntouchedFilesFromRootFileList(FolderMeta root, Differences differences, List<FileMeta> subFiles)
        {
            for (int i = 0; i < subFiles.Count; i++)
            {
                FileMeta file = subFiles.ElementAt(i);
                if (!this.checkFileExistence(file, differences))
                {
                    RemoveFile(file, root.files);
                    //i--;
                }
                else
                {
                    if (root.FolderType != ComponentMeta.Type.Modified)
                        root.FolderType = ComponentMeta.Type.Modified;
                }
            }
        }
        
        private  void ClearRootUnTouchedFolders(FolderMeta root)
        {
            for (int i = 0; i < root.folders.Count; i++)
            {
                FolderMeta folder = root.folders.ElementAt(i);
                if (folder.FolderType == ComponentMeta.Type.NotTouched)
                {
                    i--;
                    root.folders.Remove(folder);
                }
            }
        }

        private void ClearRootUnTouchedFiles(FolderMeta root)
        {
            for (int i = 0; i < root.files.Count; i++)
            {
                FileMeta file = root.files.ElementAt(i);
                if (file.FileType == ComponentMeta.Type.NotTouched)
                {
                    i--;
                    root.files.Remove(file);
                }
            }
        }

        private void ClearFileList(FolderMeta root, List<FileMeta> fileList, ComponentMeta.Type type)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList.ElementAt(i);
                if (file.Path.Equals(root.Path+ root.Name)|| file.Path.Equals(root.Path + root.Name + @"\"))
                {
                    FileMeta fileToBeChecked = CheckFileInList(file, root.files);
                    if (fileToBeChecked == null)
                    {
                        file.FileType = type;
                        root.AddFile(file);
                    }
                    else
                    {
                        fileToBeChecked.FileType = type;
                    }
                    if (root.FolderType != ComponentMeta.Type.Modified)
                    {

                        root.FolderType = ComponentMeta.Type.Modified;
                    }
                    fileList.Remove(file);
                    i--;
                }
            }
        }

    

        private void ClearFolderList(FolderMeta root, List<FolderMeta> folderList, ComponentMeta.Type type)
        {
            
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta folder = folderList.ElementAt(i);
                if (folder.Path.Equals(root.Path+ root.Name)||folder.Path.Equals(root.Path + root.Name + @"\"))
                {
                    FolderMeta folderToBeChecked = CheckFolderInList(folder,root.folders);
                    if (folderToBeChecked == null)
                    {
                        folder.FolderType = type;
                        root.AddFolder(folder);
                    }
                    else folderToBeChecked.FolderType = type;
                
                    if (root.FolderType != ComponentMeta.Type.Modified)
                    {
                        root.FolderType = ComponentMeta.Type.Modified;
                    }
                    folderList.Remove(folder);
                    i--;
                }
            }
        }
        private void LabelSubFoldersAndFiles(FolderMeta rootFolder, ComponentMeta.Type type)
        {
            List<FileMeta> fileList = rootFolder.files;
            List<FolderMeta> folderList = rootFolder.folders;
            foreach (FileMeta file in fileList)
            {
                file.FileType = type;
            }
            foreach (FolderMeta folder in folderList)
            {
                folder.FolderType = type;
                LabelSubFoldersAndFiles(folder, type);
            }
        }
              private bool checkFileExistence(FileMeta file, Differences differences)
        {
            //check the file exists in Differences or not
            bool fileExistence = true;
            List<FileMeta> newFileList = differences.getNewFileList();
            List<FileMeta> modifiedFileList = differences.getModifiedFileList();
            if (CheckFileInList(file, newFileList) ==null&& CheckFileInList(file, modifiedFileList) ==null)
            {
                fileExistence = false;
                // Console.WriteLine( "FileAbpath: "+ file.AbsolutePath + " not exists");
            }
            else if (CheckFileInList(file, newFileList) != null)
            {
                file.FileType = ComponentMeta.Type.New;
                //newFileList.Remove(file);
                // Console.WriteLine("File: " + file.AbsolutePath + " is a new file");
            }
            else if (CheckFileInList(file, modifiedFileList) !=null)
            {
                file.FileType = ComponentMeta.Type.Modified;
                //modifiedFileList.Remove(file);
                //Console.WriteLine("File: "+ file.AbsolutePath +" is a modified file");
            }
            return fileExistence;
        }

        private bool checkFolderExistence(FolderMeta folder, Differences differences)
        {
            bool folderExistence = true;
            List<FolderMeta> newFolderList = differences.getNewFolderList();

            if (CheckFolderInList(folder, newFolderList) == null)
            {
                folderExistence = false;
            }
            else
            {
                folder.FolderType = ComponentMeta.Type.New;
                //newFolderList.Remove(folder);
            //    Console.WriteLine("Folder: "+ folder.AbsolutePath +" is a new folder");
            }

            return folderExistence;

        }
          private void RemoveFile(FileMeta fileToBeRemoved, List<FileMeta> fileList)
        {
            string fileToBeRemovedInfo = fileToBeRemoved.Path + fileToBeRemoved.Name;
            foreach(FileMeta file in fileList)
            {
                if (fileToBeRemovedInfo.Equals(file.Path + file.Name))
                {
                    fileList.Remove(file);
                    break;
                }
            }
        }
        private void RemoveFolder(FolderMeta folderToBeRemoved, List<FolderMeta> folderList)
        {
            string folderToBeRemovedInfo = folderToBeRemoved.Path + folderToBeRemoved.Name;
            foreach (FolderMeta folder in folderList)
            {
                if (folderToBeRemovedInfo.Equals(folder.Path + folder.Name))
                {
                    folderList.Remove(folder);
                    break;
                }
            }
        }
        

        private void LabelFolderList(List<FolderMeta> subFolders)
        {
            foreach (FolderMeta folderToBeLabelled in subFolders)
            {
                if (folderToBeLabelled.FolderType == ComponentMeta.Type.New || folderToBeLabelled.FolderType == ComponentMeta.Type.Deleted)
                {
                    LabelSubFoldersAndFiles(folderToBeLabelled, folderToBeLabelled.FolderType);
                }
            }
        } */


/********************************************************some test methods******************************************************/
  /*      public void CheckDifferences(Differences differences)
        {
            if (differences.getDeletedFileList().Count == 0)
                Console.WriteLine("Differences status test: DeletedFileList empty");
            else
            {
                foreach (FileMeta file in differences.getDeletedFileList())
                    Console.WriteLine("DeletedFiles exists: " + file.AbsolutePath);
            }
            if (differences.getNewFileList().Count == 0)
                Console.WriteLine("Differences status test: NewFileList empty");
            else
            {
                foreach (FileMeta file in differences.getNewFileList())
                    Console.WriteLine("NewFiles exists: " + file.AbsolutePath);
            }
            if (differences.getModifiedFileList().Count == 0)
                Console.WriteLine("Differences status test: ModifiedFileList empty");
            else
            {
                foreach (FileMeta file in differences.getModifiedFileList())
                    Console.WriteLine("ModifiedFiles exists: " + file.AbsolutePath);
            }
            if (differences.getDeletedFolderList().Count == 0)
                Console.WriteLine("Differences status test: DeletedFolderList empty");
            else
            {
                foreach (FolderMeta folder in differences.getDeletedFolderList())
                    Console.WriteLine("DeletedFolders exists: " + folder.AbsolutePath);
            }
            if (differences.getNewFolderList().Count == 0)
                Console.WriteLine("Differences status test: NewFolderList empty");
            else
            {
                foreach (FolderMeta folder in differences.getNewFolderList())
                    Console.WriteLine("NewFolders exists: " + folder.AbsolutePath);
            }

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

        }*/

        /*
        public static void printResultFolder(List<FolderMeta> folderList)
        {
            foreach( FolderMeta folder in folderList ){
                if(folder != null)
                Console.WriteLine("   "+folder.Name);
            }

        }
        public static void printResultFile(List<FileMeta> fileList)
        {

            foreach (FileMeta file in fileList)
            {
                if(file !=null)   
                Console.WriteLine("   "+file.Name);
            }

        }
        public static void printResultConflicts(List<Conflicts> conflictList)
        {
            foreach (Conflicts conflict in conflictList)
            {
                if(conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FolderConflict)
                {
                    Console.WriteLine("PCFodler: " + conflict.CurrentPCFolder.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n"+" conflicts with USBFolder: " +"\n"+ conflict.USBFolder.AbsolutePath + "[" + conflict.USBFolderFileType + "]");

                }
                else if (conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FileConflict)
                {
                    Console.WriteLine("PCFile: " + conflict.CurrentPCFile.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n" + " conflicts with USBFile: " + "\n" + conflict.USBFile.AbsolutePath + "[" + conflict.USBFolderFileType + "]");

                }
                else if (conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FileVSFolderConflict)
                {
                    Console.WriteLine("PCFile: " + conflict.CurrentPCFile.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n"+" conflicts with USBFolder: " +"\n"+ conflict.USBFolder.AbsolutePath + "[" + conflict.USBFolderFileType + "]");
                }
                else if(conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FolderVSFileConflict)
                {
                    Console.WriteLine("PCFolder: " + conflict.CurrentPCFolder.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n" + " conflicts with USBFile: " + "\n" + conflict.USBFile.AbsolutePath + "[" + conflict.USBFolderFileType + "]");
                }
                else if (conflict.FolderOrFileConflictType == Conflicts.FolderFileType.FolderVSSubFolderConflict)
                {
                    Console.WriteLine("PCFolder: " + conflict.CurrentPCFolder.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n" + " conflicts with USBFile: " + "\n" + conflict.USBFolder.AbsolutePath + "[" + conflict.USBFolderFileType + "]");
                }
                else if (conflict.FolderOrFileConflictType == Conflicts.FolderFileType.SubFolderVSFolderConflict)
                {
                    Console.WriteLine("PCFolder: " + conflict.CurrentPCFolder.AbsolutePath + "[" + conflict.PCFolderFileType + "]" + "\n" + " conflicts with USBFile: " + "\n" + conflict.USBFolder.AbsolutePath + "[" + conflict.USBFolderFileType + "]");
                }
               
            }

        }

        //test code for comparePCwithUSB
        public void ComaprePCwithUSBTest(ComparisonResult comparisonResult)
        {   Differences USBDifferences = comparisonResult.USBDifferences;
            Differences PCDifferences = comparisonResult.PCDifferences;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            Console.WriteLine("PCtwoDifferences:");
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
            Console.WriteLine("PConeDifferences:");
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
   */
    }

        
}
