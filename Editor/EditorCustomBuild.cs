﻿using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class EditorCustomBuild
{
  [MenuItem("Build/Windows Release Build With External")]
  public static void BuildReleaseGame()
  {
    BuildGame(BuildOptions.ShowBuiltPlayer);
  }

  [MenuItem("Build/Windows Dev Build With External")]
  public static void BuildDevGame()
  {
    BuildGame(BuildOptions.Development | BuildOptions.ShowBuiltPlayer);
  }

  [MenuItem("Build/Mac Dev Build With External")]
  public static void BuildMacDevGame()
  {
    BuildGame(BuildOptions.Development | BuildOptions.ShowBuiltPlayer, BuildTarget.StandaloneOSX);
  }

  static void BuildGame(BuildOptions buildOptions, BuildTarget targetPlatform = BuildTarget.StandaloneWindows)
  {
    // executable
    string buildFolder = "Assets/../Build";
    if (!Directory.Exists(buildFolder))
    {
      UnityEngine.Debug.Log("Creating directory " + buildFolder);
      Directory.CreateDirectory(buildFolder);
    }

    string buildName = PlayerSettings.productName;
    if ((buildOptions & BuildOptions.Development) == BuildOptions.Development) buildName += "_dev";
    else buildName += "_release";
    buildName += "_" + VersionManager.getFormatedVersion();

    buildFolder += "/" + buildName;
    if (!Directory.Exists(buildFolder))
    {
      UnityEngine.Debug.Log("Creating directory " + buildFolder);
      Directory.CreateDirectory(buildFolder);
    }

    BuildPlayerOptions options = new BuildPlayerOptions();

    // editor build scenes list
    options.scenes = getScenePaths();

    options.target = targetPlatform;

    // path
    if(targetPlatform == BuildTarget.StandaloneOSX)
    {
      buildFolder += "_osx";
      options.locationPathName = buildFolder + "/" + buildName + ".app";
    }else
    {
      options.locationPathName = buildFolder + "/" + buildName + ".exe";
    }
    
    // flags (dev build)
    options.options = buildOptions;

    UnityEngine.Debug.Log(options.locationPathName);

    BuildPipeline.BuildPlayer(options);

    

    // externals
    FileUtil.DeleteFileOrDirectory(buildFolder + "/external/");
    string externalPath = HalperExternal.GetExternalFolder();

    //externalPath = externalPath.Replace("/", "\\");

    Debug.Log("now copying external folder : "+externalPath);

    if (System.IO.Directory.Exists(externalPath))
    {
      FileUtil.CopyFileOrDirectory(externalPath, buildFolder + "/external");
    }
    else
    {
      Debug.LogWarning("folder "+externalPath+" doesn't exist ? can't copy it");
    }
  }


  static protected string[] getScenePaths()
  {

    List<string> sceneNames = new List<string>();
    int count = SceneManager.sceneCountInBuildSettings;

    UnityEngine.Debug.Log("getting scenes paths (count : " + count+")");

    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

    for (int i = 0; i < scenes.Length; i++)
    {
      sceneNames.Add(scenes[i].path);
      UnityEngine.Debug.Log("  #" + i + " --> " + scenes[i].path);
    }

    return sceneNames.ToArray();
  }

}