﻿#pragma checksum "..\..\..\Showcase\FancyBalloon.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7ABE28B0705C484FCBE16807D47E0185"
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
    /// FancyBalloon
    /// </summary>
    public partial class FancyBalloon : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\Showcase\FancyBalloon.xaml"
        internal Samples.FancyBalloon me;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Media.Animation.BeginStoryboard FadeIn_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Media.Animation.BeginStoryboard HighlightCloseButton_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Media.Animation.BeginStoryboard FadeCloseButton_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Media.Animation.BeginStoryboard FadeBack_BeginStoryboard1;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Media.Animation.BeginStoryboard FadeOut_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Controls.Grid grid;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Controls.TextBlock Something;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\Showcase\FancyBalloon.xaml"
        internal System.Windows.Controls.Image imgClose;
        
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
            System.Uri resourceLocater = new System.Uri("/Sample Project;component/showcase/fancyballoon.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Showcase\FancyBalloon.xaml"
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
            this.me = ((Samples.FancyBalloon)(target));
            return;
            case 2:
            
            #line 36 "..\..\..\Showcase\FancyBalloon.xaml"
            ((System.Windows.Media.Animation.Storyboard)(target)).Completed += new System.EventHandler(this.OnFadeOutCompleted);
            
            #line default
            #line hidden
            return;
            case 3:
            this.FadeIn_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 4:
            this.HighlightCloseButton_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 5:
            this.FadeCloseButton_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 6:
            this.FadeBack_BeginStoryboard1 = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 7:
            this.FadeOut_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 8:
            this.grid = ((System.Windows.Controls.Grid)(target));
            
            #line 61 "..\..\..\Showcase\FancyBalloon.xaml"
            this.grid.MouseEnter += new System.Windows.Input.MouseEventHandler(this.grid_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Something = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.imgClose = ((System.Windows.Controls.Image)(target));
            
            #line 115 "..\..\..\Showcase\FancyBalloon.xaml"
            this.imgClose.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.imgClose_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
