using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 캐릭터의 Transform
    public float moveSpeed = 5.0f; // 카메라가 따라가는 속도
    public Vector3 offset; // 캐릭터로부터의 오프셋, Z값 포함

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // 캐릭터의 현재 위치 + 오프셋
        desiredPosition.z = transform.position.z; // 카메라의 Z값을 현재 Z값으로 고정

        // 부드럽게 이동을 구현하되, 속도를 직접 조절
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
    }
}
