   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  I ESample Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:SamplesSamples 3clr-namespace:Samples.CommandsSamples.Commands    Samples.ShowcaseWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    \ XHardcodet.Wpf.TaskbarNotification, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null)tb http://www.hardcodet.net/taskbar  0sys&clr-namespace:System;assembly=mscorlib  :d2http://schemas.microsoft.com/expression/blend/2008  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  !localclr-namespace:Samples  -Commandsclr-namespace:Samples.Commands    � Title$  WPF NotifyIcon��$	��874��$	��880��$	��750��$	��800�� � 
ResizeMode$ NoResize=� � 	Resources     BooleanToVisibilityConverter S� 
MethodName S� 
ObjectType   System.Enum  ActivationModes S� MethodParameters M� TypeName��%Y   &	        &	      �� S� $ 	GetValues��"   M� $ tb:PopupActivationMode������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.��� 	 � Color$	 	#FFFFFFFF��
 � Offset$
 0Q�� $	 	#FFFFE7D1��$
 1Q� � Triggers :�  :� RoutedEvent$$ tb:TaskbarIcon.TrayToolTipOpen�� :� 
SourceName$ tb��.��� -   .+�3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  -     Name$ tb��$	��Top=� 
 tbMenu � ContextMenu# �     
IconSource ��  �� Path$ SelectedItem.Source� �� ElementName$ iconList�� �� Mode$ Default=�   ToolTipText �� $
 Text�$ txtToolTipText��$ Default=�y��� $ 	IsChecked� �� 	Converter# [  $ iconVisibility��$ Default=�   MenuActivation �� $ SelectedItem�$ lstMenuTrigger��$ Default=�   PopupActivation �� $ SelectedItem�$ lstPopupTrigger��$ Default=�   DoubleClickCommand .  (Samples.Commands.ShowSampleWindowCommand  "   DoubleClickCommandParameter ��  �� RelativeSource �� *Self+   	TrayPopup   Samples.FancyPopup     TrayToolTip   Samples.FancyToolTip   J� Opacity$
 0.85Q�   InfoText �� $ ToolTipText�� -    �Name$ Balloons��$��486.5,578.58,0,0q�$	��Top=�$	��248��$	��372��$
