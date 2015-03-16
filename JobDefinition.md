# JobDefinition Class Description #

JobDefinition is a serializable abstract class used to describe the meta data for jobs. [PCJob](http://code.google.com/p/cleansync/wiki/JobDefinition#PCJob_Class_Description) and [USBJob](http://code.google.com/p/cleansync/wiki/JobDefinition#USBJob_Class_Description)  inherit from JobDefinition

---

# Attributes #
## Public ##
  * **enum JobStatus { Complete, Incomplete, NotReady**
    * Complete indicates that both PCs synchronized in this Job have already been identified.
    * Incomplete indicates that
    * NotReady
  * **string JobName**
  * **JobStatus JobState**
  * **string RelativeUSBPath**

# Methods #
## Public ##
  * **void ToggleStatus(JobStatus state)**
    * Toggles the state of the job, based on a given state.

---

# PCJob Class Description #
[USBJob](http://code.google.com/p/cleansync/wiki/JobDefinition#USBJob_Class_Description) stores the metadata of the job information that is stored on the PC.

---

# Attributes #
## Public ##
  * **string PCPath**
  * **FolderMeta FolderInfo**
    * Stores the last known metadata of the root folder in the PC.
  * **string PCID**
## Private ##
  * **[USBJob](http://code.google.com/p/cleansync/wiki/JobDefinition#USBJob_Class_Description) usbJob**
# Methods #
## Public ##
  * **USBJob GetUsbJob()**
  * **void SetUsbJob(USBJob usb)**

---

# USBJob Class Description #
[USBJob](http://code.google.com/p/cleansync/wiki/JobDefinition#USBJob_Class_Description) stores the metadata of the job information that is stored on the USB.

---

# Attributes #
## Public ##
  * **string PCOnePath**
  * **string PCTwoPath**
  * **Differences diff**
  * **string PCOneID**
  * **string PCTwoID**
  * **string MostRecentPCID**
    * The ID of the most recent PC which did a synchronization.
  * **bool PCOneDeleted**
    * Checks if PCOne has deleted this job.
  * **bool PCTwoDeleted**
    * Checks if PCtwo has deleted this job.