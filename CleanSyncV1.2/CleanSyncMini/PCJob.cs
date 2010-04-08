using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSync
{
    [Serializable]
    public class PCJob : JobDefinition
    {
        public string PCPath
        {
            get;
            set;
        }

        public FolderMeta FolderInfo
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

        public PCJob()
            : base()
        {
        }

        public PCJob(string jobName, string pathOnPC, string pathOnUSB, string PCID, JobConfig config)
            : base(jobName, pathOnUSB)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            this.PCID = PCID;
            usbJob = null;
            this.JobSetting = config;
        }

        public PCJob(USBJob jobOnUSB, string pathOnPC, string PCID)
            : base(jobOnUSB.JobName, jobOnUSB.AbsoluteUSBPath)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            this.PCID = PCID;
            usbJob = jobOnUSB;
            this.JobSetting = new JobConfig();
        }
    }
}
