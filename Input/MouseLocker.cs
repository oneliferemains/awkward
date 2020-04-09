using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLocker
{

  static public void updateCursorLocking()
  {
#if ENABLE_LEGACY_INPUT_MANAGER
    if (Input.GetMouseButtonDown(0))
    {
      lockMouse();
    }

    if (Application.isEditor)
    {
      if (Input.GetKeyUp(KeyCode.Escape))
      {
        unlockMouse();
      }
    }

#endif
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
