using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperComponentsGenerics
{

  /// <summary>
  /// won't include itself in the search
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="context"></param>
  /// <returns></returns>
  static public T[] getComponentsInChildren<T>(Transform context) where T : Component
  {
    return getComponents<T>(context, false);
  }

  static public T[] getComponents<T>(Transform context, bool includeItself = true) where T : Component
  {
    List<T> all = new List<T>();
    
    T[] comps = null;
    if (includeItself) comps = context.GetComponents<T>();
    if (comps != null) all.AddRange(comps);

    foreach(Transform child in context)
    {
      comps = child.GetComponents<T>();
      if (comps != null) all.AddRange(comps);

      if(child.childCount > 0)
      {
        all.AddRange(getComponents<T>(child, false));
      }
    }
    return all.ToArray();
  }

  static public T[] getObjectOfType<T>()
  {
    GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
    List<T> output = new List<T>();
    //T comp = null;
    for (int i = 0; i < objs.Length; i++)
    {
      //comp = objs[i].GetComponent<T>();
      //if (comp != null) output.Add(comp);

      output.AddRange(objs[i].GetComponentsInChildren<T>());
    }
    return output.ToArray();
  }

  static public T[] findComponents<T>() where T : Component
  {
    return GameObject.FindObjectsOfType<T>();
  }

  static public T findComponent<T>() where T : Component
  {
    return GameObject.FindObjectOfType<T>();
  }

  /// <summary>
  /// on itself or in any children
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="comp"></param>
  /// <returns></returns>
  static public T getComponent<T>(Component comp) where T : Component
  {
    T t = comp.GetComponent<T>();
    if (t == null) t = comp.GetComponentInChildren<T>();
    return t;
  }

  static public T getComponent<T>(GameObject elmt) where T : Component
  {
    T t = elmt.GetComponent<T>();
    if (t == null) t = elmt.GetComponentInChildren<T>();
    return t;
  }

  static public T getComponent<T>(string carryName) where T : Component
  {
    GameObject obj = GameObject.Find(carryName);
    if (obj == null) return null;

    T t = obj.GetComponent<T>();
    if (t == null) t = obj.GetComponentInChildren<T>();
    return null;
  }

  public static T getComponentContext<T>(Transform tr, string endName) where T : Component
  {
    tr = HalperTransform.findChild(tr, endName);
    if (tr == null) return null;
    return tr.GetComponent<T>();
  }

  public static T getComponentByCarryName<T>(string carryName) where T : Component
  {
    GameObject obj = GameObject.Find(carryName);

    if (obj == null)
    {
      //Debug.LogWarning("couldn't find " + carryName);
      return null;
    }

    return getComponent<T>(obj);
  }


  static public T getManager<T>(string nm) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    return tmp;
  }




  static public T getManager<T>(string nm, bool dontDestroy = false) where T : MonoBehaviour
  {
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null)
    {
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    if (dontDestroy) GameObject.DontDestroyOnLoad(tmp);

    return tmp;
  }

}
