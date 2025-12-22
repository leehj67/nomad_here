using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal; // Light2D

public class Player_Move : MonoBehaviourPun, IPunObservable
{
    // ===== Weight Settings =====
    [Header("Weight Move Penalty")]
    public float weightPenaltyK = 0.25f;   // 무게 1.0당 속도 감소 강도
    public float minSpeedFactor = 0.45f;   // 최소 속도 비율

    // ===== Flashlight Rotate =====
    [Header("Flashlight Rotate")]
    public Transform flashlightPivot;             // FlashlightPivot (Empty)
    public float flashlightAngleOffset = 90f;     // 정면 보정(-90/0/90 등)
    public float rotateSpeed = 18f;               // 회전 부드러움
    private Vector2 lastFaceDir = Vector2.down;   // 기본 정면(아래)

    [Header("Flashlight Light2D (URP)")]
    public Light2D flashlightLight;               // Pivot 자식 Light2D
    public bool startFlashlightOn = false;        // 시작 상태

    [Header("Flashlight Beam Settings (Inspector)")]
    public float beamLength = 10f;                // 거리(Outer Radius)
    public float beamInner = 0.5f;                // Inner Radius
    [Range(0f, 1f)] public float beamFalloff = 0.8f;
    public Vector2 beamScale = new Vector2(1f, 1f);

    // ===== UI =====
    [Header("Camera Effect")]
    public float normalCameraSize = 5f;
    public float lowStaminaCameraSize = 3.5f;
    public Camera playerCamera;

    public InventoryManager inventoryUI;
    public Joystick joystick;
    public Image staminaImage;
    public Image lowStaminaOverlay;

    // 🔦 후레쉬 아이콘
    public Image flashlightIcon;
    bool isFlashlightOn = false;

    // ===== Movement =====
    public float Speed = 3f;
    public float runSpeedMultiplier = 1.6f;

    Rigidbody2D rigid;
    Animator anim;

    float h, v;
    bool isMoving;
    bool isRunning;
    float dirX, dirY;

    // ===== Item =====
    private PlayerInventory inventory;
    private Collider2D detectedItem;

    // ===== Fuse / Gimmick Interact =====
    [Header("Interact")]
    public string fuseTag = "fuse";          // Fuse(스위치) 오브젝트 Tag
    public float interactRange = 0.7f;       // 탐지 박스 크기(반지름 느낌)
    private Collider2D detectedFuse;

    // ===== Stamina =====
    public float maxStamina = 100f;
    public float runDrainPerSecond = 10f;
    public float recoverPerSecond = 8f;
    public float minStaminaToRun = 10f;

    float currentStamina;
    bool runButtonHeld = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rigid.gravityScale = 0;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

        inventory = GetComponent<PlayerInventory>();
        currentStamina = maxStamina;
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            Camera cam = Camera.main;
            playerCamera = cam;

            CameraFollow2D follow = cam != null ? cam.GetComponent<CameraFollow2D>() : null;
            if (follow != null)
                follow.target = transform;

