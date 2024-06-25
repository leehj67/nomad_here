using UnityEngine;

public class CameraSpaceController : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void SetCameraPosition(Vector3 position, Vector2 spriteSize)
    {
        // 카메라 위치 설정
        mainCamera.transform.position = new Vector3(position.x, position.y, mainCamera.transform.position.z);

        // 카메라의 Orthographic Size를 조정하여 Sprite를 화면에 꽉 차게 보이도록 설정합니다.
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = spriteSize.x / spriteSize.y;

        if (screenAspect >= targetAspect)
        {
            mainCamera.orthographicSize = spriteSize.y / 2;
        }
        else
        {
            float differenceInSize = targetAspect / screenAspect;
            mainCamera.orthographicSize = spriteSize.y / 2 * differenceInSize;
        }
    }
}
