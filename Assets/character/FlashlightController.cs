using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform을 참조
    public Vector3 offset; // 라이트의 위치 오프셋

    private Light2D flashlight; // 스포트 라이트

    void Start()
    {
        flashlight = GetComponent<Light2D>();
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어의 위치에 따라 라이트 위치 조정
            transform.position = player.position + offset;

            // 플레이어의 방향에 따라 라이트 회전 조정
            if (player.localScale.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // 오른쪽
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // 왼쪽
            }
        }
    }
}
