
let checkbox = document.getElementById("defaultAnswers");
let defaultZone = document.getElementById("defaultAnswersSelect");
let customZone = document.getElementById("customAnswers");
let addAo = document.getElementById("addAo");
let aoError = document.getElementById("aoError");

let aoc1 = document.getElementById("aoc1");
let aoc2 = document.getElementById("aoc2");

let count = 3;

checkbox.addEventListener("change", function () {
    if (this.checked) {
        defaultZone.hidden = false;
        customZone.hidden = true;
        checkbox.setAttribute("value", "true");
    } else {
        defaultZone.hidden = true;
        customZone.hidden = false;
        checkbox.setAttribute("value", "false");
    }
});

aoc1.addEventListener("change",function () {
    if (this.checked){
        aoc1.setAttribute("value","true");
        document.getElementById("aoch1").setAttribute("value","true")
    }
    else {
        aoc1.setAttribute("value","false");
        document.getElementById("aoch1").setAttribute("value","false")
    }
});
aoc2.addEventListener("change",function () {
    if (this.checked){
        aoc2.setAttribute("value","true");
        document.getElementById("aoch2").setAttribute("value","true")
    }
    else {
        aoc2.setAttribute("value","false");
        document.getElementById("aoch2").setAttribute("value","false")
    }
});

addAo.addEventListener("click", function () {
    addElement();
});

function addElement() {
    
    if (count<7) {
        let group = document.createElement('div');
        group.setAttribute("class","aogroup form-group");
        group.setAttribute("id","answeroptiongroup"+count);
        
        let textbox = document.createElement('input');
        textbox.setAttribute("type", "text");
        textbox.setAttribute("id", "ao" + count);
        textbox.setAttribute("name", "answerOptions");
        textbox.setAttribute("class", "form-control");
        
        let label = document.createElement('label');
        label.setAttribute("for", "ao" + count);
        label.setAttribute("id", "aoL" + count);
        label.innerHTML = "Answer option" + count + ":";

        let remove = document.createElement('button');
        remove.setAttribute("id", "rmAo" + count);
        remove.classList.add("btn", "btn-light", "mx-2");
        remove.innerHTML = "Remove " + count;
        
        let correctLabel = document.createElement('label');
        correctLabel.setAttribute("for","aoc"+count);
        correctLabel.innerHTML = "Correct answer:";
        
        let correct = document.createElement('input');
        correct.setAttribute("type","checkbox");
        correct.setAttribute("id", "aoc"+count);
        correct.setAttribute("value","false");
        
        let correcthdn = document.createElement('input');
        correcthdn.setAttribute("type","hidden");
        correcthdn.setAttribute("id","aoch"+count);
        correcthdn.setAttribute("name","aoc");
        correcthdn.setAttribute("value","false");
        
        group.appendChild(label);
        group.appendChild(textbox);
        group.appendChild(correctLabel);
        group.appendChild(correct);
        group.appendChild(correcthdn);
        group.appendChild(remove);
        customZone.appendChild(group);
        
        console.log(count);      
        const test = count;
        remove.addEventListener("click", function () {
            removeAo(test);
        });
        correct.addEventListener("change", function () {
            correctCheckbox(test)
        });
    }
    else {
        aoError.innerHTML = "Maximum amount of answer options has been reached\n";
        aoError.hidden = false;
    }
    count++;

}

//werkt nog niet
function removeAo(number) {
    console.log(number);
    let zone = document.getElementById("answeroptiongroup"+number);
    customZone.removeChild(zone);
}

function correctCheckbox(count) {
    let check = document.getElementById("aoc" + count);
    let hid = document.getElementById("aoch" + count);
    console.log(count);
    
    if (check.checked){
        check.setAttribute("value","true");
        hid.setAttribute("value","true")
    }
    else {
        check.setAttribute("value","false");
        hid.setAttribute("value","false")
    }
}

document.getElementById('newStatement').addEventListener('click', function () {
    let statementError = document.getElementById("statementError");
    let aoError =  document.getElementById("aoError");
    let defaultAnswers = document.getElementById("defaultAnswers");
    let ao1 = document.getElementById("ao1");
    let ao2 = document.getElementById("ao2");
    let statement = document.getElementById("statementName");
    let correctAnswer;
    let correctAnswers = document.getElementsByName("aoc");
    console.log(correctAnswers);
    for (let i = 0; i < correctAnswers.length; i++) {
        if (correctAnswers[i].value==="true"){
            correctAnswer = true;
        }
    }
    aoError.innerText = "Default answers must be selected or 2 or more custom answer options must be filled in!";
    statementError.innerText = "";
    
    if (defaultAnswers.checked){
        aoError.innerText = ""
    }
    if (!defaultAnswers.checked&&ao1.value!==""&&ao2.value!==""){
        let aos = document.getElementsByName("answerOptions");
        let aoValues = [];
        for (let i = 0; i < aos.length; i++) {
            aoValues[i] = aos[i].value;
        }
        if (aoValues.length !== new Set(aoValues).size){
            aoError.innerText = "Answer options must be unique!"
        }
        else if (aoValues.includes("")){
            aoError.innerText = "All answer options must be filled in!"
        }
        else if (!correctAnswer){
            aoError.innerText = "One answer must be correct!"
        }
        else {
            aoError.innerText = ""
        }
    }
    if (statement.value===""){
        statementError.innerText="Statement must be filled in!"
    }
    if (aoError.innerText === ""&&statementError.innerText === ""){
        document.getElementById("statementForm").submit();
    }
});