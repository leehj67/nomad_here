using UnityEngine;

public class DoorLockerByGimmick : MonoBehaviour
{
    public Door[] doorsToUnlock;   // 메인도어(들)
    public GimmickManager gimmick; // 씬에 있는 GimmickManager 연결(없으면 자동 탐색)
    public bool unlockWhenCanEscape = true;

    void Awake()
    {
        if (gimmick == null) gimmick = FindObjectOfType<GimmickManager>();
    }

    void Update()
    {
        if (gimmick == null || doorsToUnlock == null) return;

        bool shouldUnlock = unlockWhenCanEscape && gimmick.CanEscape;

        for (int i = 0; i < doorsToUnlock.Length; i++)
        {
            var d = doorsToUnlock[i];
            if (d == null) continue;

            // 잠금 상태만 갱신 (기존 Door 동작은 그대로)
            d.locked = !shouldUnlock;
        }
    }
}
