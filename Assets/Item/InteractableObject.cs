using UnityEngine;
using TMPro;

public class InteractableObjectWithTMP : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float interactionDistance = 2f; // 상호작용 반경
    public int objectType; // 오브젝트 종류 (0: Type1, 1: Type2, ...)

    private bool isPlayerInRange = false;

    void Update()
    {
        // 플레이어와 오브젝트 사이의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 플레이어가 상호작용 반경 안에 있는지 확인
        if (distance < interactionDistance)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                UIManager.instance.ShowInteractionText("press 'E'"); // 텍스트 활성화
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                UIManager.instance.HideInteractionText(); // 텍스트 비활성화
            }
        }

        // 플레이어가 상호작용 반경 안에 있고 E 키를 눌렀을 때
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            UIManager.instance.AddObject(objectType); // UIManager에 오브젝트 타입 전달
            UIManager.instance.HideInteractionText(); // 텍스트 비활성화
            gameObject.SetActive(false); // 스프라이트 오브젝트 비활성화
        }
    }
}
