
namespace InterKinectFace
{
    //NORMAIS DO SISTEMA
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    //RELACIONADOS AOS KINECT
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Controls;
    using Coding4Fun.Kinect.Wpf;
    using Kinect.Toolbox;
    using Microsoft.Speech.Recognition;
    using Microsoft.Speech.AudioFormat;

    //RELACIONADOS AO POWERPOINT e ENTRADA DE DADOS
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.PowerPoint;
    using Microsoft.WindowsAPICodePack.Shell;
    using System.Windows.Media.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Windows.Media;
    using System.Windows.Forms;
    using WindowsInput;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    //MOVIMENTOS E GESTOS
    using Auxiliar.Movimentos.Poses;
    using Auxiliar.Movimentos.Gestos;
    using Auxiliar.Movimentos.Gestos.Aceno;
    using Auxiliar.Movimentos;
    using Auxiliar.Basicos;
    using Microsoft.Samples.Kinect.KinectExplorer;
    using InterKinectFace.Configs;



    public partial class MainWindow
    {

        //######################### VARIAVEIS INICIO ###################################################################################################################################################

        //ARRAY DE SKELETOS UTILIZADO PARA IDENTIFICAR QUAL ESTA COM STATUS DE TRACKED 
        public Skeleton[] skeletons;

        //DELEGATE PARA PASSAR PARAMETROS A FUNÇÂO ASSINCRONA abrirPPT() 
        public delegate void delParametros(String botao);

