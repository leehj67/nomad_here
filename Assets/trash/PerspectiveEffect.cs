using UnityEngine;

public class PerspectiveEffect : MonoBehaviour
{
    public Transform player;
    public float perspectiveFactor = 0.1f; // 원근법 효과 정도

    void Update()
    {
        // 플레이어의 Y 위치에 따라 배경 오브젝트의 크기를 조절하여 원근법 효과를 적용합니다.
        float scale = 1 + (player.position.y * perspectiveFactor);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
