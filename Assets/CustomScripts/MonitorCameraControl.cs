using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MonitorCameraControl : MonoBehaviour
{
    InputDevice headDevice;
    public float SPEED = 15.0f;

    void Start()
    {
        headDevice = InputDevices.GetDeviceAtXRNode(XRNode.Head);
    }

    void Update()
    {
        if (headDevice != null && headDevice.isValid == false)
        {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * SPEED, -Input.GetAxis("Mouse X") * SPEED, 0));
            float x = transform.rotation.eulerAngles.x;
            float y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(x, y, 0);
        } 
    }
}

    