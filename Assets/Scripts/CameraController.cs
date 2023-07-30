using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player
    public float smoothSpeed = 0.125f;
    public Vector3 posOffset;
    public float heightRotOffset;
    public bool isEnd = false;
    [SerializeField] private Vector3 endPosition = new Vector3(0, 421, -317);
    [SerializeField] private Vector3 endRotation = new Vector3(57.2547836f, 0, 0);

    void LateUpdate()
    {
        if (!isEnd)
        {
            Vector3 desiredPosition = target.position + target.TransformDirection(posOffset);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // Update the camera's rotation to match the player's
            Quaternion targetRot = Quaternion.Euler(target.rotation.eulerAngles.x + heightRotOffset, target.rotation.eulerAngles.y, target.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, smoothSpeed * Time.deltaTime);
        } else
        {
            Vector3 desiredPosition = endPosition;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            Quaternion targetRot = Quaternion.Euler(endRotation.x, endRotation.y, endRotation.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, smoothSpeed * Time.deltaTime);
        }
    }

    public void ToggleEndGame()
    {
        isEnd = true;
    }
}
