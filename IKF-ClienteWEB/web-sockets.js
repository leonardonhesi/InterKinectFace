window.onload = function () {
    var status = document.getElementById("status");
    var canvas = document.getElementById("canvas");
    var canvas1 = document.getElementById("canvas1");
    var canvas2 = document.getElementById("canvas2");
   var canvas3 = document.getElementById("canvas3");
   var canvas4 = document.getElementById("canvas4");
   var canvas5 = document.getElementById("canvas5");
   var canvas6 = document.getElementById("canvas6");
    var context = canvas.getContext("2d");
    var context1 = canvas1.getContext("2d");
    var context2 = canvas2.getContext("2d");
    var context3 = canvas3.getContext("2d");
    var context4 = canvas4.getContext("2d");
    var context5 = canvas5.getContext("2d");
    var context6 = canvas6.getContext("2d");    
    //var avancando = false;
    //var voltando = false;
    //var contagemDir = 60;
    //var contagemEsq = 60;
	   
	
    var camera = new Image();

    function recebeJuntas(maoDireita,maoEsquerda,cabeca)
    {
	if ( maoDireita > (cabeca + 65) )
	{
		if ( !avancando && contagemDir == 0)
		{		
			contagemDir = 60;			
			avancando = true;			
			var cube = $('#cube');	
			var stepData = cube.jmpress("active").data("stepData");
			if(stepData.down)
			cube.jmpress("select", stepData.down);
		}
		else
		{
			contagemDir -= 1;			
			avancando = false;
		}
	}
	else if ( maoEsquerda < (cabeca - 65) )
	{
		if ( !voltando && contagemEsq == 0)
		{		
			contagemEsq = 60;
			voltando = true;			
			var cube = $('#cube');	
			var stepData = cube.jmpress("active").data("stepData");
			if(stepData.up)
			cube.jmpress("select", stepData.up);
		}
		else
		{
			contagemEsq -= 1;
			voltando = false;
		}		

	}

     
    }

    camera.onload = function () {
                context1.drawImage(camera, 0, 0);
		context2.drawImage(camera, 0, 0);
		context3.drawImage(camera, 0, 0);
		context4.drawImage(camera, 0, 0);
		context5.drawImage(camera, 0, 0);
		context6.drawImage(camera, 0, 0);
    }

    if (!window.WebSocket) {
        status.innerHTML = "Já é hora de atualizar seu browser! chegou HTML5";
        return;
    }

    status.innerHTML = "Conectando com IKF-SERVER...";

    // Inicializa um novo socket colocar IP maquina que esta rodando o IKF
    var socket = new WebSocket("ws://10.0.0.5:8181/KinectHtml5");

    // Conexão estabelecida
    socket.onopen = function () {
        status.innerHTML = "Conexão estabelecida.";
    };

    // Conexão fechada
    socket.onclose = function () {
        status.innerHTML = "Conexão fechada.";
    }

    // Receber dados do servidor
    socket.onmessage = function (event) {
        
		if (typeof event.data === "string") {
            

            // Obtem o dado do JASON.
            var jsonObject = JSON.parse(event.data);
            
		//A Iformação não e um frame, é um reconhecimento de pose		
		if ( jsonObject.frame === "NAO" )
		{
			if ( jsonObject.pose === "AVANCA")
			{
				var cube = $('#cube');	
				var stepData = cube.jmpress("active").data("stepData");
				if(stepData.down)
				cube.jmpress("select", stepData.down);

			}
			else if ( jsonObject.pose === "VOLTA")
			{
				var cube = $('#cube');	
				var stepData = cube.jmpress("active").data("stepData");
				if(stepData.up)
				cube.jmpress("select", stepData.up);

			}

			
		}
		else
		{        
			// DADOS DO SKELETON
			context.clearRect(0, 0, canvas.width, canvas.height);
			//Definir cor preta para a marcação dos pontos das  juntas
			context.fillStyle = "rgb(0, 0, 0)";
		
		//var maoDireitax;
		//var maoEsquerdax;
		//var cabecax;
            

            // Percorrer e mostrar as juntas do skeleto
            for (var i = 0; i < jsonObject.skeletons.length; i++) {
                for (var j = 0; j < jsonObject.skeletons[i].joints.length; j++) {
                    var joint = jsonObject.skeletons[i].joints[j];

                    /*if ( joint.name === "handright")
		    {
			maoDireitax = joint.x;			
		    }
		    else if ( joint.name === "handleft")
		    {
			maoEsquerdax = joint.x;
		    } 
		    else  if ( joint.name === "head")
		    {
			cabecax = joint.x;
		    }
		    */
                    
		    // Desenhar
                    context.beginPath();
                    context.arc(joint.x, joint.y, 10, 0, Math.PI * 2, true);
                    context.closePath();
		    context.fill();        
                }
		
            }
		//recebeJuntas(maoDireitax,maoEsquerdax,cabecax);	
           }
        }
        else if (event.data instanceof Blob) {
            // IMAGEM DA TELA
            // 1. Obter os dados
            var blob = event.data;

            // 2. Criar nova URL para o Objeto Blob
            window.URL = window.URL || window.webkitURL;

            var source = window.URL.createObjectURL(blob);

            // 3. Atualizar origem da imagem no componente.
            camera.src = source;

            // 4. Liberar a memoria utilizada
            window.URL.revokeObjectURL(source);
        }
    };

};
