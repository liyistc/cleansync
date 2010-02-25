using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSyncMinimalVersion
{
    [Serializable]
    public class JobUSB
    {
        internal string jobName
        {
            get;
            set;
        }
        internal string pathOnePC
        {
            get;
            set;
        }
        internal string pathTwoPC
        {
            get;
            set;
        }
        internal string pathUSB
        {
            get;
            set;
        }
        internal int PCID;
        internal FolderMeta rootFolder;
        internal Differences differences
        {
            get;
            set;
        }
        public JobUSB(string JobName, string pathOnePC, string pathUSB, int PCID, FolderMeta rootFolder)
        {
            this.jobName = JobName;
            this.pathOnePC = pathOnePC;
            this.pathUSB = pathUSB;
            this.PCID = PCID;
            this.rootFolder = rootFolder;
        }

        internal void SetDifferenceMeta()
        {
            throw new NotImplementedException();
        }

        public bool Equals(JobUSB job)
        {
            if (this.jobName.Equals(job.jobName)) return true;
            else return false;
        }
    }
}
