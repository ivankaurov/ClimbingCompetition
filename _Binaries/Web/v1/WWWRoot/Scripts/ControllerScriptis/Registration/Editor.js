function SetText(elementId, textToSet, selectData)
{
    if (!selectData) {
        $("#" + elementId).val(textToSet);
        return true;
    }
    var oldText = $("#" + elementId).val();
    if (oldText == undefined || oldText == null)
        oldText = "";
    if (textToSet == undefined || textToSet == null)
        textToSet = "";
    var startIndex = oldText.length;
    if(startIndex > 0 && textToSet.toLowerCase().indexOf(oldText.toLowerCase().replace('ё','е')) < 0)
        return false;
    $("#" + elementId).val(textToSet);
    var endIndex = textToSet.length;
    if(endIndex > startIndex) {
        $("#" + elementId).focus();
        var elem = document.getElementById(elementId);
        try {
            elem.setSelectionRange(startIndex, endIndex);
        }
        catch(ex)
        {
            var range = elem.createTextRange();
            range.moveStart("character", startIndex);
            range.select();
        }
    }
    return true;
}

function surnameChanged(indx, src) {
    var surnameOldSelector = "#hidden_surname_" + indx;
    var nameOldSelector = "#hidden_name_" + indx;

    var surname = $("#surname_" + indx).val();
    var surnameOld = $(surnameOldSelector).val();
    if (src == "surname" && surnameOld == surname)
        return;
    var nameOld = $(nameOldSelector).val();
    var name = (src == "name") ? $("#name_" + indx).val() : "";
    if (src == "name" && nameOld == name && surnameOld == surname)
        return;

    var manyReg = $("#many_reg_" + indx).val();

    if (surname != surnameOld)
        $(surnameOldSelector).val(surname);
    if (name != nameOld)
        $(nameOldSelector).val(name);

    if (surname == null || surname.length < 3 || src=="name" && name.length < 1)
        return;
    if (src == "surname" && surnameOld.length > surname.length
        || src == "name" && nameOld.length > name.length)
        return;

    var objPacked = {
        surname: surname,
        name: name,
        region: $("#region_" + indx).val(),
        compId: $("#comp_" + indx).val()
    };
    $.post("/Registration/ClimberSupportData", objPacked, function (data, textStatus, jqXHR) {
        if (textStatus != "success")
            return;
        if (data == undefined || data == null || data.Surname == undefined) {
            if (src == "surname") {
                $("#name_" + indx).val("");
                $(nameOldSelector).val("");
            }
            $("#year_" + indx).val("");
            $("#gender_" + indx).val("");
            $("#qf_" + indx).val("");
            /*if (manyReg || manyReg == "1")
                $("#region_" + indx).val("");*/
            return;
        }
        if (!SetText("surname_" + indx, data.Surname, src == "surname"))
            return;
        if (!SetText("name_" + indx, data.Name, src == "name"))
            return;
        $(surnameOldSelector).val(data.Surname);
        $(nameOldSelector).val(data.Name);
        $("#year_" + indx).val(data.YearOfBirth);
        $("#gender_" + indx).val(data.Gender);
        $("#qf_" + indx).val(data.Qf);
        /*if (objPacked.region != data.Team)
            $("#region_" + indx).val(data.Team);*/
    });
}