        //BOTÔES DE ROLAMENTO
        public static readonly DependencyProperty PageLeftEnabledProperty = DependencyProperty.Register("PageLeftEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        public static readonly DependencyProperty PageRightEnabledProperty = DependencyProperty.Register("PageRightEnabled", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        private const double ScrollErrorMargin = 0.001;
        private const int PixelScrollByAmount = 20;

        //CONTROLE DO MOUSE DURANTE A APRESENTAÇÂO
        private const float ClickThreshold = 0.33f;
        private const float SkeletonMaxX = 0.60f;
        private const float SkeletonMaxY = 0.40f;

        comandoVoz ComandosVoz;

        //GERENCIADOR DO SENSOR KINECT
        private KinectSensorChooser sensorChooser;

        //HABILITA CONTROLE MOUSE
        private bool hblMouse = false;
        public bool isForwardGestureActive = false;
        public bool isBackGestureActive = false;


        //RASTREADOR MOVIMENTOS E GESTOS
        List<IRastreador> rastreadores;

        //CLASSE RESPONSAVEL PELAS CONFIGURAÇÔES
        private Configurar setup;

        //CLASSE RESPONSAVEL PELA TRANSMISSÃO DADOS E DA TELA
        public Trasmitir.Trasmitir sendDados = new Trasmitir.Trasmitir();


        //########################## FIM VARIAVEIS ################################################################################################################################################


        //####################### METODOS INICIO ##################################################################################################################################################

        //CONSTRUTOR DA CLASSE
        public MainWindow()
        {
            setup = new Configurar();
            if (setup.getConfigOk())
            {

                this.InitializeComponent();

                //RETIRA O MOUSE DA TELA QUANDO FECHA A APRESENTAÇÂO, EVITANDO QUE ATRAPALHE A INTERAÇÂO COM O WPF
                NativeMethods.SendMouseInput(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);
                inicializa();
                adicionarBotoes();
            }
            else
            {
                System.Windows.MessageBox.Show("Arquivo configuração não encontrado", "AVISO");
                this.Close();

            }
        }

        //INICIALIZADORES DO KINECT
        private void inicializa()
        {

            //SOMENTE INICIALIZA O SOCKET SE A OPÇÃO RANSMITIR ESTIVER HABILITADA
            if (setup.getStream() == "S")
            {
                sendDados.InitializeSockets();
            }

            // INICIALIZA O SENSOR CHOOSER E O UI
            this.sensorChooser = new KinectSensorChooser();

            //REGISTRA O METODO CHAMADO NO EVENTO "KinectChanged"
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            //ASSOCIA O SENSOR COM O UI
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;

            //INICIALIZA O SENSOR
            this.sensorChooser.Start();

            //INICIALIZA OS RASTREADORES DE MOVIMENTOS E GESTOS
            InicializarRastreadores();

            //InicializarFonteAudio();//VERIFICAR QUANDO NÂO TIVER O KINECT CONECTADO POS DA ERRO
            //verificar a inicialização

            //ASSOCIA O CHOOSER RECEM CRIADO AO KINECTREGION DO WPF
            var regionSensorBinding = new System.Windows.Data.Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        //#####################MOVIMENTOS E GESTOS ###############################################################################################################################################

        //INICIALIZAR O RASTREADOR DE MOVIMENTOS E GESTOS
        private void InicializarRastreadores()
        {

            rastreadores = new List<IRastreador>();

            //RASTREAR AS POSES EM T - HABILITA CONTROLE DO MOUSE
            Rastreador<PoseT> rastreadorPoseT = new Rastreador<PoseT>();
            rastreadorPoseT.MovimentoIdentificado += PoseTIdentificada;

            //RASTREAR OS GESTOS DE ACENO - PARA FECHAR APLIÇÂO
            Rastreador<Aceno> rastreadorAceno = new Rastreador<Aceno>();
            rastreadorAceno.MovimentoIdentificado += AcenoIndentificado;

            //RASTREAR A POSE DE AVANÇAR O SLIDE
            Rastreador<PoseAvanca> rastreadorPoseAvanca = new Rastreador<PoseAvanca>();
            rastreadorPoseAvanca.MovimentoIdentificado += AvancarSlide;

            //RASTREAR A POSE DE VOLTAR O SLIDE
            Rastreador<PoseVoltar> rastreadorPoseVoltar = new Rastreador<PoseVoltar>();
            rastreadorPoseVoltar.MovimentoIdentificado += VoltarSlide;

            //ADICIONA OS RASTREADORES A LISTA DE RASTREADORES
            rastreadores.Add(rastreadorPoseT);
            rastreadores.Add(rastreadorAceno);
            rastreadores.Add(rastreadorPoseAvanca);
            rastreadores.Add(rastreadorPoseVoltar);

        }

        //METODOS DE RECONHECIMENTO DE POSES E GESTOS
        //POSE AVANÇA RECONHECIDA AVANÇAR SLIDE
        private void AvancarSlide(object sender, EventArgs e)
        {
            //SOMENTE UTILIZA ESTE COMANDO SE  INTERAÇÂO COM O MOUSE NÂO ESTIVER ATIVO
            if (!hblMouse)
            {
                System.Windows.Forms.SendKeys.SendWait("{Right}");
                sendDados.enviaPose("AVANCA");
            }
        }

        //POSE VOLTAR RECONHECIDA VOLTAR SLIDE SLIDE
        private void VoltarSlide(object sender, EventArgs e)
        {
            if (!hblMouse)
            {
                System.Windows.Forms.SendKeys.SendWait("{Left}");
                sendDados.enviaPose("VOLTA");
            }
        }

        //ACENO IDENTIFICADO HABILITA O CONTROLE DO MOUSE
        private void AcenoIndentificado(object sender, EventArgs e)
        {
            hblMouse = !hblMouse;
        }

        //POSE T IDENTIFICADA, FINALIZA A APLICAÇÂO
        private void PoseTIdentificada(object sender, EventArgs e)
        {
            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F4);

            //FINALIZAR OS RASTREADORES
        }


        //########################################################################################################################################################################################

        //CHAMADA PARA TRATAR O FOCO NO POWERPOINT AO ABRIR
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        public void focoPPT()
        {
            IntPtr splashwindow = FindWindowByCaption(IntPtr.Zero, "Apresentação de slides do PowerPoint - [Apresentação1]");
            SetForegroundWindow(splashwindow);
        }

        //########################################################################################################################################################################################

        //ADICIONA OS BOTÔES DE ACORDO COM OS ARQUIVOS PPT NO DIRETORIO
        public void adicionarBotoes()
        {

            // LIMPA O PAINEL 
            this.wrapPanel.Children.Clear();

            //OBTEM AS INFORMAÇÔES DO DIRETORIO APRESENTAR
            DirectoryInfo dirInfo = new DirectoryInfo(setup.getDiretorio());
            FileInfo[] fileNames = dirInfo.GetFiles("*.*"); //Melhoria apenas arquivos de apresentação e caso use BD função extrair as apresentações 

            //PERCORRE OS ARQUIVOS NO DIRETORIO
            foreach (FileInfo fi in fileNames)
            {
                //PARA CADA ARQUIVO OBTEM O THUMBNAI DO METODO "GetThumbnai" CRIA UM BOTÂO ONDE O LABEL É O NOME DO ARQUIVO"
                var button = new KinectTileButton { Label = fi.Name, Background = new ImageBrush(GetThumbnail(fi.FullName)) }; // Melhorias tirar extenção do nome do arquivo
                //ADICIONA AO PAINEL
                this.wrapPanel.Children.Add(button);

            }

            // ADICIONAR OS EVENTOS DE SCROLLVIEW
            this.UpdatePagingButtonState();
            scrollViewer.ScrollChanged += (o, e) => this.UpdatePagingButtonState();
        }

        //OBTEM THUMBNAIL DOS ARQUIVOS PPT
        private BitmapImage GetThumbnail(string filePath)
        {

            ShellFile shellFile = ShellFile.FromFilePath(filePath);
            BitmapSource shellThumb = shellFile.Thumbnail.ExtraLargeBitmapSource;
            BitmapImage bImg = new BitmapImage();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            var memoryStream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(shellThumb));
            encoder.Save(memoryStream);
            bImg.BeginInit();
            bImg.StreamSource = memoryStream;
            bImg.EndInit();
            return bImg;
        }

