using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5.0f;
    
    private Vector2 moveInput;

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // 플레이어의 움직임에 따라 환경 이동
        Vector3 moveDirection = new Vector3(-moveInput.x, -moveInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.position += moveDirection;
    }
}
