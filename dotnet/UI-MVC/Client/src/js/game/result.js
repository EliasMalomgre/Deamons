document.getElementById("hide").addEventListener("click", function () {
    let trs = document.getElementsByTagName("INPUT");
    for (let i = 0; i < trs.length; i++) {
        if (trs[i].checked === true){
            if (trs[i].parentElement.parentElement.style.display === 'none'){
                trs[i].parentElement.parentElement.style.display = ''
            }
            else {
                trs[i].parentElement.parentElement.style.display = 'none'
            }
        }
    }});


