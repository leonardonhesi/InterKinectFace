using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fleck;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace InterKinectFace.Trasmitir
{
    public class Trasmitir
    {
        //DELEGATE PARA PASSAR PARAMETROS A FUNÇÂO ASSINCRONA asyncTransmite() 
        public delegate void delEnvia(List<Skeleton> usuario);
        public delegate void delPose(string nome);
        public delegate void delTela();
        
        private int contaQuadro = 0;

        static List<IWebSocketConnection> _sockets;

        static bool _initialized = false;
        //Obter o IP do sistema ou pela configuração
        public WebSocketServer server = new WebSocketServer(pegaIP());
       
        //OBTEM O IP LOCAL PARA INICIALIZAR SOCKET
        public static string pegaIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return "ws://"+localIP+":8181";
        }

        public Trasmitir()
        {
            //InitializeSockets();
        }

        public void InitializeSockets()
        {
            _sockets = new List<IWebSocketConnection>();

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    // Console.WriteLine("Conectado em " + socket.ConnectionInfo.ClientIpAddress);
                    _sockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    //Console.WriteLine("Desconectado de " + socket.ConnectionInfo.ClientIpAddress);
                    _sockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    //Console.WriteLine(message);
                };
            });

            _initialized = true;

            //Console.ReadLine();
        }

        public void FinalizarSocket()
        {

            server.Dispose();
        }

        public void enviaPose(string nome)
        {
            if (!_initialized) return;

            //CHAMADA ASSINCRONA PARA NÂO BLOQUEAR KINECT ENQUANTO TRANSMITE RECONHECIMENTO DA POSE
            delPose EnviaDelegate = new delPose(asyncPose);
            EnviaDelegate.BeginInvoke(nome, null, null);
            
        
        }

        public void transmitir(Skeleton Skeletons)
        {

            if (!_initialized) return;

            List<Skeleton> users = new List<Skeleton>();
            users.Add(Skeletons);

            //CHAMADA ASSINCRONA PARA NÂO BLOQUEAR KINECT ENQUANTO TRANSMITE A TELA E SKELETO
            delEnvia EnviaDelegate = new delEnvia(asyncTransmite);
            EnviaDelegate.BeginInvoke(users, null, null);

            
            //Realiza um controle para não mandar 30 imagens por segundo mas uma imagem a cada 1 segundo
            if (contaQuadro == 0)
            {

                delTela EnviaTela = new delTela(asyncTela);
                EnviaTela.BeginInvoke(null, null);
                contaQuadro += 30;
            }
            else
            {

                contaQuadro -= 1;
            }
        }


        //Realiza as transmissões assincronas dados do skeleto
        public void asyncTransmite(List<Skeleton> usuario)
        {
            if (usuario.Count > 0)
            {
                string json = usuario.Serialize();

                foreach (var socket in _sockets)
                {
                    socket.Send(json);

                }
            }


        }

        //Realiza as transmissões assincronas dados de Tela
        public void asyncTela()
        {

            printSerialize tela = new printSerialize();
            var blob = tela.CreateBlob();
            foreach (var socket in _sockets)
            {
                socket.Send(blob);
            }
        }

        //Realiza as transmissões assincronas poses
        public void asyncPose(string nome)
        {
            string json = poseSerialize.Seriall(nome);
            foreach (var socket in _sockets)
            {
                socket.Send(json);

            }
        
        }

    }


}

