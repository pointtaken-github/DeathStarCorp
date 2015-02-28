var BZApp = angular.module('BZApp', []);
BZApp.controller('BZController', function ($scope, $http, $sce, $q) {
    $scope.buzzwords = {};

    function init() {
        $http({
            method: 'GET',
            url: _spPageContextInfo.webAbsoluteUrl + "/blog/_api/web/lists/getbytitle('Buzzwords')/items",
            headers: { "Accept": "application/json;odata=verbose" }
        }).success(function (data, status, headers, config) {
            $scope.buzzwords = data.posts;
            $scope.$apply();

        }).error(function () {
            $scope.buzzwords = [];
        });
    }

    $scope.getBuzzwords = function () {
        $.getJSON("https://spreadsheets.google.com/feeds/list/1RTzw3jcVj00hQdYJhUZT8Uu8U_Uc8J9AG2FvZAvE4as/od6/public/values?alt=json", function (data) {
            var items = data.feed.entry;
            var batchUrl = _spPageContextInfo.webAbsoluteUrl +"/blog";
            var endPointUrl = _spPageContextInfo.webAbsoluteUrl + "/blog/_api/web/lists/getbytitle('Buzzwords')/items";
            
            var batchExecutor = new RestBatchExecutor(batchUrl, { 'X-RequestDigest': $('#__REQUESTDIGEST').val() });

            var commands = [];
            var hashtags = [];
            for (var i = items.length - 1; i >= 0; i--) {
            	var words = items[i].gsx$content.$t;
            	var tagsList = words.split(' ');
            	$.each(tagsList, function(i, val) {
            		if (tagsList[i].indexOf('#') == 0) {
                        var name = tagsList[i];
                        var value = hashtags[name];
                        var count = value == null ? 1 : value++;
                        hashtags[name] = count;
            		}
            	});
            };

            for (var key in hashtags) {
	            var batchRequest = new BatchRequest();
	            batchRequest.endpoint = endPointUrl;
	            batchRequest.payload = {
					"__metadata": { "type": "SP.Data.BuzzwordsListItem" },
    				"Title": key,
	    			"Count": hashtags[key].toString() };
	            batchRequest.verb = "POST";
                batchRequest.headers = {"Accept": "application/json;odata=verbose"};
	            commands.push({id: batchExecutor.loadChangeRequest(batchRequest), title: "Create buzzwords"});
            }

            batchExecutor.executeAsync().done(function (result) {
            	var d = result;
			    var msg = [];
			    $.each(result, function (k, v) {
			        var command = $.grep(commands, function (command) {
			            return v.id === command.id;
			        });
			        if (command.length) {
			            msg.push("Command--" + command[0].title + "--" + v.result.result);
			        }
			    });

			    console.log(msg.join('\r\n'));
                init();

				}).fail(function (err) {
				    console.log(JSON.stringify(err));
				});
            });
    }
    init();
      
});

