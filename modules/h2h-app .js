$(document).ready(function(){

	$.ajax({
	                url: 'http://www.arcticsharepointchallenge.com/api/teams',
	                type: 'GET',
	                data: { sentence: title + "|||" + content },
	                datatype: 'json',
	                success: function(data) {
	     
	                    //var accessToken = 'tO61@b%2DwRl9q!bWyRvPA6H^@qEBW)dcbp5SGp0gMONCav(0c*QB3OfTu94A#BJ';
	                    //$scope.test = accessToken;
	                    //var siteId = "85878188";
	     
	                    console.log(data);
	                },
	                error: function(err) {
	                    alert(err);
	                }
	            });

	});