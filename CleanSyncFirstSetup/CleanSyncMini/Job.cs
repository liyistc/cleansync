using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace cleanSyncMinimalVersion
{
    [Serializable]
    class Job
    {
        internal string pathPC
        {
            get;
            set;
        }
        internal string pathUSB
        {
            get;
            set;
        }
        internal string jobName
        {
            get;
            set;
        }
        internal FolderMeta FM
        {
            get;
            set;
        }
        internal int PCID
        {
            get;
            set;
        }
        internal JobUSB JobUSB
        {
            get;
            set;
        }
        internal string pathPC2
        {
            get;
            set;
        }
        public Job(string pathPC, string pathUSB, string jobName)
        {
            this.pathPC = pathPC;
            this.pathUSB = pathUSB;
            this.jobName = jobName;
            FM = null;
            PCID = 0;
            pathPC2 = "";
        }
        public Job(JobUSB jobUSB, string pathPC)
        { 
        }
    }
}
