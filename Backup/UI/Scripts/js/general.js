function ShowPopupDiv() {
    document.getElementById("popupDiv").style.display = "inline";
    $(function () {
        $("#popupDiv").draggable({
            cursor: 'move',
            handle: 'h3'
        });
    });
}

function HidePopupDiv() {
    var actionStatus = document.getElementById("actionStatus").value;
    var statusMsg = document.getElementById("statusMsg").value;
    if (actionStatus == "") CloseDialog();
    else alert(statusMsg);
}

function CancelPopup() {
    document.getElementById("popupDiv").style.display = "none";
}

function expandIcon(x) {
    var img = document.getElementById("img" + x);
    var table = document.getElementById("table" + x);
    if (img.alt == "Expand Category") {
        img.src = "../../Content/images/Minus-Green-Button-icon.png";
        img.alt = "Collapse Category";
        table.style.display = "inline";
        table.focus();
    } else {
        img.src = "../../Content/images/Add-Green-Button-icon.png";
        table.style.display = "none";
        img.alt = "Expand Category";
    }

}


function usermsgg() {
    var response = document.getElementById("actionStatus").value;
    var msg = document.getElementById("statusMsg").value;
    if (response == "") {
        //ClearForm();
        return;
    }
    $(document).ready(function () {
        if (response == 1) {
            new $.Zebra_Dialog('<strong>' + msg + '</strong>',
             {
                 'buttons': false,
                 'modal': false,
                 'position': ['right - 20', 'top + 80'],
                 'auto_close': 5000
             });
            //ClearForm();
        } else {
            $.Zebra_Dialog('<strong>' + msg + '</strong>', {
                'type': 'error',
                'title': 'Error'
            });
            CloseDialog();
        }
    });
}

function usermsg(msg) {
    if (msg === undefined)
        msg = 'Process has completed successfully';
    $(document).ready(function () {
        new $.Zebra_Dialog('<strong>' + msg + '</strong>',
        {
            'buttons': false,
            'modal': false,
            'position': ['right - 20', 'top + 80'],
            'auto_close': 5000
        });
    });
}

function errormsg(msg) {
    $(document).ready(function () {
        msg = typeof msg !== 'undefined' ? msg : 'An error has occurred';
        $.Zebra_Dialog('<strong>' + msg + '</strong>', {
            'type': 'error',
            'title': 'Error'
        });
    });
}

function ValidatedPasswords() {
    if (document.getElementById("newpassword") != null) {
        var newpass = document.getElementById("newpassword").value;
        var confirmpass = document.getElementById("confirmpassword").value;
        if (newpass !== confirmpass) {
            errormsg("Password and Confirm Password do not match.");
            return false;
        }
    }
    return true;
}

function PasswordChangeResponse() {
    if (document.getElementById("statusmsg") == null) {
        usermsg("Password changed successfully.");
    }
}

function RequestPasswordReset() {
    var email = Trim(document.getElementById("email").value);
    if (email == "")
        return false;
    if (!validateEmail(email)) {
        document.getElementById("statusmsg").innerHTML = "<font color=\"Red\">Invalid Email</font>";
        return false;
    }
    //Status :: checking for user
    document.getElementById("statusmsg").innerHTML = "<font color=\"Blue\">Checking for user...</font>";
    var response = CheckForUser(email);
    if (response != "SUCCESS") {
        document.getElementById("statusmsg").innerHTML = "<font color=\"Red\">User with email '" + email + "' doesn't exists. CANNOT PROCEED</font>";
        return false;
    }
    document.getElementById("statusmsg").innerHTML = "<font color=\"Blue\">User found. Sending link to email... Please wait.</font>";
    var url = "/PasswordReset/HandleRequest";
    sendRequest(url, "PasswordResetDiv", "loading");
    return false;
}

function AdjustComputedTotals() {

    var Quatity = parseFloat(document.getElementById("Quatity").value);
    var UnityPrice = parseFloat(document.getElementById("UnityPrice").value);
    var txttotal = document.getElementById("TotalPrice");
    txttotal.value = roundNumber(Quatity * UnityPrice, 2);
}

