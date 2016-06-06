var ResponseDiv;
var xmlHttp;
var ResponseTb;

function GetXmlHttpObject() {
    var objXMLHttp = null;
    if (window.XMLHttpRequest) {
        objXMLHttp = new XMLHttpRequest();
    }
    else if (window.ActiveXObject) {
        objXMLHttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    return objXMLHttp;
}

function stateChanged() {
    if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
        document.getElementById(ResponseDiv).innerHTML = xmlHttp.responseText;
        //initNotificationAccordion();
        initNotificationDD();
    }
}

function stateChangedFX() {
    if (xmlHttp.readyState == 4 || xmlHttp.readyState == "complete") {
        var fxVal = roundNumber(parseFloat(xmlHttp.responseText), 2);
        document.getElementById("origFxValue").value = fxVal;
        document.getElementById(ResponseTb).disabled = false;
        document.getElementById(ResponseTb).value = fxVal;
    }
}

function initNotificationAccordion() {
    $(document).ready(function () {
        $("ul#noti-root").accordionMenu({
            accordion: true,
            speed: 300,
            closedSign: '',
            openedSign: ''
        });
    });
}

function initNotificationDD() {
    $(document).ready(function () {

        $("#noti-accordion").accordion({
            collapsible: true,
            header: "h3",
            clearStyle: true,
            fillSpace: true,
            active: 2
        });

        $(".dropdown dt span#linkglobal").click(function () {

            // Change the behaviour of onclick states for links within the menu.
            var toggleId = "#noti-accordion";

            // Hides all other menus depending on JQuery id assigned to them
            $(".dropdown dd div").not(toggleId).hide();

            //Only toggles the menu we want since the menu could be showing and we want to hide it.
            $(toggleId).toggle('slow');

            //Change the css class on the menu header to show the selected class.
            if ($(toggleId).css("display") == "none") {
                $(this).removeClass("noti-yellow");
                $(this).addClass("noti-pink");
            } else {
                $(this).removeClass("noti-pink");
                $(this).addClass("noti-yellow");
            }

        });

        $(".dropdown dd ul li a").click(function () {

            // This is the default behaviour for all links within the menus
            var text = $(this).html();
            $(".dropdown dt a span").html(text);
            $("#noti-accordion").hide();
        });

        $(document).bind('click', function (e) {

            // Lets hide the menu when the page is clicked anywhere but the menu.
            var $clicked = $(e.target);
            if (!$clicked.parents().hasClass("dropdown")) {
                $("#noti-accordion").hide();
                $(".dropdown dt #linkglobal").removeClass("selected");
            }
        });

    });
}

function GetRequests() {
    var url = "/RequestReview/GetRequests";
    document.getElementById("loading").style.display = "inline";
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("loading").style.display = "none";
    document.getElementById("popupDiv").innerHTML = xmlHttp.responseText;
    ShowPopupDiv();
}

function LoadRequests() {
    var url = "/RequestReview/GetRequests";
    ResponseDiv = "notificationDiv";
    xmlHttp = GetXmlHttpObject();
    xmlHttp.onreadystatechange = stateChanged;
    xmlHttp.open("GET", url, true);
    xmlHttp.send(null);
}

function LoadRequestsForAuth() {
    var url = "/RequestReview/GetRequestsForAuth";
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("ortapoAuth").innerHTML = xmlHttp.responseText;
}

function GetORItems(element) {
    var url = "/BinCard/GetORItems?orId=" + element.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);

    var response = xmlHttp.responseText.split("~");
    document.getElementById("oritemcontent").innerHTML = response[0];
    document.getElementById("warehousecontent").innerHTML = response[1];
    document.getElementById("donorcontent").innerHTML = response[2];

}

function GetItemPackages(element) {
    var url = "/BinCard/GetItemPackages?orItemId=" + element.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("itempackcontent").innerHTML = xmlHttp.responseText;
}

function GetPPItemPackages(element) {
    var url = "/BinCard/GetPPItemPackages?ppItemId=" + element.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("itempackcontent").innerHTML = xmlHttp.responseText;
}

function GetPPItems(element) {
    var url = "/BinCard/GetPPItems?ppId=" + element.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    
    var response = xmlHttp.responseText.split("~");
    document.getElementById("ppitemcontent").innerHTML = response[0];
    document.getElementById("warehousecontent").innerHTML = response[1];
    document.getElementById("donorcontent").innerHTML = response[2];
}

function getRecurringPx(select) {
    var url = "/OrderRequest/GetRecurringPx?itemId=" + select.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    var unitpx = parseFloat(xmlHttp.responseText);
    var qty = document.getElementById("Quatity").value;
    document.getElementById("UnityPrice").value = roundNumber(unitpx, 2);
    var Quatity = parseFloat(qty != "" ? qty : 0);
    var txttotal = document.getElementById("TotalPrice");
    txttotal.value = roundNumber(unitpx * Quatity, 2);
}

