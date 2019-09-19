using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorProfilWindow : EditorWindow
{
  [MenuItem("Tools/Profiles")]
  static void init()
  {
    EditorWindow.GetWindow(typeof(EditorProfilWindow));
  }
  
  ProfileData profiles;
  
  int profile_index = -1;

  void OnEnabled()
  {
    
  }

  void fetchProfil()
  {
    profiles = ProfileManager.getProfilData();

    //default
    solveIndex();
  }

  void solveIndex()
  {

    //default
    profile_index = ProfileManager.getPpActiveSelectionIndex();

    //dans l'editeur on ne veut pas utiliser le fichier de profil
    //pour pouvoir build et avoir un profil setup qui changera pas

    /*
    string profilName = "";

    if (profiles.profiles.Length > 0)
    {
      profilName = ProfileManager.loadActiveProfil(profiles.profiles[0]); // returns value from config file OR default value

      Debug.Log("active profil ? " + profilName);

      //si le config file (ou le ProfileData) a filé une valeur
      if (profilName.Length > 0)
      {
        for (int i = 0; i < profiles.profiles.Length; i++)
        {
          if (profilName.ToLower() == profiles.profiles[i].ToLower())
          {
            Debug.Log("found index of profil " + profilName);
            profile_index = i;
            return;
          }
        }
      }

    }
    
    */
  }

  void OnGUI()
  {
    if (profiles == null)
    {
      GUILayout.Label("No profiles in project. You need to create a profile data object");

      fetchProfil();

      return;
    }

    if(profiles.profiles.Length <= 0)
    {
      GUILayout.Label("ProfileData exists but is empty");
      return;
    }

    solveIndex();
    
    GUILayout.Label("Profiles (" + profile_index + ")");

    int newIndex = EditorGUILayout.Popup(profile_index, profiles.profiles);
    if(newIndex != profile_index)
    {
      PlayerPrefs.SetInt(ProfileManager.profile_index_pp, newIndex);
      profile_index = newIndex;
      Debug.Log("swap profile to " + profiles.profiles[profile_index]);

      checkProfile(profiles.profiles[profile_index]);
    }

    if(GUILayout.Button("refresh from config file"))
    {
      //solveIndex();
      fetchProfil();
    }
  }

  void checkProfile(string profileName)
  {
    if(HalperExternal.generateProfil(profileName, "default"))
    {
      Debug.Log("profile " + profileName + " generated");
      AssetDatabase.Refresh();
    }
  }
}
