/// <reference path="..\jquery-1.7.1-vsdoc.js" />
/// <reference path="..\_references.js" />

// кнопка "Новая заявка Applications.cshtml"
function addClimbersClick() {
    $('#appsTable').hide(); //уберем список спортсменов
    $('#dataLoading').show();
    $('.hide-when-edit').hide(); //скроем кнопки "правка" и "новая заявка"
    var compId = $('#compId').val();
    var regId = $('#regionId').val();
    var params = {
        compId: compId,
        regionId: regId
    };
    //AJAXом загрузим форму подачи заявки NewClimbers.cshtml
    $('#newClimbersPanel').load('/Applications/NewClimbers', params,
        function (responseText, textStatus, XMLHttpRequest) {
            $('#dataLoading').hide();
            if (textStatus != 'success') {
                //при ошибке вернем всё как было (откроем список заявок и кнопки)
                $('#appsTable').show();
                $('.hide-when-edit').show();
                alert('Ошибка загрузки заявки');
            }
            else {
                //покажем загруженную форму
                $('#newClimbersPanel').show();
                $('.cancel-one-climber').click(function () { //событие по кнопке "Удалить заявку" для загруженных участников
                    if (confirm('Отменить заявку данного участника')) {
                        $(this).parent('div').remove();
                    }
                });
                $('#addnewclimber').click(function () { //Событие по кнопке "новый участник"
                    var nextIndex = parseInt($('#newNextIndex').val());
                    $('#newNextIndex').val(nextIndex + 1);
                    var pId = 'pId' + nextIndex;
                    //добавим слой для отображения
                    $('#dataLoading').show();
                    $('#newClimbersFormList').append('<div id="' + pId + '"><div id="' + pId + '_ld">Идет загрузка</div></div>');
                    var innerParams = { compId: $('#compId').val(), index: nextIndex };
                    //AJAXом загрузим форму для одного участника EditClimber.cshtml
                    $('#' + pId).load('/Applications/EditClimber', innerParams,
                    function (responseTextI, textStatusI, XMLHttpRequestI) {
                        $('#dataLoading').hide();
                        if (textStatusI != 'success') {
                            $('#' + pId).remove();
                            alert('Ошибка загрузки новой заявки');
                        }
                        else {
                            //Добавим кнопку "Отмена" и событие
                            $('#' + pId + '_ld').remove();
                            $('#' + pId).append('<input type="button" class="cancel-one-climber hide-when-apply" value="Удалить заявку" />');
                            $('#' + pId).children('.cancel-one-climber').click(function () {
                                if (confirm('Отменить заявку данного участника')) {
                                    $(this).parent('div').remove();
                                }
                            });
                        }
                    });
                });
            }
        }
    );
}

//Начало подачи списка новых заявок NewClimbers.cshtml
function newClimbersAppStarted() {
    //скроем кнопки "подать заявку", "новый участник" и "отмена"
    $('.hide-when-apply').hide();
    $('#dataLoading').show();
}

//при ошибке подачи заявок откроем кнопки NewClimbers.cshtml
function newClimbersAppFailed() {
    alert('Ошибка проверки поданной заявки');
    $('.hide-when-apply').show();
    $('#dataLoading').hide();
}

//при валидации заявок без сбоев NewClimbers.cshtml
function newClimbersAppSucceded() {
    $('#dataLoading').hide();
    //при неуспешной валидации установим кнопки "отмена"
    $('.cancel-one-climber').click(function () {
        if (confirm('Отменить заявку данного участника')) {
            $(this).parent('div').remove();
        }
    });
}

//Кнопка "ОТМЕНА" NewClimbers.cshtml
function cancelNewAppClick() {
    if (confirm('Вы уверены, что хотите отказаться от подачи заявок?')) {
        $('#newClimbersPanel').children().remove();
        $('#newClimbersPanel').hide();
        $('#appsTable').show();
        $('.hide-when-edit').show();
    }
}

//Начало сохранения проверенных заявок (ConfirmNewClimbers.cshtml)
function newClimbersConfirmSubmit() {
    var formData = $('#confirmClimbersForm').serialize();
    //скроем кнопки подачи заявок
    $('.hide-when-apply').hide();
    $('#dataLoading').show();
    $.post("/Applications/ConfirmNewClimbers", formData, newClimbersConfirmCompleted);
}

//заявки сохранены (ConfirmNewClimbers.cshtml)
function newClimbersConfirmCompleted(data, textStatus, jqXHR) {
    $('.hide-when-apply').show();
    $('#dataLoading').hide();
    if (textStatus != "success") {
        alert('Ошибка сохранения заявок: ' + textStatus);
        return;
    }
    if (data.Status != 0) {
        $('#confirmErrors').show();
        $('#confirmErrors').children().remove();
        for (var i = 0; i < data.Errors.length; i++) {
            $('#confirmErrors').append('<li>' + data.Errors[i] + '</li>');
        }
        return;
    }
    $('#newClimbersPanel').children().remove();
    $('#newClimbersPanel').hide();
    $('#appsTable').show();
    $('.hide-when-edit').show();
    updateAppList();
}

function updateAppList() {
    var compId = $('#compId').val();
    var regId = $('#regionId').val();
    var grpId = $('#groupId').val();
    $('#dataLoading').show();
    $('.apps-full-list').load('/Applications/Applications/' + compId, 'groupId=' + grpId + '&regionId=' + regId + '&showDiv=false',
        function (responseText, textStatus, XMLHttpRequest) {
            $('#dataLoading').hide();
            if (textStatus != 'success' && textStatus != 'completed') {
                alert('Ошибка загрузки списка участников: ' + responseText);
                return;
            }
            $('.hide-when-edit').show();
        }
    );
}