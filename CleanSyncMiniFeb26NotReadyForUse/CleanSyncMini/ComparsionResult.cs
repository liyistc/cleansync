﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    public class ComparsionResult
    {

        public Differences USBDifferences
        {
            get;
            set;
        }
        public Differences PCDifferences
        {
            get;
            set;
        }
        public List<Conflicts> conflictList
        {
            get;
            set;
        }

        public ComparsionResult(Differences USBDifferences, Differences PCDifferences, List<Conflicts> conflictList)
        {
            this.USBDifferences = USBDifferences;
            this.PCDifferences = PCDifferences;
            this.conflictList = conflictList;
        }
    }
}