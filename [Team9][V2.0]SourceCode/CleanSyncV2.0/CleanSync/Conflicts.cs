/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 Conflicts*******************
 * 
 * Written By : Yu Qiqi
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
using CleanSync;
using System.ComponentModel;

namespace DirectoryInformation
{
    public class Conflicts : INotifyPropertyChanged
    {
       

        #region Enumerators
        /// <summary>
        /// Indicate the content of a Conflict Object 
        /// </summary>
        public enum FolderFileType
        {
            FileConflict,
            FolderVSFileConflict,
            FileVSFolderConflict
        }
        /// <summary>
        /// User choice(Keep PC updates or Keep USB updates)
        /// </summary>
        public enum UserChoice
        {
            KeepPCUpdates,
            KeepUSBUpdates
        }

        /// <summary>
        /// type for folders and files inside the conflict object
        /// </summary>
        public enum ConflictType
        {
            New,
            Modified,
            Deleted
        }
        #endregion

        #region Entry Methods
        private void Initialization()
        {
            this.Name = GetName();
            this.setIconImage();
            setConflictionTypImageSource();
            SetModifiedTime();
        }
        #endregion

        #region Constructor
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
        #endregion

        #region Variables
        public event PropertyChangedEventHandler PropertyChanged;
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
        /// <summary>
        /// Conflict Item Name
        /// </summary>
        public string Name
        {
            set;
            get;
        }
        /// <summary>
        /// Last Modified Time on PC
        /// </summary>
        public string PCModifiedTime
        {
            set;
            get;
        }
        /// <summary>
        /// Last Modified Time on USB drive
        /// </summary>
        public string USBModifiedTime
        {
            set;
            get;
        }
        /// <summary>
        /// Conflict Type Image for Conflict Item on PC
        /// </summary>
        public string PCConflictTypeImage
        {
            get;
            set;
        }
        /// <summary>
        /// Conflict Type Image for Conflict Item on USB drive
        /// </summary>
        public string USBConflictTypeImage
        {
            get;
            set;
        }
        /// <summary>
        /// Indicator: Item on USB drive is selected
        /// </summary>
        private bool usbSelectedFlag;
        /// <summary>
        /// Indicator: Item on PC is selected
        /// </summary>
        private bool pcSelectedFlag;
        #endregion

        #region Display Helper
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
        private void setIconImage()
        {
            this.itemIconImage = IconExtractor.GetFileIconImage(this.GetName());

        }
        private string GetName()
        {
            string result = null;
            switch (this.FolderOrFileConflictType)
            {
                case Conflicts.FolderFileType.FileConflict:
                    result = this.CurrentPCFile.Path + this.CurrentPCFile.Name;
                    break;
                case Conflicts.FolderFileType.FolderVSFileConflict:
                    result = this.USBFile.Path + this.USBFile.Name;
                    break;
                case Conflicts.FolderFileType.FileVSFolderConflict:
                    result = this.CurrentPCFile.Path + this.CurrentPCFile.Name;
                    break;
            }
            return result;
        }
        private void SetModifiedTime()
        {
            switch (this.FolderOrFileConflictType)
            {
                case Conflicts.FolderFileType.FileConflict:
                    this.PCModifiedTime = CurrentPCFile.LastModifiedTime.ToLocalTime().ToString("dd/MM/yyyy H:mm");
                    this.USBModifiedTime = USBFile.LastModifiedTime.ToLocalTime().ToString("dd/MM/yyyy H:mm");
                    break;
                case Conflicts.FolderFileType.FolderVSFileConflict:
                    this.PCModifiedTime = "Root Folder";
                    this.USBModifiedTime = USBFile.LastModifiedTime.ToLocalTime().ToString("dd/MM/yyyy H:mm");
                    break;
                case Conflicts.FolderFileType.FileVSFolderConflict:
                    this.PCModifiedTime = CurrentPCFile.LastModifiedTime.ToLocalTime().ToString("dd/MM/yyyy H:mm");
                    this.USBModifiedTime = "Root Folder";
                    break;
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

        /// <summary>
        /// Icon image source of conflict item
        /// </summary>
        public System.Windows.Media.ImageSource itemIconImage
        {
            get;
            set;
        }
        #endregion

        #region Get User Choice for Confict Handling
        /// <summary>
        /// Get the user choice for conflict handling
        /// </summary>
        /// <returns>Conflicts.UserChoice</returns>
        public Conflicts.UserChoice getUserChoice()
        {
            if (USBSelected) return Conflicts.UserChoice.KeepUSBUpdates;
            else return Conflicts.UserChoice.KeepPCUpdates;
        }
        /// <summary>
        /// Bool: usb changes is selected
        /// </summary>
        public bool USBSelected
        {
            get { return usbSelectedFlag; }
            set
            {
                usbSelectedFlag = value;
                OnPropertyChanged("USBSelected");
            }
        }
        /// <summary>
        /// bool: PC changes is selected
        /// </summary>
        public bool PCSelected
        {
            get { return pcSelectedFlag; }
            set
            {
                pcSelectedFlag = value;
                OnPropertyChanged("PCSelected");
            }
        }
        #endregion


    }
}
