using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InterKinectFace.Configs
{
    public class Configurar
    {

        private String diretorio;
        private String stream;
        private bool configOk;

        public String getDiretorio()
        {
            return this.diretorio;
        }
        public String getStream()
        {
            return this.stream;
        }

        public void setDiretorio(String diretorio)
        {
            this.diretorio = diretorio;
        
        }
        public void setStream(String stream)
        {
            this.stream = stream;
        }

        public bool getConfigOk()
        {

            return this.configOk;
            
        }
        public void setConfigOk(bool configOk)
        {

            this.configOk = configOk;
        }

        //Construtor
        public Configurar()
        {

            this.setConfigOk(true);
            if (File.Exists("config.xml"))
            {


                //Carrega diretorio transferencia pelo arquivo de configuração
                //Ler xml configuração
                XmlTextReader xmlconfig = new XmlTextReader("config.xml");

                while (xmlconfig.Read())
                {
                    switch (xmlconfig.NodeType)
                    {
                        case XmlNodeType.Element:

                            while (xmlconfig.MoveToNextAttribute())
                            {
                                switch (xmlconfig.Name)
                                {
                                    case "Diretorio":
                                        this.setDiretorio(xmlconfig.Value.ToString());
                                        break;

                                    case "Transmite":
                                        this.setStream(xmlconfig.Value.ToString());
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                this.setConfigOk(false);
            
            }
        

        }
        
    
    }
}
