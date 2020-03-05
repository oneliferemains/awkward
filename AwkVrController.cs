using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AwkVrController : AwkVrObject
{

  [RuntimeInitializeOnLoadMethod]
  static protected void create()
  {
    AwkVrController lrc = GameObject.FindObjectOfType<AwkVrController>();
    if (lrc == null)
    {
      GameObject.Instantiate(Resources.Load("rig"));
    }
  }

  public AwkVrControllerHand hand_left;
  public AwkVrControllerHand hand_right;

  List<VRTriggerWrapper> triggers = new List<VRTriggerWrapper>();
  List<VRButtonWrapper> buttons = new List<VRButtonWrapper>();

  Vector2 _rightStick;
  public Action<Vector2> onRightStick;

  Vector2 _leftStick;
  public Action<Vector2> onLeftStick;

  OVRInput.Controller inputActiveController = OVRInput.Controller.LTouch;

  protected override void setup()
  {
    base.setup();

    buttons.Add(new VRButtonWrapper(this, OVRInput.Controller.LTouch, OVRInput.Button.One));
    buttons.Add(new VRButtonWrapper(this, OVRInput.Controller.LTouch, OVRInput.Button.Two));
    buttons.Add(new VRButtonWrapper(this, OVRInput.Controller.RTouch, OVRInput.Button.One));
    buttons.Add(new VRButtonWrapper(this, OVRInput.Controller.RTouch, OVRInput.Button.Two));

    triggers.Add(new VRTriggerWrapper(this, OVRInput.Controller.RTouch, OVRInput.Axis1D.PrimaryIndexTrigger));
    triggers.Add(new VRTriggerWrapper(this, OVRInput.Controller.RTouch, OVRInput.Axis1D.PrimaryHandTrigger));
    triggers.Add(new VRTriggerWrapper(this, OVRInput.Controller.LTouch, OVRInput.Axis1D.PrimaryIndexTrigger));
    triggers.Add(new VRTriggerWrapper(this, OVRInput.Controller.LTouch, OVRInput.Axis1D.PrimaryHandTrigger));

    //Debug.Assert(rigTr != null, name);

    //Debug.Log(buttons.Count + " , " + triggers.Count);

    if (DebugWall.instance != null)
    {
      DebugWall.instance.addLog("setup() position : " + rigTr.position);
    }
  }

  protected override void setupVr()
  {
    base.setupVr();

    if (DebugWall.instance != null)
    {
      DebugWall.instance.addLog("VR rig moved : " + rigTr.position);
    }
  }

  protected override void update()
  {
    base.update();

    //OVRInput.Update();

    DebugWall.instance.addDyna("rig ? " + rigTr.position);

    //OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger);

    //https://developer.oculus.com/documentation/unity/latest/concepts/unity-ovrinput/

#if !UNITY_EDITOR
    updateVRInput();
#else
    simulateInput();
#endif

  }

  public VRButtonWrapper getButton(OVRInput.Controller controller, OVRInput.Button button)
  {
    for (int i = 0; i < buttons.Count; i++)
    {
      if (buttons[i].isButton(controller, button)) return buttons[i];
    }
    Debug.LogWarning("can't find button " + controller + " , " + button);
    return null;
  }

  public VRTriggerWrapper getTrigger(OVRInput.Controller controller, OVRInput.Axis1D button)
  {
    for (int i = 0; i < triggers.Count; i++)
    {
      if (triggers[i].isTrigger(controller, button)) return triggers[i];
    }
    Debug.LogWarning("can't find trigger " + controller + " , " + button);
    return null;
  }


  void updateVRInput()
  {
    for (int i = 0; i < buttons.Count; i++)
    {
      buttons[i].updateVr();
    }

    for (int i = 0; i < triggers.Count; i++)
    {
      triggers[i].updateVr();
    }

    checkVrStick(ref _leftStick, OVRInput.Controller.LTouch);
    checkVrStick(ref _rightStick, OVRInput.Controller.RTouch);
  }

  void simulateInput()
  {
    //float sign = Mathf.Sign(Input.mouseScrollDelta.y);
    if (Input.mouseScrollDelta.y != 0f)
    {
      inputActiveController = inputActiveController == OVRInput.Controller.LTouch ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
      Debug.Log(inputActiveController);
    }

    editorUpdateHandPosition(); // place transform to mouse cursor

    if (Input.GetMouseButton(0))
    {
      getTrigger(inputActiveController, OVRInput.Axis1D.PrimaryIndexTrigger).update(1f);
    }
    else if (Input.GetMouseButtonUp(0))
    {
      getTrigger(OVRInput.Controller.LTouch, OVRInput.Axis1D.PrimaryIndexTrigger).update(0f);
      getTrigger(OVRInput.Controller.RTouch, OVRInput.Axis1D.PrimaryIndexTrigger).update(0f);
    }

    if (Input.GetMouseButton(1))
    {
      getTrigger(inputActiveController, OVRInput.Axis1D.PrimaryHandTrigger).update(1f);
    }
    else if (Input.GetMouseButtonUp(1))
    {
      getTrigger(OVRInput.Controller.LTouch, OVRInput.Axis1D.PrimaryHandTrigger).update(0f);
      getTrigger(OVRInput.Controller.RTouch, OVRInput.Axis1D.PrimaryHandTrigger).update(0f);
    }

    //left stick

    if (Input.GetKey(KeyCode.Z))
    {
      if (onLeftStick != null) onLeftStick(Vector2.up);
    }
    else if (Input.GetKey(KeyCode.S))
    {
      if (onLeftStick != null) onLeftStick(Vector2.down);
    }

    if (Input.GetKey(KeyCode.Q))
    {
      if (onLeftStick != null) onLeftStick(Vector2.left);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      if (onLeftStick != null) onLeftStick(Vector2.right);
    }

    //right stick
    if (inputActiveController == OVRInput.Controller.LTouch)
    {
      checkVirtualSticks(ref _leftStick, onLeftStick);
    }
    else
    {
      checkVirtualSticks(ref _rightStick, onRightStick);
    }

    //going up/down with lift
    getButton(OVRInput.Controller.LTouch, OVRInput.Button.Two).update(KeyCode.R);
    getButton(OVRInput.Controller.LTouch, OVRInput.Button.One).update(KeyCode.F);

    //serialization
    getButton(OVRInput.Controller.RTouch, OVRInput.Button.One).update(KeyCode.B); // save
    getButton(OVRInput.Controller.RTouch, OVRInput.Button.Two).update(KeyCode.N); // delete
  }

  void checkVirtualSticks(ref Vector2 buffer, Action<Vector2> callback = null)
  {
    //Action<Vector2> callback = inputActiveController == OVRInput.Controller.LTouch ? onLeftStick : onRightStick;

    Vector2 input = Vector2.zero;

    if (Input.GetKey(KeyCode.UpArrow)) input = Vector2.up;
    else if (Input.GetKey(KeyCode.DownArrow)) input = Vector2.down;
    else if (Input.GetKey(KeyCode.LeftArrow)) input = Vector2.left;
    else if (Input.GetKey(KeyCode.RightArrow)) input = Vector2.right;
    else
    {
      if (buffer.sqrMagnitude != 0f)
      {
        if (callback != null) callback(Vector2.zero);
        buffer = Vector2.zero;
      }
    }

    buffer = input;

    if (buffer.sqrMagnitude != 0f)
    {
      if (callback != null) callback(input);
    }

  }

  void checkVrStick(ref Vector2 refStick, OVRInput.Controller controller)
  {
    Vector2 stickFrame = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller);

    if (stickFrame.sqrMagnitude != 0f)
    {
      if (controller == OVRInput.Controller.RTouch)
      {
        if (onRightStick != null) onRightStick(stickFrame);
      }
      else if (controller == OVRInput.Controller.LTouch)
      {
        if (onLeftStick != null) onLeftStick(stickFrame);
      }

      refStick = stickFrame; // no need

      DebugWall.instance.addInputs("stick : " + stickFrame);
    }
  }

  /// <summary>
  /// override position of left hand transform for screen position
  /// </summary>
  void editorUpdateHandPosition()
  {
    //any press
    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
    {
      Vector3 position = Input.mousePosition;
      //position.z = Camera.main.transform.position.z;
      position.z = 1f;

      Transform target = hand_left.transform;

      if (inputActiveController == OVRInput.Controller.RTouch) target = hand_right.transform;

      target.position = Camera.main.ScreenToWorldPoint(position);
      target.rotation = Camera.main.transform.rotation;

      //Debug.Log(Input.mousePosition + " => " + hand_left.handAnchor.position);
    }
  }

  static public AwkVrController getRig()
  {
    return GameObject.FindObjectOfType<AwkVrController>();
  }


}


