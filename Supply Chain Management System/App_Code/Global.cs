using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Global
/// </summary>

    public static class Globals
    {
       public static DateTime serverTime = DateTime.Now;
        public static DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Bangladesh Standard Time");
	}
