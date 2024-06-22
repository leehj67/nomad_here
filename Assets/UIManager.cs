using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyGameNamespace; // GameManager 네임스페이스 참조
public class UIManager : MonoBehaviour
{
    public static UIManager instance; // 싱글톤 인스턴스
    public Image[] uiImages; // UI에 표시될 이미지 배열
    public Sprite[] objectSprites; // 오브젝트 종류에 따른 스프라이트 배열
    public TextMeshProUGUI interactionText; // "E를 누르세요" 텍스트 TMP UI
    public Vector2 defaultSize = new Vector2(100, 100); // 기본 이미지 크기
    public Vector2 selectedSize = new Vector2(150, 150); // 선택된 이미지 크기
    public SpriteRenderer handSpriteRenderer; // 2D 캐릭터 손의 스프라이트 렌더러

    private int nextImageIndex = 0; // 다음에 업데이트할 이미지 인덱스
    private int currentSelectedIndex = -1; // 현재 선택된 이미지 인덱스

    void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        HandleMouseScroll();
        UpdateHandSprite();
    }

    public void AddObject(int objectType)
    {
        if (nextImageIndex < uiImages.Length)
        {
            uiImages[nextImageIndex].sprite = objectSprites[objectType];
            uiImages[nextImageIndex].enabled = true;
            nextImageIndex++;
        }
    }

    public void ShowInteractionText(string text)
    {
        interactionText.text = text;
        interactionText.enabled = true;
    }

    public void HideInteractionText()
    {
        interactionText.enabled = false;
    }

    private void HandleMouseScroll()
    {
        if (uiImages.Length == 0)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                currentSelectedIndex = (currentSelectedIndex + 1) % uiImages.Length;
            }
            else if (scroll < 0)
            {
                currentSelectedIndex--;
                if (currentSelectedIndex < 0)
                    currentSelectedIndex = uiImages.Length - 1;
            }

            UpdateUISelection();
        }
    }

    private void UpdateUISelection()
    {
        for (int i = 0; i < uiImages.Length; i++)
        {
            if (i == currentSelectedIndex)
            {
                uiImages[i].rectTransform.sizeDelta = selectedSize;
            }
            else
            {
                uiImages[i].rectTransform.sizeDelta = defaultSize;
            }
        }
    }

    private void UpdateHandSprite()
    {
        if (currentSelectedIndex >= 0 && currentSelectedIndex < uiImages.Length)
        {
            handSpriteRenderer.sprite = uiImages[currentSelectedIndex].sprite;
            handSpriteRenderer.enabled = true;
        }
        else
        {
            handSpriteRenderer.enabled = false;
        }
    }
}
