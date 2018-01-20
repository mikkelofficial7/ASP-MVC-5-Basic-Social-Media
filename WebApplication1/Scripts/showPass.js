$(function () {
    $("#showPass").change(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $("#password").attr("type", "text");
            $("#password-confirm").attr("type", "text");
        } else {
            $("#password").attr("type", "password");
            $("#password-confirm").attr("type", "password");
        }
    });
});

$(document).ready(function () {
    /********** PASSWORD MATCH *************/
    function checkPasswordMatch() {
            var password = $("#password").val();
            var confirmPassword = document.getElementById("password-confirm").value;
            
            if (password != confirmPassword) {
                $("#match").css("display", "inline");
                $("#match").html("Not Match");
                $("#match").css('color', 'red');
                $("#match").css('float', 'left');
                $("#match").css('padding', '3px');
                $("#match").css('border-radius', '5px');
            }
            else if (password == "" || confirmPassword == "") {
                $("#match").css("display", "none");
            }
            else {
                $("#match").css("display", "inline");
                $("#match").html("Match");
                $("#match").css('color', 'blue');
                $("#match").css('float', 'left');
                $("#match").css('padding', '3px');
                $("#match").css('border-radius', '5px');
            }
    }
    $("#password, #password-confirm").keyup(checkPasswordMatch);
});

$(".submit").click(function () {
    document.addEventListener('invalid', (function () {
        return function (e) {
            e.preventDefault();
            document.getElementById("nama1").focus();
            document.getElementById("notif-error").innerHTML = "Fields Cannot Be Empty!";
            $("#notif-error").css("color", "red");
        };
    })(), true);
});