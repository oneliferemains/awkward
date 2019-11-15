using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour
{
  private void Start()
  {
    //enabled = false;
  }

  /// <summary>
  /// must be called by engine manager
  /// </summary>
  virtual public void reboot()
  {
    enabled = false;
    StopAllCoroutines();
    StartCoroutine(processReboot());
  }

  IEnumerator processReboot()
  {
    //Debug.Log(name + " processing reboot");

    enabled = false;

    yield return null;

    setupEarly();

    yield return null;

    setup();

    yield return null;

    setupLate();

    yield return null;
    
    enabled = true;

    //Debug.Log(name + " done rebooting (" + enabled + ")");
  }
  
  virtual public void setupEarly()
  { }

  virtual public void setup()
  { }
  
  virtual public void setupLate()
  { }
}
