using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperTransform {

  static public List<Transform> getTransformsOfName(Transform parent, string nameStart)
  {
    if (parent == null) Debug.LogError("no tr ?");

    List<Transform> trs = new List<Transform>();
    foreach(Transform tr in parent)
    {
      if(tr.name.StartsWith(nameStart))
      {
        trs.Add(tr);
      }

      if (tr.childCount > 0) trs.AddRange(getTransformsOfName(tr, nameStart));
    }
    return trs;
  }

  static public Transform getTransform<T>() where T : Component
  {
    T t = GameObject.FindObjectOfType<T>();
    if (t != null) return t.transform;
    return null;
  }

  static public Transform getTransform<T>(Component comp) where T : Component
  {
    T t = HalperComponentsGenerics.getComponent<T>(comp);
    if (t != null) return t.transform;
    return null;
  }

  static public string getFullTransformHierarchyPathToString(this Transform tr)
  {
    string path = "";
    while (tr != null)
    {
      path = tr.name + "/" + path;
      tr = tr.parent;
    }
    return path;
  }

  /// <summary>
  /// Permet de virer tout enfants et component (sauf le transform)
  /// </summary>
  public static void cleanTransform(Transform tr)
  {

    //HierarchyAnimatorHighlighter.ShowIcon(!HierarchyAnimatorHighlighter.ShowIcon());
    if (tr == null) return;

    Debug.Log("cleaning " + tr.name);

    //remove all children
    while (tr.childCount > 0)
    {
      GameObject.Destroy(tr.GetChild(0).gameObject);
    }

    SpriteRenderer[] renders = tr.GetComponents<SpriteRenderer>();
    foreach (SpriteRenderer render in renders) { GameObject.Destroy(render); }

    Collider[] colliders = tr.GetComponents<Collider>();
    foreach (Collider collider in colliders) { GameObject.Destroy(collider); }


  }


  /// <summary>
	/// Retourne le nombre de parent du Transform.
	/// </summary>
	/// <param name="transform">Le Transform ciblé.</param>
	/// <returns>Le nombre de Transform parent, 0 si ce Transform n'a pas de parent (le pauvre).</returns>
	static public int getHierarchyDepth(this Transform transform)
  {
    int depth = 0;

    Transform parent = transform.parent;

    while (parent != null)
    {
      depth++;

      parent = parent.parent;
    }

    return depth;

  }// getHierarchyDepth()

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


  /// <summary>
  /// Retourne une chaine de la hierarchie du Transform dans la scène.
  /// </summary>
  /// <param name="transform">Le Transform cible.</param>
  /// <returns>Le chemin d'accès au Transform dans la scène.</returns>
  static public string getHierarchyPathToString(this Transform transform)
  {
    string path = transform.name;

    Transform parent = transform.parent;

    while (parent != null)
    {
      path = parent.name + "/" + path;

      parent = parent.parent;
    }

    return path;
  }


  /// <summary>
  /// Permet de remonter a un parent précis dans la hiérarchie.
  /// </summary>
  /// <param name="transform">Le Transform de départ</param>
  /// <param name="reverseHierarchyStep">Le nombre d'étape pour remontrer la hiérarchie.</param>
  /// <returns>La référence du Transform parent.</returns>
  static public Transform getTransformParent(this Transform transform, int reverseHierarchyStep = -1)
  {
    if (reverseHierarchyStep > 0)
    {
      Debug.LogWarning("reverseHierarchyStep doit toujours être inférieur ou égal à 0 !");

      return null;
    }

    while (reverseHierarchyStep < 0 && transform != null)
    {
      transform = transform.parent;

      reverseHierarchyStep++;
    }

    return transform;
  }

  /// <summary>
  /// returns first child containing param name
  /// </summary>
  /// <param name="warning">Génère-t-on un warning si on ne trouve rien ?</param>
  static public Transform looselyFindParent(this Transform transform, string name, bool strict = false, bool warning = true)
  {
    Transform parent = transform.parent;

    if (!string.IsNullOrEmpty(name))
    {
      while (parent != null)
      {
        if (strict && parent.name == name) return parent;
        else if (parent.name.Contains(name))
        {
          return parent;
        }
        else parent = parent.parent;
      }
    }

    if (warning) Debug.LogWarning("Can't find Transform's parent with name \"" + name + "\"", transform);

    return parent;

  }// looselyFindParent()



  static public Transform[] findSameChildren(Transform transformOrigin, Transform transformFilter)
  {
    List<Transform> sameChildrenOrigin = new List<Transform>();

    foreach (Transform childOrigin in transformOrigin)
    {
      foreach (Transform childFilter in transformFilter)
      {
        if (string.Compare(childOrigin.name, childFilter.name) == 0)
        {
          sameChildrenOrigin.Add(childOrigin);
        }
      }
    }

    return sameChildrenOrigin.ToArray();

  }// findSameChildren()

  static public Transform findChild(Transform parent, string endName)
  {
    if (parent.name.EndsWith(endName)) return parent;

    if (parent.childCount > 0)
    {
      for (int i = 0; i < parent.childCount; i++)
      {
        Transform child = findChild(parent.GetChild(i), endName);
        if (child != null) return child;
      }
    }

    return null;
  }

}
