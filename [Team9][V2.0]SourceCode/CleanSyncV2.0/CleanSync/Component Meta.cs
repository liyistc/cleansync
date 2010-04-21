/***********************************************************************
 * 
 * *****************CleanSync Version 2.0 Component Meta*****************
 * 
 * Written By : Lv Wenhao
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
using System.IO;

namespace DirectoryInformation
{
    [Serializable]
    public abstract class ComponentMeta
    {
        public enum Type
        {
            New,
            Modified,
            Deleted,
            NotTouched
        }
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }
        public string AbsolutePath
        {
            get;
            set;
        }

        public string rootDir
        {
            get;
            set;
        }

        public ComponentMeta()
        {
            this.Name = "";
            this.Path = "";
            this.AbsolutePath = "";
        }

        public ComponentMeta(string name, string path, string absolutePath)
        {
            this.Name = name;
            this.Path = path;
            this.AbsolutePath = AbsolutePath;
        }

        public ComponentMeta(string path, string rootDir)
        {
            this.rootDir = rootDir;
            AbsolutePath = path;
            path = System.IO.Path.GetFullPath(path).Substring(rootDir.Length);
            Name = System.IO.Path.GetFileName(path);
            Path = path.Substring(0, (path.Length - Name.Length));
            if (Path.IndexOf('\\') != 0) Path = @"\" + Path;
            //Console.WriteLine(Path + "\n" + Name);
        }
		public ComponentMeta(ComponentMeta root)
        {
            this.Name = root.Name;
            this.Path = root.Path;
            this.AbsolutePath = root.AbsolutePath;
            this.rootDir = root.rootDir;
        }

        #region ClearingListSpaces
        public static void ClearFolderList(List<FolderMeta> folderList)
        {
            for (int i = 0; i < folderList.Count; i++)
            {
                FolderMeta folder = folderList[i];
                if (folder == null)
                {
                    folderList.RemoveAt(i);
                    i--;
                }
            }
        }
        public static void ClearFileList(List<FileMeta> fileList)
        {
            for (int i = 0; i < fileList.Count; i++)
            {
                FileMeta file = fileList[i];
                if (file == null)
                {
                    fileList.RemoveAt(i);
                    i--;
                }
            }
        }
        #endregion

        public static bool operator >(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.ToLower().CompareTo(second.Name.ToLower()) > 0;
        }

        public static bool operator <(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.ToLower().CompareTo(second.Name.ToLower()) < 0;
        }

    }

    

    

}
