using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSync
{
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

        public USBJob():base()
        {
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
