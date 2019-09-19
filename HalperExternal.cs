using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

static public class HalperExternal
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

  static string generatePath(string profil, string file, string ext = "txt")
  {
    string path = GetExternalFolder(); // default
    if (profil.Length > 0) path = generatePath(profil); // override default

    path += file + "." + ext; // add file

    return path;
  }
  static string generatePath(string profil)
  {
    return GetExternalFolder() + "/" + profil + "/";
  }

  public static string load(string profil, string file)
  {
    string dat = File.ReadAllText(generatePath(profil, file));
    return dat;
  }

  static public void load(string profil, string file, Action<string> onFileLoaded)
  {
    onFileLoaded(load(profil, file));
  }

  public static void saveToJson(string profil, string file, string content)
  {
    //File.WriteAllText(generatePath(profil, file), JsonUtility.ToJson(content));
    saveToJson(profil, file, content);
    //Debug.Log("saved text to json");
  }
  public static void saveToJson(string profil, string file, object data)
  {
    //make sure profil folder exists
    string profilPath = generatePath(profil);
    if(!Directory.Exists(generatePath(profilPath)))
    {
      Directory.CreateDirectory(profilPath);
    }

    File.WriteAllText(generatePath(profil, file), JsonUtility.ToJson(data));
    Debug.Log("saved json file "+file+" ("+profil+")");
  }

  public static T loadFromJson<T>(string profil, string file)
  {
    return (T)JsonUtility.FromJson<T>(load(profil, file));
  }

  public static bool hasExternalFile(string profil, string file)
  {
    return File.Exists(generatePath(profil, file));
  }

  public static bool hasProfil(string profil)
  {
    return Directory.Exists(generatePath(profil));
  }

  public static bool generateProfil(string profil, string profilToCopy = "")
  {
    if(!hasProfil(profil))
    {
      string targetDir = generatePath(profil);
      Directory.CreateDirectory(targetDir);

      if(profilToCopy.Length > 0)
      {
        
        string sourceDir = generatePath(profilToCopy);
        CopyDir.Copy(sourceDir, targetDir);
       
      }

      return true; // created
    }

    return false; // no need
  }
}
