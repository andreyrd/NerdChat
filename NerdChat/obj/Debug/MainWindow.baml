   M S B A M L   `   `   `  ����C  ?NerdChat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:NerdChatNerdChat  5   	       NerdChat.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:NerdChat     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��4 �6   �� 5      $��* �6   .+�5   
   R� -   5         R�Name$ channelTree��5      $��4,4,4,4q�5      $��0��5      $��8,8,0,8q�5      $��2��5      �� 5   
   $��1��5      $��0��5      $��2��5      $��4��5       �� ResizeDirection$ Columns=�5       �� ResizeBehavior$ PreviousAndNext=�5      
������ 5       �� 5"   
   -   5         ��Name$ chatBox��5#      $��0,8,8,2q�5$       �� TextWrapping$
 Wrap=�5%      $��0��5&      $��2��5'       �� 
IsReadOnly . $/��)Welcome to NerdChat, a crappy IRC client!��5)      �� 5*   
   -   5        $ messageField��5+      $��20��5,      $��1��5-      $��2��5.      $��0,2,8,8q�5/      $
 Wrap=�50      51      52      