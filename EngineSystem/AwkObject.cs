using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AwkObject : MonoBehaviour
{

  protected string log = "";
  protected bool logs = false; // display logs
  protected bool debugIsSceneOwnerActiveScene = false; // au moment où il est généré
  
  IEnumerator Start()
  {
    enabled = false;

    setupEarly();

    yield return null;

    setup();

    yield return null;

    setupLate();

    enabled = true;
  }

  virtual public void setupEarly()
  { }

  virtual public void setup()
  { }

  virtual public void setupLate()
  { }

  virtual public void reboot()
  { }

  private void Update()
  {
    update();
  }

  virtual protected void update()
  { }
  
  virtual protected string toString()
  {
    log = "[" + GetType() + "] " + name;
    return log;
  }

  public bool isReady()
  {
    return enabled;
  }

  virtual protected string getStamp()
  {
    return getStamp(this);
  }

  static public string getStamp(MonoBehaviour obj, string color = "gray")
  {
    return "<color=" + color + ">" + obj.GetType() + "</color> | <b>" + obj.name + "</b> | ";
  }

  protected void editorFocus(bool parentIsActiveSceneCheck = false)
  {
#if UNITY_EDITOR
    if (parentIsActiveSceneCheck && !debugIsSceneOwnerActiveScene) return;
    editorFocusGameObject(gameObject);
#endif
  }

  static public void editorFocusGameObject(GameObject obj)
  {
#if UNITY_EDITOR
    UnityEditor.Selection.activeGameObject = obj;
#endif
  }

  static public void editorFocusGameObject(GameObject obj, bool parentIsActiveSceneCheck)
  {
#if UNITY_EDITOR
    if (parentIsActiveSceneCheck && !HalperScene.isActiveScene(obj.gameObject.scene)) return;
    editorFocusGameObject(obj);
#endif
  }

  public bool editorIsSelected()
  {
#if UNITY_EDITOR
    if (UnityEditor.Selection.activeGameObject == gameObject) return true;
#endif
    return false;
  }




}
