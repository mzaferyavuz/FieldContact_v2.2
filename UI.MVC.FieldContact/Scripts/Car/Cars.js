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
            CarBrand: $("#brand").val(),
            ActualKm: $("#actualKm").val()

        }

        RequestToServerJson("/Car/CarOperation?requestDate" + Date(), data, "AddOrUpdate", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

})

var LoadDataTable = function () {
    var dt = $("#carList").DataTable();
    dt.destroy();

    var oTable = $("#carList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "CarBrand", "bSortable": false },
            {
                data: "ContractDate", "bSortable": false,
                render: function (d) {
                    var dateSplit = d;
                    if (dateSplit === null) {
                        return ""
                    }
                    else {
                        dateSplit = d.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }

                }
            },
        { data: "ContractKm", "bSortable": false },
        { data: "ActualKm", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Car/GetCars?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);



                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    $("#id").val(data.ID);
                    $("#name").val(data.Name);
                    $("#brand").val(data.CarBrand);
                    $("#actualKm").val(data.ActualKm);
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