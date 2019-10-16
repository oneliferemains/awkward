using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// keep a transform inside cam frustum
// uses the MainCamera if none provided
public class BoundToCamFrustum : MonoBehaviour
{
    public Camera cam;
    
    void Awake()
    {
      if(cam == null)
      {
        cam = Camera.main;
      }
    }
    
    void LateUpdate()
    {
      Vector3 viewportPos = cam.WorldToViewportPoint(transform.position); 
      viewportPos.x = Mathf.Clamp01(viewportPos.x);
      viewportPos.y = Mathf.Clamp01(viewportPos.y);
      transform.position = cam.ViewportToWorldPoint(viewportPos);
    }
}
