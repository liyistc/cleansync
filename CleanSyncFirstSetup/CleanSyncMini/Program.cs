using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cleanSyncMinimalVersion;
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
            Job USBjob = ReadAndWrite.ImportIncompleteFromUSB(USBpath);
            Console.WriteLine("jobName:" + USBjob.jobName + "\nPC Path:" + USBjob.pathPC + "\nUSB Path:" + USBjob.pathUSB);
            Console.WriteLine(USBjob.FM.getString());
            Console.ReadLine();
        }
    }
}
