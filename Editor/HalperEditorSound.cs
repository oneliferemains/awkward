using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

static public class HalperEditorSound {

#if UNITY_EDITOR

  /// <summary>
  /// Joue l'AudioClip visé par assetPath.<para/>
  /// Le chemin commence a la racine du projets et fini par l'extension de l'AudioClip.<para/>
  /// Exemple : "Assets/Unexported/Sounds/onDebugLogError.ogg"
  /// </summary>
  /// <param name="audioClipAssetPath">Le chemin relatif au projet de l'AudioClip.</param>
  static public bool playAudioClip(string audioClipAssetPath)
  {
    AudioClip audioClip = (AudioClip)EditorGUIUtility.Load(audioClipAssetPath);

    playAudioClip(audioClip);

    return audioClip != null;

  }// playAudioClip()


  /// <summary>
  /// Joue un son dans l'éditeur Unity.<para/>
  /// ATTENTION : utilise AudioPreview pour jouer le son, du coup le son est joué a son volume maximum !<para/>
  /// Nb.: Utilisez UnityEditor.EditorGUIUtility.Load("Assets/.../audioClipName.ext") pour récupérer votre référence a l'audioClip.
  /// </summary>
  /// <param name="audioClip">L'AudioClip a jouer.</param>
  static public void playAudioClip(AudioClip audioClip)
  {
    if (audioClip == null) return;

    Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

    System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

    MethodInfo method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);

    method.Invoke(null, new object[] { audioClip });

  }// playClip()


  /// <summary>
  /// Stop tous les son actif de l'éditeur (pour la plupart lancés via playClip().
  /// </summary>
  static public void stopAllClips()
  {
    Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

    System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");

    MethodInfo method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { }, null);

    method.Invoke(null, new object[] { });

  }// stopAllClips()

#endif
}
