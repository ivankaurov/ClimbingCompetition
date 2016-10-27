function btnSubmit_Click() {
    if (confirm("Подать заявку?")) {
        SetLoadingState(true, false);
        $("#frm_ClimbersList").submit();
    }
}

function lnkNewClimber_Click() {
    var indx = parseInt($("#frm_NewClimber_Index").val());
    indx = indx + 1;
    $("#frm_NewClimber_Index").val(indx);
    SetLoadingState(true, true);
    $("#frm_NewClimber").submit();
}

function lnkNewClimber_Failed() {
    alert("Ошибка загрузки формы для нового участника");
}

function lnkNewClimber_Completed() {
    SetLoadingState(false);
}

function SetLoadingState(loading, disabled) {
    if (loading) {
        $('#div_loader').show();
        $(".hide-when-edit").hide();
        if (disabled) {
            $(".frm-editor").attr("disabled", "disabled");
        }
        else {
            $(".frm-editor").attr("readonly", "readonly");
        }
    }
    else {
        $('#div_loader').hide();
        $(".hide-when-edit").show();
        $(".frm-editor").removeAttr("disabled");
        $(".frm-editor").removeAttr("readonly");
    }
}