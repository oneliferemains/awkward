using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLocker
{

  static public void updateCursorLocking()
  {
    if (Input.GetMouseButtonDown(0))
    {
      if (HalperUi.IsPointerOverUIElement()) return;
      lockMouse();
    }

    if (Input.GetKeyUp(KeyCode.Escape)) // unlock cursor
    {
      unlockMouse();
    }

  }

  static public void unlockMouse()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  static public void lockMouse()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  static public bool isUnlocked() => Cursor.visible && Cursor.lockState == CursorLockMode.None;

}