function sendRequest(url, rspDiv, msgDiv) {
    jQuery.ajax({
        UpdateTargetId: rspDiv,
        LoadingElementId: msgDiv,
        HttpMethod: 'Get',
        url: url,
        async: false,
        beforeSend: function (jqXHR, settings) {
            document.getElementById(msgDiv).style.display = "inline";
        },
        success: function (data) {
            $('#' + rspDiv).html(data);
            document.getElementById(msgDiv).style.display = "none";
        }
    });
}

function LoadBCDetails(element) {
    var url = '/BinCard/ManageBin?binId=' + element.value;
    sendRequest(url, 'bincardDiv', 'loading');
}

function getBInItemz(element) {
    if (element.value == "")
        return;
    sendRequest("/BinCard/GetStockItemzByProject?projectId=" + element.value, "binitemcontent", "loading");
}

function LoadGIV(element) {
    var kk = element.value;
    var url = '/GoodsIssuedVoucher/LoadGiv?roId=' + element.value;
    sendRequest(url, 'givDiv', 'loading');
    ClearFormById('#' + 'frmgiv', 'refNo');
    document.getElementById("roid").value = kk;
}

function ClearGIV() {
    var kk = document.getElementById("roid").value;
    ClearFormById('#' + 'frmgiv', 'refNo');
    document.getElementById("roid").value = kk;
}

function RemoveRow(rowId, index, confirmMsg) {
    if (confirm(confirmMsg)) {
        var row = document.getElementById(rowId);
        row.style.display = "none";
        document.getElementById("IsRemoved" + index).value = "true";
        if (document.getElementById("QtyReceived" + index).value == "")
            document.getElementById("QtyReceived" + index).value = 0;
        document.getElementById("undoRemoves").style.display = "";
    }
}

function UndoRemoves(count, confirmMsg)
{
    if (confirm(confirmMsg)) {
        var row = "";
        for (i = 0; i < count; i++) {
            if (document.getElementById("IsRemoved" + i).value == "true") {
                row = document.getElementById("Row" + i);
                row.style.display = "";
                document.getElementById("IsRemoved" + i).value = "false";
            }
        }
        document.getElementById("undoRemoves").style.display = "none";
    }
}

function ValidateGIVBeforeSave(count, failureMsg)
{
    var row = "";
    for (i = 0; i < count; i++) {
        if (document.getElementById("IsRemoved" + i).value != "true") 
            return true;
    }
    errormsg(failureMsg);
    return false;
}

function GetPreviousQTY(element) {
    if (Trim(element.value) == "") {
        document.getElementById("prevQTY").value = "0";
    }
    else {
        var url = '/BinCard/GetPrevQTYReceived?grnItemId=' + element.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("prevQTY").value = xmlHttp.responseText;
    }
}

function GetPreviousQTYReleased(element) {
    if (Trim(element.value) == "") {
        document.getElementById("prevQTY").value = "0";
    }
    else {
        var url = '/BinCard/GetPrevQTYIssued?roItemId=' + element.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("prevQTYIssed").value = xmlHttp.responseText;
    }
}

function toggleDisplay(element) {
    if (Trim(element.value) == "OR") {
        $('#ppcontentsId').hide();
        $('#orcontentsId').show();
        document.getElementById("refNoLable").innerHTML = 'Order Request No.';
        document.getElementById("itemLable").innerHTML = 'Order Request Items';
    }
    else {
        document.getElementById("refNoLable").innerHTML = 'Procurement Plan No.';
        document.getElementById("itemLable").innerHTML = 'Procurement Items';
        $('#orcontentsId').hide();
        $('#ppcontentsId').show();
    }
}

function LoadFDetails(element) {
    var url = '/FleetManager/LoadFleetDetails/' + element.value;
    sendRequest(url, 'fleetDiv', 'loading');
    initPostFundsView(0);
}

function sendAjaxRequest(url) {
    jQuery.ajax({
        UpdateTargetId: "popup-page",
        LoadingElementId: 'popup-loading',
        HttpMethod: 'Post',
        url: url,
        OnBegin: ShowDialogue('Item Details'),
        success: function (data) {

            //            $.Zebra_Dialog(data, {
            //                'type': '',
            //                'buttons':  ['Close']
            //                
            //            });


            $('#popup-page').html(data);
            CenterPopup();
            document.getElementById('popup-loading').style.display = "none";
        }
    });
}

function disablebutton(element) {
    document.getElementById(element).disabled = true;
}

function hidebutton(element) {
    document.getElementById(element).style.visibility = 'hidden';
}

