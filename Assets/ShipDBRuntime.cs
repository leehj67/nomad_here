using System.Collections.Generic;
using UnityEngine;

public class ShipDBRuntime : MonoBehaviour
{
    public static ShipDBRuntime Instance { get; private set; }

    [System.Serializable]
    public class ShipState
    {
        public int day = 1;
        public int food = 100;
        public int parts = 100;
        public int energy = 100;
    }

    [System.Serializable]
    public class SettlementRecord
    {
        public int day;
        public bool success;              // 탑승자 1명 이상 + 기믹 완료 등
        public int boardedCount;          // 탑승자 수
        public int totalCarriedItems;     // 탑승자들이 들고 온 아이템 총합
        public string note;              // 디버그/메모
    }

    public ShipState ship = new ShipState();
    public List<SettlementRecord> logs = new List<SettlementRecord>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Append(SettlementRecord rec)
    {
        logs.Add(rec);
        Debug.Log($"[ShipDB] Day={rec.day} success={rec.success} boarded={rec.boardedCount} carriedItems={rec.totalCarriedItems} note={rec.note}");
    }
}
