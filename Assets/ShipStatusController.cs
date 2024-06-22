using UnityEngine;
using TMPro;
using MyGameNamespace; // GameManager 네임스페이스 참조

public class ShipStatusController : MonoBehaviour
{
    public TextMeshProUGUI part1StatusText; // Part1의 상태를 표시할 TMP 텍스트
    public TextMeshProUGUI part2StatusText; // Part2의 상태를 표시할 TMP 텍스트
    public TextMeshProUGUI part3StatusText; // Part3의 상태를 표시할 TMP 텍스트
    public TextMeshProUGUI part4StatusText; // Part4의 상태를 표시할 TMP 텍스트

    void Start()
    {
        // 각 파츠의 초기 상태 설정
        UpdateAllPartsStatus();
    }

    // 모든 파츠의 상태를 업데이트하는 메서드
    void UpdateAllPartsStatus()
    {
        UpdatePartStatus(0);
        UpdatePartStatus(1);
        UpdatePartStatus(2);
        UpdatePartStatus(3);
    }

    // 특정 파츠의 상태를 업데이트하는 메서드
    public void UpdatePartStatus(int partNumber)
    {
        int health = GameManager.Instance.GetPartHealth(partNumber); // Instance로 수정
        string status = GetStatusFromHealth(health);

        switch (partNumber)
        {
            case 0:
                part1StatusText.text = $"Part 1: {status}";
                break;
            case 1:
                part2StatusText.text = $"Part 2: {status}";
                break;
            case 2:
                part3StatusText.text = $"Part 3: {status}";
                break;
            case 3:
                part4StatusText.text = $"Part 4: {status}";
                break;
            default:
                Debug.LogError("Invalid part number");
                break;
        }
    }

    // 건강 상태에 따른 텍스트 반환 메서드
    string GetStatusFromHealth(int health)
    {
        if (health > 75)
        {
            return "Normal";
        }
        else if (health > 50)
        {
            return "Damaged";
        }
        else if (health > 25)
        {
            return "Critical";
        }
        else
        {
            return "Destroyed";
        }
    }

    // 외부에서 파츠의 상태를 수정할 수 있는 메서드
    public void SetPartHealth(int partNumber, int health)
    {
        GameManager.Instance.UpdatePartHealth(partNumber, health); // Instance로 수정
        UpdatePartStatus(partNumber);
    }
}
