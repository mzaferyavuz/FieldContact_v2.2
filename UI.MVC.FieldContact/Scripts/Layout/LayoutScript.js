$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/User/UserInfo?requestDate" + Date(),
        contentType: "aplication/json: charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#userNamePart").empty();
            $("#userNamePart").append(data.UserName);

        }
    })
});