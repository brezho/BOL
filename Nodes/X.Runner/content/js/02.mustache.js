(function () {
    var root = this;

    var mustache = new function () {
        this.loadedTemplates = [];
    };

    if (typeof exports !== 'undefined') {
        if (typeof module !== 'undefined' && module.exports) {
            exports = module.exports = mustache;
        }
        exports.mustache = mustache;
    } else {
        root['mustache'] = mustache;
    }

    mustache.transform = function (url, data) {
        var tmpl = _.find(mustache.loadedTemplates, function (tmpl) { return tmpl.Url == url; });

        if (tmpl == null) {
            var tmplDefinition = http.load(url, false);
            var splitted = url.split('/');
            var name = '';
            _.each(splitted, function (split) { name += split.replace('.', '_'); });
            tmpl = { Name: name, Url: url };
            mustache.loadedTemplates.push(tmpl);
            ich.addTemplate(name, tmplDefinition);
        }

        var isArr = $.isArray(data);
        var instruction;
        if (isArr) {
            var i = 0;
            _.each(data, function (item) { item.index = i; i++; });
            instruction = 'ich.' + tmpl.Name + '({"data" : data}, true);';
        }
        else instruction = 'ich.' + tmpl.Name + '(data, true);';
        return eval(instruction);
    };

}).call(this);
