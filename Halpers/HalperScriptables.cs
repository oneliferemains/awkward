using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
[CreateAssetMenu(menuName = "protoss/create DataClass", order = 100)]
public class DataClass : ScriptableObject
{
}
*/

static public class HalperScriptables {

#if UNITY_EDITOR
  static public T getScriptableObjectInEditor<T>(string nameEnd = "") where T : ScriptableObject
  {
    string[] all = AssetDatabase.FindAssets("t:"+typeof(T).Name);
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(T));
      T data = obj as T;

      if (data == null) continue;
      if(nameEnd.Length > 0)
      {
        if (!data.name.EndsWith(nameEnd)) continue;
      }

      return data;
    }
    Debug.LogWarning("can't locate scriptable of type " + typeof(T).Name + " (filter name ? " + nameEnd + ")");
    return null;
  }
#endif

}
