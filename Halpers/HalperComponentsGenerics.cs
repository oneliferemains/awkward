using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperComponentsGenerics
{

  static public T[] getAllComponentsInHierarchy<T>(Transform context) where T : Component
  {
    List<T> all = new List<T>();
    all.AddRange(context.GetComponentsInChildren<T>());
    all.AddRange(context.GetComponentsInParent<T>());
    return all.ToArray();
  }

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


  static public T[] fetchComponentsInChildren<T>(Transform origin)
  {
    List<T> list = new List<T>();
    foreach (Transform _t in origin)
    {
      list.AddRange(fetchComponents<T>(_t));
    }
    return list.ToArray();
  }

  static public T[] fetchComponents<T>(Transform origin)
  {
    List<T> list = new List<T>();

    T script = origin.GetComponent<T>();
    if (script != null && script.ToString() != "null") list.Add(script);

    //Debug.Log(origin+","+origin.childCount);

    foreach (Transform child in origin)
    {
      T[] children = fetchComponents<T>(child);
      list.AddRange(children);
      //Debug.Log(child.name+" found "+list.Count, child);
    }

    return list.ToArray();
  }

  static public Transform fetchInChildren(Transform parent, string partName, bool strict = false, bool toLowercase = false)
  {
    foreach (Transform t in parent)
    {
      string nm = t.name;
      if (toLowercase) nm = nm.ToLower();

      if (strict)
      {
        if (nm == partName) return t;
      }
      else
      {
        if (nm.IndexOf(partName) > -1) return t;
      }

      Transform child = fetchInChildren(t, partName, strict, toLowercase);
      if (child != null) return child;
    }
    return null;
  }

  static public bool isInChildren(Transform parent, Transform target)
  {
    bool isIn = false;
    if (parent == target) isIn = true;

    if (!isIn)
    {
      foreach (Transform child in parent)
      {
        if (isIn) continue;

        if (child == target) isIn = true;
        if (child.childCount > 0) isIn = isInChildren(child, target);
      }
    }

    return isIn;
  }

  static public MeshRenderer fetchMeshRendererInParent(Transform tr)
  {
    MeshRenderer _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<MeshRenderer>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

  static public SpriteRenderer fetchSpriteRendererInParent(Transform tr)
  {
    SpriteRenderer _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<SpriteRenderer>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

  static public Collider fetchCapsuleColliderInParent(Transform tr)
  {
    CapsuleCollider _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<CapsuleCollider>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

  static public Collider fetchColliderInParent(Transform tr)
  {
    Collider _tmp = null;
    while (_tmp == null && tr != null)
    {
      _tmp = tr.GetComponent<Collider>();
      if (_tmp == null) tr = tr.parent;
    }
    return _tmp;
  }

  static public Transform[] getAllTransform(GameObject[] list)
  {
    List<Transform> tmp = new List<Transform>();
    for (int i = 0; i < list.Length; i++)
    {
      tmp.AddRange(getAllTransform(list[i].transform));
    }
    return tmp.ToArray();
  }
  static public Transform[] getAllTransform(Transform t)
  {
    List<Transform> trs = new List<Transform>();
    trs.Add(t);
    foreach (Transform child in t)
    {
      if (child.childCount > 0)
      {
        Transform[] children = getAllTransform(child);
        //for (int i = 0; i < children.Length; i++) Debug.Log(child.name+" >> "+children[i].name);
        trs.AddRange(children);
      }
      else
      {
        trs.Add(child);
      }
    }
    return trs.ToArray();
  }

  /// <summary>
  /// (root)/parent/parent/object
  /// </summary>
  static public string getHierarchyFullPath(Transform obj)
  {
    string path = obj.name;

    Transform parent = obj.parent;
    while (parent != null)
    {
      path = parent.name + "/" + path;
      parent = parent.parent;
    }

    return path;
  }


}
