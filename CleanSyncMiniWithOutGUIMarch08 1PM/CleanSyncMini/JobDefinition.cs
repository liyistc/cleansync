using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSyncMini
{
    public enum JobStatus { Complete, Incomplete };
    [Serializable]
    public abstract class JobDefinition
    {
        public string JobName
        {
            get;
            set;
        }
        
        public string USBPath
        {
            get;
            set;
        }

        public JobStatus JobState
        {
            get;
            set;
        }

        public JobDefinition(string jobName,string pathOnUSB)
        {
            JobName = jobName;
            USBPath = pathOnUSB;
            JobState = JobStatus.Incomplete;
        }
    }

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

        

        public int PCID
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

        public PCJob(string jobName, string pathOnPC, string pathOnUSB)
            : base(jobName, pathOnUSB)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            PCID = 0;
            usbJob = null;
        }

        public PCJob(USBJob jobOnUSB, string pathOnPC)
            : base(jobOnUSB.JobName, jobOnUSB.USBPath)
        {
            PCPath = pathOnPC;
            FolderInfo = null;
            PCID = 1;
            usbJob = jobOnUSB;
        }
    }

    [Serializable]
    public class USBJob : JobDefinition
    {
        const int PCONE = 1;
        const int INVALID = -1;

        public string PCOnePath
        {
            get;
            set;
        }

        public string PCTwoPath
        {
            get;
            set;
        }

        public Differences diff
        {
            get;
            set;
        }

        public int PCOneID
        {
            get;
            set;
        }

        public int PCTwoID
        {
            get;
            set;
        }

        public int MostRecentPCID
        {
            get;
            set;
        }

        public USBJob(PCJob jobOnPC) 
            : base(jobOnPC.JobName,jobOnPC.USBPath)
        {
            PCOnePath = jobOnPC.PCPath;
            PCTwoPath = "";
            diff = null;
            PCOneID = jobOnPC.PCID;
            PCTwoID = INVALID;
            MostRecentPCID = PCONE;
        }
        
    }
}
