/***********************************************************************
 * 
 * ****************CleanSync Version 2.0 JobDefinition*****************
 * 
 * Written By : Lv Wenhao & Li Yi & Li Zichen
 * Team 0110
 * 
 * 15/04/2010
 * 
 * ************************All Rights Reserved**************************
 * 
 * *********************************************************************/
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectoryInformation;
using System.ComponentModel;

namespace CleanSync
{
    public enum JobStatus { Complete, Incomplete, NotReady};
    [Serializable]
    public abstract class JobDefinition : INotifyPropertyChanged
    {
        [field: NonSerialized] 
        public event PropertyChangedEventHandler PropertyChanged;

        public string JobName
        {
            get;
            set;
        }

        [NonSerialized]
        private string absUsbPath;

        public string AbsoluteUSBPath
        {
            get { return absUsbPath; }
            set { absUsbPath = value; }
        }

        [NonSerialized]
        private bool isSynced;
        public System.Windows.Visibility Synchronized
        {
            get
            {
                if (isSynced && JobState.Equals(JobStatus.Complete))
                {
                    return System.Windows.Visibility.Visible;
                }
                else
                {
                    return System.Windows.Visibility.Hidden;
                }
            }
            set
            {
                if (value.Equals(System.Windows.Visibility.Visible))
                {
                    isSynced = true;
                }
                else
                {
                    isSynced = false;
                }
                OnPropertyChanged("Synchronized");
            }
        }

        [NonSerialized]
        private string imageSource;
        public string ImageSource
        {
            get
            {
                if (this.JobState.Equals(JobStatus.Complete))
                {
                    return @"Pic/green.png";
                }
                else if (this.JobState.Equals(JobStatus.Incomplete))
                {
                    return @"Pic/red.png";
                }
                else if(this.JobState.Equals(JobStatus.NotReady))
                {
                    return @"Pic/black.png";
                }
                else return @"Pic/black.png";
            }
            set
            {
                imageSource = value;
            }
        }
       

        public JobStatus JobState
        {
            get;
            set;
        }

        public string RelativeUSBPath
        {
            get;
            set;
        }


        public JobDefinition(string jobName,string pathOnUSB)
        {
            JobName = jobName;
            string root = Path.GetPathRoot(pathOnUSB);
            RelativeUSBPath = pathOnUSB.Substring(root.Length);
            AbsoluteUSBPath = pathOnUSB;
            JobState = JobStatus.Incomplete;
            isSynced = false;
        }

        public void ToggleStatus(JobStatus state)
        {
            if (state.Equals(JobStatus.Complete))
            {
                JobState = JobStatus.NotReady;
                this.isSynced = false;
            }
            else if (state.Equals(JobStatus.NotReady))
                JobState = JobStatus.Complete;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
