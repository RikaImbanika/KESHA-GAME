using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamScript : MonoBehaviour
{
    public float sensetivityX;
    public float sensetivityY;

    public Transform orientation;
    public Inventory inventory;

    public float xRotation;
    public float yRotation;

    public Camera showingCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!showingCamera.enabled)
            if (!inventory.opened)
                if (!inventory._marketOpened)
                {
                    float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensetivityX;
                    float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensetivityY;

                    xRotation += mouseX;
                    yRotation -= mouseY;

                    yRotation = Mathf.Clamp(yRotation, -90f, 90f);

                    transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
                    orientation.rotation = Quaternion.Euler(yRotation, xRotation, 0);
                }
    }

    public void Rotate(float v)
    {
        xRotation += v;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(yRotation, xRotation, 0);
    }

    public void StaticRotate(float xRot, float yRot)
    {
        xRotation = xRot;
        yRotation = yRot;
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(yRot, xRot, 0);
    }
}
