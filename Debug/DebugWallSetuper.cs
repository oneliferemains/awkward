using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// make sure there is a debug wall
/// </summary>

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
