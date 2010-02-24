using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace ConsoleApplication1
{
    class Program
    {

        public static string Name;
        public static bool Insert;
        static void Main(string[] args)
        {
                getUSBName();
                while (true)
                { }
        }
        public static void getUSBName()
        {
            Name = "Didn't find any USB plug in+\n";           
            AddInsertUSBHandler();        
        }

        private static void getDrives()
        {
            Name = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo driveiformation in allDrives)
            {
                string type = driveiformation.VolumeLabel;
                if (type != "")
                    Name += type + "\n";
                else
                    Name += driveiformation.RootDirectory + "------does not have a volume label\n";
            }
            Console.WriteLine(Name);
        }

        static ManagementEventWatcher w = null;

        static void AddInsertUSBHandler()
        {
            
                WqlEventQuery q;
                ManagementScope scope = new ManagementScope("root\\CIMV2");
                scope.Options.EnablePrivileges = true;
                try {

                        q = new WqlEventQuery();
                        q.EventClassName = "__InstanceCreationEvent";
                        q.WithinInterval = new TimeSpan(0, 0, 3);
                        q.Condition = "TargetInstance ISA 'Win32_USBControllerdevice'";
                        w = new ManagementEventWatcher(scope, q);
                        w.EventArrived += USBInserted;
                        w.Start();
                }
                catch {
                          w.Stop();
                        }
        }

        static void USBInserted(object sender, EventArgs e)
        {
            Insert = true;
            getDrives();
            Insert = false;
        }
    }

    }
