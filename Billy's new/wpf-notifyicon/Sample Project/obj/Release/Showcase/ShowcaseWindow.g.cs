﻿#pragma checksum "..\..\..\Showcase\ShowcaseWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C48A328CF78BB4CE8B8DC40146D45108"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hardcodet.Wpf.TaskbarNotification;
using Samples;
using Samples.Commands;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace Samples {
    
    
    /// <summary>
    /// ShowcaseWindow
    /// </summary>
    public partial class ShowcaseWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 65 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal Hardcodet.Wpf.TaskbarNotification.TaskbarIcon tb;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid Balloons;
        
        #line default
        #line hidden
        
        
        #line 113 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.TextBox txtBalloonTitle;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.TextBox txtBalloonText;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.RadioButton rbInfo;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.RadioButton rbError;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.RadioButton rbCustomIcon;
        
        #line default
        #line hidden
        
        
        #line 170 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Button showBalloonTip;
        
        #line default
        #line hidden
        
        
        #line 188 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Button hideBalloonTip;
        
        #line default
        #line hidden
        
        
        #line 211 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid Popups;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.ListBox lstPopupTrigger;
        
        #line default
        #line hidden
        
        
        #line 260 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid ToolTips;
        
        #line default
        #line hidden
        
        
        #line 273 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.TextBox txtToolTipText;
        
        #line default
        #line hidden
        
        
        #line 320 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Button removeToolTip;
        
        #line default
        #line hidden
        
        
        #line 343 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid ContextMenus;
        
        #line default
        #line hidden
        
        
        #line 353 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.ListBox lstMenuTrigger;
        
        #line default
        #line hidden
        
        
        #line 418 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid CustomBalloons;
        
        #line default
        #line hidden
        
        
        #line 431 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Button showCustomBalloon;
        
        #line default
        #line hidden
        
        
        #line 444 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.TextBox customBalloonTitle;
        
        #line default
        #line hidden
        
        
        #line 464 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Button hideCustomBalloon;
        
        #line default
        #line hidden
        
        
        #line 486 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.Grid Common;
        
        #line default
        #line hidden
        
        
        #line 497 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.CheckBox iconVisibility;
        
        #line default
        #line hidden
        
        
        #line 505 "..\..\..\Showcase\ShowcaseWindow.xaml"
        internal System.Windows.Controls.ListBox iconList;
        
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
            System.Uri resourceLocater = new System.Uri("/Sample Project;component/showcase/showcasewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Showcase\ShowcaseWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 53 "..\..\..\Showcase\ShowcaseWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).AddHandler(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(this.OnNavigationRequest));
            
            #line default
            #line hidden
            return;
            case 2:
            this.tb = ((Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)(target));
            return;
            case 3:
            this.Balloons = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.txtBalloonTitle = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txtBalloonText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.rbInfo = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 7:
            this.rbError = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 8:
            this.rbCustomIcon = ((System.Windows.Controls.RadioButton)(target));
            return;
            case 9:
            this.showBalloonTip = ((System.Windows.Controls.Button)(target));
            
            #line 175 "..\..\..\Showcase\ShowcaseWindow.xaml"
            this.showBalloonTip.Click += new System.Windows.RoutedEventHandler(this.showBalloonTip_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.hideBalloonTip = ((System.Windows.Controls.Button)(target));
            
            #line 193 "..\..\..\Showcase\ShowcaseWindow.xaml"
            this.hideBalloonTip.Click += new System.Windows.RoutedEventHandler(this.hideBalloonTip_Click);
            
            #line default
            #line hidden
            return;
            case 11:
            this.Popups = ((System.Windows.Controls.Grid)(target));
            return;
            case 12:
            this.lstPopupTrigger = ((System.Windows.Controls.ListBox)(target));
            return;
            case 13:
            this.ToolTips = ((System.Windows.Controls.Grid)(target));
            return;
            case 14:
            this.txtToolTipText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 15:
            this.removeToolTip = ((System.Windows.Controls.Button)(target));
            
            #line 321 "..\..\..\Showcase\ShowcaseWindow.xaml"
            this.removeToolTip.Click += new System.Windows.RoutedEventHandler(this.removeToolTip_Click);
            
            #line default
            #line hidden
            return;
            case 16:
            this.ContextMenus = ((System.Windows.Controls.Grid)(target));
            return;
            case 17:
            this.lstMenuTrigger = ((System.Windows.Controls.ListBox)(target));
            return;
            case 18:
            this.CustomBalloons = ((System.Windows.Controls.Grid)(target));
            return;
            case 19:
            this.showCustomBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 432 "..\..\..\Showcase\ShowcaseWindow.xaml"
            this.showCustomBalloon.Click += new System.Windows.RoutedEventHandler(this.showCustomBalloon_Click);
            
            #line default
            #line hidden
            return;
            case 20:
            this.customBalloonTitle = ((System.Windows.Controls.TextBox)(target));
            return;
            case 21:
            this.hideCustomBalloon = ((System.Windows.Controls.Button)(target));
            
            #line 465 "..\..\..\Showcase\ShowcaseWindow.xaml"
            this.hideCustomBalloon.Click += new System.Windows.RoutedEventHandler(this.hideCustomBalloon_Click);
            
            #line default
            #line hidden
            return;
            case 22:
            this.Common = ((System.Windows.Controls.Grid)(target));
            return;
            case 23:
            this.iconVisibility = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 24:
            this.iconList = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
