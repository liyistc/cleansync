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
            FolderMeta root = BuildTree("C:\\Users\\Pirororor\\Desktop\\Album 1");
            StreamWriter writer = new StreamWriter("C:\\Users\\Pirororor\\Desktop\\Haha.txt");
            writer.Write(root.getString());
        }
        /*BuildTree
         * Description: Given a source directory, builds the tree structure of the directory and subdirectories and returns the root.
         * */
        public static FolderMeta BuildTree(string sourceDir)
        {
            FolderMeta thisFolder = new FolderMeta(sourceDir);
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

                thisFolder.AddFile(new FileMeta(fileDir));
            }
            // Recurse into subdirectories of this directory.
            string[] subdirEntries = Directory.GetDirectories(sourceDir);
            Array.Sort(subdirEntries);
            foreach (string subdir in subdirEntries)
            {
                thisFolder.AddFolder(BuildTree(subdir));
            }
            return thisFolder;
        }
    }
}
