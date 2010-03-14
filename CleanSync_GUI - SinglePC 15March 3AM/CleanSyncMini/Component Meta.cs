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

        public static bool operator >(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.CompareTo(second.Name) > 0;
        }
        
        public static bool operator <(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.CompareTo(second.Name) < 0;
        }
       
    }

    [Serializable]
    public class FileMeta : ComponentMeta{

        public Type FileType
        {
            set;
            get;
        }
        public DateTime LastModifiedTime
        {
            get;
            set;
        }
        
        public DateTime CreationTime
        {
            get;
            set;
        }

        public long Size
        {
            get;
            set;
        }

        public FileMeta(string path, string rootDir) : base(path, rootDir)
        {
            FileInfo fileInfo = new FileInfo(path);
            CreationTime = fileInfo.CreationTime;
            LastModifiedTime = fileInfo.LastWriteTime;
            //SizeKiloByte = ConvertToKiloByte(fileInfo);
            Size = fileInfo.Length;
            FileType = ComponentMeta.Type.NotTouched;
        }

        public FileMeta(FileMeta file) : base(file.AbsolutePath,file.rootDir)
        {
            this.FileType = file.FileType;
            this.CreationTime = file.CreationTime;
            this.LastModifiedTime = file.LastModifiedTime;
            this.Size = file.Size;
        }

        public static int ConvertToKiloByte(FileInfo fileInfo)
        {
            return (int)(fileInfo.Length * 0.0009765625);
        }

        public string getString()
        {
            return this.Path + "\n\t" + this.Name + "(" + this.FileType + ")" + "\n";
        }
    }

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
        public  List<FileMeta> files
        {
            set;
            get;
        }
        public long Size
        {
            set;
            get;
        }
        public FolderMeta(string path, string rootDir)
            : base(path, rootDir)
        {
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
            FolderType = ComponentMeta.Type.NotTouched;
        }

        public FolderMeta(FolderMeta root):base(root.AbsolutePath,root.rootDir)
        {
            folders = new List<FolderMeta>();
            files = new List<FileMeta>();
            this.FolderType = root.FolderType;
            this.Size = root.Size;
            
            foreach (FolderMeta folder in root.folders)
            {
                this.folders.Add(new FolderMeta(folder));
            }
            foreach (FileMeta file in root.files)
            {
                this.files.Add(new FileMeta(file));
            }
        }


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

        public IEnumerator<FileMeta> GetFiles()
        {
            return files.GetEnumerator();
        }

        public String getString()
        {
            IEnumerator<FileMeta> file = this.GetFiles();
            IEnumerator<FolderMeta> folder = this.GetFolders();
            
            String stringVer = "";
            stringVer += this.Path + "\n\t" + this.Name + "("+this.FolderType+")"+"\n";
            while(file.MoveNext()) stringVer += file.Current.getString()+"\n";
            while (folder.MoveNext())
            {
                stringVer += folder.Current.getString();
            }
            return stringVer;
        }
    }

}
