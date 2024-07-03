using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    // 우주선 상태
    [SerializeField]
    private int shipFood = 100;
    public int ShipFood
    {
        get { return shipFood; }
        private set { shipFood = Mathf.Clamp(value, 0, 100); }
    }

    [SerializeField]
    private int shipParts = 100;
    public int ShipParts
    {
        get { return shipParts; }
        private set { shipParts = Mathf.Clamp(value, 0, 100); }
    }

    [SerializeField]
    private int shipEnergy = 100;
    public int ShipEnergy
    {
        get { return shipEnergy; }
        private set { shipEnergy = Mathf.Clamp(value, 0, 100); }
    }

    // 날짜 상태
    [SerializeField]
    private int day = 0;
    public int Day
    {
        get { return day; }
        set
        {
            day = value;
            OnDayChanged();
        }
    }

    // 플레이어 상태
    [System.Serializable]
    public class PlayerState
    {
        [SerializeField]
        private int health = 100;
        public int Health
        {
            get { return health; }
            set { health = Mathf.Clamp(value, 0, 100); }
        }

        [SerializeField]
        private int stamina = 100;
        public int Stamina
        {
            get { return stamina; }
            set { stamina = Mathf.Clamp(value, 0, 100); }
        }

        [SerializeField]
        private int hunger = 0;
        public int Hunger
        {
            get { return hunger; }
            set { hunger = Mathf.Clamp(value, 0, 100); }
        }
    }

    public PlayerState[] PlayerStates;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize player states for 4 players
        PlayerStates = new PlayerState[4];
        for (int i = 0; i < PlayerStates.Length; i++)
        {
            PlayerStates[i] = new PlayerState();
        }

        // 예제: 게임 시작 시 첫 번째 날로 설정
        Day = 1;
    }

    // Methods to update ship state
    public void UpdateShipFood(int amount)
    {
        ShipFood += amount;
    }

    public void UpdateShipParts(int amount)
    {
        ShipParts += amount;
    }

    public void UpdateShipEnergy(int amount)
    {
        ShipEnergy += amount;
    }

    // Methods to update player state
    public void UpdatePlayerHealth(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Health += amount;
        }
    }

    public void UpdatePlayerStamina(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Stamina += amount;
        }
    }

    public void UpdatePlayerHunger(int playerIndex, int amount)
    {
        if (IsValidPlayerIndex(playerIndex))
        {
            PlayerStates[playerIndex].Hunger += amount;
        }
    }

    private bool IsValidPlayerIndex(int index)
    {
        return index >= 0 && index < PlayerStates.Length;
    }

    private void OnDayChanged()
    {
        EventManager.Instance.SelectRandomEvent();
    }

    // 게임 내 다른 로직에서 Day 프로퍼티를 변경할 수 있는 메서드 추가
    public void AdvanceDay()
    {
        Day++;
    }
}
