import '../../css/_buttons.scss'
const buttons = document.getElementById("partyParent").getElementsByTagName("BUTTON");
for (let i = 0; i < buttons.length; i++) {
    let button = buttons[i];
    button.addEventListener("click", function () {

        let input = document.getElementById(button.getAttribute("id")+"hidden");
        if (button.classList.contains("red-border")){
            input.value = button.id;
            button.classList = null;
            button.classList.add("btn","green-border","ml-auto","party-selection-button")
        }
        else {
            input.value = "";
            button.classList = null;
            button.classList.add("btn","red-border","ml-auto","party-selection-button")
        }
    })
}
