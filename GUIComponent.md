#GUI Component description

# GUI Component #
![http://cleansync.googlecode.com/files/GUI%20Components.jpg](http://cleansync.googlecode.com/files/GUI%20Components.jpg)

GUI is implemented using WPF. It consists of the main class GUI, which is the main window for CleanSync, and several GUI helper classes.
### GUI ###
The GUI class is the main window for the user interface.
### USBDetection ###
This class is in charge of detecting whenever a removable device has been plugged in or plugged out.
### Balloon / Balloon Decorator ###
The balloon class is used to create information balloons displayed on the system tray. The balloon decorator is used to define the balloon.
### IconExtractor ###
The IconExtractor is used to load icons from the system registry. It is mainly used to load the new, modified and deleted icons at the analysis results screen.
### DifferenceToTreeConvertor ###
DifferenceToTreeConvertor converts a difference into tree form for display purposes.

Methods
Public
  * FolderMeta ConvertDifferencesToTreeStructure(Differences difference)
Private
  * bool ConvertFolderListToTree(FolderMeta root, bool haveDifference, List`<`FolderMeta`>` folders)
    * Adds a list of FolderMeta into a given root. Parent folders are created if it does not exist yet.
  * bool ConvertFileListToTree(FolderMeta root, bool haveDifference, List`<`FileMeta`>` files)
    * Adds a list of FileMeta into a given root. Parent folders are created if it does not exist yet.  