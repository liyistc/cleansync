   M S B A M L   `   `   `  ���� I  ESample Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Samples.Tutorials.CommandsSamples.Tutorials.Commands  .    (Samples.Tutorials.Commands.CommandWindow   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    \ XHardcodet.Wpf.TaskbarNotification, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null)tb http://www.hardcodet.net/taskbar  4local(clr-namespace:Samples.Tutorials.Commands  :d2http://schemas.microsoft.com/expression/blend/2008  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  $	��300��$	��300��.���   � 	Resources  3   -Samples.Tutorials.Commands.ShowMessageCommand   MessageCommand��%   &	          .+�3  -Hardcodet.Wpf.TaskbarNotification.TaskbarIcon  -     Name$ CustomCommandNotifyIcon��   
IconSource$ /Icons/Error.ico��   LeftClickCommand# [      LeftClickCommandParameter$# Left mouse button was clicked��   DoubleClickCommand# [  "   DoubleClickCommandParameter$  Double click on NotifyIcon��  -   $ RoutedCommandNotifyIcon��$ /Icons/Inactive.ico��$ ApplicationCommands.Cut��$ ApplicationCommands.Paste��   LeftClickCommandTarget ��  �� ElementName$ txtInput��	   DoubleClickCommandTarget	 �� $ txtInput���� $��
26,26,24,0q�$	��Top=�$
��Bold"�$
��Wrap=��� 
 � Language$
 de-ch�$M�GLeft / Double clicks on red NotifyIcon executes simple custom commands.���� $��26,112,24,80q�$
��Bold"�$
��Wrap=��� $
 de-ch�$G�ALeft / Double clicks on grey NotifyIcon executes routed commands.���� �� $
 de-ch�$&� Single click: Cuts selected text���� �� $
 de-ch�$-�'Double click: Paste text from clipboard���� -    ��Name$ txtInput��$��
26,0,24,48q�$��Bottom=�$��22��$��hello world�� �� TextWrapping$
 Wrap=