var WPApp = angular.module('WelcomeApp', []);
WPApp.controller('WelcomeController', function ($scope, $http, $sce) {
    $scope.me = {};
    $scope.test = "";
    var device;
    function init() {
        debugger;
        $http({
            method: 'GET',
            url: "_spPageContextInfo.webAbsoluteUrl/_api/SP.UserProfiles.PeopleManager/GetMyProperties",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (data, status, headers, config) {
            $scope.me = data;
            $scope.$apply();

        }).error(function () {
            $scope.me = [];
        });
    }

    $scope.postToWP = function(id) {
        LS.SP.REST.GetListItem(_spPageContextInfo.webAbsoluteUrl + "/blog", "Posts", id).then(function (item)
        {
            var title = "";
            var content = "";
            var deferreds = [];

            title = item.d.Title;
            content = item.d.Body;
            publishPost(title, content).then(function () {
                var metadata = {
                   "SirkulaerStatus": "Publisert"
                };
                LS.SP.REST.UpdateListItem(_spPageContextInfo.webAbsoluteUrl, listName, id, metadata);                
            });  
        }).then(function () {
            $scope.init();
        });
    };

    function publishPost(title, content)    {
        $.ajax({
            url: 'https://yoda.p.mashape.com/yoda', 
            type: 'GET', 
            data: { sentence: title + "|||" + content }, 
            datatype: 'json',
            success: function(data) {

                var accessToken = 'tO61@b%2DwRl9q!bWyRvPA6H^@qEBW)dcbp5SGp0gMONCav(0c*QB3OfTu94A#BJ'; 
                $scope.test = accessToken;
                var siteId = "85878188";

                jQuery.ajax({
                    url: 'https://public-api.wordpress.com/rest/v1/sites/' + siteId + '/posts/new',
                    type: 'POST',
                    data: {
                        title: data.split("|||")[0],
                        content: data.split("|||")[1]
                    },
                    beforeSend: function(xhr) {
                        xhr.setRequestHeader('Authorization', 'BEARER ' + accessToken);
                    },
                    success: function(response) {
                        init();
                    }
                });
            },
            error: function(err) {
                alert(err);
            },
            beforeSend: function(xhr) {
                xhr.setRequestHeader("X-Mashape-Authorization", "YYaN98uxYGmshEbJ1rjS25xsVLv6p1GGdzmjsnBjh7Jidk1Gkf"); // Enter your Mashape key here
            }
        });
    }    

    $scope.armBigRedButton = function () {
        device.callFunction('armbutton', '', function (err, data) {
            console.log('ERROR', err)
            console.log('DATA', data)
        });
    };

    init();
    spark.on('login', function (err, login) {
        console.log('Spark is logged in!');

        spark.listDevices().then(function (devices) {
            device = devices[0];

            var url = 'https://api.spark.io/v1/devices/' + device.id + '/events/?access_token=' + login.access_token;
            var eventSource = new EventSource(url);
            eventSource.addEventListener('big-button-clicked', function(e) {
                var dataFromCore = JSON.parse(e.data);
                console.log('big button was clicked!', dataFromCore);
                $scope.postToWP(2);

            });
        });

        spark.claimCore('51ff6f065082554949420887', function (err, data) {
        });

    });

    spark.login({ username: 'kjetil.odegaarden@gmail.com', password: 'krovelvellevold' });
    
});