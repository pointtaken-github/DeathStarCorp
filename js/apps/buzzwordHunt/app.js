var BZApp = angular.module('BZApp', []);
BZApp.controller('BZController', function ($scope, $http, $sce, $q) {
    $scope.buzzwords = {};
    $scope.getBuzzwords = function () {
        $.getJSON("https://spreadsheets.google.com/feeds/list/1RTzw3jcVj00hQdYJhUZT8Uu8U_Uc8J9AG2FvZAvE4as/od6/public/values?alt=json", function (data) {
            var test = data;
        });
    }
    
});

