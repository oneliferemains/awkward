using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 
/// https://docs.unity3d.com/ScriptReference/EditorBuildSettingsScene.html
/// 
/// </summary>

[CreateAssetMenu(menuName = "protoss/create DataBuildSettingProfilScenes", order = 100)]
public class DataBuildSettingProfilScenes : ScriptableObject
{
  [Header("overrides")]
  //public string build_path = "";
  public string productName = "";

  [Header("scenes")]
  public string[] build_settings_scenes_paths;

#if UNITY_EDITOR
  [ContextMenu("apply")]
  public void apply()
  {
    List<EditorBuildSettingsScene> tmp = new List<EditorBuildSettingsScene>();

    foreach(string path in build_settings_scenes_paths)
    {
      tmp.Add(new EditorBuildSettingsScene(path, true));
    }

    EditorBuildSettings.scenes = tmp.ToArray();

    if (productName.Length > 0) PlayerSettings.productName = productName;
  }

  [ContextMenu("record")]
  public void record()
  {
    List<string> tmp = new List<string>();

    EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
    foreach(EditorBuildSettingsScene sc in scenes)
    {
      Debug.Log(sc.path);
      tmp.Add(sc.path);
    }
    build_settings_scenes_paths = tmp.ToArray();

    EditorUtility.SetDirty(this);
  }
#endif

}