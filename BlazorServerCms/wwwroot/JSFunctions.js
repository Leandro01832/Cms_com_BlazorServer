

    var porcentagem = 0;
var funcaoCarregarPagina2 = null;



// ...

window.zerar = (m) => {
    porcentagem = 0;
    var prog = document.querySelector(".progressbar div");
    prog.setAttribute("style", "width: " + porcentagem + "%");
    clearInterval(funcaoCarregarPagina2);
}


window.ExecutarReload = (m) => {
    location.reload();
}

window.ConfigurarPaginacao = (m) => {
    var elements2 = document.getElementsByClassName("produto");
    var elements3 = document.getElementsByClassName("caps");
    var element2 = document.getElementById("auto");
    var element3 = document.getElementById("tamanho");
    var time = parseInt(document.getElementById("tempo").value) * 1000;
    var ss = 0;
    var porcentagem = 0;
    var funcaoCarregarPagina = null;
    const progresso = document.querySelector(".progressbar div")  

    function myStopFunction() {
        clearInterval(funcaoCarregarPagina);
    }

    if (parseInt(element2.value) == 1) {
       funcaoCarregarPagina = setInterval(function () {
            ss += 1000;
            porcentagem = parseInt((ss / time) * 100);
            progresso.setAttribute("style", "width: " + porcentagem + "%");
            if (porcentagem > 99) 
                myStopFunction();            
        }, 1000);
    }

    for (var i = 0; i < elements2.length; i++) {
        if (parseInt(element3.value) == 81) {
            elements2[i].style.width = "90px";
            elements2[i].style.minHeight = "108px";
            elements2[i].style.height = "auto";
        }
        else
            if (parseInt(element3.value) == 41) {
                elements2[i].style.width = "172px";
                elements2[i].style.minHeight = "210px";
                elements2[i].style.height = "auto";
            }
            else
                if (parseInt(element3.value) == 11) {
                    elements2[i].style.width = "344px";
                    elements2[i].style.minHeight = "430px";
                    elements2[i].style.height = "auto";
                }
    }

    if (parseInt(element3.value) == 81) {
        for (var i = 0; i < elements3.length; i++)
            elements3[i].style.fontSize = "0.8em";
    }
    else
        if (parseInt(element3.value) == 41) {
            for (var i = 0; i < elements3.length; i++)
                elements3[i].style.fontSize = "1.2em";
        }
        else
            if (parseInt(element3.value) == 11) {
                for (var i = 0; i < elements3.length; i++)
                    elements3[i].style.fontSize = "1.8em";
            }
}

window.PreencherProgressBar = (m) =>
{
    var time = parseInt(m);
    var ss = 0;
    const progresso = document.querySelector(".progressbar div"); 

    function myStopFunction2() {
        clearInterval(funcaoCarregarPagina2);
    }
    
        funcaoCarregarPagina2 = setInterval(function () {
            ss += 1000;
            porcentagem = parseInt((ss / time) * 100);
            progresso.setAttribute("style", "width: " + porcentagem + "%");
            if (porcentagem > 99)
                myStopFunction2();
        }, 1000);    
}

window.Clicou = (m) => {
    var elements1 = document.getElementsByClassName("info");
    var element = document.getElementById("cabecalho");
    for (var i = 0; i < elements1.length; i++) {
        elements1[i].style.display = "block";
    }
    element.style.display = "flex";
}

window.GeminiResponse = (p) => {

    //const prompt = p;
    
    //const result = await model.generateContent(prompt);
    //const response = await result.response;
    //const text = response.text();
    //console.log(text);

    return p;

}


