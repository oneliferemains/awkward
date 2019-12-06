using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugWall : AwkObject
{
  static public DebugWall instance;

  public TextMesh txtLogs;
  public TextMesh txtDynamics;

  int lastFrame = 0;

  protected override void build()
  {
    base.build();

    instance = this;
  }
  
  public void addLog(string log)
  {
    txtLogs.text += "\n"+log;
  }

  public void addDyna(string ct)
  {
    //clear last frame
    if(lastFrame != Time.frameCount)
    {
      txtDynamics.text = "";
      lastFrame = Time.frameCount;
    }

    txtDynamics.text += "\n"+ct;
  }
  
}
