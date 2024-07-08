using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private void Update()
    {
        // HP 비율 계산
        float healthRatio = currentHealth / maxHealth;
        healthBar.fillAmount = healthRatio;
    }

    // 외부에서 호출하여 HP를 감소시키는 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