function GetRFPPVDetails(select) {
    if (select.value != "") {
        var url = "/PaymentVoucher/LoadRFPItemDetais?RFPId=" + select.value;
        sendRequest(url, "pvDiv", "loading");
    }
}

function GetPOGNDetails(select, viewname) {
    if (select.value != "") {
        var url = "/GoodsReceivedNote/LoadGRNItemDetais?PoId=" + select.value + "&viewName=" + viewname;
        sendRequest(url, "ContentDiv", "loading");
    }

    //ClearFormById('frmGRN');
}

function GetDisableNextService(select) {
    var nextmileage = document.getElementById("nextmileage");
    if (select.value != "") {
        if (select.value == "Service") {
            nextmileage.disabled = false;
        } else { nextmileage.value = 0; nextmileage.disabled = true; }

        if (select.value == "Maintenance") {
            jQuery(function () {
                jQuery('#maintenaceTypeId').show();
            });
        } else {
            jQuery(function () {
                jQuery('#maintenaceTypeId').hide();
            });
        }
    } else {
        nextmileage.disabled = false;
        jQuery(function () {
            jQuery('#maintenaceTypeId').hide();
        });
    }
}

function validateTemplate() {
    if (document.getElementById("POId").value == "") {
        alert('Please select a purchase order No.'); return false
    } else {
        var delieveredby = document.getElementById("txtdeliveredbyId").value;
        document.getElementById("txtdeliveredbyId").value = delieveredby || "  ";
        return true;
    }
}

function getServiceCheckList(select) {
    var fleetId = document.getElementById("fleetId").value;
    var gInfoId = document.getElementById("gInfoId").value;
    jQuery.ajax({
        UpdateTargetId: "popup-page",
        LoadingElementId: 'popup-loading',
        url: '/FleetManager/LoadServiceCheckListPopUp?categoryId=' + select.value + '&FleetId=' + fleetId + '&gInfoId=' + gInfoId,
        async: false,

        OnBegin: ShowDialogue('Service Check List'),
        success: function (data) {
            $('#popup-page').html(data);
            CenterPopup();
        }
    });
}

function GetBudgetLines() {

    jQuery('#dplProjectNo').live('change', function () {
        jQuery.ajax({
            UpdateTargetId: "PBlines",
            LoadingElementId: 'loading',
            HttpMethod: 'Post',
            url: '/OrderRequest/GetBudgetLines',
            data: {

                id: $('#dplProjectNo option:selected').val()

            },
            success: function (data) {

                $('#PBlines').html(data);
            }
        });
    });
}


function GetR4PaymentDetails2() {

    if ($('#dplpo option:selected').val() == "")
        return;
    else {
        jQuery('#dplpo').live('change', function () {
            jQuery.ajax({
                UpdateTargetId: "R4PDiv",
                LoadingElementId: 'loading',
                HttpMethod: 'Post',
                url: '/Request4Payment/LoadR4P',
                data: {

                    id: $('#dplpo option:selected').val()

                },
                success: function (data) {
                    $('#R4PDiv').html(data);
                    cleanRFPNumbers();
                    return;
                }
            });
        });
    }
}

function GetR4AdvanceDetails2() {

    jQuery('#dplpo').live('change', function () {
        jQuery.ajax({
            UpdateTargetId: "R4ADiv",
            LoadingElementId: 'loading',
            HttpMethod: 'Post',
            url: '/Request4Advance/LoadR4A',
            data: {

                id: $('#dplpo option:selected').val()

            },
            success: function (data) {
                $('#R4ADiv').html(data);
            }
        });
    });
}

function clearTADiv() {
    jQuery(function () {
        jQuery('#TADetailsDiv').html('');
    });
}


function clearDiv() {
    jQuery(function () {
        jQuery('#requestItemDiv').html('');
    });
}

