using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Collections.Generic;
using System.Text;


namespace InterKinectFace
{
    public class comandoVoz
    {

        public SpeechRecognitionEngine speechRecognitionEngine;

        private KinectSensor kinectSensor;
        
        private String[] pComandos;
        
        private bool palavraChave = false;
        
        public event Action<RecognitionResult, bool> OrderDetected;
        
        //Inicializador da classe que recebe um array com as strings dos comandos de voz
        public comandoVoz(params string[] orders)
        {
            pComandos = orders;
        }

        public void Start(KinectSensor sensor)
        {
            kinectSensor = sensor;
            Record();
        }

        private List<string> GerarListaComandos()
        {
            List<string> comandos = new List<string>();

            foreach (String comando in pComandos)
            {
                comandos.Add(comando);
            }

            return comandos;
        }

        private Grammar GerarGramatica()
        {
            Choices comandos = new Choices();
            List<string> listaComandos = GerarListaComandos();

            comandos.Add("Kinect");
            comandos.Add("Cancel");

           
            StringBuilder sentencaBase = new StringBuilder();
            string inicial = sentencaBase.ToString();

            for (int indiceLista = 0; indiceLista < listaComandos.Count; indiceLista++)
            {
                StringBuilder sentencaFinal = new StringBuilder(inicial);
                sentencaFinal.Append(listaComandos[indiceLista]);
                comandos.Add(sentencaFinal.ToString());
            }

            GrammarBuilder construtor = new GrammarBuilder(comandos);
            construtor.Culture = speechRecognitionEngine.RecognizerInfo.Culture;

            Grammar gramatica = new Grammar(construtor);

            return gramatica;
        }

        void KinectSpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
          
            
            RecognitionResult resultado = e.Result;
            if (resultado.Confidence > 0.5)
            {
                if (resultado.Text == "Kinect")
                {
                    palavraChave = true;

                }

                else if (resultado.Text == "Cancel")
                {

                    palavraChave = false;

                }

                OrderDetected(resultado, palavraChave);
            }
            
            
        }

        void Record()
        {
            KinectAudioSource source = kinectSensor.AudioSource;

            Func<RecognizerInfo, bool> encontrarIdioma = reconhecedor =>
            {
                string value;
                reconhecedor.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase)
                && "en-US".Equals(reconhecedor.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };

            var recognizerInfo = SpeechRecognitionEngine.InstalledRecognizers().Where(encontrarIdioma).FirstOrDefault();

            if (recognizerInfo == null)
            {
                return;
            }


 
            source.AutomaticGainControlEnabled = false;
            source.BeamAngleMode = BeamAngleMode.Adaptive;
            

            speechRecognitionEngine = new SpeechRecognitionEngine(recognizerInfo.Id);
            speechRecognitionEngine.SpeechRecognized += KinectSpeechRecognitionEngine_SpeechRecognized;
            speechRecognitionEngine.LoadGrammar(GerarGramatica());



            //using (Stream sourceStream = source.Start())
            //{

            speechRecognitionEngine.SetInputToDefaultAudioDevice();
                //speechRecognitionEngine.SetInputToAudioStream(sourceStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                
            
            
            //}


        }

        public void Stop()
        {


            if (speechRecognitionEngine != null)
            {
               /*
                speechRecognitionEngine.UnloadAllGrammars();
                speechRecognitionEngine.RecognizeAsyncCancel();
                speechRecognitionEngine.RecognizeAsyncStop();
                speechRecognitionEngine.SetInputToNull();
                speechRecognitionEngine.Dispose();
                */
            }
        }
    }
}
