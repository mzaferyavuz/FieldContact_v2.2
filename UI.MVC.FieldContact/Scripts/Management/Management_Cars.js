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

        var hiredD = $("#hiredDate").val();
        var contractD = $("#contactDate").val();
        var data = {
            ID: $("#id").val(),
            Name: $("#name").val(),
            CarBrand: $("#brand").val(),
            HiredDate: $("#hiredDate").val(),
            ContractDate: $("#contactDate").val(),
            HiredKm: $("#hiredKm").val(),
            ContractKm: $("#contractKm").val(),
            ActualKm: $("#actualKm").val()

        }

        RequestToServerJson("/Management/CarOperation?requestDate" + Date(), data, "AddOrUpdate", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $("#name").val('');
        $("#brand").val('');
        $("#hiredDate").val('');
        $("#contactDate").val('');
        $("#hiredKm").val('');
        $("#contractKm").val('');
        $("#actualKm").val('');
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
            data: "HiredDate", "bSortable": false,
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
        { data: "HiredKm", "bSortable": false },
        { data: "ContractKm", "bSortable": false },
        { data: "ActualKm", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Management/GetCars?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Management/CarOperation?requestDate" + Date(), data, "Remove", function () {
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

                    var prStart = data.HiredDate;
                    var dateSplit = prStart;
                    if (dateSplit === null) {
                        prStart = "";
                    }
                    else {
                        dateSplit = dateSplit.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        prStart = dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }
                    var plnEnd = data.ContractDate;
                    var dateSplit = plnEnd;
                    if (dateSplit === null) {
                        plnEnd = "";
                    }
                    else {
                        dateSplit = dateSplit.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        plnEnd = dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }

                    $("#id").val(data.ID);
                    $("#name").val(data.Name);
                    $("#brand").val(data.CarBrand);
                    $("#hiredDate").val(prStart);
                    $("#contactDate").val(plnEnd);
                    $("#hiredKm").val(data.HiredKm);
                    $("#contractKm").val(data.ContractKm);
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