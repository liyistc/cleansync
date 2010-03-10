using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DirectoryInformation;
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
            test.Add(null);
            test.Add(null);
            test.Add(null);
            test.Add(null);
            Console.WriteLine("try1: \n");
            foreach (string i in test)
                Console.WriteLine(i == null ? "null" : i);
            tester(test);
            Console.WriteLine("Try 2: \n");

            foreach (string i in test)
                Console.WriteLine(i == null ? "null" : i);
        }

        public static void tester( List<string> folders)
        {
            int lastFreeIndex = 0;
            int pointer = folders.Count - 1;
            bool doing = true;
            while (doing)
            {
                while (lastFreeIndex < pointer && folders[lastFreeIndex] != null)
                {
                    lastFreeIndex++;
                }
                while (lastFreeIndex < pointer && folders[pointer] == null)
                {
                    pointer--;
                }
                if (lastFreeIndex < pointer)
                {
                    folders[lastFreeIndex] = folders[pointer];
                    folders[pointer] = null;
                }
                else doing = false;
            }

            
            while (lastFreeIndex < folders.Count)
            {
                folders.RemoveAt(lastFreeIndex);
            }
        }

    }
}
