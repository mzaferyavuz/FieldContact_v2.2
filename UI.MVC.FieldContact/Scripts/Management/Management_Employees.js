$(document).ready(function () {
    LoadDataTable();

    $("#saveChanges").off("click").on("click", function () {
        var Auth = new Array();
        $.each($("input[name='authBoxes']:checked"), function () {
            Auth.push($(this).val());
        });

        var opType = $('#opType').val();

        var data = {
            ID: $("#id").val(),
            Name: $("#name").val(),
            LastName: $("#lastname").val(),
            HireDate: $("#hireDate").val(),
            BirthDate: $("#birthDate").val(),
            PersonelPhone: $("#personalPhone").val(),
            CompPhone: $("#compPhone").val(),
            Salary: $("#salary").val(),
            Email: $("#position").val(),
            Auth: Auth
        }

        RequestToServerJson("/Management/EmployeeOperation?requestDate" + Date(), data, opType, function () {
            LoadDataTable();
            $("#responsive").modal("hide");
        }, function () {
            //başarısız
        });


    });

    $(".newButton").off("click").on("click", function () {
        $("#id").val(0);
        $('#opType').val('Add');
        $("#name").val('');
        $("#lastname").val('');
        $("#hireDate").val('');
        $("#birthDate").val('');
        $("#personalPhone").val('');
        $("#compPhone").val('');
        $("#salary").val('');
        $("#position").val('');
    });
})


var LoadDataTable = function () {
    var dt = $("#empList").DataTable();
    dt.destroy();

    var oTable = $("#empList").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "Name", "bSortable": true },
        { data: "LastName", "bSortable": false },
        {
            data: "HireDate", "bSortable": false,
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
            data: "BirthDate", "bSortable": false,
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
        { data: "PersonelPhone", "bSortable": false },
        { data: "CompPhone", "bSortable": false },
        { data: "Salary", "bSortable": false },
        { data: "Email", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Management/GetEmployees?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Management/EmployeeOperation?requestDate" + Date(), data, "Remove", function () {
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

                    var birth = data.BirthDate;
                    var dateSplit = birth;
                    if (dateSplit === null) {
                        birth = "";
                    }
                    else {
                        dateSplit = dateSplit.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        birth = dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }
                    var hire = data.HireDate;
                    var dateSplit = hire;
                    if (dateSplit === null) {
                        hire = "";
                    }
                    else {
                        dateSplit = dateSplit.replace(")", "(").split("(");
                        var dat = new Date();
                        dat.setTime(dateSplit[1]);
                        hire = dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
                    }


                    $("#id").val(data.ID);
                    $('#opType').val('Update');
                    $("#name").val(data.Name);
                    $("#lastname").val(data.LastName);
                    $("#hireDate").val(hire);
                    $("#birthDate").val(birth);
                    $("#personalPhone").val(data.PersonelPhone);
                    $("#compPhone").val(data.CompPhone);
                    $("#salary").val(data.Salary);
                    $("#position").val(data.Email);
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