function SwitchDisplay(select) {
    if (select.value != "") {
        if (select.value == "Partner") {
            jQuery(function () {
                jQuery('#handedoverid').show();
                jQuery('#currentprojectid').hide();
            });
        } else {
            jQuery(function () {
                jQuery('#currentprojectid').show();
                jQuery('#handedoverid').hide();
            });
        }
    }
}

function ComputeTotals() {
    if (document.getElementById("Quatity").value != "" && document.getElementById("UnityPrice").value != "") {
        var Quatity = parseFloat(document.getElementById("Quatity").value);
        var UnityPrice = parseFloat(document.getElementById("UnityPrice").value);
        var txttotal = document.getElementById("TotalPrice");
        document.getElementById("UnityPrice").value = roundNumber(UnityPrice, 2);
        txttotal.value = roundNumber(Quatity * UnityPrice, 2);
    }
}

function ComputeTotalDistance() {
    if (document.getElementById("txtstartMilleage").value != "" && document.getElementById("txtendMilleage").value != "") {
        var StartM = parseInt(document.getElementById("txtstartMilleage").value);
        var EndM = parseInt(document.getElementById("txtendMilleage").value);
        if (EndM < StartM) {
            $.Zebra_Dialog('<strong>End milleage cant be less than the start milleage</strong>', {
                'type': 'information',
                'title': 'Information'
            });
            document.getElementById("txtendMilleage").value = null;
        } else {
            var txttotal = document.getElementById("txttotaldistance");
            txttotal.value = roundNumber(EndM - StartM, 2);
        }
    }
}

function ValidateSalvageValue(element) {
    if (document.getElementById("purcahseValue").value != "") {
        var Pxvalue = parseInt(document.getElementById("purcahseValue").value);
        var salvageValue = parseInt(element.value);
        if (salvageValue > Pxvalue) {
            $.Zebra_Dialog('<strong>The salvage value cant exceed the purchase value</strong>', {
                'type': 'information',
                'title': 'Information'
            });
            element.value = null;
        }
    }
}

function ValidateFirstMilleage() {
    var lastmilg = document.getElementById("lastmilg").value;
    var currentmilg = document.getElementById("currentmilg").value;
    var startmilg = document.getElementById("txtstartMilleage");
    if (startmilg.value < lastmilg) {
        $.Zebra_Dialog('<strong>The start milleage must be greater or equal to the last milleage ( ' + lastmilg + ' )</strong>', {
            'type': 'information',
            'title': 'Information'
        });
        startmilg.value = currentmilg;
    }
}

function FeedbackDialog(msgStr) {

    var type = 'information';
    var title = 'Feedback';
    var msg = '';

    var detail = msgStr.split('|');

    if (detail.length == 3) {
        type = detail[0];
        title = detail[1];
        msg = detail[2];
    }

    if (detail.length == 2) {
        title = detail[0];
        msg = detail[1];
    }


    if (detail.length == 1) {
        msg = detail[0];
    }

    $.Zebra_Dialog(msg, {
        'type': type,
        'title': title
    });
}


function CheckIfOneIsSelected() {
    var n = $("input:checked").length;
    if (n == 0) {
        alert('Atleast one item should be selected');
        return false;
    }
    else if (document.getElementById("orItemCount") != null) {
        var orItemCount = document.getElementById("orItemCount").value;
        var oneChecked = false;
        for (var i = 0; i < orItemCount; i++) {
            if (document.getElementById("EntityORItems_" + i + "__AddToList").checked) {
                oneChecked = true;
                break;
            }
        }
        if (!oneChecked) {
            alert('Atleast one item should be selected');
            return false;
        }
    }
    return true;
}

function VerifyPPApproval() {
    var n = $("input:checked").length;
    if (n == 0) {
        alert('NO single item has been selected for approval. Cannot Proceed');
        return false;
    }
    return true;
}

function disableTAForm(status) {
    if (status > 0) {
        document.getElementById("taGenBtn").style.display = "none";
        document.getElementById("taORList").disabled = true;
        //document.getElementById("editLink").style.display = "inline";
    }
    else {
        document.getElementById("taGenBtn").disabled = false;
        document.getElementById("taORList").disabled = false;
        //document.getElementById("editLink").style.display = "none";
    }
}

