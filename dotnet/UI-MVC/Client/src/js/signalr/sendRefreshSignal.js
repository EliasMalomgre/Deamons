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

let tsessioncode = document.getElementById("teacherSessionCode").value;

console.log("sessioncode: "+tsessioncode);

connection.start().then(()=>{
    connection.invoke("RefreshTeacherResults",tsessioncode)
        .then(()=>console.log("Sending refresh signal"))
        .catch(()=>console.log("Could not send refresh signal"));
}).catch(function (e) {
    console.log(e+ "can't connect to hub.");
});