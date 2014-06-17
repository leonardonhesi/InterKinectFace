using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

namespace InterKinectFace.Trasmitir
{
    /// <summary>
    /// Serializar informações para enviar quando reconhecer uma pose.
    /// </summary>
    public static class poseSerialize
    {
        
        [DataContract]
        class poseEnviar
        {
            [DataMember(Name = "frame")]
            public string FRAME { get; set; }
            
            [DataMember(Name = "pose")]
            public string POSE { get; set; }
            
        }

      
        public static string Seriall(string nomePose)
        {
                poseEnviar enviarPose = new poseEnviar
                {
                    FRAME = "NAO",
                    POSE = nomePose
                };

                return Serialize(enviarPose);
        }

        private static string Serialize(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();

            return retVal;
        }
    }
}
