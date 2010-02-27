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
            PCJob job = new PCJob("C:\\test", "D:\\test");
            List<FileMeta> files = new List<FileMeta>();
            files.Add(new FileMeta("HAHA.txt", "\\a\\b\\"));
            SyncLogic.SyncPCToUSBModifiedFile(job, files);
            foreach(string path in ReadAndWrite.paths)
            {
                Console.WriteLine(path);
            }
        }
    }
}
