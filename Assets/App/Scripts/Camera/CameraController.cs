using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    [SerializeField] private float timeOffset = 0.2f;
    private Vector3 velocity;

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, playerPos.position + Vector3.forward * -10f, ref velocity, timeOffset);
    }
}
