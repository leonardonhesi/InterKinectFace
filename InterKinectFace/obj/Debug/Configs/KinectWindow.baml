   M S B A M L   `   `   `  ����O  KInterKinectFace, Version=1.2013.12.27, Culture=neutral, PublicKeyToken=nulla5clr-namespace:Microsoft.Samples.Kinect.KinectExplorer'Microsoft.Samples.Kinect.KinectExplorer  5   	   ^ ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers 5   	   :    4Microsoft.Samples.Kinect.KinectExplorer.KinectWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Glocal5clr-namespace:Microsoft.Samples.Kinect.KinectExplorer     5   	   mkt^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   \ XMicrosoft.Kinect.Toolkit.Controls, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullS OMicrosoft.Kinect.Toolkit, Version=1.7.0.0, Culture=neutral, PublicKeyToken=null2k(http://schemas.microsoft.com/kinect/2013   5   	     � Title$&   InterKinectFace - Configuração��5   	   $
��1200��60   $	��700��6=   $	��500��5   	   $	��700��5	   	   $��Segoe UI)�5
   	    � WindowStartupLocation$ CenterScreen=�6    � WindowStyle$ ThreeDBorderWindow=�6D    � 	Resources 5       �� MergedDictionaries �� SourceL   FMicrosoft.Samples.Kinect.KinectExplorer.KinectWindowsViewerSwapCommand   SwapCommand�� 5   
    5      �� 5      $f `pack://application:,,,/Microsoft.Samples.Kinect.WpfViewers;component/KinectControlResources.xaml?�6%   %   &	          5      63   5      .��5      � -   5         �Name$ 
layoutGrid��5      $��	10 0 10 0q�6     
KinectFont#��[ 62    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   $	��300��6'   �� 5      $
��Auto �6    � ColumnDefinitions 5    
   �� 5"      $	��400��6   �� 5$      $
��Auto �6   $	��314��6,   .+�5&   
   ]� $��0��6   $��2��6!   $��
10 0 10 20q�65   .+�5'      �� $
��Left=�6   $�� ��6*   $
��Fill=�64   $��32��6C   $��81��6O   $
��Left=�6Z   $��0 10 0 0q�6u   �� 5(      $��Right=�6   $��Right=�5)      $��Bottom=�5*        TitleForegroundBrush#��[ 5+        LabelFontSize#��[ 6K   �� 6v   $�InterKinectFace��6z   6�   �� 5,      $��Images\Status.png��6   $
��None=�6/   $��Center=�6>   5-      � 50   
   $��0��6   $��1��6   $��10,10,10,315q�6,   .+�51      � -   5        $ MainViewerHost��51      .+�52      � -   5        $ ColorVis��52        DarkNeutralBrush#��[ 6)   $��	0,0,146,0q�6X   .+�53      (� $��Center=�6   $��Center=�6;   	 (� Stretch$	 Uniform=�6V   .��55      � .+�56      ;  5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  -   5        
  Name$
 ColorViewer��56   3   6  0Microsoft.Samples.Kinect.WpfViewers.KinectViewer   CollectFrameRate .     RetainImageOnSensorChange . 7  1Microsoft.Samples.Kinect.WpfViewers.KinectControl   KinectSensorManager 6`   �� *KinectSensorManager+6�   �� 57      .+�58   "   >  8Microsoft.Samples.Kinect.WpfViewers.KinectSkeletonViewer     	ImageType$ Color=�5<   %   y�59   %   �� *(&KinectSensorManager.ColorStreamEnabled+  BoolToVisibilityConverter �� 	Converter# [ ��5:   %   �� *ActualWidth+ �� ElementName$ ColorViewer����5;   %   �� *ActualHeight+$ ColorViewer�� 5<   7   �� *KinectSensorManager+5=      5>      5?      �� 5@        LabelForegroundBrush#��[ 5A      $��Right=�5B      $	��Top=�65     MediumNeutralBrush#��[ 5C      $��50��5D      $��50��6$   $��0,-2,0,0q�60   ._�5E      ��  �� Orientation$ Vertical=�6%   .+�5F      ��   HeaderFontSize#��[ 6(   $��Center=�6�   $��-2q�6�   ��6S   �� *	FrameRate+$ ColorViewer��6�   �� 5G       	 FPSFontSize#��[	 6(   $��Center=�6P   $��-2q�6m   �� 6z   $	�FPS��6~   6�   5H      5I      �� 5J      ���#7777 J� SnapsToDevicePixels . $��0,-2,0,0q�6�   $	��700��6�   y�6-   �� *(&KinectSensorManager.ColorStreamEnabled+ �� ConverterParameter$
 True��# [ 6�   5K      5L      5M      �� 5P   
   $ Vertical=�5Q      $��1��5R      $��1��5S      $��1��5T      $��10,10,0,217q�5U      $
��Left=�5V      $	��210��6(   .+�5W      8  2Microsoft.Samples.Kinect.WpfViewers.KinectSettings  -   5        $��0 20 0 0q�5W        .    DepthTreatment 6n   �� *DepthTreatment+ �� Mode$ TwoWay=� 6�   �� *KinectSensorManager+�� 5Y      -   5         ��Name$ kinectSensors��5Y   .   
�������$��1��6E   �� 5Z      $��2��6   $��0 5 0 5q�6    	 �� X1$ 0��61   	 �� X2$ 1��68   #��[ 6?    �� Stretch$
 Fill=�6j    . 6y   �� 5[      $��3��6   #��[ 6&   #��[ 6U   #��[ 6   �� 6�   $�Status��6�   6�   �� 5\      $��4��6   $
��Auto=�6(   .��5]      �� -   5        $ kinectStatus��5]      5^      5`      �� 5a   
   $
