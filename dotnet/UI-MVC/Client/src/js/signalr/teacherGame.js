"use strict";
let getUrl = window.location;
let baseUrl = getUrl .protocol + "//" + getUrl.host + "/";
console.log("base url:"+baseUrl);
let connection;

const signalR = require("@microsoft/signalr");
if(getUrl.host.includes("localhost")){
    console.log("setting connection for signalR dev environment");
    connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5000/api/sessionHub").configureLogging(signalR.LogLevel.Debug)
        .build();
}
//testing cloud
else if(getUrl.host.includes(":")){
    let hostIp = getUrl.host.split(":")[0];
    console.log("setting connection for signalR cloud test environment");
    connection = new signalR.HubConnectionBuilder()
        .withUrl(getUrl.protocol+"//"+hostIp+":5000"+"/api/sessionHub").configureLogging(signalR.LogLevel.Debug)
        .build();
}
else{
    console.log("setting connection for signalR production environment");
    connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl+"api/sessionHub").configureLogging(signalR.LogLevel.Debug)
        .build();
}

let btnNextStatement=document.getElementById("nextStatement");
let sessioncode = document.getElementById("sessionCode").value;

connection.start().then(()=>{
    connection.invoke("AddTeacherGroup",sessioncode)
        .then(()=>console.log("adding teacher to group"))
        .catch(()=>console.log("Could not add teacher to group"));
}).catch(function (e) {
    console.log(e+ "can't connect to hub.");
});

btnNextStatement.onclick = function () {
    connection.invoke("SendGameReadySignal",sessioncode)
        .catch(()=>console.log("sending game ready signal failed"));
    document.getElementById("form").submit();
};

connection.on("RefreshUsers", function (amountOfUsers) {
   let h3AmountOfUsers = document.getElementById("numOfStudents");
   let maxAmount = document.getElementById("maxAmountStudents");
   if(h3AmountOfUsers!=undefined&&maxAmount!=undefined){
       h3AmountOfUsers.innerHTML="Amount of users: " + amountOfUsers + "/" + maxAmount.value;
   }
});

connection.on("RefreshPage",()=>{
    console.log("got reload signal");
    location.reload();
});
    
