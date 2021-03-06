﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

static public class HalperExternal
{


	public static string GetExternalFolderName()
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
    string path = GetExternalFolderName(); // default
    if (profil.Length > 0) path = generatePath(profil); // override default

    path += file + "." + ext; // add file

    return path;
  }
  static string generatePath(string profil)
  {
    return GetExternalFolderName() + "/" + profil + "/";
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
    saveToJson(profil, file, (object)content);
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

    string json = JsonUtility.ToJson(data, true);
    File.WriteAllText(generatePath(profil, file), json);
    Debug.Log("saved json file : " + file + " | profil : <b>" + profil + "</b> (length " + json.Length + ")\n" + json);
  }

  public static T loadFromJson<T>(string profil, string file)
  {
    //Debug.Log("load " + typeof(T).ToString() + " for file : " + file);
    string json = load(profil, file);
    //Debug.Log(json);
    return JsonUtility.FromJson<T>(json);
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


  public static void removeAllFilesOfAType(string pathToFolder, string extension)
  {
    if (!Directory.Exists(pathToFolder))
    {
      Debug.LogWarning("trying to remove all files of extension : "+extension+" at path : "+pathToFolder + " but path doesn't exists ?");
      return;
    }

    Debug.Log("<b>removing files</b> of extension " + extension + " in folder " + pathToFolder);

    int count = 0;
    string[] files = Directory.GetFiles(pathToFolder);
    for (int i = 0; i < files.Length; i++)
    {
      if (File.Exists(files[i]))
      {
        if (files[i].EndsWith(extension))
        {
          File.Delete(files[i]);
          //Debug.Log("  L removed " + files[i]);
          count++;
        }
      }
    }

    Debug.Log("completed ! removed " + count + " files with extension " + extension);
  }

}
