using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// USed to declare specific additionnal scene to load on startup
/// </summary>

public class EngineLoaderFeederBoot : MonoBehaviour
{
  
  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    new GameObject("~loader-boot").AddComponent<EngineLoaderFeederBoot>();
  }

  private IEnumerator Start()
  {
    //need to wait frame 2 before load scenes
    yield return null;
    yield return null;
    yield return null;

    EngineLoaderFeederBase[] feeders = GameObject.FindObjectsOfType<EngineLoaderFeederBase>();
    if (feeders.Length > 0)
    {
      Debug.Log("EngineLoaderFeederBoot found " + feeders.Length + " feeders");

      for (int i = 0; i < feeders.Length; i++)
      {
        feeders[i].feed();
      }
    }

    GameObject.Destroy(gameObject);
  }
}
