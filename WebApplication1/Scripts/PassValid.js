$(document).ready(function () {
    /********** PASSWORD MATCH *************/
    function checkPasswordMatch() {
        var password = $("#password").val();
        var confirmPassword = document.getElementById("password-confirm").value;

        if (password != confirmPassword) {
            $("#password").css("border", "1px solid red");
            $("#password-confirm").css("border", "1px solid red");
        }
        else if (password == "" || confirmPassword == "") {
            $("#password").css("border", "1px solid #9ca6b7");
            $("#password-confirm").css("border", "1px solid #9ca6b7");
        }
        else {
            $("#password").css("border", "1px solid #9ca6b7");
            $("#password-confirm").css("border", "1px solid #9ca6b7");
        }
    }
    $("#password, #password-confirm").keyup(checkPasswordMatch);
});