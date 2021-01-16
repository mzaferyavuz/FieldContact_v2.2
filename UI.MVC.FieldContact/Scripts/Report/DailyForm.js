$(document).ready(function () {


    $('input[name=singleOrPlural]').change(function () {
        var prop = $('.getCar').prop("disabled");
        if (prop) {
            $('.getCar').prop("disabled", false);
        }
        else {
            $('.getCar').prop("disabled", true);
        }

    })


    $("#sendReport").off('click').on('click', function () {

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(showPosition);
        } else {
            $('#mapHolder').html = "Konum servisi desteklenmiyor.";
        }

        function showPosition(position) {
            lat = position.coords.latitude;
            lon = position.coords.longitude;
            //latlon = new google.maps.LatLng(lat, lon)

            var compID = $('.getCompany').val()
            var carID = $('.getCar').val()
            var descr = $('#description').val()
            var data = {
                JobID: compID,
                CarID: carID,
                Description: descr,
                EmployeeID: 1,
                Lati: JSON.stringify(lat),
                Longi: JSON.stringify(lon),
                Latitude: lat,
                Longitude: lon

            }

            RequestToServerJson("/Report/SaveDailyForm?requestDate" + Date(), data, function () {


            }, function () {
                //başarısız
            });
        }

        function showError(error) {
            switch (error.code) {
                case error.PERMISSION_DENIED:
                    $("#mapHolder").html = "Konum isteğine izin vermelisiniz."
                    break;
                case error.POSITION_UNAVAILABLE:
                    $("#mapHolder").html = "Konum bilgilisine ulaşılamadı.Tekrar deneyiniz."
                    break;
                case error.TIMEOUT:
                    $("#mapHolder").html = "Konum alma zaman aşımına uğradı.Tekrar deneyiniz."
                    break;
                case error.UNKNOWN_ERROR:
                    $("#mapHolder").html = "Bilinmeyen bir hata oluştu.Tekrar deneyiniz."
                    break;
            }
        }





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

