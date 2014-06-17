//------------------------------------------------------------------------------
// <copyright file="KinectWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.KinectExplorer
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Microsoft.Kinect;
    using Microsoft.Samples.Kinect.WpfViewers;
    using System.Collections.ObjectModel;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Controls;
    using System;
    using InterKinectFace;
    using System.IO;
    using System.Text;
    using InterKinectFace.Configs;


    public partial class KinectWindow : Window
    {
        //Classe com as configurações
        Configurar config;

        public static readonly DependencyProperty KinectSensorProperty =
            DependencyProperty.Register(
                "KinectSensor",
                typeof(KinectSensor),
                typeof(KinectWindow),
                new PropertyMetadata(null));

        private readonly KinectWindowViewModel viewModel;

        public KinectWindow()
        {

            try
            {
                this.viewModel = new KinectWindowViewModel();

                this.viewModel.KinectSensorManager = new KinectSensorManager();

                Binding sensorBinding = new Binding("KinectSensor");
                sensorBinding.Source = this;
                BindingOperations.SetBinding(this.viewModel.KinectSensorManager, KinectSensorManager.KinectSensorProperty, sensorBinding);

                this.viewModel.KinectSensorManager.SkeletonStreamEnabled = true;
                this.DataContext = this.viewModel;

                foreach (KinectSensor kinect in KinectSensor.KinectSensors)
                {
                    this.KinectSensor = kinect;
                    this.KinectSensor.Start();
                }

                InitializeComponent();

                //Caso exista o arquivo de configuração preenche a tela com estas configurações
                config = new Configurar();
                if (config.getConfigOk())
                {
                    this.txtDiretorio.Text = config.getDiretorio();

                    if (config.getStream() == "S")
                    {
                        this.chkTrasmite.IsChecked = true;

                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro :" + ex.Message);
            }
        }

        public KinectSensor KinectSensor
        {
            get { return (KinectSensor)GetValue(KinectSensorProperty); }
            set { SetValue(KinectSensorProperty, value); }
        }

        public void StatusChanged(KinectStatus status)
        {
            this.viewModel.KinectSensorManager.KinectSensorStatus = status;
        }

        private void KinectSettings_Loaded_1(object sender, RoutedEventArgs e)
        {

        }


        //Botão para mostrar a caixa de dialogo para que seja selecionado o diretorio das apresentações PPT
        private void btnDiretorio_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            txtDiretorio.Text = dialog.SelectedPath;
        }

        //Finalizar e gravar o arquivo de configuração
        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {

            //txtDiretorio

            //Apagar anterior se existir

            if (File.Exists("config.xml"))
            {
                File.Delete("config.xml");
            }

            if (txtDiretorio.Text != "")
            {
                //Verifica se o diretorio seleciona existe
                if (Directory.Exists(txtDiretorio.Text))
                {

                    StreamWriter gravar = new StreamWriter("config.xml", true, Encoding.ASCII);

                    String stream = "N";
                    if (chkTrasmite.IsChecked == true)
                    {
                        stream = "S";
                    }

                    gravar.WriteLine("<CONFIG>");
                    gravar.WriteLine("<add Diretorio=\"" + txtDiretorio.Text + "\"/>");
                    gravar.WriteLine("<add Transmite=\"" + stream + "\"/>");
                    gravar.WriteLine("</CONFIG>");
                    gravar.Close();
                    MessageBox.Show("Arquivo salvo com sucesso!", "AVISO");
                }

                else
                {
                    //Erro se o diretorio não existir
                    MessageBox.Show("Diretório Inválido verifique!", "AVISO");

                }
            }
            else
            {
                //Erro caso textbox estiver vazio
                MessageBox.Show("Digite um diretorio para salvar as Tranferências", "AVISO");

            }


        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {


        }

        //Inicializa a apresentação
        private void KinectCircleButton_Click_1(object sender, RoutedEventArgs e)
        {

            //Antes verifica o arquivo de configuração
            if (config.getConfigOk())
            {
                try
                {
                    //Finaliza o sensor desta janela
                    if (this.KinectSensor.Status == KinectStatus.Connected)
                    {
                        this.KinectSensor.Stop();
                    }

                    //Passa o sensor desta janela para a proxima que devera ser inicializado pelo outra janela ao fechar
                    MainWindow apresentar = new MainWindow();
                    apresentar.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro:" + ex.Message + " o sensor esta conectado?","ERRO");

                }
            }
            else
            {
                MessageBox.Show("Falta gravar o arquivo de configuração", "ERRO");
            }
        }


    }

    public class KinectWindowViewModel : DependencyObject
    {
        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DepthTreatmentProperty =
            DependencyProperty.Register(
                "DepthTreatment",
                typeof(KinectDepthTreatment),
                typeof(KinectWindowViewModel),
                new PropertyMetadata(KinectDepthTreatment.ClampUnreliableDepths));

        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        public KinectDepthTreatment DepthTreatment
        {
            get { return (KinectDepthTreatment)GetValue(DepthTreatmentProperty); }
            set { SetValue(DepthTreatmentProperty, value); }
        }
    }


    public class KinectWindowsViewerSwapCommand : RoutedCommand
    {
    }
}
