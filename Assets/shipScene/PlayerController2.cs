using UnityEngine;
using TMPro;

public class PlayerController2 : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float interactionRange = 2.0f; // 상호작용 범위
    public TextMeshProUGUI interactionText; // TMP 텍스트
    public Transform cameraTarget; // 카메라가 이동할 목표 객체
    public Material outlineMaterial; // 강조 재질
    private CameraController cameraController;
    private SpriteRenderer previousRenderer;
    private Material originalMaterial;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        interactionText.gameObject.SetActive(false); // 초기에는 텍스트를 숨김
    }

    void Update()
    {
        HandleMovement();
        CheckForInteraction();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, moveY, 0);
        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    void CheckForInteraction()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        bool showInteractionText = false;
        GameObject interactableObject = null;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Interactable"))
            {
                showInteractionText = true;
                interactableObject = collider.gameObject;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (IsShipComputer(interactableObject))
                    {
                        cameraController.MoveToTarget(cameraTarget);
                    }
                }
                break;
            }
        }

        interactionText.gameObject.SetActive(showInteractionText);
        HighlightObject(interactableObject);
    }

    bool IsShipComputer(GameObject obj)
    {
        // "ship" 객체의 자식인지 확인
        Transform current = obj.transform;
        while (current != null)
        {
            if (current.name == "ship")
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }

    void HighlightObject(GameObject obj)
    {
        if (previousRenderer != null && previousRenderer.gameObject != obj)
        {
            RemoveHighlight();
        }

        if (obj != null)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                originalMaterial = renderer.material;
                renderer.material = outlineMaterial;
                previousRenderer = renderer;
            }
        }
    }

    void RemoveHighlight()
    {
        if (previousRenderer != null)
        {
            previousRenderer.material = originalMaterial;
            previousRenderer = null;
            originalMaterial = null;
        }
    }

    // 디버깅을 위한 Gizmos 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
