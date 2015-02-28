var WPApp = angular.module('WPApp', []);
WPApp.controller('WPController', function ($scope, $http, $sce) {
    $scope.me = {};

    function init() {
        $(document).ready(function() {
               var xhr = new XMLHttpRequest();
xhr.open("get", "https://www.arcticsharepointchallenge.com//api/Teams", true);
xhr.onload = function(){  //instead of onreadystatechange
    //do something
};
xhr.send(null);
            });
        return;
            $http({
            method: 'GET',
            url: "//www.arcticsharepointchallenge.com//api/Teams",
            headers: { "Accept": "application/xml;odata=verbose" }
            }).success(function (data, status, headers, config) {
                debugger;
                var metadata = {
                   "team": data.d.CompanyName,
                   "Title": data.d.Name
                };
                $.each(result, function (k, v) {
                    var metadata = {
                       "team": v.CompanyName,
                       "Title": v.Name
                    };
                     LS.SP.REST.AddListItem("https://aspc1505.sharepoint.com/sites/rbm", "H2H", metadata);
                });

            }
           
            ).error(function () {
                
            });
        

        var txt = "Hello "+localStorage["displayname"]+"!<br /> Welcome to the Deathstar marketing tool!<br />  We notice that you work in the "+localStorage["department"]+" dept as a trusted "+localStorage["title"]+".<br /> We will redirect you to your dashboard right away.";
        SP.SOD.executeOrDelayUntilScriptLoaded(function () { showWaitMessage(txt) }, 'SP.js')
      
    }
    

    init();
    function showWaitMessage(txt) {
        window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('<div style=\"text-align: left;display: inline-table;margin-top: 13px;\">"+txt+"', '', 1000, 1900);");
    };
    function closeWaitMessage () {
        if (window.parent.waitDialog != null) {
            window.parent.waitDialog.close();
        }
    };
});