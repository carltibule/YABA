import dayjs from "dayjs";

const utc = require('dayjs/plugin/utc')
const timezone = require('dayjs/plugin/timezone') // dependent on utc plugin
dayjs.extend(utc);
dayjs.extend(timezone);

export default class DateTimeHelper {
    static ISO8601_DATETIMEFORMAT = () => {
        return 'YYYY-MM-DD HH:mm';
    };
    
    static toLocalDateTime = (dateTimeString, formatString) => {
        return dayjs(dateTimeString).local().format(formatString ?? this.ISO8601_DATETIMEFORMAT());
    };

    static toString = (dateTimeString, formatString) => {
        return dayjs(dateTimeString).format(formatString);
    };

    static getFriendlyDate = (dateTimeString) => {
        const dateTimeStringInUtc = dayjs(dateTimeString).utc();
        const currentUtcDateTime = dayjs().utc();

        if(currentUtcDateTime.diff(dateTimeStringInUtc, 'day') <= 7) {
            return dateTimeStringInUtc.local().format('dddd');
        } else if(currentUtcDateTime.diff(dateTimeStringInUtc, 'week') <= 4) {
            const differenceInWeeks = currentUtcDateTime.diff(dateTimeStringInUtc, 'week');
            return `${differenceInWeeks} ${ differenceInWeeks <= 1 ? 'week' : 'weeks'} ago`;
        } else if(currentUtcDateTime.diff(dateTimeStringInUtc, 'month') <= 12) {
            const differenceInMonths = currentUtcDateTime.diff(dateTimeStringInUtc, 'month');
            return `${differenceInMonths} ${ differenceInMonths <= 1 ? 'month' : 'months' } ago`;
        } else {
            const differenceInYears = currentUtcDateTime.diff(dateTimeStringInUtc, 'year');
            return `${differenceInYears} ${ differenceInYears <= 1 ? 'year' : 'years'} ago`;
        }
    };
}