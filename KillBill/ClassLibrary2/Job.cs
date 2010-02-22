using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{

    [Serializable]
    class JobComputer
    {
        internal string JobName
        {
            get; set;
        }

        internal string ComputerPath 
        {
            get; set;
        }
        
        internal int ID
        {
            get; set;
        }

        public FolderMeta DirectoryInformation
        {
            get;
            set;
        }

        private JobExternal jobExternal;

        public JobComputer(string jobName, string computerPath)
        {
            this.JobName = jobName;
            this.ComputerPath = computerPath;
        }

        public void DockExternalJob(JobExternal jobExternal)
        {
            this.jobExternal = jobExternal;
        }

        public void UndockExternalJob()
        {
            this.jobExternal = null;
        }

    }

    [Serializable]
    class JobExternal
    {
        internal string ExternalPath
        {
            get;
            set;
        }

        internal string JobName
        {
            get;
            set;
        }

        internal int LastSynchronizedID
        {
            get;
            set;
        }

        public FolderMeta UpdatedDirectoryInformation
        {
            get;
            set;
        }

        public JobExternal(string jobName, string externalPath)
        {
            this.JobName = jobName;
            this.ExternalPath = externalPath;
        }
    }

}
