function Load_view_list(content_id) {

//    var url = window.location.pathname;

//    if (url[url.length - 1] == "/") {
//        url += "ListView";
//    } else {
//        url += "/ListView";
//    }

//    if (content_id != "") {
//        $("#ContentDiv").load(url);
//    }
}

function ShowDialogue(width, height, position, title) {

    //$("#dialog-page").html("");
    $("#popup-load").show();
    $("#content-dialog").dialog("open").dialog("option", "width", width)
        .dialog("option", "height", height)
        .dialog("option", "position", position)
        .dialog("option", "title", title);

}

$(function () {

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
        modal: true
    });

    $("#add-grn").click(function (e) {

        e.preventDefault();
        ShowDialogue(750, 550, "center", "Add Goods received note");
        $("#dialog-page").load($(this).attr("href"));

    });

    //    $("a.mytest").click(function (e) {

    //        e.preventDefault();
    //        $("#dialog-page").html("");
    //        $("#popup-load").show();
    //        $("#content-dialog").dialog("open").dialog("option", "width", 750)
    //        .dialog("option", "height", 550)
    //        .dialog("option", "position", "center")
    //        .dialog("option", "title", "Testing");
    //        //alert($(this).attr("href"));
    //        $("#dialog-page").load($(this).attr("href"));

    //    });

});

    