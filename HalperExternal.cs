using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HalperExternal
{
	public static string GetExternalFolder()
  {
#if UNITY_EDITOR
    return Application.dataPath + "/external/";
#elif UNITY_STANDALONE_WIN
    return Application.dataPath + "/../external/";
#elif UNITY_STANDALONE_OSX
    return Application.dataPath + "/../../external/";
#else
    return Application.dataPath + "/../external/";
#endif
  }
}
