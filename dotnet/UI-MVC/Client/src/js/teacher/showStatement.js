import '../../css/_buttons.scss'
document.getElementById("extra").addEventListener("click", function () {
    let info = document.getElementById("info");
    if (info.style.display === "none"){
        info.style.display = "block"
    }
    else {
        info.style.display = "none"
    }
});

