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


//gets btnNext from waitingscreen if on that page
let btnNext = document.getElementById("btnNext");
let curstatementId = document.getElementById("currentStatementId").value;
let sessionCode = document.getElementById("teacherSessionCode").value;
btnNext.style.visibility = "hidden";

connection.start().then(()=>{
    console.log("adding to group");
    connection.invoke("AddToGroup",sessionCode)
        .catch((e)=>console.log(e+ "\n could not add to group"));
    connection.invoke("RefreshTeacherResults",sessionCode)
        .catch((e)=>console.log(e+ "\n Could not refresh teacher results"));
    
}).then(()=>{
    console.log("getting teacher position");
    connection.invoke("GetTeacherPosition",sessionCode)
        .catch((e)=>console.log(e+ "\n could not get teacher position"));
}).catch((e)=>console.log(e));

connection.on("SessionPosition", function (position,amountOfStatements) {
    console.log("ts is on position: " + position);
    console.log("user is on position: " + curstatementId);
    //If teacher has not started yet.
    if(!(position==-1)){
        if(position>=curstatementId || position==amountOfStatements+1){
            document.getElementById("form").submit();
        } 
    }
});

connection.on("StopWaiting", function (tsPosition) {
    console.log("Signal to stop waiting");
    console.log("ts is on position: " + tsPosition);
    console.log("user is on position: " + curstatementId);
    //Only if user is not ahead of teacher
        if(curstatementId-1<=(tsPosition+1)||tsPosition==-1){
            document.getElementById("form").submit();
        }
        else{
            console.log("still ahead of teacher.");
        }
});
