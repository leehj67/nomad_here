using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightController : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform�� ����
    public Vector3 offset; // ����Ʈ�� ��ġ ������

    private Light2D flashlight; // ����Ʈ ����Ʈ

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
            // �÷��̾��� ��ġ�� ���� ����Ʈ ��ġ ����
            transform.position = player.position + offset;

            // �÷��̾��� ���⿡ ���� ����Ʈ ȸ�� ����
            if (player.localScale.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // ������
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // ����
            }
        }
    }
}
