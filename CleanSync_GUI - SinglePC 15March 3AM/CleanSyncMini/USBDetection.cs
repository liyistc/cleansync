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

namespace CleanSyncMini
{
    public partial class USBDetection : Form
    {
        //GUI userInterface;
        BackgroundWorker bgWorker;
        private List<string> drives;

        public USBDetection()
        {
            InitializeComponent();
            drives = new List<string>();
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
                                    
                                    SetDrives();
                                    bgWorker.ReportProgress(0);
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            //MessageBox.Show("Removal");
                            bgWorker.ReportProgress(1);
                            break;
                    }
                    break;
            }
            //we detect the media arrival event 
            base.WndProc(ref m);
        }

        private void SetDrives()
        {
            //System.Threading.Thread.Sleep(10000);   
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo driveiformation in allDrives)
            {
                if (driveiformation.DriveType.ToString().Equals("Fixed") || driveiformation.DriveType.ToString().Equals("Removable"))
                {
                    drives.Add(driveiformation.RootDirectory.ToString());                    
                }
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
