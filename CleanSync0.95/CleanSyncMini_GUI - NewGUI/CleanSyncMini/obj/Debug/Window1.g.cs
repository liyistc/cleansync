﻿#pragma checksum "..\..\Window1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6E7AFC4E6FFC4D7BCA0210D9BB05627"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hardcodet.Wpf.TaskbarNotification;
using Samples.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CleanSyncMini {
    
    
    /// <summary>
    /// GUI
    /// </summary>
    public partial class GUI : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 81 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid Frame;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid IconFrame;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\Window1.xaml"
        internal Hardcodet.Wpf.TaskbarNotification.TaskbarIcon CleanSyncNotifyIcon;
        
        #line default
        #line hidden
        
        
        #line 94 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid JobListHost;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\Window1.xaml"
        internal System.Windows.Controls.GroupBox JobListHostBox;
        
        #line default
        #line hidden
        
        
        #line 97 "..\..\Window1.xaml"
        internal System.Windows.Controls.ListBox JobList;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid JobStateIndicator;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\Window1.xaml"
        internal System.Windows.Controls.StackPanel JobState1;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\Window1.xaml"
        internal System.Windows.Controls.StackPanel JobState2;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\Window1.xaml"
        internal System.Windows.Controls.StackPanel JobState3;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid InforHost;
        
        #line default
        #line hidden
        
        
        #line 127 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid MainFrameInfor;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label JobName;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label LocalPath;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label RemotePath;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AbsoluteUSBPath;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label JobNameDisplay;
        
        #line default
        #line hidden
        
        
        #line 135 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label LocalPathDisplay;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label RemotePathDisplay;
        
        #line default
        #line hidden
        
        
        #line 137 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label USBPathDisplay;
        
        #line default
        #line hidden
        
        
        #line 142 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox Automation;
        
        #line default
        #line hidden
        
        
        #line 145 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox Filter;
        
        #line default
        #line hidden
        
        
        #line 148 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox RunMode;
        
        #line default
        #line hidden
        
        
        #line 156 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid NewJobFrameInfor;
        
        #line default
        #line hidden
        
        
        #line 159 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox NewJobName;
        
        #line default
        #line hidden
        
        
        #line 164 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox AttachDropBox1;
        
        #line default
        #line hidden
        
        
        #line 165 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox AttachDropBox2;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button FolderSelection1;
        
        #line default
        #line hidden
        
        
        #line 179 "..\..\Window1.xaml"
        internal System.Windows.Controls.ListBox FolderSelection2;
        
        #line default
        #line hidden
        
        
        #line 188 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox NewJobAutomation;
        
        #line default
        #line hidden
        
        
        #line 191 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox NewJobFilter;
        
        #line default
        #line hidden
        
        
        #line 194 "..\..\Window1.xaml"
        internal System.Windows.Controls.ComboBox NewJobRunMode;
        
        #line default
        #line hidden
        
        
        #line 200 "..\..\Window1.xaml"
        internal System.Windows.Controls.ProgressBar FirstSyncProgressBar;
        
        #line default
        #line hidden
        
        
        #line 201 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label BarLabel;
        
        #line default
        #line hidden
        
        
        #line 204 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid AcceptFrameInfor;
        
        #line default
        #line hidden
        
        
        #line 207 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptJobName;
        
        #line default
        #line hidden
        
        
        #line 208 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptLocalPath;
        
        #line default
        #line hidden
        
        
        #line 209 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptRemotePath;
        
        #line default
        #line hidden
        
        
        #line 210 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptUSBPath;
        
        #line default
        #line hidden
        
        
        #line 211 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptJobNameDisplay;
        
        #line default
        #line hidden
        
        
        #line 212 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptLocalPathDisplay;
        
        #line default
        #line hidden
        
        
        #line 213 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptRemotePathDisplay;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label AcceptUSBPathDisplay;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\Window1.xaml"
        internal System.Windows.Controls.TextBox AttachDropBox;
        
        #line default
        #line hidden
        
        
        #line 220 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button FolderSelection;
        
        #line default
        #line hidden
        
        
        #line 240 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid ButtonsHost;
        
        #line default
        #line hidden
        
        
        #line 241 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid NewJobFrameGridButtons;
        
        #line default
        #line hidden
        
        
        #line 244 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button FirstSync;
        
        #line default
        #line hidden
        
        
        #line 254 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button CancelCreat;
        
        #line default
        #line hidden
        
        
        #line 269 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid MainFrameGridButtons;
        
        #line default
        #line hidden
        
        
        #line 272 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button NewJob;
        
        #line default
        #line hidden
        
        
        #line 282 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button RemoveJob;
        
        #line default
        #line hidden
        
        
        #line 292 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button Analyse;
        
        #line default
        #line hidden
        
        
        #line 302 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button StartSync;
        
        #line default
        #line hidden
        
        
        #line 316 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid AcceptFrameGridButtons;
        
        #line default
        #line hidden
        
        
        #line 319 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button Accept;
        
        #line default
        #line hidden
        
        
        #line 328 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid Conflict;
        
        #line default
        #line hidden
        
        
        #line 331 "..\..\Window1.xaml"
        internal System.Windows.Controls.ListView ConflictList;
        
        #line default
        #line hidden
        
        
        #line 353 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid ConflictButtons;
        
        #line default
        #line hidden
        
        
        #line 356 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button Confirm;
        
        #line default
        #line hidden
        
        
        #line 357 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button ConflictCanel;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid Analyser;
        
        #line default
        #line hidden
        
        
        #line 364 "..\..\Window1.xaml"
        internal System.Windows.Controls.TreeView CompareResultLeft;
        
        #line default
        #line hidden
        
        
        #line 383 "..\..\Window1.xaml"
        internal System.Windows.Controls.TreeView CompareResultRight;
        
        #line default
        #line hidden
        
        
        #line 403 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label PCFreeSpace;
        
        #line default
        #line hidden
        
        
        #line 404 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label PCRequiredSpace;
        
        #line default
        #line hidden
        
        
        #line 411 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label RDFreeSpace;
        
        #line default
        #line hidden
        
        
        #line 412 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label RDRequiredSpace;
        
        #line default
        #line hidden
        
        
        #line 418 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid AnalyseProgressBar;
        
        #line default
        #line hidden
        
        
        #line 419 "..\..\Window1.xaml"
        internal System.Windows.Controls.ProgressBar SyncProgressBar;
        
        #line default
        #line hidden
        
        
        #line 420 "..\..\Window1.xaml"
        internal System.Windows.Controls.Label SyncBarLabel;
        
        #line default
        #line hidden
        
        
        #line 423 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid AnalyseButtons;
        
        #line default
        #line hidden
        
        
        #line 424 "..\..\Window1.xaml"
        internal System.Windows.Controls.Grid AnalyseFrameGridButtons;
        
        #line default
        #line hidden
        
        
        #line 427 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button AnalyseStartSync;
        
        #line default
        #line hidden
        
        
        #line 429 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button AnalyseCancel;
        
        #line default
        #line hidden
        
        
        #line 436 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button Close;
        
        #line default
        #line hidden
        
        
        #line 437 "..\..\Window1.xaml"
        internal System.Windows.Controls.Button minmize;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CleanSnycMini;component/window1.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Window1.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 6 "..\..\Window1.xaml"
            ((CleanSyncMini.GUI)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Frame = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.IconFrame = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.CleanSyncNotifyIcon = ((Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)(target));
            
            #line 84 "..\..\Window1.xaml"
            this.CleanSyncNotifyIcon.TrayMouseDoubleClick += new System.Windows.RoutedEventHandler(this.CleanSyncNotifyIcon_TrayMouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 5:
            this.JobListHost = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.JobListHostBox = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 7:
            this.JobList = ((System.Windows.Controls.ListBox)(target));
            
            #line 97 "..\..\Window1.xaml"
            this.JobList.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.JobList_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.JobStateIndicator = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.JobState1 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 10:
            this.JobState2 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 11:
            this.JobState3 = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 12:
            this.InforHost = ((System.Windows.Controls.Grid)(target));
            return;
            case 13:
            this.MainFrameInfor = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.JobName = ((System.Windows.Controls.Label)(target));
            return;
            case 15:
            this.LocalPath = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.RemotePath = ((System.Windows.Controls.Label)(target));
            return;
            case 17:
            this.AbsoluteUSBPath = ((System.Windows.Controls.Label)(target));
            return;
            case 18:
            this.JobNameDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 19:
            this.LocalPathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 20:
            this.RemotePathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 21:
            this.USBPathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 22:
            this.Automation = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 23:
            this.Filter = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 24:
            this.RunMode = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 25:
            this.NewJobFrameInfor = ((System.Windows.Controls.Grid)(target));
            return;
            case 26:
            this.NewJobName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 27:
            this.AttachDropBox1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 28:
            this.AttachDropBox2 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 29:
            this.FolderSelection1 = ((System.Windows.Controls.Button)(target));
            
            #line 166 "..\..\Window1.xaml"
            this.FolderSelection1.Click += new System.Windows.RoutedEventHandler(this.FolderSelection_Click1);
            
            #line default
            #line hidden
            return;
            case 30:
            this.FolderSelection2 = ((System.Windows.Controls.ListBox)(target));
            return;
            case 31:
            this.NewJobAutomation = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 32:
            this.NewJobFilter = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 33:
            this.NewJobRunMode = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 34:
            this.FirstSyncProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 35:
            this.BarLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 36:
            this.AcceptFrameInfor = ((System.Windows.Controls.Grid)(target));
            return;
            case 37:
            this.AcceptJobName = ((System.Windows.Controls.Label)(target));
            return;
            case 38:
            this.AcceptLocalPath = ((System.Windows.Controls.Label)(target));
            return;
            case 39:
            this.AcceptRemotePath = ((System.Windows.Controls.Label)(target));
            return;
            case 40:
            this.AcceptUSBPath = ((System.Windows.Controls.Label)(target));
            return;
            case 41:
            this.AcceptJobNameDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 42:
            this.AcceptLocalPathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 43:
            this.AcceptRemotePathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 44:
            this.AcceptUSBPathDisplay = ((System.Windows.Controls.Label)(target));
            return;
            case 45:
            this.AttachDropBox = ((System.Windows.Controls.TextBox)(target));
            return;
            case 46:
            this.FolderSelection = ((System.Windows.Controls.Button)(target));
            
            #line 220 "..\..\Window1.xaml"
            this.FolderSelection.Click += new System.Windows.RoutedEventHandler(this.FolderSelection_Click);
            
            #line default
            #line hidden
            return;
            case 47:
            this.ButtonsHost = ((System.Windows.Controls.Grid)(target));
            return;
            case 48:
            this.NewJobFrameGridButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 49:
            this.FirstSync = ((System.Windows.Controls.Button)(target));
            
            #line 244 "..\..\Window1.xaml"
            this.FirstSync.Click += new System.Windows.RoutedEventHandler(this.FirstSync_Click);
            
            #line default
            #line hidden
            return;
            case 50:
            this.CancelCreat = ((System.Windows.Controls.Button)(target));
            
            #line 254 "..\..\Window1.xaml"
            this.CancelCreat.Click += new System.Windows.RoutedEventHandler(this.CancelCreat_Click);
            
            #line default
            #line hidden
            return;
            case 51:
            this.MainFrameGridButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 52:
            this.NewJob = ((System.Windows.Controls.Button)(target));
            
            #line 272 "..\..\Window1.xaml"
            this.NewJob.Click += new System.Windows.RoutedEventHandler(this.NewJob_Click);
            
            #line default
            #line hidden
            return;
            case 53:
            this.RemoveJob = ((System.Windows.Controls.Button)(target));
            
            #line 282 "..\..\Window1.xaml"
            this.RemoveJob.Click += new System.Windows.RoutedEventHandler(this.RemoveJob_Click);
            
            #line default
            #line hidden
            return;
            case 54:
            this.Analyse = ((System.Windows.Controls.Button)(target));
            
            #line 292 "..\..\Window1.xaml"
            this.Analyse.Click += new System.Windows.RoutedEventHandler(this.Analyse_Click);
            
            #line default
            #line hidden
            return;
            case 55:
            this.StartSync = ((System.Windows.Controls.Button)(target));
            return;
            case 56:
            this.AcceptFrameGridButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 57:
            this.Accept = ((System.Windows.Controls.Button)(target));
            
            #line 319 "..\..\Window1.xaml"
            this.Accept.Click += new System.Windows.RoutedEventHandler(this.Accept_Click);
            
            #line default
            #line hidden
            return;
            case 58:
            this.Conflict = ((System.Windows.Controls.Grid)(target));
            return;
            case 59:
            this.ConflictList = ((System.Windows.Controls.ListView)(target));
            return;
            case 60:
            this.ConflictButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 61:
            this.Confirm = ((System.Windows.Controls.Button)(target));
            
            #line 356 "..\..\Window1.xaml"
            this.Confirm.Click += new System.Windows.RoutedEventHandler(this.ConflictConfirm_Click);
            
            #line default
            #line hidden
            return;
            case 62:
            this.ConflictCanel = ((System.Windows.Controls.Button)(target));
            
            #line 357 "..\..\Window1.xaml"
            this.ConflictCanel.Click += new System.Windows.RoutedEventHandler(this.ConflictCancel_Click);
            
            #line default
            #line hidden
            return;
            case 63:
            this.Analyser = ((System.Windows.Controls.Grid)(target));
            return;
            case 64:
            this.CompareResultLeft = ((System.Windows.Controls.TreeView)(target));
            return;
            case 65:
            this.CompareResultRight = ((System.Windows.Controls.TreeView)(target));
            return;
            case 66:
            this.PCFreeSpace = ((System.Windows.Controls.Label)(target));
            return;
            case 67:
            this.PCRequiredSpace = ((System.Windows.Controls.Label)(target));
            return;
            case 68:
            this.RDFreeSpace = ((System.Windows.Controls.Label)(target));
            return;
            case 69:
            this.RDRequiredSpace = ((System.Windows.Controls.Label)(target));
            return;
            case 70:
            this.AnalyseProgressBar = ((System.Windows.Controls.Grid)(target));
            return;
            case 71:
            this.SyncProgressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 72:
            this.SyncBarLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 73:
            this.AnalyseButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 74:
            this.AnalyseFrameGridButtons = ((System.Windows.Controls.Grid)(target));
            return;
            case 75:
            this.AnalyseStartSync = ((System.Windows.Controls.Button)(target));
            
            #line 427 "..\..\Window1.xaml"
            this.AnalyseStartSync.Click += new System.Windows.RoutedEventHandler(this.AnalyseStartSync_Click);
            
            #line default
            #line hidden
            return;
            case 76:
            this.AnalyseCancel = ((System.Windows.Controls.Button)(target));
            
            #line 429 "..\..\Window1.xaml"
            this.AnalyseCancel.Click += new System.Windows.RoutedEventHandler(this.Cancel_Click);
            
            #line default
            #line hidden
            return;
            case 77:
            this.Close = ((System.Windows.Controls.Button)(target));
            
            #line 436 "..\..\Window1.xaml"
            this.Close.Click += new System.Windows.RoutedEventHandler(this.Close_Click);
            
            #line default
            #line hidden
            return;
            case 78:
            this.minmize = ((System.Windows.Controls.Button)(target));
            
            #line 437 "..\..\Window1.xaml"
            this.minmize.Click += new System.Windows.RoutedEventHandler(this.minmize_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}