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
            /*
            MainLogic MainLogOne = new MainLogic(0);
            MainLogic MainLogTwo = new MainLogic(1);
        

            string PCpath = "C:\\PCOne\\pic";
            string USBpath ="C:\\usb\\test";
            string Jobname = "temp";
            /*
            string PCpath = Console.ReadLine();
            string USBpath = Console.ReadLine();
            string Jobname = Console.ReadLine();


            Console.WriteLine("About to create a clean sync job with name: "+Jobname);
            Console.ReadLine();

            PCJob testJob = MainLogOne.CreateJob(Jobname, PCpath, USBpath);

            Console.WriteLine("Job Created. Job name: " + testJob.JobName + " Job state: " + testJob.JobState + " PC path: " + testJob.PCPath + " USB path: " + testJob.USBPath);
            Console.WriteLine("press any key to start the first sync from PC one to usb");
            Console.ReadLine();

            MainLogOne.FirstTimeSync(testJob);
            //Console.WriteLine(testJob.FolderInfo.Name);

            Console.WriteLine("first sync succeeded.");

            List<string> jobInfoPath = ReadAndWrite.ImportJobList(0);
            List<PCJob> pcJobListOne = ReadAndWrite.GetPCJobs(jobInfoPath);

            /*Console.ReadLine();
            Console.WriteLine("Testing Compare");
            FolderMeta newVersionMeta = ReadAndWrite.BuildTree("F:\\PCOne\\pic");
            CompareLogic compareLogic = new CompareLogic();
            Differences differences = compareLogic.CompareDirectories(newVersionMeta, testJob.FolderInfo);
            compareLogic.compareTest(differences);
            Console.ReadLine();

            //Console.WriteLine("jobName:" + pcJobList.First().JobName + "\nPC Path:" + pcJobList.First().PCPath + "\nUSB Path:" + pcJobList.First().USBPath);
            //Console.WriteLine();

            //List<USBJob> USBjobList = ReadAndWrite.GetIncompleteUSBJobList(USBpath);
            //Console.WriteLine("jobName:" + USBjobList.First().JobName + "\nPC One Path:" + USBjobList.First().PCOnePath + "\nPC Two Path:"+USBjobList.First().PCTwoPath + "\nUSB Path:" + USBjobList.First().USBPath);
            //Console.WriteLine(pcJobList.First().FolderInfo.getString());
            //Console.ReadLine();

            string USB = "C:\\usb";
            string PCpathTwo = "C:\\PCTwo\\pic";
            
            /*
            string USB = Console.ReadLine();
            string PCpathTwo = Console.ReadLine();

            Console.WriteLine("About to accept the half-done job on PC two. press any key to continue");
            Console.ReadLine();

            List<USBJob> incompleteList = MainLogTwo.AcceptJob(USB);
            //Console.WriteLine(incompleteList.First().JobName);
            PCJob jobTwo = MainLogTwo.CreateJob(incompleteList.First(), PCpathTwo);

            Console.WriteLine("job completed. PC path: " + jobTwo.PCPath + " job state: " + jobTwo.JobState + " USB Path: " + jobTwo.USBPath);
            incompleteList = ReadAndWrite.GetIncompleteUSBJobList(USB);
            
            List<string> jobListPath = ReadAndWrite.ImportJobList(1);
            List<PCJob> pcJobsListTwo = ReadAndWrite.GetPCJobs(jobListPath);
            //Console.WriteLine(pcJobs.First().JobName + "\n" + pcJobs.First().PCPath + "\n" + pcJobs.First().USBPath);
            Console.WriteLine("press any key to start the first sync from usb to PC two.");
            Console.ReadLine();
            
            MainLogTwo.AcceptJobSync(pcJobsListTwo.First());
            
            Console.WriteLine("sync succeeded");
            //Console.ReadLine();

            List<string> usbJobListPath = ReadAndWrite.GetUSBJobListPath(USB);
            List<USBJob> usbJobsList = ReadAndWrite.GetUSBJobs(usbJobListPath);
            MainLogOne.setUSBJobList(usbJobsList);

            Console.WriteLine("Ready for second sync with PC one.Press any key to continue");
            Console.ReadLine();
            
            MainLogOne.USBPlugIn(USB);
            ComparisonResult result = MainLogOne.Compare(pcJobListOne.First());
            
            Console.WriteLine("Comparison result:");
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.ComaprePCwithUSBTest(result);

            Console.WriteLine("press any key to proceed the sync");
            Console.ReadLine();

            MainLogOne.CleanSync(result, pcJobListOne.First());
            
            Console.WriteLine("sync succeeded. Ready for sync with PC two.Press any key to continue.");
            Console.ReadLine();

            MainLogTwo.USBPlugIn(USB);
            result = MainLogTwo.Compare(pcJobsListTwo.First());
            
            Console.WriteLine("Comparison result:");
            
            compareLogic.ComaprePCwithUSBTest(result);
            
            Console.WriteLine("press any key to proceed the sync");

            Console.ReadLine();
            
            SyncLogic.CleanSync(result, pcJobsListTwo.First());
            
            Console.WriteLine("sync succeeded. Ready for sync with PC one. Press any key to continue.");
            Console.ReadLine();

            MainLogOne.USBPlugIn(USB);
            result = MainLogOne.Compare(pcJobListOne.First());
            
            Console.WriteLine("Comparison result:");
            compareLogic.ComaprePCwithUSBTest(result);
            
            Console.WriteLine("press any key to proceed the sync");
            Console.ReadLine();
            SyncLogic.CleanSync(result, pcJobListOne.First());

            Console.WriteLine("sync finished.Press any key to exit.");
            Console.ReadLine();
             * */
            AutomatedTestDriver.runTestCase("C:\\test2\\config.txt");
            AutomatedTestDriver.runTestCase("C:\\test\\config.txt");
        }
    }
}
