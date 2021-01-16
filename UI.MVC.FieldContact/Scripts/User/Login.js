$(document).ready(function () {
    $("#tryLogin").off("click").on("click", function () {
        var userName = $('input[name=username]').val()
        var password = $('input[name=password]').val()

        $.post("/User/Login?requestDate" + Date(), { "userName": userName, "password": password })
    })

    $("#sendEmail").off("click").on("click", function () {
        var mailAddress = $("#mailAddress").val();

        RequestToServerJson("/User/ForgotPassword?mailAddress=" + mailAddress + "&requestDate=" + Date(), "", "", function () {

        }, function () {

        });
    })

});



var RequestToServerJson = function (url, jsonData, operation, successFunction, failFunction) {
    try {
        App.blockUI({ target: "body", boxed: true, zIndex: 9999, message: "yükleniyor" })
        $.post(url, { OperationType: operation, Data: jsonData }, function (data, textStatus, jqXHR) {
            if (data.Status === 1) {
                toastr.success(data.Message, "İşlem")

                successFunction();
            }
            if (data.Status === 2) {
                toastr.error(data.Message, "İşlem")
            }
        }, "json").fail(function (e) {
            failFunction();
        }).always(function () {
            window.setTimeout(function () {
                App.unblockUI();
            }, 1000);
        })
    } catch (e) {

    }
}