

var WPApp = angular.module('publishPost', []);
WPApp.controller('PublishPostController', function ($scope, $http, $sce) {
    $scope.posts = {};
    $scope.test = "";
    debugger;
    function init() {
        $http({
            method: 'GET',
            url: "https://public-api.wordpress.com/rest/v1/sites/deathstarcorp.wordpress.com/posts/?number=5&pretty=1",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (data, status, headers, config) {
            $scope.posts = data.posts;

        }).error(function () {
            $scope.posts = [];
        });
    }
    $scope.postToWP = function () {

        var items =  LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl + "/blog", "Posts", "Blog_x0020_status", "Published");
        var title = items[0].Title;
        var content = items[0].Body;

        var token;
        jQuery.ajax({
            url: 'https://public-api.wordpress.com/oauth2/authorize?client_id=39622&redirect_uri=https://aspc1505.sharepoint.com/&response_type=token',
            type: 'GET',
            headers: { "Accept": "application/json;odata=verbose" },
            success: function (response) {
                var token = response;
            }
        });


        var accessToken = getParameterByName('access_token'); 
        $scope.test = accessToken;
        var siteId = "85878188";
        
        jQuery.ajax({
            url: 'https://public-api.wordpress.com/rest/v1/sites/' + siteId + '/posts/new',
            type: 'POST',
            data: {
                title: title,
                content: Body
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'BEARER ' + accessToken);
            },
            success: function (response) {
                // response
            }
        });
      
        
        
    }
    init();
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
    
});
