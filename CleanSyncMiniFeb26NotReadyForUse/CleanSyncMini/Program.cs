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
            MainLogic MainLog = new MainLogic();
        

            string PCpath = "F:\\PCOne\\pic";
            string USBpath ="F:\\usb\\test";
            string Jobname = "temp";
            /*
            string PCpath = Console.ReadLine();
            string USBpath = Console.ReadLine();
            string Jobname = Console.ReadLine();*/



            PCJob testJob = MainLog.CreateJob(Jobname, PCpath, USBpath);
            MainLog.FirstTimeSync(testJob);
            Console.WriteLine(testJob.FolderInfo.Name);

            List<string> jobInfoPath = ReadAndWrite.ImportJobList();
            List<PCJob> pcJobList = ReadAndWrite.GetPCJobs(jobInfoPath);

            Console.ReadLine();
            Console.WriteLine("Testing Compare");
            FolderMeta newVersionMeta = ReadAndWrite.BuildTree("F:\\PCOne\\pic");
            CompareLogic compareLogic = new CompareLogic();
            Differences differences = compareLogic.CompareDirectories(newVersionMeta, testJob.FolderInfo);
            compareLogic.compareTest(differences);
            Console.ReadLine();

            Console.WriteLine("jobName:" + pcJobList.First().JobName + "\nPC Path:" + pcJobList.First().PCPath + "\nUSB Path:" + pcJobList.First().USBPath);
            Console.WriteLine();

            List<USBJob> USBjobList = ReadAndWrite.GetIncompleteUSBJobList(USBpath);
            Console.WriteLine("jobName:" + USBjobList.First().JobName + "\nPC One Path:" + USBjobList.First().PCOnePath + "\nPC Two Path:"+USBjobList.First().PCTwoPath + "\nUSB Path:" + USBjobList.First().USBPath);
            Console.WriteLine(pcJobList.First().FolderInfo.getString());
            //Console.ReadLine();

            string USB = "F:\\usb";
            string PCpathTwo = "F:\\PCTwo\\pic";
            
            /*
            string USB = Console.ReadLine();
            string PCpathTwo = Console.ReadLine();*/
           
            List<USBJob> incompleteList = MainLog.AcceptJob(USB);
            //Console.WriteLine(incompleteList.First().JobName);
            MainLog.CreateJob(incompleteList.First(), PCpathTwo);
            
            incompleteList = ReadAndWrite.GetIncompleteUSBJobList(USB);
            
            List<string> jobListPath = ReadAndWrite.ImportJobList();
            List<PCJob> pcJobs = ReadAndWrite.GetPCJobs(jobListPath);
            Console.WriteLine(pcJobs.First().JobName + "\n" + pcJobs.First().PCPath + "\n" + pcJobs.First().USBPath);

            MainLog.AcceptJobSync(pcJobs.First());
            Console.ReadLine();

           

        }
    }
}
