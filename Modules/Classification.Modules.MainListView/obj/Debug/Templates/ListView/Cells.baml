   M S B A M L   `   `   `  ����M  IMainListViewModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null{Bclr-namespace:Classification.Modules.MainListViewModule.ViewModels4Classification.Modules.MainListViewModule.ViewModels  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullc _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� Source   TrueKey  FalseKey  ActNameCellTemplate � ColumnDefinitions ��Name �� RelativeSource �� AncestorType �� Path  	SaveImage �� 
TargetType  OkButton �� BasedOn	 �� Property
 �� Value �� TextAlignment �� Triggers �� Value �� Binding �� 
TargetName  NumberCellTemplate  SignDateCellTemplate �� StringFormat  KeysCellTemplate  CancelButton 	 SimplyCancelImage ��Name 
 RubricsCellTemplate�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      8metro,http://metro.mahapps.com/winfx/xaml/controls  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      LlocalBclr-namespace:Classification.Modules.MainListViewModule.ViewModels  5      1system&clr-namespace:System;assembly=mscorlib  5      $prismhttp://prismlibrary.com/  5        5      �� 5   
   $M Gpack://application:,,,/Classification.Core;component/Images/Images.xaml?�6   �� 5   
   $N Hpack://application:,,,/Classification.Core;component/Styles/Buttons.xaml?�6   %|  &	        &	      &	 9     7  7  &	     &	 �    &	 L    7  7 	 &	
 1    7  7 	 ��5      True��6+   ��5      	False��6-   �� 5      .R�5   
   �  5      �� 5      $��50 �6#   �� 5      $	��350 �6#   .+�5      �� $ 
saveButton��6   $��0��5      ��5      ��  �� " ��$% DataContext.SaveDocumentCommand�.��5      � .+�5      �� 8��  $��Center=�5      $��50��5      $��Center=�5       58      ��5N      �� 5O      " ��6   8  .�5P      �� 	 � {�
 .  66   5Q      5S      �� 5U      $
��Wrap=�6   $
��Left=�5V      $
 Left=�5W      $��1��5X      $��16(�5Y      �� 5Z      
���  ���6   �� *
ItemNumber+6.    �� 5[      $�. ��6    �� 5\      �6   �� *	ActName+5]      5^       5_   
   �� 5`      $ false��6�    6   �� $ 
IsSelected� �� " ��.a�5a      �� $ 
saveButton��6/   	 � y�$
 Hidden=�6G   5b      �� 5c      $
 true��6>    6   �� $ 
IsNotSaved�.a�5d      �� $ 
saveButton��6.   	 � {�
 . 6F   5e      5g      �� 5i      .R�5j   
   �� $��Center=�5k      $ Center=�5l      $��16(�5m      ��5j      �� *	ActNumber+5m      5n      �� 5p      .R�5q   
   �� $ Center=�5r      $��Center=�5s      $��16(�5t      ��5q      �� *
SignDate+$ {0:dd.MM.yyyy}��5t      5u      �� 5w      .R�5x   
   �� $
��Left=�5y      
������ $��16(�5{      ��5x      �� *Keys+��5|      �� 5}      .R�5~      �  5      �� 5�      $��30 �6/   �� 5�      $
��auto �6/   .+�5�      �� $��0��6!   8��  ��5�   !   ��  �� " ��$" DataContext.RemoveKeyCommand���5�   !   �� *ItemId+.��5�      �� $��30��6$   ��5�   "   �� 5�   &   " ��6,   .�5�   *   �� 	 � ��8
  6C   5�   '   5�      5�      �� 5�      $
 item��6^   $��1��6:   $
��Wrap=�6J   ��6$   �� *Item+6^   5�      5�      5�       5�   
   �� 5�      $ false��6�    6   �� $ 
IsSelected� �� " ��.a�5�      �� 	 � {�
 .  6.   5�      5�      �� 5�      .R�5�   
   �� $
��Left=�5�      
������ $	��345��5�      $��16(�5�      ��5�      �� *	Rubrics+��5�      �� 5�      .R�5�      �  5�      �� 5�      $��30 �6/   �� 5�      $	��300 �6/   .+�5�      �� $��0��6!   8��  ��5�   !   ��  �� " ��$% DataContext.RemoveRubricCommand���5�   !   �� *ItemId+.��5�      �� $��30��6$   ��5�   "   �� 5�   &   " ��6,   .�5�   *   �� 	 � ��8
  6C   5�   '   5�      5�      �� 5�      $��1��6:   $
��Wrap=�6J   ��6$   �� *Item+6J   5�      5�      5�       5�   
   �� 5�      $ false��6�    6   �� $ 
IsSelected� �� " ��.a�5�      �� 	 � {�
 .  6.   5�      5�      5�      