   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=nullc _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8  �� MergedDictionaries �� Source   TrueKey  FalseKey  UsualyListViewHeader �� 
TargetType �� Property �� Value �� 
TargetType �� TextAlignment  KeywordsHeader �� Orientation  RubricsHeader  
NameHeader � RowDefinitions	 � ToolTip
 �� RelativeSource �� Mode �� AncestorType �� Path  
RobotImage d� 	IsChecked �� Mode  OkButton � ToolTip �� UpdateSourceTrigger�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      8metro,http://metro.mahapps.com/winfx/xaml/controls  5      Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      1system&clr-namespace:System;assembly=mscorlib  5      $prismhttp://prismlibrary.com/  5        5
      �� 5   
   $N Hpack://application:,,,/Classification.Core;component/Styles/Buttons.xaml?�6   �� 5   
   $M Gpack://application:,,,/Classification.Core;component/Images/Images.xaml?�6   %�  &	        &	      &	 9     &	 �    &	 p    &	 O    7  7  ��5      True��6+   ��5      	False��6-   �� 5      " ��6)   .�5   
   ��  � ��$ 18(�6%   �� 5   
    � �� 5      �� 5      " ��6"   .R�5      �� $��1q�6   ���	#FF8F8F8F
�������._�5      �� $ Center=�6+   $��Center=�6L   $��Center=�6i   5      5      5      �� 5   
    �@�OverridesDefaultStyle . 62   5      �� 5       " ��6#   .�5!   
   ��  � ��$ 18(�6%   �� 5"   
    � �� 5#      �� 5$      " ��6"   .R�5%      �� $��1q�6   ���	#FF8F8F8F
�������._�5&      �� $ 
Horizontal=�6%   $��Center=�6?   $��Center=�6\   .+�5'      �� $ Center=�6/   �� 5(      $��3 0 0 0q�6O   
���  ����6(   �� *NotReadyKeywordsCount+6`   5)      5*      5+      5-      �� 5.   
    �@�OverridesDefaultStyle . 62   5/      �� 51      " ��6"   .�52   
   ��  � ��$ 18(�6%   �� 53   
    � �� 54      �� 55      " ��6"   .R�56      �� $��1q�6   ���	#FF8F8F8F
�������._�57      �� $ 
Horizontal=�6%   $��Center=�6?   $��Center=�6\   .+�58      �� $ Center=�6/   �� 59      $��3 0 0 0q�6N   
���  ����6(   �� *NotReadyRubricsCount+6_   5:      5;      5<      5>      �� 5?   
    �@�OverridesDefaultStyle . 62   5@      �� 5C      " ��6   .�5D   
   ��  � ��$ 18(�6%   �� 5E   
    � �� 5F      �� 5G      " ��6"   .R�5H      �� $��1q�6   ���	#FF8F8F8F
�������._�5I      �  5J      .+�5M      �� $?	 9Количество документов в списке��6)   $
��Left=�5O   &   ��5N   &   �� 
 �� $ FindAncestor=�" ��$ Items.Count��� 5P   "   8��  $��30��6M   5Q      �� 5R      $)��#Показывать готовые��6'   $��Right=�6�    6D   �� *ShowReadyDocumentsInListView+$ TwoWay=�6�   �� 5T      $��Center=�5V   %   $��Center=�5W   %   8�� ��5U   %   �� 
 �� $ FindAncestor=�" ��$3 -DataContext.OpenSerializationInterfaceCommand�.��5Y   "   �� $��Center=�6,   $��Center=�5Z   (   $��18(�5[   (   
���   ��� 5_   "   
���  ��$( "Документы в работе��5a   &   �5_   &   �� *InWorkDocumentsCount+$ OneWay=�$ PropertyChanged=�5a   &    �� 5b   "   $�/��6&    �� 5c   "   
��� ���$D >Документы готовые к выгрузке в Excel��5e   &   �5c   &   �� *ReadyItemsCount+$ OneWay=�$ PropertyChanged=�.�5e   U   5f   #    �� 5g   "   $�/��6&    �� 5h   "   ���	#FF2BD11A$8 2Документы выгруженные в Excel��5j   &   �5h   &   �� *ExportedDocumentsCount+$ OneWay=�$ PropertyChanged=�5j   &   5m   #   5n      5o      5p      5q      5s      �� 5t   
    �@�OverridesDefaultStyle . 62   5u      5w      