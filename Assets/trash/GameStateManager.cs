using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public static GameStateManager Instance;

    [SerializeField]
    private int shipFood = 100;
    public int ShipFood { get { return shipFood; } private set { shipFood = value; } }
    [SerializeField]
    private int shipParts = 100;
    public int ShipParts { get { return shipParts; } private set { shipParts = value; } }
    [SerializeField]
    private int shipEnergy = 100;
    public int ShipEnergy { get { return shipEnergy; } private set { shipEnergy = value; } }
    [SerializeField]
    private int day = 1;
    public int Day
    {
        get { return day; }
        set
        {
            day = value;
            photonView.RPC("OnDayChanged", RpcTarget.All, day);
        }
    }

    public GameObject dayPanelPrefab;
    private GameObject dayPanelInstance;
    private TMP_Text dayText;
    public float displayDuration = 3f;

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

    public PlayerState[] PlayerStates;

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

    public void UpdateShipFood(int amount)
    {
        ShipFood += amount;
        UpdateUI();
    }

    public void UpdateShipParts(int amount)
    {
        ShipParts += amount;
        UpdateUI();
    }

    public void UpdateShipEnergy(int amount)
    {
        ShipEnergy += amount;
        UpdateUI();
    }

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
        return index >= 0 && index < PlayerStates.Length;
    }

    public void AdvanceDay()
    {
        if (isAdvancingDay) return;

        Debug.Log("AdvanceDay called");
        isAdvancingDay = true;
        Day++;
        isAdvancingDay = false;
    }

    [PunRPC]
private void OnDayChanged(int newDay)
{
    day = newDay;
    if (PhotonNetwork.IsMasterClient)
    {
        EventManager.Instance.SelectRandomEvent(); // 이벤트 선택
        // 결과 eventId를 사용하려 했으나, SelectRandomEvent는 void를 반환하므로 다음 RPC 호출 부분 수정 필요
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
            // 이벤트 효과 적용
            ApplyEventEffects(gameEvent);
        }
    }

    public void ApplyEventEffects(GameEvent gameEvent)
    {
        // 이벤트 효과 동기화
        photonView.RPC("SyncStats", RpcTarget.All, gameEvent.foodChange, gameEvent.partsChange, gameEvent.energyChange);

        for (int i = 0; i < PlayerStates.Length; i++)
        {
            photonView.RPC("SyncPlayerStats", RpcTarget.All, i, gameEvent.healthChange, gameEvent.staminaChange, gameEvent.hungerChange);
        }
    }

    [PunRPC]
    public void SyncStats(int foodChange, int partsChange, int energyChange)
    {
        ShipFood += foodChange;
        ShipParts += partsChange;
        ShipEnergy += energyChange;
        UpdateUI();
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

    public void ShowDayPanel()
    {
        if (dayPanelInstance == null)
        {
            dayPanelInstance = Instantiate(dayPanelPrefab, transform);
            dayText = dayPanelInstance.GetComponentInChildren<TMP_Text>();
        }

        if (dayPanelInstance != null && dayText != null)
        {
            dayPanelInstance.SetActive(true);
            dayText.text = $"Day-{Day}";
            Invoke("HideDayPanel", displayDuration);
        }
        else
        {
            Debug.LogError("dayPanelInstance or dayText is not assigned properly.");
        }
    }

    public void ShowTimerPanel()
    {
        if (timerInstance == null)
        {
            timerInstance = Instantiate(timerPrefab, transform);
            timerText = timerInstance.GetComponentInChildren<TMP_Text>();
            continueButton = timerInstance.GetComponentInChildren<Button>();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        if (timerInstance != null)
        {
            timerInstance.SetActive(true);
            timerText.text = $"Time remaining: {timer} seconds";
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
          EndTimerAndProceed(); // 메서드 이름 수정
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

        PhotonNetwork.LoadLevel("PlayScene"); // 다음 씬으로 전환
    }

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
    }
}
