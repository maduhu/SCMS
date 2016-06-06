function Load_view_list(link_id) {

    var url = window.location.pathname;

    if (url[url.length - 1] == "/") {
        url += "ListView";
    } else {
        url += "/ListView";
    }

    $("#ContentDiv").load(url);

    $('#popup-load').hide();
    
}

$(function () {

    $.ajaxSetup({
        beforeSend: function () {

        
        },
        complete: function () {


        },
        success: function () {
        
         }
    });

    // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
    $("#dialog:ui-dialog").dialog("destroy");

    $("#content-dialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    $("#item-link").click(function () {

        $("#popup-load").show();
        $("#dialog-page").html("");
        $("#content-dialog").dialog("open").dialog("option", "width", 400)
        .dialog("option", "height", 550)
        .dialog("option", "position", "center")
        .dialog("option", "title", "Item");

    });

    $("#add-link").click(function () {

        $("#popup-load").show();
        $("#dialog-page").html("");
        $("#content-dialog").dialog("open").dialog("option", "width", 1000)
        .dialog("option", "height", 650)
        .dialog("option", "position", "center")
        .dialog("option", "title", "Create Inventory item");

    });

    $("a.editlink").click(function () {

        $("#popup-load").show();
        $("#dialog-page").html("");
        $("#content-dialog").dialog("open").dialog("option", "width", 1000)
        .dialog("option", "height", 650)
        .dialog("option", "position", "center")
        .dialog("option", "title", "Edit Inventory item");

    });

    $("a.detaillink").click(function () {

        $("#popup-load").show();
        $("#dialog-page").html("");
        $("#content-dialog").dialog("open").dialog("option", "width", 1000)
        .dialog("option", "height", 450)
        .dialog("option", "position", "center")
        .dialog("option", "title", "Details: Inventory item");

    });

});

    