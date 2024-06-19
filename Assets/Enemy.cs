using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float attackRange = 5.0f; // 공격 범위
    public float damagePerSecond = 10.0f; // 초당 피해량

    private void Update()
    {
        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 거리가 공격 범위 이내일 경우 피해 처리
        if (distance <= attackRange)
        {
            // 플레이어에게 피해를 줌
            player.GetComponent<PlayerHealth>().TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
