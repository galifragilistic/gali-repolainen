   M S B A M L   `   `   `  ����J  FTicTacToeServer, Version=0.0.0.1, Culture=neutral, PublicKeyToken=nullE'clr-namespace:TicTacToeServer.ViewModelTicTacToeServer.ViewModel  5   	        TicTacToeServer.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (ignorehttp://www.ignore.com     5   	   6vm'clr-namespace:TicTacToeServer.ViewModel     5   	   $	��300��5	   	   $	��300��5
   	     � Title$  TicTacToe Server��5   	    � DataContext 5   	   �� *Main+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ Skins/MainSkin.xaml?�6%   %    5      .��5      � -   5         �Name$ 
LayoutRoot��5       � RowDefinitions 5   
   �� 5      �� 5      $��40 �6   .+�5   
   �� $
��Auto=�6   $
��Auto=�6:   .��5      �� {�.  $��2 2 2 0q�6=   ��6   �� *Log+6=   5      � 5   
   $��1��6    � ColumnDefinitions 5      �� 5       �� 5!      �� 5"      .+�5(      �� $��4q�60   ��6   �� *
ServerIP+6=   �� 5)      $��1��62   $��4q�6B   ��6   �� *
ServerPort+6O   �� 5*      $��4q�6   $��2��5+      ��5*   !   �� *ServerButtonContent+��6I   �� *StartServerCommand+5+      5-      5.      5/      