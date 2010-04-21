/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 FileMeta*******************
 * 
 * Written By : Lv Wenhao & Li Zichen
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
    public class FileMeta : ComponentMeta
    {

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

        public FileMeta() : base()
        {
            CreationTime = DateTime.Now;
            LastModifiedTime = DateTime.Now;
            Size = 0;
            FileType = ComponentMeta.Type.NotTouched;
        }

        public FileMeta(string path, string rootDir)
            : base(path, rootDir)
        {
            FileInfo fileInfo = new FileInfo(path);
            CreationTime = fileInfo.CreationTimeUtc;
            LastModifiedTime = fileInfo.LastWriteTimeUtc;
            Size = fileInfo.Length;
            FileType = ComponentMeta.Type.NotTouched;
        }

        public FileMeta(FileMeta file)
            : base(file.AbsolutePath, file.rootDir)
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
}
