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

        public ComponentMeta(string path, string rootDir)
        {
            path = System.IO.Path.GetFullPath(path).Substring(rootDir.Length);
            Name = System.IO.Path.GetFileName(path);
            Path = path.Substring(0, (path.Length - Name.Length));
            Console.WriteLine(Path + "\n" + Name);
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

        public int SizeKiloByte
        {
            get;
            set;
        }

        public FileMeta(string path, string rootDir) : base(path, rootDir)
        {
            FileInfo fileInfo = new FileInfo(path);
            CreationTime = fileInfo.CreationTime;
            LastModifiedTime = fileInfo.LastAccessTime;
            SizeKiloByte = ConvertToKiloByte(fileInfo);
        }

        public static int ConvertToKiloByte(FileInfo fileInfo)
        {
            return (int)(fileInfo.Length * 0.0009765625);
        }

        public string getString()
        {
            return this.Path +"\n\t" + this.Name + "\n";
        }
    }

    [Serializable]
    public class FolderMeta : ComponentMeta
    {

        private LinkedList<FolderMeta> folders;
        private LinkedList<FileMeta> files;

        public FolderMeta(string path, string rootDir)
            : base(path, rootDir)
        {
            folders = new LinkedList<FolderMeta>();
            files = new LinkedList<FileMeta>();
        }

        public void AddFile(FileMeta file)
        {
            files.AddLast(file);
        }

        public void AddFolder(FolderMeta folder)
        {
            folders.AddLast(folder);
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
            stringVer += this.Path + "\n\t" + this.Name + "\n";
            while(file.MoveNext()) stringVer += file.Current.getString()+"\n";
            while (folder.MoveNext())
            {
                stringVer += folder.Current.getString();
            }
            return stringVer;
        }
    }

}