function sendAjaxRequest(url, title) {
    jQuery.ajax({
        UpdateTargetId: "popup-page",
        LoadingElementId: 'popup-loading',
        url: url,
        async: false,

        OnBegin: ShowDialogue(title),
        success: function (data) {
            $('#popup-page').html(data);
            CenterPopup();
            document.getElementById('popup-loading').style.display = "none";
        }
    });


    //    $.ajax({
    //        type: "POST",
    //        url: "WebService.asmx/PullProfile",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        data: "{'strCustomer':'" + $('#ctl00_MainContent_txtCustomer').val() + "'}",
    //        //data : {'id' : 'yourid' , 'type':'mytype'}, // This will be a array if you have more parameter
    //        success: function (data) {
    //            $('#ctl00_MainContent_txtDBActNum').val(data.d)
    //        }
    //    });

}

function AssetDepreciationDetails(select) {

    //var url = "/Inventory/ViewDeprSummary/" + select.value;
    var url = "/Inventory/LoadAssetDepreciation?assetId=" + select.value;
    if (select.value != "") {
        sendRequest(url, 'AssetsDiv', 'loading');

       // xmlHttp = GetXmlHttpObject();
       // xmlHttp.open("GET", url, false);
       // xmlHttp.send(null);
       // //document.getElementById("AssetDetailsDiv").innerHTML = xmlHttp.responseText;
       // document.getElementById("AssetsDiv").innerHTML = xmlHttp.responseText;
       //// if (document.getElementById('table') != null) { initSorter(); sorter.size(20); }
    }
}

function GetSubofficeWareHouses(select) {
    if (select.value == "")
        return;
    var url = "/GoodsReceivedNote/GetWareHouses?countrysubOfficeId=" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("warehouse").innerHTML = xmlHttp.responseText;
    }
}

function selectedIndexChange(select) {
    if (select.value == "")
        return;
    var url = "/OrderRequest/GetBudgetLines/" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        //document.getElementById(msgDiv).style.display = "none";
        document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
    }
}

function getCPSuboffices(select) {
    if (select.value == "")
        return;
    sendRequest("/Admin/SystemUser/GetCPSubOffices/" + select.value, "cpSubOffices", "loading");
}

function selectedIndexChangeTAuthExpense(select) {
    if (select.value == "")
        return;
    var url = "/TravelAuth/GetBudgetLineForTAuthExpense/" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        //document.getElementById(msgDiv).style.display = "none";
        document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
    }
}

function selectedIndexChangeTAuthDates(select) {
    if (select.value == "")
        return;
    var url = "/TravelAuth/GetBudgetLineForTAuthDates/" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        //document.getElementById(msgDiv).style.display = "none";
        document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
    }
}

function selectedIndexChange4EC(select) {
    var url = "/ExpenseClaim/GetBudgetLines/" + select.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
}

function selectedIndexChange4SP(select) {
    var url = "/SalaryPament/GetBudgetLines/" + select.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
}

function selectedIndexChange4TAF(select, index, isDate, i) {
    var isDateBL = isDate > 0 ? true : false;
    var url = "/TravelAuth/GetBudgetLinesForTAAuth?id=" + select.value + "&isDate=" + isDateBL + "&index=" + i;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    document.getElementById("blDiv" + index).innerHTML = respText;
}

function selectedIndexChangePONos4PP(select) {
    if (Trim(select.value) != "") {
        var url = "/ProcurementPlan/GetProjectNumbers?projectId=" + select.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("ProjectNoDiv").innerHTML = xmlHttp.responseText;
    }
}

function selectFinanceLimit(select) {
    if (select.value != "") {
        var actionType = document.getElementById("actionType").value;
        var activityCode = document.getElementById("activityCode").value;
        var approverId = document.getElementById("approverId").value;
        var url = "/Admin/Approver/GetStaffByFL?FinancialLimitId=" + select.value + "&ActionType=" + actionType + "&ActivityCode=" + activityCode + "&Id=" + approverId;
        sendRequest(url, "popup-page", "popup-loading");
    }
}

function selectFinanceLimit4Project(select) {
    if (select.value != "") {
        var actionType = document.getElementById("actionType").value;
        var activityCode = document.getElementById("activityCode").value;
        var approverId = document.getElementById("approverId").value;
        var pdId = document.getElementById("projectDonorId").value;
        var url = "/Budget/GetStaffByFL?FinancialLimitId=" + select.value + "&ActionType=" + actionType + "&ActivityCode=" + activityCode + "&Id=" + approverId + "&ProjectDonorId=" + pdId;
        sendRequest(url, "popup-page", "popup-loading");
    }
}

function loadDonorByPN(select) {
    if (select.value != "") {
        var url = "/OrderRequest/GetDonorName/" + select.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("donorDiv").innerHTML = xmlHttp.responseText;
    }
}

function GetAssetNo(select) {
    if (Trim(select.value) == "") {
        document.getElementById("AssetNoId").value = "";
    }
    else {
        var url = "/GoodsReceivedNote/GetAssetNo?ProjectDId=" + select.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("AssetNoId").value = xmlHttp.responseText;
    }
}

