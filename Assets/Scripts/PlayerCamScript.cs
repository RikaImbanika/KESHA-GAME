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

    private float smoothedSpeedX;
    private float smoothedSpeedY;

    public Camera showingCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        S.PlayerCamScript = this;
    }

    void Update()
    {
        if (!showingCamera.enabled)
            if (!inventory.opened)
                if (!inventory._marketOpened)
                {
                    float mouseX = Input.GetAxisRaw("Mouse X") * sensetivityX;
                    float mouseY = Input.GetAxisRaw("Mouse Y") * sensetivityY;

                    float t = S.Inventory._zoomed / S.Inventory._zoomTime;

                    //1

                    xRotation += mouseX * (1 - t);
                    yRotation -= mouseY * (1 - t);

                    //2

                    float k = Mathf.Clamp(Time.deltaTime * 2.75f, 0, 1f);

                    float x = Mathf.Pow(mouseX, 0.85f);
                    if (mouseX < 0)
                        x = -Mathf.Pow(-mouseX, 0.85f);

                    float y = Mathf.Pow(mouseY, 0.85f);
                    if (mouseY < 0)
                        y = -Mathf.Pow(-mouseY, 0.85f);

                    smoothedSpeedX = smoothedSpeedX * (1 - k) + x * k * 0.25f;
                    smoothedSpeedY = smoothedSpeedY * (1 - k) + y * k * 0.25f;

                    xRotation += smoothedSpeedX * t;
                    yRotation -= smoothedSpeedY * t;

                    yRotation = Mathf.Clamp(yRotation, -90f, 90f);

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
