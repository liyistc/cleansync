using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    class Job
    {
        public string PCPath{
            get; set;
        }

        public string MetaData{
            get; set;
        }

        public string USBPath{
            get; set;
        }

        public Job(string PCPath, string USBPath, string name)
        {
            this.PCPath = PCPath;
            this.USBPath = USBPath;
        }

    }



    class JobUSB
    {
        private Differences latestDifferences;
        private string lastModifiedPCID;

        public JobUSB() { }

        public Differences GetDifferences()
        {
            return latestDifferences;
        }

        public void UpdateLastModifiedPCID(string id)
        {
            this.lastModifiedPCID = id;
        }

        public void UpdateJobUSBDifferences(Differences USBDifferences)
        {
            this.latestDifferences = USBDifferences;
        }
    }
}
