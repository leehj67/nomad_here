using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform targetPosition;
    public SpriteRenderer targetSpriteRenderer;
    public float offsetDistance = 1.0f;
    public float cooldownTime = 1.0f;

    [Header("Lock")]
    public bool locked = false;         // ✅ 잠금 플래그
    public bool showDebug = false;

    private bool isCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (locked)
        {
            if (showDebug) Debug.Log("[Door] Locked. Entry blocked.");
            return;
        }

        if (isCooldown) return;

        StartCooldown();
        Vector3 targetPos = targetPosition.position + targetPosition.right * offsetDistance;
        other.transform.position = targetPos;
    }

    private void StartCooldown()
    {
        isCooldown = true;
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        isCooldown = false;
    }
}
