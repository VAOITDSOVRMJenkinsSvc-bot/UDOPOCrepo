var XrmTable = XrmTable || {};
XrmTable.Constants = {
    DefaultSettings: {
        AllowTabOnCardFields: true,
        AllowSort: true,
        RecordsPerPage: 20,
        MultiSelect: false,
        AllowSelect: false,
        AllowDrag: false,
        QuickSort: true,
        name: "XrmTable",
        RenderType: "table",
        ModuleName: "XrmTable",
        debug: false,
        NavigationDisabled: false,
        KeyboardPaginationEnabled: true,
        RememberFocus: true
    },
    Direction: { /* THESE VALUES MUST MATCH KEYCODES */
        UP: 38,
        DOWN: 40,
        RIGHT: 39,
        LEFT: 37,
        HOME: 36,
        END: 35
    },
    CSSClass : {
        HIDDEN: 'hidden',
        SELECTED: 'ui-selected',
        TABLE: "table table-bordered table-hover table-striped table-responsive table-focusoncell",
        ROW: "selectable-row",
        THEAD: "panel-heading panel-title",
        THEADWITHCAPTION: "withcaption",
        CAPTION: "panel-heading panel-title",
        PAGENUMBER_ACTIVE: "active",
        PAGENUMBER_DISABLED: "disabled",
        PAGENUMBER_AREA: "pagination pagination-sm",
        PAGENUMBER_GLYPH_FIRST: "glyphicon glyphicon-fast-backward",
        PAGENUMBER_GLYPH_PREV: "glyphicon glyphicon-backward",
        PAGENUMBER_GLYPH_NEXT: "glyphicon glyphicon-forward",
        PAGENUMBER_GLYPH_LAST: "glyphicon glyphicon-fast-forward",
        SORT_GLYPH_ASCENDING: "glyphicon glyphicon-arrow-up",
        SORT_GLYPH_DESCENDING: "glyphicon glyphicon-arrow-down",
    },
    KeyCode: {
        BACKSPACE: 8,
        TAB: 9,
        ENTER: 13,
        SHIFT: 16,
        ESC: 27,
        SPACE: 32,
        PAGE_UP: 33,
        PAGE_DOWN: 34,
        END: 35,
        HOME: 36,
        LEFT: 37,
        UP: 38,
        RIGHT: 39,
        DOWN: 40,
        DELETE: 46
    },
    SortType : {
        ASCENDING: 'ascending',
        DESCENDING: 'descending',
        NONE: 'none'
    },
    Selectors : {
        ROW: 'tr, [role="row"]',
        CELL: 'th, td, [role="gridcell"]',
        SCROLL_ROW: 'tr:not([data-fixed]), [role="row"]',
        SORT_HEADER: 'th[aria-sort]',
        TABBABLE: '[tabindex="0"]'
    },

}