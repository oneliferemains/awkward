using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZQSDMovement : MonoBehaviour
{
  public bool flight = true;
  public bool physic = false; // collision

  protected Rigidbody _rigid;

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

  void Update()
  {
    dir = getMotion();

    if(_rigid != null)
    {
      solveRigidUpdate(dir);
    }
    
    //no physic
    if (_rigid == null) transform.Translate(dir * speed * Time.deltaTime, Space.Self);
  }

  private void FixedUpdate()
  {

    Vector3 vel = _rigid.velocity;

    // FRICTION

    float friction = 50f;
    vel.x = Mathf.MoveTowards(vel.x, 0f, Time.fixedDeltaTime * friction);
    vel.z = Mathf.MoveTowards(vel.z, 0f, Time.fixedDeltaTime * friction);
    if (flight) vel.y = Mathf.MoveTowards(vel.y, 0f, Time.fixedDeltaTime * friction);
    
    _rigid.velocity = vel;
  }

  virtual protected void solveRigidUpdate(Vector3 inputDirection)
  {
    if (_rigid == null) return;

    Vector3 vel = _rigid.velocity;

    // INPUTS | override XZ vel based on inputs

    if (inputDirection.sqrMagnitude > 0f)
    {
      Vector3 worldDirection = transform.TransformPoint(inputDirection);
      worldDirection = (worldDirection - _rigid.position).normalized;

      vel.x = worldDirection.x * speed;
      vel.z = worldDirection.z * speed;

      if (flight) vel.y = worldDirection.y * speed;
      //vel = (transform.TransformPoint(dir) - transform.position).normalized * speed;
    }

    _rigid.velocity = vel;
  }

  virtual protected Vector3 getMotion()
  {
    Vector3 tmp = Vector3.zero;

    if (Input.GetKey(KeyCode.Z)) tmp.z = 1f;
    else if (Input.GetKey(KeyCode.S)) tmp.z = -1f;
    else tmp.z = 0f;

    if (Input.GetKey(KeyCode.Q)) tmp.x = -1f;
    else if (Input.GetKey(KeyCode.D)) tmp.x = 1f;
    else tmp.x = 0f;

    return tmp;
  }

}