function initDatePicker() {
    jQuery.noConflict();
    $(".datepicker").datepicker
        ({
            dateFormat: 'yy-MM-dd',
            showStatus: true,
            showWeeks: true,
            highlightWeek: true,
            numberOfMonths: 1,
            showAnim: "scale",
            showOptions: {
                origin: ["top", "left"]
            }
        });
}

function cleanTANumbers() {
    var num;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("tb" + i) == null)
            break;
        num = document.getElementById("tb" + i).value;
        num = roundNumber(num, 2);
        if (num > 0)
            document.getElementById("tb" + i).value = num;
        else
            document.getElementById("tb" + i).value = "";
    }
    if (document.getElementById("transportTb") != null) {
        num = document.getElementById("transportTb").value;
        num = roundNumber(num, 2);
        if (num > 0)
            document.getElementById("transportTb").value = num;
        else
            document.getElementById("transportTb").value = "";
    }
    initDatePicker();
}

function validateSupplier() {
    var num;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("tb" + i) == null)
            break;
        num = document.getElementById("tb" + i).value;
        if (Trim(num) == "") {
            alert("Unit cost is required. Cannot Proceed!");
            return false;
        }
    }
    var supplier = document.getElementById("cbSupplier").value
    if (Trim(supplier) == "") {
        alert("Supplier is required. Cannot Proceed!");
        return false;
    }
    return true;
}

function Trim(sVar) {
    return sVar.replace(/^\s+|\s+$/g, "");
}

function ClearForm() {
    $(":input").not(":button, :submit, :reset, :hidden, .datepicker").each(function () {
        this.value = "";
    });
    $("textarea").each(function () {
        this.value = "";
    });
}

function ClearFormById(formId) {
    $(formId).find(":input").not(":button, :submit, :reset, :hidden, .datepicker").each(function () {
        this.value = "";
    });
    $("textarea").each(function () {
        this.value = "";
    });
}

function ClearOtherAssetForm() {
    var checkbox = document.getElementById("useLifeSpan");
    var salvageTb = document.getElementById("salvageValue");
    var purchaseTb = document.getElementById("purcahseValue");
    var depTb = document.getElementById("depreciation");
    var lifespanTb = document.getElementById("lifespan");
    var projNoSelect = document.getElementById("dplProNo");
    var weightTb = document.getElementById("weight");
    checkbox.checked = false;
    salvageTb.value = purchaseTb.value = depTb.value = lifespanTb.value = "";
    projNoSelect.value = weightTb.value = "";
}

function clearAssetForm() {
    document.getElementById("txtweight").value = "";
    document.getElementById("txtsalvageValue").value = "";
}

function ClearFormById(formId, control) {
    //Eliminate this ref No. control
    var refNo = "";
    if (document.getElementById(control) != null) {
        refNo = document.getElementById(control).value
    }
    $(formId).find(":input").not(":button, :submit, :reset, :hidden, .datepicker").each(function () {
        this.value = "";
    });
    $("textarea").each(function () {
        this.value = "";
    });

    if (document.getElementById(control) != null) {
        document.getElementById(control).value = refNo;
    }
}

function ClearFormWithDTP() {
    $(":input").not(":button, :submit, :reset, :hidden, .datepicker").each(function () {
        this.value = "";
    });
    $("textarea").each(function () {
        this.value = "";
    });
    initDatePicker();
}

function checkLineLimit(seed, count) {
    var blAmount = parseFloat(document.getElementById("lineAmount" + seed).value);
    var lineLimit = parseFloat(document.getElementById("lineLimit" + seed).value);
    var total = 0;
    if (blAmount > lineLimit) {
        alert("You cannot specify an amount greater than what was committed on this Budget Line");
        document.getElementById("lineAmount" + seed).value = roundNumber(lineLimit, 2);
    }
    for (i = 1; i <= count; i++) {
        total += parseFloat(document.getElementById("lineAmount" + i).value);
    }
    total = roundNumber(total, 2);
    document.getElementById("tbTotalAmount").value = total;
}

