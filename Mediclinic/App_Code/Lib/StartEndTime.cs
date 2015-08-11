using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class StartEndTime
{
    public TimeSpan StartTime;
    public TimeSpan EndTime;
    public StartEndTime(TimeSpan startTime, TimeSpan endTime)
    {
        this.StartTime = startTime;
        this.EndTime = endTime;
    }

    private static StartEndTime nullStartEndTime = new StartEndTime(new TimeSpan(-1,-1,-1), new TimeSpan(-1,-1,-1));
    public static StartEndTime NullStartEndTime
    { get {
        return nullStartEndTime;
    } }

    public static TimeSpan NullTimeSpan = new TimeSpan(-1, -1, -1);
}
