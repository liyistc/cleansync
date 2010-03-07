using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DirectoryInformation;

namespace CleanSyncMini
{
    public static class AutomatedTestDriver

    {
        public static void runTestCase(string testpath)
        {
            Console.WriteLine("************* Test Case: " + Path.GetDirectoryName(testpath) + " ****************");

            StreamReader reader = new StreamReader(testpath);
            MainLogic MainLogOne = new MainLogic(0);
            MainLogic MainLogTwo = new MainLogic(1);

            string USBpath = "C:\\usb\\test";
            string Jobname = "temp";
            string USB = "C:\\usb";

            // Read the config file
            string PCpath = reader.ReadLine();  
            string PCpathTwo = reader.ReadLine();
            string modifiedPCpath = reader.ReadLine();
            string modifiedPCpathTwo = reader.ReadLine();
            string oriPCPath = reader.ReadLine();
            string oriPCPathTwo = reader.ReadLine();
            string configFilePath = reader.ReadLine();

            PCJob testJob = MainLogOne.CreateJob(Jobname, PCpath, USBpath);
            MainLogOne.FirstTimeSync(testJob);

            List<string> jobInfoPath = ReadAndWrite.ImportJobList(0);
            List<PCJob> pcJobListOne = ReadAndWrite.GetPCJobs(jobInfoPath);

            List<USBJob> USBjobList = ReadAndWrite.GetIncompleteUSBJobList(USBpath);

            
            //string PCpathTwo = "C:\\test\\PCTwo\\pic";
            List<USBJob> incompleteList = MainLogTwo.AcceptJob(USB);

            MainLogTwo.CreateJob(incompleteList.First(), PCpathTwo);
            incompleteList = ReadAndWrite.GetIncompleteUSBJobList(USB);

            List<string> jobListPath = ReadAndWrite.ImportJobList(1);
            List<PCJob> pcJobsListTwo = ReadAndWrite.GetPCJobs(jobListPath);
        

            MainLogTwo.AcceptJobSync(pcJobsListTwo.First());

            //Console.ReadLine();
            //Apply modification to pc1.
            DeleteFolder(PCpath);
            
            CopyFolder(modifiedPCpath, PCpath);

            List<string> usbJobListPath = ReadAndWrite.GetUSBJobListPath(USB);
            List<USBJob> usbJobsList = ReadAndWrite.GetUSBJobs(usbJobListPath);
            MainLogOne.setUSBJobList(usbJobsList);

            MainLogOne.USBPlugIn(USB);
            ComparisonResult result = MainLogOne.Compare(pcJobListOne.First());

            CompareLogic compareLogic = new CompareLogic();
            Console.WriteLine("--------------------------------");
            compareLogic.ComaprePCwithUSBTest(result);

            //Console.ReadLine();
           

            MainLogOne.CleanSync(result, pcJobListOne.First());

            //Console.WriteLine("second sync");
            //Console.ReadLine();
            //Apply modification to pctwo
            DeleteFolder(PCpathTwo);
            
            CopyFolder(modifiedPCpathTwo, PCpathTwo);

            MainLogTwo.USBPlugIn(USB);
            result = MainLogTwo.Compare(pcJobsListTwo.First());
            Console.WriteLine("--------------------------------");
            compareLogic.ComaprePCwithUSBTest(result);
            //Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobsListTwo.First());
            //Console.ReadLine();

            MainLogOne.USBPlugIn(USB);
            result = MainLogOne.Compare(pcJobListOne.First());
            Console.WriteLine("--------------------------------");
            compareLogic.ComaprePCwithUSBTest(result);
            //Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobListOne.First());

            
            Console.WriteLine("\nLog file compare result:");
            compareFiles("CleanSyncLog.txt", configFilePath);
            
            Console.ReadLine();
            //Recover contents
            DeleteFolder(PCpath);
            DeleteFolder(PCpathTwo);

            
            CopyFolder(oriPCPath, PCpath);
            CopyFolder(oriPCPathTwo, PCpathTwo);
            DeleteFile("CleanSyncLog.txt");
            DeleteFolderContent("C:\\usb");
            
            reader.Close();
        }

        private static void CopyFile(string source, string destination)
        {
            File.Copy(source, destination, true);
        }

        private static void CopyFolder(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            
            foreach (string file in Directory.GetFiles(source))
            {
                CopyFile(file, destination + @"\" + Path.GetFileName(file));
            }
            foreach (string folder in Directory.GetDirectories(source))
            {
                CopyFolder(folder, destination + @"\" + Path.GetFileName(folder));
            }
        }

        private static void DeleteFile(string path)
        {
            File.Delete(path);
            //LogFile.FileDeletion(path);
        }

        private static void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
            //LogFile.FolderDeletion(path);
        }

        private static void DeleteFolderContent(string path)
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

        private static void compareFiles(string path1, string path2)
        {
            FileStream file1 = new FileStream(path1, FileMode.Open);
            FileStream file2 = new FileStream(path2, FileMode.Open);
            int i = 0, j = 0;

            try
            {
                do
                {
                    i = file1.ReadByte();
                    j = file2.ReadByte();
                    if (i != j) break;
                } while (i != -1 && j != -1);
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.Message);
            }
            if (i != j)
            {
                Console.WriteLine("Log files differ.");
                //return false;
            }
            else
            {
                Console.WriteLine("Log files are the same.");
                //return true;
            }
            file1.Close();
            file2.Close();  
        }
    }
}