function cleanRFPNumbers() {
    var total = 0;
    for (i = 1; i > 0; i++) {
        if (document.getElementById("lineAmount" + i) == null)
            break;
        total += parseFloat(document.getElementById("lineAmount" + i).value);
        document.getElementById("lineAmount" + i).value = roundNumber(parseFloat(document.getElementById("lineAmount" + i).value), 2);
    }
    total = roundNumber(total, 2);
    document.getElementById("tbTotalAmount").value = total;
}

function clenaRFANumbers() {
    var total = parseFloat(document.getElementById("TotalPrice").value);
    total = roundNumber(total, 2);
    document.getElementById("TotalPrice").value = total;
}

function validateRFP() {
    if (Trim(document.getElementById("paymentType").value) == "") {
        alert("Payment Type is required");
        return false;
    }

    if (Trim(document.getElementById("subject").value) == "") {
        alert("Subject is required");
        return false;
    }

    //    if (Trim(document.getElementById("supplier").value) == "") {
    //        alert("Supplier is required");
    //        return false;
    //    }

    if (Trim(document.getElementById("paymentTerm").value) == "") {
        alert("Payment Terms are required");
        return false;
    }

    var requestType = document.getElementById("FullPayment").checked;
    if (!requestType)
        requestType = document.getElementById("Rate_Instalment").checked;
    if (!requestType)
        requestType = document.getElementById("Adv_Payment_percentage").checked;
    if (!requestType)
        requestType = document.getElementById("Adv_Final_Payment_percentage").checked;
    if (!requestType) {
        alert("Request For must be selected.");
        return false;
    }
    return true;
}

function initTabs(index) {
    $(function () {
        $("#tabs").tabs({ selected: index });
    });
}

function initTAEdit() {
    if (document.getElementById("supplierAdded") == null)
        return;
    var supplierAdded = document.getElementById("supplierAdded").value;
    var taId = document.getElementById("taId").value;
    var url = "";
    if (supplierAdded == 1)
        url = "/TenderAnalysis/BackToAddSuppliers/" + taId;
    else
        url = "/TenderAnalysis/SelectItems/" + taId;
    sendRequest(url, "TADetailsDiv", "loading");
}

function computePOItemTotals(index) {
    var quantity = parseFloat(document.getElementById("quantity" + index).value);
    var unitCost = parseFloat(document.getElementById("unitPrice" + index).value);
    var totalCost = quantity * unitCost;
    document.getElementById("totalPrice" + index).value = roundNumber(totalCost, 2);
    checkBLBalance(index);
}

function clearPOItemNumbers() {
    var quantity, unitCost, totalCost;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("quantity" + i) == null)
            break;
        quantity = parseFloat(document.getElementById("quantity" + i).value);
        unitCost = parseFloat(document.getElementById("unitPrice" + i).value);
        totalCost = quantity * unitCost;
        document.getElementById("unitPrice" + i).value = roundNumber(unitCost, 2);
        document.getElementById("totalPrice" + i).value = roundNumber(totalCost, 2);
    }
}

function VerifyEditPO(count) {
    for (i = 0; i < count; i++) {
        if (!checkBLBalance(i)) {
            return false;
        }
    }
    return true;
}

function VerifyPOItemNumbers(count) {
    if (!CheckIfOneIsSelected())
        return false;
    for (i = 0; i < count; i++) {
        if (document.getElementById("addToPO" + i) != null && document.getElementById("addToPO" + i).checked) {
            if (!checkBLBalance(i)) {
                return false;
            }
        }
    }
    return true;
}

function checkUncheck(tb, checkbox) {
    if (Trim(tb.value) != "")
        document.getElementById(checkbox).checked = true;
    else
        document.getElementById(checkbox).checked = false;
}

