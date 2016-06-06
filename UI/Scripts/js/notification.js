$(document).ready(function () {
    $(".dropdown dt span#linkglobal").click(function () {
        
        // Change the behaviour of onclick states for links within the menu.
        var toggleId = "#noti-accordion";

        // Hides all other menus depending on JQuery id assigned to them
        $(".dropdown dd div").not(toggleId).hide();

        //Only toggles the menu we want since the menu could be showing and we want to hide it.
        $(toggleId).toggle();

        //Change the css class on the menu header to show the selected class.
        if ($(toggleId).css("display") == "none") {
            $(this).removeClass("noti-pink");
            $(this).addClass("noti-yellow");
        } else {
            $(this).removeClass("noti-yellow");
            $(this).addClass("noti-pink");
        }

    });

    $(".dropdown dd ul li a").click(function () {

        // This is the default behaviour for all links within the menus
        var text = $(this).html();
        $(".dropdown dt a span").html(text);
        $("#noti-root").hide();
    });

    $(document).bind('click', function (e) {

        // Lets hide the menu when the page is clicked anywhere but the menu.
        var $clicked = $(e.target);
        if (!$clicked.parents().hasClass("dropdown")) {
            $("#noti-root").hide();
            $(".dropdown dt #linkglobal").removeClass("selected");
        }
    });
});