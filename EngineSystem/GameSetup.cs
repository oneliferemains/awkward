using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// something else must call feeds()
/// </summary>

abstract public class GameSetup : MonoBehaviour
{
  bool _ready = false;

  private IEnumerator Start()
  {
    _ready = false;

    yield return null;
    
    _ready = true;

    Debug.Log("GameSetup is done");

    yield return null; // setup
    yield return null; // setup late

    yield return null; // jic
    
    reboot();
  }
  
  private void Update()
  {
    if (Input.GetKeyUp(KeyCode.Backspace)) Application.Quit();
    if (Input.GetKeyUp(KeyCode.RightShift)) reboot();
    if (Input.GetKeyUp(KeyCode.N)) ScreenshotManager.call();
  }

  public void reboot()
  {
    Debug.Log(GetType()+" <b>REBOOT</b>");

    AwkObject[] bos = GameObject.FindObjectsOfType<AwkObject>();
    foreach(AwkObject bo in bos)
    {
      bo.reboot();
    }
  }

  public bool isReady()
  {
    return _ready;
  }
}
