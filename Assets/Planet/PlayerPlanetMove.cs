using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPlanetMove : MonoBehaviourPun
{
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.0f;
    private float currentSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection; // 마지막 이동 방향을 저장
    public RectTransform staminaBarRectTransform;
    public float maxStamina = 100f;
    private float currentStamina;

    public Button toggleRunWalkButton;
    private bool isRunning = true;

    public VariableJoystick joystick;

    public float staminaRecoveryDelay = 3.0f;
    private bool isRecoveringStamina = false;
    private bool isTeleporting = false;

    private GameObject playerUI; // 플레이어 UI

    public void InitializeUI(GameObject uiInstance)
    {
        playerUI = uiInstance;
        staminaBarRectTransform = uiInstance.GetComponentInChildren<RectTransform>();
        toggleRunWalkButton = uiInstance.GetComponentInChildren<Button>();
        joystick = uiInstance.GetComponentInChildren<VariableJoystick>();

        toggleRunWalkButton.onClick.AddListener(ToggleRunWalk);
        UpdateButtonColor();
        UpdateStaminaBar();
    }

    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Rigidbody2D>());
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
        currentSpeed = runSpeed;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            moveInput.x = joystick.Horizontal;
            moveInput.y = joystick.Vertical;
            if (moveInput != Vector2.zero)
            {
                lastMoveDirection = moveInput.normalized; // 마지막 이동 방향 갱신
            }

            HandleStamina();
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine || isTeleporting)
            return;

        Vector2 newPosition = rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void HandleStamina()
    {
        if (isRunning && moveInput.magnitude > 0 && currentStamina > 0)
        {
            currentStamina -= 20f * Time.deltaTime;
        }
        else if (!isRecoveringStamina)
        {
            currentStamina += 10f * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaBar();
        CheckStaminaForRunning();
    }

    void ToggleRunWalk()
    {
        isRunning = !isRunning;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        UpdateButtonColor();
    }

    void CheckStaminaForRunning()
    {
        if (isRunning && currentStamina <= 0)
        {
            ToggleRunWalk();
            StartCoroutine(RecoverStamina());
        }
    }

    IEnumerator RecoverStamina()
    {
        isRecoveringStamina = true;
        yield return new WaitForSeconds(staminaRecoveryDelay);

        while (currentStamina < maxStamina)
        {
            currentStamina += 10f * Time.deltaTime;
            UpdateStaminaBar();
            yield return null;
        }

        isRecoveringStamina = false;
    }

    void UpdateStaminaBar()
    {
        if (staminaBarRectTransform != null)
        {
            float width = (currentStamina / maxStamina) * 200;
            staminaBarRectTransform.sizeDelta = new Vector2(width, staminaBarRectTransform.sizeDelta.y);
        }
    }

    void UpdateButtonColor()
    {
        if (toggleRunWalkButton != null)
        {
            ColorBlock colors = toggleRunWalkButton.colors;
            colors.normalColor = isRunning ? Color.green : Color.red;
            colors.pressedColor = isRunning ? Color.green : Color.red;
            colors.selectedColor = isRunning ? Color.green : Color.red;
            colors.highlightedColor = isRunning ? Color.green : Color.red;
            toggleRunWalkButton.colors = colors;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            Door door = collision.GetComponent<Door>();
            if (door != null && door.targetPosition != null)
            {
                StartCoroutine(TeleportToTarget(door.targetPosition));
            }
        }
    }

    private IEnumerator TeleportToTarget(Transform targetPosition)
    {
        isTeleporting = true;
        yield return new WaitForEndOfFrame(); // 현재 프레임 완료 대기

        // 이동 방향을 고려한 오프셋 적용
        Vector3 offset = lastMoveDirection * 2.0f;  // 마지막 이동 방향에 따라 오프셋 조정
        transform.position = targetPosition.position + offset;  // 타겟 위치에 오프셋 추가

        isTeleporting = false;
    }
}
