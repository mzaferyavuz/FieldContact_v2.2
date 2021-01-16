var formID;
$(document).ready(function () {
    LoadForms();
    $('.viewDetails').off('click').on('click', function () {
        alert();
    });


})

var GetServiceForm = function () {
    $.ajax({
        type: "GET",
        url: "/Report/GetServiceForm?ID=" + formID + "&requestDate=" + Date(),
        contentType: "aplication/json: charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#companyName").empty();
            $("#companyName").append(data.CompanyName);
            $("#companyAdress").empty();
            $("#companyAdress").append(data.CompAddress);
            $("#coEmp1").empty();
            $("#coEmp1").append(data.CoEmp1);
            $("#coEmp2").empty();
            $("#coEmp2").append(data.CoEmp1);
            $("#startDate").val(data.StartDate);
            $("#endDate").val(data.EndDate);
            $("input[name='HVAC']").prop("checked", data.HVAC);
            $("input[name='toplanti']").prop("checked", data.IsMeeting);
            $("input[name='ucretli']").prop("checked", !(data.IsFree));
            $("input[name='ucretsiz']").prop("checked", data.IsFree);
            $("input[name='arizaServis']").prop("checked", data.IsArıza);
            $("input[name='supervision']").prop("checked", data.IsSupervice);
            $("input[name='devreyeAlma']").prop("checked", data.IsDevreyeAlma);
            $("input[name='educat']").prop("checked", data.IsEducate);
            $("input[name='montaj']").prop("checked", data.IsMontage);
            $("input[name='bakim']").prop("checked", data.IsBakim);
            $("input[name='bakim1']").prop("checked", data.IsFirsManint);
            $("input[name='bakim2']").prop("checked", data.IsSecondMaint);
            $("input[name='bakim3']").prop("checked", data.IsThirdMaint);
            $("input[name='teslim']").prop("checked", data.IsDelivering);
            $("input[name='warrantied']").prop("checked", data.IsWarrantied);
            $("#jobDesc").val(data.JobDescription);
            $("#serviceDetails").val(data.ServiceDetails);


            $("#employeeId1").empty();
            $("#employeeId1").append(data)
            $('.serviceFormtoShow').removeClass("hidden");
        }
    });
}

var LoadForms = function () {
    var dt = $("#serviceReports").DataTable();
    dt.destroy();

    var oTable = $("#serviceReports").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "CompanyName", "bSortable": true },
        { data: "EmployeeName", "bSortable": true },
        { data: "StarttDate", "bSortable": true },
        { data: "EnddDate", "bSortable": true },
        { data: "ServisTipi", "bSortable": true },
        { data: "View", "bSortable": false }],
        "sAjaxSource": "/Report/GetForms?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.viewDetails').off('click').on('click', function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;
                    formID = data.ID;
                    //$.ajax({
                    //    type: "GET",
                    //    url: "/Report/GetServiceForm?ID=" + formID + "&requestDate=" + Date(),
                    //    contentType: "aplication/json: charset=utf-8",
                    //    dataType: "json",
                    //    success: function (data) {
                    //        $("#employeeId1").empty();
                    //        $("#employeeId1").append(data)
                    //    },
                    //    fail: function (data) {

                    //    }
                    //})
                   
                    GetServiceForm();

                });
                $(".viewButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson()
                })
            })
        }


    });
}


var RequestToServerJson = function (url, id, successFunction, failFunction) {
    try {
        $.get(url, id, function (data, status, jqXHR) {

        }, "json").fail(function (e) {

        }).always(function () {
            failFunction();
        });

    } catch (e) {

    }
}