function validateConsigneeEmails() {
    var email1 = document.getElementById("consigneeEmail1").value;
    var email2 = document.getElementById("consigneeEmail2").value;

    if (email1 != "" && !validateEmail(email1)) {
        $("#csEmail1").addClass('gridwarning');
        document.getElementById("csEmail1").title = "Invalid email";
        return false;
    }
    else {
        $("#csEmail1").removeClass('gridwarning');
        document.getElementById("csEmail1").title = "";
    }

    if (email2 != "" && !validateEmail(email2)) {
        $("#csEmail2").addClass('gridwarning');
        document.getElementById("csEmail2").title = "Invalid email";
        return false;
    }
    else {
        $("#csEmail2").removeClass('gridwarning');
        document.getElementById("csEmail1").title = "";
    }

    //Validate checkboxes
    if (document.getElementById("otherCheck").checked && Trim(document.getElementById("otherTb").value) == "") {
        alert("Other required document must be specified if checkbox is checked.");
        return false;
    }
    if (document.getElementById("pfgCheck").checked && Trim(document.getElementById("pfgPercentage").value) == "") {
        alert("Pre-financing Guarantee Percentage must be specified if checkbox is checked.");
        return false;
    }
    return true;
}

function validateEmail(elementValue) {
    var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    return emailPattern.test(elementValue);
}

function reloadTabs() {
    var docType = document.getElementById("activityCode").value;
    var index = 0;
    switch (docType) {
        case "OR":
            index = 0;
            break;
        case "PO":
            index = 1;
            break;
        case "RFP":
            index = 2;
            break;        
        case "PP":
            index = 3;
            break;
        case "GRN":
            index = 3;
            break;
        case "WRN":
            index = 4;
            break;
        case "CC":
            index =5;
            break;
        case "PARAMS":
            index = 7;
            break;
        default:
            index = 0;
            break;
    }
    initTabs(index);
}

function ValidatedApprovers() {
    if ((document.getElementById("staffResponsible").value == document.getElementById("delegatedStaff").value) && document.getElementById("unlimited") == null) {
        errormsg("You cannot assign the same person as Staff Responsible and Delegate");
        return false;
    }
    return true;
}

function CleanupFLimit() {
    if (document.getElementById("flLimit") != null) {
        var limit = parseFloat(document.getElementById("flLimit").value);
        limit = roundNumber(limit, 2);
        document.getElementById("flLimit").value = limit;
    }
}

function CleanUpSublineNos() {
    var amount = parseFloat(document.getElementById("amount").value);
    amount = roundNumber(amount, 2);
    document.getElementById("amount").value = amount;
}

function ViewBudgetInSelectedCurrency(select) {
    var projectDonorId = document.getElementById("projectId").value;
    var url = "/Budget/ViewBudgetInCurr?Id=" + projectDonorId + "&CurrencyId=" + select.value;
    sendRequest(url, 'budgetDiv', 'loading');
}

function ValidateGRN() {
    var quantity = "";
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("Qty" + i) == null)
            break;
        quantity = Trim(document.getElementById("Qty" + i).value);
        if (quantity == "" || !isNumber(quantity)) {
            errormsg('Some of the quantities entered are invalid.');
            return false;
        }
    }
    return true;
}

