   M S B A M L   `   `   `  ����  	CleanSync    CleanSync.Balloon   5      ^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      8x,http://schemas.microsoft.com/winfx/2006/xaml    5      \ XHardcodet.Wpf.TaskbarNotification, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null)tb http://www.hardcodet.net/taskbar  5      -   5          >�Name$  me��5      $	��120��5      $	��240��5       � 	Resources 5	       
  FadeIn g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value  HighlightCloseButton  FadeCloseButton  FadeBack  FadeOut��%�  &	        &	 
    &	 �    &	 �    &	 �    �� 5
   
   .��5      U� $ 00:00:00X�6,   $
 grid��6A   $ (UIElement.Opacity)�6^   .\�5      �� $ 00:00:00��6'   $ 0Q�6:   �� 5      $ 00:00:01��6'   $
 0.95Q�6:   �� 5      $ 00:00:03��6'   $
 0.95Q�6:   5      5      �� 5   
   .��5      U� $ 00:00:00X�6,   $ imgClose��6A   $ (UIElement.Opacity)�6b   .\�5      �� $ 00:00:00��6'   $	 0.4Q�6:   �� 5      $ 00:00:00.3000000��6'   $ 1Q�6B   5      5      �� 5   
   .��5      U� $ 00:00:00X�6,   $ imgClose��6A   $ (UIElement.Opacity)�6b   .\�5      �� $ 00:00:00��6'   $ 1Q�6:   �� 5      $ 00:00:00.3000000��6'   $	 0.4Q�6B   5      5      �� 5   
   .��5      U� $ 00:00:00X�6,   $
 grid��6A   $ (UIElement.Opacity)�6^   .\�5       �� $ 00:00:00��6'   $ 1Q�6:   �� 5!      $ 00:00:00.3000000��6'   $ 1Q�6B   5"      5#      �� 5$   
   -   5        .��5%      U� $ 00:00:00X�6,   $
 grid��6A   $ (UIElement.Opacity)�6^   .\�5&      �� $ 00:00:00��6'   $ 1Q�6:   �� 5'      $ 00:00:00.3000000��6'   $	 0.2Q�6B   5(      5)      5*       � Triggers 5+      :� 5,   
    :� RoutedEvent$# tb:TaskbarIcon.BalloonShowing��6   .[�5-      �� -   5        	 ��Name$	 FadeIn_BeginStoryboard��5-   C   #��[  6   6C   5.      :� 5/   
   $ Mouse.MouseEnter��6   
 :� 
SourceName$
 imgClose��66   .[�50      �� -   5        $*	 $HighlightCloseButton_BeginStoryboard��50   Q   #��[ 6   6Q   51      :� 52   
   $ Mouse.MouseLeave��6   $
 imgClose��66   .[�53      �� -   5        $%	 FadeCloseButton_BeginStoryboard��53   L   #��[ 6   6L   54      :� 55   
   $ Mouse.MouseEnter��6   .[�56      ��  �� BeginStoryboardName$ FadeIn_BeginStoryboard��6   �� 57      -   5        $	 FadeBack_BeginStoryboard1��57      #��[ 6A   58      :� 59   
   $# tb:TaskbarIcon.BalloonClosing��6   .[�5:      �� -   5        $	 FadeOut_BeginStoryboard��5:   D   #��[ 6   6D   5;      :� 5<   
   $ tb:TaskbarIcon.MouseLeave��6   .[�5=      �� -   5        $!	 TaskbarIcon_BeginStoryboard��5=   D   #��[ 6   6D   5>      .��5@      � -	   5         �Name$
 grid��5@      .+�5A   
   �� $��Stretch=�5B      $��5,5,5,5q�5C      $��1,1,1,1q�5D      
������  J� Effect 5F      3  -System.Windows.Media.Effects.DropShadowEffect  5G         Color$ 	#FF747474��6#   ��5I      �� 5J       �� EndPoint$ 0.5,1&�5K       �� 
StartPoint$ 0.5,0&�5L      .��5M      �  � Color$ 	#FF4B4B4B��5N       � Offset$ 0Q�5O      � 5P      $ 	#FF8F8F8F��5Q      $ 1Q�5R      5S      5U      �� 5V   
   $
��Left=�5W      $��0,10,0,0q�5X      $��72��5Y      $��Pic/Info.png��5Z      $
��Fill=�5[      $��72��6   $	��Top=�6"   �� 5\   
   -
   5        $��BallonContent��5\      $��72,49.2,10,0q�5]      $	��Top=�5^      
�������$
��Wrap=�5`      6   F� 5a   
   ���	#FFFFFFFF �� Stretch$
 Fill=�5c      $��72,38.2,34,0q�5d      $	��Top=�5e      $��1��5f      ���Ѐ����`����`	 J� SnapsToDevicePixels . ��5h      �� 5i      $ 	0.973,0.5&�5j      $ 	0.005,0.5&�5k      .��5l      � $ Transparent��5m      $ 1Q�5n      � 5o      $ 
WhiteSmoke��5p      $ 0Q�5q      5r      5t      �� 5u   
   
�������$��
72,10,10,0q�6,   $	��Top=�6@   $
��23.2��6X   $
��Wrap=�6�   $
��Bold"�6�   ��6f   ��  �� Path$ BalloonText� �� ElementName$ me�� �� Mode$ Default=�6�   �� 5v   
   -   5         ��Name$ imgClose��5v   �   $��Right=�6   $��	0,10,10,0q�6,   $	��Top=�6?   $��16��6W   $��16��6b   $��Pic/Close.png��6n   $
��Fill=�6�    J� Opacity$	 0.4Q�6�    � ToolTip$ Close Balloon��6�   6�   5x      5y      