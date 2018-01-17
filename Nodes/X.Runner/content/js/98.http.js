
(function () {
    var root = this;

    var http = new function () { };

    if (typeof exports !== 'undefined') {
        if (typeof module !== 'undefined' && module.exports) {
            exports = module.exports = http;
        }
        exports.http = http;
    } else {
        root['http'] = http;
    }

    http.get = function (url) {
        var request = http.getRequest();
        request.open("GET", url, false);
        request.send(null);
        item = request.responseText;
        return item;
    };

    http.load = function (url, useCache) {

        var splitted = url.split('/');
        var name = '';
        _.each(splitted, function (split) { name += split.replace('.', '_'); });

        var item = null;
        if (useCache == true) item = $.jStorage.get(name);

        if (item == null) {
            var response = http.get(url);
            if (useCache == true) $.jStorage.set(name, response)
        }

        return response;
    };


    http.getRequest = function makeHttpObject() {
        try { return new XMLHttpRequest(); }
        catch (error) { }
        try { return new ActiveXObject("Msxml2.XMLHTTP"); }
        catch (error) { }
        try { return new ActiveXObject("Microsoft.XMLHTTP"); }
        catch (error) { }

        throw new Error("Could not create HTTP request object.");
    }

}).call(this);
