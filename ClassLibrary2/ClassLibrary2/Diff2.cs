using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    class Diff2
    {
        FileMeta file;
        DifferenceType type;
        public enum DifferenceType{
            New,
            Deleted,
            Modified
        }
        public Diff2(FileMeta fileMeta, DifferenceType type)
        {
            this.file = fileMeta;
            this.type = type;
        }

        List<FileMeta> FilesToDelete;
        List<FileMeta> FilesModified;

    }
}
