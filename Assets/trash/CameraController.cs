using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float zoomSpeed = 2.0f;
    private Camera mainCamera;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private float targetOrthographicSize;
    private float initialOrthographicSize;
    private Vector3 initialPosition;

    void Start()
    {
        mainCamera = Camera.main;
        initialOrthographicSize = mainCamera.orthographicSize;
        initialPosition = mainCamera.transform.position;
        targetPosition = mainCamera.transform.position;
        targetOrthographicSize = initialOrthographicSize;
    }

    void Update()
    {
        if (isMoving)
        {
            // 카메라 위치 이동
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 카메라 줌 인/아웃
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, zoomSpeed * Time.deltaTime);

            // 이동이 완료되었는지 확인
            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.1f &&
                Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveToTarget(Transform targetTransform)
    {
        Bounds targetBounds = CalculateBounds(targetTransform);

        // 타겟의 중심으로 카메라 이동
        targetPosition = new Vector3(targetBounds.center.x, targetBounds.center.y, mainCamera.transform.position.z);

        // 타겟의 크기에 맞게 orthographicSize 조정
        float verticalSize = targetBounds.extents.y;
        float horizontalSize = targetBounds.extents.x / mainCamera.aspect;
        targetOrthographicSize = Mathf.Max(verticalSize, horizontalSize);

        isMoving = true;
    }

    public void ResetCamera()
    {
        targetPosition = initialPosition; // 원래 위치로 변경
        targetOrthographicSize = initialOrthographicSize; // 원래 줌 크기로 변경
        isMoving = true;
    }

    private Bounds CalculateBounds(Transform targetTransform)
    {
        Bounds bounds = new Bounds(targetTransform.position, Vector3.zero);
        Renderer[] renderers = targetTransform.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }
}
