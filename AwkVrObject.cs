using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwkVrObject : AwkObject
{
  
  protected OVRManager ovrManager;
  protected Camera ovrCamera;

  protected LineRigController rig;
  protected Transform rigTr;

  protected override void setupEarly()
  {
    base.setupEarly();

    ovrManager = GameObject.FindObjectOfType<OVRManager>();
    //Debug.Assert(ovrManager);
    //Debug.Log(ovrManager);
    if(ovrManager != null)
    {
      ovrCamera = ovrManager.GetComponentInChildren<Camera>();
      if (ovrCamera != null) rigTr = ovrCamera.transform;
    }

    Debug.Assert(rigTr);
    
    rig = GameObject.FindObjectOfType<LineRigController>();

    //Debug.Log(name + " " + rig + " " + rigTr);
  }

  protected override void onEnabled()
  {
    //base.onEnabled();

    enabled = false;

#if UNITY_EDITOR
    setupVr();
#else
    StartCoroutine(processWaitForVr());
#endif

    //enabled = false;
  }

  IEnumerator processWaitForVr()
  {
    while (ovrManager == null)
    {
      ovrManager = GameObject.FindObjectOfType<OVRManager>();
      yield return null;
    }

    ovrCamera = ovrManager.GetComponentInChildren<Camera>();
    Debug.Assert(ovrCamera != null);

    while (ovrCamera.transform.position.sqrMagnitude == 0f) yield return null;

    rigTr = ovrCamera.transform;

    setupVr();

    enabled = true; // jic
  }

  virtual protected void setupVr()
  {
    enabled = true;
  }
}
