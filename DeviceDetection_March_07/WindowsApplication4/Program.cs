using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;


namespace WindowsApplication4
{
    class Program
    {
        internal static string getDrives()
        {
            string Name = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo driveiformation in allDrives)
            {
                string type = "";
                if (driveiformation.DriveType.ToString().Equals("Fixed") || driveiformation.DriveType.ToString().Equals("Removable"))
                {
                    type = driveiformation.VolumeLabel;
                    if (type != "")
                        Name += type + "\n";
                    else
                        Name += driveiformation.RootDirectory + "------does not have a volume label\n";
                }
            }
            return Name;
        }
    }
}
