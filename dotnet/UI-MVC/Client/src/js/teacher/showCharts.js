import Chart from 'chart.js';
let chart;
let ctx = document.getElementById("chartContainer");

export function showDebateChart(input, statement) {
    if (chart) {chart.destroy();}
    console.log(input);
    let data = [];
    let label = [];
    if (input!==null){
        for (let i = 0; i < input.length; i++) {
            data[i] = input[i].y;
            label[i] = input[i].label;
        }
    }
    chart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: label,
            datasets: [{
                label: statement,
                data: data,
                backgroundColor: [
                    '#8CB369',
                    '#D7263D',
                    '#E0ACD5',
                    '#F85A3E',
                    '#1098F7',
                    '#F49D37',
                    '#AA1155'
                ],
                borderColor: [
                    '#8CB369',
                    '#D7263D',
                    '#E0ACD5',
                    '#F85A3E',
                    '#1098F7',
                    '#F49D37',
                    '#AA1155'
                ],
                borderWidth: 1
            }]
        },
        options: {
            title: {
                display: true,
                text: statement,
                fontSize: 24

            }
        }
    });
    chart.update();
}

export function showPartyChart(input) {
    console.log(input);
    if (chart) {chart.destroy();}
    let data = [];
    let label = [];
    let colour = [];
    if (input!==undefined){
        for (let i = 0; i < input.length; i++) {
            data[i] = input[i].y;
            label[i] = input[i].label;
            colour[i] = input[i].colour;
        }
    }   
    chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: label,
            datasets: [{
                label: "Percentage per party",
                data: data,
                backgroundColor: colour,
                borderColor: colour,
                hoverBackgroundColor: colour,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });
    chart.update();
}