window.MarcarIndice = (id) => {
    var indice = parseInt(id);
    var ind2 = indice;
    var elemento = "";
    var elemento2 = "";
    var divs = document.getElementsByClassName("DivPag");
    var largura = window.screen.width;
    var quantDiv = 0;
    var scrolando = 0;
    var calculo = 0;

    if (largura > 500) {
        quantDiv = parseInt((21 * largura) / 1024);
        scrolando = 32;
    }
    else {
        if (indice < 100) {
            quantDiv = parseInt((13 * largura) / 344);
            scrolando = 28;
        }
        else if (indice > 99 && indice < 1000) {
            quantDiv = parseInt((11 * largura) / 344);
            scrolando = 28;
        }
        else if (indice > 999 && indice < 10000) {
            quantDiv = parseInt((8 * largura) / 344);
            scrolando = 28;
        }
        else if (indice > 9999 && indice < 100000) {
            quantDiv = parseInt((6 * largura) / 344);
            scrolando = 28;
        }
    }

    var resto = parseInt(indice % quantDiv);

    if (resto == 0) {
        ind2++;
        elemento = "DivPagina" + indice;
        elemento2 = "DivPagina" + ind2;
        var el = document.getElementById(elemento);
        var el2 = document.getElementById(elemento2);
        var posicao = el.getBoundingClientRect();
        var posicao2 = el2.getBoundingClientRect();

        if (posicao.left < posicao2.left)
            quantDiv = quantDiv + 1;
    }
    else
    while (resto != 0) {
        ind2++;
        var ind3 = ind2 + 1;
        resto = ind2 % quantDiv;
        if (resto == 0) {
            elemento = "DivPagina" + ind2;
            elemento2 = "DivPagina" + ind3;
            var el = document.getElementById(elemento);
            var el2 = document.getElementById(elemento2);
            var posicao = el.getBoundingClientRect();
            var posicao2 = el2.getBoundingClientRect();

            if (posicao.left < posicao2.left)
                quantDiv = quantDiv + 1;
        }

    }


    resto = parseInt(indice % quantDiv);
    var filas = parseInt(indice / quantDiv);
    if (filas > 0 && resto == 0)
        filas--;
    if (filas > 0)
        calculo = (scrolando * filas);

    console.log("Largura: " + largura);
    console.log("Tamanho: " + quantDiv);
    console.log("Calculo: " + calculo);
    console.log("Filas: " + filas);

    document.getElementsByClassName("DivPagina")[0].scrollBy(0, -200000);
    if(calculo > 0)
    document.getElementsByClassName("DivPagina")[0].scrollBy(0, calculo);
}

window.FullScreen =(teste) => {
    var elem = document.getElementById("corpoPagina");
    if (elem.requestFullscreen) {
        elem.requestFullscreen();
    } else if (elem.mozRequestFullScreen) { /* Firefox */
        elem.mozRequestFullScreen();
    } else if (elem.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
        elem.webkitRequestFullscreen();
    } else if (elem.msRequestFullscreen) { /* IE/Edge */
        elem.msRequestFullscreen();
    }
}

window.SelecionarLivro = (id) =>
{
    var elemento = document.getElementById("select" + id);
    var input = document.getElementById("url");
    var lista = document.getElementById("lista");
    input.value = elemento.innerText;
    lista.innerHTML = "";
}

function focusById(elementId) {
    var element = document.getElementById(elementId);    
        element.focus();    
}

window.CopiarLink = (m) => {
    let textoCopiado = document.getElementById("texto");
    textoCopiado.select();
    textoCopiado.setSelectionRange(0, 99999)
    document.execCommand("copy");
    alert("O texto é: " + textoCopiado.value);
}

window.SetarLink = (m) => {
    let texto = document.getElementById("texto");
    texto.value = m;
}

window.DarAlert = (m) => {
    alert(m);
}

window.AcessarSites = (url) => {
    window.open(url, '_blank');
}

window.sairFullScreen = (url) => {
    document.exitFullscreen();
}


window.share = (config) => {

    var arr = config.split("/");

    var endereco = window.location.href;
    var titulo = arr[0];
    var resumo = arr[1];    

    if (navigator.share !== undefined) {
        navigator.share({
            title: titulo,
            text: resumo,
            url: endereco,
        })
            .then(() => alert('Compartilhamento feito com sucesso!!!'))
            .catch((error) => alert('Erro ao compartilhar!!! ' + error, error));
    }
}



