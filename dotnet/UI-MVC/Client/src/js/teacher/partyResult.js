import {showPartyChart} from './showCharts'
let getUrl = window.location;
let baseUrl = getUrl .protocol + "//" + getUrl.host + "/";
const sessionCode = document.getElementById("sessionCode").value;

if(getUrl.host.includes("localhost")){
    fetch('https://localhost:5000/api/session/PartyChart/'+sessionCode, {
        mode: 'cors',
        method: 'GET'
    })    .then(function (response) {if (response.ok) return response.json(); })
        .then(function (data) { showPartyChart(data); });
}
else if(getUrl.host.includes(":")) {
    let hostIp = getUrl.host.split(":")[0];
    fetch(getUrl.protocol+'//'+hostIp+':5000'+'/api/session/PartyChart/'+sessionCode, {
        mode: 'cors',
        method: 'GET'
    })    .then(function (response) {if (response.ok) return response.json(); })
        .then(function (data) { showPartyChart(data); });
}
else{
    fetch(baseUrl+'api/session/PartyChart/'+sessionCode, {
        mode: 'cors',
        method: 'GET'
    })    .then(function (response) {if (response.ok) return response.json(); })
        .then(function (data) { showPartyChart(data); });
}
