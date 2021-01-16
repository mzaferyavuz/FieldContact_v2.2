var jobId;
$(document).ready(function () {
    $("#getPropEmp").off("click").on("click", function () {
        var dataText = $("#dateChoose").val();
        $.ajax({
            type: "GET",
            url: "/Job/AvaliablePersonel?date=" + dataText + "&requestDate=" + Date(),
            contentType: "aplication/json: charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('.empSelection').prop("disabled", false);
                var dt = $(".empSelection").select2();
                dt.empty();
                var otable = $(".empSelection").select2({
                    placeholder: "Çalışan Seçimi",
                    "data": data.aaData,
                    allowClear: true
                });
            }
        });
    });

    $(".newButton").off("click").on("click", function () {
        $("#opType").val("Add")
    });

    $(".newButton2").off("click").on("click", function () {
        $("#opType2").val("Add")
    });
    
    $(".updateButton").off("click").on("click", function () {
        $("#opType").val("Update")
    });

    $(".updateButton2").off("click").on("click", function () {
        $("#opType2").val("Update")
    });

    LoadCurrentJobs();
    $('#projSelect').change(function () {
        var selectValue = $('#projSelect').val();
        if (selectValue == "allProjects") {
            LoadAllJobs();
        }
        else if (selectValue == "currentProjects") {
            LoadCurrentJobs();
        }
        else if (selectValue == "finishedProjects") {
            LoadFinishedJobs();
        }
    });

    $("#saveChanges").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            DatetoFinish: $("#dateToFinish").val(),
            EstimatedWorkForce: $("#estimatedMan").val(),
            JobDescrition: $("#jobDescription").val(),
            //JobEndDate: $("#orderQuantity").val(),
            JobName: $("#jobName").val(),
            JobType: $(".jobType").val(),
            Priority: $(".prioritySelection").val(),
            CompanyID: $(".firmSelection").val(),
        }

        var operType = $("#opType").val();
        if (operType == "Add") {

            RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Add", function () {

                $("#responsive").modal("hide");
                var selectValue = $('#projSelect').val();
                if (selectValue == "allProjects") {
                    LoadAllJobs();
                }
                else if (selectValue == "currentProjects") {
                    LoadCurrentJobs();
                }
                else if (selectValue == "finishedProjects") {
                    LoadFinishedJobs();
                }
            }, function () {
                //başarısız
            });
        }
        else if (operType == "Update") {

            RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Update", function () {

                $("#responsive").modal("hide");
                var selectValue = $('#projSelect').val();
                if (selectValue == "allProjects") {
                    LoadAllJobs();
                }
                else if (selectValue == "currentProjects") {
                    LoadCurrentJobs();
                }
                else if (selectValue == "finishedProjects") {
                    LoadFinishedJobs();
                }
            }, function () {
                //başarısız
            });
        }



    });



    $("#saveChanges2").off("click").on("click", function () {
        var data = {
            ID: $("#id").val(),
            EmployeeID: $(".empSelection").val(),
            Date: $("#dateChoose").val(),
            JobID: jobId,
        }

        var operType = $("#opType2").val();
        if (operType == "Add") {

            RequestToServerJson("/Job/JobScheduleOperation?requestDate" + Date(), data, "Add", function () {

                $("#responsive2").modal("hide");
                GetProjectDetails();
            }, function () {
                //başarısız
            });
        }
        else if (operType == "Update") {

            RequestToServerJson("/Job/JobScheduleOperation?requestDate" + Date(), data, "Update", function () {

                $("#responsive2").modal("hide");
                GetProjectDetails();
            }, function () {
                //başarısız
            });
        }



    });


})


var LoadAllJobs = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();


    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "JobName", "bSortable": true },
        { data: "Priority", "bSortable": true },
        { data: "JobDescrition", "bSortable": true },
        { data: "CompanyName", "bSortable": true },
        { data: "EstimatedWorkForce", "bSortable": true },
        { data: "UsedWorkForce", "bSortable": true },
        { data: "JobType", "bSortable": true },
        { data: "CreatorName", "bSortable": true },
        {
            data: "JobCreationDate", "bSortable": true,
            render: function (d) {
                var dateSplit = d.replace(")", "(").split("(");
                var dat = new Date();
                dat.setTime(dateSplit[1]);
                return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
            }
        },
        {
            data: "DatetoFinish", "bSortable": true,
            render: function (d) {
                var dateSplit = d.replace(")", "(").split("(");
                var dat = new Date();
                dat.setTime(dateSplit[1]);
                return dat.getDate() + "/" + (dat.getMonth() + 1) + "/" + dat.getFullYear();
            }
        },
        {
            data: "JobEndDate", "bSortable": true,
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
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Job/GetAllJobs?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.assignButton').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    jobId = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });

                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    $("#id").val(data.ID);
                    $("#opType").val("Update")
                    $("#jobName").val(data.JobName);
                    $(".prioritySelection").val(data.Priorty);
                    $("#jobDescription").val(data.JobDescrition);
                    $(".firmSelection").val(data.CompanyName);
                    $("#estimatedMan").val(data.EstimatedWorkForce);
                    $(".jobType").val(data.JobType);
                    $("#datetoFinish").val(data.DatetoFinish);
                });

                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Remove", function () {

                        var selectValue = $('#projSelect').val();
                        if (selectValue == "allProjects") {
                            LoadAllJobs();
                        }
                        else if (selectValue == "currentProjects") {
                            LoadCurrentProjects();
                        }
                        else if (selectValue == "finishedProjects") {
                            LoadFinishedProjects();
                        }
                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });

                $(".approveButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Done", function () {

                        //başarılı

                    }, function () {
                        //başarısız
                    });
                });

            })
        }
    });
}


