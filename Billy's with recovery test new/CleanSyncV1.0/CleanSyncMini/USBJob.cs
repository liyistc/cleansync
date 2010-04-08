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

        /*Bily's new code*/
        public bool Synchronizing
        {
            get;
            set;
        }

        public bool SynchronizingPcToUSB
        {
            get;
            set;
        }

        public bool SynchronizingUSBToPC
        {
            get;
            set;
        }

        public bool ReSynchronizing
        {
            get;
            set;
        }

        public bool RecoveryPossible
        {
            get;
            set;
        }

        //end new code

        /*Billy's modified code*/
        public USBJob(PCJob jobOnPC)
            : base(jobOnPC.JobName, jobOnPC.AbsoluteUSBPath)
        {
            PCOnePath = jobOnPC.PCPath;
            PCTwoPath = "";
            Synchronizing = false;
            SynchronizingPcToUSB = false;
            SynchronizingUSBToPC = true;
            ReSynchronizing = false;
            RecoveryPossible = true;
            diff = new Differences();
            PCOneID = jobOnPC.PCID;
            PCTwoID = INVALID;
            MostRecentPCID = INVALID;
            //DeletePCID = null;
        }
        //end modified code

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

    }
}
