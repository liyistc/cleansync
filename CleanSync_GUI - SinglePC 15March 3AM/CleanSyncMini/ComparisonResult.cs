using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectoryInformation
{
    public class ComparisonResult
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

        public ComparisonResult(Differences USBDifferences, Differences PCDifferences, List<Conflicts> conflictList)
        {
            this.USBDifferences = USBDifferences;
            this.PCDifferences = PCDifferences;
            this.conflictList = conflictList;
        }


        public List<string> ConvertComparisonResultToListOfString(ComparisonResult comparisonResult)
        {
            List<string> differencesAndConflicts = new List<string>();
            Differences USBDifferences = comparisonResult.USBDifferences;
            Differences PCDifferences = comparisonResult.PCDifferences;
            List<Conflicts> conflictList = comparisonResult.conflictList;
            differencesAndConflicts.Add(PCDifferences.ToString());
            differencesAndConflicts.Add(USBDifferences.ToString());
            differencesAndConflicts.Add(this.ConflictlistToString(conflictList));
            return differencesAndConflicts;
        }

        private string ConflictlistToString(List<Conflicts> conflictList)
        {
            string result="";
            foreach (Conflicts conflict in conflictList)
                result += conflict.ToString();
            return result;
        }
    }
}
