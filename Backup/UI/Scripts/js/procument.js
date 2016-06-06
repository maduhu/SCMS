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

function GetDonor(select) {
    if (select.value == "" || document.getElementById("donorDiv") == null)
        return;
    var url = "/OrderRequest/GetDonorName/" + select.value;
    sendRequest(url, 'donorDiv', 'loading');
}

function selectedIndexChangePONos(select) {
    var url = "/OrderRequest/GetProjectNos/" + select.value;
    if (select.value != "") {
        sendRequest(url, 'ProjectNoDiv', 'loading');
    }
}

function GetR4APayDetails(select) {
    var url = "/Request4Advance/LoadR4A/" + select.value;
    sendRequest(url, 'R4ADiv', 'loading');
}

function GetR4PaymentDetails(select) {
    var url = "/Request4Payment/LoadR4P/" + select.value;
    sendRequest(url, 'R4PDiv', 'loading');
}

function GetTAnalysis(select) {
    var url = "/PurchaseOrder/LoadPOrder/" + select.value;
    sendRequest(url, 'PODiv', 'loading');
}

function GetORDetails(select) {
    var url = "/PurchaseOrder/LoadSingleQItems/" + select.value;
    sendRequest(url, 'PODiv', 'loading');
}


function GetWBItems(select) {
    var url = "/WayBill/GetWBItems/" + select.value;
    sendRequest(url, 'itemsDiv', 'loading');
}


function GetWBRecvdItems(select) {
    var url = "/WayBill/GetWBReceivdItems/" + select.value;
    sendRequest(url, 'itemsDiv', 'loading');
    document.getElementById("destinationOffice").innerHTML = document.getElementById("destOffice").value;
}

function InvtselectedIndexChange(select) {
    var url = "/WRForm/GetAssets/" + select.value;
    if (select.value != "") {
        xmlHttp = GetXmlHttpObject();
        xmlHttp.open("GET", url, false);
        xmlHttp.send(null);
        document.getElementById("PBlines").innerHTML = xmlHttp.responseText;
        if (document.getElementById("txtqtyordered") != null) {
            document.getElementById('txtqtyordered').value = '';
        }
    }
}

function CheckAvailableQty() {
    if (Trim(document.getElementById("inventoryId").value) == "" || (document.getElementById("dplassetId") != null && Trim(document.getElementById("dplassetId").value) == ""))
        return false;
    if (document.getElementById("dplassetId") != null) {
        var assetid = document.getElementById("dplassetId").value;
       }
    else {var assetid = null;}
    var invid = document.getElementById("inventoryId").value;
    var qty = document.getElementById("txtqtyordered").value;
    var wrnId = document.getElementById("wrnId").value;
    var url = "/WRForm/IsQtyenough?InventoryId=" + invid + "&releasedQty=" + qty + "&wrnId=" + wrnId + "&assetId=" + assetid;
    var xmlHttp = GetXmlHttpObject();
    xmlHttp.open("GET", url, false);
    xmlHttp.send(null);
    var response = xmlHttp.responseText.split('~')
    if (response[0] === '0') {
        alert(response[1]);
        return false;
    } else { return true; }
}