var LoadFinishedJobs = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();

    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "JobName", "bSortable": true },
        { data: "Priority", "bSortable": true },
        { data: "JobDescrition", "bSortable": true },
        { data: "CompanyName", "bSortable": true },
        { data: "EstimatedWorkForce", "bSortable": true },
        { data: "UsedWorkForce", "bSortable": true },
        { data: "JobType", "bSortable": true },
        { data: "CreatorName", "bSortable": true },
        {
            data: "JobCreationDate", "bSortable": true,
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
            data: "DatetoFinish", "bSortable": true,
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
            data: "JobEndDate", "bSortable": true,
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
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Job/GetFinishedJobs?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.assignButton').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    jobId = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });

                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    $("#id").val(data.ID);
                    $("#opType").val("Update")
                    $("#jobName").val(data.JobName)
                    $(".prioritySelection").val(data.Priorty);
                    $("#jobDescription").val(data.JobDescrition);
                    $(".firmSelection").val(data.CompanyName);
                    $("#estimatedMan").val(data.EstimatedWorkForce);
                    $(".jobType").val(data.JobType);
                    $("#datetoFinish").val(data.DatetoFinish);
                });


                $(".deleteButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Remove", function () {

                        var selectValue = $('#projSelect').val();
                        if (selectValue == "allProjects") {
                            LoadAllJobs();
                        }
                        else if (selectValue == "currentProjects") {
                            LoadCurrentJobs();
                        }
                        else if (selectValue == "finishedProjects") {
                            LoadFinishedJobs();
                        }
                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });

                $(".approveButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Done", function () {

                        //başarılı

                    }, function () {
                        //başarısız
                    });
                });

            })
        }


    });
}


var LoadCurrentJobs = function () {
    var dt = $("#projectDetails").DataTable();
    dt.destroy();

    var oTable = $("#projectDetails").dataTable({
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[5, 25, 50, -1], [5, 25, 50, "All"]],
        "serverSide": true,
        "columns": [{ data: "JobName", "bSortable": true },
        { data: "Priority", "bSortable": true },
        { data: "JobDescrition", "bSortable": true },
        { data: "CompanyName", "bSortable": true },
        { data: "EstimatedWorkForce", "bSortable": true },
        { data: "UsedWorkForce", "bSortable": true },
        { data: "JobType", "bSortable": true },
        { data: "CreatorName", "bSortable": true },
        {
            data: "JobCreationDate", "bSortable": true,
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
            data: "DatetoFinish", "bSortable": true,
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
            data: "JobEndDate", "bSortable": true,
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
        { data: "Detaylar", "bSortable": false }],
        "sAjaxSource": "/Job/GetCurrentJobs?" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);
                $('.assignButton').off('click').on('click', function () {

                    $("#projectDetailDiv").removeClass("hidden");

                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    var id = data.ID;

                    jobId = id;

                    $("#projectDetailDiv").removeClass("hidden");
                    GetProjectDetails();
                });

                $(".updateButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    $("#id").val(data.ID);
                    $("#opType").val("Update")
                    $("#jobName").val(data.JobName)
                    $(".prioritySelection").val(data.Priorty);
                    $("#jobDescription").val(data.JobDescrition);
                    $(".firmSelection").val(data.CompanyName);
                    $("#estimatedMan").val(data.EstimatedWorkForce);
                    $(".jobType").val(data.JobType);
                    $("#datetoFinish").val(data.DatetoFinish);
                });




                $('.deleteButton').off('click').on('click', function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Remove", function () {

                        var selectValue = $('#projSelect').val();
                        if (selectValue == "allProjects") {
                            LoadAllJobs();
                        }
                        else if (selectValue == "currentProjects") {
                            LoadCurrentJobs();
                        }
                        else if (selectValue == "finishedProjects") {
                            LoadFinishedJobs();
                        }
                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });

                $(".approveButton").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Detaylar = null;

                    RequestToServerJson("/Job/JobOperation?requestDate" + Date(), data, "Done", function () {

                        //başarılı

                    }, function () {
                        //başarısız
                    });
                });
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
        "columns": [{ data: "JobName", "bSortable": true },
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
        { data: "EmployeeName", "bSortable": false },
        { data: "Atender", "bSortable": false },
        { data: "Operation", "bSortable": false }],
        "sAjaxSource": "/Job/GetDetails?ID=" + jobId + "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);

                $(".deleteButton2").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    RequestToServerJson("/Job/JobScheduleOperation?requestDate" + Date(), data, "Remove", function () {
                        GetProjectDetails();
                        //başarılı
                    }, function () {
                        //başarısız
                    });
                });

                $(".updateButton2").off("click").on("click", function () {
                    var row = $(this).closest("tr");
                    var data = oTable.fnGetData(row);
                    data.Operation = null;

                    $("#opType2").val("Update");
                    $("#id").val(data.ID);
                    $("#dateChoose").val(data.Date);
                    $("#empSelection").val(data.EmployeeName);
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