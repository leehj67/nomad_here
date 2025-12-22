using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public static GameStateManager Instance;

    [Header("Ship Resources")]
    [SerializeField]
    private int shipFood = 100;
    public int ShipFood { get { return shipFood; } private set { shipFood = value; } }

    [SerializeField]
    private int shipParts = 100;
    public int ShipParts { get { return shipParts; } private set { shipParts = value; } }

    [SerializeField]
    private int shipEnergy = 100;
    public int ShipEnergy { get { return shipEnergy; } private set { shipEnergy = value; } }

    [Header("Day / Turn")]
    [SerializeField]
    private int day = 1;
    public int Day
    {
        get { return day; }
        set
        {
            day = value;
            // 모든 클라이언트에 Day 변경 브로드캐스트
            photonView.RPC("OnDayChanged", RpcTarget.All, day);
        }
    }

    [Header("Day Panel UI")]
    public GameObject dayPanelPrefab;
    private GameObject dayPanelInstance;
    private TMP_Text dayText;
    public float displayDuration = 3f;

    [Header("Timer UI")]
    public GameObject timerPrefab;
    private GameObject timerInstance;
    private TMP_Text timerText;
    private float timer = 60f;
    private Button continueButton;

    private bool isAdvancingDay = false;

    [System.Serializable]
    public class PlayerState
    {
        public int Health = 100;
        public int Stamina = 100;
        public int Hunger = 0;
    }

    [Header("Player States")]
    public PlayerState[] PlayerStates;

    // 우주선 UI 참조
    private SpaceshipUIManager spaceshipUIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            PlayerStates = new PlayerState[playerCount];
            for (int i = 0; i < PlayerStates.Length; i++)
            {
                PlayerStates[i] = new PlayerState();
            }

            InitializeGame();
        }
        else
        {
            Debug.LogError("PhotonNetwork.CurrentRoom is null. Make sure you are connected to a Photon room.");
        }
    }

    private void InitializeGame()
    {
        ShipFood = 100;
        ShipParts = 100;
        ShipEnergy = 100;
        UpdateUI();

        ShowDayPanel();
        ShowTimerPanel();
    }

    private void Update()
    {
        if (timerInstance != null && timerInstance.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"Time remaining: {Mathf.Ceil(timer)} seconds";
            }

            if (timer <= 0)
            {
                EndTimerAndProceed();
            }
        }
    }

    // ===========================
    //  Ship 자원 변경용 메서드들
    // ===========================

    // 인벤토리 / 이벤트 등에서 호출할 공식 메서드
    public void AddShipFood(int amount)
    {
        ShipFood = Mathf.Clamp(ShipFood + amount, 0, 100);
        UpdateUI();
    }

    public void AddShipParts(int amount)
    {
        ShipParts = Mathf.Clamp(ShipParts + amount, 0, 100);
        UpdateUI();
    }

    public void AddShipEnergy(int amount)
    {
        ShipEnergy = Mathf.Clamp(ShipEnergy + amount, 0, 100);
        UpdateUI();
    }

    // 기존 UpdateShipXXX 를 이미 다른 코드에서 쓰고 있을 수 있으니,
    // 내부적으로 AddShipXXX 를 호출하게 연결
    public void UpdateShipFood(int amount)
    {
        AddShipFood(amount);
    }

    public void UpdateShipParts(int amount)
    {
        AddShipParts(amount);
    }

    public void UpdateShipEnergy(int amount)
    {
        AddShipEnergy(amount);
    }

    // ===========================
    //  Player 스탯 변경
    // ===========================

    public void UpdatePlayerHealth(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Health += amount;
            UpdateUI();
        }
    }

    public void UpdatePlayerStamina(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Stamina += amount;
            UpdateUI();
        }
    }

    public void UpdatePlayerHunger(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Hunger += amount;
            UpdateUI();
        }
    }

    private bool IsValidPlayerIndex(int index)
    {
        return PlayerStates != null && index >= 0 && index < PlayerStates.Length;
    }

    // ===========================
    //  Day / 이벤트 처리
    // ===========================

    public void AdvanceDay()
    {
        if (isAdvancingDay) return;

        Debug.Log("AdvanceDay called");
        isAdvancingDay = true;
        Day++;  // setter에서 OnDayChanged RPC 호출
        isAdvancingDay = false;
    }

    [PunRPC]
    private void OnDayChanged(int newDay)
    {
        day = newDay;

        // 마스터에서만 이벤트 선택
        if (PhotonNetwork.IsMasterClient)
        {
            EventManager.Instance.SelectRandomEvent();
            // SelectRandomEvent() 내부에서 ApplyEventToAllClients 를 호출하도록 설계해도 됨
        }

        UpdateUI();
        ShowDayPanel();
        ShowTimerPanel();
    }

    [PunRPC]
    public void ApplyEventToAllClients(string eventId)
    {
        GameEvent gameEvent = EventManager.Instance.GetEventById(eventId);
        if (gameEvent != null)
        {
            ApplyEventEffects(gameEvent);
        }
    }

    public void ApplyEventEffects(GameEvent gameEvent)
    {
        // 우주선 자원 변화 동기화
        photonView.RPC("SyncStats", RpcTarget.All,
            gameEvent.foodChange, gameEvent.partsChange, gameEvent.energyChange);

        // 플레이어 스탯 변화 동기화
        for (int i = 0; i < PlayerStates.Length; i++)
        {
            photonView.RPC("SyncPlayerStats", RpcTarget.All,
                i, gameEvent.healthChange, gameEvent.staminaChange, gameEvent.hungerChange);
        }
    }

    [PunRPC]
    public void SyncStats(int foodChange, int partsChange, int energyChange)
    {
        AddShipFood(foodChange);
        AddShipParts(partsChange);
        AddShipEnergy(energyChange);
    }

    [PunRPC]
    public void SyncPlayerStats(int playerIndex, int healthChange, int staminaChange, int hungerChange)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Health += healthChange;
            PlayerStates[playerIndex].Stamina += staminaChange;
            PlayerStates[playerIndex].Hunger += hungerChange;
            UpdateUI();
        }
    }

    // ===========================
    //  Day Panel / Timer UI
    // ===========================

    public void ShowDayPanel()
    {
        if (dayPanelPrefab == null)
        {
            Debug.LogError("dayPanelPrefab is not assigned.");
            return;
        }

        if (dayPanelInstance == null)
        {
            dayPanelInstance = Instantiate(dayPanelPrefab, transform);
            dayText = dayPanelInstance.GetComponentInChildren<TMP_Text>();
        }

        if (dayPanelInstance != null && dayText != null)
        {
            dayPanelInstance.SetActive(true);
            dayText.text = $"Day-{Day}";
            CancelInvoke(nameof(HideDayPanel));
            Invoke(nameof(HideDayPanel), displayDuration);
        }
        else
        {
            Debug.LogError("dayPanelInstance or dayText is not assigned properly.");
        }
    }

    public void ShowTimerPanel()
    {
        if (timerPrefab == null)
        {
            Debug.LogError("timerPrefab is not assigned.");
            return;
        }

        if (timerInstance == null)
        {
            timerInstance = Instantiate(timerPrefab, transform);
            timerText = timerInstance.GetComponentInChildren<TMP_Text>();
            continueButton = timerInstance.GetComponentInChildren<Button>();
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueButtonClicked);
            }
        }

        if (timerInstance != null)
        {
            timer = 60f; // 새 타이머 시작
            timerInstance.SetActive(true);
            if (timerText != null)
            {
                timerText.text = $"Time remaining: {timer} seconds";
            }
        }
        else
        {
            Debug.LogError("timerInstance is not assigned properly.");
        }
    }

    private void HideDayPanel()
    {
        if (dayPanelInstance != null)
        {
            dayPanelInstance.SetActive(false);
        }
    }

    private void OnContinueButtonClicked()
    {
        EndTimerAndProceed();
    }

    private void EndTimerAndProceed()
    {
        photonView.RPC("RPC_EndTimerAndProceed", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_EndTimerAndProceed()
    {
        timer = 60f;

        if (timerInstance != null)
        {
            timerInstance.SetActive(false);
        }

        // 여기서 PlayScene으로 전환 (탐사 시작)
        PhotonNetwork.LoadLevel("PlayScene");
    }

    // ===========================
    //  UI 연동
    // ===========================

    public void UpdateUI()
    {
        if (spaceshipUIManager != null)
        {
            spaceshipUIManager.UpdateUI();
        }
    }

    public void SetSpaceshipUIManager(SpaceshipUIManager uiManager)
    {
        spaceshipUIManager = uiManager;
        // 새로 세팅 시 현재 상태를 바로 반영
        spaceshipUIManager.UpdateUI();
    }
}
