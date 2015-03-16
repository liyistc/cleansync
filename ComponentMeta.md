# ComponentMeta #

> ## This is the basic file and folder metadata definition extends form the basic metadata definition "ComponentMeta". It provides the basic information about each files, also, give the structure of the folders using tree structure. ##


---

## Attributes ##
### Public ###
  * **enum Type`{` New,Modified,Deleted,NotTouched `}`**
    * Defines the type (or status) of one file(folder).
      * New: first created
      * Modified: being modified by other side
      * Deleted: Deleted by other side
      * NotTouched: No change on both sides
  * **string Name**
    * Name of the file/folder metadata
  * **string Path**
    * Relative path based on the root folder selected. Eg:"D:\temp\temp1\a.txt", if we selected
> > > "D:\temp" as the root, then path will be "temp1\a.txt".
  * **public string AbsolutePath ====
    * Stores the absolutePath of the file/folder metadata
  ***public string rootDir ====
    * Give the root folder selected by user

---

## Constructor ##
  * **ComponentMeta(string path, string rootDir)**

---

## Methods ##
### Public ###
  * **static bool operator `>`(ComponentMeta first, ComponentMeta second)**
    * Compare if the first component metadata is larger than second.
  * **static bool operator `<`(ComponentMeta first, ComponentMeta second)**
    * Compare if the first component metadata is smaller than second.

---

# FileMeta #

> > An instance extends from ComponentMeta which specifies the file informations.

---

## Attributes ##
### Public ###
  * **Type FileType**
    * Type of the file: using the Type in the ComponentMeta enum.
  * **public DateTime LastModifiedTime**
    * Get the last modified time for the file.
  * **public DateTime CreationTime**
    * Get the creation time of the file.
  * **public long Size**
    * Get the size of the file.

---

## Constructor ##
  * **FileMeta(string path, string rootDir) : base(path, rootDir)**
    * Inheritated from the ComponentMata.
  * **FileMeta(FileMeta file) : base(file.AbsolutePath,file.rootDir)**
    * Using the given FileMeta to construct new FileMeta object.

---

## Methods ##
### Public ###
  * **static int ConvertToKiloByte(FileInfo fileInfo)**
    * Return file size in K bytes.
  * **string getString()**
    * Provide a string representation of the file metadata.

---

# FolderMeta #

> An instance extends from ComponentMeta which specifies the folder informations.

---

## Attributes ##
### Public ###
  * **Type FolderType**
    * Type of the folder: using the Type in the ComponentMeta enum.
  * **List`<`FolderMeta`>` folders**
    * List of folders inside one folder.
  * **List`<`FileMeta`>` files**
    * List of files inside one folder.
  * **long Size**
    * Size of the folder.

---

## Constructor ##
  * **FolderMeta(string path, string rootDir)**
    * Inheritated from the ComponentMata.
  * **FolderMeta(FolderMeta root):base(root.AbsolutePath,root.rootDir)**
    * Given one folder metadata to generate its own metadata.

---

## Methods ##
### Public ###
  * **void AddFile(FileMeta file)**
    * Add one file to the file list.
  * **void AddFolder(FolderMeta folder)**
    * Add one folder to the folder list.
  * **IEnumerator`<`FolderMeta`>` GetFolders()**
    * Get all the folders in the folder metadata.
  * **IEnumerator`<`FileMeta`>` GetFiles()**
    * Get all the files in the folder metadata.
  * **String getString()**
    * Return a string representation of the folder metadata.