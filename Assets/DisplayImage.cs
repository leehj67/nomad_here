using UnityEngine;
using UnityEngine.UI;

public class DisplayImage : MonoBehaviour
{
    public Image display; // UI에 이미지를 표시할 Image 컴포넌트
    public ImageStorage storage; // ImageStorage 컴포넌트가 있는 객체

    private bool[] keyPressed;

    void Start()
    {
        keyPressed = new bool[10]; // 0부터 9까지의 키 상태를 저장
    }

    void Update()
    {
        // 0부터 9까지의 숫자 키에 대해 검사합니다.
        for (int i = 0; i < 10; i++)
        {
            KeyCode alphaKey = KeyCode.Alpha0 + i;
            KeyCode keypadKey = KeyCode.Keypad0 + i;

            // 키가 눌려있는지 확인하고 상태가 바뀌었는지 체크
            if (Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey))
            {
                if (!keyPressed[i])
                {
                    UpdateDisplay(i);
                    keyPressed[i] = true; // 키 누름 상태 업데이트
                }
            }
            else if (Input.GetKeyUp(alphaKey) || Input.GetKeyUp(keypadKey))
            {
                keyPressed[i] = false; // 키 릴리스 상태 업데이트
            }
        }
    }

    void UpdateDisplay(int index)
    {
        if (index < storage.images.Length)
        {
            display.sprite = storage.images[index];
        }
    }
}
