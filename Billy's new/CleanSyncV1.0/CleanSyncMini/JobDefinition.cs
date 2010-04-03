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

        public JobDefinition()
        {
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
}
