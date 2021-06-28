let count = document.getElementById('removeCount').getAttribute('value');
let zone = document.getElementById('answerOptions');
let addButton = document.getElementById('addAnswerOption');

let i;
for (i = 1; i<count;i++){
    let button = document.getElementById('removeAoBtn'+i);
    let check = document.getElementById('AoCheck'+i);
    const num = i;
    button.addEventListener('click', function (){
        removeElements(num);
    });
    check.addEventListener('change', function () {
        correctCheckbox(num);
    });
    correctCheckbox(num);
}

addButton.addEventListener('click', function () {
    count++;
    addElements();
});

function removeElements(number) {
    let group = document.getElementById('AoGroup'+number);
    group.remove();
    count--;
}

function addElements() {
    const num = count;
    
    let group = document.createElement('div');
    group.setAttribute('id','AoGroup'+num);
    group.classList.add('form-group');

    let textbox = document.createElement('input');
    textbox.setAttribute("type", "text");
    textbox.setAttribute("id", "AoText" + num);
    textbox.setAttribute("name", "answerOptions");
    textbox.setAttribute("class", "form-control");

    let remove = document.createElement('button');
    remove.setAttribute("id", "removeAoBtn" + num);
    remove.setAttribute("type","button");
    remove.innerHTML = "Remove";

    let correctLabel = document.createElement('label');
    correctLabel.setAttribute("for","AoLabel"+num);
    correctLabel.innerHTML = "Is correct:";

    let correct = document.createElement('input');
    correct.setAttribute("type","checkbox");
    correct.setAttribute("id", "AoCheck"+num);
    correct.setAttribute("value","false");

    let correcthdn = document.createElement('input');
    correcthdn.setAttribute("type","hidden");
    correcthdn.setAttribute("id","AoHid"+num);
    correcthdn.setAttribute("name","aoc");
    correcthdn.setAttribute("value","false");
    
    remove.addEventListener('click', function () {
            removeElements(num);
    });
    
    group.appendChild(textbox);
    group.appendChild(correctLabel);
    group.appendChild(correct);
    group.appendChild(correcthdn);
    group.appendChild(remove);
    zone.appendChild(group);
    
    correct.addEventListener('change', function () {
        correctCheckbox(num);
    })
}

function correctCheckbox(count) {
    let check = document.getElementById("AoCheck" + count);
    let hid = document.getElementById("AoHid" + count);

    if (check.checked){
        check.setAttribute("value","true");
        hid.setAttribute("value","true")
    }
    else {
        check.setAttribute("value","false");
        hid.setAttribute("value","false")
    }
    
    
}