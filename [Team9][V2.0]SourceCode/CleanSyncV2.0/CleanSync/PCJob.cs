/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 PCJob*******************
 * 
 * Written By : Li Yi & Li Zichen & Tso Shuk Yi
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
using DirectoryInformation;

namespace CleanSync
{
    /// <summary>
    /// PCJob stores the information about the Job on the PC. It is serializable, to be saved onto the PC.
    /// </summary>
    [Serializable]
    public class PCJob : JobDefinition
    {
        #region Constructors
        public PCJob(string jobName, string pathOnPC, string pathOnUSB, string PCID, JobConfig config)
            : base(jobName, pathOnUSB)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            this.PCID = PCID;
            usbJob = null;
            this.JobSetting = config;
            Synchronizing = false;
        }

        public PCJob(USBJob jobOnUSB, string pathOnPC, string PCID)
            : base(jobOnUSB.JobName, jobOnUSB.AbsoluteUSBPath)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            this.PCID = PCID;
            usbJob = jobOnUSB;
            this.JobSetting = new JobConfig();
            Synchronizing = false;
        }
        #endregion

        #region Attributes about this computer
        public string PCPath
        {
            get;
            set;
        }

        public string PCID
        {
            get;
            set;
        }

        public JobConfig JobSetting
        {
            get;
            set;
        }
        #endregion

        #region Attributes about last synchronization
        public bool Synchronizing
        {
            get;
            set;
        }

        public FolderMeta FolderInfo
        {
            get;
            set;
        }
        #endregion

        #region USBJob 
        [NonSerialized]
        private USBJob usbJob;

        public USBJob GetUsbJob()
        {
            return usbJob;
        }

        public void SetUsbJob(USBJob usb)
        {
            usbJob = usb;
        }
        #endregion
    }
}
