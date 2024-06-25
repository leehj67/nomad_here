using UnityEngine;

public class PlayerController3 : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector2 moveInput;
    
    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        // 플레이어를 화면 하단에 고정
        transform.position = new Vector3(0, -4, 0);
    }
}
