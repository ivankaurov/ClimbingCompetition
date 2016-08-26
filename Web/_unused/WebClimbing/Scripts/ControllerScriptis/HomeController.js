/// <reference path="..\_references.js />
$(
    function () {
        $('.date-edit').datepicker({ dateFormat: "dd.mm.yy" });
    }
);
function groupSaveCompleted() {
    $('#ajax_load').css('display', 'none');
}

function groupSaveFailed() {
    groupSaveCompleted();
    alert('Ошибка сохранения');
}