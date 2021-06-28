const button = document.getElementById("nextStatement");
button.addEventListener("click", function () {
    document.getElementById("currentStatement").value++;
});
document.getElementById("hide").addEventListener("click", function () {
    let parties = document.getElementsByName("party");
    for (let i = 0; i < parties.length; i++) {
        if (parties[i].style.display === 'none'){
            parties[i].style.display = ''
        }
        else {
            parties[i].style.display = 'none'
        }    }
});
