const gameType = document.getElementById("type");
gameType.addEventListener("change", hideTests);
let colours = document.getElementById("colours");
let colourSelectors = colours.getElementsByTagName("SELECT");
let colourGroup = document.getElementById("colourGroup");
let test = document.getElementById("test");
const standardColors = ["#8CB369", "#D7263D", "#F85A3E", "#1098F7", "#F49D37", "#AA1155", "#E0ACD5"];

function hideTests(){
    let options = document.getElementById("test").children;
    let colour3 = document.getElementById("colour3");
    let colour3group = document.getElementById("colour3group");
    let colour4 = document.getElementById("colour4");
    let colour4group = document.getElementById("colour4group");
    let colour5 = document.getElementById("colour5");
    let colour5group = document.getElementById("colour5group");
    let colour6 = document.getElementById("colour6");
    let colour6group = document.getElementById("colour6group");


    if (gameType.value === "DEBATEGAME"||gameType.value === "PARTYGAME"){
        options[0].style.display = "block";
        for (let i = 1; i < options.length; i++) {
            options[i].style.display = "none";
            test.value = options[0].value;
        }
    }
    else {
        if (options.length>1){
            options[0].style.display = "none";
            test.value = options[1].value;
            for (let i = 1; i < options.length; i++) {
                options[i].style.display = "block";
            }
        }
        else {
            gameType.value = "DEBATEGAME";
            document.getElementById("error").innerText = "You must possess custom tests to be able to play a custom game!"
        }
           
    }
    if (gameType.value === "CUSTOMGAME_DEBATE"){
        colourGroup.style.display = "block";
        colour3group.style.display = "block";
        colour4group.style.display = "block";
        colour5group.style.display = "block";
        colour6group.style.display = "block";
    }
    else if (gameType.value === "DEBATEGAME"){
        colourGroup.style.display = "block";
        colour3group.style.display = "none";
        colour3.value = standardColors[3];
        colour3.style.backgroundColor = standardColors[3];
        colour4group.style.display = "none";
        colour4.value = standardColors[4];
        colour4.style.backgroundColor = standardColors[4];
        colour5group.style.display = "none";
        colour5.value = standardColors[5];
        colour5.style.backgroundColor = standardColors[5];
        colour6group.style.display = "none";
        colour6.value = standardColors[6];
        colour6.style.backgroundColor = standardColors[6];
    }
    else if (gameType.value === "CUSTOMGAME_PARTY"){
    }
    else if (gameType.value === "PARTYGAME"){
        colourGroup.style.display = "none";
        document.getElementById("forceWaiting").checked = true;
    }
}
hideTests();

let check = document.getElementById("select");
check.addEventListener("change",function () {
    if (this.checked){
        check.setAttribute("value","true");
        document.getElementsByName("selectStatements")[0].setAttribute("value","true")
    }
    else {
        check.setAttribute("value","false");
        document.getElementsByName("selectStatements")[0].setAttribute("value","false")
    }
});
    
for (let i = 0; i < colourSelectors.length; i++) {
    colourSelectors[i].style.backgroundColor = standardColors[i];
    colourSelectors[i].innerHTML = `
        <option value=${standardColors[i]}></option>
        <option value="#7FFFD4"></option>
        <option value="#0000FF"></option>
        <option value="#000080"></option>
        <option value="#800080"></option>
        <option value="#FF1493"></option>
        <option value="#EE82EE"></option>
        <option value="#FFC0CB"></option>
        <option value="#006400"></option>
        <option value="#008000"></option>
        <option value="#9ACD32"></option>
        <option value="#FFFF00"></option>
        <option value="#FFA500"></option>
        <option value="#FF0000"></option>
        <option value="#A52A2A"></option>
        <option value="#DEB887"></option>
        <option value="#F5F5DC"></option>`;
    let options = colourSelectors[i].getElementsByTagName("OPTION");
    for (let j = 0; j < options.length; j++) {
        options[j].style.backgroundColor = options[j].value;
    }
    colourSelectors[i].addEventListener("change", function () {
        colourSelectors[i].style.backgroundColor = colourSelectors[i].value
    })
}

document.getElementById("customColors").addEventListener("change", function () {
    if (this.checked){
        colours.style.display = "flex";
    }
    else {
        colours.style.display = "none";
        for (let i = 0; i < colourSelectors.length; i++) {
            colourSelectors[i].value = standardColors[i];
            colourSelectors[i].style.backgroundColor = standardColors[i];
        }
    }
});

document.getElementById("skip").addEventListener("change", function () {
    let skipColour = document.getElementById("skipColour");
    let skipColourGroup = document.getElementById("skipColourGroup");

    if (this.checked){
        skipColourGroup.style.display = "block";
    }
    else {
        skipColourGroup.style.display = "none";
        skipColour.value = standardColors[6];
        skipColour.style.backgroundColor = standardColors[6];
    }
});