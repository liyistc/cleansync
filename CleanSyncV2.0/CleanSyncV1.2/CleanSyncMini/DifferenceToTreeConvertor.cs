using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Text.RegularExpressions;

namespace CleanSync
{
    class DifferenceToTreeConvertor
    {
        /*Billy's new methods*/
        private bool ClearInambiguityInFolder(FolderMeta folder, ComponentMeta.Type type)
        {
            bool changeType = false;
            foreach (FileMeta file in folder.files)
            {
                if (file == null) changeType = true;
                else if (file.FileType != type) changeType = true;
            }
            for(int i = 0; i < folder.folders.Count; i++)
            {
                FolderMeta subFolder = folder.folders[i];
                if (subFolder == null) changeType = true;
                else if (subFolder.FolderType == type)
                {
                    if (ClearInambiguityInFolder(subFolder, type)) changeType = true;
                    if (subFolder.FolderType == ComponentMeta.Type.Modified && subFolder.folders.Count == 0 && subFolder.files.Count == 0)
                        folder.folders[i] = null;
                }
                else  //subfolder.FolderType != type 
                {
                    changeType = true;
                }
            }
            if (changeType) folder.FolderType = ComponentMeta.Type.Modified;
            FolderMeta.ClearFolderList(folder.folders);
            FolderMeta.ClearFileList(folder.files);
            return changeType;
        }

        //end new methods


        /*Billy's Modified Methods*/
        private bool ConvertFolderListToTree(FolderMeta root, bool haveDifference, List<FolderMeta> folders)
        {

            if (!haveDifference && folders.Count > 0) haveDifference = true;
            foreach (FolderMeta folder in folders)
            {
                if (folder != null)
                {
                    /*Plugin code*/
                    if(folder.FolderType == ComponentMeta.Type.New || folder.FolderType == ComponentMeta.Type.Deleted)
                        ClearInambiguityInFolder(folder, folder.FolderType); 
                    /*end plugin code*/

                    string[] parents = Regex.Split(folder.Path, SPLITTER);
                    FolderMeta currentFolder = root;
                    string currentPath = @"\";
                    GetToDirectParentFolder(parents, ref currentFolder, ref currentPath);
                    currentFolder.AddFolder(new FolderMeta(folder));
                }
            }
            return haveDifference;
        }
        //end modified methods
        public const string SPLITTER = @"\\";
        public FolderMeta ConvertDifferencesToTreeStructure(Differences difference)
        {
            FolderMeta root = new FolderMeta(@"\", @"", ComponentMeta.Type.NotTouched);
            bool haveDifference = false;

            haveDifference = ConvertFolderListToTree(root, haveDifference, difference.getNewFolderList());
            haveDifference = ConvertFolderListToTree(root, haveDifference, difference.getDeletedFolderList());
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getNewFileList());
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getModifiedFileList());
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getDeletedFileList());
            if (haveDifference) root.FolderType = ComponentMeta.Type.Modified;
            root.sortComponents();
            return root;
        }

        public Differences ConvertTreeStructureToDifferences(FolderMeta root)
        {
            Differences differences = new Differences();
            ConvertTreeStructureToDifferences(root, differences);
            return differences;
        }

        private void ConvertTreeStructureToDifferences(FolderMeta root, Differences differences)
        {
            foreach (FileMeta file in root.files)
            {
                if(file != null)
                switch (file.FileType)
                {
                    case ComponentMeta.Type.New: differences.AddNewFileDifference(file);
                        break;
                    case ComponentMeta.Type.Modified: differences.AddModifiedFileDifference(file);
                        break;
                    case ComponentMeta.Type.Deleted: differences.AddDeletedFileDifference(file);
                        break;
                }
            }
            foreach (FolderMeta folder in root.folders)
            {
                if(folder != null)
                switch (folder.FolderType)
                {
                    case ComponentMeta.Type.New: differences.AddNewFolderDifference(folder);
                        break;
                    case ComponentMeta.Type.Deleted: differences.AddDeletedFolderDifference(folder);
                        break;
                    case ComponentMeta.Type.Modified: ConvertTreeStructureToDifferences(folder, differences);
                        break;
                }
            }
        }

        private static void GetToDirectParentFolder(string[] parents, ref FolderMeta currentFolder, ref string currentPath)
        {
            foreach (string parent in parents)
            {
                if (parent.Equals("")) continue;
                currentFolder = currentFolder.AddAndGetFolder(new FolderMeta(parent, currentPath, ComponentMeta.Type.Modified));
                currentPath = currentPath += parent + @"\";
            }
        }



        private bool ConvertFileListToTree(FolderMeta root, bool haveDifference, List<FileMeta> files)
        {

            if (!haveDifference && files.Count > 0) haveDifference = true;
            foreach (FileMeta file in files)
            {
                if (file != null)
                {
                    string[] parents = Regex.Split(file.Path, SPLITTER);
                    FolderMeta currentFolder = root;
                    string currentPath = @"\";
                    GetToDirectParentFolder(parents, ref currentFolder, ref currentPath);
                    currentFolder.AddFile(new FileMeta(file));
                }
            }
            return haveDifference;
        }


    }
}
