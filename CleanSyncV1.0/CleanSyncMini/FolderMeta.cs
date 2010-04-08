using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    [Serializable]
    public class FolderMeta : ComponentMeta
    {
        public Type FolderType
        {
            set;
            get;
        }

        public List<FolderMeta> folders
        {
            set;
            get;
        }
        public List<FileMeta> files
        {
            set;
            get;
        }
        public long Size
        {
            set;
            get;
        }

        public FolderMeta() :base()
        {
            FolderType = ComponentMeta.Type.NotTouched;
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
        }

        public FolderMeta(string name, string path, Type folderType)
            : base(name, path, "")
        {
            this.FolderType = folderType;
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
        }

        public FolderMeta(string path, string rootDir)
            : base(path, rootDir)
        {
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
            FolderType = ComponentMeta.Type.NotTouched;
            //updateFolderSize();
        }



        public FolderMeta(FolderMeta root)
            : base(root.AbsolutePath, root.rootDir)
        {
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
            this.FolderType = root.FolderType;
            this.Size = root.Size;

            foreach (FolderMeta folder in root.folders)
            {
                if (folder != null)
                    this.folders.Add(new FolderMeta(folder));
                else
                    this.folders.Add(null);
            }
            foreach (FileMeta file in root.files)
            {
                if (file != null)
                    this.files.Add(new FileMeta(file));
                else this.files.Add(null);
            }
        }



        /*
        private void updateFolderSize()
        {
            foreach (FolderMeta folder in this.folders)
            {
                this.Size += folder.Size;
            }
            foreach (FileMeta file in this.files)
            {
                this.Size += file.Size;
            }
        }
         */

        public void AddFile(FileMeta file)
        {
            files.Add(file);
        }

        public void AddFolder(FolderMeta folder)
        {
            folders.Add(folder);
        }

        public IEnumerator<FolderMeta> GetFolders()
        {
            return folders.GetEnumerator();
        }

        public FolderMeta AddAndGetFolder(FolderMeta folder)
        {
            IEnumerator<FolderMeta> folders = this.GetFolders();
            while (folders.MoveNext())
            {
                if (folders.Current == null) continue;
                if (folder.Path + folder.Name == folders.Current.Path + folders.Current.Name) return folders.Current;
            }
            this.AddFolder(folder);
            return folder;
        }

        public IEnumerator<FileMeta> GetFiles()
        {
            return files.GetEnumerator();
        }

        public String getString()
        {
            IEnumerator<FileMeta> file = this.GetFiles();
            IEnumerator<FolderMeta> folder = this.GetFolders();

            String stringVer = "";
            stringVer += this.Path + "\n\t" + this.Name + "(" + this.FolderType + ")" + "\n";
            while (file.MoveNext()) stringVer += file.Current.getString() + "\n";
            while (folder.MoveNext())
            {
                stringVer += folder.Current.getString();
            }
            return stringVer;
        }
    }
}
