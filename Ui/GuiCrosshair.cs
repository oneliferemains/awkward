using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCrosshair : MonoBehaviour
{
  public Texture2D crosshairRef;
  public Color crosshairColor;

  private void OnGUI()
  {
    float size = crosshairRef.width;
    GUI.DrawTexture(
      new Rect(Screen.width * 0.5f - size * 0.5f, Screen.height * 0.5f - size * 0.5f, crosshairRef.width, crosshairRef.height), 
      crosshairRef, ScaleMode.ScaleToFit, true, 0f, crosshairColor, 0f, 0f);
  }
}
