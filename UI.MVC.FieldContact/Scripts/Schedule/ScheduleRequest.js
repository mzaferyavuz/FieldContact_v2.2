$(document).ready(function () {





    $("#sendReport").off('click').on('click', function () {

        var compID = $(".getCompany").val()
        var selectedDate = $("#selectedDate").val()
        var data = {
            JobID: compID,
            Date: selectedDate
        }

        RequestToServerJson("/Schedule/SaveScheduleRequest?requestDate=" + Date(), data, function () {
            $.get("/Schedule/ScheduleRequst?requestDate=" + Date(), function () {

            }, function () {

            })
        }, function () {

        })







    })
});




var RequestToServerJson = function (url, jsonData, successFunction, failFunction) {
    try {
        App.blockUI({ target: "body", boxed: true, zIndex: 9999, message: "yükleniyor" })
        $.post(url, { Data: jsonData }, function (data, textStatus, jqXHR) {
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

