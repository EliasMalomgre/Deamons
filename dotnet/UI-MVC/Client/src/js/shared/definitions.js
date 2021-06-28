let words = document.getElementById("statement").getElementsByTagName("A");
for (let i = 0; i < words.length; i++) {
    words[i].addEventListener('click', function () {
        document.getElementById("definitionField").style.display = "block";
        document.getElementById("word").innerText = words[i].innerText;
        document.getElementById("explanation").innerText = document.getElementById(words[i].innerText.trim()).value;
    });
}