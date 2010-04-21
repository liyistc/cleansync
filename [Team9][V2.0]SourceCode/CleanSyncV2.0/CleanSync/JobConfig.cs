/***********************************************************************
 * 
 * *******************CleanSync Version 2.0 JobConfig*******************
 * 
 * Written By : Li Yi
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
using System.ComponentModel;
using System.Globalization;

namespace CleanSync
{
    [Serializable]
    public class JobConfig
    {
        public JobConfig()
        {
            this.ConflictConfig = AutoConflictOption.Off;
            this.SyncConfig = AutoSyncOption.Off;
        }

        public JobConfig(AutoConflictOption conflictOpt, AutoSyncOption syncOpt)
        {
            ConflictConfig = conflictOpt;
            SyncConfig = syncOpt;
        }

        public AutoConflictOption ConflictConfig
        {
            get;
            set;
        }

        public AutoSyncOption SyncConfig
        {
            get;
            set;
        }
    }
}
