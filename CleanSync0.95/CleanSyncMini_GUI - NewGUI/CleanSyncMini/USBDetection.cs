using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.Collections.ObjectModel;

namespace CleanSyncMini
{
    public partial class USBDetection : Form
    {
        //GUI userInterface;
        BackgroundWorker bgWorker;
        private List<string> drives;
        public ObservableCollection<string> usbDriveList;

        public USBDetection()
        {
            InitializeComponent();
            drives = new List<string>();
            usbDriveList = new ObservableCollection<string>();
            SetDrives();

        }

        public void addBackgroundWorker(BackgroundWorker bgWorker)
        {
            this.bgWorker = bgWorker;
        }
        
        public void runDetection(USBDetection usbDetector)
        {
            Application.Run(usbDetector);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_VOLUME
        {
            public int dbcv_size;
            public int dbcv_devicetype;
            public int dbcv_reserved;
            public int dbcv_unitmask;
        }

        protected override void WndProc(ref Message m)
        {
            //you may find these definitions in dbt.h and winuser.h 
            const int WM_DEVICECHANGE = 0x0219;
            const int DBT_DEVICEARRIVAL = 0x8000;  // system detected a new device 
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  // system detected a new device 
            const int DBT_DEVTYP_VOLUME = 0x00000002;  // logical volume 
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch (m.WParam.ToInt32())
                    {
                        case DBT_DEVICEARRIVAL:
                            {
                                int devType = Marshal.ReadInt32(m.LParam, 4);
                                if (devType == DBT_DEVTYP_VOLUME)
                                {
                                    DEV_BROADCAST_VOLUME vol;
                                    vol = (DEV_BROADCAST_VOLUME)
                                    Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                                    
                                    //SetDrives();
                                    bgWorker.ReportProgress(0);
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            //MessageBox.Show("Removal");
                            //SetDrives();
                            bgWorker.ReportProgress(1);
                            break;
                    }
                    break;
            }
            //we detect the media arrival event 
            base.WndProc(ref m);
        }

        public void SetDrives()
        {
            drives.Clear();
            usbDriveList.Clear();

            foreach (ManagementObject drive in new ManagementObjectSearcher(
                        "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
            {
                // associate physical disks with partitions
                foreach (ManagementObject partition in new ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
                {
                    //Console.WriteLine("Partition=" + partition["Name"]);

                    // associate partitions with logical disks (drive letter volumes)
                    foreach (ManagementObject disk in new ManagementObjectSearcher(
                        "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                    {
                        //Console.WriteLine(disk["Name"]);
                        drives.Add((string)disk["Name"]+@"\");
                    }
                }

            }

            foreach (string drive in drives)
            {
                usbDriveList.Add(drive);
            }

        }

        public List<string> GetDrives()
        {
            return drives;
        }

        private void USBDetection_Load(object sender, EventArgs e)
        {

        }
    }
}
