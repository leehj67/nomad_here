using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerMain : MonoBehaviourPun
{
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.0f;
    private float currentSpeed;
    public float maxStamina = 100f;
    private float currentStamina;
    public float maxHP = 100f;
    private float currentHP;

    public Image staminaBar;
    public Image hpBar;
    public RectTransform itemUI;
    public VariableJoystick joystick;
    public Button toggleRunWalkButton;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isRunning = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentStamina = maxStamina;
        currentHP = maxHP;
        currentSpeed = runSpeed;

        if (photonView.IsMine)
        {
            InitializeUI();
        }
        else
        {
            Destroy(GetComponentInChildren<Canvas>().gameObject);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        moveInput.x = joystick.Horizontal;
        moveInput.y = joystick.Vertical;

        HandleStamina();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleRunWalk();
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 newPosition = rb.position + moveInput * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void HandleStamina()
    {
        if (isRunning && moveInput.magnitude > 0 && currentStamina > 0)
        {
            currentStamina -= 20f * Time.deltaTime;
        }
        else
        {
            currentStamina += 10f * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaBar();
    }

    private void ToggleRunWalk()
    {
        isRunning = !isRunning;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
        {
            float width = (currentStamina / maxStamina) * 200;
            staminaBar.rectTransform.sizeDelta = new Vector2(width, staminaBar.rectTransform.sizeDelta.y);
        }
    }

    private void UpdateHPBar()
    {
        if (hpBar != null)
        {
            float width = (currentHP / maxHP) * 200;
            hpBar.rectTransform.sizeDelta = new Vector2(width, hpBar.rectTransform.sizeDelta.y);
        }
    }

    public void InitializeUI()
    {
        GameObject uiCanvas = GameObject.Find("Canvas");
        staminaBar = uiCanvas.transform.Find("StaminaBar").GetComponent<Image>();
        hpBar = uiCanvas.transform.Find("HPBar").GetComponent<Image>();
        itemUI = uiCanvas.transform.Find("ItemUI").GetComponent<RectTransform>();
        toggleRunWalkButton = uiCanvas.transform.Find("ToggleRunWalkButton").GetComponent<Button>();
        toggleRunWalkButton.onClick.AddListener(ToggleRunWalk);
    }
}
