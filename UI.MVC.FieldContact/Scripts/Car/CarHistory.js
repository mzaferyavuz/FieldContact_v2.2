﻿$(document).ready(function () {

    LoadDataTable();

    $("#listDate").off('click').on('click', function () {
        LoadDataTable();

    })





})

var LoadDataTable = function () {

    var dateData = $("#selectedDate").val()


    var dt = $("#prodList").DataTable();
    dt.destroy();

    var oTable = $("#prodList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "CarName", "bSortable": false },
        { data: "Brand", "bSortable": false },
        { data: "Name", "bSortable": false }],
        "sAjaxSource": "/Car/PastCarHistory?dateData=" + dateData + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $('.viewDetails').off('click').on('click', function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    RequestToServerJson("/Report/ShowOnMap?requestDate" + Date(), data, "Show", function () {

                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });


            }).fail(function (e) {

            })
        }
    });



}

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