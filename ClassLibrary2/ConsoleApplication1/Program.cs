using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamWriter writer = new StreamWriter("C:\\Users\\Pirororor\\Desktop\\what.txt");
            FolderMeta root = BuildTree("C:\\Users\\Pirororor\\Desktop\\Album 1");
           writer.Write(root.getString());
        }

        private static FolderMeta BuildTree(string rootDir)
        {
           return  BuildTree(rootDir, rootDir);
        }
        /*BuildTree
         * Description: Given a source directory, builds the tree structure of the directory and subdirectories and returns the root.
         * */
        public static FolderMeta BuildTree(string sourceDir, string rootDir)
        {
            FolderMeta thisFolder = new FolderMeta(sourceDir, rootDir);
            // Process the list of files found in the directory.
            string[] fileEntries;
            try
            {
                fileEntries = Directory.GetFiles(sourceDir);
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("Access to: " + sourceDir + " denied!");
            }
            catch (DirectoryNotFoundException)
            {
                throw new DirectoryNotFoundException(sourceDir);
            }
            catch (Exception) { throw; }

            Array.Sort(fileEntries);
            foreach (string fileDir in fileEntries)
            {

                thisFolder.AddFile(new FileMeta(fileDir, rootDir));
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                thisFolder.AddFolder(BuildTree(subdir, rootDir));
            }
            return thisFolder;
        }
    }
}
