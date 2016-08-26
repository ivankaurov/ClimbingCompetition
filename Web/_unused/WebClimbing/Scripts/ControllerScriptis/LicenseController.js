/// <reference path="..\jquery-1.7.1-vsdoc.js" />
/// <reference path="..\jquery-ui-1.8.20.js" />
function searchFailed() {
    $('#search_results').text('Произошла ошибка выборки данных. Наши специалисты уже работают над этой проблемой');
}

function searchCompleted() {
    $('.date-edit').datepicker({ dateFormat: "dd.mm.yy" });
}

function updateFailed() {
    alert('Ошибка сохранения');
}