abstract public class VrInputWrapper
{
  protected OVRInput.Controller controller;
  protected AwkVrController lrc; // bridge to hand transforms

  public VrInputWrapper(AwkVrController lrc, OVRInput.Controller controller)
  {
    this.lrc = lrc;
    this.controller = controller;
  }

  abstract public void updateVr();

  protected bool isController(OVRInput.Controller controller)
  {
    return this.controller == controller;
  }

  public Transform getHandTransform()
  {
#if UNITY_EDITOR
    return controller == OVRInput.Controller.LTouch ? lrc.hand_left.transform : lrc.hand_right.transform;
#else
    return controller == OVRInput.Controller.LTouch ? lrc.hand_left.handAnchor : lrc.hand_right.handAnchor;
#endif
  }
}

public class VRTriggerWrapper : VrInputWrapper
{
  OVRInput.Axis1D button;

  float _state = 0f;

  public Action onEmpty;
  public Action onFull;
  public Action<float> onChange;
  public Action onNotEmpty;

  public VRTriggerWrapper(AwkVrController lrc, OVRInput.Controller controller, OVRInput.Axis1D btnRange) : base(lrc, controller)
  {
    button = btnRange;
  }

  public override void updateVr()
  {
    update(OVRInput.Get(button, controller));
  }

