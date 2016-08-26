/// <reference name="MicrosoftAjax.js"/>

Type.registerNamespace("WebApatity");

WebApatity.scriptLib = function(element) {
    WebApatity.scriptLib.initializeBase(this, [element]);
}

WebApatity.scriptLib.prototype = {
    initialize: function() {
        WebApatity.scriptLib.callBaseMethod(this, 'initialize');
        
        // Добавьте здесь пользовательскую инициализацию
    },
    dispose: function() {        
        //Добавьте здесь настраиваемые действия удаления
        WebApatity.scriptLib.callBaseMethod(this, 'dispose');
    }
}
WebApatity.scriptLib.registerClass('WebApatity.scriptLib', Sys.UI.Behavior);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

function OnTimer() {
            //раскомментить для бегущей строки
               //PageMethods.GetTimeSpan(OnRequestComplete, OnError);
            }
    
            function OnRequestComplete(result) {
                var label = document.getElementById("ctl00_Label1");
                if (result.toString().indexOf("осталось") > 0) {
                    label.style.fontSize = "X-Large";
                } else {
                    label.style.fontSize = "Large";
                }
                label.innerHTML = result;
            }
    
            function OnError(result)
            {
                var lbl = document.getElementById("ctl00_Label1");
                lbl.innerHTML = "";
            }
var timeMax = 60;    
        var timeLeft = timeMax;
      function pageLoad() {
        var pM = Sys.WebForms.PageRequestManager.getInstance();
        pM.add_endRequest(endRequest);
        timeLeft=timeMax;
      }
      
      function endRequest(sender, args) {
        if(args.get_error() != null) {
            /*alert(args.get_error().message);*/
            args.set_errorHandled(true);
        }
      }