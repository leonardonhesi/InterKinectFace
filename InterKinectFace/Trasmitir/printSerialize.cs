using Microsoft.Kinect;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace InterKinectFace.Trasmitir
{
    class printSerialize
    {

        public byte[] CreateBlob()
        {

            String file = "enviar.png";

            Bitmap bmpScreenshot;
            Graphics gfxScreenshot;
            bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //obtem o print da tela
            gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);

            //reduz o formato do arquivo de imagem 
            Bitmap result = new Bitmap(640, 480);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(bmpScreenshot, 0, 0, 640, 480);
            bmpScreenshot = result;

            //salva o arquivo
            bmpScreenshot.Save("enviar.png", ImageFormat.Png);

            // Converter bitmap para Blob para transmitir.
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
                }
            }
        }

    }

}
