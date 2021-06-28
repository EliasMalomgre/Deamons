import {showDebateChart} from './showCharts'
let getUrl = window.location;
let baseUrl = getUrl .protocol + "//" + getUrl.host + "/";
let trs = document.getElementsByTagName("TR");
const sessionId = document.getElementById("sessionId").value;
for (let i = 1; i < trs.length; i++) {
    trs[i].addEventListener('click', function () {
        let index = trs[i].children[0].innerText-1;
        let statement = trs[i].children[1].innerText;
        fetchChart(sessionId, index, statement)
    })
}

function fetchChart(sessionId, index, statement){
    if(getUrl.host.includes("localhost")){
        fetch('https://localhost:5000/api/session/pastdebatechart/'+sessionId+'/'+index, {
            mode: 'cors',
            method: 'GET'
        })    .then(function (response) {if (response.ok) return response.json(); })
            .then(function (data) { showDebateChart(data, statement); });
    }
    else if(getUrl.host.includes(":")) {
        let hostIp = getUrl.host.split(":")[0];
        fetch(getUrl.protocol+'//'+hostIp+':5000'+'/api/session/pastdebatechart/'+sessionId+'/'+index, {
            mode: 'cors',
            method: 'GET'
        })    .then(function (response) {if (response.ok) return response.json(); })
            .then(function (data) { showDebateChart(data, statement); });
    }
    else{
        fetch(baseUrl+'api/session/pastdebatechart/'+sessionId+'/'+index, {
            mode: 'cors',
            method: 'GET'
        })    .then(function (response) {if (response.ok) return response.json(); })
            .then(function (data) { showDebateChart(data, statement); });
    }
}

fetchChart(sessionId, 0 , trs[1].children[1].innerText);
