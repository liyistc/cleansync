﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;
using CleanSyncMinimalVersion;

namespace CleanSync
{
    public static class ReadAndWrite
    {
        public static FolderMeta BuildTree(string sourceDir)
        {
            return BuildTree(sourceDir, sourceDir);
        }

        public static FolderMeta BuildTree(string sourceDir, string rootDir)
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
               // Console.WriteLine(fileDir);
                thisFolder.AddFile(new FileMeta(fileDir, rootDir));
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                //Console.WriteLine("subdir: " + subdir);
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

        public static void EmptyFolder(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                DeleteFile(file);
            }
        }

        public static void CopyFile(string source,string destination)
        {
            File.Copy(source, destination,true);
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

        internal static List<Job> ImportJobList()
        {
            return DataInputOutput<List<Job>>.LoadFromBinary(Directory.GetCurrentDirectory()+@"/jobList.data");
        }

        internal static List<Job> ImportIncompleteJobList()
        {
            return DataInputOutput<List<Job>>.LoadFromBinary(Directory.GetCurrentDirectory() + @"/incompleteJobList.data");
        }

        internal static List<JobUSB> ImportIncompleteFromUSB(string USBPath)
        {
            return DataInputOutput<List<JobUSB>>.LoadFromBinary(Path.GetPathRoot(USBPath) + @"incompleteJob.data");
        }

        internal static void ExportJobList(List<Job> jobs)
        {
            DataInputOutput<List<Job>>.SaveToBinary(Directory.GetCurrentDirectory() + @"/jobList.data",jobs);
        }

        internal static void ExportIncompleteJobList(List<Job> incompleteJobs)
        {
            DataInputOutput<List<Job>>.SaveToBinary(Directory.GetCurrentDirectory() + @"/incompleteJobList.data",incompleteJobs);
        }
        internal static void ExportIncompleteToUSB(List<JobUSB> incompleteUSBJobs, string path)
        {
            DataInputOutput<List<JobUSB>>.SaveToBinary(path + @"incompleteJob.data", incompleteUSBJobs);
        }
        internal static void ExportCompleteToUSB(Job completeJob)
        {
            DataInputOutput<Job>.SaveToBinary(completeJob.pathUSB + @"/completeJob.data", completeJob);
        }
    }
}