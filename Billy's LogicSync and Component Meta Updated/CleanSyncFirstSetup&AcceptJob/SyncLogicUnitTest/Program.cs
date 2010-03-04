using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using TestStubs;

namespace SyncLogicUnitTest
{
    class Tester
    {
        /* To test SyncLogic*/
        static void Main()
        {
           /* PCJob job = new PCJob("C:\\test", "D:\\test");
            List<FileMeta> files = new List<FileMeta>();
            files.Add(new FileMeta("HAHA.txt", "\\a\\b\\"));
            SyncLogic.SyncPCToUSBModifiedFile(job, files);
            foreach(string path in ReadAndWrite.paths)
            {
                Console.WriteLine(path);
            }
            * */

            List<string> test = new List<string>();
            test.Add("1");
            test.Add("HALLO");
            test.Add(null);
            test.Add(null);
            test.Add("HAHAHA");
            Console.WriteLine("try1: \n");
            foreach (string i in test)
                Console.WriteLine(i == null ? "null" : i);
            Console.WriteLine("Try 1 fin: \n");
            tester(test);
            Console.WriteLine("Try 2: \n");

            foreach (string i in test)
                Console.WriteLine(i == null ? "null" : i);
        }

        public static void tester( List<string> folders)
        {
            int lastFreeIndex = 0;
            for (int i = 0; i < folders.Count; i++)
            {
                if (folders[i] != null)
                {
                    Console.WriteLine(folders[i]);
                    if (lastFreeIndex < i)
                    {
                        Console.WriteLine(i + " " + lastFreeIndex);
                        //ReadAndWrite.RenameFolder(job.USBPath + "\\" + listType + i, job.USBPAth + "\\" + listType + lastFreeIndex);
                        folders[lastFreeIndex] = folders[i];
                        folders[i] = null;
                    }
                    lastFreeIndex++;
                }
            }
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }
    }
}
