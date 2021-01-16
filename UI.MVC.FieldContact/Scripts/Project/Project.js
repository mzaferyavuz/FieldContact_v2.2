

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};


var selectedProjectID;
$(document).ready(function () {


    LoadAllProjects();
    $('#projSelect').change(function () {
        var selectValue = $('#projSelect').val();
        if (selectValue == "allProjects") {
            LoadAllProjects();
        }
        else if (selectValue == "currentProjects") {
            LoadCurrentProjects();
        }
        else if (selectValue == "finishedProjects") {
            LoadFinishedProjects();
        }
    });


    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            ProductID: $(".productSelection").val(),
            Delivered: $("#deliveredQuantity").val(),
            ContQuantity: $("#contQuantity").val(),
            OrderQuantity: $("#orderQuantity").val(),
            SalePrice: $("#salePrice").val(),
            CompanyID: selectedProjectID
        }

        RequestToServerJson("/Project/ProjectOrderOperation?requestDate" + Date(), data, "Add", function () {
            GetProjectDetails();
            $("#responsive").modal("hide");
            var selectValue = $('#projSelect').val();
            if (selectValue == "allProjects") {
                LoadAllProjects();
            }
            else if (selectValue == "currentProjects") {
                LoadCurrentProjects();
            }
            else if (selectValue == "finishedProjects") {
                LoadFinishedProjects();
            }
        }, function () {
            //başarısız
        });


    });

    $("#saveChangesEdit").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            Delivered: $("#deliveredQuantityEdit").val(),
            ContQuantity: $("#contQuantityEdit").val(),
            OrderQuantity: $("#orderQuantityEdit").val(),
            SalePrice: $("#salePriceEdit").val(),
            CompanyID: selectedProjectID
        }

        RequestToServerJson("/Project/ProjectOrderOperation?requestDate" + Date(), data, "Update", function () {
            GetProjectDetails();
            $("#responsiveEdit").modal("hide");
            var selectValue = $('#projSelect').val();
            if (selectValue == "allProjects") {
                LoadAllProjects();
            }
            else if (selectValue == "currentProjects") {
                LoadCurrentProjects();
            }
            else if (selectValue == "finishedProjects") {
                LoadFinishedProjects();
            }
        }, function () {
            //başarısız
        });


    });

});



var LoadAllProjects = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();


    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
            {
                data: "ProjectStart", "bSortable": true,
                render: function (d) {
                    var dateSplit = d.replace(")", "(").split("(");
                    var dat = new Date();
                    dat.setTime(dateSplit[1]);
                    return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                }
            },
            {
                data: "PlannedEndDate", "bSortable": true,
                render: function (d) {
                    var dateSplit = d.replace(")", "(").split("(");
                    var dat = new Date();
                    dat.setTime(dateSplit[1]);
                    return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                }
            },
            {
                data: "EndDate", "bSortable": true,
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
        { data: "Teslim", "bSortable": true },
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Project/GetAllProject?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.viewDetails').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    selectedProjectID = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });
            })
        }
    });
}


var LoadFinishedProjects = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();

    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
            {
                data: "ProjectStart", "bSortable": true,
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
                data: "PlannedEndDate", "bSortable": true,
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
                data: "EndDate", "bSortable": true,
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
        { data: "Teslim", "bSortable": true },
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Project/GetFinishedProjects?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.viewDetails').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    selectedProjectID = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });
                
            })
        }


    });
}


var LoadCurrentProjects = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();

    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
            {
                data: "ProjectStart", "bSortable": true,
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
                data: "PlannedEndDate", "bSortable": true,
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
                data: "EndDate", "bSortable": true,
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
        { data: "Teslim", "bSortable": true },
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Project/GetCurrentProjects?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.viewDetails').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    selectedProjectID = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });


                
            }).fail(function (e) {

            });
        }


    });
}


var GetProjectDetails = function () {
    var dt = $("#detailedTable").DataTable();
    dt.destroy();
    var oTable = $("#detailedTable").dataTable({
        "paging": false,
        "ordering": false,
        "info": true,
        "searching": true,
        "lengthMenu": [[-1], ["All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "ContQuantity", "bSortable": false },
        { data: "OrderQuantity", "bSortable": false },
        { data: "Delivered", "bSortable": false },
        { data: "Description", "bSortable": false },
        { data: "Cost", "bSortable": false },
        { data: "ContTotalCost", "bSortable": false },
        { data: "OrderTotalCost", "bSortable": false },
        { data: "SalePrice", "bSortable": false },
        { data: "ContTotalPrice", "bSortable": false },
        { data: "OrderTotalPrice", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Project/GetDetails?ID=" + selectedProjectID + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Project/ProjectOrderOperation?requestDate" + Date(), data, "Remove", function () {
                        GetProjectDetails();
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
                    $("#productSelected").val(data.Name)
                    $("#deliveredQuantityEdit").val(data.Delivered);
                    $("#contQuantityEdit").val(data.ContQuantity);
                    $("#orderQuantityEdit").val(data.OrderQuantity);
                    $("#salePriceEdit").val(data.SalePrice);
                });


            }).fail(function (e) {

            });
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