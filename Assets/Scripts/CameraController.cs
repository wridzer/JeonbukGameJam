using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player
    public float smoothSpeed = 0.125f;
    public Vector3 posOffset;
    public float heightRotOffset;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.TransformDirection(posOffset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Update the camera's rotation to match the player's
        Quaternion targetRot = Quaternion.Euler(target.rotation.eulerAngles.x + heightRotOffset, target.rotation.eulerAngles.y, target.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, smoothSpeed * Time.deltaTime);
    }
}
