using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse2D : MonoBehaviour
{
  public float zPosition = 0;
  public Camera cam = null;

  void Awake()
  {
    if(cam == null)
    {
      cam = Camera.main;
    }
  }

  void Update()
  {
    Vector3 mouseScreenPos = Input.mousePosition;
    mouseScreenPos.z = -cam.transform.position.z;
    transform.position = cam.ScreenToWorldPoint(mouseScreenPos);
  }
}
