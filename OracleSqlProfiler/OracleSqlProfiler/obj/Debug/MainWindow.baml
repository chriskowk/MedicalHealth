   M S B A M L   `   `   `  ����L  HOracleSqlProfiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:OracleSqlProfilerOracleSqlProfiler  5   	   "    OracleSqlProfiler.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:OracleSqlProfiler     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5       � Title$ OracleSqlProfiler��5   	   $	��768��6#   $
��1024��60    � Icon$ 1.ico��6=   .��5	      � $��5,5,5,5q�6    � 	Resources 5
       �� 
TargetType �� Property �� Value��%  '��       '�� s     �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ Right=�64   5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ 30��6'   �� 5       � ��$ 80��6&   5      5       � RowDefinitions 5      �� 5   
   $��30 �6   �� 5   
   $��5 �6   �� 5   
   $��30 �6   �� 5   
   $��5 �6   �� 5   
   $��30 �6   �� 5   
   $��5 �6   �� 5   
   $��30 �6   �� 5   
   $��5 �6   �� 5   
   $��30 �6   �� 5   
   $��5 �6   �� 5    
   $��16 �6   �� 5!   
   $��5 �6   �� 5"   
   $��* �6   �� 5#   
   $��5 �6   �� 5$   
   $	��100 �6    � ColumnDefinitions 5&      �� 5'   
   $��80 �6   �� 5(   
   $	��160 �6   �� 5)   
   $	��100 �6   �� 5*   
   $	��160 �6   �� 5+   
   $��* �6   .+�5-      �� $��0��6   .��6   Source6"   �� 5.      -   5        $��	tb_source��5.      $��0��6   $��1��6,   $��3��6<   $��Center=�6P   $ ��172.18.99.243:1521/orclbak��6k   	 � ToolTip$ 	 like xx.xx.xx.xx:1521/orcl��6�   �� 5/      -   5        $��tb_log��5/      
�������
 �� 
IsReadOnly
 . $��0��6D   $��5��6Q   $��9��6a   $��Stretch=�6r   " �� VerticalScrollBarVisibility$ Visible=�6�   $��5,0,0,0q�6�   �� 50      $��2��6   .��6   	User ID6#   �� 51      -   5        $��tb_user��51      $��2��6   $��1��6*   $��Center=�6:   $
��apps��6U   �� 52      $��2��6   $��2��6   .��6)   
Password63   G� 53      -   5        $��tb_password��53      $��2��6%   $��3��62   $��Center=�6B   �� 54      -   5        $��cb_save��54      $��4��6   $��1��6+   $��Save to table��6;   $
��Left=�6S   $��Center=�6n   6�   �� 55      -   5        $��
tb_oraname��55      $��4��6    $��2��6-   $��2��6=   $��Center=�6Q   $��10,0,0,0q�6l   �� 56      -   5        $��cb_curruser��56      $��6��6"   $��1��6/   $��Trace only current user��6?   $
��Left=�6a   $��Center=�6|   �� 57      $��6��6   $��2��6   .��6)   Trace module67   �� 58      -	   5        $��	tb_module��58      $��6��6   $��3��6,   $��Center=�6<   $��w3wp.exe��6W   � 59      $��8��6   $��2��6   $��2��6(    5:   
   �� 5;      $��* �6   .+�5=   
   �� $��Right=�6   $��Center=�61    �� Orientation$ 
Horizontal=�6L   .+�5>      �� -
   5        $��button1��5>   "   $	��2,0q�6   {�. $��_Start��6B   6S   �� 5?      -   5        $��button2��5?   "   $	��2,0q�6   {�.  $��St_op��6C   6S   �� 5@      -   5        $��	btn_close��5@   &   $��2,0,0,0q�6   $��E_xit��67   6G   5A      5B      �� 5D      $��10��6   $��5��6   XVshows only 500 lines order by LAST_ACTIVE_TIME desc, see more to save data to table...6�   &   System.Windows.Controls.DataGrid  5E      -   5        $��dg_SQLlines��5E      $��12��6:   $��5��6H      
IsReadOnly .    AutoGenerateColumns .  ��6"   ��    RowStyle 5F   
   �� 5G      )  #System.Windows.Controls.DataGridRow"  6   .�5H      ��  � ��$	 100��6.   5I         Columns 5K   
   0  *System.Windows.Controls.DataGridTextColumn  5L      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6!      Header$
 ROW#��6,   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6:   �� *ROWNUM+  5M      $ 70 6!   $ SCHEMA��6,    6<   �� *SCHEMA+  5N      $ 70 6!   $ MODULE��6,    6<   �� *MODULE+  5O      $ * 6!      MinWidth$	 120Q�6+   $ SQL TEXT��6:    6L   �� *
SQL_TEXT+  5P      $	 100 6!   $ PARSE CALLS��6-    6B   �� *PARSE_CALLS+  5Q      $ 80 6!   $ BUFFER GETS��6,    6A   �� *BUFFER_GETS+  5R      $ 80 6!   $ 
DISK READS��6,    6@   �� *
DISK_READS+  5S      $ 90 6!   $ 
EXECUTIONS��6,    6@   �� *
EXECUTIONS+  5T      $ 80 6!   $ CPU TIME��6,    6>   �� *
CPU_TIME+  5U      $	 100 6!   $ CPU TIME PER��6-    6C   �� *CPU_TIME_PER_EXE+  5V      $ 80 6!   $ ELAPSED PER��6,    6A   �� *ELAPSED_TIME_PER_EXE+  5W      $	 125Q�6!   $ LAST LOAD TIME��60    6H   �� *LAST_LOAD_TIME+ �� StringFormat$ yyyy-MM-dd HH:mm:ss��  5X      $	 125Q�6!   $ LAST ACTIVE TIME��60    6J   �� *LAST_ACTIVE_TIME+$ yyyy-MM-dd HH:mm:ss��5Z      �� 5[      -   5        $��	tb_Status��5[      
�������$��14��68   $��5��6F   $��Stretch=�6Z   $ �� HorizontalScrollBarVisibility$
 Auto=�6v   $ Visible=�6�   5\      5]      