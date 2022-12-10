using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 20f;
    float roty = 0f;
    float rotx = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        roty += Input.GetAxisRaw("Mouse X") * sensitivity;
        rotx -= Input.GetAxisRaw("Mouse Y") * sensitivity;

        rotx = Mathf.Clamp(rotx, -90f, 90f);

        transform.localRotation = Quaternion.Euler(rotx, 0, 0);
        transform.parent.localRotation = Quaternion.Euler(transform.parent.rotation.x, roty, transform.parent.rotation.y);
    }
}
