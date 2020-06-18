using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class BasicMouseOrbiter : MonoBehaviour
{
  public float sensivityX = 5f;
  public float sensivityY = 3f;

  public Transform horizontalPivot;
  public Transform verticalPivot;

  float angleX = 0f;
  float angleY = 0f;

  protected Vector2 _dlt;

  IEnumerator Start()
  {
    enabled = false;

    //must wait a few frame before tracking mouse mouvement
    //so that camera is straight when starting
    yield return null;
    yield return null;
    yield return null;
    yield return null;
    yield return null;

#if ENABLE_LEGACY_INPUT_MANAGER
    _dlt = Input.mousePosition;
#endif

    //_dlt = Mouse.current.position;

    angleX = transform.eulerAngles.y;
    angleY = transform.localEulerAngles.x;

    MouseLocker.lockMouse();

    setup();

    enabled = true;
  }

  virtual protected void setup()
  { }

  virtual protected Vector2 solveDelta()
  {
    Vector2 output = (Vector2)Input.mousePosition - _dlt;
    return output;
  }

  void Update()
  {
    MouseLocker.updateCursorLocking();

    //no mouvement when not locked
    if (MouseLocker.isUnlocked()) return;

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

        euler = horizontalPivot.localEulerAngles;
        euler.y = angleX;
        horizontalPivot.eulerAngles = euler;
      }

      if(solvedDelta.y != 0f)
      {
        angleY += solvedDelta.y; // right

        euler = verticalPivot.localEulerAngles;
        euler.x = -angleY;

        euler.x = Mathf.Clamp(euler.x, -75f, 75f);

        verticalPivot.localEulerAngles = euler;
      }

      //transform.Rotate(Vector2.up * solvedDelta.x, Space.World);
      //transform.Rotate(Vector2.right * -solvedDelta.y, Space.Self);
    }

    _dlt = dlt; // buff for next frame
  }

}
