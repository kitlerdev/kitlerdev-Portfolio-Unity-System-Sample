using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Assign your player transform here

    [Header("Camera Settings")]
    public float smoothSpeed = 5f; // How smooth the camera follows
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Camera position offset

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired camera position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Update camera position
        transform.position = smoothedPosition;
    }
}