using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ĳ������ Transform
    public float moveSpeed = 5.0f; // ī�޶� ���󰡴� �ӵ�
    public Vector3 offset; // ĳ���ͷκ����� ������, Z�� ����

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset; // ĳ������ ���� ��ġ + ������
        desiredPosition.z = transform.position.z; // ī�޶��� Z���� ���� Z������ ����

        // �ε巴�� �̵��� �����ϵ�, �ӵ��� ���� ����
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, moveSpeed * Time.deltaTime);
    }
}
