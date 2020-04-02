using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HalperCustomBuild
{
  static public string getExternalFolderName() => "external";

  /// <summary>
  /// path_to/external/ (returned path CONTAINS final '/' char)
  /// </summary>
  static public string getExternalPath()
  {
    string external = getExternalFolderName() + "/";

#if UNITY_EDITOR
    return Application.dataPath + "/" + external;
#elif UNITY_STANDALONE_OSX
    return Application.dataPath + "/../../"+external;
#else
    return Application.dataPath + "/../"+external;
#endif

  }

}
