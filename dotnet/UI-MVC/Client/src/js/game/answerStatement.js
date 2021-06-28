import '../../css/_answerStatement.scss'

const buttons = document.getElementById("form").getElementsByTagName("BUTTON");
for (let i = 0; i < buttons.length; i++) {
    buttons[i].addEventListener("click", function () {
        let answer = document.getElementById("answer");
        answer.value = document.getElementById(buttons[i].innerText).value;
        let argument = document.getElementById("argument");
        
        if (answer.value==="3"){
            argument.removeAttribute('required');
        }
        if (argument==null){
            document.getElementById("form").submit();
            for (let j = 0; j < buttons.length; j++) {
                buttons[j].disabled  = true;
            }
        }
        
        else if (argument.value!==""||answer.value===3){
            document.getElementById("form").submit();
            for (let j = 0; j < buttons.length; j++) {
                buttons[j].disabled  = true;
            }
        }
    });    
}
document.getElementById("currentStatementId").value++;
