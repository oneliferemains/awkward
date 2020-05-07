using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsMovementJumper : BasicZQSDMovement
{
  public bool jump = false;
  public float jumpForce = 10f;
  bool _jumping = false;
  //float jumpTrackingForce = 0f;

  bool _pressJump = false;
  //bool _primeJump = false;
  bool _grounded = false;

  public LayerMask groundCheckLayer;

  protected override void solveRigidUpdate(Vector3 inputDirection)
  {
    //velocity XZ
    base.solveRigidUpdate(inputDirection);

    Vector3 vel = _rigid.velocity;

    if (!flight && jump)
    {
      _grounded = checkGrounded();

      //Debug.Log(_grounded + " / " + _primeJump + " / " + direction.y);

      if (_grounded)
      {
        if(!_jumping)
        {
          //to avoid slope launch
          if (vel.y > 0f) vel.y = 0f;
        }
        else
        {
          if (vel.y <= 0f) _jumping = false;
        }

        if (inputDirection.y > 0f && !_pressJump)
        {
          vel.y = jumpForce;
          _pressJump = true;
          _jumping = true;
          //Debug.Log(Time.frameCount + " prime");
        }
        else if (inputDirection.y <= 0f && _pressJump)
        {
          _pressJump = false;
        }

        //attendre la relache de la pression de la touche
        //if (dir.y <= 0f) _primeJump = false;
      }
    }

    _rigid.velocity = vel;
  }


  bool checkGrounded()
  {
    RaycastHit _hit;

    //~ taille de la capsule
    float sphRadius = 0.2f;

    //if(Physics.SphereCast(transform.position + Vector3.up * 0.01f, sphRadius, Vector3.down, out _hit, 100f, groundCheckLayer))

    Vector3 closest = Vector3.zero;

    if (pointOverlap(transform.position, 0.1f, 0, ref closest))
    {
      float dst = Vector3.Distance(transform.position, closest);
      //Debug.Log(dst);

      bool grnd = dst < sphRadius * 0.55f;

      Debug.DrawLine(transform.position, closest, grnd ? Color.red : Color.green);

      if (grnd) return true;
    }

    //dans une slope on peut pas juste dire qu'on est pas au sol
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

}
