using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    public class Conflicts
    {

        public enum FolderFileType
        {
            FolderConflict,
            FileConflict
        }

        public enum ConflictType
        {
            New,
            Modified,
            Deleted

        }
       

        public Conflicts(FolderMeta currentPCFolder, FolderMeta USBFolder, ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFolder = currentPCFolder;
            this.USBFolder  = USBFolder;
            this.FolderOrFileConflictType = FolderFileType.FolderConflict;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;
            
        }
        public Conflicts(FileMeta currentPCFile, FileMeta USBFile,ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFile = currentPCFile;
            this.USBFile = USBFile;
            this.FolderOrFileConflictType = FolderFileType.FileConflict;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;

        }
        public FolderMeta CurrentPCFolder
        {
            get;
            set;
        }
       
        public FolderMeta USBFolder
        {
            get;
            set;
        }
        public FileMeta CurrentPCFile
        {
            get;
            set;
        }
        public FileMeta USBFile
        {
            get;
            set;
        }
        public FolderFileType FolderOrFileConflictType
        {
            get;
            set;
        }

        public ConflictType PCFolderFileType
        {
            get;
            set;
        }
        public ConflictType USBFolderFileType
        {
            get;
            set;
        }


        public override string ToString()
        {
            string result ="";
            if(this.FolderOrFileConflictType == Conflicts.FolderFileType.FolderConflict)
            { 
                result  += "[FolderConflict]";
                result  +=  "PCFolder: " +this.CurrentPCFolder.AbsolutePath + "("+ this.PCFolderFileType + ")" + " conflicts with  USBFolder" + this.USBFolder.AbsolutePath +"("+ this.USBFolderFileType +")";
            }
            else
            {
                result  += "[FileConflict]";
                result  +=  "PCFile: " +this.CurrentPCFile.AbsolutePath + "("+ this.PCFolderFileType + ")" + " conflicts with  USBFile" + this.USBFile.AbsolutePath +"("+ this.USBFolderFileType +")";
            }


            return result;
        }
    }
}
