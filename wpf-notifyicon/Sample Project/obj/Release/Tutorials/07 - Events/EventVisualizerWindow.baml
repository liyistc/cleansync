   M S B A M L   `   `   `  ����   Sample Project4    .Samples.Tutorials.Events.EventVisualizerWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    \ XHardcodet.Wpf.TaskbarNotification, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null)tb http://www.hardcodet.net/taskbar  $	��300��$	��300��  � 	Resources     ShowMovement g� AutoReverse g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value  ShowMouseUp  ShowToolTipOpened��%�  &	        &	 �    &	 :    ��  . .��U� $ 00:00:00X�$ MoveIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.1000000��$	 1.2Q�U� $ 00:00:00X�$ MoveIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.1000000��$	 1.2Q���  . .��U� $ 00:00:00X�$ LeftMouseIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.1000000��$
 1.35Q�U� $ 00:00:00X�$ LeftMouseIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.1000000��$
 1.35Q���  . .��U� $ 00:00:00X�$ ToolTipIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.2000000��$	 1.4Q�U� $ 00:00:00X�$ ToolTipIndicator��$V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $ 00:00:00��$ 1Q��� $ 00:00:00.2000000��$	 1.4Q� � Triggers :�  :� RoutedEvent$" tb:TaskbarIcon.TrayMouseMove��	 :� 
SourceName$	 
notifyIcon��.[��� #��[  :� $$ tb:TaskbarIcon.TrayLeftMouseUp��$	 
notifyIcon��.[��� -   
 ��Name$!
 ShowMouseUp_BeginStoryboard��#��[ :� $$ tb:TaskbarIcon.TrayToolTipOpen��$	 
notifyIcon��.[��� -   $'
 !ShowToolTipOpened_BeginStoryboard��#��[ .��� .+�3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  -     Name$ 
notifyIcon��   ToolTipText$ hello world��   
IconSource$ /Icons/Error.ico��A� -    A�Name$ MoveIndicator��$
��Left=�$��	24,62,0,0q�$	��Top=�$��19��$��19�����	#FF549D2D J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~���  �� ScaleX$ 1Q� �� ScaleY$ 1Q���  �� AngleX$ 0Q� �� AngleY$ 0Q���  �� Angle$ 0Q�T�  T� X$ 0Q� T� Y$ 0Q�����  �� EndPoint$ 0.528,0.694&� �� 
StartPoint$ -0.056,-0.118&�.���  � Color$ 	#FFFFFFFF�� � Offset$ 0Q�� $ 	#FF65A135��$ 1Q�A� -   $ LeftMouseIndicator�����	#FF549D2D$
��Left=�$��
24,106,0,0q�$	��Top=�$��19��$��19��$ 0.5,0.5&�z�V� .~��� �� $ 0Q�$ 0Q��� $ 0Q�T� $ 0Q�$ 0Q����� $ 0.528,0.694&�$ -0.056,-0.118&�.��� $ 	#FFFFFFFF��$ 0Q�� $ 	#FF65A135��$ 1Q�A� -   $ ToolTipIndicator�����	#FF549D2D$
��Left=�$��	24,0,0,94q�$��19��$ 0.5,0.5&�$��19��$��Bottom=�z�V� .~��� $ 1Q�$ 1Q��� $ 0Q�$ 0Q��� $ 0Q�T� $ 0Q�$ 0Q����� $ 0.528,0.694&�$ -0.056,-0.118&�.��� $ 	#FFFFFFFF��$ 0Q�� $ 	#FF65A135��$ 1Q��� $��
63,62,91,0q�$	��Top=�$��19��$
��Wrap=���  � Language$ de-ch�$�TrayMouseMove Event���� $��63,106,91,0q�$	��Top=�$��19��$
��Wrap=��� $ de-ch�$�TrayLeftMouseUp Event���� $��
63,0,91,94q�$
��Wrap=�$��Bottom=�$��19���� $ de-ch�$�TrayToolTipOpen Event���� $��
10,10,10,0q�$	��Top=�$��31��$
��Wrap=�$
��Bold"��� $ de-ch�$N�HThe green ellipses are animated based on routed events of the NotifyIcon��