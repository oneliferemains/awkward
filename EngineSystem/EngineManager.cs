using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineManager : MonoBehaviour
{
  AwkObject[] objs;

  bool _loading = false;

  private void Awake()
  {
    _loading = true;
  }

  IEnumerator Start()
  {
    _loading = false;

    yield return null;

    GameSetup gs = GameSetup.FindObjectOfType<GameSetup>();

    if (gs != null)
    {
      while (!gs.isReady()) yield return null;
    }

    _loading = true;

    Debug.Log(AwkObject.getStamp(this)+" loading done");

    StartCoroutine(waitForReboot());
  }

  IEnumerator waitForReboot()
  {
    fetch();

    Debug.Log(AwkObject.getStamp(this) + " waiting before reboot");

    bool allReady = false;
    while(!allReady)
    {
      allReady = true;
      for (int i = 0; i < objs.Length; i++)
      {
        if (!objs[i].isReady()) allReady = false;
      }
      yield return null;
    }
    
    yield return null; // jic

    Debug.Log(AwkObject.getStamp(this) + " done waiting before reboot");
    reboot();
  }

  void fetch()
  {
    objs = GameObject.FindObjectsOfType<AwkObject>();
  }

  public bool isLoading()
  {
    return _loading;
  }

  public void reboot()
  {
    Debug.Log(AwkObject.getStamp(this) + " rebooting");
    
    fetch();
    for (int i = 0; i < objs.Length; i++)
    {
      objs[i].reboot();
    }
  }

  private void Update()
  {
    
    if(Input.GetKeyUp(KeyCode.Delete))
    {
      reboot();
    }

    if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Backspace))
    {
      Application.Quit();
    }
  }
}
