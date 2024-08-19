using UnityEngine;

public class Door : MonoBehaviour
{
     public Transform targetPosition; // 이동할 타겟 위치
    public SpriteRenderer targetSpriteRenderer; // 타겟 스프라이트 렌더러
    public float offsetDistance = 1.0f; // 문 앞에 캐릭터를 배치할 거리
    public float cooldownTime = 1.0f; // 문 사용 쿨다운 시간

    private bool isCooldown = false; // 쿨다운 상태를 확인하는 플래그

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCooldown)
        {
            StartCooldown(); // 쿨다운 시작
            Vector3 targetPos = targetPosition.position + targetPosition.right * offsetDistance; // 타겟 위치에서 오른쪽으로 일정 거리 오프셋
            other.transform.position = targetPos;
        }
    }

    private void StartCooldown()
    {
        isCooldown = true;
        Invoke("ResetCooldown", cooldownTime); // 지정된 쿨다운 시간 후에 쿨다운 리셋
    }

    private void ResetCooldown()
    {
        isCooldown = false;
    }
}
