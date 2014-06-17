using Auxiliar.Basicos;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliar.Movimentos.Poses
{
    public class PoseAvanca : Pose
    {
        public PoseAvanca()
        {
            this.Nome = "PoseAvanca";
            //Quantos quadros são necessarios para validar a pose
            this.QuadroIdentificacao = 20;
        }

        //Utilizado para validar as poses, recebe o esqueleto
        protected override bool PosicaoValida(Skeleton esqueletoUsuario)
        {
            //Obtem as juntas especificas, que no caso são a mão direita e a cabeça
            Joint maoDireita = esqueletoUsuario.Joints[JointType.HandRight];
            Joint cabeca = esqueletoUsuario.Joints[JointType.Head];

            //Definise uma margem de erro
            double margemErro = 0.20;
            //Valida o esqueleto na posção no caso eixo x da mão sendo igual ao eixo x dacabeça + 0.65
            //braço direito levantado
            bool posicao = Util.CompararComMargemErro(margemErro, maoDireita.Position.X, (cabeca.Position.X + 0.65));

            return posicao;
        }
    }
}