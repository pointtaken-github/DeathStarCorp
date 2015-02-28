var WPApp = angular.module('WPApp', []);
WPApp.controller('WPController', function ($scope, $http, $sce) {
    $scope.me = {};

    function init() {
        /*if(localStorage["displayname"]!=null){
            $scope.me.displayname=localStorage["displayname"];
            $scope.me.department=localStorage["department"];
            $scope.me.title=localStorage["title"];
            $scope.me.office=localStorage["office"];
            
        }*/
        //else
        //{
            $http({
                method: 'GET',
                url: _spPageContextInfo.webAbsoluteUrl + "/_api/SP.UserProfiles.PeopleManager/GetMyProperties",
                headers: { "Accept": "application/json;odata=verbose" }
            }).success(function (data, status, headers, config) {
                $scope.me = data.d;
                localStorage["displayname"] = data.d.DisplayName;

                var properties = data.d.UserProfileProperties.results;
                for (var i = 0; i < properties.length; i++) {
                    var property = properties[i];
                    
                    if (property.Key == "Office") {
                        $scope.me.office = property.Value;
                        localStorage["office"] = property.Value;

                    }
                    if (property.Key == "Department") {
                        $scope.me.department = property.Value;
                        localStorage["department"] = property.Value;
                    }
                    if (property.Key == "Title") {
                        $scope.me.title = property.Value;
                        localStorage["title"] = property.Value;

                    }
                    var url = "";
                    var img = "";
                    
                
                }
                 switch($scope.me.Title) {
                        case "Editor":
                            localStorage["url"]="https://aspc1505.sharepoint.com/sites/rbm/blog/Pages/EditorDash.aspx";
                            localStorage["img"]="<img src=\"https://aspc1505.sharepoint.com/sites/rbm/SiteCollectionImages/SitePages/Home/smallyoda.jpg\"></img>";
                            break;
                        case "BOSS":
                            localStorage["url"]="https://aspc1505.sharepoint.com/sites/rbm/blog/Pages/EditorDash.aspx";
                            localStorage["img"]="<img src=\"https://aspc1505.sharepoint.com/sites/rbm/SiteCollectionImages/SitePages/Home/icon_user.png\"></img>";
                            break;
                        case "Case Officer":
                            localStorage["url"]="https://aspc1505.sharepoint.com/sites/rbm/blog/Pages/CODashboard.aspx";
                            localStorage["img"]="<img src=\"https://aspc1505.sharepoint.com/sites/rbm/SiteCollectionImages/SitePages/Home/icon_caseofficer.png\"></img>";
                            break;
                        case "Employee":
                            localStorage["url"]="https://aspc1505.sharepoint.com/sites/rbm/blog/Pages/UserDash.aspx";
                            localStorage["img"]="<img src=\"https://aspc1505.sharepoint.com/sites/rbm/SiteCollectionImages/SitePages/Home/icon_user.png\"></img>";
                            break;
                        default:
                            localStorage["url"]="https://aspc1505.sharepoint.com/sites/rbm/blog/Pages/UserDash.aspx";
                            localStorage["img"]="<img src=\"https://aspc1505.sharepoint.com/sites/rbm/SiteCollectionImages/SitePages/Home/icon_user.png\"></img>";

                    }
                    var txt = localStorage["img"] + "<br />Hello "+localStorage["displayname"]+"!<br /> Welcome to the Deathstar marketing tool!<br />  We notice that you work in the "+localStorage["department"]+" dept as a trusted "+localStorage["title"]+".<br /> We will redirect you to your dashboard right away.";
                    SP.SOD.executeOrDelayUntilScriptLoaded(function () { showWaitMessage(txt,localStorage["url"]) }, 'SP.js')
                }).error(function () {
                    
                });
       // }

        
      
    }
    

    init();
    function showWaitMessage(txt,url) {
        window.parent.eval("window.waitDialog = SP.UI.ModalDialog.showWaitScreenWithNoClose('<div style=\"text-align: left;display: inline-table;margin-top: 13px;\">"+txt+"', '', 1000, 1900);");
        setTimeout(function(){ window.location.href=url }, 8000);
    };
    function closeWaitMessage () {
        if (window.parent.waitDialog != null) {
            window.parent.waitDialog.close();
        }
    };
});