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

  [RuntimeInitializeOnLoadMethod]
  static public void create()
  {
    DebugWall tmp = GameObject.FindObjectOfType<DebugWall>();
    if (tmp == null)
    {
      GameObject.Instantiate(Resources.Load("debug-wall"));
    }
  }

  public TextMesh txtLogs;
  public TextMesh txtDynamics;
  public TextMesh txtInputs;

  int lastFrameInputs = 0;
  int lastFrameDynamics = 0;

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

    //rig.getButton()
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
