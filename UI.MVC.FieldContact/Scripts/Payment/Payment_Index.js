var selectedID;
var selectedActivityID;

$(document).ready(function () {
    var idCheck = $("#pageRequesterId").val();
    if (idCheck == 1) {
        $("#employeeSelection").removeClass("hidden");
    }
    else {
        $("#activityLog").removeClass("hidden");
        selectedID = 0;
        $("#selEmpId").val = 0;
        GetPaymentActivities();
    }

    $(".getDetails").off("click").on("click", function () {
        $("#activityLog").removeClass("hidden");
        selectedID = $(".empSelection").val();
        $("#selEmpId").val = $(".empSelection").val();
        GetPaymentActivities();
    })
    

    $("#saveChanges").off("click").on("click", function () {
        var Data = {
            ID: $("#id").val(),
            ActiviyTypeID: $(".activityTypeSelection").val(),
            ActivityOwnerID: $("#selEmpId").val()
        }

        RequestToServerJson("/Payment/ActivityOperation?requestDate" + Date(), Data, "Add", function () {
            GetPaymentActivities();
            
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });


    $("#saveBill").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            PaymentActivityID: selectedActivityID,
            PaymentDate: $("#payDate").val(),
            Amount: $("#payAmount").val(),
            Description: $("#description").val()
        }

        RequestToServerJson("/Payment/PaymentActivityOperation?requestDate" + Date(), data, "Add", function () {
            GetActivityDetails();
            GetPaymentActivities();
            $("#responsive2").modal("hide");
        }, function () {
            //başarısız
        });


    });

})


var GetPaymentActivities = function () {
    var dt = $("#detailedTable").DataTable();
    dt.destroy();

    var oTable = $("#detailedTable").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [
            {
                data: "Date", "bSortable": true,
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
            { data: "ActivityTypeName", "bSortable": false },
            { data: "Tutar", "bSortable": false },
            { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Payment/GetPaymentActivities?ID=" + selectedID + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                  $("#bakiyeShow").empty();
                  $("#bakiyeShow").append(data.dtBakiye);
                
                
                $('.viewDetails').off('click').on('click', function () {

                    $("#activityDetail").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    selectedActivityID = id;

                    $("#activityDetail").removeClass("hidden");
                    GetActivityDetails();
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

var GetActivityDetails = function () {
    var dt = $("#paymentTable").DataTable();
    dt.destroy();

    var oTable = $("#paymentTable").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [
            {
                data: "PaymentDate", "bSortable": true,
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
            { data: "Description", "bSortable": false },
            { data: "Amount", "bSortable": false }],
        "sAjaxSource": "/Payment/GetActivityDetails?ID=" + selectedActivityID + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                //$('.viewDetails').off('click').on('click', function () {

                //    $("#activityDetail").removeClass("hidden");

                //    var row = $(this).closest("tr");
                //    var data = oTable.fnGetData(row);
                //    var id = data.ID;

                //    selectedActivityID = id;

                //    $("#activityDetail").removeClass("hidden");
                //    GetProjectDetails();
                //});



            }).fail(function (e) {

            });
        }


    });
}