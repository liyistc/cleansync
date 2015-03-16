# Introduction #
JobLogic handles all requests for job-related work. It manages the jobs in the system and delegates the work further to CompareLogic and SyncLogic.

  * void InitializePCJobInfo()
    * Loads all pcJob found on the PC
  * void InitializeUSBJobInfo(string usbRoot,string pcID)
    * Searches and Loads all usbJob found on the USB drives and mount them to the respective pcJobs.