using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{

  IEnumerator Start()
  {
    enabled = false;

    GameSetup gs = GameSetup.FindObjectOfType<GameSetup>();

    while (!gs.isReady()) yield return null;
    
    setup();

    yield return null;

    setupLate();

    enabled = true;
  }


  virtual protected void setup()
  { }

  virtual protected void setupLate()
  { }

  virtual public void reboot()
  { }

  private void Update()
  {
    update();
  }

  virtual protected void update()
  { }
}
