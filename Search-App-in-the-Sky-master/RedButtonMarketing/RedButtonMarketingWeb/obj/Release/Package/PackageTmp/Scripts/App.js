function VresizeAppPart(spAppEntryHostUrl, spAppSenderId) {

    if (window.parent == null)
        return;

    var height = ($('#AllContentDiv').outerHeight()) + 0;
    var offTop = ($('#AllContentDiv').offset().top);
    var iframeHeight = height + (offTop * 2);

    var message = "<Message senderId=" + spAppSenderId + ">" + "resize(100%," + iframeHeight + ")</Message>";
    window.parent.postMessage(message, spAppEntryHostUrl);
}