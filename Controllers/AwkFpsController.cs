// FPS Controller
// 1. Create a Parent Object like a 3D model
// 2. Make the Camera the user is going to use as a child and move it to the height you wish. 
// 3. Attach a Rigidbody to the parent
// 4. Drag the Camera into the m_Camera public variable slot in the inspector
// Escape Key: Escapes the mouse lock
// Mouse click after pressing escape will lock the mouse again

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AwkFpsController : MonoBehaviour
{
  public float speed = 5.0f;
  private float m_MovX;
  private float m_MovY;
  private Vector3 m_moveHorizontal;
  private Vector3 m_movVertical;
  private Vector3 m_velocity;
  private Rigidbody m_Rigid;
  private float m_yRot;
  private float m_xRot;
  private Vector3 m_rotation;
  private Vector3 m_cameraRotation;
  private float m_lookSensitivity = 3.0f;
  private bool m_cursorIsLocked = true;

  [Header("The Camera the player looks through")]
  public Camera m_Camera;

  float minimumY = -80f;
  float maximumY = 85f;
  float rotationY = 0f;

  // Use this for initialization
  private void Start()
  {
    m_Rigid = GetComponent<Rigidbody>();
    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  public void Update()
  {

    m_MovX = horizontalAxis();
    m_MovY = depthAxis();

    m_moveHorizontal = transform.right * m_MovX;
    m_movVertical = transform.forward * m_MovY;

    m_velocity = (m_moveHorizontal + m_movVertical).normalized * speed;

    //mouse movement 
    m_yRot = Input.GetAxisRaw("Mouse X");
    m_rotation = new Vector3(0, m_yRot, 0) * m_lookSensitivity;

    m_xRot = Input.GetAxisRaw("Mouse Y");
    //m_cameraRotation = new Vector3(m_xRot, 0, 0) * m_lookSensitivity;

    //apply camera rotation

    //move the actual player here
    if (m_velocity != Vector3.zero)
    {
      m_Rigid.MovePosition(m_Rigid.position + m_velocity * Time.fixedDeltaTime);
    }

    if (m_rotation != Vector3.zero)
    {
      //rotate the camera of the player
      m_Rigid.MoveRotation(m_Rigid.rotation * Quaternion.Euler(m_rotation));
    }

    if (m_Camera != null)
    {
      //negate this value so it rotates like a FPS not like a plane
      //m_Camera.transform.Rotate(-m_cameraRotation);

      if (m_xRot != 0f)
      {
        rotationY += m_xRot;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        m_Camera.transform.localEulerAngles = new Vector3(-rotationY, m_Camera.transform.localEulerAngles.y, 0);
      }
    }

    //InternalLockUpdate();

  }

  float depthAxis()
  {
    if (Input.GetKey(KeyCode.UpArrow)) return 1f;
    if (Input.GetKey(KeyCode.DownArrow)) return -1f;

    if (Input.GetKey(KeyCode.S)) return -1f;
    if (Input.GetKey(KeyCode.Z)) return 1f;

    return 0f;
  }

  float horizontalAxis()
  {
    if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
    if (Input.GetKey(KeyCode.RightArrow)) return 1f;

    if (Input.GetKey(KeyCode.Q)) return -1f;
    if (Input.GetKey(KeyCode.D)) return 1f;

    return 0f;
  }

  //controls the locking and unlocking of the mouse
  private void InternalLockUpdate()
  {
    if (Input.GetKeyUp(KeyCode.Escape))
    {
      m_cursorIsLocked = false;
    }
    else if (Input.GetMouseButtonUp(0))
    {
      m_cursorIsLocked = true;
    }

    if (m_cursorIsLocked)
    {
      UnlockCursor();
    }
    else if (!m_cursorIsLocked)
    {
      LockCursor();
    }
  }

  private void UnlockCursor()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void LockCursor()
  {
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

}
