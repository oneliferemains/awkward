using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

/// <summary>
/// % = ctrl
/// # = shit
/// & = alt
/// </summary>

public class HalperEditor {

  [MenuItem("Tools/Clear pprefs")]
  public static void ClearPprefs()
  {
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();
  }

#if UNITY_EDITOR

  [MenuItem("Tools/Clear console #&c")]
  public static void ClearConsole()
  {
    var assembly = Assembly.GetAssembly(typeof(SceneView));
    var type = assembly.GetType("UnityEditor.LogEntries");
    var method = type.GetMethod("Clear");
    method.Invoke(new object(), null);
  }

  [MenuItem("Tools/pause %#&w")]
  public static void PauseEditor()
  {
    Debug.Log("PAUSE EDITOR");
    Debug.Break();
    //UnityEditor.EditorApplication.isPlaying = false;

  }

  static public T editor_draw_selectObject<T>(T instance = null, string overrideSelectLabel = "") where T : Component
  {
    if (instance == null)
    {
      instance = GameObject.FindObjectOfType<T>();
    }

    if (instance == null)
    {
      GUILayout.Label("can't find " + typeof(T) + " in scene");
      return null;
    }

    EditorGUILayout.ObjectField(typeof(T).ToString(), instance, typeof(T), true);
    if (GUILayout.Button(overrideSelectLabel + " " + typeof(T)))
    {
      Selection.activeGameObject = instance.gameObject;
    }

    return instance;
  }
  
  static public void editorCenterCameraToObject(GameObject obj)
  {
    GameObject tmp = Selection.activeGameObject;

    Selection.activeGameObject = obj;

    if (SceneView.lastActiveSceneView != null)
    {
      SceneView.lastActiveSceneView.FrameSelected();
    }

    if (tmp != null) Selection.activeGameObject = tmp;
  }


  /// <summary>
  /// get the sorting layer names<para/>
  /// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
  /// </summary>
  /// <returns>The Sorting Layers (as string[])</returns>
  static public string[] getSortingLayerNames()
  {
    System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

    PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);

    return (string[])sortingLayersProperty.GetValue(null, new object[0]);

  }// getSortingLayerNames()

  /// <summary>
  /// get the unique sorting layer IDs<para/>
  /// from : http://answers.unity3d.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
  /// </summary>
  /// <returns>The sorting layer unique IDs (as int[])</returns>
  static public int[] getSortingLayerUniqueIDs()
  {
    System.Type internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);

    PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);

    return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);

  }// getSortingLayerUniqueIDs()



  /// <summary>
  /// Retourne l'ID local d'un UnityEngine.Object dans une scène<para/>
  /// Viens de https://forum.unity3d.com/threads/how-to-get-the-local-identifier-in-file-for-scene-objects.265686/
  /// </summary>
  /// <param name="obj">L'objet cible.</param>
  /// <returns>L'ID de l'objet. Retourne 0 ou -1 si pas sauvegardé.</returns>
  static public int getLocalIdInFile(Object obj)
  {
    if (obj == null) return -1;

    PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

    SerializedObject serializeObject = new SerializedObject(obj);

    inspectorModeInfo.SetValue(serializeObject, InspectorMode.Debug, null);

    SerializedProperty propertyLocalID = serializeObject.FindProperty("m_LocalIdentfierInFile");

    int localID = propertyLocalID.intValue;

    inspectorModeInfo.SetValue(serializeObject, InspectorMode.Normal, null);

    return localID;

  }// getLocalIdInFile()

#endif

}
