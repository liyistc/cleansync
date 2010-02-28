using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;
using CleanSyncMini;

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

                thisFolder.AddFile(new FileMeta(fileDir, rootDir));
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                thisFolder.AddFolder(BuildTree(subdir, rootDir));
            }
            return thisFolder;
        }


        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path,true);
        }

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

        public static void EmptyFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        public static void CopyFile(string soucre,string destination)
        {
            File.Copy(soucre, destination,true);
        }

        public static void CopyFolder(string source,string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\"+ Path.GetFileName(file));
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder));
            } 
        }

        //Modified
        internal static List<string> ImportJobList(int n)
        {
            return DataInputOutput<List<string>>.LoadFromBinary(GetPCJobListPath(n));
        }

       
        internal static USBJob ImportIncompleteJobFromUSB(string incompleteUSBJobPath)
        {
            return DataInputOutput<USBJob>.LoadFromBinary(incompleteUSBJobPath);
        }

        //Modified
        internal static void ExportPCJobPathsList(List<string> pcJobPathsList,int n)
        {
            DataInputOutput<List<string>>.SaveToBinary(GetPCJobListPath(n),pcJobPathsList);
        }

        internal static void ExportPCJob(PCJob pcJob)
        {
            DataInputOutput<PCJob>.SaveToBinary(GetStoredPathOnPC(pcJob),pcJob);
        }
        internal static void ExportIncompleteJobToUSB(USBJob incompleteUSBJob)
        {
            DataInputOutput<USBJob>.SaveToBinary(GetIncompleteUSBFilePath(incompleteUSBJob),incompleteUSBJob);
        }
        

        internal static string GetStoredPathOnPC(PCJob pcJob)
        {            
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\JobsList"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\JobsList");
            return Directory.GetCurrentDirectory() + @"\JobsList\" + pcJob.JobName+pcJob.PCID + ".jInfo";
        }

        //Modified
        internal static string GetPCJobListPath(int n)
        {
            return Directory.GetCurrentDirectory() + @"\jobPathList"+n+".pathList";
        }

        
        internal static string GetIncompleteUSBFilePath(USBJob usbJob)
        {
            if (!Directory.Exists(GetUSBRootPath(usbJob) + @"\incompleteJobs"))
                Directory.CreateDirectory(GetUSBRootPath(usbJob) + @"\incompleteJobs");
            return GetUSBRootPath(usbJob) + @"\incompleteJobs\" + usbJob.JobName + ".jUSBInfo";
        }

        internal static string GetIncompleteUSBFolderPath(string USBPath)
        {
            if (!Directory.Exists(Path.GetPathRoot(USBPath) + @"\usb\incompleteJobs"))
                Directory.CreateDirectory(Path.GetPathRoot(USBPath) + @"\usb\incompleteJobs");
            return Path.GetPathRoot(USBPath) + @"\usb\incompleteJobs";
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
            if (!Directory.Exists(GetUSBRootPath(usbJob) + @"\usbJobsList"))
                Directory.CreateDirectory(GetUSBRootPath(usbJob) + @"\usbJobsList");
            return GetUSBRootPath(usbJob) + @"\usbJobsList\" + usbJob.JobName + ".jInfo";
        }

        internal static void ExportUSBJobPathsList(List<string> StoredUSBJobInfoPaths,USBJob usbJob)
        {
            DataInputOutput<List<string>>.SaveToBinary(GetUSBRootPath(usbJob)+@"\usbJobPathList.pList",StoredUSBJobInfoPaths);
        }

        internal static void ExportUSBJob(USBJob usbJob)
        {
            DataInputOutput<USBJob>.SaveToBinary(GetStoredPathOnUSB(usbJob), usbJob);
        }

        internal static string GetUSBRootPath(USBJob usbJob)
        {
            return Path.GetPathRoot(usbJob.USBPath) + @"\usb";
        }

        internal static List<string> GetUSBJobListPath(string usbPath)
        {
            return DataInputOutput<List<string>>.LoadFromBinary(usbPath+@"\usbJobPathList.pList");
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
    }
}
