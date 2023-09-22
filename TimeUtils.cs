using System.ComponentModel;

namespace JFUtils;

[Description("Thanks to Azu ;)")]
public static class TimeUtils
{
    private static string GetCurrentTimeString()
    {
        return string.Format("<b>{0}</b>", GetCurrentTime().ToString("HH:mm"));
    }

    [Description("HH.mm")]
    public static (int, int) GetCurrentTimeValue()
    {
        var theTime = GetCurrentTime();
        if (theTime == DateTime.Now) return (-1, -1);
        return (theTime.Hour, theTime.Minute);
    }

    private static DateTime GetCurrentTime()
    {
        var now = DateTime.Now;
        if (!EnvMan.instance) return now;

        var smoothDayFraction = EnvMan.instance.m_smoothDayFraction;
        var num = (int)(smoothDayFraction * 24f);
        var num2 = (int)((smoothDayFraction * 24f - num) * 60f);
        var second = (int)(((smoothDayFraction * 24f - num) * 60f - num2) * 60f);
        var theTime = new DateTime(now.Year, now.Month, now.Day, num, num2, second);
        //int currentDay = EnvMan.instance.GetCurrentDay();
        //return TimeUtils.GetCurrentTimeString(theTime);
        return theTime;
    }
}