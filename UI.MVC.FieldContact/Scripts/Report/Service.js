$(document).ready(function () {
    $(".companySelection").select2({
        placeholder: "Firma Secimi",
        allowClear: true
    });
    $(".companySelection").change(function () {
        var selectedID = $(".companySelection").val()

        $.ajax({
            type: "GET",
            url: "/Report/CompanySelection?ID=" + selectedID + "&requestDate=" + Date(),
            contentType: "aplication/json: charset=utf-8",
            dataType: "json",
            success: function (data) {

                $("#companyAdress").empty();
                $("#companyAdress").append(data.Address);
                $('#companyEmployee1').prop("disabled", false);
                $('.companyEmployee2').prop("disabled", false);
                

                
                var loadedData;
                var dt = $("#companyEmployee1").select2();
                dt.empty();
                var dt2 = $("#companyEmployee2").select2();
                dt2.empty();
                var otable = $("#companyEmployee1").select2({
                    placeholder:"Yetkili Seçimi",
                    "data": data.aaData,
                    allowClear: true

                    
                });
                var otable = $("#companyEmployee2").select2({
                    placeholder: "Yetkili Seçimi",
                    "data": data.aaData,
                    allowClear: true


                });

                

            },
            fail: function(data) {

            }
            
        })
        $("#companyEmployee1").change(function () {
            var empId = $("#companyEmployee1").val()
            $.ajax({
                type: "GET",
                url: "/Report/CompEmpSelection?ID=" + empId + "&requestDate=" + Date(),
                contentType: "aplication/json: charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#employeeId1").empty();
                    $("#employeeId1").append(data)
                }
            })
        })
        $("#companyEmployee2").change(function () {
            var empId = $("#companyEmployee2").val()
            $.ajax({
                type: "GET",
                url: "/Report/CompEmpSelection?ID=" + empId + "&requestDate=" + Date(),
                contentType: "aplication/json: charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#employeeId2").empty();
                    $("#employeeId2").append(data)
                }
            })
        })
    })

    $("#saveForm").off('click').on('click', function () {
        var ariza = ($("input[name='arizaServis']").is(":checked")) ? "true" : "false";
        var bakim = ($("input[name='bakim']").is(":checked")) ? "true" : "false";
        var delivering = ($("input[name='teslim']").is(":checked")) ? "true" : "false";
        var devreyeAlma = ($("input[name='devreyeAlma']").is(":checked")) ? "true" : "false";
        var educate = ($("input[name='educat']").is(":checked")) ? "true" : "false";
        var firsMaint = ($("input[name='bakim1']").is(":checked")) ? "true" : "false";
        var isFree = ($("input[name='ucretli']").is(":checked")) ? "true" : "false";
        var meeting = ($("input[name='toplanti']").is(":checked")) ? "true" : "false";
        var secondMaint = ($("input[name='bakim2']").is(":checked")) ? "true" : "false";
        var supervice = ($("input[name='supervision']").is(":checked")) ? "true" : "false";
        var thirdMaint = ($("input[name='bakim3']").is(":checked")) ? "true" : "false";
        var warrantied = ($("input[name='warrantied']").is(":checked")) ? "true" : "false";
        var havac = ($("input[name='HVAC']").is(":checked")) ? "true" : "false";
        var montage = ($("input[name='montaj']").is(":checked")) ? "true" : "false";
        var compId = $(".companySelection").val()
        var data = {
            HVAC: havac,
            IsMontage: montage,
            IsArıza: ariza,
            IsBakım: bakim,
            IsDelivering: delivering,
            IsDevreyeAlma: devreyeAlma,
            IsEducate: educate,
            IsFirsManint: firsMaint,
            IsFree: isFree,
            IsMeeting: meeting,
            IsSecondMaint: secondMaint,
            IsSupervice: supervice,
            IsThirdMaint: thirdMaint,
            IsWarrantied: warrantied,
            CompanyID: compId,
            JobDescription: $("#jobDesc").val(),
            ServiceDetails: $("#serviceDetails").val(),
            StartDate: $("#startDate").val(),
            EndDate: $("#endDate").val()
        }

        RequestToServerJson("/Report/SaveServiceForm?requestDate=" + Date(), data, function () {

        }, function () {

        });
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