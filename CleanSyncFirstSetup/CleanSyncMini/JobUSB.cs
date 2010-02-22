using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace cleanSyncMinimalVersion
{
    [Serializable]
    class JobUSB
    {
        internal string JobName;
        internal int PCID;
        internal FolderMeta rootFolder;
        internal Differences differences
        {
            get;
            set;
        }
        public JobUSB(string JobName, int PCID, FolderMeta rootFolder)
        {
            this.JobName = JobName;
            this.PCID = PCID;
            this.rootFolder = rootFolder;
        }

        internal void SetDifferenceMeta()
        {
            throw new NotImplementedException();
        }
    }
}
