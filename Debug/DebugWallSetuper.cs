using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugWallSetuper : MonoBehaviour
{

  private void Awake()
  {
    create();
  }

  static public void create()
  {
    DebugWall tmp = GameObject.FindObjectOfType<DebugWall>();
    if (tmp == null)
    {
      GameObject.Instantiate(Resources.Load("debug-wall"));
    }
  }

}
