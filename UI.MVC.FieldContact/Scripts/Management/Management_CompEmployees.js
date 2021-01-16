$(document).ready(function () {
    LoadDataTable();

    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            Name: $("#name").val(),
            LastName: $("#lastname").val(),
            Phone: $("#phone").val(),
            EmailAddress: $("#emailAdd").val(),
            CompanyID: $(".compSelection").val(),
            Title: $("#title").val()

        }

        RequestToServerJson("/Management/CompEmployeeOperation?requestDate" + Date(), data, "Add", function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });


    $("#saveChangesEdit").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            Name: $("#nameEdit").val(),
            LastName: $("#lastnameEdit").val(),
            Phone: $("#phoneEdit").val(),
            EmailAddress: $("#emailAddEdit").val(),
            Title: $("#titleEdit").val()

        }

        RequestToServerJson("/Management/CompEmployeeOperation?requestDate" + Date(), data, "Update", function () {
            LoadDataTable();
            $("#responsiveUpdate").modal("hide");
        }, function () {
            //başarısız
        });


    });


    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $("#name").val('');
        $("#lastname").val('');
        $("#phone").val('');
        $("#emailAdd").val('');
        $(".compSelection").val('');
        $("#title").val('');
    });
})


var LoadDataTable = function () {
    var dt = $("#compEmpList").DataTable();
    dt.destroy();

    var oTable = $("#compEmpList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "LastName", "bSortable": false },
        { data: "Phone", "bSortable": false },
        { data: "EmailAddress", "bSortable": false },
        { data: "EmployeeCompany", "bSortable": false },
        { data: "Title", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Management/GetCompEmployees?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Management/CompEmployeeOperation?requestDate" + Date(), data, "Remove", function () {
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
                    $("#nameEdit").val(data.Name);
                    $("#lastnameEdit").val(data.LastName);
                    $("#phoneEdit").val(data.Phone);
                    $("#emailAddEdit").val(data.EmailAddress);
                    $(".compSelection").val(data.EmployeeCompany);

                    $("#titleEdit").val(data.Title);

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