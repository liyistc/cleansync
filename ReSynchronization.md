# Re-Synchronization #
Re-Synchronization is invoked when synchronizing the computer to the USB successively. Instead of a normal synchronization where updates on both sides are updated, re-synchronization only checks what updates from the computer are to be copied over to the USB. SyncLogic defines the differences on the USB as the old differences from this computer, and the differences on the USB as the new differences. SyncLogic will compare each file and folder between the old and new differences and make changes to the old differences accordingly:

Comparing contents between 2 folders which are modified:

If a folder exists in both differences:
  1. The old folder is deleted, the new folder is new
    * This means the folder is previously deleted, and now a new folder with the same name is created. The folder‟s difference will then be set to modified, and the sub folders and files will be compared recursively.
  1. The old folder is new, the new folder is deleted
    * The folder new folder will be removed, and no changes need to be made to the other computer, so the difference will be removed.
  1. The old folder is new, the new folder is modified
    * In this case, a special method will be called to check accordingly the subfiles and subfolders. If a subfile or subfolder is now deleted, it will be deleted from the differences. Else, any changes will be updated
  1. The old folder is modified, the new folder is deleted
    * A special method will also be called, to check if the there are any new files that will also be deleted from the differences. Also, previously deleted files and folders will also be added back to the differences.
  1. Both folders are modified
    * The folders will be checked recursively again.
If a file exists in both differences:
  1. The old file is deleted, the new file is new
    * A previously deleted file has been created again, thus, the difference will be set to modified.
  1. The old file is new, the new file is deleted
    * The file will be removed from the differences.
  1. The old file is new, the new file is modified
    * The new file will be set as new again.
  1. The old file is modified, the new file is modified
    * The file will still be modified
  1. The old file is modified, the new file is deleted
    * The difference will be set to deleted

To allow for restoration of previous state and backing up of deleted files and folders, normal synchronization works as follows:
  1. Copy all the changes from the USB to a temporary folder
  1. Extract the changes from the PC to a temporary folder on the USB
  1. Do the resynchronization to the main USB folder.
  1. Delete the temporary USB folder.
If the synchronization is interrupted or cancelled, the temporary folders will be flushed except for the data on the temporary folder containing the original differences which will be set back as the original differences.

1. Copy all the changes from the USB to a temporary folder