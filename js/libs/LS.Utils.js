// replacement function for console.log(), which avoids 'undefined' exception in IE 8
window.LogMsg = function (msg) {
    if (window.console) {
        console.log(msg);
    }
};

// set up namespaces
window.LS = window.LS || {};
LS.Utils = LS.Utils || {};

// namespace creation function
LS.Utils.EnsureNamespace = function (ns) {
    nsArr = ns.split('.'); // split into array
    var obj = window; // start at window object
    for (var i = 0; i < nsArr.length; i++) {
        if (nsArr[i] == "window") // skip window if this is included in string
            continue;
        obj[nsArr[i]] = obj[nsArr[i]] || {}; // create an empty object
        obj = obj[nsArr[i]]; // get the new object and continue
    }
    LogMsg("Added namespace: " + ns);
};

// check if a global variable exists
LS.Utils.GlobalExists = function (global) {
    var globalArr = global.split('.'); // split into array
    var obj = window; // start at window object
    for (var i = 0; i < globalArr.length; i++) {
        if (globalArr[i] == "window") // skip window if this is included in string
            continue;
        if (!obj[globalArr[i]])
            return false; // the global namespace does not exist
        obj = obj[globalArr[i]]; // get the new object and continue
    }
    return true; // the global namespace exists
};

// execute a callback when a global is present
LS.Utils.ExecuteOnGlobal = function (global, func) {
    if (!LS.Utils.GlobalExists(global)) {
        setTimeout(function () {
            LS.Utils.ExecuteOnGlobal(global, func);
        }, 100);
    } else {
        func();
    }
};

// add a script to the page
LS.Utils.AddScript = function (url) {
    var script = document.createElement("script");
    script.setAttribute("src", url);
    var head = document.getElementsByTagName("head")[0];
    head.appendChild(script);
};

// check for a global variable, load a script if it doesn't exist and execute a callback once the global variable is present
LS.Utils.EnsureLibrary = function (global, url, func) {
    if (!LS.Utils.GlobalExists(global)) {
        LS.Utils.AddScript(url);
    }

    LS.Utils.ExecuteOnGlobal(global, func);
};

// adapted from http://stackoverflow.com/a/21152762
LS.Utils.GetQueryString = (function () {
    var queryStrings = {};

    var qs = window.location.search.substr(1).split("&");
    for (var i = 0; i < qs.length; i++) {
        var item = qs[i];
        queryStrings[item.split("=")[0]] = decodeURIComponent(item.split("=")[1]);
    }

    return function (key) {
        return queryStrings[key];
    };
})();

// Source: SR http://code.msdn.microsoft.com/office/SharePoint-2013-Folder-661709eb
LS.Utils.ReplaceQueryStringAndGet = function (url, key, value) {
    var re = new RegExp("([?|&])" + key + "=.*?(&|$)", "i");
    var separator = url.indexOf('?') !== -1 ? "&" : "?";
    if (url.match(re)) {
        return url.replace(re, '$1' + key + "=" + value + '$2');
    } else {
        return url + separator + key + "=" + value;
    }
};

// helper for sorting arrays by property value (where getObjectProperty is a function that gets the object property that you would like to sort by)
LS.Utils.SortFunction = function (getObjectProperty, sortAscending) {
    return function (objA, objB) {
        var a = getObjectProperty(objA),
			b = getObjectProperty(objB);

        if (!sortAscending)
            var c = a,
				a = b,
				b = c; // swap a and b
        if (a < b)
            return -1;
        if (a > b)
            return 1;
        return 0;
    };
};

// if string 'str' contains a string in the array 'arr' return true, otherwise return false
LS.Utils.StringContainsArrayString = function (str, arr) {
    return (jQuery.grep(arr, function (value, index) {
        return str.indexOf(value) > -1;
    })).length > 0;
};

// return a copy of the array with duplicates removed
LS.Utils.ArrayUnique = function (array) {
    var uniqueArray = [];
    jQuery.each(array, function (index, value) {
        if (jQuery.inArray(value, uniqueArray) === -1)
            uniqueArray.push(value);
    });
    return uniqueArray;
};

// AJAX error callback
LS.Utils.DisplayAJAXError = function (request, status, error) {
    LogMsg(["Error", error]);
};

LS.Utils.DisplayError = function () {
    LogMsg(Array.prototype.slice.call(arguments));
};

// source: SO http://stackoverflow.com/a/2117523
LS.Utils.CreateGuid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0,
			v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};

LS.Utils.RateLimit = (function () {
    var dictionary = {};
    var unlock = function (key) {
        dictionary[key].Timer = -1; /* allow execution of 'func' when 'rate' timeout has expired */
        if (dictionary[key].Queued) { /* if requested during the cooldown, trigger the function when the cooldown has finished */
            dictionary[key].Queued = false;
            execute(key);
        }
    };
    var execute = function (key) {
        var item = dictionary[key];
        item.Timer = setTimeout(function () {
            unlock(key);
        }, item.Rate);
        item.OnExecute();
    };
    var executeOrQueue = function (key) {
        /* allow the function to be executed subsequent times at the rate specified */
        if (dictionary[key].Timer == -1) {
            execute(key);
        } else {
            dictionary[key].Queued = true;
        }
    };
    var addAndExecute = function (key, func, rate) {
        dictionary[key] = {
            OnExecute: func,
            Timer: -1,
            Queued: false,
            Rate: rate
        };
        execute(key); /* execute the function the first time and start the rate limit timer */
    };

    return function (key, func, rate) {
        if (!dictionary[key]) { /* add the key to the dictionary if it doesn't already exist */
            addAndExecute(key, func, rate);
        } else {
            executeOrQueue(key);
        }
    };
})();

// #region - Prototype extensions

// #region - String

// source: http://cwestblog.com/2011/07/25/javascript-string-prototype-replaceall/
if (!String.prototype.replaceAll) {
    String.prototype.replaceAll = function (target, replacement) {
        return this.split(target).join(replacement);
    };
}

// source: SO http://stackoverflow.com/a/18234317
if (!String.prototype.format) {
    String.prototype.format = function () {
        var str = this.toString();
        if (!arguments.length)
            return str;
        var args = typeof arguments[0],
			args = (("string" == args || "number" == args) ? arguments : arguments[0]);
        for (arg in args)
            str = str.replace(RegExp("\\{" + arg + "\\}", "gi"), args[arg]);
        return str;
    };
}
// #endregion

// #region - Date
if (!Date.addYears) {
    Date.prototype.addYears = function (modifier) {
        return new Date(this.getFullYear() + modifier, this.getMonth(), this.getDate());
    };
}

if (!Date.addMonths) {
    Date.prototype.addMonths = function (modifier) {
        return new Date(this.getFullYear(), this.getMonth() + modifier, this.getDate());
    };
}

if (!Date.addDays) {
    Date.prototype.addDays = function (modifier) {
        return new Date(this.getFullYear(), this.getMonth(), this.getDate() + modifier);
    };
}
// #endregion

// #endregion
