﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMouseOrbiter : MonoBehaviour
{
  public float sensivityX = 5f;
  public float sensivityY = 3f;

  float angleX = 0f;
  float angleY = 0f;

  protected Vector2 _dlt;

  private void Start()
  {
#if ENABLE_LEGACY_INPUT_MANAGER
    _dlt = Input.mousePosition;
#endif

    //_dlt = Mouse.current.position;

    angleX = transform.eulerAngles.y;
    angleY = transform.localEulerAngles.x;

    lockMouse();
  }

  void lockMouse()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  virtual protected Vector2 solveDelta()
  {
    Vector2 output = (Vector2)Input.mousePosition - _dlt;
    return output;
  }

  void manageCursorLocking()
  {
#if ENABLE_LEGACY_INPUT_MANAGER
    if (Input.GetMouseButtonDown(0))
    {
      lockMouse();
    }

    if(Application.isEditor)
    {
      if (Input.GetKeyUp(KeyCode.Escape))
      {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
      }
    }
    
#endif
  }

  void Update()
  {
    manageCursorLocking();

    Vector2 dlt = solveDelta();

    //Debug.Log(dlt);

    if (dlt.sqrMagnitude == 0f) return;

    //Debug.Log(dlt);

    Vector2 solvedDelta;
    
    solvedDelta.x = dlt.x * sensivityX;
    solvedDelta.y = dlt.y * sensivityY;

    if (solvedDelta.sqrMagnitude > 0.01f)
    {
      solvedDelta *= Time.deltaTime;

      Vector3 euler = Vector3.zero;
      
      if(solvedDelta.x != 0f)
      {
        angleX += solvedDelta.x; // up

        euler = transform.eulerAngles;
        euler.y = angleX;
        transform.eulerAngles = euler;
      }

      if(solvedDelta.y != 0f)
      {
        angleY += solvedDelta.y; // right

        euler = transform.localEulerAngles;
        euler.x = -angleY;

        euler.x = Mathf.Clamp(euler.x, -75f, 75f);

        transform.localEulerAngles = euler;
      }

      //transform.Rotate(Vector2.up * solvedDelta.x, Space.World);
      //transform.Rotate(Vector2.right * -solvedDelta.y, Space.Self);
    }

    _dlt = dlt; // buff for next frame
  }

}
