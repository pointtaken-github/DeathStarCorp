Array.prototype.contains = function(v) {
    for(var i = 0; i < this.length; i++) {
        if(this[i] === v) return true;
    }
    return false;
};

Array.prototype.unique = function() {
    var arr = [];
    for(var i = 0; i < this.length; i++) {
        if(!arr.contains(this[i])) {
            arr.push(this[i]);
        }
    }
    return arr; 
}

var res;
var device;
$(document).ready(function () {
    $('#new-button').click(function () {
        //Using a generic object.
        var options = {
            title: "Blogpost",
            width: 600,
            height: 800,
            url: "../blog/Lists/Suggestions/NewForm.aspx"
        };

        SP.UI.ModalDialog.showModalDialog(options);

        return false;
    });
    $('#search-button').click(function () {
  
        return false;
    })

setTimeout(function(){ setFullScreen(); }, 3000);

});
function setFullScreen(){
 SetFullScreenMode(true);

PreventDefaultNavigation();

};




var editorsDashboardApp = angular.module('editorsDashboard', []);
      
    editorsDashboardApp.controller('editorsDashboardController', function ($scope, $http,$timeout) {
         
         var mostPopularBuzzwords;

       //spark funksjon for å lytte på eventet
       //hvis eventet så 
      
        $scope.blogItems = [];
        $scope.filters = {};
        $scope.buzzWordList = {};
        $scope.counter = 0;
        $scope.newSuggestions = {};
        $scope.bloggedPosts = {};
        var mytimeout;// = $timeout($scope.onTimeout,30000);
        spark.on('login', function (err, login) {
                console.log('Spark is logged in!');
                
                spark.listDevices().then(function (devices) {
                    device = devices[0];
         
                    var url = 'https://api.spark.io/v1/devices/' + device.id + '/events/?access_token=' + login.access_token;
                    var eventSource = new EventSource(url);

                    eventSource.addEventListener('big-button-clicked', function(e) {
                        var dataFromCore = JSON.parse(e.data);
                        console.log('big button was clicked!', dataFromCore);
                        $scope.postToWP();
                        
                    });
                    
                });
         
                spark.claimCore('51ff6f065082554949420887', function (err, data) {
                });
         
            });
         
            spark.login({ username: 'kjetil.odegaarden@gmail.com', password: 'krovelvellevold' });

        $scope.init = function () {
            mytimeout = $timeout($scope.onTimeout,15000);
            var items = [];
            var allBuzzwords = [];
            /*Get main items*/
            PT.SP.REST.SEARCH.SearchByContentType('Suggestion').then(function(results){
                
                res = results;
               
                $.each(res, function (i) {
                    var item = new BlogItem();
                    item.title = res[i].Cells.results[2].Value;
                    item.url = res[i].Cells.results[4].Value;
                    item.buzzWord = res[i].Cells.results[5].Value;
                    allBuzzwords.push(item.buzzWord);
                    item.blogStatus = res[i].Cells.results[6].Value;
                      // item.suggestionStatus  =res
                      item.modified = res[i].Cells.results[7].Value;
                    item.suggestionStatus = res[i].Cells.results[8].Value;
                    $.each(res[i].Cells.results, function (k) {
                        //console.log(k + " - " + res[i].Cells.results[k].Key + " " + res[i].Cells.results[k].Value);
                    });
                    items.push(item);

                });


                
                //it got to be a more elegant way to do this, but im tired.

                var uniques = allBuzzwords.unique(); 
                mostPopularBuzzwords = [];
                var thisCount = {};
                $.each(uniques, function(k){
                 thisCount = items.filter(function(item){
                   return item.buzzWord == uniques[k];
                     });
                     
                      mostPopularBuzzwords.push({
                          label: uniques[k],
                          y: thisCount.length
                      });
                    
                });
                mostPopularBuzzwords.sort(function(a,b) {

                  // assuming distance is always a valid integer
                  return parseInt(b.value,10) - parseInt(a.value,10);

                });
            
                var buzzWordString = "";
              
               $scope.buzzWordList =  mostPopularBuzzwords;
                $scope.blogItems = items;
                

                var newItems = items.filter(function(item) { 
                              return item.suggestionStatus == 'New' ;
                          });
                $scope.newSuggestions = newItems;
                var workingItems = items.filter(function(item){
                  return item.suggestionStatus == 'WriteBlog';
                });

                $scope.workingBlogs = workingItems;

                $scope.$apply();

                var chart = new CanvasJS.Chart("chartContainer");
                chart.options.axisY = { prefix: "", suffix: " times" };
                chart.options.title = { text: "Most used topics (hashtags)" };

                var series1 = { //dataSeries - first quarter
                    type: "column",
                    name: "First Quarter",
                    showInLegend: false
                };   
                chart.options.data = [];
                chart.options.data.push(series1);
                series1.dataPoints = mostPopularBuzzwords;
                chart.render();

                var chart2 = new CanvasJS.Chart("chartContainer2",
  {
    title:{
      text: "Desktop Search Engine Market Share, Dec-2012"
    },
                animationEnabled: true,
    legend:{
      verticalAlign: "center",
      horizontalAlign: "left",
      fontSize: 20,
      fontFamily: "Helvetica"        
    },
    theme: "theme2",
    data: [
    {        
      type: "pie",       
      indexLabelFontFamily: "Garamond",       
      indexLabelFontSize: 20,
      indexLabel: "{label} {y}%",
      startAngle:-20,      
      showInLegend: true,
      toolTipContent:"{legendText} {y}%",
      dataPoints: [
        {  y: 83.24, legendText:"Google", label: "Google" },
        {  y: 8.16, legendText:"Yahoo!", label: "Yahoo!" },
        {  y: 4.67, legendText:"Bing", label: "Bing" },
        {  y: 1.67, legendText:"Baidu" , label: "Baidu"},       
        {  y: 0.98, legendText:"Others" , label: "Others"}
      ]
    }
    ]
  });
  chart2.render();


            }).fail(LS.Utils.DisplayAJAXError);



            $scope.bloggedPosts = LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl, "Posts", "Blog_x0020_status", "Published").then(function (items)
            {
              var bloggedItems = [];
               $.each(items,function(i){
                    var item = new BlogItem();
                    item.title = items[i].Title;
                    item.suggestionStatus = items[i].Blog_x0020_status;
                    item.modified = items[i].Modified;
                    bloggedItems.push(item);

               }); 
               $scope.bloggedPosts = bloggedItems;
            });  
             $scope.writtenPosts = LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl, "Posts", "Blog_x0020_status", "Draft").then(function (items)
            {
              var writtenItems = [];
               $.each(items,function(i){
                    var item = new BlogItem();
                    item.title = items[i].Title;
                    item.suggestionStatus = items[i].Blog_x0020_status;
                    item.modified = items[i].Modified;
                    writtenItems.push(item);
                  
               }); 
               $scope.writtenPosts = writtenItems;

               
            });  

             //Better to construct options first and then pass it as a parameter
  


  
           
        };
    
        $scope.kpinewBlogs = function(){
      
          var relationNumber = $scope.newSuggestions.length;
          if (relationNumber < 10)
            {
             return "error box";
          }
          else if (relationNumber > 10 && relationNumber < 20)
          {
             return "warning box";
          }
          else if (relationNumber > 30){
            return "success box";
          }
          else{
            return "warning box";
          }
        };

        $scope.kpibloggedPosts = function(){
      
          var relationNumber = $scope.bloggedPosts.length;

          if (relationNumber < 10)
            {
             return "success box";
          }
          else if (relationNumber > 10 && relationNumber < 20)
          {
             return "warning box";
          }
          else if (relationNumber > 30){
            return "error box";
          }
          else{
            return "warning box";
          }
        };
        $scope.kpirelationNumberBlogs = function(){

          var relationNumber = $scope.newSuggestions.length / $scope.writtenPosts.length;

        if (relationNumber < 5)
        {
           return "success box";
        }
        else if (relationNumber > 5 && relationNumber < 10)
        {
           return "warning box";
        }
        else if (relationNumber > 10){
          return "error box";
        }
        else{
          return "warning box";
}

         
        };
        //timing functionality courtesy of http://stackoverflow.com/questions/12050268/angularjs-make-a-simple-countdown
        $scope.onTimeout = function(){
            LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl , "Posts", "Blog_x0020_status", "Accepted").then(function (items)
            {
                if(items.length> 0){
                    var id=items[0].Id;
                    console.log('finding accepted blog post');
                    $scope.armBigRedButton(id);
                    
                    
                }else{
                    console.log('no blogposts');
                }
            });  

              mytimeout = $timeout($scope.onTimeout,30000);
              $scope.counter++;
              
          //Sjekk akspeterte bloggposter
          //hvis > 0
          //arm button
          
        };
       
        //$scope.hashtagCounters = "40,4,55";
        
        
        $scope.stop = function(){
            $timeout.cancel(mytimeout);
        }
        $scope.armBigRedButton = function (id) {
            var metadata = {
                               "Blog_x0020_status": "Approved"
                            };
                            //debugger;
                            LS.SP.REST.UpdateListItem(_spPageContextInfo.webAbsoluteUrl , "Posts", id, metadata).then(function(){
                                 device.callFunction('armbutton', '', function (err, data) {
                                    console.log('ERROR', err)
                                    console.log('DATA', data)
                                });
                            });       
        };
        
        $scope.postToWP = function(id) {
            
             LS.SP.REST.GetListItemsByFieldValue(_spPageContextInfo.webAbsoluteUrl + "/blog", "Posts", "Blog_x0020_status", "Accepted").then(function (items)
            {
                if(items.length> 0){
                    var id=items[0].Id;
                    LS.SP.REST.GetListItem(_spPageContextInfo.webAbsoluteUrl , "Posts", id).then(function (item)
                    {
                        var title = "";
                        var content = "";
                        var deferreds = [];

                        title = item.d.Title;
                        content = item.d.Body;
                        $scope.publishPost(title, content, id);   
                    }).then(function () {
                        $scope.init();
                    });
                }
                    
                
            });  
            
        };

        $scope.publishPost = function(title, content,id)    {
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
                            var metadata = {
                               "Blog_x0020_status": "Published"
                            };
                            //debugger;
                            LS.SP.REST.UpdateListItem(_spPageContextInfo.webAbsoluteUrl + "/blog", "Posts", id, metadata).then(function(){
                                $scope.init();

                            });        
                           
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

       
    });

