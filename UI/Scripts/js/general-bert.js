
function AjaxFailure() {

    alert("Something went wrong, refresh page and try again. If failure persists contract site administrator !");

}

function ShowDialogue(width, height, position, title) {

    $("#dialog-page").html("");
    $("#popup-load").show();
    $("#content-dialog").dialog("open").dialog("option", "width", width)
        .dialog("option", "height", height)
        .dialog("option", "position", position)
        .dialog("option", "title", title);

}

function AjaxCall(post_url, post_data) {

    $.ajax({
        type: 'POST',
        url: post_url,
        data: post_data,
        success: function (data) {
            alert(data);
            $("#ContentDiv").html("");
            $("#ContentDiv").html(data);
        },
        dataType: "html"
    });

}

$(document).ready(function () {

    $.ajaxSetup({
        beforeSend: function () {

        },
        complete: function () {
            $('#popup-load').hide();
        },
        success: function () {
            $('#popup-load').hide();
        },
        error: function () {
            AjaxFailure();
        }
    });

    // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
    $("#dialog:ui-dialog").dialog("destroy");

    $("#content-dialog").dialog({
        autoOpen: false,
        //zIndex: 1000000,
        modal: true,
        buttons: {
            Cancel: function () {
                $(this).dialog("close");
            }
        },
        close: function () {
          
        }
    });

});