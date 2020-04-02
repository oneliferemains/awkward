using UnityEditor;
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
    BuildGame(BuildOptions.Development | BuildOptions.ShowBuiltPlayer, getMacOsBuildTarget());
  }

  static void BuildGame(BuildOptions buildOptions, BuildTarget targetPlatform = BuildTarget.StandaloneWindows)
  {
    float startTime = Time.time;
    Debug.Log("started build process");

    string buildFolder = generateBuildsFolder();

    // generate build name
    string buildName = PlayerSettings.productName;
    if ((buildOptions & BuildOptions.Development) == BuildOptions.Development) buildName += "_dev";
    else buildName += "_release";
    buildName += "_" + VersionManager.getFormatedVersion();

    Debug.Log("  L <b>build name</b> : "+buildName);

    // generate the output build's folder
    buildFolder += "/" + buildName;
    if (!Directory.Exists(buildFolder))
    {
      UnityEngine.Debug.Log("  ... creating directory " + buildFolder);
      Directory.CreateDirectory(buildFolder);
    }

    Debug.Log("  L <b>output folder created</b> : "+buildFolder);

    BuildPlayerOptions options = new BuildPlayerOptions();

    // use all editor build scenes list
    options.scenes = getScenePaths();

    Debug.Log("  L building with <b>"+options.scenes.Length+"</b> scenes from editor settings");

    options.target = targetPlatform;

    Debug.Log("  L target platform is <b>"+options.target+"</b>");

    // path
    if (targetPlatform == getMacOsBuildTarget())
    {
      buildFolder += "_osx";
      options.locationPathName = buildFolder + "/" + buildName + ".app";
    }else
    {
      options.locationPathName = buildFolder + "/" + buildName + ".exe";
    }

    Debug.Log("  L path is <b>" + options.locationPathName+ "</b>");

    // flags (dev build)
    options.options = buildOptions;

    Debug.Log("  L now <b>building app</b> ...");

    BuildPipeline.BuildPlayer(options);

    Debug.Log("  L app building success !");


    // externals
    FileUtil.DeleteFileOrDirectory(buildFolder + "/"+HalperCustomBuild.getExternalFolderName()+"/");

    string externalPath = HalperExternal.GetExternalFolder();
    string externalOutputPath = buildFolder + "/"+ HalperCustomBuild.getExternalFolderName();

    //externalPath = externalPath.Replace("/", "\\");

    Debug.Log("  L now <b>copying external folder</b> : " + externalPath);

    if (Directory.Exists(externalPath))
    {
      FileUtil.CopyFileOrDirectory(externalPath, externalOutputPath);
    }
    else
    {
      Debug.LogWarning("folder "+externalPath+" doesn't exist ? can't copy it");
    }

    HalperExternal.removeAllFilesOfAType(externalOutputPath, "meta");

    Debug.Log("build process <color=green>completed</color>");
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

  static protected BuildTarget getMacOsBuildTarget()
  {

#if UNITY_2017_1_OR_NEWER
    return BuildTarget.StandaloneOSX;
#else
    return BuildTarget.StandaloneOSXIntel64; // always intel ?
#endif

  }


  [MenuItem("Build/Open builds folder")]
  static protected void openBuildsFolder()
  {
    generateBuildsFolder();
    EditorContextMenu.startCmd(getBuildsFolderPath());
  }

  static protected string getBuildsFolderPath()
  {
    string buildFolder = Application.dataPath; // path/to/Assets
    buildFolder = buildFolder.Substring(0, buildFolder.LastIndexOf('/')); // "/Assets" from path
    buildFolder += "/Build";

    return buildFolder;
  }

  static protected string generateBuildsFolder()
  {
    string buildFolderPath = getBuildsFolderPath();

    // create the root Build/ folder of not present
    if (!Directory.Exists(buildFolderPath))
    {
      Debug.Log("  ... creating directory " + buildFolderPath);
      Directory.CreateDirectory(buildFolderPath);
    }

    //Debug.Log("  L root build folder created");

    return buildFolderPath;
  }
}