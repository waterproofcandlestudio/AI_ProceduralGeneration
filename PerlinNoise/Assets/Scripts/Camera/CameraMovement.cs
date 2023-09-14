// Miguel Rodríguez Gallego
using UnityEngine;

/// <summary>
///     Lets player move camera freely around the map when he presses "spacebar"
/// </summary>
public class CameraMovement : MonoBehaviour
{
    Transform cameraHolder;

    [Header("Rotation")]
    [SerializeField] float sensivity = 2f;
    [SerializeField] float viewClampYMin = -90;
    [SerializeField] float viewClampYMax = 90;

    [Header("Movement")]
    [SerializeField] float mainSpeed = 100.0f; //regular speed
    [SerializeField] float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    [SerializeField] float maxShift = 1000.0f; //Maximum speed when holdin gshift

    float totalRun = 1.0f;
    float xRot;
    float yRot;
    bool cameraLocked;

    void Start() => InitializeVariables();
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (cameraLocked)
                ActivateCameraMovement();
            else
                DesactivateCameraMovement();
        }

        if (cameraLocked)
            return;

        CalculateViewMouse(GetMouseInput());
        CameraMovementLogic();
    }

    /// <summary>
    /// 
    ///     CalculateViewMouse && CalculateViewGamePad -- Mouse && GamePad view camera
    ///     
    ///     Called by Update.
    ///     
    ///     Returns player view camera rotation.
    ///     It works with 2 types of movement:
    ///     Rotate the entire player on Y axis to look left or right.
    ///     Rotate the camera on X axis to look up and down.
    ///     
    ///     It also considers if player is aiming or not to reduce speed!
    ///     
    ///     Compatible with gamepad via different methods and sensivities
    ///     (gamePad is the same sensivity X2)
    /// 
    /// </summary>
    /// <param name="input"></param>
    void CalculateViewMouse(Vector2 input)
    {
        float mouseX = input.x * sensivity;
        float mouseY = input.y * sensivity;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, viewClampYMin, viewClampYMax);
        yRot += mouseX;

        cameraHolder.localRotation = Quaternion.Euler(new Vector3(xRot, yRot, 0));
    }
    /// <summary>
    ///     Camera movement logic
    /// </summary>
    void CameraMovementLogic()
    {
        Vector3 p = GetKeyboardInput();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime / Time.timeScale;

        transform.Translate(p);
    }

    Vector3 GetMouseInput() => new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    Vector3 GetKeyboardInput()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) p_Velocity += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S)) p_Velocity += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A)) p_Velocity += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D)) p_Velocity += new Vector3(1, 0, 0);
        return p_Velocity;
    }
    void ActivateCameraMovement()
    {
        cameraLocked = false;
        LockCursor();
    }
    void DesactivateCameraMovement()
    {
        cameraLocked = true;
        UnlockCursor();
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void InitializeVariables()
    {
        cameraHolder = Camera.main.transform;
        DesactivateCameraMovement();
        xRot = transform.eulerAngles.x;
        yRot = transform.eulerAngles.y;
    }
}
