using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exceptions
{

    /// <summary>
    /// Thrown when synchronization is interrupted because USB is plugged out
    /// </summary>
    public class USBUnpluggedException : Exception
    {
        public USBUnpluggedException()
            : base("USB has been unplugged! Synchronization failed.")
        {
        }
    }

    /// <summary>
    /// Thrown when synchronization is interrupted because the user has cancelled the job.
    /// </summary>
    public class SyncCancelledException : Exception
    {
        public SyncCancelledException(string message)
            : base(message)
        {
        }
    }
}
