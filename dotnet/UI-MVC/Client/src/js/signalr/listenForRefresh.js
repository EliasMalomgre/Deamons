"use strict"; 
let getUrl = window.location;
let baseUrl = getUrl.protocol + "//" + getUrl.host + "/";
console.log("base url:" + baseUrl);
let connection;

const signalR = require("@microsoft/signalr");
//local dev env
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
//production cloud
else{
    console.log("setting connection for signalR production environment");
     connection = new signalR.HubConnectionBuilder()
        .withUrl(baseUrl+"api/sessionHub").configureLogging(signalR.LogLevel.Debug)
        .build();
}


connection.start().then(()=>{
    connection.invoke("AddTeacherGroup",sessioncode)
        .then(()=>console.log("adding teacher to group"))
        .catch(()=>console.log("Could not add teacher to group"));
}).catch((e)=>console.log(e));

let sessioncode = document.getElementById("sessionCode").value;

connection.on("RefreshPage",()=>{
    console.log("got reload signal");
    location.reload();
});