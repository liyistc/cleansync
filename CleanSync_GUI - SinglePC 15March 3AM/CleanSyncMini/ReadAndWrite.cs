using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;
using CleanSyncMini;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CleanSyncMini
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

        public static void DeleteFile(string path)
        {
            File.Delete(path);
            LogFile.FileDeletion(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path,true);
            LogFile.FolderDeletion(path);
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
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }
            File.Copy(source, destination,true);
            LogFile.FileCopy(source, destination);
        }

        public static void CopyFile(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize)
        {
            FileInfo fileInfo = new FileInfo(source);

            worker.ReportProgress((int)(100000.0 * (double)fileInfo.Length / onePercentSize), source);
            CopyFile(source, destination);
        }

        public static void CopyFolder(string source, string destination, System.ComponentModel.BackgroundWorker worker, double onePercentSize)
        {
            Directory.CreateDirectory(destination);
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
            Directory.CreateDirectory(destination);
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

        public static void RenameFile(string source, string destination)
        {
            File.Move(source, destination);
        }

        public static void RenameFolder(string source, string destination)
        {
            Directory.Move(source, destination);
        }

        //Modified
        internal static List<string> ImportJobList()
        {
            List<string> ret;
            ReSetPCFolderAccess();
            ret = DataInputOutput<List<string>>.LoadFromBinary(GetPCJobListPath());
            SetPCFolderAccess();
            return ret;
        }
       
        internal static USBJob ImportIncompleteJobFromUSB(string incompleteUSBJobPath)
        {
            USBJob ret;
            ReSetUSBFolderAccess(incompleteUSBJobPath);
            ret= DataInputOutput<USBJob>.LoadFromBinary(incompleteUSBJobPath);
            SetUSBFolderAccess(incompleteUSBJobPath);
            return ret;
        }

        //Modified
        internal static void ExportPCJobPathsList(List<string> pcJobPathsList)
        {
            CreateDataFolder(GetPCRootPath());
            ReSetPCFolderAccess();
            string path = GetPCJobListPath();

            DataInputOutput<List<string>>.SaveToBinary(path,pcJobPathsList);

            SetPCFolderAccess();
            
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
            /*DirectoryInfo di;
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\JobsList"))
            {
                di = Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\JobsList");
                di.Attributes = FileAttributes.Directory | FileAttributes.System | FileAttributes.Hidden;
            }*/
            return GetPCRootPath() + @"\JobsList\" + pcJob.JobName + ".jInfo";
        }

        //Modified
        internal static string GetPCJobListPath()
        {
            return GetPCRootPath() + @"\jobPathList" + ".pList";
        }

        internal static string GetUSBJobListPathLocation(string usbPath)
        {
            return GetUSBRootPath(usbPath) + @"\usbJobPathList.pList";
        }   
        internal static string GetIncompleteUSBFilePath(USBJob usbJob)
        {
            /*if (!Directory.Exists(GetUSBRootPath(usbJob) + @"\incompleteJobs"))
            {
                Directory.CreateDirectory(GetUSBRootPath(usbJob) + @"\incompleteJobs");          
            }*/
            return GetUSBRootPath(usbJob) + @"\incompleteJobs\" + usbJob.JobName + ".jUSBInfo";
        }

        internal static string GetIncompleteUSBFolderPath(string AbsoluteUSBPath)
        {
            return GetUSBRootPath(AbsoluteUSBPath) + @"\incompleteJobs";
        }

        internal static string GetIncompleteUSBFolderLocation(string usbRoot)
        {
            return GetUSBRootPath(usbRoot) + @"\incompleteJobs";
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
            foreach (string file in Directory.GetFiles(rootFolder))
            {
                usbJobs.Add(DataInputOutput<USBJob>.LoadFromBinary(file));
            }
            return usbJobs;
        }

        internal static void RemoveIncompleteUSBJob(USBJob jobUSB)
        {
            DeleteFile(GetIncompleteUSBFilePath(jobUSB));
        }

        internal static List<PCJob> GetPCJobs(List<string> jobPaths)
        {
            List<PCJob> jobLists = new List<PCJob>();
            foreach (string jobPath in jobPaths)
            {
                jobLists.Add(DataInputOutput<PCJob>.LoadFromBinary(jobPath));
            }
            return jobLists;
        }

        internal static string GetStoredPathOnUSB(USBJob usbJob)
        {
            
            return GetUSBRootPath(usbJob) + @"\usbJobsList\" + usbJob.JobName + ".jInfo";
        }    
      
        internal static string GetStoredFolderOnUSB(string usbPath)
        {
            return GetUSBRootPath(usbPath) + @"\usbJobsList";
        }
        
        internal static void ExportUSBJobPathsList(List<string> StoredUSBJobInfoPaths,USBJob usbJob)
        {
            List<string> export = new List<string>();
            foreach (string path in StoredUSBJobInfoPaths)
                export.Add(path.Substring(Path.GetPathRoot(path).Length));
            
            string filePath = GetUSBJobListPathLocation(usbJob.AbsoluteUSBPath);
            
            string folderPath = GetUSBRootPath(usbJob.AbsoluteUSBPath);
            CreateDataFolder(folderPath);

            ReSetUSBFolderAccess(GetUSBRootPath(usbJob));
            DataInputOutput<List<string>>.SaveToBinary(filePath,export);
            SetUSBFolderAccess(GetUSBRootPath(usbJob));
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
            return Path.GetPathRoot(usbJob.AbsoluteUSBPath) +@"\_cs_job_data";
        }

        internal static string GetUSBRootPath(string usbPath)
        {
            return Path.GetPathRoot(usbPath) + @"\_cs_job_data";
        }

        private static string GetPCRootPath()
        {
            return Directory.GetCurrentDirectory()+@"\_cs_job_data";
        }

        internal static List<string> GetUSBJobListPath(string AbsoluteUSBPath)
        {
            List<string> ret;
            ReSetUSBFolderAccess(AbsoluteUSBPath);
            ret= DataInputOutput<List<string>>.LoadFromBinary(GetUSBJobListPathLocation(AbsoluteUSBPath));
            SetUSBFolderAccess(AbsoluteUSBPath);
            return ret;
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

        private static void CreateDataFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(rootPath);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
            }
        }

        /*internal static void MoveUSBJobToDelete(USBJob usbJob,string pcID)
        {
            string path = GetDeleteJobUSBFolder(usbJob.AbsoluteUSBPath)+@"\"+Path.GetFileName(GetStoredPathOnUSB(usbJob));
            if (pcID.Equals(usbJob.PCOneID))
                usbJob.DeletePCID = usbJob.PCTwoID;
            else
                usbJob.DeletePCID = usbJob.PCOneID;
            ExportUSBJob(usbJob);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Move(GetStoredPathOnUSB(usbJob), path);            
            
            //usbJob.RelativeUSBPath = path.Substring(Path.GetPathRoot(path).Length);
        }*/

        /*internal static void MovePCJobToDelete(PCJob pcJob)
        {
            string path = GetDeleteJobPCFolder()+@"\"+Path.GetFileName(GetStoredPathOnPC(pcJob));

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            File.Move(GetStoredPathOnPC(pcJob),path);
            
            //pcJob.PCPath = path;
        }

        internal static string GetDeleteJobUSBFolder(string usbRoot)
        {
            return GetUSBRootPath(usbRoot) + @"\deletedJobs";
        }

        internal static string GetDeleteJobPCFolder()
        {
            return GetPCRootPath() + @"\deletedJobs";
        }*/

        private static void ReSetPCFolderAccess()
        {
            if (!Directory.Exists(GetPCRootPath()))
                return;
            DirectoryInfo di = new DirectoryInfo(GetPCRootPath());
            GrantAccess(GetPCRootPath());
            di.Attributes = FileAttributes.Normal;
        }

        private static void SetPCFolderAccess()
        {
            if (!Directory.Exists(GetPCRootPath()))
                return;
            DirectoryInfo di = new DirectoryInfo(GetPCRootPath());
            GrantAccess(GetPCRootPath());
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
        }

        private static void ReSetUSBFolderAccess(string path)
        {
            if (!Directory.Exists(GetUSBRootPath(path)))
                return;
            DirectoryInfo di = new DirectoryInfo(GetUSBRootPath(path));
            GrantAccess(GetUSBRootPath(path));
            di.Attributes = FileAttributes.Normal;
        }
        private static void SetUSBFolderAccess(string path)
        {
            if (!Directory.Exists(GetUSBRootPath(path)))
                return;
            DirectoryInfo di = new DirectoryInfo(GetUSBRootPath(path));
            GrantAccess(GetUSBRootPath(path));
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden | FileAttributes.System;
        }

        private static void GrantAccess(string folderPath)
        {
            WindowsIdentity self = System.Security.Principal.WindowsIdentity.GetCurrent();
            FileSystemAccessRule rule = new FileSystemAccessRule(
               self.Name, FileSystemRights.FullControl,
               AccessControlType.Allow);
            // add the rule to the file's existing ACL list
            DirectorySecurity security = Directory.GetAccessControl(folderPath);
            /*AuthorizationRuleCollection acl = security.GetAccessRules(
               true, true, typeof(System.Security.Principal.NTAccount));*/
            security.AddAccessRule(rule);
            // persist changes
            Directory.SetAccessControl(folderPath, security);
        }

        /*internal static string GetDeletedPCJobPath(PCJob pcJob)
        {
            return GetDeleteJobPCFolder() + @"\" + Path.GetFileName(GetStoredPathOnPC(pcJob));
        }

        internal static string GetDeletedUSBJobPath(USBJob usbJob)
        {
            return GetDeleteJobUSBFolder(usbJob.AbsoluteUSBPath) + @"\" + Path.GetFileName(GetStoredPathOnUSB(usbJob));
        }*/
    }
}
