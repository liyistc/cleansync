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

        //Billy's new methods//
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



        //End new methods


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

       public static void CopyFolder(FolderMeta folder, string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize,System.ComponentModel.DoWorkEventArgs eArg)
       {
           CreateDirectory(destination + folder.Path + folder.Name);
           //LogFile.FolderCopy(source, destination);
           foreach (FileMeta file in folder.files)
           {
               if (file != null)
                   CopyFile(source + @"\" + file.Path + @"\" + file.Name, destination + @"\" + file.Path + @"\" + file.Name, worker, onePercentSize,eArg);
           }
           foreach (FolderMeta subFolder in folder.folders)
           {
               if (subFolder != null)
                   CopyFolder( subFolder, source, destination ,worker,onePercentSize,eArg);
           }
       }
       
        public static bool DirectoryIsEmpty(string directory)
        {
            return (Directory.GetFiles(directory).Length == 0 && Directory.GetFiles(directory).Length == 0);
        }

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
            }
            catch (Exception)
            {
                MoveFile(source, target);
            }
        }
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
            }
            catch (Exception)
            {
                MoveFolder(source, target);
            }
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
            if (Directory.GetFiles(source + root.Path + root.Name).Length == 0 && Directory.GetDirectories(source + root.Path + root.Name).Length == 0)
            {
                DeleteFolder(source + root.Path + root.Name);
            }
        }

        //end new
        public static string GetMyDocumentsDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        
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

        //static int buildTreeCount = 0;
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
            //LogFile.FileDeletion(path);
        }

       

        public static void DeleteFolder(string path)
        {
            try
            {              
                ClearFolderAttributes(path);
                Directory.Delete(path, true);
            }
            /*catch (ArgumentNullException)
            {
                throw new ArgumentNullException("The Directory " + path + " cannot be null.");
            }
            catch (DirectoryNotFoundException)
            {
         
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + path + " denied!");
            }*/
            catch (Exception)
            {
                //throw new Exception("Cannot delete folder: " + path);
				System.Threading.Thread.Sleep(20);
                DeleteFolder(path);
            }
            //LogFile.FolderDeletion(path);
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

		private static bool CheckFolderEmpty(FolderMeta folder)
        {
            
            if(!CheckFileEmpty(folder.files)) 
                return false;
            foreach(FolderMeta subFolder in folder.folders)
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
 
        public static void DeleteFolderContent(string path)
        {
          //  if (!Directory.Exists(path))
           //     return;
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
            //LogFile.FileCopy(source, destination);
        }

        public static void CreateDirectory(string destination)
        {
            //if (Directory.Exists(destination)) return;
            try
            {
                if (!Directory.Exists(destination))
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

        public static void CopyFolder(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize,System.ComponentModel.DoWorkEventArgs eArg)
        {
            CreateDirectory(destination);
            //LogFile.FolderCopy(source, destination);
            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\" + Path.GetFileName(file), worker, onePercentSize,eArg);
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder), worker, onePercentSize,eArg);
            }
        }

        public static void CopyFolder(string source,string destination)
        {
            CreateDirectory(destination);
            //LogFile.FolderCopy(source, destination);

            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\"+ Path.GetFileName(file));
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder));
            }
        }
        /*
        public static void CopyFolder(string source, PCJob pcJob, FolderMeta folder, System.ComponentModel.BackgroundWorker worker, double onePercentSize)
        {
            string destination = pcJob.PCPath + folder.Path + folder.Name;
            CreateDirectory(destination);
            LogFile.FolderCopy(source, destination);
            foreach (FileMeta file in folder.files)
            {
                if (file != null)
                    CopyFile(source + @"\" + file.Name, destination + @"\" + file.Name,worker,onePercentSize);
            }
            foreach (FolderMeta subFolder in folder.folders)
            {
                if (subFolder != null)
                    CopyFolder(source + @"\" + subFolder.Name, pcJob, subFolder,worker,onePercentSize);
            }
        }
        */
     
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
    
            CreateDataFolder(GetPCRootPath(pcJob.PCID));

            ReSetPCFolderAccess(pcJob);
            
            DataInputOutput<PCJob>.SaveToBinary(GetStoredPathOnPC(pcJob),pcJob);

            SetPCFolderAccess(pcJob);
            //LogFile.ExportToPC(GetStoredPathOnPC(pcJob));
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

        internal static string GetStoredFolderOnPC(string thisPCID)
        {
            return GetPCRootPath(thisPCID) + @"\JobsList";
        }
        
        

        internal static void ExportUSBJob(USBJob usbJob)
        {
            CreateDataFolder(GetUSBRootPath(usbJob));
            ReSetUSBFolderAccess(GetUSBRootPath(usbJob));
            
            DataInputOutput<USBJob>.SaveToBinary(GetStoredPathOnUSB(usbJob), usbJob);

            SetUSBFolderAccess(GetUSBRootPath(usbJob));
            //LogFile.ExportToUSB(GetStoredPathOnUSB(usbJob));
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
            return GetCurrentDirectory() + @"\_cs_job_data_"+thisPCID.Replace(@":","");
        }

        public static string GetCurrentDirectory()
        {
            try
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+@"\CleanSync";
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

        private static void SetPCFolderAccess(PCJob pcJob)
        {
            if (!Directory.Exists(GetPCRootPath(pcJob.PCID)))
                return;
            try
            {
                DirectoryInfo di = new DirectoryInfo(GetPCRootPath(pcJob.PCID));

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

        /*public static string GetTempStorageFolder(PCJob pcJob)
        {
            return GetTempStorageFolderRoot(pcJob) + @"\"+ pcJob.JobName;
        }

        public static string GetTempStorageFolderRoot(PCJob pcJob)
        {
            return GetCurrentDirectory() + @"\_cs_recent_deleted_files_"+pcJob.PCID.Replace(":","");
        }
        
        public static void CreateTempStorageFolder(PCJob pcJob)
        {
            if (!Directory.Exists(GetTempStorageFolderRoot(pcJob)))
                CreateDirectory(GetTempStorageFolderRoot(pcJob));
            if (!Directory.Exists(GetTempStorageFolder(pcJob)))
                CreateDirectory(GetTempStorageFolder(pcJob));
        }*/

    }
}
