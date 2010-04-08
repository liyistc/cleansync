using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSync;
using System.ComponentModel;

namespace DirectoryInformation
{
    public class Conflicts : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public enum FolderFileType
        {
            FolderConflict,
            FileConflict,
            FolderVSFileConflict,
            FileVSFolderConflict,
            FolderVSSubFolderConflict,
            SubFolderVSFolderConflict
        }

        public enum UserChoice
        {
            KeepPCUpdates,
            KeepUSBUpdates,
            Untouched
        }

        public enum ConflictType
        {
            New,
            Modified,
            Deleted
        }
     

        public Conflicts(FolderFileType type,FolderMeta currentPCFolder, FolderMeta USBFolder, ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFolder = currentPCFolder;
            this.USBFolder  = USBFolder;
            this.FolderOrFileConflictType = type;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;
            Initialization();
            
        }

        private void Initialization()
        {
            this.Name = GetName();
            this.setIconImage();
            setConflictionTypImageSource();
            SetModifiedTime();
        }

        private void setConflictionTypImageSource()
        {
            switch (this.PCFolderFileType)
            {
                case ConflictType.New:
                    this.PCConflictTypeImage = "Pic/Create.png";
                    break;
                case ConflictType.Deleted:
                    this.PCConflictTypeImage = "Pic/Delete.png";
                    break;
                case ConflictType.Modified:
                    this.PCConflictTypeImage = "Pic/Modify.png";
                    break;
            }

            switch (this.USBFolderFileType)
            {
                case ConflictType.New:
                    this.USBConflictTypeImage = "Pic/Create.png";
                    break;
                case ConflictType.Deleted:
                    this.USBConflictTypeImage = "Pic/Delete.png";
                    break;
                case ConflictType.Modified:
                    this.USBConflictTypeImage = "Pic/Modify.png";
                    break;
            }
        }
        public Conflicts(FolderFileType type,FolderMeta currentPCFolder, FileMeta USBFile, ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFolder = currentPCFolder;
            this.USBFile = USBFile;
            this.FolderOrFileConflictType = type;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;
            Initialization();
        }
        public Conflicts(FolderFileType type, FileMeta currentPCFile, FolderMeta USBFolder, ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFile = currentPCFile;
            this.USBFolder = USBFolder;
            this.FolderOrFileConflictType =type;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;
            Initialization();
        }
        public Conflicts(FolderFileType type,FileMeta currentPCFile, FileMeta USBFile, ConflictType PCFolderFileType, ConflictType USBFolderFileType)
        {
            this.CurrentPCFile = currentPCFile;
            this.USBFile = USBFile;
            this.FolderOrFileConflictType = type;
            this.PCFolderFileType = PCFolderFileType;
            this.USBFolderFileType = USBFolderFileType;
            Initialization();
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

        public string Name
        {
            set;
            get;
        }

        public string PCModifiedTime
        {
            set;
            get;
        }
        public string USBModifiedTime
        {
            set;
            get;
        }

        private bool usbSelectedFlag;
        public bool USBSelected
        {
            get { return usbSelectedFlag; }
            set
            {
                usbSelectedFlag = value;
                OnPropertyChanged("USBSelected");
            }
        }

        private bool pcSelectedFlag;
        public bool PCSelected
        {
            get { return pcSelectedFlag; } 
            set
            {
                pcSelectedFlag = value;
                OnPropertyChanged("PCSelected");
            }
        }
        public System.Windows.Media.ImageSource itemIconImage
        {
            get;
            set;
        }
        public string PCConflictTypeImage
        {
            get;
            set;
        }
        public string USBConflictTypeImage
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

        internal string GetName()
        {
            string result = null;
            switch (this.FolderOrFileConflictType)
            {
                case Conflicts.FolderFileType.FileConflict:
                    result = this.CurrentPCFile.Path + this.CurrentPCFile.Name;
                    break;
                case Conflicts.FolderFileType.FolderConflict:
                    result = this.CurrentPCFolder.Path + this.CurrentPCFolder.Name;
                    break;
                case Conflicts.FolderFileType.FolderVSFileConflict:
                    result = this.USBFile.Path + this.USBFile.Name;
                    break;
                case Conflicts.FolderFileType.FileVSFolderConflict:
                    result = this.CurrentPCFile.Path + this.CurrentPCFile.Name;
                    break;
                case Conflicts.FolderFileType.FolderVSSubFolderConflict:
                    result = this.USBFolder.Path + this.USBFolder.Name;
                    break;
                case Conflicts.FolderFileType.SubFolderVSFolderConflict:
                    result = this.CurrentPCFolder.Path + this.CurrentPCFolder.Name;
                    break;

            }
            return result;
        }

        private void SetModifiedTime()
        {
            switch (this.FolderOrFileConflictType)
            {
                case Conflicts.FolderFileType.FileConflict:
                    this.PCModifiedTime = CurrentPCFile.LastModifiedTime.ToLocalTime().ToString("MM/dd/yyyy H:mm");
                    this.USBModifiedTime = USBFile.LastModifiedTime.ToLocalTime().ToString("MM/dd/yyyy H:mm");
                    break;
                case Conflicts.FolderFileType.FolderConflict:
                    this.PCModifiedTime = "------";
                    this.USBModifiedTime = "------";
                    break;
                case Conflicts.FolderFileType.FolderVSFileConflict:
                    this.PCModifiedTime = "------";
                    this.USBModifiedTime = USBFile.LastModifiedTime.ToLocalTime().ToString("MM/dd/yyyy H:mm");
                    break;
                case Conflicts.FolderFileType.FileVSFolderConflict:
                    this.PCModifiedTime = CurrentPCFile.LastModifiedTime.ToLocalTime().ToString("MM/dd/yyyy H:mm");
                    this.USBModifiedTime = "------";
                    break;
                case Conflicts.FolderFileType.FolderVSSubFolderConflict:
                    this.PCModifiedTime = "------";
                    this.USBModifiedTime = "------";
                    break;
                case Conflicts.FolderFileType.SubFolderVSFolderConflict:
                    this.PCModifiedTime = "------";
                    this.USBModifiedTime = "------";
                    break;

            }
        }

        public Conflicts.UserChoice getUserChoice()
        {
            if (USBSelected) return Conflicts.UserChoice.KeepUSBUpdates;
            else return Conflicts.UserChoice.KeepPCUpdates;
        } 

        private void setIconImage()
        {
            if (this.FolderOrFileConflictType.Equals(Conflicts.FolderFileType.FolderConflict))
            {
                this.itemIconImage = IconExtractor.GetFolderIconImage();
            }
            else
            {
                this.itemIconImage = IconExtractor.GetFileIconImage(this.GetName());
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