  public void update(float state)
  {
    DebugWall.instance.addInputs(controller.ToString() + " | (" + button.ToString() + ") | " + state);

    if (state != _state)
    {
      _state = state;

      if (_state <= 0f)
      {
        if (onEmpty != null) onEmpty();
      }
      else if (_state >= 1f)
      {
        if (onFull != null) onFull();
      }

      if (onChange != null) onChange(_state);
    }

    if (_state > 0f)
    {
      if (onNotEmpty != null) onNotEmpty();
    }
  }

  public bool isTrigger(OVRInput.Controller controller, OVRInput.Axis1D button)
  {
    if (!isController(controller)) return false;
    return this.button == button;
  }
}

public class VRButtonWrapper : VrInputWrapper
{
  OVRInput.Button button;
  bool _state = false;

  public Action onRelease;
  public Action onPress;
  public Action onPressing;

  public VRButtonWrapper(AwkVrController lrc, OVRInput.Controller controller, OVRInput.Button btnToggle) : base(lrc, controller)
  {
    button = btnToggle;
  }

  public override void updateVr()
  {
    update(OVRInput.Get(button, controller));
  }

  public void update(bool state)
  {
    DebugWall.instance.addInputs(controller.ToString() + " | (" + button.ToString() + ") | " + state);

    if (state != _state)
    {
      _state = state;
      if (_state)
      {
        if (onPress != null) onPress();
      }
      else
      {
        if (onRelease != null) onRelease();
      }

      //if (onStateChange != null) onStateChange(_state, controller == OVRInput.Controller.LTouch ? lrc.hand_left.handTr : lrc.hand_right.handTr);
    }

    if (state && onPressing != null) onPressing();
  }

  public void update(KeyCode key)
  {
    bool keyState = Input.GetKey(key);
    update(keyState);
  }

  public bool isButton(OVRInput.Controller controller, OVRInput.Button button)
  {
    if (!isController(controller)) return false;
    return this.button == button;
  }
}