        //METODO QUE TRATA OS FRAMES DO SKELETO "30 POR SEGUNDO" ENVIADOS PELO EVENTO "SkeletonFrameReady"
        void sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return;
                }

                Skeleton[] allSkeletons = new Skeleton[skeletonFrameData.SkeletonArrayLength];
                
                //ADICIONEI PARA RASTREAR SOMENTE UM ESQUELETO
                //Skeleton sd;
                
                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //OBTEM SOMENTE UM ESQUELETO
                //sd = (from s in allSkeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();
                
                foreach (Skeleton sd in allSkeletons) 
                {

                    // O PRIMEIRO SKELETO COM O ESTADO TRACKED MOVE O CURSOR DO MOUSE
                    if (sd.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        //VERIFICA SE AS JUNTAS DAS MÃOS ESTÃO EM ESTADO TRACKED
                        if (sd.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked && sd.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                        {
                            //SUBMETE O ESQUELETO RASTREADO AO METODO RASTREAR MOVIMENTOS E GESTOS
                            foreach (IRastreador rastreador in rastreadores)
                            {
                                rastreador.Rastrear(sd);
                            }


                            //VERIFICA SE OPÇÂO DE CONTROLE DO MOUSE ESTA HABILITADA
                            if (hblMouse)
                            {
                                int cursorX, cursorY;

                                //OBTEM AS JUNTAS DA MÃOS
                                Joint jointRight = sd.Joints[JointType.HandRight];
                                Joint jointLeft = sd.Joints[JointType.HandLeft];

                                // APLICA SCALA A POSIÇÃO DAS JUNTAS DE ACORDO COM A TELA PRINCIPAL (MONITOR)
                                Joint scaledRight = jointRight.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);
                                Joint scaledLeft = jointLeft.ScaleTo((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, SkeletonMaxX, SkeletonMaxY);

                                // Melhoria canhoto ou destro
                                //if (LeftHand.IsChecked.GetValueOrDefault())
                                //{
                                //    cursorX = (int)scaledLeft.Position.X;
                                //    cursorY = (int)scaledLeft.Position.Y;
                                //}
                                //else
                                //{
                                cursorX = (int)scaledRight.Position.X;
                                cursorY = (int)scaledRight.Position.Y;
                                //}

                                bool leftClick;

                                // Verificar o click  da outra mão conjunto com melhoria de canhoto e destro
                                //if ((LeftHand.IsChecked.GetValueOrDefault() && jointRight.Position.Y > ClickThreshold) ||(!LeftHand.IsChecked.GetValueOrDefault() && jointLeft.Position.Y > ClickThreshold))

                                if (jointLeft.Position.Y > ClickThreshold)
                                {
                                    leftClick = true;
                                }
                                else
                                {
                                    leftClick = false;
                                }

                                //ENVIA AS INFORMAÇÕES PARA A CLASSE RESPONSAVEL PELO CLIQUE DO MOUSE
                                NativeMethods.SendMouseInput(cursorX, cursorY, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, leftClick);
                            }


                            //TRASMITIR A TELA CASO OPÇÂO ESTEJA MARCADA
                            if (setup.getStream() == "S")
                            {
                                sendDados.transmitir(sd);
                            }
                            return;
                        }
                    }
                } 
            }


        }

        //TRATAMENTO PARA QUANDO MUDAR O STATUS DO SENSOR KINECT
        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            // O SENSOR TORNOU-SE VELHO DESCONECTADO POR QUALQUER MOTIVO
            if (args.OldSensor != null)
            {
                try
                {
                    //DESABILITA TODOS OS FLUXOS
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {

                }
            }

            //O SENSOR È NOVO ACABA DE SER CONECTADO
            if (args.NewSensor != null)
            {
                try
                {
                    //HABILITA OS FLUXOS
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }

                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {

                }
            }
        }

        //CLIQUE DO BOTÂO AO SELECIONAR A APRESENTAÇÂO
        private void KinectTileButtonClick(object sender, RoutedEventArgs e)
        {

            //FINALIZA PROCESSO PREEXISTENTE DO POWER POINT
            Process[] pros = Process.GetProcesses();
            for (int i = 0; i < pros.Length; i++)
            {
                if (pros[i].ProcessName.ToLower().Contains("powerpnt"))
                {
                    pros[i].Kill();
                }
            }

            if (this.sensorChooser.Status == ChooserStatus.SensorStarted)
            {

                //OBTEM PROPIEDADES DO BOTÂO QUE FOI PRECIONADO
                var button = (KinectTileButton)e.OriginalSource;

                //CHAMADA ASSINCRONA PARA NÂO BLOQUEAR A EXECUÇÂO PRINCIPAL COM OS EVENTOS DO KINECT ENQUANTO ABERTA A APRESENTAÇÂO
                delParametros PPTDelegate = new delParametros(abrirPPT);
                PPTDelegate.BeginInvoke(Convert.ToString(button.Label), null, null);

                e.Handled = true;
            }
            else
            {
                System.Windows.MessageBox.Show("O Sensor Kinect não está devidamente conectado", "ERRO");
            }
        }

        //ABRE O ARQUIVO DE APRESENTAÇÂO RECEBE O LABEL DO BOTÂO QUE NO CASO È O NOME DO ARQUIVO
        private void abrirPPT(String botao)
        {

            try
            {
                //REGISTRA O METODO NO EVENTO FRAMEREADY DO SKELETO
                registrarEvento();

                //INTEROPS DO POWERPOINT PARA INICIAR UMA APRESENTAÇÂO
                String nomeArquivo = setup.getDiretorio() + "\\" + botao;

                Microsoft.Office.Interop.PowerPoint.Application ppApp = new Microsoft.Office.Interop.PowerPoint.Application();
                ppApp.Visible = MsoTriState.msoTrue;
                Presentations ppPresens = ppApp.Presentations;
                Presentation objPres = ppPresens.Open(nomeArquivo, MsoTriState.msoFalse, MsoTriState.msoTrue, MsoTriState.msoTrue);

                Slides objSlides = objPres.Slides;

                Microsoft.Office.Interop.PowerPoint.SlideShowWindows objSSWs;
                Microsoft.Office.Interop.PowerPoint.SlideShowSettings objSSS;

                //EXECUTA O SLIDE SHOW
                objSSS = objPres.SlideShowSettings;
                objSSS.Run();
                objSSWs = ppApp.SlideShowWindows;

                //DETERMINA O FOCO PARA A APRESENTAÇÂO
                focoPPT();

                //ENQUANTO TIVERMOS SLIDES PARA APRESENTAR
                while (objSSWs.Count >= 1)
                {
                    System.Threading.Thread.Sleep(100);
                }

                //FECHA A APRESENTAÇÂO REALIZANDO AS LIBERAÇÔES DE MEMORIA
                objPres.Close();
                Marshal.FinalReleaseComObject(objPres);
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //ROTINAS DE FECHAMENTO
                ppApp.Quit();
                Marshal.FinalReleaseComObject(ppApp);
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //ELIMINA O PROCESSO
                Process[] pros = Process.GetProcesses();
                for (int i = 0; i < pros.Length; i++)
                {
                    if (pros[i].ProcessName.ToLower().Contains("powerpnt"))
                    {
                        pros[i].Kill();
                    }
                }

                //DESREGISTRA O EVENTO PARA O MOUSE NÂO ATRAPALAHAR WPF PRINCIPAL
                desregistrarEvento();

                //RETIRA O MOUSE DA TELA QUANDO FECHA A APRESENTAÇÂO, EVITANDO QUE ATRAPALHE A INTERAÇÂO COM O WPF
                NativeMethods.SendMouseInput(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);

                //DESATIVA A INTERAÇÂO DO MOUSE EVITANDO QUE COMEÇE ATIVO NO PROXIMO SLIDE
                hblMouse = false;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro: " + ex.Message);
                //ELIMINA O PROCESSO
                Process[] pros = Process.GetProcesses();
                for (int i = 0; i < pros.Length; i++)
                {
                    if (pros[i].ProcessName.ToLower().Contains("powerpnt"))
                    {
                        pros[i].Kill();
                    }
                }

                //DESREGISTRA O EVENTO PARA O MOUSE NÂO ATRAPALAHAR WPF PRINCIPAL
                desregistrarEvento();

                //RETIRA O MOUSE DA TELA QUANDO FECHA A APRESENTAÇÂO, EVITANDO QUE ATRAPALHE A INTERAÇÂO COM O WPF
                NativeMethods.SendMouseInput(0, 0, (int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, false);

                //DESATIVA A INTERAÇÂO DO MOUSE EVITANDO QUE COMEÇE ATIVO NO PROXIMO SLIDE
                hblMouse = false;

            }

        }


        //################   RELACIONADOS AOS SCROLLS DOS BOTÔES #####################################################

        //TRATAMENTO DOS BOTÔES KinectHouverButton
        public bool PageLeftEnabled
        {
            get
            {
                return (bool)GetValue(PageLeftEnabledProperty);
            }

            set
            {
                this.SetValue(PageLeftEnabledProperty, value);
            }
        }
        public bool PageRightEnabled
        {
            get
            {
                return (bool)GetValue(PageRightEnabledProperty);
            }

            set
            {
                this.SetValue(PageRightEnabledProperty, value);
            }
        }
        private void PageRightButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + PixelScrollByAmount);
        }
        private void PageLeftButtonClick(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - PixelScrollByAmount);
        }
        private void UpdatePagingButtonState()
        {
            this.PageLeftEnabled = scrollViewer.HorizontalOffset > ScrollErrorMargin;
            this.PageRightEnabled = scrollViewer.HorizontalOffset < scrollViewer.ScrollableWidth - ScrollErrorMargin;
        }

        //############################################################################################################

        //FECHAMENTO DA JANELA FINALIZA O CHOOSER
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //FINALIZA O SOCKET CASO ESTEJA INICIALIZADO
            if (setup.getStream() == "S")
            {
                sendDados.FinalizarSocket();
            }

            //SOMENTE FINALIZAR SENSOR SE ESTIVER CONECTADO
            if (this.sensorChooser.Status == ChooserStatus.SensorStarted)
            {
                this.sensorChooser.Kinect.Stop();
                //DESREGISTRA O METODO CHAMADO NO EVENTO "KinectChanged"
                this.sensorChooser.KinectChanged -= SensorChooserOnKinectChanged;
                this.sensorChooserUi.KinectSensorChooser.Stop();
                this.sensorChooser.Stop();

                //this.ComandosVoz.Stop(); VERIFICAR QUANDO NÂO TIVER O KINECT CONECTADO POS DA ERRO
                KinectWindow postBack = new KinectWindow();
                postBack.Show();
            }
        }

        //REGISTRAR E DESREGISTRAR O EVENTO SkeletonFrameReady PARA A FUNÇAO sensor_SkeletonFrameReady
        private void registrarEvento()
        {
            this.sensorChooser.Kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(sensor_SkeletonFrameReady);
        }
        private void desregistrarEvento()
        {
            this.sensorChooser.Kinect.SkeletonFrameReady -= sensor_SkeletonFrameReady;

        }

        //#### INICIO VOZ ####
        void ComandosVoz_OrderDetected(RecognitionResult resultado, bool wordKey)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                if (wordKey)
                {
                    this.sensorChooserUi.IsListening = true;
                }
                else if (!wordKey)
                {

                    this.sensorChooserUi.IsListening = false;

                }

                if (this.sensorChooserUi.IsListening)
                {

                    string comando = resultado.Text;

                    switch (comando)
                    {

                        case "mouse":
                            hblMouse = true;
                            break;
                        case "stop":
                            if (hblMouse == true)
                            {
                                hblMouse = false;
                            }
                            break;
                        case "close":
                            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F4);
                            break;
                        /* 
                        case "next":
                            InputSimulator.SimulateKeyDown(VirtualKeyCode.RIGHT);
                            break;
                        case "back":
                            InputSimulator.SimulateKeyDown(VirtualKeyCode.LEFT);
                            break;
                        case "start":
                            InputSimulator.SimulateKeyDown(VirtualKeyCode.F5);
                            break;
                        case "close":
                            InputSimulator.SimulateModifiedKeyStroke(VirtualKeyCode.MENU, VirtualKeyCode.F4);
                            break;
                        */
                    }

                }
            }

            ));


        }

        private void InicializarFonteAudio()
        {
            //ComandosVoz = new comandoVoz("next", "back", "start", "close");
            ComandosVoz = new comandoVoz("mouse", "stop", "close");

            ComandosVoz.OrderDetected += ComandosVoz_OrderDetected;

            ComandosVoz.Start(this.sensorChooser.Kinect);

        }

        //#### FIM VOZ ####

        //########################### METODOS FIM #######################################################################################

    }
}