using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

static public class EditorContextMenu {

  [MenuItem("Assets/git")]
  static public void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    string fullPath = "git";

    startCmd("git", "--cd=" + Environment.CurrentDirectory);
    
    fullPath = Path.Combine(Environment.CurrentDirectory, "Assets/Lib/fwProtoss");
    if (Directory.Exists(fullPath))
    {
      startCmd("git", "--cd=" + fullPath);
    }
    
  }

  [MenuItem("Assets/readme")]
  static public void openReadme()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    string fullPath = Environment.CurrentDirectory+"/README.md";

    //Debug.Log(fullPath);
    
    if (!File.Exists(fullPath))
    {
      FileStream stream = File.Create(fullPath);
      stream.Close();
    }

    startCmd(fullPath);
  }
  
  static public void startCmd(string fullPath, string args = "")
  {
    ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    if(args.Length > 0) startInfo.Arguments = args;

    //Debug.Log(Environment.CurrentDirectory);

    Process.Start(startInfo);

  }
  
}
