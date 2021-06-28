const buttons = document.getElementById("partyParent").getElementsByTagName("BUTTON");
for (let i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function () {
        document.getElementById("partyName").value = buttons[i].id;
    })
}