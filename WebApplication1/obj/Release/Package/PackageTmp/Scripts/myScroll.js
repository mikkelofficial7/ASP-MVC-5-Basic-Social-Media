$(window).scroll(function () {
    sessionStorage.scrollTop = $(this).scrollTop();
});

$(document).ready(function () {
    if (sessionStorage.scrollTop != "undefined") {
        $(window).scrollTop(sessionStorage.scrollTop);
    }
});

$(document).ready(function () {
    $('.linavbar').click(function () {
        sessionStorage.scrollTop = 0;
    });
});

function myFunction() {
    $("body").load();
}