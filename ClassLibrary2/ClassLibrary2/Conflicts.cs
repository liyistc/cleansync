using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    public class Conflicts
    {

        public enum ConflictType
        {
            FolderConflict,
            FileConflict
        }

        public Conflicts(FolderMeta currentPCFolder, FolderMeta USBFolder, ConflictType type)
        {
            this.currentPCFolder = currentPCFolder;
            this.USBFolder  = USBFolder;
            this.type = type;
            
        }
        public Conflicts(FileMeta currentPCFile, FileMeta USBFile,ConflictType type)
        {
            this.currentPCFile = currentPCFile;
            this.USBFile = USBFile;
            this.type = type;

        }

        public FolderMeta currentPCFolder
        {
            get;
            set;
        }
        public FolderMeta USBFolder
        {
            get;
            set;
        }
        public FileMeta currentPCFile
        {
            get;
            set;
        }
        public FileMeta USBFile
        {
            get;
            set;
        }
        public ConflictType type
        {
            get;
            set;
        }
    }
}
