using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// Un loader indépendant a qui on peut demander de load une scene spécifique
/// Quand la scene sera load il va process les feeders lié a chaques scenes demandés
/// Puis se détruire
/// </summary>

public class EngineLoader : MonoBehaviour
{
  static public List<EngineLoader> loaders = new List<EngineLoader>();

  protected List<Coroutine> queries = new List<Coroutine>();
  
  public const string prefixResource = "resource-";

  private void Awake()
  {
    //Debug.Log(EngineObject.getStamp(this) + " created");
    loaders.Add(this);
  }

  private void OnDestroy()
  {
    //Debug.Log(EngineObject.getStamp(this) + " destroyed");
    loaders.Remove(this);
  }

  static protected EngineLoader createLoader()
  {
    return new GameObject("[loader("+Random.Range(0,1000)+")]").AddComponent<EngineLoader>();
  }

  static protected bool checkForFilteredScenes()
  {
    string[] filter = { "ui", "screen", "resource", "level" };
    for (int i = 0; i < filter.Length; i++)
    {
      if (doActiveSceneNameContains(filter[i]))
      {
        //SceneManager.LoadScene("game");
        Debug.LogWarning("<color=red><b>" + filter[i] + " SCENE ?!</b></color> can't load that");
        return false;
      }
    }
    return true;
  }
  
  static public bool hasAnyScenesInBuildSettings()
  {

    if (SceneManager.sceneCountInBuildSettings <= 1)
    {
      Debug.LogWarning("could not launch loading because <b>build settings scenes list count <= 1</b>");
      return false;
    }

    return true;
  }
  
  public Coroutine solveFeeders(Scene scene, Action onComplete = null)
  {
    return StartCoroutine(processFeeders(scene, onComplete));
  }
  
  IEnumerator processFeeders(Scene scene, Action onFeedersCompleted = null)
  {
    ///// feeder, additionnal scenes (from feeder script)
    GameObject[] roots = scene.GetRootGameObjects();
    List<EngineLoaderFeederBase> feeders = new List<EngineLoaderFeederBase>();
    for (int i = 0; i < roots.Length; i++)
    {
      feeders.AddRange(roots[i].GetComponentsInChildren<EngineLoaderFeederBase>());
    }

    for (int i = 0; i < feeders.Count; i++)
    {
      feeders[i].feed();
    }

    bool done = false;
    do
    {
      done = true;
      for (int i = 0; i < feeders.Count; i++)
      {
        if (feeders[i] != null) done = false;
      }
      yield return null;
    } while (!done);

    yield return null;

    if (onFeedersCompleted != null) onFeedersCompleted();
  }
  
  public Coroutine asyncUnloadScenes(string[] sceneNames, Action onComplete = null)
  {
    return StartCoroutine(processUnload(sceneNames, onComplete));
  }

  IEnumerator processUnload(string[] sceneNames, Action onComplete = null)
  {
    List<AsyncOperation> asyncs = new List<AsyncOperation>();

    for (int i = 0; i < sceneNames.Length; i++)
    {
      asyncs.Add(SceneManager.UnloadSceneAsync(sceneNames[i]));
    }

    while(asyncs.Count > 0)
    {
      int c = 0;
      while(c < asyncs.Count)
      {
        if (asyncs[c].isDone) asyncs.RemoveAt(c);
        else c++;
      }

      yield return null;
    }

    if(onComplete != null) onComplete();

    GameObject.Destroy(gameObject);
  }

  public Coroutine asyncLoadScenes(string[] sceneNames, Action onComplete = null)
  {
    return StartCoroutine(processLoadScenes(sceneNames, onComplete));
  }
  
