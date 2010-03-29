using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;
using CleanSync;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CleanSync
{
    public static class ReadAndWrite
    {
        public static FolderMeta BuildTree(string rootDir)
        {
            return BuildTree(rootDir, rootDir);
        }

        private static FolderMeta BuildTree(string sourceDir, string rootDir)
        {
            FolderMeta thisFolder = new FolderMeta(sourceDir, rootDir);
            // Process the list of files found in the directory.
            string[] fileEntries;
            fileEntries = GetDirectoryFiles(sourceDir);

            Array.Sort(fileEntries);
            foreach (string fileDir in fileEntries)
            {

                FileMeta newFile = new FileMeta(fileDir, rootDir);
                thisFolder.AddFile(newFile);
                thisFolder.Size += newFile.Size;
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                FolderMeta newFolder = BuildTree(subdir, rootDir);
                thisFolder.AddFolder(newFolder);
                thisFolder.Size += newFolder.Size;
            }
            return thisFolder;
        }

        public static string[] GetDirectoryFiles(string sourceDir)
        {
            string[] fileEntries ;
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
                throw new Exception("Cannot delete file: "+path);
            }
            LogFile.FileDeletion(path);
        }

        public static void DeleteFolder(PCJob pcJob, FolderMeta folder)
        {
            foreach (FileMeta file in folder.files)
            {
                DeleteFile(pcJob.PCPath + file.Path + file.Name);
            }
            foreach (FolderMeta subFolder in folder.folders)
            {
                DeleteFolder(pcJob,subFolder);
            }
            if (Directory.GetFiles(pcJob.PCPath + folder.Path + folder.Name).Length == 0 && Directory.GetDirectories(pcJob.PCPath + folder.Path + folder.Name).Length == 0)
                Directory.Delete(pcJob.PCPath + folder.Path + folder.Name);
        }

        public static void DeleteFolder(string path)
        {
            try
            {              
                ClearFolderAttributes(path);
                Directory.Delete(path, true);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("The Directory " + path + " cannot be null.");
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
                throw new Exception("Cannot delete folder: " + path);
            }
            LogFile.FolderDeletion(path);
        }

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

        public static void DeleteFolderContent(string path)
        {
            if (!Directory.Exists(path))
                return;
            foreach (string folder in Directory.GetDirectories(path))
            {
                DeleteFolder(folder);
            }
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        public static void EmptyFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        public static void CopyFile(string source,string destination)
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
            LogFile.FileCopy(source, destination);
        }

        public static void CreateDirectory(string destination)
        {
            if (Directory.Exists(destination)) return;
            try
            {
                Directory.CreateDirectory(destination);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("The destination " + destination + " cannot be null.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + destination + " denied!");
            }
            catch (Exception)
            {
                throw new Exception("Cannot create directory at path: " + destination);
            }
        }

        public static void CopyFile(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(source);
                if (onePercentSize > 0)
                {
                    worker.ReportProgress((int)(100000.0 * (double)fileInfo.Length / onePercentSize), source);
                }
                else
                {
                    worker.ReportProgress(100000, source);
                }
                CopyFile(source, destination);
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

        public static void CopyFolder(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize)
        {
            CreateDirectory(destination);
            LogFile.FolderCopy(source, destination);
            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\" + Path.GetFileName(file), worker, onePercentSize);
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder), worker, onePercentSize);
            }
        }

        public static void CopyFolder(string source,string destination)
        {
            CreateDirectory(destination);
            LogFile.FolderCopy(source, destination);

            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\"+ Path.GetFileName(file));
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder));
            }
        }

        public static void CopyFolder(string source, PCJob pcJob, FolderMeta folder)
        {
            string destination = pcJob.PCPath + folder.Path + folder.Name;
            CreateDirectory(destination);
            LogFile.FolderCopy(source, destination);
            foreach (FileMeta file in folder.files)
            {
                if (file != null)
                    CopyFile(source + @"\" + file.Name, destination + @"\" + file.Name);
            }
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder != null)
                    CopyFolder(source + @"\" + subFolder.Name, pcJob, subFolder);
            }
        }

        public static void RenameFile(string source, string destination)
        {
            try
            {
                File.Move(source, destination);
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

        public static void RenameFolder(string source, string destination)
        {
            try
            {
                Directory.Move(source, destination);
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

        
       
        internal static USBJob ImportIncompleteJobFromUSB(string incompleteUSBJobPath)
        {
            USBJob ret;
            ReSetUSBFolderAccess(incompleteUSBJobPath);
            ret= DataInputOutput<USBJob>.LoadFromBinary(incompleteUSBJobPath);
            SetUSBFolderAccess(incompleteUSBJobPath);
            return ret;
        }

        

        internal static void ExportPCJob(PCJob pcJob)
        {
    
            CreateDataFolder(GetPCRootPath());

            ReSetPCFolderAccess();
            
            DataInputOutput<PCJob>.SaveToBinary(GetStoredPathOnPC(pcJob),pcJob);

            SetPCFolderAccess();
            LogFile.ExportToPC(GetStoredPathOnPC(pcJob));
        }
        internal static void ExportIncompleteJobToUSB(USBJob incompleteUSBJob)
        {
            CreateDataFolder(GetUSBRootPath(incompleteUSBJob.AbsoluteUSBPath));
            
            ReSetUSBFolderAccess(incompleteUSBJob.AbsoluteUSBPath);
            
            DataInputOutput<USBJob>.SaveToBinary(GetIncompleteUSBFilePath(incompleteUSBJob),incompleteUSBJob);

            SetUSBFolderAccess(incompleteUSBJob.AbsoluteUSBPath);
        }
        

        internal static string GetStoredPathOnPC(PCJob pcJob)
        {
            
            return GetPCRootPath() + @"\JobsList\" + pcJob.JobName + ".jInfo";
        }

        

        
        internal static string GetIncompleteUSBFilePath(USBJob usbJob)
        {
            
            return GetUSBRootPath(usbJob) + @"\incompleteJobs\" + usbJob.JobName + ".jUSBInfo";
        }

        internal static string GetIncompleteUSBFolderPath(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\incompleteJobs";
        }

        /*internal static string GetIncompleteUSBFolderLocation(string usbRoot)
        {
            return GetUSBRootPath(usbRoot) + @"\incompleteJobs";
        }*/

        internal static List<USBJob> GetIncompleteUSBJobList(string USBRoot)
        {
            List<USBJob> incompleteUSBJob = new List<USBJob>();
            foreach (string file in Directory.GetFiles(ReadAndWrite.GetIncompleteUSBFolderPath(USBRoot)))
            {
                incompleteUSBJob.Add(ReadAndWrite.ImportIncompleteJobFromUSB(file));
            }
            return incompleteUSBJob;
        }

        internal static List<PCJob> GetPCJobList(string rootFolder)
        {
            List<PCJob> pcJobs = new List<PCJob>();
            foreach (string file in Directory.GetFiles(rootFolder))
            {
                pcJobs.Add(DataInputOutput<PCJob>.LoadFromBinary(file));
            }
            return pcJobs;
        }

        
        

        internal static List<USBJob> GetUSBJobList(string rootFolder)
        {
            List<USBJob> usbJobs = new List<USBJob>();
            foreach (string file in GetDirectoryFiles(rootFolder))
            {
                usbJobs.Add(DataInputOutput<USBJob>.LoadFromBinary(file));
            }
            return usbJobs;
        }

        internal static void RemoveIncompleteUSBJob(USBJob jobUSB)
        {
            DeleteFile(GetIncompleteUSBFilePath(jobUSB));
        }

        internal static List<PCJob> GetPCJobs(string[] pcJobFilePaths)
        {
            List<PCJob> jobLists = new List<PCJob>();
            foreach (string jobPath in pcJobFilePaths)
            {
                jobLists.Add(DataInputOutput<PCJob>.LoadFromBinary(jobPath));
            }
            return jobLists;
        }

        internal static string GetStoredPathOnUSB(USBJob usbJob)
        {
            
            return GetUSBRootPath(usbJob) + @"\usbJobsList\" + usbJob.JobName + ".jInfo";
        }

        internal static USBJob GetUSBJob(string path)
        {
            ReSetUSBFolderAccess(GetRootPath(path));
            USBJob usbJob = DataInputOutput<USBJob>.LoadFromBinary(path);
            SetUSBFolderAccess(GetRootPath(path));
            return usbJob;
        }

        
      
        internal static string GetStoredFolderOnUSB(string usbPath)
        {
            return GetUSBRootPath(usbPath) + @"\usbJobsList";
        }

        internal static string GetStoredFolderOnPC()
        {
            return GetPCRootPath() + @"\JobsList";
        }
        
        

        internal static void ExportUSBJob(USBJob usbJob)
        {
            CreateDataFolder(GetUSBRootPath(usbJob));
            ReSetUSBFolderAccess(GetUSBRootPath(usbJob));
            
            DataInputOutput<USBJob>.SaveToBinary(GetStoredPathOnUSB(usbJob), usbJob);

            SetUSBFolderAccess(GetUSBRootPath(usbJob));
            LogFile.ExportToUSB(GetStoredPathOnUSB(usbJob));
        }

        internal static string GetUSBRootPath(USBJob usbJob)
        {
            return GetRootPath(usbJob) + @"\CleanSync\_cs_job_data";
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
            return GetRootPath(usbPath) + @"\CleanSync\_cs_job_data";
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

        public static string GetPCRootPath()
        {
            return GetCurrentDirectory() + @"\_cs_job_data";
        }

        private static string GetCurrentDirectory()
        {
            try
            {
                return Directory.GetCurrentDirectory();
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

        

        internal static List<USBJob> GetUSBJobs(List<string> usbJobListPath)
        {
            List<USBJob> jobLists = new List<USBJob>();
            foreach (string usbJob in usbJobListPath)
            {
                jobLists.Add(DataInputOutput<USBJob>.LoadFromBinary(usbJob));
            }
            return jobLists;
        }

        public static void CreateDataFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(rootPath);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
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

        

        private static void ReSetPCFolderAccess()
        {
            if (!Directory.Exists(GetPCRootPath()))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetPCRootPath());

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

        private static void SetPCFolderAccess()
        {
            if (!Directory.Exists(GetPCRootPath()))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetPCRootPath());

                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
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
        private static void SetUSBFolderAccess(string path)
        {
            if (!Directory.Exists(GetUSBRootPath(path)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetUSBRootPath(path));

                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
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
}
