using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 
/// https://docs.unity3d.com/Manual/LogFiles.html
/// C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log
/// 
/// </summary>

public class DebugOpenLog : MonoBehaviour
{
  //string path = "c:/Users/%UserProfile%/AppData/";

  void Update()
  {
    if(Input.GetKeyUp(KeyCode.L))
    {
      throw new System.NotImplementedException("nope");
      //startCmd(path);
    }
  }

  static public void startCmd(string fullPath, string args = "")
  {
    Debug.Log(fullPath);

    ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    if (args.Length > 0) startInfo.Arguments = args;

    //Debug.Log(Environment.CurrentDirectory);

    Process.Start(startInfo);

  }

}