  IEnumerator processLoadScenes(string[] sceneNames, Action onComplete = null)
  {
    //Debug.Log(getStamp() + " ... processing " + sceneNames.Length + " scenes", transform);

    for (int i = 0; i < sceneNames.Length; i++)
    {
      string sceneName = sceneNames[i];

      //do not load the current active scene
      if (doActiveSceneNameContains(sceneName))
      {
        Debug.LogWarning(sceneName+" is current active scene, need to load it ?");
        continue;
      }

      //don't double load same scene
      bool alreadyLoaded = getLoadedScene(sceneName).isLoaded;

      Debug.Log(getStamp() + Time.frameCount + "  is " + sceneName + " already loaded ? "+ alreadyLoaded);
      
      if (alreadyLoaded)
      {
        Debug.LogWarning("  <b>"+sceneName + "</b> is considered as already loaded, skipping loading of that scene");
        continue;
      }

      //Debug.Log(sceneNames[i]);

      IEnumerator process = processLoadScene(sceneNames[i]);
      while (process.MoveNext()) yield return null;

      //Debug.Log("  ... scene of index " + i + " | " + sceneNames[i] + " | is done loading");
    }
    
    //needed so that all new objects loaded have time to exec build()
    yield return null;

    //Debug.Log(getStamp() + "  ... processing " + sceneNames.Length + " is done");

    if (onComplete != null) onComplete();

    GameObject.Destroy(gameObject);
  }

  IEnumerator processLoadScene(string sceneLoad, Action onComplete = null)
  {
    //can't reload same scene
    //if (isSceneOfName(sceneLoad)) yield break;

    if (!checkIfInBuildSettings(sceneLoad))
    {
      Debug.LogError("asked to load <b>" + sceneLoad + "</b> but this scene is <b>not added to BuildSettings</b>");
      yield break;
    }

#if UNITY_EDITOR
    Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> loading ... ");
#endif

    AsyncOperation async = SceneManager.LoadSceneAsync(sceneLoad, LoadSceneMode.Additive);
    while (!async.isDone)
    {
      yield return null;
      //Debug.Log(sceneLoad + " "+async.progress);
    }

    //Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> async is done ... ");

    Scene sc = SceneManager.GetSceneByName(sceneLoad);
    while (!sc.isLoaded) yield return null;

    //Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> at loaded state ... ");

    cleanScene(sc);

    //Debug.Log(getStamp() + "  L <b>" + sceneLoad + "</b> solving feeders ... ");

    Coroutine feeders = solveFeeders(sc, delegate()
    {
      feeders = null;
    });
    while (feeders != null) yield return null;

    yield return null;

    //ResourceManager.reload(); // add resources if any

    //Debug.Log(getStamp() + " ... '<b>" + sceneLoad + "</b>' loaded");

    if (onComplete != null) onComplete();
  }
  
  protected string getStamp()
  {
    return AwkObject.getStamp(this, "cyan");
  }
  
  static public Coroutine loadScene(string nm, Action onComplete = null)
  {
    return loadScenes(new string[] { nm }, onComplete);
  }
  static public Coroutine loadScenes(string[] nms, Action onComplete = null)
  {
    return createLoader().asyncLoadScenes(nms, onComplete);
  }

  static public void unloadScene(Scene sc)
  {
    unloadScene(sc.name);
  }

  static public void unloadScene(string nm, Action onComplete = null)
  {
    if(nm.Length <= 0)
    {
      onComplete();
      return;
    }

    unloadScenes(new string[] { nm }, onComplete);
  }
  static public void unloadScenes(string[] nms, Action onComplete = null)
  {
    createLoader().asyncUnloadScenes(nms, onComplete);
  }

  static public void unloadScenesInstant(Scene sc) { unloadScenesInstant(new string[] { sc.name }); }
  static public void unloadScenesInstant(string[] nms)
  {
    for (int i = 0; i < nms.Length; i++)
    {
      Scene sc = getLoadedScene(nms[i]);
      if (sc.isLoaded)
      {
        Debug.Log("unloading : " + sc.name);
        SceneManager.UnloadSceneAsync(nms[i]);
      }
    }
  }

  static public Coroutine queryScene(string sceneName, Action onComplete = null)
  {
    return queryScenes(new string[] { sceneName }, onComplete);
  }
  static public Coroutine queryScenes(string[] sceneNames, Action onComplete = null)
  {
    return createLoader().asyncLoadScenes(sceneNames, onComplete);
  }

