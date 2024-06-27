using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlanetMove : MonoBehaviour
{
    public float walkSpeed = 2.5f; // 걷기 속도
    public float runSpeed = 5.0f; // 달리기 속도
    private float currentSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    public CameraSpaceController cameraController;

    public RectTransform staminaBarRectTransform; // 스테미너 바 UI 참조 (RectTransform)
    public float maxStamina = 100f; // 최대 스테미너 값
    private float currentStamina; // 현재 스테미너 값

    public Button toggleRunWalkButton; // 달리기/걷기 전환 버튼
    private bool isRunning = true; // 현재 달리기 모드인지 여부

    public VariableJoystick joystick; // 조이스틱 입력 참조

    public float staminaRecoveryDelay = 3.0f; // 스테미너 회복 대기 시간
    private bool isRecoveringStamina = false; // 스테미너 회복 중인지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
        currentSpeed = runSpeed; // 초기 속도는 달리기 모드
        toggleRunWalkButton.onClick.AddListener(ToggleRunWalk);
        UpdateButtonColor();
        UpdateStaminaBar();
    }

    void Update()
    {
        moveInput.x = joystick.Horizontal;
        moveInput.y = joystick.Vertical;

        HandleStamina();
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void HandleStamina()
    {
        if (isRunning && moveInput.magnitude > 0 && currentStamina > 0)
        {
            currentStamina -= 20f * Time.deltaTime; // 달리기 시 스테미너 감소
        }
        else if (!isRecoveringStamina)
        {
            currentStamina += 10f * Time.deltaTime; // 걷기 또는 정지 시 스테미너 회복
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaBar();
        CheckStaminaForRunning();
    }

    void ToggleRunWalk()
    {
        isRunning = !isRunning;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        UpdateButtonColor(); // 버튼 색상 업데이트
    }

    void CheckStaminaForRunning()
    {
        if (isRunning && currentStamina <= 0)
        {
            ToggleRunWalk(); // 스테미너가 없다면 자동으로 걷기 모드로 전환
            StartCoroutine(RecoverStamina()); // 스테미너 회복 코루틴 시작
        }
    }

    IEnumerator RecoverStamina()
    {
        isRecoveringStamina = true;
        yield return new WaitForSeconds(staminaRecoveryDelay); // 대기 시간

        while (currentStamina < maxStamina)
        {
            currentStamina += 10f * Time.deltaTime; // 스테미너 회복
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            UpdateStaminaBar();
            yield return null;
        }

        isRecoveringStamina = false;
    }

    void UpdateStaminaBar()
    {
        float width = (currentStamina / maxStamina) * 200; // 스테미너 바의 기본 길이를 200으로 가정
        staminaBarRectTransform.sizeDelta = new Vector2(width, staminaBarRectTransform.sizeDelta.y);
    }

    void UpdateButtonColor()
    {
        ColorBlock colors = toggleRunWalkButton.colors;
        colors.normalColor = isRunning ? Color.green : Color.red; // 달리기 모드: 초록색, 걷기 모드: 빨간색
        colors.pressedColor = isRunning ? Color.green : Color.red;
        colors.selectedColor = isRunning ? Color.green : Color.red;
        colors.highlightedColor = isRunning ? Color.green : Color.red;
        toggleRunWalkButton.colors = colors;
    }
}
