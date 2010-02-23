using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMinimalVersion;
using CleanSync;

namespace CleanSyncMini
{
    class Program
    {
        static void Main(string[] args)
        {
            MainLogic MainLog = new MainLogic();

            
            string PCpath = Console.ReadLine();
            string USBpath = Console.ReadLine();
            string Jobname = Console.ReadLine();

            Job testJob = MainLog.CreateJob(PCpath, USBpath, Jobname);
            MainLog.FirstTimeSync(testJob);

            List<Job> jobList = ReadAndWrite.ImportIncompleteJobList();
            Console.WriteLine("jobName:" + jobList.First().jobName + "\nPC Path:" + jobList.First().pathPC + "\nUSB Path:" + jobList.First().pathUSB);
            Console.WriteLine();
            List<JobUSB> USBjobList = ReadAndWrite.ImportIncompleteFromUSB(USBpath);
            Console.WriteLine("jobName:" + USBjobList.First().jobName + "\nPC Path:" + USBjobList.First().pathOnePC + "\nUSB Path:" + USBjobList.First().pathUSB);
            Console.WriteLine(USBjobList.First().rootFolder.getString());
            //Console.ReadLine();
            

            string USB = Console.ReadLine();
            string PCpathTwo = Console.ReadLine();
            List<JobUSB> incompleteList = MainLog.AcceptJob(USB);
            //Console.WriteLine(incompleteList.First().jobName);
            MainLog.CreateJob(incompleteList.First(), PCpathTwo);
            incompleteList = ReadAndWrite.ImportIncompleteFromUSB(USB);
            List<Job> completeJobList = ReadAndWrite.ImportJobList();
            Console.WriteLine(completeJobList.First().jobName + "\n" + completeJobList.First().pathPC + "\n" + completeJobList.First().pathUSB);
            
            MainLog.AcceptJobSync(completeJobList.First());
            Console.ReadLine();

        }
    }
}
