LS.Utils.EnsureNamespace("LS.SP.REST");

LS.SP.REST.GetSites = function () {
	// Credit: http://social.msdn.microsoft.com/Forums/sharepoint/en-US/34441fc0-50c8-4db0-b539-05a9b9e28a3b/get-a-list-with-all-sharepoint-sites-with-javascript?forum=sharepointdevelopment

	// URL exclusions (don't include the protocol or tenant sub-domain: i.e. "company365")
	var urlExclusions = [".sharepoint.com/sites/contentTypeHub", "-public.sharepoint.com", "-my.sharepoint.com"];
	var trimDuplicates = false;
	var queryText = 'contentclass:"STS_Site"'; // get site collections

	// get SharePoint Online tenant sub-domain
	var subDomain = window.location.hostname.split('.')[0].replace("-my", "").replace("-public", "");

	// add URL exclusions to query
	jQuery.each(urlExclusions, function (index, value) {
		queryText += ' -path:"' + window.location.protocol + '//' + subDomain + value + '"';
	});

	LogMsg("Search query: " + queryText);

	var queryUrl = window.location.protocol + "//" + window.location.hostname + "/_api/search/query?querytext='" + queryText + "'&rowlimit=500&trimduplicates=" + trimDuplicates.toString(); // 500 is max per page. Exclusions must be included in the query to get the desired results in the first page (or you could implement paging).

	return jQuery.ajax({
		url: queryUrl,
		async: true,
		method: "GET",
		headers: {
			"Accept": "application/json; odata=verbose"
		}
	}).then(function (data) {
		var results = data.d.query.PrimaryQueryResult.RelevantResults.Table.Rows.results;
		var sites = jQuery.map(results, function (value, index) {
			return value.Cells.results[6].Value;
		});
		return LS.Utils.ArrayUnique(sites); // prevent duplicates
	}).fail(LS.Utils.DisplayAJAXError);
};

// http://www.plusconsulting.com/blog/2013/05/crud-on-list-items-using-rest-services-jquery/
LS.SP.REST.GetListItem = function (url, listName, id) {
	LogMsg("Getting item '" + id + "' from list '" + listName + "'");
	return jQuery.ajax({
		url: url + "/_api/web/lists/getbytitle('" + listName + "')/items(" + id + ")",
		method: "GET",
		headers: {
			"Accept": "application/json; odata=verbose"
		}
	});
};

LS.SP.REST.GetListItems = function (url, listName) {
	return jQuery.ajax({
		url: url + "/_api/web/lists/getbytitle('" + listName + "')/items",
		type: 'GET',
		dataType: 'json',
		beforeSend: function (xhr) {
			xhr.setRequestHeader("accept", "application/json; odata=verbose")
		}
	}).then(function(data) {
		return data.d.results;
	});
};

LS.SP.REST.GetListItemsByFieldValue = function (url, listName, fieldName, fieldValue) {
    return jQuery.ajax({
        url: url + "/_api/web/lists/getbytitle('" + listName + "')/items?$filter="+fieldName+" eq '"+fieldValue+"'",
        type: 'GET',
        dataType: 'json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("accept", "application/json; odata=verbose")
        }
    }).then(function (data) {
        return data.d.results;
    });
};


LS.SP.REST.GetListFields = function (url, listName, fieldInternalNames) {
	return jQuery.ajax({
		url: url + "/_api/web/lists/getbytitle('" + listName + "')/fields?$select=Title,InternalName,TypeAsString",
		type: 'GET',
		dataType: 'json',
		beforeSend: function (xhr) {
			xhr.setRequestHeader("accept", "application/json; odata=verbose")
		}
	}).then(function (data) {
		var array = data.d.results;
		if (fieldInternalNames) {
			array = jQuery.grep(array, function (value, index) {
				return jQuery.inArray(value.InternalName, fieldInternalNames) > -1;
			});
		}
		return array;
	});
};

