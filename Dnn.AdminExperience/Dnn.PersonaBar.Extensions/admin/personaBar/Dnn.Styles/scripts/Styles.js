﻿define(['main/config'], function (cf) {
    'use strict';
    var identifier;
    var config = cf.init();

    var init = function (wrapper, util, params, callback) {
        identifier = params.identifier;

        window.dnn.initStyles = function () {
            return {
                utility: util,
                params: params,
                moduleName: "Styles"
            };
        };

        if (typeof callback === 'function') {
            callback();
        }
    };

    var load = function (params, callback) {
        if (typeof callback === 'function') {
            callback();
        }
    };

    return {
        init: init,
        load: load
    };
});
