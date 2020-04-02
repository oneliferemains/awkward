using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineGenericTools
{
  static public string getStamp(Object obj, string color = "#ffffff")
  {
    return Time.frameCount+" <color="+color+">"+obj.GetType()+"</color> | ";
  }

}
