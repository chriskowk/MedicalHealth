   M S B A M L   `   `   `  ����  TFSideKicks"    TFSideKicks.FileDateModifier   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   X TXceed.Wpf.Toolkit, Version=2.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   :d2http://schemas.microsoft.com/expression/blend/2008  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5	   	   -   5          � Title$  File Date Modifier��5   	    � WindowStyle$ 
ToolWindow=�6$    � 
ResizeMode$ NoResize=�5   	   
������� � SizeToContent$ WidthAndHeight=�5
   	    � WindowStartupLocation$ CenterScreen=�6(    � Icon$ schedule.ico��6M   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��10 �6   .+�5   
   �� -   5        $��label1��5   ]   $��
Path Type:��6   $��28��6%   $
��Left=�61   $��4,0,0,0q�6L   $	��Top=�6k   $��1��6�   	� 5   
   -   5        $��radioButton1��5   �   $
��File��6    	� 	GroupName$ pathType��6%   $��16��6:   $
��Left=�6F   $��75,6,0,0q�6�   $	��Top=�6�   $��1��6�    d� 	IsChecked 6a   �� *FilePathSelected+6�   	� 5   
   -   5        $��radioButton2��5   �   $��	Directory��6   $ pathType��6*   $��16��6?   $
��Left=�6K   $��	123,6,0,0q�6�   $	��Top=�6�   $��82��6�   $��1��6�    6f   �� *DirPathSelected+6�   �� 5   
   	 �� Orientation$	 
Horizontal=�6   $��Bottom=�6.   $��2��6I   .+�5      �� $
��auto=�6   $
��Auto=�6@   $	��6,0q�6c   $	��405��6p   $
��Auto��6|   .��5      �� -   5        $��_txtSelectedPath��5   �   $
��Left=�6   $	��Top=�6x   $
��auto��6�   $	��405��6�   ��65   �� *SelectedPath+
 �� UpdateSourceTrigger$
 PropertyChanged=�6�   5      �� 5      $��	Browse...��6   $��25��6)   $	��Top=�65   $��75��6N   ��6Y   �� *BrowseCommand+5      �� 5   
   $��Center=�6   $	��Top=�62   $��	66,3,65,0q�6J   $��4��6]   .+�5      �� $	 
Horizontal=�6   $��1��62   $��50��6?   $��5q�6K   .+�5      �� $��75��6   $��Created:��6#   $��Center=�66   $��Center=�6Z   &   Xceed.Wpf.Toolkit.DateTimePicker  5       $	��250��6&   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Value 62   �� *CreatedDateTime+{�6T   ��  �� ElementName$ createdCheckBox�� �� Path$ 	IsChecked�,  &Xceed.Wpf.Toolkit.Primitives.InputBase   CultureInfo 6�   �� *CultureInfo+�� 5!      -   5        $��createdCheckBox��5!      $��Center=�63   $��Center=�6W   $��15��6y   $��10q�6�    6�   �� *CreatedChecked+5"      �� 5#      $	 
Horizontal=�6   $��2��62   $��50��6?   $��5q�6K   .+�5$      �� $��75��6   $��	Modified:��6#   $��Center=�67   $��Center=�6[     5%      $	��250��6&    62   �� *ModifiedDateTime+{�6U   �� $ modifiedCheckBox��$ 	IsChecked� 6�   �� *CultureInfo+�� 5&      -   5        $��modifiedCheckBox��5&      $��Center=�63   $��Center=�6W   $��15��6y   $��10q�6�    6�   �� *ModifiedChecked+5'      �� 5(      $	 
Horizontal=�6   $��3��62   $��50��6?   $��5q�6K   .+�5)      �� $��75��6   $��	Accessed:��6#   $��Center=�67   $��Center=�6[     5*      $	��250��6&    62   �� *AccessedDateTime+{�6U   �� $ accessedCheckBox��$ 	IsChecked� 6�   �� *CultureInfo+�� 5+      -   5        $��accessedCheckBox��5+      $��Center=�63   $��Center=�6W   $��15��6y   $��10q�6�    6�   �� *AccessedChecked+5,      �� 5-      $	 
Horizontal=�6   $��4��62   $��50��6?   $��5q�6K   .+�5.      �� $��UPDATE��6   $��40��6*   $��Center=�66   $	��Top=�6S   $	��150��6k   $��5,5,10,5q�6w   ��6�   �� *UpdateDateCommand+�� 5/      -	   5        $��	_btnClose��5/   �   $��CLOSE��6   $��40��6)   $��Center=�65   $	��Top=�6R   $	��150��6j   $��10,5,5,5q�6v   6�   50      51      �� 52   
   -
   5        $��textBox1��52   b   $��Select Path:��6   $��29��6(   $
��Left=�64   $��	12,12,0,0q�6O   $	��Top=�6r   $	��142��6�   $��SemiBold"�6�   $��15(�6�   $��0��6�   �� 53   
   $��Get Latest Version��6   $��30��6.   $��Center=�6:   $��5,5,5,5q�6W   $��Center=�6h   $	��120��6�   ��6�   �� *TFGetLatestCommand+�� 54   
   $��Exec WinRAR��6   $��30��6'   $��Right=�63   $��5,5,10,5q�6O   $��Center=�6a   $	��100��6|   ��6�   �� *ExecWinRARCommand+�� 56   
   -   5        $��
textBlock1��56   q   $��15(�6   $��SemiBold"�6"   $��29��68   $
��Left=�6D   $��12,8,0,0q�6_   $��File Date Attributes:��6�   $	��Top=�6�   $	��142��6�   $��3��6�   �� 57   
   -   5        $��
rectangle1��57   A   $��4��6   $	��254��6!   $��	12,0,11,0q�6.   
���   �6S   58      59      