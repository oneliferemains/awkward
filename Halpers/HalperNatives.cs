using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

using System.Diagnostics;

/// <summary>
/// tools to gain data from current device
/// </summary>

static public class HalperNatives {

  /// <summary>
  /// y a des ordis qui sont config avec des , au lieu de .
  /// </summary>
  static public float floatParseSafe(string strFloat)
  {
    return float.Parse(strFloat, NumberStyles.Any, CultureInfo.InvariantCulture);
  }

  static public string generateUniqId()
  {
    return Guid.NewGuid().ToString();
  }

  /// <summary>
  /// yyyy-mm-dd_hh:mm
  /// </summary>
  static public string getFullDate()
  {
    DateTime dt = DateTime.Now;
    return dt.Year + "-" + dt.Month + "-" + dt.Day + "_" + dt.Hour + "-" + dt.Minute;
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

    string hour = (dt.Hour< 10) ? "0" + dt.Hour: dt.Hour.ToString();
    string min = (dt.Minute < 10) ? "0"+dt.Minute : dt.Minute.ToString();

    return hour + separator + min;
  }

  static public void os_openFolder(string folderPath)
  {
    folderPath = folderPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
    
    //https://stackoverflow.com/questions/334630/opening-a-folder-in-explorer-and-selecting-a-file
    string argument = "/select, \"" + folderPath + "\"";

    //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.process.start?view=netframework-4.7.2#System_Diagnostics_Process_Start_System_String_System_String_
    System.Diagnostics.Process.Start("explorer.exe", argument);
  }


  //https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
  /// <summary>
  /// Windows Store Apps: Application.persistentDataPath points to %userprofile%\AppData\Local\Packages\<productname>\LocalState.
  /// iOS: Application.persistentDataPath points to /var/mobile/Containers/Data/Application/<guid>/Documents.
  /// Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices
  /// </summary>
  /// <returns></returns>
  static public string getDataPath()
  {
    return Application.persistentDataPath;
  }


  static public bool isMobile()
  {
    return Input.touchSupported;
  }


  /// <summary>
  /// meant to call cmd on windows
  /// </summary>
  /// <param name="fullPath"></param>
  /// <param name="args"></param>
  static public void startCmd(string fullPath, string args = "")
  {
    ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    if (args.Length > 0) startInfo.Arguments = args;

    //Debug.Log(Environment.CurrentDirectory);

    Process.Start(startInfo);

  }

}
