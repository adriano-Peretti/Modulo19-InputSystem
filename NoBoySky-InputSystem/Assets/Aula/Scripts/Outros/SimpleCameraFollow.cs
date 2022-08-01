using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target = null;
    public float smoothSpeed = 80f;
    //public Vector3 cameraOffset = new Vector3(0f, 5f, -10f);
    public float distance = -3f;

    Vector3 velocityRef = Vector3.zero;

    private void FixedUpdate()
    {
        Quaternion lookDirection = Quaternion.LookRotation(target.forward);

        Vector3 position = lookDirection * new Vector3(0.0f, 0.0f, distance) + target.position;

        transform.rotation = lookDirection;
        transform.position = Vector3.Slerp(transform.position, position, smoothSpeed * Time.deltaTime);

    }

}