function GetItemModels(select) {
    var url = "/FleetManager/GetAssetModel?mkId=" + select.value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("ModelDiv").innerHTML = xmlHttp.responseText;
}

function GetFilteredAssets() {
    var url = "/FleetManager/GetFilteredAssets?assetName=" + document.getElementById("AssetDescc").value;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("assetIds").innerHTML = xmlHttp.responseText;
}

function GetProjectNos4RegAsset(select) {
    var url = "/OrderRequest/GetProjectNos4RegAsset/" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("ProjectNoDiv").innerHTML = xmlHttp.responseText;
    }
}

function ComputeQty(select, index) {
    var qtyDelivered = 0;
    var qtyPacks = 0;
    var qtyOrdered = document.getElementById("qtyordered" + index).value;
    var qtyPrevRecu = document.getElementById("qtyprev" + index).value;
    var packId = document.getElementById("Id" + index).value;

    if (select.value == "") {
        document.getElementById("QtyDelivered" + index).value = (qtyOrdered - qtyPrevRecu);
        document.getElementById("Qty" + index).value = (qtyOrdered - qtyPrevRecu);
    }
    else {
        var qty = document.getElementById("Qty" + index).value;
        var url = "/GoodsReceivedNote/ComputeQty?packId=" + select.value + "&Qty=" + qty;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        qtyDelivered = parseInt(xmlHttp.responseText);
        if (qtyDelivered > (qtyOrdered - qtyPrevRecu)) {
            qtyDelivered = (qtyOrdered - qtyPrevRecu);
            url = "/GoodsReceivedNote/ComputePack?packId=" + packId + "&Qty=" + qtyDelivered;
            xmlHttp = GetXmlHttpObject();
            xmlHttp.open("GET", url, false);
            xmlHttp.send(null);
            qtyPacks = parseInt(xmlHttp.responseText);
            document.getElementById("Qty" + index).value = qtyPacks;
        }
        document.getElementById("QtyDelivered" + index).value = qtyDelivered;
    }
}

function ComputeQtyOnBlur(select, index) {
    var qtyDelivered = 0;
    var qtyPacks = 0;
    var qtyOrdered = document.getElementById("qtyordered" + index).value;
    var qtyPrevRecu = document.getElementById("qtyprev" + index).value;
    var packId = document.getElementById("Id" + index).value;

    if (packId == "") {
        document.getElementById("QtyDelivered" + index).value = (qtyOrdered - qtyPrevRecu);
        document.getElementById("Qty" + index).value = (qtyOrdered - qtyPrevRecu);
    }
    else {        
        var url = "/GoodsReceivedNote/ComputeQty?packId=" + packId + "&Qty=" + select.value;
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        qtyDelivered = parseInt(xmlHttp.responseText);
        if (qtyDelivered > (qtyOrdered - qtyPrevRecu)) {
            qtyDelivered = (qtyOrdered - qtyPrevRecu);
            url = "/GoodsReceivedNote/ComputePack?packId=" + packId + "&Qty=" + qtyDelivered;
            xmlHttp = GetXmlHttpObject();
            xmlHttp.open("GET", url, false);
            xmlHttp.send(null);
            qtyPacks = parseInt(xmlHttp.responseText);
            document.getElementById("Qty" + index).value = qtyPacks;            
        }
        document.getElementById("QtyDelivered" + index).value = qtyDelivered;
    }
}

function ValidateQtyDelivered(textbox, index) {
    if (textbox.value != "") {
        var qtyOrdered = parseInt(document.getElementById("qtyordered" + index).value);
        var qtyPrevRecu = parseInt(document.getElementById("qtyprev" + index).value);
        var balance = qtyOrdered - qtyPrevRecu;
        var qtyDelivered = parseInt(textbox.value);
        if (qtyDelivered > balance)
            textbox.value = balance;
    }
}

function ValidateQtyDamaged(textbox, index) {
    if (document.getElementById("QtyDelivered" + index).value != "" && textbox.value != "") {
        var qtyDelivered = parseInt(document.getElementById("QtyDelivered" + index).value);
        var qtyDamaged = parseInt(textbox.value);
        if (qtyDamaged > qtyDelivered)
            textbox.value = qtyDelivered;
    }
}

function ValidateGIVQtyReceived(textbox, index)
{
    if (textbox.value != "") {
        var qtyReceived = parseInt(textbox.value);
        var qtyReleased = parseInt(document.getElementById("QtyReleased" + index).value);
        var prevRecu = parseInt(document.getElementById("PrevRecu" + index).value);

        if (qtyReceived > (qtyReleased - prevRecu))
        {
            qtyReceived = qtyReleased - prevRecu;
            textbox.value = qtyReceived;
        }
    }
}

function GetItemPecentageDepre(select) {
    var url = "/GoodsReceivedNote/GetDepreciationPctge?itemId=" + select.value
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    document.getElementById("depreciation").value = xmlHttp.responseText;
}

