using System;
using UnityEngine;
using System.Collections;

public static class Utils {

    public static double GetTime(this DateTime startDate)
    {
        Int64 retval = 0;
        var st = new DateTime(1970, 1, 1);
        TimeSpan t = (startDate.ToUniversalTime() - st);
        retval = (Int64)(t.TotalMilliseconds + 0.5);
        return retval;
    }
}
