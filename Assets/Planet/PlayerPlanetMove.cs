using UnityEngine;
using System.Collections;

public class PlayerPlanetMove : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public CameraSpaceController cameraController;
    private bool isTeleporting = false;
    public VariableJoystick joystick; // Joystick 스크립트 참조

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 조이스틱 입력 받아오기
        moveInput.x = joystick.Horizontal;
        moveInput.y = joystick.Vertical;
    }

    void FixedUpdate()
    {
        if (!isTeleporting)
        {
            Vector2 newPosition = rb.position + moveInput * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            Door door = collision.GetComponent<Door>();
            if (door != null && door.targetPosition != null && door.targetSpriteRenderer != null)
            {
                // 문을 통해 이동
                StartCoroutine(TeleportToTarget(door.targetPosition, door.targetSpriteRenderer));
            }
        }
    }

    private IEnumerator TeleportToTarget(Transform targetPosition, SpriteRenderer targetSpriteRenderer)
    {
        isTeleporting = true;
        yield return new WaitForEndOfFrame(); // 한 프레임 대기

        // 타겟 스프라이트의 중앙과 크기 가져오기
        Vector3 spriteCenter = targetSpriteRenderer.bounds.center;
        Vector2 spriteSize = targetSpriteRenderer.bounds.size;

        // 카메라 이동 및 줌
        cameraController.SetCameraPosition(spriteCenter, spriteSize);

        yield return new WaitForEndOfFrame(); // 한 프레임 대기

        // 이동 후 문에서 약간 떨어진 위치로 이동하여 무한 반복 방지
        Vector3 offset = (targetPosition.position - transform.position).normalized * 2.0f; // 1 유닛 떨어진 위치
        transform.position = targetPosition.position + offset;

        isTeleporting = false;
    }
}