��Left=�6   $	��264��60   $��
10,305,0,0q�6=   $��1��6Q   $	��Top=�6^   $	��508��6v   .+�5c      �� $	��198��6   $��0,0,33,0q�6&   .@�5d      �� $��Diretorio Apresentações��6   .��5e      � ���	#FFE5E5E5$��	0,1,-2,-3q�62    5f      �� 5g      $	��69* �6/   �� 5h      $
��143* �6/   �� 5i      $
��293* �6/   .+�5k      �� -   5         ��Name$ txtDiretorio��5k   "   $
��Left=�68   $��23��6S   $��	10,51,0,0q�6_    �� TextWrapping$
 Wrap=�6r   $�� ��6�   $	��Top=�6�   $	��278��6�   $��3��6�    � ToolTip$2 ,Diretorio onde ficam os arquivos Power Point��6�   �� 5l      -	   5         ��Name$ btnDiretorio��5l   !   $	��...��67   $
��Left=�6E   $��
227,51,0,0q�6`   $	��Top=�6t   $��41��6�    J� RenderTransformOrigin$ -0.16,-0.273&�6�   $��1��6�   $��2��6�   �� 5m      $��Selecione o Diretorio:��6    $
��Left=�6A   $��	10,10,0,0q�6\   $	��Top=�6o   $ 0.289,0&�6�   $��2��6�   $	��131��6�   �� 5o      -
   5         ��Name$ chkTrasmite��5o   #   $��Transmitir Tela��68   $
��Left=�6R   $��
10,106,0,0q�6m   $	��Top=�6�   $��2��6�   5q      5r      �� 5s      $��Comando de Voz��6   .��5t      � ���	#FFE5E5E5$��0,0,0,6q�62    5u      �� 5v      $	��61* �6/   �� 5w      $
��119* �6/   �� 5x      $	��23* �6/   �� 5y      $
��262* �6/   .+�5{      	� -   5          	�Name$  rdoSim��5{   &   $	��Sim��66   $
��Left=�6D   $��	20,45,0,0q�6_   $	��Top=�6r   $��19��6�   $��88��6�   $��2��6�   	� 5|      -   5        $  rdoNao��5|   &   $
��Não��66   $
��Left=�6D   $��	23,44,0,0q�6_   $	��Top=�6r   $��19��6�   $��88��6�   $��1��6�   �� 5}      $$��Selecione as Frases de comando��6    $
��Left=�6I   $��10,25,0,112q�6d   $��3��6y   $	��181��6�   �� 5      $��	Avançar:��6!   $
��Left=�64   $��	20,82,0,0q�6O   $	��Top=�6b   $��2��6z   $��26��6�   $��55��6�   �� 5�      -   5        ! ��Name$! 
cmbAvancar��5�   #   $
��Left=�67   $��	27,91,0,0q�6R   $	��Top=�6e   $	��120��6}   $- 'Palavra utilizada para avançar o slide��6�   $��1��6�   $��3��6�   $��22��6�   .@�5�      �� -   5        " ��Name$" avancarNext��5�   +   $
��Next��6@   �� 5�      -   5        $" 	avancarGo��5�   +   $��Go��6>   �� 5�      -   5        $" avancarShow��5�   +   $
��Show��6@   �� 5�      -   5        $" avancarMove��5�   +   $
��Move��6@   5�      �� 5�      $��Retroceder:��6!   $
��Left=�67   $��
20,113,0,0q�6R   $	��Top=�6f   $��2��6~   $��26��6�   $��71��6�   �� 5�      -   5        $! cmbRetroceder��5�   #   $
��Left=�6:   $��
27,118,0,0q�6U   $	��Top=�6i   $	��120��6�   $/ )Palavra utilizada para retroceder o slide��6�   $��1��6�   $��3��6�   $��22��6�   .@�5�      �� -   5        $" retrocederBack��5�   +   $
��Back��6C   �� 5�      -   5        $" retrocederBefore��5�   +   $��Before��6E   �� 5�      -   5        $" retrocederGoback��5�   +   $��Goback��6E   �� 5�      -   5        $" retrocederShowBack��5�   +   $��	Show Back��6G   5�      �� 5�      $��Fechar:��6    $
��Left=�62   $��	19,91,0,0q�6M   $	��Top=�6`   $��3��6x   $��26��6�   $��47��6�   �� 5�      -   5        $! 	cmbfechar��5�   #   $
��Left=�66   $��	88,95,0,0q�6Q   $	��Top=�6d   $	��120��6|   $4 .Palavra utilizada para fechar a apresentação��6�   $��3��6�   $��22��6�   .@�5�      �� -   5        $" fecharClose��5�   +   $��Close��6@   �� 5�      -   5        $" 
fecharExit��5�   +   $
��Exit��6?   �� 5�      -   5        $" 	fecharEnd��5�   +   $	��End��6>   �� 5�      -   5        $" fecharFinish��5�   +   $��finish��6A   5�      �� 5�      $��Habilita comandos de voz��6    $
��Left=�6C   $��	20,10,0,0q�6^   $	��Top=�6q   $ 0.316,-0.154&�6�   $��2��6�   $��26��6�   $	��147��6�   �� 5�      $��Comando:��6    $
��Left=�63   $��
19,122,0,0q�6N   $	��Top=�6b   $��3��6z   $��24��6�   $��64��6�   �� 5�      -   5        $! 
cmbComando��5�   #   $
��Left=�67   $��
88,122,0,0q�6R   $	��Top=�6f   $	��120��6~   $< 6Palavra de comando que precede todos os outro comandos��6�   $��3��6�   $��22��6�   .@�5�      �� -   5        $" comandoKinect��5�   +   $��Kinect��6B   �� 5�      -   5        $" comandoScreen��5�   +   $��Screen��6B   �� 5�      -   5        $" comandoPowerPoint��5�   +   $��
PowerPoint��6F   �� 5�      -    5        $" comandoHello��5�   +   $��Hello��6A   5�      5�      5�      5�      � 5�      5�      �� 5�   
   -!   5        $ 	btnGravar��5�      $��Gravar��6$   $
��Left=�65   $��359,513,0,0q�6P   $��1��6e   $	��Top=�6r   $	��121��6�   $��24��6�   6�   :  4Microsoft.Kinect.Toolkit.Controls.KinectCircleButton  5�      -"   5        $��Start��5�   #   $
��Left=�63   $��430,305,0,0q�6N   $��1��6c   $	��Top=�6p   $	��210��6�   $ -0.436,0.621&�6�   $	��283��6�   $��22(�6�   5�      5�      