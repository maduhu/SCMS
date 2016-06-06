function onChangeCurrency() {
    var select = document.getElementById("convCurrencyId");
    var selectText = "";
    if($(select).is("select")) 
        selectText = select.options[select.selectedIndex].innerHTML;
    else
        selectText = document.getElementById("convCurrencyId").value;
    if (document.getElementById("orCurrency") != null)
        document.getElementById("orCurrency").innerHTML = selectText;
    if (document.getElementById("fxRateInfo") != null && document.getElementById("expenseCurrency") != null) {
        var expenseCurr = document.getElementById("expenseCurrency").value;
        document.getElementById("fxRateInfo").innerHTML = "*** 1 " + expenseCurr + " = ?? " + selectText;
    }
}

function onChangeProjectNo(select, i) {
    i = i - 1;
    var cbBLId = "ORItems_" + i + "__BudgetLineID";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "ORItems_" + i + "__BudgetLineID";
    var url = "/OrderRequest/GetBudgetLineForORItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForPO(select, i) {
    i = i - 1;
    var cbBLId = "POItems_" + i + "__BudgetLineID";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "POItems_" + i + "__BudgetLineID";
    var url = "/OrderRequest/GetBudgetLineForPOItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForRFP(select, i) {
    i = i - 1;
    var cbBLId = "paymentDetails_" + i + "__BudgetLineId";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "paymentDetails_" + i + "__BudgetLineId";
    var url = "/OrderRequest/GetBudgetLineForRFPItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForRFA(select, i) {
    i = i - 1;
    var cbBLId = "PaymentDetails_" + i + "__BudgetLineID";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "PaymentDetails_" + i + "__BudgetLineID";
    var url = "/OrderRequest/GetBudgetLineForRFAItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForECF(select, i) {
    i = i - 1;
    var cbBLId = "ExpenseClaimItems_" + i + "__BudgetLineId";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "ExpenseClaimItems_" + i + "__BudgetLineId";
    var url = "/OrderRequest/GetBudgetLineForECFItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForSPM(select, i) {
    i = i - 1;
    var cbBLId = "SalaryPaymentItems_" + i + "__BudgetLineId";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "SalaryPaymentItems_" + i + "__BudgetLineId";
    var url = "/OrderRequest/GetBudgetLineForSPMItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForCreatePO(select, i) {
    i = i - 1;
    var cbBLId = "ORItems_" + i + "__EntityORItem_BudgetLineId";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "ORItems_" + i + "__BudgetLineId";
    var url = "/OrderRequest/GetBudgetLineForCreatePOItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function onChangeProjectNoForCreatePOTA(select, i) {
    i = i - 1;
    var cbBLId = "ORTenderItems_" + i + "__TBQuoteEntity_OrderRequestItem_BudgetLineId";
    var parent = document.getElementById(cbBLId).parentNode;
    var blSelect = "ORItems_" + i + "__BudgetLineId";
    var url = "/OrderRequest/GetBudgetLineForCreatePOTAItems/" + select.value;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    //document.getElementById(msgDiv).style.display = "none";
    var respText = xmlHttp.responseText.replace("#", i);
    respText = respText.replace("#", i);
    parent.innerHTML = respText;
}

function AuthCompleteWithFundsCheck() {
    if (document.getElementById("fundsInsufficientFor") != null) {
        if (document.getElementById("fundsInsufficientFor").value == "OR") {
            CenterPopupOR();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "PO") {
            CenterPopupPO();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "RFP") {
            CenterPopupRFP();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "RFA") {
            CenterPopupRFP();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "ECF") {
            CenterPopupECF();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "SPM") {
            CenterPopupSPM();
            return;
        }
    }
    CenterPopup(); 
    LoadRequests();
}

function ReviewComplete(msg, title) {
    if (document.getElementById("fundsInsufficientFor") != null) {
        if (document.getElementById("fundsInsufficientFor").value == "OR") {
            CenterPopupOR();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "PO") {
            CenterPopupPO();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "RFP") {
            CenterPopupRFP();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "RFA") {
            CenterPopupRFP();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "ECF") {
            CenterPopupECF();
            return;
        }
        if (document.getElementById("fundsInsufficientFor").value == "SPM") {
            CenterPopupSPM();
            return;
        }
    }
    LoadRequests();
    CloseDialog();
    $(document).ready(function () {
        $.Zebra_Dialog('<strong>' + msg + '</strong>', {
            'modal': false,
            'type': 'information',
            'title': title,
            'auto_close': 3000
        });
    });
}

