using System;

public class HalperTime
{

  /// <summary>
  /// yyyy-mm-dd_hh:mm
  /// </summary>
  static public string getFullDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Year + "-" + dt.Month + "-" + dt.Day + "_" + dt.Hour + "-" + dt.Minute;
  }

  static public string getDbDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Year + "-" + dt.Month + "-" + dt.Day;
  }

  static public string getFrDate(bool addZeros = false)
  {
    DateTime dt = DateTime.Now;
    if (addZeros)
    {
      string day = dt.Day < 10 ? "0" + dt.Day : dt.Day.ToString();
      string month = dt.Month < 10 ? "0" + dt.Month : dt.Month.ToString();
      return day + "-" + month + "-" + dt.Year;
    }
    return dt.Day + "-" + dt.Month + "-" + dt.Year;
  }

  static public string getNowHourMin(char separator = ':')
  {
    DateTime dt = DateTime.Now;

    string hour = (dt.Hour < 10) ? "0" + dt.Hour : dt.Hour.ToString();
    string min = (dt.Minute < 10) ? "0" + dt.Minute : dt.Minute.ToString();

    return hour + separator + min;
  }

}
