$(document).ready(function () {

    //$("#projectStart").inputmask("d/m/y", {
    //    "placeholder": "dd/mm/yyyy"
    //});
    //$("#plannedEndDate").inputmask("d/m/y", {
    //    "placeholder": "dd/mm/yyyy"
    //});
    //$("#commissioningDate").inputmask("d/m/y", {
    //    "placeholder": "dd/mm/yyyy"
    //});

    LoadDataTable();


    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            Name: $("#name").val(),
            Cost: $("#cost").val(),
            Description: $("#description").val(),
            UnitsInStock: $("#unitsInStock").val(),
            UnitsInOrder: $("#unitsInOrder").val()

        }

        RequestToServerJson("/Management/ProductOperation?requestDate" + Date(), data, "AddOrUpdate", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $("#name").val('');
        $("#cost").val('');
        $("#description").val('');
        $("#unitsInStock").val('');
        $("#unitsInOrder").val('');
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
        { data: "Cost", "bSortable": false },
        { data: "Description", "bSortable": false },
        { data: "UnitsInStock", "bSortable": false },
        { data: "UnitsOrder", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Management/GetProducts?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Management/ProductOperation?requestDate" + Date(), data, "Remove", function () {
                        LoadDataTable();
                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });

                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    $("#id").val(data.ID);
                    $("#name").val(data.Name);
                    $("#cost").val(data.Address);
                    $("#description").val(data.City);
                    $("#unitsInStock").val(data.PostalCode);
                    $("#unitsInOrder").val(data.PhoneNumber);
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