function checkBalance() {
    if (document.getElementById("TotalPrice") == null || document.getElementById("TotalPrice").value == "" || document.getElementById("currencyId") == null
        || document.getElementById("currencyId").value == "" || document.getElementById("BudgetLineID") == null || document.getElementById("BudgetLineID").value == "") {
        return false;
    }
    var blId = document.getElementById("BudgetLineID").value;
    var totalPrice = document.getElementById("TotalPrice").value;
    var currencyId = document.getElementById("currencyId").value;
    var url = "/Budget/VerifyAvailableFunds?BudgetLineId=" + blId + "&CurrencyId=" + currencyId + "&TotalPrice=" + totalPrice;
    xmlHttp = GetXmlHttpObject();

    document.getElementById("BudgetLineID").disabled = true;
    document.getElementById("fundsCheck").innerHTML = "<font color=\"blue\">Verifying available funds in BL...</font>";
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "SUCCESS") {
        document.getElementById("fundsCheck").innerHTML = "<font color=\"green\">Done</font>";
        document.getElementById("BudgetLineID").disabled = false;
        return true;
    }
    else {
        document.getElementById("fundsCheck").innerHTML = "<font color=\"red\">Insufficient funds in selected BL.</font>";
        alert("Insufficient funds in selected BL. Please select another Budget Line or change the quantity requested.");
    }
    document.getElementById("BudgetLineID").disabled = false;
    return false;
}

function checkBalanceForEditORItem() {
    var blId = document.getElementById("BudgetLineID").value;
    var totalPrice = document.getElementById("TotalPrice").value;
    var currencyId = document.getElementById("currencyId").value;
    var orItem = "&OrItemId=" + document.getElementById("orItemId").value;
    var url = "/Budget/VerifyAvailableFunds?BudgetLineId=" + blId + "&CurrencyId=" + currencyId + "&TotalPrice=" + totalPrice + orItem;
    xmlHttp = GetXmlHttpObject();

    document.getElementById("BudgetLineID").disabled = true;
    document.getElementById("fundsCheck").innerHTML = "<font color=\"blue\">Verifying available funds in BL...</font>";
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "SUCCESS") {
        document.getElementById("fundsCheck").innerHTML = "<font color=\"green\">Done</font>";
        document.getElementById("BudgetLineID").disabled = false;
        return true;
    }
    else {
        document.getElementById("fundsCheck").innerHTML = "<font color=\"red\">Insufficient funds in selected BL.</font>";
        alert("Insufficient funds in selected BL. Please select another Budget Line or change the quantity requested.");
    }
    document.getElementById("BudgetLineID").disabled = false;
    return false;
}

function checkBalance4Review(select) {
    var blId = select.value;
    var parentId = select.parentNode.id;
    var s_index = parentId.indexOf("s");
    var index = parseInt(parentId.substring(s_index + 1));
    var extraUrl = "";
    if (document.getElementById("extraUrl") != null)
        extraUrl = document.getElementById("extraUrl").value;
    index--;
    var totalPrice = document.getElementById("totalPrice" + index).value;
    var currencyId = document.getElementById("currencyId").value;
    var url = "/Budget/VerifyAvailableFunds?BudgetLineId=" + blId + "&CurrencyId=" + currencyId + "&TotalPrice=" + totalPrice + extraUrl;
    var jqIdName = "#row" + index;
    xmlHttp = GetXmlHttpObject();

    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "SUCCESS") {
        $(jqIdName).removeClass('gridwarning');
        $(jqIdName).addClass('gridodd');
        document.getElementById("budgetCheckStatus").innerHTML = "";
        return true;
    }
    else {

        $(jqIdName).removeClass('gridodd');
        $(jqIdName).addClass('gridwarning');
        document.getElementById("budgetCheckStatus").innerHTML = "<font color=\"Red\">&nbsp;&nbsp;Highlighted row(s) below show Budget Line(s) with insufficient balance</font>";
    }
    return false;
}

function checkBalance4TAF(select, index, isDate) {
    var blId = select.value;
    var amount = 0;
    var jqIdName = "";
    if (isDate > 0) {
        amount = parseFloat(document.getElementById("advanceRequired" + index).value);
        jqIdName = "#row" + index;
    }
    else {
        amount = parseFloat(document.getElementById("totalExpenseAdvance" + index).value);
        jqIdName = "#rowExp" + index;
    }

    var currencyId = document.getElementById("currencyId" + index).value;
    var url = "/Budget/VerifyAvailableFunds?BudgetLineId=" + blId + "&CurrencyId=" + currencyId + "&TotalPrice=" + amount;

    xmlHttp = GetXmlHttpObject();

    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "SUCCESS") {
        $(jqIdName).removeClass('gridwarning');
        $(jqIdName).addClass('gridodd');
        document.getElementById("budgetCheckStatus").innerHTML = "";
        return true;
    }
    else {

        $(jqIdName).removeClass('gridodd');
        $(jqIdName).addClass('gridwarning');
        document.getElementById("budgetCheckStatus").innerHTML = "<font color=\"Red\">&nbsp;&nbsp;Highlighted row(s) below show Budget Line(s) with insufficient balance</font>";
    }
    return false;
}

