using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.ComponentModel;

namespace CleanSyncMini
{
    class usbDetection2
    {
        BackgroundWorker bgWorker;

        public usbDetection2(BackgroundWorker bgWorker)
        {
            this.bgWorker = bgWorker;
            instanceCreation();
            instanceDeletion();
        }

        public void instanceCreation()
        {
            ManagementEventWatcher w = null;
            ManagementOperationObserver observer = new ManagementOperationObserver();
            ManagementScope scope = new ManagementScope();
            WqlEventQuery q = new WqlEventQuery();
            q.EventClassName = "__InstanceCreationEvent";
            q.WithinInterval = new TimeSpan(0, 0, 3);
            q.Condition = @"TargetInstance ISA 'Win32_DiskDrive' ";
            w = new ManagementEventWatcher(scope, q);
            w.EventArrived += UsbEventArrived;
            w.Start();
        }
        public void instanceDeletion()
        {
            ManagementEventWatcher w = null;

            ManagementOperationObserver observer = new ManagementOperationObserver();
            ManagementScope scope = new ManagementScope();
            WqlEventQuery q = new WqlEventQuery();
            q.EventClassName = "__InstanceDeletionEvent";
            q.WithinInterval = new TimeSpan(0, 0, 3);
            q.Condition = @"TargetInstance ISA 'Win32_DiskDrive' ";
            w = new ManagementEventWatcher(scope, q);
            w.EventArrived += USBEventLeaved;
            w.Start();
        }

        public void UsbEventArrived(object sender, EventArrivedEventArgs e)
        {
            DriveInfo[] drive = DriveInfo.GetDrives();
            foreach (DriveInfo driveinfo in drive)
            {
                if (driveinfo.VolumeLabel != "")
                {
                    bgWorker.ReportProgress(0, driveinfo.VolumeLabel + "-------" + driveinfo.RootDirectory + " Drive");
                    //System.Windows.Forms.MessageBox.Show(driveinfo.VolumeLabel + "-------" + driveinfo.RootDirectory + " Drive");
                }
                else
                    Console.WriteLine(driveinfo.RootDirectory + "----- does not have a volume label");
            }
        }
        public void USBEventLeaved(object sender, EventArrivedEventArgs e)
        {
            DriveInfo[] drive = DriveInfo.GetDrives();
            foreach (DriveInfo driveinfo in drive)
            {
                if (driveinfo.VolumeLabel != null)
                    Console.WriteLine(driveinfo.VolumeLabel + "-------" + driveinfo.RootDirectory + " Drive");
                else
                    Console.WriteLine(driveinfo.RootDirectory + "----- does not have a volume label");
            }
        }
    }
}
