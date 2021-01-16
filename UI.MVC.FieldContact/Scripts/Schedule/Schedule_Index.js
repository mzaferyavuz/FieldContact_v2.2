$(document).ready(function () {
    $("#listDate").off("click").on("click", function () {
        var selectedDate = $("#selectedDate").val();
        if (selectedDate == "") {
            alert("Lütfen Bir Tarih Seçiniz");
        }
        else {
            $("#partialDiv").removeClass("hidden");
            $(".newButton").removeClass("hidden");
            LoadDataTable();

        }

    })

    $("#saveChanges").off("click").on("click", function () {
        var selectedDate = $("#selectedDate").val();
        var data = {
            ID: $("#id").val(),
            EmployeeID: $(".employeeSelection").val(),
            JobID: $(".companySelection").val(),
            Date: selectedDate

        }

        RequestToServerJson("/Schedule/SchedulEdit?requestDate" + Date(), data, "Add", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $("#saveChangesEdit").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            JobID: $(".companyEditSelection").val()

        }

        RequestToServerJson("/Schedule/SchedulEdit?requestDate" + Date(), data, "Update", function () {
            LoadDataTable();
            $("#responsiveEdit").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
    });
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
        "columns": [{ data: "NameSurname", "bSortable": true },
        { data: "Santiye", "bSortable": false },
        { data: "İsiAtayan", "bSortable": false },
        {
            data: "Date", "bSortable": false,
            render: function (d) {
                var dateSplit = d.replace(")", "(").split("(");
                var dat = new Date();
                dat.setTime(dateSplit[1]);
                return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
            }
        },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Schedule/ScheduleTable?dateData=" + dateData + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Schedule/ProductOperation?requestDate" + Date(), data, "Remove", function () {
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
                    $("#nameSurname").val(data.NameSurname);
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