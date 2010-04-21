/***********************************************************************
 * 
 * *****************CleanSync Version 2.0 ReadAndWrite*****************
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
using System.IO;
using DirectoryInformation;
using CleanSync;
using System.Security.AccessControl;
using System.Security.Principal;
using Exceptions;

namespace CleanSync
{
    public static class ReadAndWrite
    {
        #region Variables
        private static int ExceptionCount = 0;
        private static int Limit = 100;
        #endregion

        #region Meta Tree Building Methods
        /// <summary>
        /// Building Meta Data Tree of a specified directory
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="rootDir"></param>
        /// <returns></returns>
        public static FolderMeta BuildTree(string rootDir)
        {
            return BuildTree(rootDir, rootDir);
        }

        private static FolderMeta BuildTree(string sourceDir, string rootDir)
        {
            FolderMeta thisFolder = new FolderMeta(sourceDir.ToLower(), rootDir.ToLower());
            // Process the list of files found in the directory.
            string[] fileEntries;
            fileEntries = GetDirectoryFiles(sourceDir);

            Array.Sort(fileEntries);
            foreach (string fileDir in fileEntries)
            {

                FileMeta newFile = new FileMeta(fileDir.ToLower(), rootDir.ToLower());
                thisFolder.AddFile(newFile);
                thisFolder.Size += newFile.Size;
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                FolderMeta newFolder = BuildTree(subdir.ToLower(), rootDir.ToLower());
                thisFolder.AddFolder(newFolder);
                thisFolder.Size += newFolder.Size;
            }
            return thisFolder;
        }
        #endregion

        #region File-system Operations
        private static void ClearFolderAttributes(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if ((di.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                di.Attributes = FileAttributes.Directory | FileAttributes.Normal;
            }
            foreach (string file in Directory.GetFiles(path))
            {
                ClearFileAttributes(file);
            }
            foreach (string folder in Directory.GetDirectories(path))
            {
                ClearFolderAttributes(folder);
            }
        }

        private static void ClearFileAttributes(string path)
        {
            FileInfo fi = new FileInfo(path);
            if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                fi.Attributes = FileAttributes.Normal;
            }
        }

        /// <summary>
        /// Delete the file at given path
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            try
            {
                ClearFileAttributes(path);
                File.Delete(path);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("The path of the file " + path + " cannot be null.");
            }
            catch (FileNotFoundException)
            {

            }
            catch (DirectoryNotFoundException)
            {

            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + path + " denied!");
            }
            catch (Exception)
            {
                throw new Exception("Cannot delete file: " + path);
            }
        }

        /// <summary>
        /// Delete the folder at given path
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolder(string path)
        {
            try
            {
                ClearFolderAttributes(path);
                Directory.Delete(path, true);
            }
            catch (Exception)
            {
                System.Threading.Thread.Sleep(20);
                DeleteFolder(path);
            }
        }

        /// <summary>
        /// Delete the folder according to FolderMeta
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="target"></param>
        public static void DeleteFolder(FolderMeta folder, string target)
        {
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder.FolderType == ComponentMeta.Type.Deleted)
                {
                    DeleteFolder(subFolder, target);
                }
            }
            foreach (FileMeta subFile in folder.files)
            {
                if (subFile.FileType == ComponentMeta.Type.Deleted)
                {
                    File.Delete(target + subFile.Path + subFile.Name);
                }
            }
            try
            {
                if (DirectoryIsEmpty(target)) DeleteFolder(target);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Copy file from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyFile(string source, string destination)
        {
            if (!Directory.Exists(Path.GetDirectoryName(destination)))
            {
                CreateDirectory(Path.GetDirectoryName(destination));
            }
            try
            {
                if (File.Exists(destination))
                {
                    FileInfo di = new FileInfo(destination);
                    if ((di.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        di.Attributes = FileAttributes.Normal;
                    }
                    File.Copy(source, destination, true);
                }
                else
                {
                    File.Copy(source, destination, true);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception("Cannot copy file from " + source + "to " + destination);
            }
        }

        /// <summary>
        /// Copy file and report progress
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="worker"></param>
        /// <param name="onePercentSize"></param>
        /// <param name="eArg"></param>
        public static void CopyFile(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize, System.ComponentModel.DoWorkEventArgs eArg)
        {
            try
            {
                if (worker.CancellationPending)
                {
                    eArg.Cancel = true;
                    throw new SyncCancelledException("Synchronization Cancelled!");//Throw exception
                }

                FileInfo fileInfo = new FileInfo(source);

                CopyFile(source, destination);

                if (onePercentSize > 0)
                {
                    worker.ReportProgress((int)(100000.0 * (double)fileInfo.Length / onePercentSize), source);
                }
                else
                {
                    worker.ReportProgress(100000, source);
                }
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("Source of the file " + source + " cannot be null");
            }
            catch (DivideByZeroException)
            {
                throw new DivideByZeroException("Total size of the file cannot be zero.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
        }

        /// <summary>
        /// copy file according to FolderMeta and report progress
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="worker"></param>
        /// <param name="onePercentSize"></param>
        /// <param name="eArg"></param>
        public static void CopyFolder(FolderMeta folder, string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize, System.ComponentModel.DoWorkEventArgs eArg)
        {
            CreateDirectory(destination + folder.Path + folder.Name);

            foreach (FileMeta file in folder.files)
            {
                if (file != null)
                    CopyFile(source + @"\" + file.Path + @"\" + file.Name, destination + @"\" + file.Path + @"\" + file.Name, worker, onePercentSize, eArg);
            }
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder != null)
                    CopyFolder(subFolder, source, destination, worker, onePercentSize, eArg);
            }
        }

        /// <summary>
        /// Copy folder from source to destination
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyFolder(string source, string destination)
        {
            CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\" + Path.GetFileName(file));
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder));
            }
        }

        /// <summary>
        /// Copy folder and report progress
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="worker"></param>
        /// <param name="onePercentSize"></param>
        /// <param name="eArg"></param>
        public static void CopyFolder(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize, System.ComponentModel.DoWorkEventArgs eArg)
        {
            CreateDirectory(destination);

            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\" + Path.GetFileName(file), worker, onePercentSize, eArg);
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder), worker, onePercentSize, eArg);
            }
        }

        /// <summary>
        /// Rename file from oldName to newName
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void RenameFile(string oldName, string newName)
        {
            try
            {
                File.Move(oldName, newName);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException();
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Rename folder from oldName to newName
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        public static void RenameFolder(string oldName, string newName)
        {
            try
            {
                Directory.Move(oldName, newName);
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Verify if a given directory is empty
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool DirectoryIsEmpty(string directory)
        {
            return (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0);
        }

        /// <summary>
        /// Move file from source to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveFile(string source, string target)
        {
            try
            {
                if (!File.Exists(target))
                {
                    if (Directory.GetDirectoryRoot(source).Equals(Directory.GetDirectoryRoot(target)))
                        Directory.Move(source, target);
                    else
                    {
                        CopyFile(source, target);
                        DeleteFile(source);
                    }
                }
                ExceptionCount = 0;
            }
            catch (Exception)
            {
                if (++ExceptionCount > Limit)
                {
                    ExceptionCount = 0;
                    throw;
                }
                MoveFile(source, target);
            }
        }

        /// <summary>
        /// Move folder from source to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveFolder(string source, string target)
        {
            try
            {
                if (!Directory.Exists(target))
                {
                    if (Directory.GetDirectoryRoot(source).Equals(Directory.GetDirectoryRoot(target)))
                        Directory.Move(source, target);
                    else
                    {
                        CopyFolder(source, target);
                        DeleteFolder(source);
                    }
                }
                ExceptionCount = 0;
            }
            catch (Exception)
            {
                if (++ExceptionCount > Limit) 
                { 
                    ExceptionCount = 0;
                    throw; 
                }
                MoveFolder(source, target);
            }
        }

        /// <summary>
        /// Move folder from source to target according to FolderMeta
        /// </summary>
        /// <param name="root"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveFolder(FolderMeta root, string source, string target)
        {
            CreateDirectory(target + root.Path + root.Name);
            foreach (FileMeta file in root.files)
            {
                MoveFile(source + file.Path + file.Name, target + file.Path + file.Name);
            }
            foreach (FolderMeta folder in root.folders)
            {
                MoveFolder(folder, source, target);
            }
            System.Threading.Thread.Sleep(30);
            if (Directory.GetFiles(source + root.Path + root.Name).Length == 0 && Directory.GetDirectories(source + root.Path + root.Name).Length == 0)
            {
                DeleteFolder(source + root.Path + root.Name);
            }
        }

        /// <summary>
        /// Move contents in the folder from source to target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveFolderContents(string source, string target)
        {
            string[] fileEntries = GetDirectoryFiles(source);

            foreach (string fileDir in fileEntries)
            {
                MoveFile(fileDir, target + @"\" + fileDir.Substring(fileDir.LastIndexOf(@"\")));
            }
            string[] subdirEntries = Directory.GetDirectories(source);
            foreach (string subdir in subdirEntries)
            {
                MoveFolder(subdir, target + @"\" + subdir.Substring(subdir.LastIndexOf(@"\")));
            }
        }

        /// <summary>
        /// Move contents in the folder
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void MoveFolderContentWithReplace(string source, string target)
        {
            CreateDirectory(target);

            string[] subfiles = GetDirectoryFiles(source);
            foreach (string subfile in subfiles)
            {
                string fileName = subfile.Substring(subfile.LastIndexOf('\\'));
                fileName = @"\" + fileName;
                if (File.Exists(target + fileName)) DeleteFile(target + fileName);
                MoveFile(subfile, target + fileName);
            }
            string[] subfolders = Directory.GetDirectories(source);

            foreach (string subfolder in subfolders)
            {
                string folderName = subfolder.Substring(subfolder.LastIndexOf('\\'));
                folderName = @"\" + folderName;
                MoveFolderContentWithReplace(subfolder, target + folderName);
                DeleteFolder(source);
            }
        }

        /// <summary>
        /// Set PC folder attribute to Normal
        /// </summary>
        /// <param name="pcJob"></param>
        private static void ReSetPCFolderAccess(PCJob pcJob)
        {
            if (!Directory.Exists(GetPCRootPath(pcJob.PCID)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetPCRootPath(pcJob.PCID));
                di.Attributes = FileAttributes.Normal;
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Set PC folder attribute to hidden
        /// </summary>
        /// <param name="pcJob"></param>
        private static void SetPCFolderAccess(PCJob pcJob)
        {
            if (!Directory.Exists(GetPCRootPath(pcJob.PCID)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetPCRootPath(pcJob.PCID));

                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// set USB folder attribute to Normal
        /// </summary>
        /// <param name="path"></param>
        private static void ReSetUSBFolderAccess(string path)
        {
            if (!Directory.Exists(GetUSBRootPath(path)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetUSBRootPath(path));

                di.Attributes = FileAttributes.Normal;
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Set USB folder attribute to Hidden
        /// </summary>
        /// <param name="path"></param>
        private static void SetUSBFolderAccess(string path)
        {
            if (!Directory.Exists(GetUSBRootPath(path)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetUSBRootPath(path));

                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException();
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Get file list in given directory
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <returns></returns>
        public static string[] GetDirectoryFiles(string sourceDir)
        {
            string[] fileEntries;
            if (!Directory.Exists(sourceDir))
            {
                fileEntries = new string[0];
                return fileEntries;
            }
            try
            {
                fileEntries = Directory.GetFiles(sourceDir);
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + sourceDir + " denied!");
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException(sourceDir);
            }
            catch (Exception) { throw; }
            return fileEntries;
        }

        private static bool CheckFolderEmpty(FolderMeta folder)
        {

            if (!CheckFileEmpty(folder.files))
                return false;
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder == null) continue;
                if (!CheckFolderEmpty(subFolder))
                    return false;
            }
            return true;
        }

        private static bool CheckFileEmpty(List<FileMeta> fileList)
        {
            foreach (FileMeta file in fileList)
            {
                if (file != null)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Delete contents in the folder specified by given path
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFolderContent(string path)
        {
            foreach (string folder in Directory.GetDirectories(path))
            {
                DeleteFolder(folder);
            }
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Empty the folder specified by given path
        /// </summary>
        /// <param name="path"></param>
        public static void EmptyFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        /// <summary>
        /// Create a directory at destination
        /// </summary>
        /// <param name="destination"></param>
        public static void CreateDirectory(string destination)
        {
            try
            {
                if (!Directory.Exists(destination))
                    Directory.CreateDirectory(destination);
                ExceptionCount = 0;
            }
            catch (Exception)
            {
                if (++ExceptionCount > Limit)
                {
                    ExceptionCount = 0;
                    throw;
                }
                CreateDirectory(destination);
            }
        }

        /// <summary>
        /// Get the directory where stores the system data
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectory()
        {
            try
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CleanSync";
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Create the folder storing system data
        /// </summary>
        /// <param name="rootPath"></param>
        public static void CreateDataFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(rootPath);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
                catch (DirectoryNotFoundException)
                {
                    throw new DirectoryNotFoundException();
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentNullException();
                }
                catch (UnauthorizedAccessException)
                {
                    throw new UnauthorizedAccessException();
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
        }
        #endregion

        #region System Directory Management
        internal static void CreateUSBResyncBackUpDirectory(USBJob usbJob)
        {
            if (!Directory.Exists(GetUSBResyncBackUpDirectoryRoot(usbJob.AbsoluteUSBPath)))
                CreateDirectory(GetUSBResyncBackUpDirectoryRoot(usbJob.AbsoluteUSBPath));
            if (!Directory.Exists(GetUSBResyncBackUpDirectory(usbJob)))
                CreateDirectory(GetUSBResyncBackUpDirectory(usbJob));
        }

        internal static string GetUSBResyncBackUpDirectory(USBJob usbJob)
        {
            return GetUSBResyncBackUpDirectoryRoot(usbJob.AbsoluteUSBPath) + @"\" + usbJob.JobName;
        }

        private static string GetUSBResyncBackUpDirectoryRoot(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\ResyncTempBackUp";
        }

        public static void createParentFolders(ComponentMeta meta, string root)
        {
            string dir = root;
            string[] parents = meta.Path.Split('\\');
            foreach (string parent in parents)
            {
                dir += @"\" + parent;
                ReadAndWrite.CreateDirectory(dir);
            }
        }

        public static string GetMyDocumentsDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        internal static void CreatePCBackUpDirectory(PCJob pcJob)
        {
            if (!Directory.Exists(GetPCBackUpFolderRoot(pcJob.PCID)))
                CreateDirectory(GetPCBackUpFolderRoot(pcJob.PCID));
            if (!Directory.Exists(GetPCBackUpFolder(pcJob)))
                CreateDirectory(GetPCBackUpFolder(pcJob));
        }

        internal static string GetPCBackUpFolder(PCJob pcJob)
        {
            return GetPCBackUpFolderRoot(pcJob.PCID) + @"\" + pcJob.JobName;
        }

        private static string GetPCBackUpFolderRoot(string pcID)
        {
            return GetPCRootPath(pcID) + @"\backup";
        }

        internal static string GetPCTempFolder(PCJob pcJob)
        {
            return GetPCTempFolderRoot(pcJob.PCID) + @"\" + pcJob.JobName;
        }

        private static string GetPCTempFolderRoot(string pcID)
        {
            return GetPCRootPath(pcID) + @"\TempStorage";
        }

        internal static void CreatePCTempDirectory(PCJob pcJob)
        {
            if (!Directory.Exists(GetPCTempFolderRoot(pcJob.PCID)))
                CreateDirectory(GetPCTempFolderRoot(pcJob.PCID));
            if (!Directory.Exists(GetPCTempFolder(pcJob)))
                CreateDirectory(GetPCTempFolder(pcJob));
        }

        internal static void CreateUSBTempDirectory(USBJob usbJob)
        {
            if (!Directory.Exists(GetUSBTempFolderRoot(usbJob.AbsoluteUSBPath)))
                CreateDirectory(GetUSBTempFolderRoot(usbJob.AbsoluteUSBPath));
            if (!Directory.Exists(GetUSBTempFolder(usbJob)))
                CreateDirectory(GetUSBTempFolder(usbJob));
        }

        internal static string GetUSBTempFolder(USBJob usbJob)
        {
            return GetUSBTempFolderRoot(usbJob.AbsoluteUSBPath) + @"\" + usbJob.JobName;
        }

        private static string GetUSBTempFolderRoot(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\TempStorage";
        }

        internal static void CreateUSBResyncDirectory(USBJob usbJob)
        {
            if (!Directory.Exists(GetUSBResyncDirectoryRoot(usbJob.AbsoluteUSBPath)))
                CreateDirectory(GetUSBResyncDirectoryRoot(usbJob.AbsoluteUSBPath));
            if (!Directory.Exists(GetUSBResyncDirectory(usbJob)))
                CreateDirectory(GetUSBResyncDirectory(usbJob));
        }

        internal static string GetUSBResyncDirectory(USBJob usbJob)
        {
            return GetUSBResyncDirectoryRoot(usbJob.AbsoluteUSBPath) + @"\" + usbJob.JobName;
        }

        private static string GetUSBResyncDirectoryRoot(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\ResyncTemp";
        }

        internal static string GetStoredPathOnUSB(USBJob usbJob)
        {

            return GetUSBRootPath(usbJob) + @"\usbJobsList\" + usbJob.JobName + ".jInfo";
        }

        internal static string GetStoredPathOnPC(PCJob pcJob)
        {

            return GetPCRootPath(pcJob.PCID) + @"\JobsList\" + pcJob.JobName + ".jInfo";
        }


        internal static string GetIncompleteUSBFilePath(USBJob usbJob)
        {

            return GetUSBRootPath(usbJob) + @"\incompleteJobs\" + usbJob.JobName + ".jUSBInfo";
        }

        internal static string GetIncompleteUSBFolderPath(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\incompleteJobs";
        }

        internal static string GetStoredFolderOnUSB(string usbPath)
        {
            return GetUSBRootPath(usbPath) + @"\usbJobsList";
        }

        internal static string GetStoredFolderOnPC(string thisPCID)
        {
            return GetPCRootPath(thisPCID) + @"\JobsList";
        }

        internal static string GetUSBRootPath(USBJob usbJob)
        {
            return GetRootPath(usbJob) + @"\_CleanSync_Data_\_cs_job_data";
        }

        private static string GetRootPath(USBJob usbJob)
        {
            try
            {
                return Path.GetPathRoot(usbJob.AbsoluteUSBPath);
            }
            catch (Exception)
            {
                throw new Exception("Get Path Root Failed.");
            }
        }

        internal static string GetUSBRootPath(string usbPath)
        {
            return GetRootPath(usbPath) + @"\_CleanSync_Data_\_cs_job_data";
        }

        public static string GetRootPath(string usbPath)
        {
            try
            {
                return Path.GetPathRoot(usbPath);
            }
            catch (Exception)
            {
                throw new Exception("Get Path Root Failed.");
            }
        }

        public static string GetPCRootPath(string thisPCID)
        {
            return GetCurrentDirectory() + @"\_cs_job_data_" + thisPCID.Replace(@":", "");
        }
        #endregion

        #region Job related operations
        internal static void ExportPCJob(PCJob pcJob)
        {

            CreateDataFolder(GetPCRootPath(pcJob.PCID));

            ReSetPCFolderAccess(pcJob);

            DataInputOutput<PCJob>.SaveToBinary(GetStoredPathOnPC(pcJob), pcJob);

            SetPCFolderAccess(pcJob);
        }

        internal static USBJob GetUSBJob(string path)
        {
            ReSetUSBFolderAccess(GetRootPath(path));
            try
            {
                USBJob usbJob = DataInputOutput<USBJob>.LoadFromBinary(path);

                SetUSBFolderAccess(GetRootPath(path));
                return usbJob;
            }
            catch (Exception)
            {
                DeleteFile(path);
                throw new Exception(path);
            }
        }

        internal static List<PCJob> GetPCJobs(string[] pcJobFilePaths)
        {
            List<PCJob> jobLists = new List<PCJob>();
            foreach (string jobPath in pcJobFilePaths)
            {
                try
                {
                    jobLists.Add(DataInputOutput<PCJob>.LoadFromBinary(jobPath));
                }
                catch (Exception)
                {
                    DeleteFile(jobPath);
                    throw new Exception(jobPath);                                 
                }
            }
            return jobLists;
        }

        internal static void ExportUSBJob(USBJob usbJob)
        {
            CreateDataFolder(GetUSBRootPath(usbJob));
            ReSetUSBFolderAccess(GetUSBRootPath(usbJob));

            DataInputOutput<USBJob>.SaveToBinary(GetStoredPathOnUSB(usbJob), usbJob);

            SetUSBFolderAccess(GetUSBRootPath(usbJob));
        }

        internal static void ExportIncompleteJobToUSB(USBJob incompleteUSBJob)
        {
            CreateDataFolder(GetUSBRootPath(incompleteUSBJob.AbsoluteUSBPath));

            ReSetUSBFolderAccess(incompleteUSBJob.AbsoluteUSBPath);

            DataInputOutput<USBJob>.SaveToBinary(GetIncompleteUSBFilePath(incompleteUSBJob), incompleteUSBJob);

            SetUSBFolderAccess(incompleteUSBJob.AbsoluteUSBPath);
        }

        internal static USBJob ImportIncompleteJobFromUSB(string incompleteUSBJobPath)
        {
            USBJob ret;
            ReSetUSBFolderAccess(incompleteUSBJobPath);
            ret = DataInputOutput<USBJob>.LoadFromBinary(incompleteUSBJobPath);
            SetUSBFolderAccess(incompleteUSBJobPath);
            return ret;
        }

        internal static List<USBJob> GetIncompleteUSBJobList(string USBRoot)
        {
            List<USBJob> incompleteUSBJob = new List<USBJob>();
            foreach (string file in Directory.GetFiles(ReadAndWrite.GetIncompleteUSBFolderPath(USBRoot)))
            {
                try
                {
                    incompleteUSBJob.Add(ReadAndWrite.ImportIncompleteJobFromUSB(file));
                }
                catch
                {
                    DeleteFile(file);
                    throw new Exception(file);
                }
            }
            return incompleteUSBJob;
        }

        internal static void RemoveIncompleteUSBJob(USBJob jobUSB)
        {
            DeleteFile(GetIncompleteUSBFilePath(jobUSB));
        }
        #endregion

        #region PC ID File Management
        internal static string GetPCIDFile()
        {
            return GetCurrentDirectory() + @"\cs_pcIDFile.id";
        }

        internal static void CreatePCIDFile(string pcID)
        {
            if (!Directory.Exists(GetCurrentDirectory()))
                CreateDirectory(GetCurrentDirectory());
            File.AppendAllText(GetPCIDFile(), pcID);
        }
        #endregion
    }
}
