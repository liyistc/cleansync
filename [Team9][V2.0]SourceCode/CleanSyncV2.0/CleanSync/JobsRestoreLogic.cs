/***********************************************************************
 * 
 * ****************CleanSync Version 2.0 JobRestoreLogic****************
 * 
 * Written By : Tso Shuk Yi
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanSync
{
    static class JobsRestoreLogic
    {
        /// <summary>
        /// Restore changes made to a PCJob whose previous synchronization process was interrupted. It will attempt to restore the folder synchronized to the previous state. However, folders that were newly copied over to the drive will remain there. No file or folder will be lost.
        /// </summary>
        /// <param name="pcJob">The PCJob to be restored</param>
        public static void RestoreInterruptedPCJobPCChanges(PCJob pcJob)
        {
            ReadAndWrite.MoveFolderContentWithReplace(ReadAndWrite.GetPCBackUpFolder(pcJob), pcJob.PCPath);
            pcJob.Synchronizing = false;
            ReadAndWrite.ExportPCJob(pcJob);
        }

        /// <summary>
        /// Restore changes made to a removable drive whose previous normal synchronization process was interrupted. It will restore the USBJob and the USB folders to the previous state.
        /// </summary>
        /// <param name="pcJob">The PCJob whose attached USBJob needs to be restored</param>
        public static void RestoreInterruptedUSB(PCJob pcJob)
        {
            USBJob usbJob = pcJob.GetUsbJob();
            string tempUSBForPCContent = ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob());
            string tempUSBForUSBContent = ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob());

            if (usbJob.SynchronizingPcToUSB)
            {
                ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
                usbJob.SynchronizingPcToUSB = false;
            }
            else if (usbJob.SynchronizingUSBToPC)
            {
                usbJob.SynchronizingUSBToPC = false;
            }
            else if (usbJob.MovingOldDiffToTemp) //sync PCtoUSB done, USBtoPC done, have not move folder over
            {
                
                ReadAndWrite.MoveFolderContentWithReplace(tempUSBForUSBContent, usbJob.AbsoluteUSBPath);
                usbJob.MovingOldDiffToTemp = false;
            }
            else // usbJob.MovingTemptoOldDiff
            {
                ReadAndWrite.MoveFolderContentWithReplace(usbJob.AbsoluteUSBPath, tempUSBForPCContent);
                ReadAndWrite.MoveFolderContents(tempUSBForUSBContent, usbJob.AbsoluteUSBPath);
                usbJob.MovingTempToOldDiff = false;
            }
            usbJob.Synchronizing = false;
            ReadAndWrite.ExportUSBJob(usbJob);
                        
        }

        /// <summary>
        /// Restore changes made to a removable drive whose previous re-synchroniztion process was interrupted. It will restore the USBJob and the USB folders to the previous state.
        /// </summary>
        /// <param name="pcJob"></param>
        public static void RestoreReSyncUSB(PCJob pcJob)
        {
            string tempUSBForPCContent = ReadAndWrite.GetUSBTempFolder(pcJob.GetUsbJob());
            string tempUSBForUSBContent = ReadAndWrite.GetUSBResyncDirectory(pcJob.GetUsbJob());
            string USBTempReSync = ReadAndWrite.GetUSBResyncBackUpDirectory(pcJob.GetUsbJob()); ;

            USBJob usbJob = pcJob.GetUsbJob();

            ReadAndWrite.DeleteFolderContent(tempUSBForPCContent);
            ReadAndWrite.DeleteFolderContent(tempUSBForUSBContent);
            ReadAndWrite.DeleteFolderContent(pcJob.AbsoluteUSBPath);
            ReadAndWrite.MoveFolderContentWithReplace(USBTempReSync, pcJob.AbsoluteUSBPath);

            usbJob.ReSynchronizing = false;
            if (!usbJob.JobState.Equals(JobStatus.Incomplete))
                ReadAndWrite.ExportUSBJob(usbJob);
            else
                ReadAndWrite.ExportIncompleteJobToUSB(usbJob);
        }
    }
}
