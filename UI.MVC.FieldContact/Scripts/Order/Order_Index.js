$(document).ready(function () {



    LoadDataTable();


    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            ProductID: $(".productSelection").val(),
            CompanyID: $(".companySelection").val(),
            Quantity: $("#quanytity").val()

        }

        RequestToServerJson("/Order/OrderRequestOperation?requestDate" + Date(), data, "Add", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $("#saveChangesEdit").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            Quantity: $("#quantityEdit").val()

        }

        RequestToServerJson("/Order/OrderRequestOperation?requestDate" + Date(), data, "Update", function () {
            LoadDataTable();
            $("#responsiveEdit").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $(".productSelection").val('');
        $(".companySelection").val('');
        $("#quanytity").val('');
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
        "columns": [{ data: "ProductName", "bSortable": true },
        { data: "CompanyName", "bSortable": false },
        { data: "Quantity", "bSortable": false },
        { data: "EmployeeName", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Order/GetOrderRequest?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Order/OrderRequestOperation?requestDate" + Date(), data, "Remove", function () {
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
                    $("#productItem").val(data.ProductName);
                    $("#companyItem").val(data.CompanyName);
                    $("#quantityEdit").val(data.Quantity);
                });

                $(".approveButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Order/OrderApproval?requestDate" + Date(), data, "Approve", function () {
                        LoadDataTable();
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