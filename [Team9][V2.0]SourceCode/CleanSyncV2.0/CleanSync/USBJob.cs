/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 USBJob**********************
 * 
 * Written By : Li Yi & Tso Shuk Yi & Li Zichen
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace CleanSync
{
    /// <summary>
    /// USBJob stores the information about the Job on the USB. It is serializable, to be saved onto the USB.
    /// </summary>
    [Serializable]
    public class USBJob : JobDefinition
    {

        #region Constructor
        public USBJob(PCJob jobOnPC)
            : base(jobOnPC.JobName, jobOnPC.AbsoluteUSBPath)
        {
            PCOnePath = jobOnPC.PCPath;
            PCTwoPath = "";
            Synchronizing = false;
            SynchronizingPcToUSB = false;
            SynchronizingUSBToPC = true;
            ReSynchronizing = false;
            diff = new Differences();
            PCOneID = jobOnPC.PCID;
            PCTwoID = INVALID;
            MostRecentPCID = INVALID;
            MovingOldDiffToTemp = false;
            MovingTempToOldDiff = false;
        }
        #endregion
        const string PCONE = "";
        const string INVALID = "INVALID";

        #region Attributes about the PCs
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

        #endregion

        #region Attributes about last synchronization
        public string MostRecentPCID
        {
            get;
            set;
        }

        public Differences diff
        {
            get;
            set;
        }

        #region Attributes to check for previously interrupted synchronizations
        public bool Synchronizing
        {
            get;
            set;
        }

        public bool ReSynchronizing
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

        public bool MovingOldDiffToTemp
        {
            get;
            set;
        }

        public bool MovingTempToOldDiff
        {
            get;
            set;
        }
        #endregion
        #endregion


    }
}
