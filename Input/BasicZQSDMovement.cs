using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZQSDMovement : MonoBehaviour
{
  public bool physic = false; // collision

  Rigidbody _rigid;

  public float speed = 10f;

  [SerializeField][ReadOnly]
  Vector3 dir = Vector3.zero;

  private void Start()
  {
    if (physic)
    {
      _rigid = GetComponent<Rigidbody>();
    }

    setup();
  }

  virtual protected void setup()
  { }

  private void FixedUpdate()
  {
    if (_rigid == null) return;

    Vector3 vel = _rigid.velocity;

    if (dir.sqrMagnitude > 0f)
    {
      Vector3 worldDirection = transform.TransformPoint(dir);
      worldDirection = (worldDirection - _rigid.position).normalized;

      vel.x = worldDirection.x * speed;
      vel.z = worldDirection.z * speed;

      //vel = (transform.TransformPoint(dir) - transform.position).normalized * speed;
    }

    _rigid.velocity = vel;
  }

  void Update()
  {
    dir = getMotion();

    //no physic
    if (_rigid == null) transform.Translate(dir * speed * Time.deltaTime, Space.Self);
  }

  virtual protected Vector3 getMotion()
  {
    if (Input.GetKey(KeyCode.Z)) dir.z = 1f;
    else if (Input.GetKey(KeyCode.S)) dir.z = -1f;
    else dir.z = 0f;

    if (Input.GetKey(KeyCode.Q)) dir.x = -1f;
    else if (Input.GetKey(KeyCode.D)) dir.x = 1f;
    else dir.x = 0f;

    return dir;
  }
}
