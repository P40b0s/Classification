   M S B A M L   `   `   `  ����  Classification,    &Classification.Shell.Views.StartWindow   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   P LPrism.Wpf, Version=6.2.0.0, Culture=neutral, PublicKeyToken=91a96d2a154366d8$prismhttp://prismlibrary.com/  5   	   J FMahApps.Metro, Version=1.3.0.150, Culture=neutral, PublicKeyToken=null8metro,http://metro.mahapps.com/winfx/xaml/controls  5   	     � WindowStartupLocation$  CenterScreen=�5   	   $
��1000��5   	   $
��1600��6   .��5      �  � RowDefinitions 5	   
   �� 5
      $	��200 �6   �� 5      $	��800 �6    � ColumnDefinitions 5   
   �� 5      $
��1250 �6   �� 5      $
��600* �6   .+�5   
   �� !  Prism.Regions.RegionManager   
RegionName$ SearchModuleRegion��6   $��0��6M   $��0��6Z   �� 5   
   $ MainListViewModuleRegion��6   $��1��6S   $��0��6`   �� 5   
   $* $ModuleDocumentTextViewerModuleRegion��6   $��2��6_   $��1��6q   5      5      