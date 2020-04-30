using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZQSDMovement : MonoBehaviour
{
  public bool physic = false; // collision

  public bool jump = false;
  public float jumpForce = 10f;

  Rigidbody _rigid;

  public float speed = 10f;

  [SerializeField][ReadOnly]
  Vector3 dir = Vector3.zero;

  bool _pressJump = false;
  bool _primeJump = false;
  bool _grounded = false;

  public LayerMask groundCheckLayer;

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

    float friction = 50f;
    vel.x = Mathf.MoveTowards(vel.x, 0f, Time.deltaTime * friction);
    vel.z = Mathf.MoveTowards(vel.z, 0f, Time.deltaTime * friction);

    if (dir.sqrMagnitude > 0f)
    {
      Vector3 worldDirection = transform.TransformPoint(dir);
      worldDirection = (worldDirection - _rigid.position).normalized;

      vel.x = worldDirection.x * speed;
      vel.z = worldDirection.z * speed;

      //vel = (transform.TransformPoint(dir) - transform.position).normalized * speed;
    }
    
    if(_primeJump)
    {
      //Debug.Log("jump");
      vel.y = jumpForce;
      _primeJump = false; // consume prime

      //Debug.Log(Time.frameCount+" <b>jump</b>");
    }
    else if(_grounded)
    {
      //to avoid slope launch
      if(vel.y > 0f) vel.y = 0f;
      //Debug.Log(Time.frameCount + " kill vertic");
    }

    _rigid.velocity = vel;
  }

  void Update()
  {
    dir = getMotion();

    _grounded = checkGrounded();

    //Debug.Log(dir);
    //Debug.Log(_grounded+" / "+ _primeJump + " / "+dir.y);

    if (_grounded)
    {
      if (dir.y > 0f && !_pressJump)
      {
        _primeJump = true;
        _pressJump = true;
        //Debug.Log(Time.frameCount+" prime");
      }
      else if(dir.y <= 0f && _pressJump)
      {
        _pressJump = false;
      }

      //attendre la relache de la pression de la touche
      //if (dir.y <= 0f) _primeJump = false;
    }

    //else if (dir.y <= 0 && _primJump) _primJump = false;

    //no physic
    if (_rigid == null) transform.Translate(dir * speed * Time.deltaTime, Space.Self);
  }
  
  bool checkGrounded()
  {
    RaycastHit _hit;

    //~ taille de la capsule
    float sphRadius = 0.2f;

    //Physics.BoxCast()
    //Physics.SphereCast()

    //check if something right under


    //if(Physics.SphereCast(transform.position + Vector3.up * 0.01f, sphRadius, Vector3.down, out _hit, 100f, groundCheckLayer))

    Vector3 closest = Vector3.zero;

    if(pointOverlap(transform.position, 0.1f, 0, ref closest))
    {
      float dst = Vector3.Distance(transform.position, closest);
      //Debug.Log(dst);

      bool grnd = dst < sphRadius * 0.55f;

      Debug.DrawLine(transform.position, closest, grnd ? Color.red : Color.green);
      
      if (grnd) return true;
    }

    //mid-air
    if (_rigid.velocity.y > 0f) return false;

    return false;
  }

  bool pointOverlap(Vector3 origin, float sphereSize, int stepCount, ref Vector3 output)
  {
    //float size = 1f;

    RaycastHit _hit;
    Vector3 offset = Vector3.up;
    offset *= ((sphereSize * 0.5f) + 0.01f);
    if (Physics.SphereCast(origin + offset, sphereSize, Vector3.down, out _hit, 0.05f, groundCheckLayer))
    {
      //Debug.Log(_hit.point);
      output = _hit.point;
      return true;
    }

    return false;
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
