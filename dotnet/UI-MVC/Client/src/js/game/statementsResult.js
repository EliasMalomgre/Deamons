const currentStatement = document.getElementById("currentStatementId");
const statementCount = document.getElementById("statementCount").value - 1;
document.getElementById("previous").addEventListener("click", function () {
    if (currentStatement.value == 0){
        currentStatement.value = statementCount;
    }
    else {
        currentStatement.value--;
    }
});

document.getElementById("next").addEventListener("click", function () {
    if (currentStatement.value == statementCount){
        currentStatement.value = 0;
    }
    else {
        currentStatement.value++;

    }
});