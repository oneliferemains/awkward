using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// created by default and hidden
/// </summary>
public class DebugWall : AwkVrObject
{
  static public DebugWall instance;
  
  public TextMesh txtLogs;
  public TextMesh txtDynamics;
  public TextMesh txtInputs;

  int lastFrameInputs = 0;
  int lastFrameDynamics = 0;

  Vector2 rsBuffer;

  protected override void build()
  {
    base.build();

    instance = this;

    hide();
  }

  protected override void setup()
  {
    base.setup();

    addLog(VersionManager.getFormatedVersion());

    if(rig != null)
    {
      rig.onRightStick += onRStick;
    }

    //rig.getButton()
  }

  void onRStick(Vector2 motion)
  {

    Debug.Log(motion+" vs "+rsBuffer);

    if (DebugWall.instance != null)
    {
      if (motion.y > 0.5f && rsBuffer.y < 0.5f) DebugWall.instance.toggle();
    }

    rsBuffer = motion;
  }

  public void addLog(string log)
  {
    txtLogs.text += "\n"+Time.frameCount+"|"+log;
    //Debug.Log(log);
  }

  public void addDyna(string ct)
  {
    //clear last frame
    if(lastFrameDynamics != Time.frameCount)
    {
      txtDynamics.text = "";
      lastFrameDynamics = Time.frameCount;
    }

    txtDynamics.text += "\n"+ct;
  }

  public void addInputs(string ct)
  {
    if (lastFrameInputs != Time.frameCount)
    {
      txtInputs.text = "";
      lastFrameInputs = Time.frameCount;
    }

    txtInputs.text += "\n" + ct;
  }
  

  void show()
  {
    txtLogs.gameObject.SetActive(true);
    txtDynamics.gameObject.SetActive(true);
    txtInputs.gameObject.SetActive(true);
  }

  void hide()
  {
    txtLogs.gameObject.SetActive(false);
    txtDynamics.gameObject.SetActive(false);
    txtInputs.gameObject.SetActive(false);
  }

  public void toggle()
  {
    Debug.Log("debug, toggling wall debug");
    if (txtLogs.gameObject.activeSelf) hide();
    else show();
  }
}
