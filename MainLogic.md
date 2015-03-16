# Introduction #
MainLogic handles request from the GUI and distributes the work to the various classes in the logic component.

---

  * List`<`USBJob`>` USBPlugIn(List`<`string`>` drives)
    * This method searches all USB drives connected to the computer for incomplete jobs. If there is an incomplete job, check if this computer is the computer that created the job. If it is not, it will return it as an incomplete USB job available to be accepted by this computer.
  * string GetPCID()
    * Uses the MAC address of this computer as the PCID.