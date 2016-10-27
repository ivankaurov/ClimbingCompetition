var UPDATE_INTERVAL = 60;
var timerId = undefined;

function listLoadStarted(divId) {
    stopTimer();
    setTimerStrip("Идет загрузка...");
}

function listLoadfailed() {
    alert("Ошибка загрузки протокола.");
}

var timeRemainig = 0;
function listLoadSucceded(divId) {
    var live = $("#is_live").val();
    if (live == "True" || live == "true") {
        startTimer(divId);
    }
    else {
        stopTimer();
        setTimerStrip("");
        $("#cbRefresh").hide();
    }
}

function setTimerStripCurrent() {
    setTimerStrip("Обновление через " + timeRemainig + " сек.");
}

function setTimerStrip(value) {
    $("#timerStrip").children("p").html(value);
}

function stopTimer() {
    if (timerId != undefined) {
        clearInterval(timerId);
        timerId = undefined;
    }
}

function onTimer(divId) {
    timeRemainig--;
    if (timeRemainig <= 0)
        updateList(divId);
    else
        setTimerStripCurrent();
}

function startTimer(divId) {
    if (timerId != undefined)
        stopTimer();
    var initialInterval = $("#refresh_period").val();
    if (initialInterval == null || initialInterval == undefined || initialInterval == "")
        initialInterval = UPDATE_INTERVAL;
    timeRemainig = initialInterval;
    setTimerStripCurrent();
    $("#cbRefresh").show();
    timerId = setInterval(onTimer, 1000, divId);
}

function updateList(divId) {
    stopTimer();
    setTimerStrip("Идет обновление");
    var objPacked = {
        listId: $("#round_list").val(),
        divId: divId
    }
    $("#" + divId).load("/Data/GetList", objPacked, function (data, textStatus, jqXHR) {
        if (textStatus != "success") {
            alert("Ошибка обновления");
            return;
        }
        listLoadSucceded(divId);
    });
}

function cbRefresh_Changed(divId) {
    var checked = $("#cbRefresh").attr("checked");
    if (checked == null || checked == undefined)
        checked = "";
    else
        checked = checked.toLocaleLowerCase();
    if (checked == "checked" || checked == "true")
        listLoadSucceded(divId);
    else {
        stopTimer();
        setTimerStrip("");
    }
}