var WPApp = angular.module('WPApp', []);
WPApp.controller('WPController', function ($scope, $http, $sce, $q) {
    $scope.posts = {};
    $scope.test = "";
    
    $scope.init = function () {
        $http({
            method: 'GET',
            url: "https://public-api.wordpress.com/rest/v1/sites/deathstarcorp.wordpress.com/posts/?number=50&pretty=1",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (data, status, headers, config) {
            $scope.posts = data.posts;
            $scope.$apply();

        }).error(function () {
            $scope.posts = [];
        });
    };
    $scope.postToWP = function () {
        LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl + "/blog", "Posts", "Blog_x0020_status", "Published").then(function (items)
        {

            var title = "";
            var content = "";
            var deferreds = [];
            for (var i = 0; i < items.length; i++) {

                title = items[i].Title;
                content = items[i].Body;
                deferreds.push(publishPost(title, content)); 

            }
            $.when.apply(null, deferreds)
            {
                $scope.init();
                //update statuses?
            }
        });    
    };
    $scope.init();
    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    };
    
    function publishPost(title, content) {
        $.ajax({
                url: 'https://yoda.p.mashape.com/yoda', // the important translation
                type: 'GET', // The HTTP Method
                data: { sentence: content }, // Parameters go here
                datatype: 'json',
                success: function (data) {
                    
                    var accessToken = 'tO61@b%2DwRl9q!bWyRvPA6H^@qEBW)dcbp5SGp0gMONCav(0c*QB3OfTu94A#BJ';//getParameterByName('access_token'); 
                    var siteId = "85878188";

                    jQuery.ajax({
                        url: 'https://public-api.wordpress.com/rest/v1/sites/' + siteId + '/posts/new',
                        type: 'POST',
                        data: {
                            title: title,
                            content: data
                        },
                        beforeSend: function (xhr) {
                            xhr.setRequestHeader('Authorization', 'BEARER ' + accessToken);
                        },
                        success: function (response) {
                        }
                    });
                },
                error: function (err) {
                    alert(err);
                },
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("X-Mashape-Authorization", "YYaN98uxYGmshEbJ1rjS25xsVLv6p1GGdzmjsnBjh7Jidk1Gkf"); // Enter your Mashape key here
                }
            });
    };
});

