using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 500f;
 
    float _xRotation = 0f;
    float _yRotation = 0f;
 
    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //control rotation around x-axis (Look up and down)
        _xRotation -= mouseY;

        //we clamp the rotation so we cant Over-rotate (like in real life)
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        //control rotation around y-axis (Look up and down)
        _yRotation += mouseX;

        //applying both rotations
        transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
    }
}
