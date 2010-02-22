using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;

namespace ConsoleApplication1
{
    class CompareLogic
    {
        public CompareLogic()
        {}

        public InitializationDifferences DoInitializationCompare(FolderMeta internalFolder, FolderMeta externalFolder)
        {
            InitializationDifferences differences = new InitializationDifferences();
            DoInitializationCompare(internalFolder, externalFolder, differences);
            return differences;
        }

        public void DoInitializationCompare(FolderMeta internalFolder, FolderMeta externalFolder, InitializationDifferences differences)
        {
            DoInitializationFileCompare(internalFolder, externalFolder, differences);
            DoInitializationFolderCompare(internalFolder, externalFolder, differences);
        }

        private void DoInitializationFolderCompare(FolderMeta internalFolder, FolderMeta externalFolder, InitializationDifferences differences)
        {
            IEnumerator<FolderMeta> internalFolderSubFolders = internalFolder.GetFolders();
            IEnumerator<FolderMeta> externalFolderSubFolders = externalFolder.GetFolders();

            internalFolderSubFolders.MoveNext();
            externalFolderSubFolders.MoveNext();
            FolderMeta internalSubFolder = internalFolderSubFolders.Current;
            FolderMeta externalSubFolder = externalFolderSubFolders.Current;

            while (internalSubFolder != null && externalSubFolder != null)
            {

                if (internalSubFolder < externalSubFolder)
                {
                    /*
                     * 
                     * Add new folder of internalSubFolder in externalSubFolder
                     * 
                     * 
                     * ***/
                    differences.ComputerToExternal.AddNewFolderDifference(internalSubFolder);

                    internalFolderSubFolders.MoveNext();
                    internalSubFolder = internalFolderSubFolders.Current;
                }
                else if (externalSubFolder < internalSubFolder)
                {
                    /*
                     * 
                     * Add new folder of externalSubFolder in internalSubFolder
                     * 
                     * 
                     * ***/
                    differences.ExternalToComputer.AddNewFolderDifference(externalSubFolder);
                    externalFolderSubFolders.MoveNext();
                    externalSubFolder = externalFolderSubFolders.Current;
                }
                else
                {
                    DoInitializationCompare(internalSubFolder, externalSubFolder, differences);
                    internalFolderSubFolders.MoveNext();
                    externalFolderSubFolders.MoveNext();
                    internalSubFolder = internalFolderSubFolders.Current;
                    externalSubFolder = externalFolderSubFolders.Current;
                }
            }
            while (internalSubFolder != null)
            {
                /*
                 * 
                 * Add new folder of internalSubFolder in externalSubFolder
                 * 
                 * 
                 * ***/

                differences.ComputerToExternal.AddNewFolderDifference(internalSubFolder);
                internalFolderSubFolders.MoveNext();
                internalSubFolder = internalFolderSubFolders.Current;
            }
            while (externalSubFolder != null)
            {
                /*
                 * 
                 * Add new folder of externalSubFolder in internalSubFolder
                 * 
                 * 
                 * ***/
                differences.ExternalToComputer.AddNewFolderDifference(externalSubFolder);
                externalFolderSubFolders.MoveNext();
                externalSubFolder = externalFolderSubFolders.Current;
            }

        }

        private void DoInitializationFileCompare(FolderMeta internalFolder, FolderMeta externalFolder, InitializationDifferences differences)
        {
            IEnumerator<FileMeta> internalFolderSubFiles = internalFolder.GetFiles();
            IEnumerator<FileMeta> externalFolderSubFiles = externalFolder.GetFiles();


            internalFolderSubFiles.MoveNext();
            externalFolderSubFiles.MoveNext();

            FileMeta internalSubFile = internalFolderSubFiles.Current;
            FileMeta externalSubFile = externalFolderSubFiles.Current;

            while (internalSubFile != null && externalSubFile != null)
            {
                if (internalSubFile < externalSubFile)
                {
                    /*
                     * 
                     * Add new file info for internalSubFile in externalSubFile
                     * 
                     * */
                    differences.ComputerToExternal.AddNewFileDifference(internalSubFile);
                    internalFolderSubFiles.MoveNext();
                    internalSubFile = internalFolderSubFiles.Current;
                }
                else if (externalSubFile < internalSubFile)
                {
                    /*
                     * 
                     * Add new file info for externalSubFile in internalSubFile
                     * 
                     * */
                    differences.ExternalToComputer.AddNewFileDifference(externalSubFile);
                    externalFolderSubFiles.MoveNext();
                    externalSubFile = externalFolderSubFiles.Current;
                }
                else
                {
                    /*
                     * 
                     * CheckForTimeDifferences
                     * 
                     * */

                    internalFolderSubFiles.MoveNext();
                    externalFolderSubFiles.MoveNext();
                    internalSubFile = internalFolderSubFiles.Current;
                    externalSubFile = externalFolderSubFiles.Current;
                }
            }

            while (internalSubFile != null)
            {
                /*
                 * 
                 * Add new file info for internalSubFile in externalSubFile
                 * 
                 * */
                differences.ComputerToExternal.AddNewFileDifference(internalSubFile);
                internalFolderSubFiles.MoveNext();
                internalSubFile = internalFolderSubFiles.Current;
            }
            while (externalSubFile != null)
            {                    
                    /*
                     * 
                     * Add new file info for externalSubFile in internalSubFile
                     * 
                     * */
                differences.ExternalToComputer.AddNewFileDifference(externalSubFile);
                externalFolderSubFiles.MoveNext();
                externalSubFile = externalFolderSubFiles.Current;
            }
        }    
    }
}
