using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliar.Movimentos
{
    
    //Classe modelo para movimentos herdada em todos os movimentos
    public abstract class Movimento
    {
        public int ContadorQuadros { get; set; }
        public string Nome { get; set; }
        public abstract EstadoRastreamento Rastrear(Skeleton esqueletoUsuario);
        protected abstract bool PosicaoValida(Skeleton esqueletoUsuario);
    }
}
