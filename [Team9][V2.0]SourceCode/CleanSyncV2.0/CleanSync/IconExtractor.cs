/***********************************************************************
 * 
 * *****************CleanSync Version 2.0 IconExtractor*****************
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
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CleanSync
{
    public class IconExtractor
    {
        [DllImport("Kernel32.dll")]
        public static extern int GetModuleHandle(string lpModuleName);
        [DllImport("Shell32.dll")]
        public static extern IntPtr ExtractIcon(int hInst, string FileName, int nIconIndex);
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
        [DllImport("Shell32.dll")]
        public static extern IntPtr ExtractIconEx(string FileName, int nIconIndex, int[] lgIcon, int[] smIcon, int nIcons);
        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);
        
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero;
                iIcon = 0;
                dwAttributes = 0;
                szDisplayName = "";
                szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010
        }

        /// <summary>
        /// Retreive icon give a specific file path
        /// </summary>
        /// <param name="strPath"></param>
        /// <param name="bSmall"></param>
        /// <returns></returns>
        public static Icon GetIcon(string strPath, bool bSmall)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            return Icon.FromHandle(info.hIcon);
        }

        private static Icon GetSysIcon(int icNo)
        {
            IntPtr HIcon = ExtractIcon(GetModuleHandle(string.Empty), "Shell32.dll", icNo);
            return Icon.FromHandle(HIcon);
        }

        /// <summary>
        /// Get system icon for folders
        /// </summary>
        /// <returns></returns>
        public static Icon GetFolderIcon()
        {
            // DLL path
            string DLLPath = System.IO.Path.Combine(Environment.SystemDirectory, "shell32.dll");
            // Extract icon
            System.IntPtr oPtr = ExtractIcon(GetModuleHandle(string.Empty), DLLPath, 4);

            return Icon.FromHandle(oPtr);
        }

        /// <summary>
        /// Get Image Source for files given their extentions
        /// </summary>
        /// <param name="extention"></param>
        /// <returns></returns>
        public static ImageSource GetFileIconImage(string extention)
        {          
            ImageSource img;
            try
            {
                if (extention.IndexOf(@".") == -1)
                {
                    img = GetSysIconImage();
                }
                extention = "a" + extention.Substring(extention.LastIndexOf(@"."));
                using (System.Drawing.Icon i = GetIcon(extention, false))
                {
                    img = Imaging.CreateBitmapSourceFromHIcon(
                                             i.Handle,
                                             new Int32Rect(0, 0, i.Width, i.Height),
                                               BitmapSizeOptions.FromEmptyOptions());
                    DestroyIcon(i.Handle);
                }
            }
            catch (Exception)
            {
                
                return GetSysIconImage();
            }
            return img;
        }

        /// <summary>
        /// Get Image Source for folders
        /// </summary>
        /// <returns></returns>
        public static ImageSource GetFolderIconImage()
        {
            ImageSource img;
            try
            {
                using (System.Drawing.Icon i = GetFolderIcon())
                {
                    img = Imaging.CreateBitmapSourceFromHIcon(
                                             i.Handle,
                                             new Int32Rect(0, 0, i.Width, i.Height),
                                               BitmapSizeOptions.FromEmptyOptions());
                    DestroyIcon(i.Handle);
                }
            }
            catch (Exception)
            {
                return GetSysIconImage();
            }
            return img;
        }

        /// <summary>
        /// Get Image Source for System icons
        /// </summary>
        /// <returns></returns>
        private static ImageSource GetSysIconImage()
        {
            ImageSource img;
            using (System.Drawing.Icon i = GetSysIcon(0))
            {
                img = Imaging.CreateBitmapSourceFromHIcon(
                                         i.Handle,
                                         new Int32Rect(0, 0, i.Width, i.Height),
                                           BitmapSizeOptions.FromEmptyOptions());
                DestroyIcon(i.Handle);
            }
            return img;
        }
    }   
}