   M S B A M L   `   `   `  ����J  FTicTacToeClient, Version=0.0.0.1, Culture=neutral, PublicKeyToken=nullE'clr-namespace:TicTacToeClient.ViewModelTicTacToeClient.ViewModel  5   	   =#clr-namespace:TicTacToeClient.ViewsTicTacToeClient.Views  5	   	        TicTacToeClient.MainWindow   5   	   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   8x,http://schemas.microsoft.com/winfx/2006/xaml    5   	   >d2http://schemas.microsoft.com/expression/blend/2008    5   	   Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5   	   &ignorehttp://www.ignore.com    5   	   4vm'clr-namespace:TicTacToeClient.ViewModel    5   	   2view#clr-namespace:TicTacToeClient.Views    5	   	   -   5          �Name$  winMain��5   	   $	��400��5   	   $	��300��5   	   $	��400��5   	   $	��300��5   	   $	��400��5   	   $	��300��5   	    � Title$ 	TicTacToe��5   	    � DataContext 5   	   �� *Main+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ Skins/MainSkin.xaml?�6%   %    5      .��5      � -   5         �Name$ 
layoutGrid��5       � ColumnDefinitions 5   
   �� 5      	 � RowDefinitions	 5    
   �� 5!      $��50 �6   �� 5"      �� 5#      $��50 �6   .+�5'   
   � $��0��6   	 5(      �� 5)      $��1* �6    �� 5*      $��2* �6     5,      �� 5-      �� 5.      �� 5/      .+�52      �� $
��Bold"�6   $	��You��6&   $��0q�64   $��Center=�6@   ��6d   �� *	MyColor+�� 53      $
��Bold"�6   $
��Them��6&   $��0q�65   $��Center=�6A   $��2��6e   ��6u   �� *
TheirColor+�� 55      $��VS��6   $��1��6!   $��2��61   $��Center=�6B   $��0q�6f   $��Center=�6r   $��30(�56      $
��Bold"�6"   �� 58      $
��Bold"�61   $��1��6C   $��0q�6P   $��Center=�6\   $��Center=�59      $��24(�66   ��58      �� *
MyPoints+��59   D   �� *	MyColor+�� 5:      $
��Bold"�67   $��1��6I   $��2��6V   $��0q�5;      $��Center=�6    $��Center=�6D   $��24(�6f   ��5:      �� *OpponentPoints+��5;   t   �� *
TheirColor+5<      $   TicTacToeClient.Views.GameView  5?   
   $��1��6Y   
   
GameStatus
 6   �� *
GameStatus+{�6:   �� *
IsMyTurn+6h   � 5A   
   $��2��6    5B      �� 5C      $��3* �6#   �� 5D      $��1* �6#   �� 5E      $��2* �6#   	 5G      �� 5H      �� 5I      .+�5L      �� $��25��61   $��Center=�6=   ��6   �� *	IPAddress+{�6`   �� *EnableControls+�� 5M      $��25��6,   $��1��68   $��Center=�6H   ��6   �� *Port+{�6k   �� *EnableControls+�� 5N      $��2��6d   $��25��6t   ��6   �� *ConnectionButtonContent+��6A   �� *ConnectCommand+6t   �� 5Q      -   5        {�.  $��1��5Q   &   $��2��63   
���������6G   �� *Log+6q   �� 5S      -   5         ��Name$ btnChat��5S      $��Chat >>��6&   $��2��68   $��1��6H   $��50��6U   $��Right=�6`   6|   � 5T      -   5         �Name$ popChat��5T       � 	StaysOpen . $��Slide=�66   $��Right=�5U      ��.   J� RenderTransformOrigin$	 5,0&�65    � PlacementTarget 5T   M   ��  �� ElementName$ 
layoutGrid��.��5V      �� 
�������$��1q�61   $	��300��6E   $	��350��5W      
�������._�5X      � $��10q�6   	 5Y      �� 5Z      �� 5[      $��40 �6,    5]      �� 5^      �� 5_      $��60 �6/   .+�5b      �� $
��Auto=�6'   $
��Auto=�6L   $��2��6o   .��5c      �� {�.  ��6&   �� *	ChatLog+6?   5d      �� 5f      $��1��6;   $��20��6H   ��6"   �� *	ChatSay+6H   �� 5g      $
��Send��6!   $��20��6O   $��1��6[   $��1��6k   $��0q�6x   ��60   �� *
SayCommand+6x   5h      5i      5j      5s      5t      5u      