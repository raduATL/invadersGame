   M S B A M L   `   `   `  ����C  ?Invaders, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:Invaders.ViewInvaders.View  5   	   7 clr-namespace:Invaders.ViewModelInvaders.ViewModel  5   	        Invaders.View.InvadersPage   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:Invaders.View     5   	   6	viewmodel clr-namespace:Invaders.ViewModel     5   	   -   5        $��window��5	        � Title$  Invaders��6	   $	��800��6(   $	��700��64    � 	Resources 5
      *  $Invaders.ViewModel.InvadersViewModel   	viewmodel3  -Invaders.ViewModel.BooleanVisibilityConverter  visibilityConverter��%:   &	        &	        5   
   6&     5   
   6/   5      .��5      �  � DataContext# [  6    � RowDefinitions 5   
   �� 5      $��80 �6   �� 5      $��* �6   .+�5   
   �� $��Right=�6   $��	0,5,120,0q�61   .+�5      �� $��Right=�6/   $��0,0,15,0q�6K   ��6   �� *Score+6K   �� 5      $��Right=�6/   $
��Auto��6K   $��20��6X   ��6   �� *Lives+�� 5      $��
player.png��6   $
��Fill=�6(   $��15��67   $��20��6C   5      �� 5      -   5         ��Name$ startButton��5      $��Start!��5      $��Center=�5      $��Center=�5      5      �� 5   
   -   5         ��Name$ playArea��5      
����  �$��2q�5   $    �� CornerRadius$ 10��68   $��5q�6J   
���   �$��1��5      ._�5       �� $��5q�6@   ��6   ��  �� Path$ Sprites���5!      �� 5"      .R�5#      �� 
���   �6!   5$      5&      5'      �� 5(   
   $��1��6   $��Center=�6"   $��Center=�6?   y�5)      �� *
GameOver+ �� 	Converter# [ .+�5+      �� $��1��6   $��	Game Over��6%   $��Center=�66   $��Arial Narrow)�5,      $	��150(�62   
��� ���$��	ExtraBold"�5-   ,   $��Italic$�6C   $��0,0,0,60q�5.      �� 50      -   5        $��Start a new game��50      
���   �$��Center=�51      $��30(�62   6@   52      �� 54   
   $��1��6   $��Paused��6!   $��Center=�6/   $��Arial Narrow)�55      $��60(�6.   
��� ���$��	ExtraBold"�56   (   $��Italic$�6?   $��
120,0,0,60q�57      y�58      �� *Paused+# [ r� 5:   
   -   5        	 r�Name$	 	backmedia��5:      
 r� Source$
 
Techno.mp3?�5;       r� LoadedBehavior$ Manual=�6#    r� UnloadedBehavior$
 Stop=�6;   $��40��5=      r� 5>   
   -   5        $	 media��5>      $
 Gunshots.wav?�5?      $ Manual=�6%   $
 Stop=�6=   $��40��5A      r� 5B   
   -   5        $	 blastedPlayerMedia��5B      $
 tickingbomb.wav?�5C      $ Manual=�6(   $
 Stop=�6@   $��40��5E      r� 5F   
   -   5        $	 blastedInvaderMedia��5F      $
 explosion1.wav?�5G      $ Manual=�6'   $
 Stop=�6?   $��40��5I      5J      5K      