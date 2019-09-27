using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// 
/// how to use :
/// - faire une scène dédiée nommée "resource-sshot"
/// - rajouter cette mono dans la scène
/// - filer a la mono une rendertexture de la taille souhaité (output png)
/// - Appeller la static "call" pour lancer le process
/// 
/// this script will create itself in the scene, take a sshot, destroy itself
/// 
/// Troubleshoot :
/// truc chelou, il faut que la cam ai un mesh dans l'axe pour avoir un pixel visible au final lors de l'encodage en PNG
/// (mettre un graaand quad dans le fond)
/// 
/// /// notes :
/// https://answers.unity.com/questions/12070/capture-rendered-scene-to-png-with-background-tran.html

/// </summary>

public class ScreenshotManager : MonoBehaviour
{
  static public string screenPath = "";
  string generatedPath = "";
  
  public RenderTexture renderTex;
  Camera cam;

  Texture2D virtualPhoto = null;
  Color32[] pixels;

  private void Start()
  {
    cam = Camera.main;
    toggleCamera(cam, true);

    setup();
  }
  
  [ContextMenu("start process")]
  void setup()
  {
    Debug.LogWarning("PRESS FOR SCREENSHOT");
    
    toggleCamera(cam, true);

    generatedPath = Application.dataPath+"/../sshots/";

    //generatedPath = Path.Combine(Application.dataPath, "/YourSubDirectory/yourprogram.exe");

    if (!Directory.Exists(generatedPath)) Directory.CreateDirectory(generatedPath); // generate folder

    Debug.Log("everything is setup for screenshot");

    takePhoto();
  }

  void takePhoto()
  {
    // capture the virtuCam and save it as a square PNG.

    //RenderTexture tempRT = new RenderTexture(sqr, sqr, 24);
    // the 24 can be 0,16,24, formats like
    // RenderTextureFormat.Default, ARGB32 etc.
    
    Debug.Log("  L rendering ...");

    //RenderTexture renderTex = new RenderTexture(1920, 1090, 16);

    //assign renderTex to cam
    cam.targetTexture = renderTex;

    //cam.targetTexture = tempRT;
    cam.Render();

    RenderTexture.active = renderTex;

    //generate tex
    int w = cam.targetTexture.width;
    int h = cam.targetTexture.height;

    if (virtualPhoto == null)
    {
      virtualPhoto = new Texture2D(w, h, TextureFormat.RGBA32, false); // false, meaning no need for mipmaps
      Debug.Log("  L generated target 2D texture ... ");

    }
    //virtualPhoto.alphaIsTransparency = true;

    Debug.Log("  L virtualizing ...");

    //extract info from render texture
    //https://docs.unity3d.com/ScriptReference/Texture2D.ReadPixels.html
    virtualPhoto.ReadPixels(
      new Rect(0, 0, w, h), // crop in origin texture
      0, 0); // position in target texture

    //pixels = virtualPhoto.GetPixels32();
    //replacePinkPixels();
    //virtualPhoto.SetPixels32(pixels);

    virtualPhoto.Apply(); // not needed ?

    Debug.Log("  L releasing ...");

    RenderTexture.active = null; //can help avoid errors
    if(cam.targetTexture != null) cam.targetTexture = null;
    //cam.targetTexture = null;
    // consider ... Destroy(tempRT);

    byte[] bytes = null;
    //virtualPhoto.alphaIsTransparency = true;

    //https://docs.unity3d.com/ScriptReference/ImageConversion.EncodeToPNG.html
    bytes = virtualPhoto.EncodeToPNG();

    string path = generatedPath + generateScreenshotName();

    Debug.Log("  L saving ... " + path);

    screenPath = path;
    
    System.IO.File.WriteAllBytes(path, bytes);
    // virtualCam.SetActive(false); ... no great need for this.

    unSetup();
  }

  protected void replacePinkPixels()
  {

    Debug.Log("  L replacing pink by alpha ...");

    //remove all pink pixel to alpha transparent ones
    Color32[] pixels = virtualPhoto.GetPixels32();
    int count = 0;
    for (int i = 0; i < pixels.Length; i++)
    {
      //pink pixel ?
      if (
        pixels[i].r > 0.99f &&
        pixels[i].g < 0.01f &&
        pixels[i].b > 0.99f)
      {
        count++;
        pixels[i].r = pixels[i].b = pixels[i].g = 0;
        pixels[i].a = 0;
      }
    }

    Debug.Log("modified " + count + " pink pixels / out of " + pixels.Length + " pixels total");

  }

  protected string generateScreenshotName()
  {
    return "sshot_"+HalperTime.getFullDate()+"_"+Random.Range(0,99999)+".png";
  }

  //roll back whatever is needed
  protected void unSetup()
  {
    RenderTexture.active = null;

    //remove scene after
    SceneManager.UnloadSceneAsync(sceneExportName);
  }

  static public string sceneExportName = "resource-sshot";

  static public AsyncOperation call()
  {
    return SceneManager.LoadSceneAsync(sceneExportName, LoadSceneMode.Additive);
  }

  /// <summary>
  /// activate camera
  /// </summary>
  static public void toggleCamera(Camera targetCamera, bool flag)
  {
    if (targetCamera.enabled != flag) targetCamera.enabled = flag;
    if (targetCamera.gameObject.activeSelf != flag) targetCamera.gameObject.SetActive(flag);
  }

}