LS.SP.REST.GetSitePermissionLevels = function(url) {
	// get an array of SP.RoleDefinition objects representing the Permission Levels for the site
	return jQuery.ajax({
		url: url + "/_api/web/RoleDefinitions?$select=Name,Description,Id,BasePermissions",
		cache: false,
		async: true,
		dataType: "json",
		beforeSend: function (xhr) {
			xhr.setRequestHeader("accept", "application/json; odata=verbose")
		}
	}).then(function (data) {
		return data.d.results;
	});
};

LS.SP.REST.GetGroupMembers = function (url, groupTitle) {
	return jQuery.ajax({
		url: url + "/_api/web/SiteGroups?$select=Users&$expand=Users&$filter=Title eq '" + groupTitle + "'",
		method: "GET",
		headers: {
			"Accept": "application/json; odata=verbose"
		}
	}).then(function (data) {
	    var results = [];
	    if (data.d.results.length>0)
		    results = data.d.results[0].Users.results;
		return results;
	});
};

LS.SP.REST.CurrentUserIsMemberOfGroup = function (groupTitle) {
	return LS.SP.REST.GetGroupMembers(_spPageContextInfo.webAbsoluteUrl, groupTitle)
		.then(function (data) {
			var user = jQuery.grep(data, function (v, i) {
				return v.Id == _spPageContextInfo.userId; // _spPageContextInfo.userId is the current user's ID for the current site collection
			});
			var userIsMember = user.length > 0;
			return userIsMember;
		});
};

LS.SP.REST.DoesUserHavePermission = function (url, spPermissionKind) {
	var restEndpoint = url + "/_api/web/effectiveBasePermissions";
	return jQuery.ajax({
		url: restEndpoint,
		type: 'GET',
		dataType: 'json',
		beforeSend: function (xhr) {
		    xhr.setRequestHeader("accept", "application/json; odata=verbose");
		}
	}).then(function (data) {
		var d = data.d;
		var permissions = new SP.BasePermissions();
		permissions.fromJson(d.EffectiveBasePermissions);
		return permissions.has(spPermissionKind);
	});
};

LS.SP.REST.GetUserId = function (url, loginName) {
	return jQuery.ajax({
		url: "{0}/_api/Web/SiteUsers(@v)?@v='{1}'".format(url, encodeURIComponent(loginName)),
		type: "GET",
		dataType: 'json',
		beforeSend: function (xhr) {
			xhr.setRequestHeader("accept", "application/json; odata=verbose")
		}
	}).then(function (data) {
		return data.d.Id;
	});
};

LS.SP.REST.EnsureUser = function (url, loginName) {
	return jQuery.ajax({
		url: "{0}/_api/Web/EnsureUser(@v)?@v='{1}'".format(url, encodeURIComponent(loginName)),
		type: "POST",
		dataType: 'json',
		headers: {
			"Accept": "application/json;odata=verbose",
			"X-RequestDigest": LS.SP.REST.GetRequestDigest()
		}
	}).then(function (data) {
		return data.d.Id;
	});
};

LS.SP.REST.GetUserById = function (url, id) {
	return jQuery.ajax({
		url: "{0}/_api/Web/GetUserById({1})".format(url, id),
		type: "GET",
		dataType: 'json',
		beforeSend: function (xhr) {
		    xhr.setRequestHeader("accept", "application/json; odata=verbose");
		}
	}).then(function (data) {
		return data.d;
	});
};

// LS.SP.REST.GetUserProperties = function (url, loginName) {
	// return jQuery.ajax({
		// url: "{0}/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)?@v='{1}'".format(url, loginName),
		// type: "GET",
		// dataType: 'json',
		// beforeSend: function (xhr) {
			// xhr.setRequestHeader("accept", "application/json; odata=verbose")
		// }
	// }).then(function (data) {
		// return data.d;
	// });
// };

