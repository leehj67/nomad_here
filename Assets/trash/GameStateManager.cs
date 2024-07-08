using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviourPunCallbacks
{
    public static GameStateManager Instance;

    // 우주선 상태
    [SerializeField]
    private int shipFood = 100;
    public int ShipFood
    {
        get { return shipFood; }
        private set { shipFood = value; }
    }

    [SerializeField]
    private int shipParts = 100;
    public int ShipParts
    {
        get { return shipParts; }
        private set { shipParts = value; }
    }

    [SerializeField]
    private int shipEnergy = 100;
    public int ShipEnergy
    {
        get { return shipEnergy; }
        private set { shipEnergy = value; }
    }

    // 날짜 상태
    [SerializeField]
    private int day = 1;
    public int Day
    {
        get { return day; }
        set
        {
            day = value;
            OnDayChanged();
        }
    }

    // 날짜 패널 프리팹
    public GameObject dayPanelPrefab; // 프리팹 참조
    private GameObject dayPanelInstance; // 인스턴스화된 패널
    private TMP_Text dayText; // 날짜를 표시할 TMP 텍스트 컴포넌트
    public float displayDuration = 3f; // 패널이 사라지는 시간 조정 가능

    // 타이머 프리팹
    public GameObject timerPrefab; // 타이머 프리팹 참조
    private GameObject timerInstance; // 인스턴스화된 타이머
    private TMP_Text timerText; // 타이머를 표시할 TMP 텍스트 컴포넌트
    private float timer = 60f; // 60초 타이머

    // UI 버튼
    private Button continueButton; // 프리팹 내부의 계속 버튼

    // 플레이어 상태
    [System.Serializable]
    public class PlayerState
    {
        [SerializeField]
        private int health = 100;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        [SerializeField]
        private int stamina = 100;
        public int Stamina
        {
            get { return stamina; }
            set { stamina = value; }
        }

        [SerializeField]
        private int hunger = 0;
        public int Hunger
        {
            get { return hunger; }
            set { hunger = value; }
        }
    }

    public PlayerState[] PlayerStates;

    // SpaceshipUIManager 참조
    private SpaceshipUIManager spaceshipUIManager;

    private PhotonView photonView; // PhotonView 참조 추가

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // PhotonView 컴포넌트 가져오기
            photonView = GetComponent<PhotonView>();
            if (photonView == null)
            {
                photonView = gameObject.AddComponent<PhotonView>(); // PhotonView가 없으면 추가
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            // Photon으로부터 플레이어 수를 받아옴
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            // Initialize player states for the number of players in the room
            PlayerStates = new PlayerState[playerCount];
            for (int i = 0; i < PlayerStates.Length; i++)
            {
                PlayerStates[i] = new PlayerState();
            }

            // 게임 시작 시 초기화 메서드 호출
            InitializeGame();
        }
        else
        {
            Debug.LogError("PhotonNetwork.CurrentRoom is null. Make sure you are connected to a Photon room.");
        }
    }

    private void InitializeGame()
    {
        // Day를 초기화
        Day = 1;

        // 다른 초기화 작업 수행
        ShipFood = 100;
        ShipParts = 100;
        ShipEnergy = 100;

        // 첫 번째 날 패널 표시
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

    // Methods to update ship state
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

    // Methods to update player state
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

    private void OnDayChanged()
    {
        // 날짜가 변경될 때마다 이벤트를 발생시킴
        EventManager.Instance.SelectRandomEvent();
        UpdateUI();
    }

    // 게임 내 다른 로직에서 Day 프로퍼티를 변경할 수 있는 메서드 추가
    public void AdvanceDay()
    {
        Day++;
        OnDayChanged(); // Day가 변경되면 OnDayChanged를 호출
    }

    // 날짜 패널 표시 메서드
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

            // 일정 시간 후 패널 비활성화
            Invoke("HideDayPanel", displayDuration);
        }
        else
        {
            Debug.LogError("dayPanelInstance or dayText is not assigned properly.");
        }
    }

    private void ShowTimerPanel()
    {
        if (timerInstance == null)
        {
            timerInstance = Instantiate(timerPrefab, transform);
            timerText = timerInstance.GetComponentInChildren<TMP_Text>();
            continueButton = timerInstance.GetComponentInChildren<Button>();
            continueButton.onClick.AddListener(OnContinueButtonClicked); // 버튼 클릭 이벤트 등록
        }

        if (timerInstance != null)
        {
            timerInstance.SetActive(true);
            timerText.text = $"Time remaining: {timer} seconds"; // 초기 타이머 값 표시
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

    // UI 업데이트 메서드
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

    private void OnContinueButtonClicked()
    {
        EndTimerAndProceed();
    }

    private void EndTimerAndProceed()
    {
        // 타이머 초기화 및 타이머 패널 비활성화
        photonView.RPC("RPC_EndTimerAndProceed", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_EndTimerAndProceed()
    {
        // 타이머 초기화
        timer = 60f;

        // 타이머 패널 비활성화
        if (timerInstance != null)
        {
            timerInstance.SetActive(false);
        }

        // 모든 플레이어가 PlayScene으로 이동
        LoadPlayScene();
    }

    private void LoadPlayScene()
    {
        // PlayScene으로 넘어가는 로직
        PhotonNetwork.LoadLevel("PlayScene");
    }
}
