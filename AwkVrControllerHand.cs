using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwkVrControllerHand : AwkObject
{
  public OVRInput.Controller hand = OVRInput.Controller.LTouch;

  public Transform handAnchor;

  protected override void setup()
  {
    base.setup();
    
    if(hand == OVRInput.Controller.LTouch)
    {
      handAnchor = GameObject.Find("LeftHandAnchor").transform;
    }else if(hand == OVRInput.Controller.RTouch)
    {
      handAnchor = GameObject.Find("RightHandAnchor").transform;
    }
  }

  public bool checkProxy(Vector3 position, float dst)
  {
    return Vector3.Distance(handAnchor.position, position) < dst;
  }

  public Transform getActiveTransform()
  {

#if UNITY_EDITOR
    return transform;
#else
    return handAnchor;
#endif

  }

  private void OnDrawGizmos()
  {

    if (handAnchor != null)
    {
      Gizmos.color = new Color(0f, 0f, 0f, 0.2f);
      Gizmos.DrawSphere(handAnchor.position, 0.1f);
    }

  }
}