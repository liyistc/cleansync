   M S B A M L   `   `   `  ����   CleanSnycMini    CleanSyncMini.GUI   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     8x,http://schemas.microsoft.com/winfx/2006/xaml    -     � Title$  	CleanSync��$	��600��$	��800�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � 
ResizeMode$ NoResize=� � AllowsTransparency .  J� Opacity$ 2Q�����  �� ImageSource$ Pic/Background.png�� �� Opacity$	 1.0Q� � 	Resources    CheckboxCell1	 �� Orientation
 d� 	IsChecked �� Path �� Mode  CheckboxCell2  CheckboxCell3��%�  &	        &	 �     &	 2    �� .R��� $	 
Horizontal=�.+��� $��Center=�
 �� $ 
PCSelected�$ TwoWay=��� $��18��$��22������ $ PCConflictTypeImage��� .R��� $��Stretch=�$	 
Horizontal=�.+��� $��18��$��18������ $ itemIconImage��� $	��450��$��Center=�$
��Left=����� $
 Name��� .R��� $	 
Horizontal=�.+��� $��Center=�
 �� $ USBSelected�$ TwoWay=��� $��18��$��22������ $ USBConflictTypeImage�.��� -   $��Frame��$��Stretch=�$��Stretch=�.+�� -   $��JobListHost��$	��200��$	��400��$
��Left=�$	��Top=�$��	40,45,0,0q�
������ $y�Visible=�.+��� -   $��JobListHostBox��$��Stretch=�$��Stretch=�$��Job List��$��Times New Roman)�$��14(�
������ �� -   $��JobList��$��Stretch=�$��Stretch=�$��0,20,0,0q�
������ 
������ � -   $��JobStateIndicator��$	��200��$��90��$
��Left=�$	��Top=�$��
40,450,0,0q�
������ $y�Visible=�.+��� -   $��	JobState1��$	 
Horizontal=�$��30��$��Stretch=�$	��Top=�$��0,0,0,0q�
������ .+��� $��30��$��30��$
��Left=�$��Center=����� $ Pic/green.png���� $
��Left=�$��Stretch=�$
��Left=�$��Ready to Sync��$��0,0,0,0q�$��10(�$��25���� -   $��	JobState2��$	 
Horizontal=�$��30��$��Stretch=�$	��Top=�$��0,30,0,0q�
������ .+��� $��30��$��30��$
��Left=�$��Center=����� $ Pic/yellow.png���� $
��Left=�$��Stretch=�$
��Left=�$#��No Connected Removable Device��$��0,0,0,0q�$��25��$��10(��� -	   $��	JobState3��$	 
Horizontal=�$��30��$��Stretch=�$	��Top=�$��0,60,0,0q�
������ .+��� $��30��$��30��$
��Left=�$��Center=����� $ Pic/red.png���� $
��Left=�$��Stretch=�$
��Left=�$��Not Fully Setup��$��0,0,0,0q�$��25��$��10(�� -
   $��	InforHost��$	��520��$	��400��$
��Left=�$	��Top=�$��
245,45,0,0q�
������ .+�� -   $��MainFrameInfor��$��Stretch=�$��Stretch=�
������ $y�Visible=�.+��� $��Stretch=�$��Stretch=�$��	0,0,0,200q�
������ .��� .+��� -   $��JobName��$
��Left=�$��	50,20,0,0q�$	��120��$��30��$	��Top=�
������ $��	JobName :��
������ $��0q��� -   $��	LocalPath��$
��Left=�$��	50,50,0,0q�$	��120��$��30��
������ $��Local Path :��
������ $��0q�$	��Top=��� -   $��
RemotePath��$
��Left=�$��	50,80,0,0q�$	��120��$��30��$	��Top=�
������ $��Remote Path :��
������ $��0q��� -   $��AbsoluteUSBPath��$
��Left=�$��
50,110,0,0q�$	��120��$��30��$	��Top=�
������ $��Removable Path :��
������ $��0q�$
��Left=�$	��Top=��� -   $��JobNameDisplay��$��30��$��
170,20,0,0q�$	��Top=�$
��Left=��� -   $��LocalPathDisplay��$��30��$��
170,50,0,0q�$	��Top=�$
��Left=��� -   $��RemotePathDisplay��$��30��$��
170,80,0,0q�$	��Top=�$
��Left=��� -   $��USBPathDisplay��$��30��$��170,110,0,0q�$	��Top=�$
��Left=��� $��Stretch=�$��Stretch=�$��	0,200,0,0q�
������ .��� .+��� -   $��
Automation��$��30��$
��Left=�$��	50,20,0,0q�$	��Top=�$	��120�� �� SelectedIndex$ 0��
������ 
������ .@��� .��<Automation>�� -   $��Filter��$��30��
������ 
������ $
��Left=�$��	50,70,0,0q�$ 0��$	��120��$	��Top=�.@��� .��
<Filter>�� -   $��RunMode��$��30��
������ 
������ $
��Left=�$��
50,110,0,0q�$ 0��$	��120��$	��Top=�.@��� .��
<Run Mode>� -   $��NewJobFrameInfor��$y�Hidden=�$��Stretch=�$��Stretch=�.+��� $��Stretch=�$	��Top=�$��0,0,0,0q�$��70��
������ .��� .+��� -   $��
NewJobName��$
��Left=�$��	50,25,0,0q�$	��200��$��20��$	��Top=�  WaterMarkJobName#��[ �� $��Stretch=�$	��Top=�$��0,70,0,0q�$	��200��
������ .��� .+��� -   $��AttachDropBox1��$
��Left=�$��	50,15,0,0q�$	��150��$��20��$	��Top=�  WaterMarkFolderSelection#��[ �� -   $��AttachDropBox2��$
��Left=�$��
300,15,0,0q�$	��150��$��20��$	��Top=�#��[ �� -   $��FolderSelection1��$	��150��$
��Left=�$��	50,35,0,0q� J� 	AllowDrop . ���� $ Pic/Folderopenpc.png���� -   $��FolderSelection2��$	��150��$
��Left=�$��
300,35,0,0q� . $��2������ $ Pic/Folderopenusb.png���� $��Stretch=�$	��Top=�$��	0,270,0,0q�$	��100��
������ .��� .+��� -   $��NewJobAutomation��$��2��$��20��$
��Left=�$��	50,10,0,0q�$	��Top=�$	��120��$ 0��
������ 
������ .@��� .��<Automation>�� -   $��NewJobFilter��
������ 
������ $
��Left=�$��	50,40,0,0q�$ 0��$	��120��$��20��$	��Top=�.@��� .��
<Filter>�� -   $��NewJobRunMode��
������ 
������ $
��Left=�$��	50,70,0,0q�$ 0��$	��120��$��20��$	��Top=�.@��� .��
<Run Mode>� -    $��FirstSyncProgressBar��$��30��$��0,0,0,0q�$��Bottom=�$��Stretch=�$y�Hidden=��� -!   $��BarLabel��$��30��$��	0,370,0,0q�$��Stretch=�$��Bottom=�$y�Hidden=�� -"   $��AcceptFrameInfor��$y�Hidden=�$��Stretch=�$��Stretch=�.+��� $��Stretch=�$��Stretch=�$��	0,0,0,240q�
������ .��� .+��� -#   $��AcceptJobName��$
��Left=�$��	50,20,0,0q�$	��120��$��30��$	��Top=�
������ $��	JobName :��
������ $��0q��� -$   $��AcceptLocalPath��$
��Left=�$��	50,50,0,0q�$	��120��$��30��$��1��$��1��
������ $��Local Path :��
������ $��0q�$	��Top=��� -%   $��AcceptRemotePath��$
��Left=�$��	50,80,0,0q�$	��120��$��30��$��1��$��1��$	��Top=�
������ $��Remote Path :��
������ $��0q��� -&   $��AcceptUSBPath��$
��Left=�$��
50,110,0,0q�$	��120��$��30��$��1��$��1��$	��Top=�
������ $��Removable Path :��
������ $��0q��� -'   $��AcceptJobNameDisplay��$��30��$��
170,20,0,0q�$
��Left=�$	��Top=�$	��200���� -(   $��AcceptLocalPathDisplay��$��30��$��
170,50,0,0q�$
��Left=�$	��Top=�$	��200���� -)   $��AcceptRemotePathDisplay��$��30��$��
170,80,0,0q�$
��Left=�$	��Top=�$	��200���� -*   $��AcceptUSBPathDisplay��$��30��$��170,110,0,0q�$
��Left=�$	��Top=�$	��200���� $��Stretch=�$��Stretch=�$��	0,160,0,0q�
������ .��� .+��� -+   $��AttachDropBox��$
��Left=�$��	50,20,0,0q�$	��150��$��20��$	��Top=�#��[ �� -,   $��FolderSelection��$	��130��$	��150��$
��Left=�$��	50,40,0,0q�$	��Top=� . ���� $ Pic/Folderopenpc.png��� --   $��ButtonsHost��$	��520��$��90��$
��Left=�$	��Top=�$��245,450,0,0q�
������ .+�� -.   $��NewJobFrameGridButtons��$��Stretch=�$��Stretch=�
������ $y�Hidden=�.+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -/   $��	FirstSync��$
��Left=�$	��Top=�$��80��$��30��$��
160,30,0,0q�.��	FirstSync�� -0   $��CancelCreat��$��Right=�$	��Top=�$��80��$��30��$��
0,30,160,0q�.��Cancel� -1   $��MainFrameGridButtons��$y�Visible=�$��Stretch=�$��Stretch=�
������ .+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -2   $��NewJob��$
��Left=�$	��Top=�$��	40,30,0,0q�$��80��$��30��.��	New Job�� -3   $��	RemoveJob��$��
160,30,0,0q�$	��Top=�$��80��$��30��$
��Left=�.��
Remove Job�� -4   $��Analyse��$��
280,30,0,0q�$	��Top=�$��80��$��30��$
��Left=�.��	Analyse�� -5   $��	StartSync��$
��Left=�$	��Top=�$��
400,30,0,0q�$��80��$��30��.��
Start Sync� -6   $��AcceptFrameGridButtons��$y�Hidden=�$��Stretch=�$��Stretch=�
������ .+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -7   $��Accept��$��Center=�$��Center=�$��30��$��80��.��Accept� -8   $��Conflict��$	��720��$	��400��$��Center=�$	��Top=�$��0,45,0,0q�
������ $y�Hidden=�.+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -9   $��ConflictList��
������ $��Stretch=�$��Stretch=����� *	Conflicts+ �� View �� .P��� $��
PC Updates�� �� CellTemplate# [  �� $��Conflict File Paths��# [ �� $��USB Updates��# [ � -:   $��ConflictButtons��$	��720��$��90��$��Center=�$	��Top=�$��	0,450,0,0q�
������ $y�Hidden=�.+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -;   $��Confirm��$
��Left=�$	��Top=�$��30��$��80��$��
260,30,0,0q�.��	Confirm�� -<   $��ConflictCanel��$��Right=�$	��Top=�$��30��$��80��$��
0,30,260,0q�.��Cancel� -=   $��Analyser��$	��720��$	��380��$��Center=�$	��Top=�$��0,45,0,0q�
������ $y�Hidden=�.+��� $��Stretch=�$��Stretch=�$��
0,0,360,30q�
������ .��� .+�R� ->   $��CompareResultLeft��$��Stretch=�$��Stretch=����� $ Pic/Computer.png��$	 0.3Q����� .���  �� Property �@Q�
IsExpanded �� Value .  �� Triggers ��  �� Binding �� $
 Type� �� Value menu.a���  �@Q�
IsSelected . �� $��Stretch=�$��Stretch=�$��
360,0,0,30q�
������ .��� .+�R� -?   $��CompareResultRight��$��Stretch=�$��Stretch=����� $ Pic/USB.png��$	 0.3Q����� .���  �@Q�
IsExpanded .  ��  �� $
 Type� menu.a���  �@Q�
IsSelected . �� $��Stretch=�$
��Left=�$��	0,350,0,0q�$	��360��
������ .��� .+��� $	 
Horizontal=�.+��� -@   $��PCFreeSpace��$��Stretch=�$
��Left=�$��0,0,0,0q�$	��180��
������ �� -A   $��PCRequiredSpace��$��Stretch=�$
��Left=�$��0,0,0,0q�$	��180��
������ �� $��Stretch=�$��Stretch=�$��360,350,0,0q�$	��360��
������ .��� .+��� $	 
Horizontal=�.+��� -B   $��RDFreeSpace��$��Stretch=�$
��Left=�$��0,0,0,0q�$	��180��
������ �� -C   $��RDRequiredSpace��$��Stretch=�$
��Left=�$��0,0,0,0q�$	��180��
������ � -D   $��AnalyseProgressBar��$	��720��$��25��$��Center=�$	��Top=�$��	0,425,0,0q�
������ $y�Hidden=�.+�� -E   $��SyncProgressBar��$��30��$��Stretch=�$��Stretch=��� -F   $��SyncBarLabel��$��30��$��0,0,0,0q�$��Bottom=�$��Stretch=�� -G   $��AnalyseButtons��$	��720��$��90��$��Center=�$	��Top=�$��	0,450,0,0q�
������ $y�Hidden=�.+�� -H   $��AnalyseFrameGridButtons��$��Stretch=�$��Stretch=�
������ .+��� $��Stretch=�$��Stretch=�
������ .��� .+��� -I   $��AnalyseStartSync��$
��Left=�$	��Top=�$��
260,30,0,0q�$��30��$��80��.��
Start Sync�� -J   $��AnalyseCancel��$��Right=�$	��Top=�$��
0,30,260,0q�$��80��$��30��.��Cancel�� -K   $��Close��$��20��
������ 
���   �
������ $��0q�$	��Top=�$ 1Q�{�. ��. $��Right=�$��20��.��X�� -L   $��minmize��
������ 
������ $��0q�
���   �$��Right=�{�. ��. $��0,0,20,0q�$ 1Q�$��20��$��1��$��20��$	��Top=�.��-