            if (cam != null)
                normalCameraSize = cam.orthographicSize;
        }

        // Light2D 자동 탐색(인스펙터 연결 안 했을 때)
        if (flashlightLight == null && flashlightPivot != null)
            flashlightLight = flashlightPivot.GetComponentInChildren<Light2D>(true);

        // 시작 상태 강제
        isFlashlightOn = startFlashlightOn;
        ApplyFlashlightSettings();
        ApplyFlashlightState();

        StartCoroutine(CheckItemCollision());
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // ===== Movement Input =====
        h = Input.GetAxisRaw("Horizontal") + (joystick ? joystick.Horizontal : 0);
        v = Input.GetAxisRaw("Vertical") + (joystick ? joystick.Vertical : 0);

        dirX = h;
        dirY = v;

        isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

        // 바라보는 방향 갱신 (멈춰도 마지막 방향 유지)
        Vector2 inputDir = new Vector2(dirX, dirY);
        if (inputDir.sqrMagnitude > 0.01f)
            lastFaceDir = inputDir.normalized;

        // 🔦 회전(켜졌을 때만)
        if (isFlashlightOn)
            UpdateFlashlightDirection(Time.deltaTime);

        // ===== Running =====
        bool runKeyHeld = Input.GetKey(KeyCode.LeftShift);
        bool wantRun = runKeyHeld || runButtonHeld;
        bool canRun = currentStamina > minStaminaToRun;

        isRunning = wantRun && canRun && isMoving;

        TickStamina(Time.deltaTime);

        // ✅ 슬롯 선택 + 픽업/드랍/상호작용 입력
        HandleInventoryInput();

        // 🔦 토글
        if (Input.GetKeyDown(KeyCode.F))
            ToggleFlashlight();

        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector2 moveVec = new Vector2(h, v).normalized;

        float baseSpeed = isRunning ? Speed * runSpeedMultiplier : Speed;

        float weight = (inventory != null) ? inventory.TotalWeight() : 0f;
        float factor = 1f / (1f + weight * weightPenaltyK);
        factor = Mathf.Clamp(factor, minSpeedFactor, 1f);

        float moveSpeed = baseSpeed * factor;
        rigid.velocity = moveVec * moveSpeed;
    }

    // =========================
    // Flashlight
    // =========================
    void UpdateFlashlightDirection(float dt)
    {
        if (flashlightPivot == null) return;

        float angle = Mathf.Atan2(lastFaceDir.y, lastFaceDir.x) * Mathf.Rad2Deg;
        angle += flashlightAngleOffset;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle);

        flashlightPivot.rotation = Quaternion.Slerp(
            flashlightPivot.rotation,
            targetRot,
            dt * rotateSpeed
        );
    }

    void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;

        ApplyFlashlightSettings();
        ApplyFlashlightState();
    }

    void ApplyFlashlightState()
    {
        if (flashlightIcon != null)
            flashlightIcon.gameObject.SetActive(isFlashlightOn);

        if (flashlightLight != null)
            flashlightLight.enabled = isFlashlightOn;
    }

    void ApplyFlashlightSettings()
    {
        if (flashlightLight == null) return;

        flashlightLight.pointLightOuterRadius = Mathf.Max(0.01f, beamLength);
        flashlightLight.pointLightInnerRadius = Mathf.Clamp(beamInner, 0f, beamLength);
        flashlightLight.falloffIntensity = beamFalloff;

        flashlightLight.transform.localScale = new Vector3(
            Mathf.Max(0.01f, beamScale.x),
            Mathf.Max(0.01f, beamScale.y),
            1f
        );
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (flashlightLight == null && flashlightPivot != null)
            flashlightLight = flashlightPivot.GetComponentInChildren<Light2D>(true);

        ApplyFlashlightSettings();
    }
