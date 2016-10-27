function ListRefreshStarted() {
    $('#newClimberRegion').val($('#regionId').val());
    SetLoadingState(true);
}

function SetLoadingState(loading) {
    if (loading) {
        $("#div_loader").show();
        $(".hide-when-edit").hide();
    }
    else {
        $("#div_loader").hide();
        $(".hide-when-edit").show();
    }
}

function ListRefreshCompleted() {
    SetLoadingState(false);
}

function ListRefreshFailed() {
    alert("Ошибка загрузки списка участников.");
}

function btnEditClimber_Click(climberId) {
    var compId = $("#compId").val();
    var formSelector = "#editorForm" + climberId;
    var loaderSelector = "#editorLoader" + climberId;
    var divSelector = "#editorDiv" + climberId;

    $(loaderSelector).show();
    $(".hide-when-edit").hide();

    var params = { compId: compId, index: 1, appId: climberId };
    $.get("/Registration/EditorForm", params, function (data, textStatus, jqXHR) {
        $(loaderSelector).hide();
        if (textStatus != "success") {
            $(".hide-when-edit").show();
            alert("Ошибка загрузки заявки");
            return;
        }
        $(formSelector).show();
        $(divSelector).append(data);
    }, "html");
}

function RefreshClimbersList() {
    var loadParams = {
        compId: $("#compId").val(),
        groupId: $("#groupId").val(),
        regionId: $("#regionId").val()
    };

    $("#div_clm_lst").load("/Registration/ClimbersList", loadParams, function (data, textStatus, jqXHR) {
        $(".hide-when-edit").show();
        if (textStatus != "success") {
            alert("Ошибка обновления списка участников");
        }
    });
}

function btnEditSubmit_Click(climberId) {
    if (!confirm("Подтвердите правку"))
        return;
    var f1 = $("#editorForm" + climberId).serialize();
    var messageDiv = "#messageDiv" + climberId;
    $(messageDiv).show();
    $.post("/Registration/SaveOneClimber", f1, function (data, textStatus, jqXHR) {
        $(messageDiv).hide();
        $(".frm-editor").removeAttr("disabled");
        if (textStatus != "success") {
            alert("Ошибка правки");
            return;
        }
        if (data.Error != "" && data.Error != undefined) {
            alert("Ошибка правки: " + data.Error);
            return;
        }
        alert("Заявка откорректирована.");
        RefreshClimbersList();
    });
    $(".frm-editor").attr("disabled", "disabled")
}


function btnCancel_Click(climberId) {
    $("#editorDiv" + climberId).empty();
    $("#editorForm" + climberId).hide();
    $(".hide-when-edit").show();
}

function btnDel_Click(climberId) {
    if (!confirm("Подтвердите удаление участника " + $("#surname_1").val() + " " + $("#name_1").val()))
        return;
    var param = { appId: climberId };
    var messageDivSelector = "#messageDiv" + climberId;
    $(".frm-editor").attr("disabled", "disabled");
    $(messageDivSelector).show();
    $.post("/Registration/DeleteApp", param, function (data, textStatus, jqXHR) {
        $(".frm-editor").removeAttr("disabled");
        if (textStatus != "success") {
            alert("Ошибка удаления");
            return;
        }
        if (data.Error != "" && data.Error != undefined) {
            alert("Ошибка удаления: " + data.Error);
            return;
        }

        $(messageDivSelector).hide();
        alert("Заявка успешно удалена");
        
        $("#row1_" + climberId).remove();
        $("#row2_" + climberId).remove();
        

        RefreshClimbersList();
    });
}