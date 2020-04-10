#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorContextMenuToolsGit : MonoBehaviour
{
  //[MenuItem("Assets/git !#&%g")]
  [MenuItem("Assets/git &%g")]
  static protected void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    //string fullPath = getFolderPathContainingGit(Environment.CurrentDirectory);

    //start a git cmd within project context (../Assets)
    //Debug.Log("base root git folder : " + fullPath);

    HalperEditor.ClearConsole();

    string fullPath = Environment.CurrentDirectory;
    Debug.Log(fullPath);

    //Debug.Log("opening gits from "+ fullPath);
    
    if (pathHasGitFolder(fullPath))
    {
      Debug.Log("current path has git folder : opening");
      HalperNatives.startCmd("git", "--cd=" + fullPath);
    }

    //Directory.Exists(fullPath)

    string dirAssets = Path.Combine(fullPath, "Assets");
    string dirLib = Path.Combine(dirAssets, "lib");

    if(Directory.Exists(dirLib))
    {

      string[] dirs = Directory.GetDirectories(dirLib);
      foreach(string dir in dirs)
      {
        if (Directory.Exists(dir))
        {
          //int lastIndex = file.LastIndexOf('\\');
          //string nm = file.Substring(file.LastIndexOf('\\'));

          if(pathHasGitFolder(dir))
          {
            openGitFolderByPath(dir);
          }
          
          //nm = nm.Substring(1); // remove last \
          //if (nm.StartsWith("fw")) nm = nm.Substring(2); // remove fw

          //openGitFolderByName(nm);
        }
      }
    }

    //Debug.Log("seaching for other gits");
    //openGitFolderByName("protoss");
  }
  
  static public void openGitFolderByName(string containsFolderName)
  {
    string fullPath = Environment.CurrentDirectory;

    //searcg for protoss git folder and open it
    string path = getFolderPathContainingGit(fullPath, containsFolderName);

    //Debug.Log("protoss : " + path);

    openGitFolderByPath(path);
  }

  static public void openGitFolderByPath(string path)
  {

    if (path.Length <= 0) Debug.LogWarning("no path given ?");
    else
    {
      Debug.Log("exec git command in : "+path);
      HalperNatives.startCmd("git", "--cd=" + path);
    }

  }

  static public bool pathHasGitFolder(string basePath)
  {
    //Debug.Log(basePath);

    string path;

    //search for a /.git/ folder
    string[] dirs = Directory.GetDirectories(basePath);
    for (int i = 0; i < dirs.Length; i++)
    {
      path = dirs[i].ToLower();
      if (path.Contains(".git")) return true;
    }

    //search for .git file (submodules)
    string[] files = Directory.GetFiles(basePath);
    for (int i = 0; i < files.Length; i++)
    {
      path = files[i].ToLower();
      if (path.Contains(".git")) return true;
    }

    return false;
  }
  
  /// <summary>
  /// recurcively search for git folder
  /// </summary>
  static private string getFolderPathContainingGit(string basePath, string folderName)
  {
    //Debug.Log("path : " + basePath);

    folderName = folderName.ToLower();
    basePath = basePath.ToLower();

    
    if(HalperIO.isLastFolderInPath(basePath, folderName))
    {
      //Debug.Log(basePath);

      if (pathHasGitFolder(basePath)) return basePath;
    }
    
    //this returns FULL PATHs
    string[] dirs = Directory.GetDirectories(basePath);

    foreach (string dir in dirs)
    {
      if (HalperIO.isLastFolderDotFolder(dir)) continue;

      string output = getFolderPathContainingGit(dir, folderName);
      if (output.Length > 0) return output;
    }
    
    return "";
  }
}
#endif