using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class WeekDayDB
{

    public static DayOfWeek GetDayOfWeek(int weekday_id)
    {
        if (weekday_id == 1)
            return DayOfWeek.Sunday;
        if (weekday_id == 2)
            return DayOfWeek.Monday;
        if (weekday_id == 3)
            return DayOfWeek.Tuesday;
        if (weekday_id == 4)
            return DayOfWeek.Wednesday;
        if (weekday_id == 5)
            return DayOfWeek.Thursday;
        if (weekday_id == 6)
            return DayOfWeek.Friday;
        if (weekday_id == 7)
            return DayOfWeek.Saturday;

        throw new ArgumentException("Unknown weekday ID");
    }

    public static int GetWeekDayID(DayOfWeek dayOfWeek)
    {
        if (dayOfWeek == DayOfWeek.Sunday)
            return 1;
        if (dayOfWeek == DayOfWeek.Monday)
            return 2;
        if (dayOfWeek == DayOfWeek.Tuesday)
            return 3;
        if (dayOfWeek == DayOfWeek.Wednesday)
            return 4;
        if (dayOfWeek == DayOfWeek.Thursday)
            return 5;
        if (dayOfWeek == DayOfWeek.Friday)
            return 6;
        if (dayOfWeek == DayOfWeek.Saturday)
            return 7;

        throw new ArgumentException("Unknown day of week");
    }

}