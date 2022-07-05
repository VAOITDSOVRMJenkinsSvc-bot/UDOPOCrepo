"use strict";
var XrmTable = XrmTable || {};

(function ($) {
    XrmTable.PagingInfo = function (options) {
        var defaultPageInfo = {
            moreRecords: false,
            pagingCookie: "",
            totalRecords: -1,
            exceededLimit: false,
            page: 1,
            totalPages: 1
        };
        if (!(this instanceof XrmTable.PagingInfo)) {
            return new XrmTable.PagingInfo(options);
        }
        if (options instanceof XrmTable.PagingInfo) {
            return new XrmTable.PagingInfo({
                moreRecords: options.getMoreRecords(),
                pagingCookie: options.getPagingCookie(),
                totalRecords: options.getTotalRecords(),
                exceededLimit: options.getExceededLimit(),
                page: options.getPageNumber(),
                totalPages: this.setTotalPages(options.getTotalPages())
            });
        }

        var _info = $.extend(true, {}, defaultPageInfo, options);

        this.setMoreRecords = function (v) {
            if (typeof v === "boolean") {
                _info.moreRecords = v;
            } else {
                throw new TypeError("MoreRecords must be a boolean.");
            }
        };
        this.getMoreRecords = function () {
            return _info.moreRecords;
        };

        this.setPagingCookie = function (v) {
            if (typeof v === "string") {
                _info.pagingCookie = v;
            } else {
                throw new TypeError("PagingCookie must be a string.");
            }
        };
        this.getPagingCookie = function () {
            return _info.pagingCookie;
        };

        this.setTotalRecords = function (v) {
            if (typeof v === "number") {
                _info.totalRecords = v;
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getTotalRecords = function () {
            return _info.totalRecords;
        };

        this.setExceededLimit = function (v) {
            if (typeof v === "boolean") {
                _info.exceededLimit = v;
            } else {
                throw new TypeError("PagingCookie must be a boolean.");
            }
        };
        this.getExceededLimit = function () {
            return _info.exceededLimit;
        };

        this.setPageNumber = function (v) {
            if (typeof v === "number") {
                _info.page = v;
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getPageNumber = function () {
            return _info.page;
        };

        this.setTotalPages = function (v) {
            if (typeof v === "number") {
                _info.totalPages = Math.ceil(v);
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getTotalPages = function () {
            return _info.totalPages;
        };
    };
})(jQuery);