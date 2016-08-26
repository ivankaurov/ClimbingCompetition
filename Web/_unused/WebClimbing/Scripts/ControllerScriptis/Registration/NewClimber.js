function btnSubmit_Click() {
    if (confirm("Подать заявку?")) {
        $(".hide-when-edit").hide();
        $("#frm_ClimbersList").submit();
    }
}

function lnkNewClimber_Click() {
    var indx = parseInt($("#frm_NewClimber_Index").val());
    indx = indx + 1;
    $("#frm_NewClimber_Index").val(indx);
    $(".hide-when-edit").hide();
    $("#frm_NewClimber").submit();
}

function lnkNewClimber_Failed() {
    alert("Ошибка загрузки формы для нового участника");
}

function lnkNewClimber_Completed() {
    $(".hide-when-edit").show();
}