��Left=�.+��� $��Stretch=�$��Stretch=����	#FF000000$��2,2,2,2q��� -     ��Name$  txtBalloonTitle��$��125,0,17,133q�$��Bottom=�$��23��.��WPF NotifyIcon�� -   $  txtBalloonText��$��125,0,17,76q�! �� AcceptsReturn! . $��47��$��Bottom=�" �� TextWrapping$
" Wrap=�$M��GYou should see a LED icon in your system tray. This is your NotifyIcon.��	� -   # 	�Name$# rbInfo��$
��Left=�$��	14,0,0,54q�$��Bottom=�$	��111��$��22��$��Show as Info��$ d� 	IsChecked$
$ TrueY��� $
��Left=�$��
14,0,0,133q�$��Bottom=�$��85��$��23��$
��Wrap=��� $�Balloon Title���� $��
14,0,0,100q�$
��Wrap=�$
��Left=�$��85��$��23��$��Bottom=��� $�Balloon Text��	� -   $# rbError��$��	14,0,0,32q�$��Bottom=�$��22��$��Show as Error��$
��Left=�$	��111��	� -   $# rbCustomIcon��$��	14,0,0,10q�$��Bottom=�$��22��$��Show Custom Icon��$
��Left=�$	��130���� -	   % ��Name$% showBalloonTip��$��Right=�$��	0,0,17,43q�$	��120��$��Show Balloon Tip��$��Bottom=�$��23���� $��10,35.96,21,0q�$	��Top=�$��56.04��$
��Wrap=��� $<�6Displays default balloon tips as supported by the OS. ���� $4�.You can use custom icons under all OS versions���� $.�( (not supported by WinForms NotifyIcon).���� $�.���� -
   $% hideBalloonTip��$��Right=�$��0,0,17,12.52q�$	��120��$��Hide Balloon Tip��$��Bottom=�$��23���� $
��Left=�$��	10,10,0,0q�$	��Top=�$
��Auto��$
��Auto��$
��Bold"�$��	Underline{�$
��Wrap=��� $ �Standard (OS) Balloon Tips��� -   $ Popups��$
��Left=�$��12,543.88,0,0q�$	��Top=�$��445.5��$��282.7��.+��� $��Stretch=�$��Stretch=����	#FF000000$��2,2,2,2q��� -   & ��Name$& lstPopupTrigger��$' �� IsSynchronizedWithCurrentItem$' FalseY�$��10,0,190,10q�( �� SelectedIndex$( 0��$	��117��$��Bottom=����� $ OneWay=�) �� Source#) [ �� $��10,31.7,14,137q�$
��Wrap=��� $ �If the user clicks on the ���� $�Notify���� $�Icon, a ���� $�P���� $&� opup can be opened and displayed���� $D�> that allows the user to quickly interact with the application���� $�. ���� $>�8Unlike custom ToolTips, this works under all OS versions���� $�. ���� $/�)Which mouse button(s) opens the Popup is ���� $�determined by the ���� $��Italic$�$
��Bold"�$�PopupActivation ���� $�	property.���� $m�g If both Popup and ContextMenu are configured for the same mouse buttons, ContextMenu takes precedence.���� �� $�(Note: ���� $o�iIn case left-clicks are used, popups are displayed with a delay to ensure the user does not double-click.���� $�)���� $
��Left=�$��	10,10,0,0q�$	��Top=�$
��Auto��$
��Auto��$
��Bold"�$��	Underline{�$
��Wrap=��� $�Popup Controls��� -   $ ToolTips��$��
10,278,0,0q�$��255.88��$	��Top=�$��447.5��$
��Left=�.+��� $��Stretch=�$��Stretch=����	#FF000000$��2,2,2,2q��� -   $  txtToolTipText��$��
10,0,25,60q�$��Bottom=�$��23��$��THIS IS A SAMPLE TEXT....�����	#FFFF0000�� $��
10,0,25,93q�$
��Wrap=�$��21��$��Bottom=��� $�ToolTipText���� $?�9 (assigned to property and also used by several bindings)���� $�:���� $��10,29.88,10,114q�$
��Wrap=��� $+�%If the user moves the mouse over the ���� $�Notify���� $�Icon, ���� $�
a ToolTip ���� $l�fcan be displayed. Starting from Windows Vista, we have convenient events to display and hide tooltips.���� $G�A You can assign arbitrary UIElements (e.g. User Controls) to the ���� $��Italic$�$
��Bold"�$�TrayToolTip ���� $�	property.���� �� $� ���� $	�If ���� $��Italic$�$
��Bold"�$�TrayToolTip ���� $9�3is not set or the app runs under an older OS (e.g. ���� $�Windows ���� $�	xp), the ���� $�NotifyIcon ���� $�falls back to the ���� $��Italic$�$
��Bold"�$�ToolTipText ���� $�	property.���� -   $��removeToolTip��$
��Left=�$��	10,0,0,10q�$��Bottom=�$	��147��$��24��$��Remove Custom ToolTip���� $��
10,0,25,44q�$��Bottom=�$��16��$
��Wrap=��� $C�=Click to fall back to ToolTipText (sets TrayToolTip to null):���� $
��Left=�$��	10,10,0,0q�$	��Top=�$
��Auto��$
��Auto��$
��Bold"�$��	Underline{�$
��Wrap=��� $�ToolTips and ToolTipText��� -   $ ContextMenus��$
��Left=�$��488,114.5,0,0q�$��370.5��$��255.88��$	��Top=�.+��� ���	#FF000000$��2,2,2,2q��� -   $& lstMenuTrigger��$��
10,0,95,10q�$' FalseY�$	��124��$��Bottom=�$( 1������ $ OneWay=�#) [ �� $��
10,97,48,0q�$	��Top=�$��22��$
��Wrap=��� $.�(Mouse events that open the context menu:���� $��
10,30,10,0q�$	��Top=�$��57��$
��Wrap=��� $/�)Assign a custon context menu through the ���� $��Italic$�$
��Bold"�$�ContextMenu ���� $�property of the ���� $�Notify���� $�
Icon. The ���� $��Italic$�$
��Bold"�$�MenuActivation ���� $B�<property determines what mouse events open the context menu.���� $��	10,10,0,0q�$	��Top=�$
��Auto��$
��Wrap=�$
��Auto��$
��Left=�$
��Bold"�$��	Underline{��� $�Context Menu���� $��	10,10,0,0q�$	��Top=�$
��Auto��$
��Wrap=�$
��Bold"�$��16(�$
��Left=��� $�WPF NotifyIcon 1.0.3���� $��12,57.62,15.5,0q�$	��Top=�$��14(�$��Italic$����	#FF303030$
��Wrap=�$��Stretch=��� $�This ���� $���is a showcase of the different features of the WPF NotifyIcon. Have a look at the used controls and styles in order to see how binding can be supported. For a real-life ���� $�example���� $�, ���� * � Language$* de-ch�$�
check out ���� $)��#http://www.hardcodet.net/netdrives/?�.��� $�	NetDrives��� -   $ CustomBalloons��$��486.5,380.38,0,0q�$��188.2��$	��Top=�$	��372��$
��Left=�.+��� $��Stretch=�$
��Auto��$��2,2,2,2q����	#FF000000�� -   $% showCustomBalloon��$
��Show��$��Right=�$��0,0,91.377,10q�$��71.623��$��23��$��Bottom=��� -   $  customBalloonTitle��$��Bottom=�$��23��$��WPF Balloon��$
" Wrap=�$��10,0,173,10q��� $��10,35,24.377,0q�$	��Top=�$
��Wrap=�$��119.68���� $�Custom ���� $�Balloons are much ���� $*�$ore flexible then standard balloons ���� $�tips ���� $�when it comes to styling.���� $U�O You can display arbitrary UI Elements (e.g. User Controls) as custom balloons.���� �� $>�8Apart from the richer UI, custom balloons also provide a���� $I�Cttached properties and events that can be used to control behavior.���� $B�< Custom balloons also work if the NotifyIcon is not visible.���� �� $�(Hint: Hover over the ���� $�balloon ���� $�to suspend the fade-out.)���� -   $% hideCustomBalloon��$��Close��$��Right=�$��0,0,9.754,10.52q�$��71.623��$��23��$��Bottom=��� $
��Left=�$��	10,10,0,0q�$	��Top=�$
��Auto��$
��Auto��$
��Bold"�$��	Underline{�$
��Wrap=��� $�Custom Balloons��� -   $ Common��$��10,114.5,0,0q�$	��Top=�$��138.38��$��447.5��$
��Left=�.+��� $��2,2,2,2q����	#FF000000�� -   + ��Name$+ iconVisibility��$��10,35.96,0,0q�$	��Top=�$��NotifyIcon Visible��$
$ TrueY�$
��Left=�$��155.42���� -   $& iconList��$��10,77.38,0,0q�$
' TrueY�$( 0��$	��123��$
��Left=�$��51��$	��Top=�.@��� $��16��$��0,4,0,0q�$��16��$��/Icons/Inactive.ico���� $��16��$��0,4,0,0q�$��16��$��/Icons/Error.ico���� $��10,55.38,0,61q�$
��Auto��$
��Wrap=�$
��Left=��� $&� Select an image to switch icons:���� $
��Left=�$��	10,10,0,0q�$	��Top=�$
��Auto��$
��Auto��$
��Bold"�$��	Underline{�$
��Wrap=��� $�Icon / Visibility��