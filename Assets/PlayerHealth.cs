using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100.0f;
    public Image healthBar; // 체력바 UI 컴포넌트

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthBar(); // 체력바 업데이트 호출

        if (health <= 0)
        {
            Debug.Log("Player Died!");
            // 플레이어 사망 처리 로직
        }
    }

    // 체력바를 현재 체력에 맞춰 업데이트하는 함수
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = health / 100.0f;
        }
    }
}
