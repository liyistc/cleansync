using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSync
{
    public enum JobStatus { Complete, Incomplete, NotReady };
    [Serializable]
    public abstract class JobDefinition
    {
        public string JobName
        {
            get;
            set;
        }
        
        [NonSerialized]
        public string AbsoluteUSBPath;
       

        public JobStatus JobState
        {
            get;
            set;
        }

        public string RelativeUSBPath
        {
            get;
            set;
        }

        public JobDefinition(string jobName,string pathOnUSB)
        {
            JobName = jobName;
            string root = Path.GetPathRoot(pathOnUSB);
            RelativeUSBPath = pathOnUSB.Substring(root.Length);
            AbsoluteUSBPath = pathOnUSB;
            JobState = JobStatus.Incomplete;
        }

        public void ToggleStatus(JobStatus state)
        {
            if (state.Equals(JobStatus.Complete))
                JobState = JobStatus.NotReady;
            else if (state.Equals(JobStatus.NotReady))
                JobState = JobStatus.Complete;
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

    [Serializable]
    public class USBJob : JobDefinition
    {
        const string PCONE = "";
        const string INVALID = "INVALID";

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

        public string PCOneID
        {
            get;
            set;
        }

        public string PCTwoID
        {
            get;
            set;
        }

        public string MostRecentPCID
        {
            get;
            set;
        }

        public bool PCOneDeleted
        {
            get;
            set;
        }

        public bool PCTwoDeleted
        {
            get;
            set;
        }

        public USBJob(PCJob jobOnPC)
            : base(jobOnPC.JobName, jobOnPC.AbsoluteUSBPath)
        {
            PCOnePath = jobOnPC.PCPath;
            PCTwoPath = "";
            diff = null;
            PCOneID = jobOnPC.PCID;
            PCTwoID = INVALID;
            MostRecentPCID = INVALID;
            //DeletePCID = null;
        }
        
    }
}
