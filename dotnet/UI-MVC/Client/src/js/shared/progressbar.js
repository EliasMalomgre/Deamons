let bar = document.getElementById("progress");
let max = bar.getAttribute("aria-valuemax");
let current = bar.getAttribute("aria-valuenow");
bar.style.width= current/max*100+"%";