


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
    var funcaoCarregarPagina2 = null;
    var time = parseInt(m);
    var ss = 0;
    var porcentagem = 0;
    const progresso = document.querySelector(".progressbar div")  

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

window.MarcarIndice = (id) => {
    var divs = document.getElementsByClassName("DivPag");

    for (var i = 0; i < divs.length; i++)
    {
        divs[i].style.color = "black";
    }
    var largura = window.screen.width;
    var tamanho = 0;
    var scrolando = 0;
    if (largura > 500) {
        tamanho = parseInt((25 * largura) / 1280);
         scrolando = 32;
    }
    else  {
        tamanho = 14;
        scrolando = 28;
    }
    var indice = parseInt(id);
    var r = parseInt(indice / tamanho);
    if(r  > 0)
        var calculo = (scrolando * r);

    console.log(largura);

    document.getElementById("DivPagina" + id).style.color = "red";
    document.getElementsByClassName("DivPagina")[0].scrollBy(0, -2000);
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



