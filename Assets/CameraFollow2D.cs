using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;                 // 따라갈 대상 (플레이어)
    public float smoothSpeed = 5f;           // 카메라 따라가는 부드러움
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
