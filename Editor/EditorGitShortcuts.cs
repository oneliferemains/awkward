using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.IO;

public class EditorGitShortcuts : MonoBehaviour
{
  [MenuItem("Assets/git")]
  static protected void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    //string fullPath = getFolderPathContainingGit(Environment.CurrentDirectory);

    //start a git cmd within project context (../Assets)
    //Debug.Log("base root git folder : " + fullPath);

    HalperEditor.ClearConsole();

    string fullPath = Environment.CurrentDirectory;

    //Debug.Log("opening gits from "+ fullPath);
    
    if (folderHasGitFolder(fullPath))
    {
      Debug.Log("current path has git folder : opening");
      HalperNatives.startCmd("git", "--cd=" + fullPath);
    }

    //Debug.Log("seaching for other gits");
    openGitFolderByName("protoss");
  }
  
  static public void openGitFolderByName(string containsFolderName)
  {
    string fullPath = Environment.CurrentDirectory;

    //searcg for protoss git folder and open it
    string path = getFolderPathContainingGit(fullPath, containsFolderName);
    
    //Debug.Log("protoss : " + path);

    if (path.Length > 0)
    {
      Debug.Log("git with name "+containsFolderName+" found and opened");
      HalperNatives.startCmd("git", "--cd=" + path);
    }
    else
    {
      Debug.Log("no git folder with name : "+ containsFolderName);
    }

  }

  static public bool folderHasGitFolder(string basePath)
  {
    string[] dirs = Directory.GetDirectories(basePath);
    for (int i = 0; i < dirs.Length; i++)
    {
      string path = dirs[i];
      path = path.ToLower();
      if (path.Contains(".git")) return true;
    }
    return false;
  }

  /// <summary>
  /// permet de savoir si le dernier dossier dans un path est celui donné en param
  /// </summary>
  static public bool isLastFolderInPath(string path, string folderName)
  {
    path = cleanPathFromFile(path);
    return path.EndsWith(folderName);
  }

  static public bool isLastFolderDotFolder(string path)
  {
    path = path.Replace("\\", "/"); // make path with all /

    // remove all last /
    while (path[path.Length-1] == '/')
    {
      path.Substring(0, path.Length - 1); 
    }
    
    string[] split = path.Split('/');
    return split[split.Length - 1].StartsWith(".");
  }

  static protected string cleanPathFromFile(string path)
  {
    path = path.Replace("\\", "/");

    int extIndex = path.LastIndexOf('.'); // some folder have .
    int lastSlash = path.LastIndexOf('/');

    if (extIndex > 0)
    {
      path = path.Substring(0, lastSlash); // remove any file name
    }

    if (path[path.Length - 1] == '/') path.Substring(0, path.Length - 1); // remove last /

    return path;
  }

  static public string getFolderPathContainingGit(string basePath, string folderName)
  {
    //Debug.Log("path : " + basePath);

    folderName = folderName.ToLower();
    basePath = basePath.ToLower();
    
    if(isLastFolderInPath(basePath, folderName))
    {
      if (folderHasGitFolder(basePath)) return basePath;
    }
    
    //this returns FULL PATHs
    string[] dirs = Directory.GetDirectories(basePath);

    foreach (string dir in dirs)
    {
      if (isLastFolderDotFolder(dir)) continue;

      string output = getFolderPathContainingGit(dir, folderName);
      if (output.Length > 0) return output;
    }
    
    return "";
  }
}
