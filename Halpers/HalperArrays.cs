using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperArrays 
{

  /// <summary>
  /// shuffle list of Object
  /// </summary>
  static public List<T> shuffle<T>(this List<T> list)
  {
    for (int i = 0; i < list.Count; i++)
    {
      T temp = list[i];
      int randomIndex = Random.Range(i, list.Count);
      list[i] = list[randomIndex];
      list[randomIndex] = temp;
    }
    return list;
  }

}
