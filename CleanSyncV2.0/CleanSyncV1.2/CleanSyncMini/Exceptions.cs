using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exceptions
{
    public class SyncInterruptedException : Exception
    {
        public SyncInterruptedException()
            : base("USB Plugged out!")
        {
        }
        /*   public bool USBJobSaved
           {
               get;
               set;
           }
         * */
    }

    public class RecoveryNotPossibleException : Exception
    {
    }

    public class SyncCancelledException : Exception
    {
        public SyncCancelledException(string message)
            : base(message)
        {
        }
    }
}
