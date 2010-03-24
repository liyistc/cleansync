using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.Text.RegularExpressions;

namespace CleanSyncMini
{
    class DifferenceToTreeConvertor
    {
        public const string SPLITTER = @"\\";
        public FolderMeta ConvertDifferencesToTreeStructure(Differences difference)
        {
            FolderMeta root = new FolderMeta(@"\", @"",ComponentMeta.Type.NotTouched);
            bool haveDifference = false;

            haveDifference = ConvertFolderListToTree(root, haveDifference, difference.getNewFolderList(), ComponentMeta.Type.New);
            haveDifference = ConvertFolderListToTree(root, haveDifference, difference.getDeletedFolderList(), ComponentMeta.Type.Deleted);
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getNewFileList(), ComponentMeta.Type.New);
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getModifiedFileList(), ComponentMeta.Type.Modified);
            haveDifference = ConvertFileListToTree(root, haveDifference, difference.getDeletedFileList(), ComponentMeta.Type.Deleted);
            if (haveDifference) root.FolderType = ComponentMeta.Type.Modified;
            return root;
        }

        private bool ConvertFolderListToTree(FolderMeta root, bool haveDifference, List<FolderMeta> folders, ComponentMeta.Type type)
        {

            if (!haveDifference && folders.Count > 0) haveDifference = true;
            foreach (FolderMeta folder in folders)
            {
                folder.FolderType = type;
                string[] parents = Regex.Split(folder.Path, SPLITTER);
                FolderMeta currentFolder = root;
                string currentPath = @"\";
                GetToDirectParentFolder(parents, ref currentFolder, ref currentPath);
                currentFolder.AddFolder(new FolderMeta(folder));
            }
            return haveDifference;
        }
        private bool ConvertFileListToTree(FolderMeta root, bool haveDifference, List<FileMeta> files, ComponentMeta.Type type)
        {

            if (!haveDifference && files.Count > 0) haveDifference = true;
            foreach (FileMeta file in files)
            {
                file.FileType = type; 
                string[] parents = Regex.Split(file.Path, SPLITTER);
                FolderMeta currentFolder = root;
                string currentPath = @"\";
                GetToDirectParentFolder(parents, ref currentFolder, ref currentPath);
                currentFolder.AddFile(new FileMeta(file));
            }
            return haveDifference;
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

    }
}
