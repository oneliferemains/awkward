using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse2D : MonoBehaviour
{
  public float zPosition = 0;
  public Camera cam = null;
  
  void Update()
  {
    align();
  }
  
  void align()
  {
    if (cam == null) cam = Camera.main;

    Vector3 mouseScreenPos = Input.mousePosition;
    mouseScreenPos.z = -cam.transform.position.z + zPosition;
    mouseScreenPos = cam.ScreenToWorldPoint(mouseScreenPos);
    transform.position = mouseScreenPos;
  }

  [ContextMenu("simulate align center")]
  void alignCenter()
  {
    if (cam == null) cam = Camera.main;
    transform.position = cam.ScreenToWorldPoint(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    Debug.Log(transform.position);
  }
}
