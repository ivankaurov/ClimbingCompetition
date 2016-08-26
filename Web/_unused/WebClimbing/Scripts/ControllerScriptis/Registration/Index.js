function ListRefreshStarted() {
    $("#div_loader").show();
    $(".hide-when-edit").hide();
    $("#newClimberRegion").val($("#regionId").val());
}

function ListRefreshCompleted() {
    $("#div_loader").hide();
    $(".hide-when-edit").show();
}

function ListRefreshFailed() {
    alert("Ошибка загрузки списка участников.");
}

function btnEditClimber_Click(climberId) {
    var compId = $("#compId").val();
    var loaderSelector = "#editorLoader" + climberId;
    var spacerSelector = "#editorSpacer" + climberId;

    $(loaderSelector).show();
    $(".hide-when-edit").hide();

    var params = { compId: compId, index: 1, appId: climberId };
    $.get("/Registration/EditorForm", params, function (data, textStatus, jqXHR) {
        $(loaderSelector).hide();
        if (textStatus != "success") {
            alert("Ошибка загрузки заявки");
            $(".hide-when-edit").show();
            return;
        }
    }, "html");
}
function btnEditClimber_Click_Completed()
{
}