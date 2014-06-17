using Auxiliar.Basicos;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliar.Movimentos.Poses
{
    public class PoseVoltar : Pose
    {
        public PoseVoltar()
        {
            this.Nome = "PoseVoltar";
            this.QuadroIdentificacao = 20;
        }

        protected override bool PosicaoValida(Skeleton esqueletoUsuario)
        {

            Joint maoEsquerda = esqueletoUsuario.Joints[JointType.HandLeft];
            Joint cabeca = esqueletoUsuario.Joints[JointType.Head];

            double margemErro = 0.20;
            bool posicao = Util.CompararComMargemErro(margemErro, maoEsquerda.Position.X, (cabeca.Position.X - 0.65));
            return posicao;
        }
    }
}
