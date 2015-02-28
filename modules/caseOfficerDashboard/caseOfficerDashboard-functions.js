LS.Utils.EnsureNamespace("PT.SP.REST.SEARCH");
LS.Utils.EnsureNamespace("PT.SP.REST");
PT.SP.REST.SEARCH.SearchByContentType = function (contentType) {
    var trimDuplicates = false;
    var queryText = "contenttype:" + contentType; // get site collections
    var selectProperties = "Title, Author, Url, owstaxIdSuggestionx0020status, BuzzwordOWSTEXT,BlogStatusOWSCHCS, LastModifiedTime, SuggestionTextStatusOWSCHCS ";
    var queryUrl = _spPageContextInfo.webAbsoluteUrl + "/_api/search/query?querytext='" + queryText + "'&rowlimit=500&trimduplicates=" + trimDuplicates.toString() + "&selectproperties='" + selectProperties + "'"; // 500 is max per page. Exclusions must be included in the query to get the desired results in the first page (or you could implement paging).
console.log(queryUrl);
    return jQuery.ajax({
        url: queryUrl,
        async: true,
        method: "GET",
        headers: {
            "Accept": "application/json; odata=verbose"
        }
    }).then(function (data) {
        var results = data.d.query.PrimaryQueryResult.RelevantResults.Table.Rows.results;
        return LS.Utils.ArrayUnique(results); // prevent duplicates
    }).fail(LS.Utils.DisplayAJAXError);
};