//check for numbers
function isNumber(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function ValidateExRate() {
    if (Trim(document.getElementById("mainCurrency").value) != "") {
        if (Trim(document.getElementById("mainCurrency").value) == Trim(document.getElementById("exCurrency").value)) {
            errormsg("Defining rate for the same currency is not allowed!");
            return false;
        }
        return true;
    }
    return false;
}

function RefreshLocDDs() {
    var content = document.getElementById("tempLocation").innerHTML.toString();
    var contArr = content.split("###");
    document.getElementById("reqDelDiv").innerHTML = contArr[0];
    document.getElementById("finalDelivDiv").innerHTML = contArr[1];
    document.getElementById("tempLocation").innerHTML = "";
}

function ComputePPTotals(index) {
    var qty = parseFloat(document.getElementById("qty" + index).value);
    var unitCost = parseFloat(document.getElementById("unitCost" + index).value);
    var totalCost = qty * unitCost;
    document.getElementById("totalCost" + index).value = roundNumber(totalCost, 2);
}

function dynamicIframe(params) {
    var tmpName = 'tmp-div';
    var iframe = document.getElementById(tmpName);
    if (iframe) {
        iframe.parentNode.removeChild(iframe);
    }

    iframe = document.createElement("iframe");

    iframe.src = params.url;
    iframe.style.display = "none";
    document.body.appendChild(iframe);
}

function WaitForFineUploader(msgDiv) {
    document.getElementById(msgDiv).innerHTML = "Waiting for upload to complete...";
    var uploadStatus = document.getElementById("uploadStatus").value;
    var uploadStatusValue = document.getElementById("uploadStatusValue").value;
    do {
        uploadStatus = document.getElementById("uploadStatus").value;
        uploadStatusValue = document.getElementById("uploadStatusValue").value;
    }
    while (uploadStatus != "complete");
    document.getElementById(msgDiv).innerHTML = "";
    if (uploadStatusValue == 1) {
        return true;
    }
    else
        return false;
}

function initFineUploader(url) {
    $('#fineUploaderElementId').fineUploader({
        request: {
            endpoint: url,
            allowedExtensions: ['jpg', 'jpeg', 'png', 'gif', 'bpm'],
            sizeLimit: 1024000, // max size, about 1MB
            minSizeLimit: 0 // min size
        }
    }).on('submit', function (event, id, filename) {
        document.getElementById("uploadStatus").value = "progress";
        document.getElementById("uploadStatusValue").value = 0;
    })
    .on('error', function (event, id, filename, reason) {
        document.getElementById("uploadStatus").value = "complete";
        document.getElementById("uploadStatusValue").value = 0;
    })
    .on('complete', function (event, id, filename, responseJSON) {
        document.getElementById("uploadStatus").value = "complete";
        document.getElementById("uploadStatusValue").value = 1;
    });
}

function initFineFileUploader(url) {
    $('#fineUploaderElementId').fineUploader({
        request: {
            endpoint: url,
            allowedExtensions: ['pdf'],
            sizeLimit: 3024000, // max size, about 3MB
            minSizeLimit: 0 // min size
        }
    }).on('submit', function (event, id, filename) {
        document.getElementById("uploadStatus").value = "progress";
        document.getElementById("uploadStatusValue").value = 0;
    })
    .on('error', function (event, id, filename, reason) {
        document.getElementById("uploadStatus").value = "complete";
        document.getElementById("uploadStatusValue").value = 0;
    })
    .on('complete', function (event, id, filename, responseJSON) {
        document.getElementById("uploadStatus").value = "complete";
        document.getElementById("uploadStatusValue").value = 1;
    });
}

function AjaxFailure() {
    if (window.console) {
        console.log('AJAX error');
    }
}

function FullPartRebook(checkbox) {
    var tbAmount = document.getElementById("tbAmount");
    if (checkbox.checked) {
        var origVal = parseFloat(document.getElementById("originalAmount").value);
        tbAmount.value = origVal.toFixed(2);
        SetTextBoxReadonly('tbAmount', true);
    }
    else {
        SetTextBoxReadonly('tbAmount', false);
    }
}

function SetTextBoxReadonly(textboxId, readonly) {
    var tbAmount = document.getElementById(textboxId);
    if (readonly && !tbAmount.hasAttribute('readonly')) {
        tbAmount.setAttribute('readonly', 'readonly');
    }
    else {
        tbAmount.removeAttribute('readonly');
    }
}

function CheckRebookAmount() {
    var rebookAmount = parseFloat(document.getElementById("tbAmount").value);
    var origVal = parseFloat(document.getElementById("originalAmount").value);
    if (rebookAmount > origVal) {
        document.getElementById("tbAmount").value = origVal.toFixed(2);
        document.getElementById("fullRebook").checked = true;
        SetTextBoxReadonly('tbAmount', true);
    }
}