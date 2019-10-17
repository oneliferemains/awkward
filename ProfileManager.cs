using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// in build on utilise seulement le fichier de config
/// dans l'editeur on utilise seulement l'index ppref
/// </summary>

static public class ProfileManager
{
  public const string profile_index_pp = "profile_index";
  
  static public string activeProfil = "none";

  [RuntimeInitializeOnLoadMethod]
  static public void solveActiveProfil()
  {
    solveActiveProfil("default");
  }

  static public void solveActiveProfil(string defaultProfileName)
  {
    activeProfil = ProfileManager.getActiveProfil(defaultProfileName);
  }

  /// <summary>
  /// détermine le profil a utiliser
  /// </summary>
  static public string getActiveProfil(string defaultProfilName = "")
  {
#if UNITY_EDITOR
    if (Application.isEditor)
    {
      int selectionIndex = getPpActiveSelectionIndex();
      Debug.Log("ppref selection is : " + selectionIndex);
      return getProfileStrings()[selectionIndex];
    }
#endif

    //trying to open config file
    ConfigFile file = new ConfigFile("profil.txt");
    
    //try to fetch category
    ConfigCategory cat = file.GetCategory("profil");
    
    //try to get category value
    if(cat != null && cat.hasParameter("profil"))
    {
      return cat.getString("profil", defaultProfilName);
    }

    //returns default when no config file value found
    return defaultProfilName;

  }

#if UNITY_EDITOR
  static public ProfileData getProfilData()
  {
    ProfileData outputData = null;

    // called on window creation
    string[] all = AssetDatabase.FindAssets("t:ProfileData");
    for (int i = 0; i < all.Length; i++)
    {
      Object obj = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(all[i]), typeof(ProfileData));
      outputData = obj as ProfileData;
    }

    return outputData;
  }

  static public string[] getProfileStrings()
  {
    return getProfilData().profiles;
  }

  static public int getPpActiveSelectionIndex()
  {
    return PlayerPrefs.GetInt(profile_index_pp, 0);
  }
#endif

}