function ApprovalComplete() {
    alert("Request was approved successully");
    LoadRequests();
}

function AuthComplete() {
    alert("Request has been authorized successully");
    LoadRequests();
}

function calculateTotals(index, count) {
    var quantity = parseFloat(document.getElementById("qty" + index).value);
    var unitPrice = parseFloat(document.getElementById("unitPrice" + index).value);
    var totalPrice = quantity * unitPrice;
    document.getElementById("totalPrice" + index).value = roundNumber(totalPrice, 2);
    var i, totalVal = 0;
    for (i = 0; i < count; i++) {
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    //Attempt to calculate MB Value
    if (document.getElementById("origLocalValue") != null) {
        var origLocalVal = parseFloat(document.getElementById("origLocalValue").value);
        var origFxVal = parseFloat(document.getElementById("origFxValue").value);
        var mbVal = (origFxVal / origLocalVal) * totalVal;
        mbVal = roundNumber(mbVal, 2);
        document.getElementById("mbValue").value = mbVal;

        //If row is highlighted, double check the BL balance
        if (document.getElementById("budgetCheckStatus").innerHTML != "") {
            index++;
            var select = document.getElementById("PBlines" + index).children[0];
            checkBalance4Review(select);
        }
    }
}

function calculateORPPTotals(index) {
    var qtyToOrder = parseFloat(document.getElementById("totalQty" + index).value);
    var quantity = parseFloat(document.getElementById("qty" + index).value);
    var unitPrice = parseFloat(document.getElementById("unitPrice" + index).value);
    if (quantity > qtyToOrder) {
        quantity = qtyToOrder;
        document.getElementById("qty" + index).value = quantity;
    }
    var totalPrice = quantity * unitPrice;
    document.getElementById("totalPrice" + index).value = roundNumber(totalPrice, 2);
}

function calculateTotalsRFP(index, count) {
    var totalPrice;
    var i, totalVal = 0;
    for (i = 0; i < count; i++) {
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    //Attempt to calculate MB Value
    var origLocalVal = parseFloat(document.getElementById("origLocalValue").value);
    var origFxVal = parseFloat(document.getElementById("origFxValue").value);
    if (origLocalVal <= 0) {
        calculateMBValue();
    }
    else {
        var mbVal = (origFxVal / origLocalVal) * totalVal;
        mbVal = roundNumber(mbVal, 2);
        document.getElementById("mbValue").value = mbVal;
    }

    //If row is highlighted, double check the BL balance
    if (document.getElementById("budgetCheckStatus").innerHTML != "") {
        index++;
        var select = document.getElementById("PBlines" + index).children[0];
        checkBalance4Review(select);
    }
}

function cleanUpNumbers() {
    var i, unitPrice, totalPrice;
    var totalVal = 0;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("unitPrice" + i) == null)
            break;
        unitPrice = parseFloat(document.getElementById("unitPrice" + i).value);
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
        document.getElementById("unitPrice" + i).value = roundNumber(unitPrice, 2);
        document.getElementById("totalPrice" + i).value = roundNumber(totalPrice, 2);
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
}

function cleanUpNumbersRFP() {
    var i, totalPrice;
    var totalVal = 0;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("totalPrice" + i) == null)
            break;
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
        document.getElementById("totalPrice" + i).value = roundNumber(totalPrice, 2);
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
}

function roundNumber(num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result.toFixed(dec);
}

