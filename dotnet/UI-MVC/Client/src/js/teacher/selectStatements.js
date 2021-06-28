let boxes = document.getElementById("statements").getElementsByTagName("INPUT");
for (let i = 0; i < boxes.length; i++) {
    boxes[i].addEventListener('click', function () {
        if (boxes[i].checked){
            
            let input = `<input type="hidden" name="SelectedStatementsId" value="${boxes[i].id}" />`;
            
            document.getElementById("select").innerHTML += input;
        }
        else {
            let inputs = document.getElementsByName("statementsId");
            for (let j = 0; j < inputs.length; j++) {
                if (inputs[j].value===boxes[i].id){
                    inputs[j].remove();
                }
            }
        }
    })
}
