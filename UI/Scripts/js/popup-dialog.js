$(function () {

    $.ajaxSetup({
        beforeSend: function () {

        },
        complete: function () {
            $('#popup-loading').hide();
        },
        success: function () {
            $('#popup-loading').hide();
        },
        error: function () {
            AjaxFailure();
        }
    });

    // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
    $("#dialog:ui-dialog").dialog("destroy");

    $("#popup-dialog").dialog({
        autoOpen: false,
        modal: true
    });

    $(".btnPopup").click(function (e) {

        e.preventDefault();
        ShowDialogue(750, 550, "center", "Test Dialogue");
        $("#popup-page").load($(this).attr("href"));

    });
});

function ShowDialogue(title) {
    $("#popup-page").html("");
    $("#popup-loading").show();
    $("#popup-dialog").dialog("open")
        .dialog("option", "width", "auto")
        .dialog("option", "height", "auto")
        .dialog("option", "position", "center")
        .dialog("option", "title", title);
}

function ShowDialogueOR(title) {
    $("#popup-page").html("");
    $("#popup-loading").show();
    $("#popup-dialog").dialog("open")
        .dialog("option", "width", "auto")
        .dialog("option", "maxHeight", 500)
        .dialog("option", "position", "center")
        .dialog("option", "title", title);
}

function LoadInPopup(url, title) {
    $(function () {
        ShowDialogue(title);
        $("#popup-page").load(url);
    });
}

function CenterPopupOR() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
    //onChangeCurrency();
    cleanUpNumbers();
    calculateMBValue();
}

function CenterPopupPO() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
    cleanUpNumbers();
    calculateMBValue();
}

function CenterPopupRFP() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
    cleanUpNumbersRFP();
    calculateMBValue();
}

function CenterPopupECF() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
    cleanUpNumbersECF();
    calculateMBValue();
}

function CenterPopupSPM() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
    cleanUpNumbersSPM();
    calculateMBValue();
}


function CenterPopup() {
    $("#popup-dialog").dialog("open").dialog("option", "position", "center");
}

function LoadInPopup4OR(url, title) {
    $(function () {
        ShowDialogueOR(title);
        $("#popup-page").load(url);
    });
}

function CloseDialog() {
    $("#popup-dialog").dialog("close");
}