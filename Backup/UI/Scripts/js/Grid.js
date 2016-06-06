function UpdateParameters(e, that, exportLinq) {
    if (!that) { that = this; }
    var grid = $(that).data('tGrid');

    // Get the export link as jQuery object
    //var $exportLink = $('#ROexport');
    var $exportLink = $('#' + exportLinq);

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}

function onGRNDataBound() {
    var grid = $('#GRN').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#Grnexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}


function onGenInventoryDataBound() {
    var grid = $('#GeneralInventory').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#Genexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}

function onConsumableDataBound() {
    var grid = $('#Consumables').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#Cexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}

function onAssetRowSelected(e) {

    var assetId = e.row.cells[0].innerHTML;
    var assetNo = e.row.cells[1].innerHTML;
    sendAjaxRequest('/Inventory/AssetInventoryPopUp/' + assetId, assetNo);
    CenterPopup();

}

function onRORowSelected(e) {
    var roId = e.row.cells[0].innerHTML;
    sendRequest('/WRForm/ViewNRNDetails/' + roId, 'wrfDiv', 'loading');
}

function onRORowDataBound(e) {
    var grid = $('#ReleaseOrder').data("tGrid");

    var column = grid.columnFromMember("Status"), // -> column member
          index = $.inArray(column, grid.columns);

    var row = e.row;
    if (e.row.cells[index].innerHTML == "AP") {
        row.cells[index].style.color = 'green';
        row.cells[index].style["font-weight"] = 'bold';
    }
    if (e.row.cells[index].innerHTML == "RJ") {
        row.cells[index].style["color"] = "red";
        row.cells[index].style["font-weight"] = 'bold';
    }

}

function onAssetDataBound() {
    var grid = $('#AssetInventory').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#Aexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}

function onDisposedAssetDataBound() {
    var grid = $('#DisposedAssets').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#Disposedexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}

function onGenInventRowSelected(e) {
    if (e.row.cells[1].innerHTML == "A") {
        var itemId = e.row.cells[0].innerHTML;
        sendAjaxRequest('/Inventory/GeneralInventoryPopUp?itemId=' + itemId, 'Asset List');
    } else {
        var itemId = e.row.cells[0].innerHTML;
        sendAjaxRequest('/Inventory/ConsumableInventoryPopUp?itemId=' + itemId, 'Consumable List');
    }
}

function onCCRowSelected(e) {
    var ccId = e.row.cells[0].innerHTML;
    sendRequest('/CompletionCertificate/ViewCCDetails?Id=' + ccId, 'CompletionDiv', 'loading');
}

function onUnRegiAStRowSelected(e) {

    var grnItemId = e.row.cells[0].innerHTML;
    sendAjaxRequest('/GoodsReceivedNote/LoadRegsiterAsset/' + grnItemId, 'Unregistered Assets');
    clearAssetForm();
    initFineUploader('/GoodsReceivedNote/HandleImageUpload');
}

function onGRNRowSelected(e) {
    var grnId = e.row.cells[0].innerHTML;
    sendRequest('/GoodsReceivedNote/ViewGRNDetails?GRNId=' + grnId + '&Verify=false', 'ContentDiv', 'loading');
}

function onGRNRowDataBound(e) {
    var grid = $("#GRN").data("tGrid");
    //var column = grid.columnFromMember("Status"), // -> column member
    //      index = $.inArray(column, grid.columns);

    var row = e.row;
    if (e.row.cells[6].innerHTML == "AP") {
        row.cells[6].style.color = 'green';
        row.cells[6].style["font-weight"] = 'bold';
    }
    if (e.row.cells[6].innerHTML == "RJ") {
        row.cells[6].style["color"] = "red";
        row.cells[6].style["font-weight"] = 'bold';
    }
}

function onCCRowDataBound(e) {
    var row = e.row;
    if (e.row.cells[7].innerHTML == "AP") {
        row.cells[7].style.color = 'green';
        row.cells[7].style["font-weight"] = 'bold';
    }
    if (e.row.cells[7].innerHTML == "RJ") {
        row.cells[7].style["color"] = "red";
        row.cells[7].style["font-weight"] = 'bold';
    }

}

function onCCDataBound() {
    var grid = $('#CC').data('tGrid');

    // Get the export link as jQuery object
    var $exportLink = $('#CCexport');

    // Get its 'href' attribute - the URL where it would navigate to
    var href = $exportLink.attr('href');

    // Update the 'page' parameter with the grid's current page
    href = href.replace(/page=([^&]*)/, 'page=' + grid.currentPage);

    // Update the 'pageSize' parameter with the grids' current pageSize state
    href = href.replace(/pageSize=([^&]*)/, 'pageSize=' + (grid.pageSize || '~'));

    // Update the 'orderBy' parameter with the grids' current sort state
    href = href.replace(/orderBy=([^&]*)/, 'orderBy=' + (grid.orderBy || '~'));

    // Update the 'filter' parameter with the grids' current filtering state
    href = href.replace(/filter=([^&]*)/, 'filter=' + (grid.filterBy || '~'));

    // Update the 'groupBy' parameter with the grids' current groupBy state
    href = href.replace(/groupBy=(.*)/, 'groupBy=' + (grid.groupBy || '~'));

    // Update the 'href' attribute
    $exportLink.attr('href', href);
}