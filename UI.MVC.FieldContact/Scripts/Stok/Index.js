$(document).ready(function () {



    LoadDataTable();


    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            UnitsInStock: $("#unitsInStock").val()

        }

        RequestToServerJson("/Stok/ProductOperation?requestDate" + Date(), data, "AddOrUpdate", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });


})

var LoadDataTable = function () {
    var dt = $("#prodList").DataTable();
    dt.destroy();

    var oTable = $("#prodList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "Description", "bSortable": false },
        { data: "UnitsInStock", "bSortable": false },
        { data: "UnitsOrder", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Stok/GetProducts?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);



                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    $("#id").val(data.ID);
                    $("#name").val(data.Name);
                    $("#description").val(data.Description);
                    $("#unitsInStock").val(data.UnitsInStock);
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
                LoadDataTable();
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