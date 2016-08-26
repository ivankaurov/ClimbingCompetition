function getCurrentTeam() {
    var sTeam = '';
    try {
        var cbTeam = null;
        if (teamSelID != null && teamSelID != undefined)
            cbTeam = document.getElementById(teamSelID);
        if (cbTeam != null)
            sTeam = cbTeam.value;
        if (sTeam == null)
            sTeam = '';
    } catch (err) { }
    return sTeam;
}

function getCurrentComp() {
    try {
        if(compID != undefined && compID != null)
            return compID;
        else
            return -1;
    } catch (errGetComp) {
        return -1;
    }
}

var tbNameToUpdate = null;
var tbAgeToUpdate = null;
var cbGenderToUpdate = null;
var cbQfToUpdate = null;
var lastUpdatedName = null;
var lastLength = 2;
var allowOnBlur = true;
function suggestClimber(tbNameID, tbAgeID, cbGenderID, cbQfID) {
    if (tbNameToUpdate != null)
        return;
    var tbName = document.getElementById(tbNameID);
    if (tbName == null)
        return;
    if (tbName.id != lastUpdatedName)
        lastLength = 2;

    allowOnBlur = false;
    tbName.blur();
    tbName.focus();
    var enteredName = tbName.value;

    if (enteredName.length <= lastLength || enteredName.length < 3) {
        lastLength = enteredName.length;
        allowOnBlur = true;
        return;
    }
    tbAgeToUpdate = document.getElementById(tbAgeID);
    cbGenderToUpdate = document.getElementById(cbGenderID);
    cbQfToUpdate = document.getElementById(cbQfID);
    tbNameToUpdate = tbName;

    lastLength = enteredName.length;

    var vServ = WebClimbing.AJAXService;

    var sTeam = getCurrentTeam();
    var curComp = getCurrentComp();
    vServ.GetClimberData(enteredName, sTeam, curComp, OnSuccessClimber, null, null);
}

function OnSuccessClimber(result) {
    allowOnBlur = true;
    if (tbNameToUpdate == null || tbNameToUpdate == undefined)
        return;
    if (result == null) {
        tbNameToUpdate = null;
        tbAgeToUpdate.value = '';
        cbGenderToUpdate.value = '';
        cbQfToUpdate.value = '';
        return;
    }

    var currentText = tbNameToUpdate.value;
    var textToSet = result.Name;
    var startIndex = currentText.length;
    if (startIndex > lastLength) {
        var curID = tbNameToUpdate.id;
        tbNameToUpdate = null;
        suggestClimber(curID, tbAgeToUpdate.id, cbGenderToUpdate.id, cbQfToUpdate.id);
        return;
    }
    var endIndex = textToSet.length;
    tbNameToUpdate.value = textToSet;
    if (endIndex > startIndex) {
        try {
            tbNameToUpdate.setSelectionRange(startIndex, endIndex);
        } catch (errI) { //если пользователь - слоупок и юзает доисторический IE
            var range = tbNameToUpdate.createTextRange();
            range.moveStart("character", startIndex);
            range.select();
        }
    }
    tbAgeToUpdate.value = result.Age;
    cbGenderToUpdate.selectedIndex = result.Gender;
    lastUpdatedName = tbNameToUpdate.id;
    if (result.Qf != '')
        cbQfToUpdate.value = result.Qf;
    else
        cbQfToUpdate.value = '';
    tbNameToUpdate = null;
}

var lblErrorV = null;

function ValidateForm(tbNameID, tbAgeID, cbGenderID, lblErrorID, nThreshold) {
    if (!allowOnBlur)
        return;
    allowOnBlur = false;
    var scs = false;
    try {
        var lblError = document.getElementById(lblErrorID);
        if (lblError == null) {
            return;
        }
        lblError.innerText = 'Проверка данных...';
        lblError.style.color = 'red';
        var compIDLocal = getCurrentComp();
        if (compIDLocal < 1)
            return;


        
        var tbName = document.getElementById(tbNameID);
        if (tbName == null || tbName.value == null || tbName.value == '') {
            lblError.innerText = '';
            return;
        }
        var tbAge = document.getElementById(tbAgeID);
        if (tbAge == null)
            return;
        
        if (tbAge.value == null || tbAge.value == '') {
            lblError.innerText = 'Возраст не введён';
            return;
        }
        var curAge;
        try { curAge = parseInt(tbAge.value); }
        catch (errAge) {
            lblError.innerText = 'Возраст введён неверно';
            return;
        }
        if (curAge == undefined || curAge == null || curAge == NaN || curAge < 0) {
            lblError.innerText = 'Возраст введён неверно';
            return;
        } else if (curAge <= 30)
            curAge += 2000;
        else if (curAge <= 99)
            curAge += 1900;
        var bakVal = tbAge.value;
        tbAge.value = curAge;
        if (tbAge.value == 'NaN') {
            tbAge.value = bakVal;
            lblError.innerText = 'Возраст введён неверно';
            return;
        }

        var cbGender = document.getElementById(cbGenderID);
        if (cbGender == null)
            return;
        if (cbGender.selectedIndex == null || cbGender.selectedIndex < 0) {
            lblError.innerText = 'Пол не указан';
            return;
        }

        var teamToSet = getCurrentTeam();

        var vServ = WebClimbing.AJAXService;
        if (lblErrorV != null)
            return;
        lblErrorV = lblError;
        scs = true;
        vServ.ValidateClimber(tbName.value, tbAge.value, (cbGender.selectedIndex > 0), compIDLocal, teamToSet, nThreshold, OnSuccessValidate);
    }
    finally {
        if (!scs)
            allowOnBlur = true;
    }
    
    //vServ.ValidateClimber(clm, compIDLocal, sTeamToSet, 0, OnSuccessValidate);
}

function OnSuccessValidate(res) {
    allowOnBlur = true;
    if (lblErrorV == null)
        return;
    var textToSet = (res == null || res == '') ? 'OK' : res;
    lblErrorV.style.color = (textToSet == 'OK') ? 'green' : 'red';
    lblErrorV.innerText = textToSet;
    lblErrorV = null;
}