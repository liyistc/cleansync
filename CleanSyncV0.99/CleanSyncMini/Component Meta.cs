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

        public static bool operator >(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.CompareTo(second.Name) > 0;
        }

        public static bool operator <(ComponentMeta first, ComponentMeta second)
        {
            return first.Name.CompareTo(second.Name) < 0;
        }

    }

    

    

}
