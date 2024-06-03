using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 캐릭터가 게임 시작 시 GameManager에서 제공하는 위치에서 시작하도록 설정
        transform.position = GameManager.Instance.PlayerStartPosition;
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 moveVelocity = moveInput.normalized * speed;
        rb.velocity = moveVelocity;

        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
