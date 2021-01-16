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
            Address: $("#address").val(),
            City: $("#city").val(),
            PostalCode: $("#postalCode").val(),
            PhoneNumber: $("#phoneNumber").val(),
            FaxNumber: $("#faxNumber").val(),
            EmailAddress: $("#emailAddress").val(),
            ProjectStart: $("#projectStart").val(),
            PlannedEndDate: $("#plannedEndDate").val(),
            EndDate: $("#commissioningDate").val()

        }

        RequestToServerJson("/Management/ProjectOperation?requestDate" + Date(), data, "AddOrUpdate", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $("#name").val('');
        $("#address").val('');
        $("#city").val('');
        $("#postalCode").val('');
        $("#phoneNumber").val('');
        $("#faxNumber").val('');
        $("#emailAddress").val('');
        $("#projectStart").val('');
        $("#plannedEndDate").val('');
        $("#commissioningDate").val('');
    });
})

var LoadDataTable = function () {
    var dt = $("#compList").DataTable();
    dt.destroy();

    var oTable = $("#compList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "Address", "bSortable": false },
        { data: "City", "bSortable": false },
        { data: "PostalCode", "bSortable": false },
        { data: "PhoneNumber", "bSortable": false },
        { data: "FaxNumber", "bSortable": false },
        { data: "EmailAddress", "bSortable": false },
        {
            data: "ProjectStart", "bSortable": false,
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
            data: "PlannedEndDate", "bSortable": false,
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
            data: "EndDate", "bSortable": false,
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
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Management/GetProjects?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Management/ProjectOperation?requestDate" + Date(), data, "Remove", function () {
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

                    var prStart = data.ProjectStart;
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
                    var plnEnd = data.PlannedEndDate;
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
                    var comDat = data.EndDate;
                    var dateSplit = comDat;
                    if (dateSplit === null) {
                        comDat = "";
                    }
                    else {
                        dateSplit = dateSplit.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        comDat = dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }


                    $("#id").val(data.ID);
                    $("#name").val(data.Name);
                    $("#address").val(data.Address);
                    $("#city").val(data.City);
                    $("#postalCode").val(data.PostalCode);
                    $("#phoneNumber").val(data.PhoneNumber);
                    $("#faxNumber").val(data.FaxNumber);
                    $("#emailAddress").val(data.EmailAddress);
                    $("#projectStart").val(prStart);
                    $("#plannedEndDate").val(plnEnd);
                    $("#commissioningDate").val(data.CommissioningDate);
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