function cleanUpNumbersECF() {
    var i, totalPrice, itemAmount;
    var totalVal = 0, totalAmount = 0;
    onChangeCurrency();
    var conversionRate = parseFloat(document.getElementById("conversionRate").value);
    if (conversionRate < 1)
        document.getElementById("conversionRate").value = roundNumber(conversionRate, 4);
    else
        document.getElementById("conversionRate").value = roundNumber(conversionRate, 2);
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("totalPrice" + i) == null)
            break;
        itemAmount = parseFloat(document.getElementById("totalPrice" + i).value);
        totalPrice = itemAmount * conversionRate;
        totalAmount += totalPrice;
        totalVal += itemAmount;
        document.getElementById("convPrice" + i).value = roundNumber(totalPrice, 2);
        document.getElementById("totalPrice" + i).value = roundNumber(itemAmount, 2);
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    document.getElementById("totalAmount").value = roundNumber(totalAmount, 2);
}

function changeECFExRate(num) {
    var i, itemCount = parseInt(document.getElementById("ecfItemCount").value);
    var fxRate = parseFloat(document.getElementById("conversionRate").value);
    var itemAmount, cnvAmount, totalAmount = 0, totalVal = 0;
    for (i = 0; i < itemCount; i++) {
        itemAmount = parseFloat(document.getElementById("totalPrice" + i).value);
        cnvAmount = itemAmount * fxRate;
        document.getElementById("convPrice" + i).value = roundNumber(cnvAmount, 2);
        totalAmount += cnvAmount;
        totalVal += itemAmount;
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    document.getElementById("totalAmount").value = roundNumber(totalAmount, 2);
    //Attempt to calculate MB Value
    calculateMBValue();
}

function cleanUpNumbersSPM() {
    var i, totalPrice;
    var totalVal = 0;
    for (i = 0; i >= 0; i++) {
        if (document.getElementById("totalPrice" + i) == null)
            break;
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
        document.getElementById("totalPrice" + i).value = roundNumber(totalPrice, 2);
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    document.getElementById("totalAmount").value = roundNumber(totalVal, 2);
}

function calculateTotalsSPM(index) {
    var totalPrice;
    var count = parseInt(document.getElementById("spmItemCount").value);
    var i, totalVal = 0;
    for (i = 0; i < count; i++) {
        totalPrice = parseFloat(document.getElementById("totalPrice" + i).value);
        totalVal += totalPrice;
    }
    document.getElementById("totalVal").value = roundNumber(totalVal, 2);
    document.getElementById("totalAmount").value = roundNumber(totalVal, 2);
    //Attempt to calculate MB Value
    var origLocalVal = parseFloat(document.getElementById("origLocalValue").value);
    var origFxVal = parseFloat(document.getElementById("origFxValue").value);
    if (origLocalVal <= 0) {
        calculateMBValue();
    }
    else {
        var mbVal = (origFxVal / origLocalVal) * totalVal;
        mbVal = roundNumber(mbVal, 2);
        document.getElementById("mbValue").value = mbVal;
    }

    //If row is highlighted, double check the BL balance
    if (document.getElementById("budgetCheckStatus").innerHTML != "") {
        index++;
        var select = document.getElementById("PBlines" + index).children[0];
        checkBalance4Review(select);
    }
}

function calculateTotalsTAF() {
    var totalAdvance = 0;
    var i = 0;
    while (document.getElementById("advanceRequired" + i) != null) {
        totalAdvance += parseFloat(document.getElementById("advanceRequired" + i).value);
        i++;
    }
    i = 0;
    while (document.getElementById("totalExpenseAdvance" + i) != null) {
        totalAdvance += parseFloat(document.getElementById("totalExpenseAdvance" + i).value);
        i++;
    }
    document.getElementById("totalVal").value = roundNumber(totalAdvance, 2);
    //Attempt to calculate MB Value
    var origLocalVal = parseFloat(document.getElementById("origLocalValue").value);
    var origFxVal = parseFloat(document.getElementById("origFxValue").value);
    if (origLocalVal <= 0) {
        calculateMBValue();
    }
    else {
        var mbVal = (origFxVal / origLocalVal) * totalAdvance;
        mbVal = roundNumber(mbVal, 2);
        document.getElementById("mbValue").value = mbVal;
    }
}

function calculateTAFAllowance(index) {
    var nights = parseInt(document.getElementById("night" + index).value);
    var rate = parseFloat(document.getElementById("dailyRate" + index).value);
    var totalAllowance = nights * rate;
    document.getElementById("totalAllowance" + index).value = roundNumber(totalAllowance, 2);
}