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
            /*Console.WriteLine("************* Test Case: " + Path.GetDirectoryName(testpath) + " ****************");

            

            StreamReader reader = new StreamReader(testpath);
            MainLogic MainLogOne = new MainLogic();
            MainLogic MainLogTwo = new MainLogic();

            string AbsoluteUSBPath = "C:\\usb\\test";
            string Jobname = "temp";
            List<string> USB = new List<string>();
            USB.Add(@"C:\");
            USB.Add(@"G:\");

            // Read the config file
            string PCpath = reader.ReadLine();  
            string PCpathTwo = reader.ReadLine();
            string modifiedPCpath = reader.ReadLine();
            string modifiedPCpathTwo = reader.ReadLine();
            string oriPCPath = reader.ReadLine();
            string oriPCPathTwo = reader.ReadLine();
            string configFilePath = reader.ReadLine();

            //Recover contents
            DeleteFolder(PCpath);
            DeleteFolder(PCpathTwo);


            CopyFolder(oriPCPath, PCpath);
            CopyFolder(oriPCPathTwo, PCpathTwo);
            DeleteFile("CleanSyncLog.txt");
            DeleteFolderContent("C:\\usb");

            PCJob testJob = MainLogOne.CreateJob(Jobname, PCpath, AbsoluteUSBPath);
            MainLogOne.FirstTimeSync(testJob);

            List<PCJob> pcJobListOne = MainLogOne.GetPCJobs();
            
            List<USBJob> incompleteList = MainLogTwo.AcceptJob(USB);

            MainLogTwo.CreateJob(incompleteList.First(), PCpathTwo);
            
            MainLogTwo.USBPlugIn(USB);

            List<PCJob> pcJobsListTwo = MainLogTwo.GetPCJobs();
           
            ComparisonResult result =  MainLogTwo.Compare(pcJobsListTwo.First());

            int[] userChoice = new int[result.conflictList.Count];
            for (int i = 0; i < userChoice.Count(); i++)
            {
                userChoice[i] = 1;
            }
            result = MainLogTwo.handleConflicts(result, userChoice);

            MainLogTwo.CleanSync(result, pcJobsListTwo.First());
            //Console.ReadLine();
            //Apply modification to pc1.
            DeleteFolder(PCpath);
            
            CopyFolder(modifiedPCpath, PCpath);

            MainLogOne.USBPlugIn(USB);
            result = MainLogOne.Compare(pcJobListOne.First());
            
            userChoice = new int[result.conflictList.Count];
            for(int i=0; i < userChoice.Count();i++)
            {
                userChoice[i] = 1;
            }
            result = MainLogOne.handleConflicts(result, userChoice);

            CompareLogic compareLogic = new CompareLogic();
            Console.WriteLine("--------------PC2ToPC1------------------");
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
            userChoice = new int[result.conflictList.Count];
            for (int i = 0; i < userChoice.Count(); i++)
            {
                userChoice[i] = 1;
            }
            result = MainLogTwo.handleConflicts(result, userChoice);
            Console.WriteLine("--------------PC1toPC2------------------");
            compareLogic.ComaprePCwithUSBTest(result);
            //Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobsListTwo.First());
            //Console.ReadLine();

            MainLogOne.USBPlugIn(USB);
            result = MainLogOne.Compare(pcJobListOne.First());
            userChoice = new int[result.conflictList.Count];
            result = MainLogOne.handleConflicts(result, userChoice);
            Console.WriteLine("--------------PC2ToPC1------------------");
            compareLogic.ComaprePCwithUSBTest(result);
            //Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobListOne.First());

            
            Console.WriteLine("\nLog file compare result:");
            compareFiles("CleanSyncLog.txt", configFilePath);
            
            Console.ReadLine();
            
            
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
            file2.Close();  */
        }
    }
}