  static public void unloadSceneByExactName(string sceneName)
  {
    Debug.Log("unloading <b>" + sceneName+"</b>");
    SceneManager.UnloadSceneAsync(sceneName);
  }

  static protected void cleanScene(Scene sc)
  {

    GameObject[] roots = sc.GetRootGameObjects();
    //Debug.Log("  L cleaning scene <b>" + sc.name + "</b> from guides objects (" + roots.Length + " roots)");
    for (int i = 0; i < roots.Length; i++)
    {
      removeGuides(roots[i].transform);
    }
    
  }

  static protected bool removeGuides(Transform obj)
  {
    if(obj.name.StartsWith("~"))
    {
      Debug.Log("   <b>removing guide</b> of name : " + obj.name, obj);
      GameObject.Destroy(obj.gameObject);
      return true;
    }

    int i = 0;
    while(i < obj.childCount)
    {
      if (!removeGuides(obj.GetChild(i))) i++;
    }

    return false;
  }

  static private string getActiveSceneName() {
    return SceneManager.GetActiveScene().name;
  }
  
  static public bool doActiveSceneNameContains(string nm, bool startWith = false) {
    string scName = getActiveSceneName();
    //Debug.Log(scName + " vs " + nm);
    if (startWith) return scName.StartsWith(nm);
    return scName.Contains(nm);
  }

  static public bool isGameScene()
  {
    return getActiveSceneName().StartsWith("game");
    //return doActiveSceneNameContains("game");
  }

  static protected bool isResourceScene()
  {
    return doActiveSceneNameContains("resource-");
  }

  static protected bool isSceneLevel()
  {
    return doActiveSceneNameContains("level-");
  }
  
  static public bool areAnyLoadersRunning()
  {
    return loaders.Count > 0;
  }

  /// <summary>
  /// loaded or loading (but called to be loaded at least)
  /// </summary>
  /// <param name="endName"></param>
  /// <returns></returns>
  static public bool isSceneAdded(string endName)
  {
    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      Scene sc = SceneManager.GetSceneAt(i);
      //Debug.Log(sc.name + " , valid ? " + sc.IsValid() + " , loaded ? " + sc.isLoaded);
      if (sc.name.Contains(endName))
      {
        return true;
      }
    }

    return false;
  }

  static public Scene getLoadedScene(string containName)
  {
    if(Time.frameCount < 2)
    {
      Debug.LogError("scenes are not flagged as loaded until frame 2");
      return default(Scene);
    }

    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      Scene sc = SceneManager.GetSceneAt(i);

      //Debug.Log(sc.name + " , valid ? " + sc.IsValid() + " , loaded ? " + sc.isLoaded);

      if (sc.isLoaded && sc.name.Contains(containName)) return sc;
    }

    //Debug.LogWarning("asking if "+containName + " scene is loaded but its not");
    return default(Scene);
  }

#if UNITY_EDITOR

  static public bool isSceneInBuildSettingsList(string scName)
  {
    bool found = true;

    found = false;

    UnityEditor.EditorBuildSettingsScene[] scenes = UnityEditor.EditorBuildSettings.scenes;
    for (int i = 0; i < scenes.Length; i++)
    {
      //UnityEditor.SceneManagement.EditorSceneManager.GetSceneByBuildIndex()
      if (scenes[i].path.Contains(scName)) found = true;
    }
    
    return found;
  }

#endif

  static public bool checkIfInBuildSettings(string sceneLoad)
  {
    bool checkIfExists = false;

    //Debug.Log("count ? "+ SceneManager.sceneCountInBuildSettings);
    
    for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
    {
      string path = SceneUtility.GetScenePathByBuildIndex(i);
    
      //Debug.Log(path);
      
      if (path.Contains(sceneLoad)) checkIfExists = true;
    }

    return checkIfExists;
  }

}
