using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exceptions
{
    public class FirstTimeSyncInterruptedException : Exception
    {
        public FirstTimeSyncInterruptedException(bool saved)
            : base("USB Plugged out!")
        {
            USBJobSaved = saved;
        }
        public bool USBJobSaved
        {
            get;
            set;
        }
    }

    public class RecoveryNotPossibleException : Exception
    {
    }

    public class SyncInterruptedException : Exception
    {

    }
}
