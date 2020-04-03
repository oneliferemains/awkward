using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMouseOrbiter : MonoBehaviour
{
  public float deltaScale = 0.66f;

  Vector2 _dlt;

  private void Start()
  {
    _dlt = Input.mousePosition;
  }

  void Update()
  {
    Vector2 mPos = Input.mousePosition;
    Vector2 dlt = mPos - _dlt;

    //Debug.Log(dlt);

    Vector2 solvedDelta = dlt * deltaScale;

    if(solvedDelta.sqrMagnitude > 0.01f)
    {
      solvedDelta *= Time.deltaTime;

      transform.Rotate(Vector2.up * solvedDelta.x, Space.World);
      transform.Rotate(Vector2.right * -solvedDelta.y, Space.Self);
    }

    _dlt = mPos; // buff
  }
}