function ValidateQuotationRef() {
    var poId = document.getElementById("poId").value;
    var quotationRef = document.getElementById("quotationRef").value;
    var url = "/PurchaseOrder/ValidateQuotationRef?QuotationRef=" + quotationRef + "&Id=" + poId;
    sendRequest(url, "qrcheck", "popup-loading");
    if (document.getElementById("qrcheck").innerHTML == "SUCCESS") {
        document.getElementById("qrMsg").innerHTML = "";
        return true;
    }
    document.getElementById("qrMsg").innerHTML = "Quotation Ref already exists";
    return false;
}

function IsPlateNoRegistered() {
    var PNo = document.getElementById("pNo").value;
    var Id = document.getElementById("Id").value;
    var url = "/FleetManager/IsPlateNoExist?PlateNo=" + PNo + "&Id=" + Id;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "0") {
        return true;
    }
    else {
        document.getElementById("pnMsg").innerHTML = xmlHttp.responseText;
        return false;
    }
}

function IsEngineNoExist() {
    var PNo = document.getElementById("ENo").value;
    var Id = document.getElementById("Id").value;
    var url = "/FleetManager/IsEngineNoExist?EngineNo=" + PNo + "&Id=" + Id;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "0") {
        return true;
    }
    else {
        document.getElementById("EnMsg").innerHTML = xmlHttp.responseText;
        return false;
    }
}

function IsChassisNoExist() {
    var CNo = document.getElementById("CNo").value;
    var Id = document.getElementById("Id").value;
    var url = "/FleetManager/IsChassisNoExist?ChassisNo=" + CNo + "&Id=" + Id;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "0") {
        return true;
    }
    else {
        document.getElementById("CnMsg").innerHTML = xmlHttp.responseText;
        return false;
    }
}

function checkBLBalance(index) {
    if ((document.getElementById("addToPO" + index) != null && document.getElementById("addToPO" + index).checked) || document.getElementById("addToPO" + index) == null) {
        var blId = document.getElementById("blId" + index).value;
        var totalPrice = document.getElementById("totalPrice" + index).value;
        var currencyId = document.getElementById("currencyId").value;
        var orItem = "";
        if (document.getElementById("orItem" + index) != null)
            orItem = "&OrItemId=" + document.getElementById("orItem" + index).value;
        var url = "/Budget/VerifyAvailableFunds?BudgetLineId=" + blId + "&CurrencyId=" + currencyId + "&TotalPrice=" + totalPrice + orItem;
        var jqIdName = "#row" + index;
        xmlHttp = GetXmlHttpObject();

        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        if (xmlHttp.responseText == "SUCCESS") {
            $(jqIdName).removeClass('gridwarning');
            $(jqIdName).addClass('gridodd');
            document.getElementById("budgetCheckStatus").innerHTML = "";
            return true;
        }
        else {

            $(jqIdName).removeClass('gridodd');
            $(jqIdName).addClass('gridwarning');
            document.getElementById("budgetCheckStatus").innerHTML = "<font color=\"Red\">&nbsp;&nbsp;Highlighted row(s) below show Budget Line(s) with insufficient balance</font>";
        }
        return false;
    }
    else
        return true;
}

function VerifyBLBalance(count) {
    var checkBLBalance = true;
    var i, index;
    var select;
    for (i = 0; i < count; i++) {
        index = i + 1;
        select = document.getElementById("PBlines" + index).children[0];
        if (!checkBalance4Review(select))
            checkBLBalance = false;
    }
    if (!checkBLBalance) {
        alert("Some of the Budget Lines selected have insufficient funds. CANNOT PROCEED!");
        return false;
    }
    return checkBLBalance;
}

function VerifyBLBalance4TAF(travelDateCount, travelExpenseCount) {
    var checkBLBalance = true;
    var i;
    var select;
    for (i = 0; i < travelDateCount; i++) {
        select = document.getElementById("ViewTravelDates_" + i + "__EntityTDate_BudgetLineId");
        if (!checkBalance4TAF(select, i, 1))
            checkBLBalance = false;
    }

    for (i = 0; i < travelExpenseCount; i++) {
        select = document.getElementById("ViewTravelExpenses_" + i + "__EntityTExpense_BudgetLineId");
        if (!checkBalance4TAF(select, i, 0))
            checkBLBalance = false;
    }

    if (!checkBLBalance) {
        alert("Some of the Budget Lines selected have insufficient funds. CANNOT PROCEED!");
        return false;
    }
    return checkBLBalance;
}

