using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.SceneManagement;

static public class HalperScene {

  static public void setupObjectChildOfSceneOfObject(GameObject target, GameObject owner)
  {

    //https://stackoverflow.com/questions/45798666/move-transfer-gameobject-to-another-scene
    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(target, owner.scene);

  }

  static public Scene[] getOpenedScenesOfPrefix(string prefix, string filter)
  {
    List<Scene> list = new List<Scene>();
    int count = SceneManager.sceneCount;
    for (int i = 0; i < count; i++)
    {
      Scene sc = SceneManager.GetSceneAt(i);

      if (sc.name.Contains(filter)) continue;
      if (!sc.name.StartsWith(prefix)) continue;

      if (sc.isLoaded) list.Add(sc);
    }
    return list.ToArray();
  }

  /// <summary>
  /// active scene
  /// </summary>
  /// <param name="partOfSceneName"></param>
  /// <returns></returns>
  static public bool isActiveScene(string partOfSceneName)
  {
    Scene sc = SceneManager.GetActiveScene();
    if (sc.name.Contains(partOfSceneName)) return true;
    return false;
  }

  static public bool isActiveScene(Scene sc)
  {
    return sc == SceneManager.GetActiveScene();
  }

  static public void loadScene(string sceneName)
  {

  }

  static public Scene getSceneFromAdded(string sceneName)
  {
    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      Scene sc = SceneManager.GetSceneAt(i);
      if(sc.isLoaded && sc.IsValid())
      {
        if (sc.name == sceneName) return sc;
      }
    }
    return default(Scene);
  }

  static public T[] getComponentsInScene<T>(Scene sc) where T : Component
  {
    List<T> output = new List<T>();
    GameObject[] roots = sc.GetRootGameObjects();
    for (int i = 0; i < roots.Length; i++)
    {
      output.AddRange(roots[i].GetComponentsInChildren<T>());
    }
    return output.ToArray();
  }

#if UNITY_EDITOR

  /// <summary>
  /// Récupère un tableau des scènes/chemin d'accès qui sont présente dans les paramètres du build
  /// </summary>
  /// <param name="removePath">Juste le nom (myScene) ou tout le chemin d'accès (Assets/folder/myScene.unity) ?</param>
  /// <returns>Le tableau avec le nom ou chemin d'accès aux scènes.</returns>
  static public string[] getAllBuildScenes(bool includeSceneOnly, bool removePath)
  {
    string[] scenes = new string[] { };

    if (includeSceneOnly)
    {
      scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
    }
    else
    {
      EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

      scenes = new string[buildScenes.Length];

      for (int i = 0; i < scenes.Length; i++)
      {
        scenes[i] = buildScenes[i].path;
      }
    }

    if (removePath)
    {
      for (int i = 0; i < scenes.Length; i++)
      {
        int slashIndex = scenes[i].LastIndexOf('/');

        if (slashIndex >= 0)
        {
          scenes[i] = scenes[i].Substring(slashIndex + 1);
        }

        scenes[i] = scenes[i].Remove(scenes[i].LastIndexOf(".unity"));
      }

      return scenes;
    }
    else return scenes;

  }// getAllBuildScenesNames()

#endif

}