LS.SP.REST.GetListItemType = function (url, listName) {
	return jQuery.ajax({
		url: url + "/_api/web/lists/getbytitle('" + listName + "')",
		type: "GET",
		dataType: 'json',
		beforeSend: function (xhr) {
			xhr.setRequestHeader("accept", "application/json; odata=verbose")
		}
	}).then(function (data) {
		LogMsg("ListItemEntityTypeFullName: " + data.d.ListItemEntityTypeFullName);
		return data.d.ListItemEntityTypeFullName;
	}, LS.Utils.DisplayAJAXError);
};

LS.SP.REST.GetRequestDigest = function () {
	UpdateFormDigest(_spPageContextInfo.webServerRelativeUrl, 1440000);
	return jQuery('#__REQUESTDIGEST').val();
};

LS.SP.REST.AddListItem = function (url, listName, metadata) {
	return LS.SP.REST.GetListItemType(url, listName)
		.then(function (listItemType) {
			var data = jQuery.extend({
				'__metadata': {
					'type': listItemType
				}
			}, metadata);
			LogMsg("Adding list item");
			LogMsg(data);
	
			return jQuery.ajax({
				url: url + "/_api/web/lists/getbytitle('" + listName + "')/items",
				type: "POST",
				contentType: "application/json;odata=verbose",
				data: JSON.stringify(data),
				dataType: 'json',
				headers: {
					"Accept": "application/json;odata=verbose",
					"X-RequestDigest": LS.SP.REST.GetRequestDigest()
				}
			});
		}).fail(LS.Utils.DisplayAJAXError);
};

LS.SP.REST.UpdateListItem = function (url, listName, id, metadata) {
	return LS.SP.REST.GetListItemType(url, listName)
		.then(function (listItemType) {
			var data = jQuery.extend({
				'__metadata': {
					'type': listItemType
				}
			}, metadata);
			LogMsg("Updating list item " + id);
			LogMsg(data);

			return LS.SP.REST.GetListItem(url, listName, id)
				.then(function (item) {
					return jQuery.ajax({
						url: item.d.__metadata.uri,
						type: "POST",
						contentType: "application/json;odata=verbose",
						data: JSON.stringify(data),
						dataType: 'json',
						headers: {
							"Accept": "application/json;odata=verbose",
							"X-RequestDigest": LS.SP.REST.GetRequestDigest(),
							"X-HTTP-Method": "MERGE",
							"If-Match": "*"
						}
					});
				}).then(function () {
					LogMsg("Item updated");
					return;
				}, LS.Utils.DisplayAJAXError);
		});
};

LS.SP.REST.DeleteListItem = function (url, listName, id) {
	LogMsg("Deleting list item " + id);
	return LS.SP.REST.GetListItem(url, listName, id)
		.then(function (item) {
			return jQuery.ajax({
				url: item.d.__metadata.uri,
				type: "POST",
				headers: {
					"Accept": "application/json;odata=verbose",
					"X-Http-Method": "DELETE",
					"X-RequestDigest": LS.SP.REST.GetRequestDigest(),
					"If-Match": "*"
				}
			});
		}).then(function () {
			LogMsg("Item deleted");
			return;
		}).fail(LS.Utils.DisplayAJAXError);
};

LS.SP.REST.AddWeb = function (url, metadata) {
    var reqData = "{ 'parameters': { '__metadata': { 'type': 'SP.WebCreationInformation' },'Title': '" + metadata.Title + "', 'Url': '" + metadata.Url + "', 'Description': '" + metadata.Description + "', 'WebTemplate': '" + metadata.WebTemplate + "','UseSamePermissionsAsParentSite': true } }";

    return jQuery.ajax({
        url: url + "/_api/web/webs/add",
        type: "POST",
        contentType: "application/json;odata=verbose",
        data: reqData,
        dataType: 'json',
        headers: {
            "Accept": "application/json;odata=verbose",
            "X-RequestDigest": $('#__REQUESTDIGEST').val()
        }
    });
};