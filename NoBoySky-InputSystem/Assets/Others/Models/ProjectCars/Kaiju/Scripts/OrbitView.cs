using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitView : MonoBehaviour {

    public Transform target;
    public float minDistance = 6f;
    public float maxDistance = 12f;
    public float zoomSpeed = 2f;

    public float xSpeed = 250f;
    public float ySpeed = 120f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    private float x = 0f;
    private float y = 0f;

    bool isHoldingMouse = false;
    private Vector2 mouseDelta = Vector2.zero;
    private float zoomValue = 0f;
    private float currentDistance = 10f;
    private Quaternion rotation;
    private Vector3 position;

[AddComponentMenu("Camera-Control/Mouse Orbit")]
   
    // Use this for initialization
    void Start () {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;

        rotation = transform.rotation;
        position = transform.position;

    }

    void OnStartRotate(InputValue value)
    {
        isHoldingMouse = value.isPressed;
    }

    void OnRotate(InputValue value)
    {
        mouseDelta = value.Get<Vector2>();
    }

    void OnZoom(InputValue value)
    {
        zoomValue = value.Get<float>();
    }


    private void LateUpdate()
    {
        if (Mathf.Abs(zoomValue) > 0)
        {
            currentDistance = currentDistance + (-zoomValue * zoomSpeed * Time.deltaTime);
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        if (isHoldingMouse)
        {
            x += mouseDelta.x * xSpeed;// * 0.02f;
            y -= mouseDelta.y * ySpeed;// * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            rotation = Quaternion.Euler(y, x, 0);
        }

        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -currentDistance) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
