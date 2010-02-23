using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanSyncMinimalVersion;
using CleanSync;
using System.IO;

namespace CleanSyncMini
{
    class Program
    {
        static void Main(string[] args)
        {
          /*  DirectoryInformation.FolderMeta root = ReadAndWrite.BuildTree("C:\\Users\\Pirororor\\Desktop\\Album 1");
            (new StreamWriter("C:\\Users\\Pirororor\\Desktop\\haha.txt")).Write(root.getString());
           */ 
            //testcode for compareLogic method: compareDirectories()
            CleanSyncCompare.CompareLogic  compareLogic = new CleanSyncCompare.CompareLogic();
            DirectoryInformation.FolderMeta Folder1Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version1");
            DirectoryInformation.FolderMeta Folder2Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version2");
            DirectoryInformation.Differences  difference  = compareLogic.CompareDirectories(Folder2Meta, Folder1Meta);
            compareLogic.compareTest(difference);
            
        /*    //test code for comparelogic method: comparePCwithUSB()
            CleanSyncCompare.CompareLogic compareLogic = new CleanSyncCompare.CompareLogic();
            DirectoryInformation.FolderMeta Folder1Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version1");
            DirectoryInformation.FolderMeta Folder2Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version2");
            DirectoryInformation.FolderMeta Folder3Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version1");
            DirectoryInformation.FolderMeta Folder4Meta = ReadAndWrite.BuildTree("C:\\Users\\Sanji\\Desktop\\xiaoQ牛btest2010version2");
            DirectoryInformation.Differences difference1 = compareLogic.CompareDirectories(Folder2Meta, Folder1Meta);
            DirectoryInformation.Differences difference2 = compareLogic.CompareDirectories(Folder4Meta, Folder3Meta);
            List<DirectoryInformation.Conflicts> conflictList =  compareLogic.ComparePCwithUSB(difference1, difference2);
            compareLogic.ComaprePCwithUSBTest(difference1, difference2, conflictList);
          */  
       /*     
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
            */

        }
    }
}
