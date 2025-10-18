using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private Vector2 minBounds;

    [SerializeField]
    private float smoothSpeed = 0.125f;

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("CameraFollow: Player Transform not assigned and Player tag not found.");
            }
        }
    }


    void LateUpdate()
    {
        if (playerTransform == null)
            return;
        Vector3 desiredPosition = new Vector3(
            playerTransform.position.x + offset.x,
            playerTransform.position.y + offset.y,
            transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        smoothedPosition.x = Mathf.Max(smoothedPosition.x, minBounds.x);
        smoothedPosition.y = Mathf.Max(smoothedPosition.y, minBounds.y);

        transform.position = smoothedPosition;
    }
}
