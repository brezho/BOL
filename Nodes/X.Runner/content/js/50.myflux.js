var Emitter = function () {
    this._callbacks = [];
    this._promises = [];
}

Emitter.prototype.on = function (eventName, callbackFunction) {
    var cbs = this._callbacks[eventName];
    if (cbs == undefined) {
        this._callbacks.push(eventName);
        this._callbacks[eventName] = [];
        cbs = this._callbacks[eventName];
    }
    cbs.push(callbackFunction);
};
Emitter.prototype.raise = function (eventName, payload) {
    var resolves = [];
    var rejects = [];

    var cbs = this._callbacks[eventName];
    if (cbs != undefined) {
        this._promises = this._callbacks[eventName].map(function (_, i) {
            return new Promise(function (resolve, reject) {
                resolves[i] = resolve;
                rejects[i] = reject;
            });
        });
        // Dispatch to callbacks and resolve/reject promises.
        this._callbacks[eventName].forEach(function (callback, i) {
            // Callback can return an obj, to resolve, or a promise, to chain.
            // See waitFor() for why this might be useful.
            Promise.resolve(callback(payload)).then(function () {
                resolves[i](payload);
            }, function () {
                rejects[i](new Error('Dispatcher callback unsuccessful'));
            });
        });
        this._promises = [];
    }
    else {
        console.log('No subscribers for ' + eventName);
    }
};


var Dispatcher = function () {
    this._callbacks = [];
    this._promises = [];
};

Dispatcher.prototype = {

    /**
     * Register a Store's callback so that it may be invoked by an action.
     * @param {function} callback The callback to be registered.
     * @return {number} The index of the callback within the _callbacks array.
     */
    register: function (callback) {
        this._callbacks.push(callback);
        return this._callbacks.length - 1; // index
    },

    /**
     * dispatch
     * @param  {object} payload The data from the action.
     */
    dispatch: function (eventName, payload) {
        // First create array of promises for callbacks to reference.
        var resolves = [];
        var rejects = [];
        this._promises = this._callbacks.map(function (_, i) {
            return new Promise(function (resolve, reject) {
                resolves[i] = resolve;
                rejects[i] = reject;
            });
        });
        // Dispatch to callbacks and resolve/reject promises.
        this._callbacks.forEach(function (callback, i) {
            // Callback can return an obj, to resolve, or a promise, to chain.
            // See waitFor() for why this might be useful.
            Promise.resolve(callback(eventName, payload)).then(function () {
                resolves[i](payload);
            }, function () {
                rejects[i](new Error('Dispatcher callback unsuccessful'));
            });
        });
        this._promises = [];
    }
};

var Store = function () {

    Emitter.call(this);

    var args = Array.prototype.slice.call(arguments);
    var me = this;
    args.forEach(function (argument, i) {
        if (argument instanceof Dispatcher) {
            argument.register(function (evt, payload) { me.handler(evt, payload); });
        }
        else me.data = argument;
    });

    if (this.data == undefined) this.data = {};
}

Store.prototype = Object.create(Emitter.prototype);
// re-assign the constructor to correct function
Store.prototype.constructor = Store;

Store.prototype.handler = function (evt, payload) { };
Store.prototype.dataChanged = function () { this.raise('___DATACHANGED'); };
Store.prototype.onDataChanged = function (callbackFunction) { this.on('___DATACHANGED', callbackFunction); };