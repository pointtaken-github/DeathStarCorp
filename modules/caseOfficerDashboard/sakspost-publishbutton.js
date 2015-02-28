$(document).ready(function () {

    $("table[id$='Tbl'].ms-formtoolbar").find('tr').first().find('td').last().append('<input type="button" value="Publiser" id="publish_button"></input>')
    $('#publish_button').click(function () {
        var path = _spPageContextInfo.serverRequestPath.split('/');
        var listName = path[path.length - 2];
        var itemID = getParameterByName('ID');

        $.getScript("../../SiteAssets/js/libs/LS.Utils.js", function () {
            $.getScript("../../SiteAssets/js/libs/LS.SP.REST.js", function () {
                setStatusPublishedForListItem(listName, itemID);
            });
        });
       
        
    });
});
function setStatusPublishedForListItem(listName, itemId) {
    var metadata = {
        "SirkulaerStatus": "Publisert"
    };
    LS.SP.REST.UpdateListItem(_spPageContextInfo.webAbsoluteUrl, listName, itemId, metadata).then(function () {
       
        statusId = SP.UI.Status.addStatus("Elementet er nå publisert. Trykk lukk for å komme tilbake til listen.");
        SP.UI.Status.setStatusPriColor(statusId, 'green');
    }).fail(LS.Utils.DisplayAJAXError);

}
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


