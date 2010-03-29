using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanSync
{
    [Serializable]
    public class JobConfig
    {
        public enum AutoConflictOption
        {
            off,
            KeepAllPCItems,
            KeepAllUSBItems,
            KeepBoth,
            IgnoreBoth
        }

        public enum AutoSyncOption
        {
            On,
            off
        }

        public JobConfig()
        {
            this.ConflictConfig = AutoConflictOption.off;
            this.SyncConfig = AutoSyncOption.off;
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
