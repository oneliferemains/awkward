using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMouseOrbiter : MonoBehaviour
{
  float deltaScaleX = 5f;
  float deltaScaleY = 3f;

  Vector2 _dlt;

  float angleX = 0f;
  float angleY = 0f;

  private void Start()
  {

    _dlt = Input.mousePosition;

    lockMouse();
  }

  void lockMouse()
  {

    Cursor.lockState = CursorLockMode.Confined;
    Cursor.visible = false;
  }

  void Update()
  {

    if(Input.GetMouseButtonDown(0))
    {
      lockMouse();
    }

    if(Input.GetKeyUp(KeyCode.Escape))
    {
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
    }

    Vector2 mPos = Input.mousePosition;
    Vector2 dlt = mPos - _dlt;

    //Debug.Log(dlt);

    Vector2 solvedDelta;
    
    solvedDelta.x = dlt.x * deltaScaleX;
    solvedDelta.y = dlt.y * deltaScaleY;

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

    _dlt = mPos; // buff
  }
}