function calculateMBValue() {
    var orCurrencyId = document.getElementById("currencyId").value;
    var mbCurrencyId = document.getElementById("mbCurrencyId").value;
    var totalVal = parseFloat(document.getElementById("totalVal").value);
    if (document.getElementById("origLocalValue") == null)
        return;
    document.getElementById("origLocalValue").value = totalVal;
    var url = "/Budget/CalculateFXValue?LocalCurrencyId=" + orCurrencyId + "&FXCurrencyId=" + mbCurrencyId + "&Amount=" + totalVal;
    ResponseTb = "mbValue";
    document.getElementById(ResponseTb).disabled = true;
    xmlHttp = GetXmlHttpObject();
    xmlHttp.onreadystatechange = stateChangedFX;
    xmlHttp.open("GET", url, true);
    xmlHttp.send(null);
}

function checkRFADetails(url, rspDiv, msgDiv) {
    var totalAdvance = parseFloat(document.getElementById("totalAdvance").value);
    if (totalAdvance > 0) {
        sendRequest(url, rspDiv, msgDiv);
        document.getElementById("tbDescription").focus();
    }
    else
        alert("You havent added any details with amounts. CANNOT PROCEED");
}

function PostFunds(postUrl, tabIndex) {
    var password = document.getElementById("password").value;
    var rfaId = document.getElementById("rfpId").value;
    var url = "/FundPosting/AuthenticatePoster?password=" + password;
    document.getElementById("statusMsg").innerHTML = "Authenticating Poster...";
    document.getElementById("btnPost").disabled = true;
    document.getElementById("btnCancel").disabled = true;
    //send request to server
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText != "SUCCESS") {
        document.getElementById("statusMsg").innerHTML = "<font color='Red'>ACCESS DENIED</font>";
        document.getElementById("btnPost").disabled = false;
        document.getElementById("btnCancel").disabled = false;
        return;
    }
    document.getElementById("statusMsg").innerHTML = "<font color='Green'>Posting Funds...</font>";
    url = postUrl + rfaId;
    //send request to server
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    if (xmlHttp.responseText == "SUCCESS") {
        document.getElementById("statusMsg").innerHTML = "";
        document.getElementById("btnPost").disabled = false;
        document.getElementById("btnCancel").disabled = false;
        CloseDialog();
        $(document).ready(function () {
            $.Zebra_Dialog('<strong>Funds posted successfully</strong>', {
                'modal': false,
                'type': 'information',
                'title': 'Posting Complete',
                'auto_close': 3000
            });
        });
        url = "/FundPosting/FundPostingList";
        sendRequest(url, "FundPostingDiv", "loading");
        initPostFundsView(tabIndex);
    }
    else {
        document.getElementById("statusMsg").innerHTML = "";
        document.getElementById("btnPost").disabled = false;
        document.getElementById("btnCancel").disabled = false;
        CloseDialog();
        $(document).ready(function () {
            $.Zebra_Dialog('<strong>Funds were not posted successfully. Please try again later.</strong>', {
                'type': 'error',
                'title': 'Fund Posting Error'
            });
        });
    }
}

function initPostFundsView(tabIndex) {
    initTabs(tabIndex);
    if (document.getElementById('table') != null) {
        initSorter();
        sorter.size(20);
    }
    if (document.getElementById('table2') != null) {
        initSorter2();
        sorter2.size(20);
    }
    if (document.getElementById('table3') != null) {
        initSorter3();
        sorter3.size(20);
    }
    if (document.getElementById('table4') != null) {
        initSorter4();
        sorter4.size(20);
    }
    if (document.getElementById('table5') != null) {
        initSorter5();
        sorter5.size(20);
    }
}

function CheckForUser(email) {
    var url = "/PasswordReset/CheckForUser/" + email;
    //send request to server
    xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    return xmlHttp.responseText;
}

function loadOrItems() {
    var select = document.getElementById("orId");
    if (select != null && select.value != "") {
        var poId = document.getElementById("poId").value;
        var orId = select.value;
        var pdId = document.getElementById("pdId").value;
        var url = "/PurchaseOrder/SelectOR?PoId=" + poId + "&OrId=" + orId + "&PdId=" + pdId;
        document.getElementById("popup-loading").style.display = "inline";
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("popup-loading").style.display = "none";
        document.getElementById("popup-page").innerHTML = xmlHttp.responseText;
        clearPOItemNumbers();
        CenterPopup();
    }
}

function GetProjectDonors4Rebooking(select) {
    if (select.value != "") {
        var url = "/Budget/GetProjectDonors/" + select.value;
        sendRequest(url, "pdDiv", "loading");
    }
}

function GetBudgetLines4Rebooking(select) {
    if (select.value != "") {
        var url = "/Budget/GetBudgetLines/" + select.value;
        sendRequest(url, "blDiv", "loading");
    }
}

