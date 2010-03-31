
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
        //private static ImageSource sysIcon = GetSysIconImage();
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
        //[StructLayout(LayoutKind.Sequential)]


        

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
            //DestroyIcon(info.hIcon);
        }

        public static Icon GetSysIcon(int icNo)
        {
            IntPtr HIcon = ExtractIcon(GetModuleHandle(string.Empty), "Shell32.dll", icNo);
            return Icon.FromHandle(HIcon);
        }

        public static Icon GetFolderIcon()
        {
            // DLL path
            string DLLPath = System.IO.Path.Combine(Environment.SystemDirectory, "shell32.dll");
            // Open folder icon index
            //int iconIndex = 4;
            // Extract icon
            System.IntPtr oPtr = ExtractIcon(GetModuleHandle(string.Empty), DLLPath, 4);

            return Icon.FromHandle(oPtr);
           
        }

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
                //using (System.Drawing.Icon i = GetSysIcon(0))
                //return sysIcon.Clone();
                return GetSysIconImage();
            }
            return img;
        }

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

/*
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;

namespace CleanSyncMini
{
    struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    public enum IconSize : uint
    {
        Large = 0x0,  //32x32
        Small = 0x1 //16x16        
    }



    //the function that will extract the icons from a file
    public class IconHandler
    {
        const uint SHGFI_ICON = 0x100;
        const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        [DllImport("Shell32", CharSet = CharSet.Auto)]
        internal extern static int ExtractIconEx(
            [MarshalAs(UnmanagedType.LPTStr)] 
            string lpszFile,      //size of the icon
            int nIconIndex,       //index of the icon
            // (in case we have more
            // then 1 icon in the file
            IntPtr[] phIconLarge, //32x32 icon
            IntPtr[] phIconSmall, //16x16 icon
            int nIcons);          //how many to get

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(
            string pszPath,            //path
            uint dwFileAttributes,    //attributes
            ref SHFILEINFO psfi,    //struct pointer
            uint cbSizeFileInfo,    //size
            uint uFlags);    //flags

        [DllImport("User32.dll")]
        private static extern int
                DestroyIcon(System.IntPtr hIcon);
        // free up the icon pointers.

        //will return an array of icons 
        public static Icon[] IconsFromFile(string Filename, IconSize Size)
        {
            int IconCount = ExtractIconEx(Filename, -1,
                            null, null, 0); //checks how many icons.
            IntPtr[] IconPtr = new IntPtr[IconCount];

            //extracts the icons by the size that was selected.
            if (Size == IconSize.Small)
                ExtractIconEx(Filename, 0, null, IconPtr, IconCount);
            else
                ExtractIconEx(Filename, 0, IconPtr, null, IconCount);

            Icon[] IconList = new Icon[IconCount];

            //gets the icons in a list.
            for (int i = 0; i < IconCount; i++)
            {
                IconList[i] = (Icon)Icon.FromHandle(IconPtr[i]).Clone();
                DestroyIcon(IconPtr[i]);
            }

            return IconList;
        }

        //extract one selected by index icon from a file.
        public static Icon IconFromFile(string Filename, IconSize Size, int Index)
        {
            int IconCount = ExtractIconEx(Filename, -1,
                            null, null, 0); //checks how many icons.
            if (IconCount < Index) return null; // no icons was found.

            IntPtr[] IconPtr = new IntPtr[1];

            //extracts the icon that we want in the selected size.
            if (Size == IconSize.Small)
                ExtractIconEx(Filename, Index, null, IconPtr, 1);
            else
                ExtractIconEx(Filename, Index, IconPtr, null, 1);

            return GetManagedIcon(IconPtr[0]);
        }

        //this will look throw the registry to find if the Extension have an icon.
        public static Icon IconFromExtension(string Extension, IconSize Size)
        {
            try
            {
                //add '.' if nessesry
                Extension = Extension.Substring(Extension.LastIndexOf(@"."));
                if (Extension[0] != '.') Extension = '.' + Extension;

                //opens the registry for the wanted key.
                RegistryKey Root = Registry.ClassesRoot;
                RegistryKey ExtensionKey = Root.OpenSubKey(Extension);
                ExtensionKey.GetValueNames();
                RegistryKey ApplicationKey =
                  Root.OpenSubKey(ExtensionKey.GetValue("").ToString());

                //gets the name of the file that have the icon.
                string IconLocation =
                  ApplicationKey.OpenSubKey("DefaultIcon").GetValue("").ToString();
                string[] IconPath = IconLocation.Split(',');

                if (IconPath[1] == null) IconPath[1] = "0";
                IntPtr[] Large = new IntPtr[1], Small = new IntPtr[1];

                //extracts the icon from the file.
                ExtractIconEx(IconPath[0],
                  Convert.ToInt16(IconPath[1]), Large, Small, 1);

                return GetManagedIcon(Size == IconSize.Large ? Large[0] : Small[0]);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error while" +
                       " trying to get icon for " +
                       Extension + " :" + e.Message);
                return null;
            }
        }
        public static Icon IconFromExtensionShell(string Extension, IconSize Size)
        {
            try
            {
                //add '.' if nessesry
                Extension = Extension.Substring(Extension.LastIndexOf(@"."));
                if (Extension[0] != '.') Extension = '.' + Extension;

                //temp struct for getting file shell info
                SHFILEINFO TempFileInfo = new SHFILEINFO();

                SHGetFileInfo(
                    Extension,
                    0,
                    ref TempFileInfo,
                    (uint)Marshal.SizeOf(TempFileInfo),
                    SHGFI_ICON | SHGFI_USEFILEATTRIBUTES | (uint)Size);

                return GetManagedIcon(TempFileInfo.hIcon);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error while" +
                  " trying to get icon for " + Extension +
                  " :" + e.Message);
                return null;
            }
        }
        public static Icon IconFromResource(string ResourceName)
        {
            Assembly TempAssembly = Assembly.GetCallingAssembly();

            return new Icon(TempAssembly.GetManifestResourceStream(ResourceName));
        }

       

        private static Icon GetManagedIcon(IntPtr hIcon)
        {
            Icon Clone = Icon.FromHandle(hIcon);

            DestroyIcon(hIcon);

            return Clone;
        }
    }

    public static class IconExtractor
    {
        //private static IconHandler iconH = new IconHandler();

        public static ImageSource GetFileIconImage(string extention)
        {

            ImageSource img;
            try
            {
                if (extention.IndexOf(@".") == -1)
                {
                    img = GetSysIconImage();
                }
                //extention = "a" + extention.Substring(extention.LastIndexOf(@"."));
                using (System.Drawing.Icon i = IconHandler.IconFromExtension(extention, IconSize.Large))
                {
                    img = Imaging.CreateBitmapSourceFromHIcon(
                                             i.Handle,
                                             new Int32Rect(0, 0, i.Width, i.Height),
                                               BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (Exception)
            {
                //using (System.Drawing.Icon i = GetSysIcon(0))
                return GetSysIconImage();
            }
            return img;
        }

        public static ImageSource GetFolderIconImage()
        {
            ImageSource img;
            try
            {
                using (System.Drawing.Icon i = IconHandler.IconFromExtension(@".exe",IconSize.Large))
                {
                    img = Imaging.CreateBitmapSourceFromHIcon(
                                             i.Handle,
                                             new Int32Rect(0, 0, i.Width, i.Height),
                                               BitmapSizeOptions.FromEmptyOptions());
                }
            }
            catch (Exception)
            {
                //using (System.Drawing.Icon i = GetSysIcon(0))
                return GetSysIconImage();
            }
            return img;
        }

        private static ImageSource GetSysIconImage()
        {
            ImageSource img;
            using (System.Drawing.Icon i = IconHandler.IconFromExtension(@".txt",IconSize.Large))
            {
                img = Imaging.CreateBitmapSourceFromHIcon(
                                         i.Handle,
                                         new Int32Rect(0, 0, i.Width, i.Height),
                                           BitmapSizeOptions.FromEmptyOptions());
            }
            return img;
        }

        
    }
}
*/