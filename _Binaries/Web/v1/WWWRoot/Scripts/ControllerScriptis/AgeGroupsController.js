/// <reference path="..\jquery-1.7.1-vsdoc.js" />
/// <reference path="..\jquery-ui-1.8.20.js" />

function getNextId() {
    var store = $("#nextId");
    var res = parseInt(store.val());
    store.val((res + 1));
    return res;
}

$(function () {
    $('#newGroup').click(function () {
        var prfx = getNextId();
        var s = 'newRow' + prfx;
        $('#tbl').append('<tr id="' + s + '"><td /></tr>');
        $('#' + s).load('/AgeGroups/GroupEdit?prefix=' + prfx, null, function () {
            $('#' + s).find('.delete-row').click(function () {
                $('#' + s).remove();
            });
        });

    });
    $('.delete-row').click(function () {
        var id = $(this).attr('id');
        var deleted_sign = $('#' + id + '_del');
        var isNew = $('#' + id + '_new').val();
        if (isNew == 'True') {
            $('#' + id + '_new').parents('tr').remove();
            return;
        }
        
        var overlineElem = $('.' + id);
        var value = deleted_sign.val();
        if (value == 'False') {
            deleted_sign.val('True');
            $(this).text('Восстановить');
            overlineElem.css('text-decoration', 'line-through');
        }
        else if (value == 'True') {
            deleted_sign.val('False');
            $(this).text('Удалить');
            overlineElem.css('text-decoration', 'none');
        }
    });
});