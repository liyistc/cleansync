using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMini;
using DirectoryInformation;

namespace CleanSyncMini
{
    class Program
    {
        static void Main(string[] args)
        {
            MainLogic MainLogOne = new MainLogic(0);
            MainLogic MainLogTwo = new MainLogic(1);
        

            string PCpath = "C:\\PCOne\\pic";
            string USBpath ="C:\\usb\\test";
            string Jobname = "temp";
            /*
            string PCpath = Console.ReadLine();
            string USBpath = Console.ReadLine();
            string Jobname = Console.ReadLine();*/



            PCJob testJob = MainLogOne.CreateJob(Jobname, PCpath, USBpath);
            MainLogOne.FirstTimeSync(testJob);
            //Console.WriteLine(testJob.FolderInfo.Name);

            List<string> jobInfoPath = ReadAndWrite.ImportJobList(0);
            List<PCJob> pcJobListOne = ReadAndWrite.GetPCJobs(jobInfoPath);

            /*Console.ReadLine();
            Console.WriteLine("Testing Compare");
            FolderMeta newVersionMeta = ReadAndWrite.BuildTree("F:\\PCOne\\pic");
            CompareLogic compareLogic = new CompareLogic();
            Differences differences = compareLogic.CompareDirectories(newVersionMeta, testJob.FolderInfo);
            compareLogic.compareTest(differences);
            Console.ReadLine();*/

            //Console.WriteLine("jobName:" + pcJobList.First().JobName + "\nPC Path:" + pcJobList.First().PCPath + "\nUSB Path:" + pcJobList.First().USBPath);
            //Console.WriteLine();

            List<USBJob> USBjobList = ReadAndWrite.GetIncompleteUSBJobList(USBpath);
            //Console.WriteLine("jobName:" + USBjobList.First().JobName + "\nPC One Path:" + USBjobList.First().PCOnePath + "\nPC Two Path:"+USBjobList.First().PCTwoPath + "\nUSB Path:" + USBjobList.First().USBPath);
            //Console.WriteLine(pcJobList.First().FolderInfo.getString());
            //Console.ReadLine();

            string USB = "C:\\usb";
            string PCpathTwo = "C:\\PCTwo\\pic";
            
            /*
            string USB = Console.ReadLine();
            string PCpathTwo = Console.ReadLine();*/
           
            List<USBJob> incompleteList = MainLogTwo.AcceptJob(USB);
            //Console.WriteLine(incompleteList.First().JobName);
            MainLogTwo.CreateJob(incompleteList.First(), PCpathTwo);
            
            incompleteList = ReadAndWrite.GetIncompleteUSBJobList(USB);
            
            List<string> jobListPath = ReadAndWrite.ImportJobList(1);
            List<PCJob> pcJobsListTwo = ReadAndWrite.GetPCJobs(jobListPath);
            //Console.WriteLine(pcJobs.First().JobName + "\n" + pcJobs.First().PCPath + "\n" + pcJobs.First().USBPath);

            MainLogTwo.AcceptJobSync(pcJobsListTwo.First());
            
            Console.ReadLine();

            List<string> usbJobListPath = ReadAndWrite.GetUSBJobListPath(USB);
            List<USBJob> usbJobsList = ReadAndWrite.GetUSBJobs(usbJobListPath);
            MainLogOne.setUSBJobList(usbJobsList);

            MainLogOne.USBPlugIn(USB);
            ComparisonResult result = MainLogOne.Compare(pcJobListOne.First());

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.ComaprePCwithUSBTest(result);

            Console.ReadLine();

            MainLogOne.CleanSync(result, pcJobListOne.First());
            
            Console.WriteLine("second sync");
            Console.ReadLine();

            MainLogTwo.USBPlugIn(USB);
            result = MainLogTwo.Compare(pcJobsListTwo.First());
            compareLogic.ComaprePCwithUSBTest(result);
            Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobsListTwo.First());
            Console.ReadLine();

            MainLogOne.USBPlugIn(USB);
            result = MainLogOne.Compare(pcJobListOne.First());
            compareLogic.ComaprePCwithUSBTest(result);
            Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobListOne.First());
            Console.ReadLine();
        }
    }
}
