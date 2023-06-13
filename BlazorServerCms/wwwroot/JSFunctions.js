window.ExecutarReload = (m) => {
    location.reload();
}

window.MarcarIndice = (id) => {
    var divs = document.getElementsByClassName("DivPag");

    for (var i = 0; i < divs.length; i++)
    {
        divs[i].style.color = "black";
    }

    document.getElementById("DivPagina" + id).style.color = "red";
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


function focusById(elementId) {
    var element = document.getElementById(elementId);    
        element.focus();    
}