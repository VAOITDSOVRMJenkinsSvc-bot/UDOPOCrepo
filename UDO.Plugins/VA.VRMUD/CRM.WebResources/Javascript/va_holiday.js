var holidays = [];

var datesUtility = {
    convert: function (d) {
        // Converts the date in d to a date-object. The input can be:
        //   a date object: returned without modification
        //  an array      : Interpreted as [year,month,day]. NOTE: month is 0-11.
        //   a number     : Interpreted as number of milliseconds
        //                  since 1 Jan 1970 (a timestamp) 
        //   a string     : Any format supported by the javascript engine, like
        //                  "YYYY/MM/DD", "MM/DD/YYYY", "Jan 31 2009" etc.
        //  an object     : Interpreted as an object with year, month and date
        //                  attributes.  **NOTE** month is 0-11.
        return (
            d.constructor === Date ? d :
                d.constructor === Array ? new Date(d[0], d[1], d[2]) :
                    d.constructor === Number ? new Date(d) :
                        d.constructor === String ? new Date(d) :
                            typeof d === "object" ? new Date(d.year, d.month, d.date) :
                                NaN
        );
    },
    compare: function (a, b) {
        // Compare two dates (could be of any type supported by the convert
        // function above) and returns:
        //  -1 : if a < b
        //   0 : if a = b
        //   1 : if a > b
        // NaN : if a or b is an illegal date
        // NOTE: The code inside isFinite does an assignment (=).
        return (
            isFinite(a = this.convert(a).valueOf()) &&
                isFinite(b = this.convert(b).valueOf()) ?
                (a > b) - (a < b) :
                NaN
        );
    },
    inRange: function (d, start, end) {
        // Checks if date in d is between dates in start and end.
        // Returns a boolean or NaN:
        //    true  : if d is between start and end (inclusive)
        //    false : if d is before start or after end
        //    NaN   : if one or more of the dates is illegal.
        // NOTE: The code inside isFinite does an assignment (=).
        return (
            isFinite(d = this.convert(d).valueOf()) &&
                isFinite(start = this.convert(start).valueOf()) &&
                isFinite(end = this.convert(end).valueOf()) ?
                start <= d && d <= end :
                NaN
        );
    }
};

function GetListOfHolidays() {
    var columns = ['va_Date'];

    CrmRestKit2011.ByQuery("va_holiday", columns).done(function (data) {
        if (data && data.d && data.d.results) {
            for (var i = 0; i < data.d.results.length; i++) {
                var holidayDate = GetDate(data.d.results[i].va_Date).setHours(0, 0, 0, 0);
                holidays.push(holidayDate);
            }
        }
    }).fail(function(err) {
        UTIL.restKitError(err, 'Failed to retrieve the VA Holidays:');
    });
}

// Returns date from a string in "/Date(???)/" format.
function GetDate(dateString) {
    return new Date(parseInt(dateString.replace("/Date(", "").replace(")/", ""), 10));
}

function GetHoldayByDate(date) {

    var dateNoHours = new Date(date.getValue()).setHours(0, 0, 0, 0);
    var d = new Date(date.getValue());

    for (var i = 0; i < holidays.length; i++) {
        if (datesUtility.compare(holidays[i], dateNoHours) == 0)
            date.setValue(d.setDate(d.getDate() + 1));

    }
    return;
}

function isHoliday(date) {

    var dateNoHours = date.setHours(0, 0, 0, 0);
    //var d = new Date(date.getValue());
    for (var i = 0; i < holidays.length; i++) {
        if (datesUtility.compare(holidays[i], dateNoHours) == 0)
            return true;

    }
    return false;
}

// RU12 Remove anonymus function place; place the GetListOfHolidays() in the load function;
//(function () {
//    GetListOfHolidays();
//}());

