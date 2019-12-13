using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWall : AwkObject
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
  }

  protected override void setup()
  {
    base.setup();

    addLog(VersionManager.getFormatedVersion());
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
  
}
