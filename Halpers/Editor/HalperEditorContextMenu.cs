using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

static public class HalperEditorContextMenu {
  
  [MenuItem("Tools/Clear console #&c")]
  public static void ClearConsole()
  {
    var assembly = Assembly.GetAssembly(typeof(SceneView));
    var type = assembly.GetType("UnityEditor.LogEntries");
    var method = type.GetMethod("Clear");
    method.Invoke(new object(), null);
  }
  [MenuItem("Tools/Clear PlayerPrefs")]
  public static void ClearPlayerPrefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();
  }

}
