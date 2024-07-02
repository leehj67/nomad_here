using UnityEngine;

public class CameraSpaceController : MonoBehaviour
{
    private Camera playerCamera;
    public RectTransform canvasRectTransform; // Canvas의 RectTransform 참조

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        // 초기 카메라 설정
        SetCameraToFitUI();
    }

    public void SetCameraToFitUI()
    {
        if (canvasRectTransform != null)
        {
            // 캔버스의 크기를 가져옵니다.
            float canvasWidth = canvasRectTransform.rect.width;
            float canvasHeight = canvasRectTransform.rect.height;

            // 화면 비율 계산
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float canvasAspect = canvasWidth / canvasHeight;

            if (screenAspect >= canvasAspect)
            {
                // 화면 비율이 캔버스 비율보다 크거나 같으면, 캔버스 높이에 맞춰 카메라 크기를 조정합니다.
                playerCamera.orthographicSize = canvasHeight / 2;
            }
            else
            {
                // 화면 비율이 캔버스 비율보다 작으면, 캔버스 넓이에 맞춰 카메라 크기를 조정합니다.
                float differenceInSize = canvasAspect / screenAspect;
                playerCamera.orthographicSize = (canvasHeight / 2) * differenceInSize;
            }
        }
    }

    public void SetCameraPosition(Vector3 position, Vector2 spriteSize)
    {
        // 카메라 위치 설정
        playerCamera.transform.position = new Vector3(position.x, position.y, playerCamera.transform.position.z);

        // 카메라의 Orthographic Size를 조정하여 Sprite를 화면에 꽉 차게 보이도록 설정합니다.
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = spriteSize.x / spriteSize.y;

        if (screenAspect >= targetAspect)
        {
            playerCamera.orthographicSize = spriteSize.y / 2;
        }
        else
        {
            float differenceInSize = targetAspect / screenAspect;
            playerCamera.orthographicSize = spriteSize.y / 2 * differenceInSize;
        }
    }
}