#endif

    // =========================
    // Stamina
    // =========================
    void TickStamina(float dt)
    {
        if (isRunning)
            currentStamina -= runDrainPerSecond * dt;
        else
            currentStamina += recoverPerSecond * dt;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina <= 0.01f)
            isRunning = false;

        UpdateStaminaUI();
        UpdateLowStaminaEffect();
    }

    void UpdateStaminaUI()
    {
        if (staminaImage == null) return;
        staminaImage.fillAmount = currentStamina / maxStamina;
    }

    // ✅ 저스태미나: 화면 어두워짐 + 카메라 줌(원본 복구)
    void UpdateLowStaminaEffect()
    {
        if (lowStaminaOverlay == null) return;

        float ratio = currentStamina / maxStamina;

        if (ratio < 0.3f)
        {
            float baseAlpha = Mathf.Lerp(0f, 0.8f, (0.3f - ratio) / 0.3f);
            float flicker = Mathf.Sin(Time.time * 18f) * 0.1f;
            float alpha = Mathf.Clamp01(baseAlpha + flicker);

            Color c = lowStaminaOverlay.color;
            c.a = alpha;
            lowStaminaOverlay.color = c;

            if (playerCamera != null)
            {
                float targetSize = Mathf.Lerp(lowStaminaCameraSize, normalCameraSize, ratio / 0.3f);
                playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, targetSize, Time.deltaTime * 4f);
            }
        }
        else
        {
            Color c = lowStaminaOverlay.color;
            c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * 5f);
            lowStaminaOverlay.color = c;

            if (playerCamera != null)
                playerCamera.orthographicSize = Mathf.Lerp(playerCamera.orthographicSize, normalCameraSize, Time.deltaTime * 2f);
        }
    }

    // =========================
    // Inventory + Interact (E key)
    // =========================
    void HandleInventoryInput()
    {
        if (!photonView.IsMine) return;
        if (inventoryUI == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) inventoryUI.SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) inventoryUI.SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) inventoryUI.SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) inventoryUI.SelectSlot(3);

        if (Input.GetKeyDown(KeyCode.E))
        {
            // ✅ 우선순위 1) Fuse(기믹) 상호작용
            if (TryInteractFuse())
                return;

            // ✅ 우선순위 2) 아이템 줍기
            if (detectedItem != null)
            {
                WorldItem w = detectedItem.GetComponent<WorldItem>();
                if (w != null) w.RequestPickup();
                return;
            }

            // ✅ 우선순위 3) 드랍
            DropSelectedItem_Fix();
        }
    }

    bool TryInteractFuse()
    {
        if (detectedFuse == null) return false;

        // Fuse 오브젝트(또는 부모)에 GimmickNode가 있다고 가정
        GimmickNode node = detectedFuse.GetComponent<GimmickNode>();
        if (node == null)
            node = detectedFuse.GetComponentInParent<GimmickNode>();
        if (node == null)
            node = detectedFuse.GetComponentInChildren<GimmickNode>();

        if (node == null)
        {
            Debug.LogWarning("[Player_Move] detectedFuse has no GimmickNode.");
            return false;
        }

        // ✅ 다인 전제: node.MarkSolved()가 마스터 승인 + RPC 동기화 처리
        node.MarkSolved();
        return true;
    }

    // ✅ 핵심: 드랍은 UI가 아니라 "진짜 인벤(List)"에서 먼저 제거해야 누적 Full이 안 남
    void DropSelectedItem_Fix()
    {
        if (inventory == null || inventoryUI == null) return;

        int slot = inventoryUI.GetSelectedSlotIndex();
        if (slot < 0) return;

        ItemData removed = inventory.RemoveAt(slot);
        if (removed == null) return;

        // UI는 인벤 기준으로 다시 그림
        inventoryUI.RebuildFromInventory(inventory.items);

        // 월드에 드랍 스폰
        Vector3 pos;
        if (inventoryUI.dropPoint != null)
            pos = inventoryUI.dropPoint.position + (Vector3)inventoryUI.dropOffset;
        else
            pos = transform.position + Vector3.right * 0.5f;

        PhotonNetwork.Instantiate(removed.itemId, pos, Quaternion.identity);
    }

    // =========================
    // Animation
    // =========================
    void UpdateAnimation()
    {
        if (anim == null) return;

        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("DirX", dirX);
        anim.SetFloat("DirY", dirY);

        if (Mathf.Abs(dirX) + Mathf.Abs(dirY) == 0)
        {
            anim.SetBool("Up", false);
            anim.SetBool("Down", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
        }
        else
        {
            anim.SetBool("Up", dirY > 0);
            anim.SetBool("Down", dirY < 0);
            anim.SetBool("Right", dirX < 0 ? false : dirX > 0);
            anim.SetBool("Left", dirX > 0 ? false : dirX < 0);
        }
    }

    // =========================
    // Collision Check (Item + Fuse)
    // =========================
    IEnumerator CheckItemCollision()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            // 탐지 범위(원래 Vector2.one*0.7f였던 것 -> interactRange로 통일)
            Vector2 boxSize = Vector2.one * Mathf.Max(0.1f, interactRange);

            Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
            detectedItem = null;
            detectedFuse = null;

            foreach (Collider2D col in cols)
            {
                if (col == null) continue;

                // fuse 우선 탐지
                if (detectedFuse == null && col.CompareTag(fuseTag))
                {
                    detectedFuse = col;
                    continue;
                }

                if (detectedItem == null && col.CompareTag("item"))
                {
                    detectedItem = col;
                    continue;
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 boxSize = Vector2.one * Mathf.Max(0.1f, interactRange);
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
#endif

    // =========================
    // Photon Serialize (필요시 확장)
    // =========================
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 필요 시 확장
    }
}
