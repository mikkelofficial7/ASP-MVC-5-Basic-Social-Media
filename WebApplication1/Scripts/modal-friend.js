﻿// Get the modal
var modal = document.getElementById('myModal');

// Get the button that opens the modal
var btn = document.getElementById("showprofile");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

var notif = document.getElementById("notif-error");

// When the user clicks on the button, open the modal 
btn.onclick = function () {
    modal.style.display = "block";
    document.addEventListener('invalid', (function () {
        return function (e) {
            e.preventDefault();
        };
    })(), true);
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}