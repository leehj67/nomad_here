using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject playerUIPrefab; // 활성화할 UI 프리팹

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Sprite clicked!"); // 클릭 이벤트 디버그 로그
        if (playerUIPrefab != null)
        {
            // UI 프리팹을 인스턴스화하여 활성화
            GameObject uiInstance = Instantiate(playerUIPrefab);
            uiInstance.transform.SetParent(GameObject.Find("Canvas").transform, false); // Canvas는 UI를 표시할 부모 오브젝트로 설정
        }
    }
}
