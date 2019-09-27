using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
  bool _ready = false;

  private IEnumerator Start()
  {
    _ready = false;

    //insert more stuff here
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
    Debug.Log("<b>REBOOT</b>");

    BaseObject[] bos = GameObject.FindObjectsOfType<BaseObject>();
    foreach(BaseObject bo in bos)
    {
      bo.reboot();
    }
  }

  public bool isReady()
  {
    return _ready;
  }
}