function ValidateTenderNumber() {
    var poId = document.getElementById("poId").value;
    var tenderNumber = document.getElementById("tenderNumber").value;
    var url = "/PurchaseOrder/ValidateTenderNumber?TenderNumber=" + tenderNumber + "&PurchaseOrderId=" + poId;
    sendRequest(url, "tncheck", "tncheck");
    if (document.getElementById("tncheck").innerHTML == "SUCCESS") {
        document.getElementById("tnMsg").innerHTML = "";
        document.getElementById("tncheck").innerHTML = "";
        return CheckWaiver();
    }
    document.getElementById("tncheck").innerHTML = "";
    document.getElementById("tnMsg").innerHTML = "Tender Number already exists";
    //alert("Tender Number '" + tenderNumber + "' is already in use. CANNOT PROCEED!");
    return false;
}

function CheckWaiver()
{
    if (document.getElementById("WaiverDD").style.display == "inline")
    {
        if (document.getElementById("yesNo").value == "")
        {
            alert("You must specify if you have received a waiver or not.");
            return false;
        }
    }
    return true;
}

function loadProjectDonors(select)
{
    if (select.value != "") {
        var url = "/PurchaseOrder/GetProjectDonors/" + select.value;
        sendRequest(url, "projectDiv", "popup-loading");
    }
}

function loadProjectDonors4OR(select) {
    if (select.value != "") {
        var url = "/PurchaseOrder/GetProjectDonorsWithOR/" + select.value;
        sendRequest(url, "projectDiv", "popup-loading");
    }
}

function loadPdProcurementPlan(select)
{
    if (select.value != "") {
        var poId = document.getElementById("poId").value;
        var url = "/PurchaseOrder/LoadAddPPItemsToPO?poId=" + poId + "&pdId=" + select.value;
        sendRequest(url, "popup-page", "popup-loading");
    }
}

function loadPdOrderRequests(select)
{
    if (select.value != "")
    {
        var poId = document.getElementById("poId").value;
        var url = "/PurchaseOrder/LoadAddPPItemsToPO?poId=" + poId + "&pdId=" + select.value;
        sendRequest(url, "popup-page", "popup-loading");
    }
}

function ChangeTenderingType(select)
{
    var poId = document.getElementById("poId").value;
    var typeId = select.value;
    var defaultTT = document.getElementById("defaultTenderingType").value;
    var selectedTT = select.options[select.selectedIndex].innerHTML;
    var url = "/PurchaseOrder/ChangeTenderingType?typeId=" + typeId + "&poId=" + poId;
    sendRequest(url, "responseText", "popup-loading");
    if (document.getElementById("responseText").innerHTML == "CheckWaiver") {
        document.getElementById("WaiverMsg").innerHTML = "For Total Value of this PO, the Tendering Type is supposed to be <b><i>" + defaultTT + "</i></b>. To downgrade to <b><i>" + selectedTT + "</i></b>, you needed to have acquired a waiver from Headquarters. Do you have a go ahead to do so?";
        document.getElementById("WaiverMsg").style.display = "inline";
        document.getElementById("WaiverDD").style.display = "inline";
    }
    else {
        document.getElementById("WaiverMsg").style.display = "none";
        document.getElementById("WaiverDD").style.display = "none";
    }
    document.getElementById("responseText").innerHTML = "";
}

function ReloadOR(orId)
{
    if (document.getElementById("requestDiv") != null)
    {
        sendRequest("/OrderRequest/ViewOrderRequestItems/" + orId, "requestDiv", "loading");
    }
}

function ReloadPO(poId) {
    if (document.getElementById("PODiv") != null) {
        sendRequest("/PurchaseOrder/ViewPurchaseOrdersDetails/" + poId, "PODiv", "loading");
    }
}

function ReloadCC(ccId) {
    if (document.getElementById("CompletionDiv") != null) {
        sendRequest("/CompletionCertificate/ViewCCDetails/" + ccId, "CompletionDiv", "loading");
    }
}

function ReloadPP(ppId) {
    if (document.getElementById("ppDiv") != null) {
        sendRequest("/ProcurementPlan/ViewPP/" + ppId, "ppDiv", "loading");
    }
}

function ReloadGRN(grnId) {
    if (document.getElementById("ContentDiv") != null) {
        sendRequest("/GoodsReceivedNote/ViewGRNDetails?GRNId=" + grnId + "&Verify=false", "ContentDiv", "loading");
    }
}

function ReloadWRO(wroId) {
    if (document.getElementById("wrfDiv") != null) {
        sendRequest("/WRForm/ViewNRNDetails/" + wroId, "wrfDiv", "loading");
    }
}

function ReloadRFP(rfpId) {
    if (document.getElementById("R4PDiv") != null) {
        sendRequest("/Request4Payment/ViewR4PDetails/" + rfpId, "R4PDiv", "loading");
    }
}