#CleanSync developers' guide

## Introduction ##
CleanSync is a one stop user-friendly and file synchronization software that facilitates the daily back-up and synchronization needs of Users who have to bring home the files from the office to continue their work at home and thus have to ensure that the two sets of files at both locations are synchronized. Other than conventional synchronization between 2 folders, CleanSync utilizes our new technology, Clean Synchronization, which allows users to sync two computers using a third USB device, while keeping disk space usage on the external device, to a minimum. With Clean Synchronization, users can synchronize between workstations using an external device without keeping track of two separate synchronization jobs of the external drive to each of the computers separately.

---

## Functionality Features ##
See: [Functionality Features](http://code.google.com/p/cleansync/wiki/FunctionalityFeatures)
  * Clean Synchronization
  * Preview feature
  * Conflict Handling
  * Folder/File renaming
  * External drive Plug-in Plug-out detection
  * Automation

---

## Glossary of Terms ##
See: [Glossary of Terms](http://code.google.com/p/cleansync/wiki/GlossaryTerms)

---

## Development Platform ##

CleanSync is developed using .NET Visual C#.

---

## Components ##
![http://cleansync.googlecode.com/files/CLEANSync%20components.jpg](http://cleansync.googlecode.com/files/CLEANSync%20components.jpg)
### GUI ###
See: [GUI Component](http://code.google.com/p/cleansync/wiki/GUIComponent)

GUI is implemented using WPF. It consists of the main class GUI, which is the main window for CleanSync, and several GUI helper classes.

### Logics ###
See: [Logics Component](http://code.google.com/p/cleansync/wiki/LogicComponents)

The logics component form the underlying engine of CleanSync. This component is in charge of handling all calculation and work.


### MetaData ###
See: [MetaData Component](http://code.google.com/p/cleansync/wiki/MetaDataComponent)

MetaData classes store various information used by CleanSync to perform its operations.

---

## Implementation Details ##
[Conflict Handling](http://code.google.com/p/cleansync/wiki/ConflictHandling?ts=1271356053&updated=ConflictHandling)

[Clean Synchronization](http://code.google.com/p/cleansync/wiki/CleanSynchronization?ts=1271353935&updated=CleanSynchronization)

[Determining folders to store meta data](http://code.google.com/p/cleansync/wiki/DeterminingFolder)

[Loading and unloading meta data to and from the hard disk.](http://code.google.com/p/cleansync/wiki/LoadUnload)

---

## Known Issues ##
See [KnownIssues](http://code.google.com/p/cleansync/wiki/KnownIssues)