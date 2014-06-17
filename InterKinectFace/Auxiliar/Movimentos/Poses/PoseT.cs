using Auxiliar.Basicos;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliar.Movimentos.Poses
{
    public class PoseT : Pose
    {
        public PoseT()
        {
            //Utilizada para fechar a apresentação
            this.Nome = "PoseT";
            this.QuadroIdentificacao = 20;
        }

        protected override bool PosicaoValida(Skeleton esqueletoUsuario)
        {
            //Obtem as posições necessarias
            Joint centroOmbros = esqueletoUsuario.Joints[JointType.ShoulderCenter];
            Joint maoDireita = esqueletoUsuario.Joints[JointType.HandRight];
            Joint cotoveloDireito = esqueletoUsuario.Joints[JointType.ElbowRight];
            Joint maoEsquerda = esqueletoUsuario.Joints[JointType.HandLeft];
            Joint cotoveloEsquerdo = esqueletoUsuario.Joints[JointType.ElbowLeft];


            double margemErro = 0.50;

            //LADO DIREITO - Verifica se cotovelo e mão direita estão na mesma altura (Y) do centro do ombro bem como Eixo X da mão maior que o Cotovelo
            //braço esticado, e tambem o eixo (z) da mão e do ombro sejam os mesmo 
            //Altura da mão direita igual ao centro do ombro
            bool maoDireitaAlturaCorreta = Util.CompararComMargemErro(margemErro, maoDireita.Position.Y, centroOmbros.Position.Y);
            //Distancia utilizando o eixo z garantindo que a altura esteja correta e o braço estendido
            bool maoDireitaDistanciaCorreta = Util.CompararComMargemErro(margemErro, maoDireita.Position.Z, centroOmbros.Position.Z);
            //Verifica mão direita eixo x seja maior que o cotovelo
            bool maoDireitaAposCotovelo = maoDireita.Position.X > cotoveloDireito.Position.X;
            //Verifica se cotovelo mesma altura centro do ombro 
            bool cotoveloDireitoAlturaCorreta = Util.CompararComMargemErro(margemErro, cotoveloDireito.Position.Y, centroOmbros.Position.Y);

            //LADO ESQUERDO - Repete-se as verificações para o lado esquerdo
            bool cotoveloEsquerdoAlturaCorreta = Util.CompararComMargemErro(margemErro, cotoveloEsquerdo.Position.Y, centroOmbros.Position.Y);
            bool maoEsquerdaAlturaCorreta = Util.CompararComMargemErro(margemErro, maoEsquerda.Position.Y, centroOmbros.Position.Y);
            bool maoEsquerdaDistanciaCorreta = Util.CompararComMargemErro(margemErro, maoEsquerda.Position.Z, centroOmbros.Position.Z);
            bool maoEsquerdaAposCotovelo = maoEsquerda.Position.X < cotoveloEsquerdo.Position.X;

            //Retorna T somente se todas as posições estiverem corretas
            return maoDireitaAlturaCorreta && maoDireitaDistanciaCorreta && maoDireitaAposCotovelo && cotoveloDireitoAlturaCorreta &&
                   maoEsquerdaAlturaCorreta && maoEsquerdaDistanciaCorreta && maoEsquerdaAposCotovelo && cotoveloEsquerdoAlturaCorreta;
        }

    }
}
