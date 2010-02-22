using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;

namespace Tree
{
    public static class ReadAndWrite
    {
        public static FolderMeta BuildTree(string sourceDir)
        {
            FolderMeta thisFolder = new FolderMeta(sourceDir);
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

                thisFolder.AddFile(new FileMeta(fileDir));
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                thisFolder.AddFolder(BuildTree(subdir));
            }
            return thisFolder;
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
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

        public static List<Job> ImportJobList()
        {
            return DataInputOutput<List<Job>>.LoadFromBinary(Directory.GetCurrentDirectory()+@"/jobList.data");
        }

        public static List<Job> ImportIncompleteJobList()
        {
            return DataInputOutput<List<Job>>.LoadFromBinary(Directory.GetCurrentDirectory() + @"/incompleteJobList.data");
        }

        public static void ExportJobList(List<Job> jobs)
        {
            DataInputOutput<List<Job>>.SaveToBinary(Directory.GetCurrentDirectory() + @"/jobList.data");
        }

        public static void ExportIncompleteJobList(List<Job> incompleteJobs)
        {
            DataInputOutput<List<Job>>.SaveToBinary(Directory.GetCurrentDirectory() + @"/incompleteJobList.data");
        }
    }
}
