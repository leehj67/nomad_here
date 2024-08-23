using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour
{
	public float moveSpeed = 5f; // ī�޶� �̵� �ӵ�, ����Ƽ �����Ϳ��� ���� ����
	private bool isMoving = true; // ī�޶� �̵� ������ Ȯ���ϴ� ����
	private Camera mainCamera;

	void Start()
	{
		mainCamera = Camera.main; // ���� ī�޶� ����
	}

	// Update�� �� �����Ӹ��� ȣ��˴ϴ�.
	void Update()
	{
		if (isMoving)
		{
			// ī�޶� Y�� �������� �ϰ�
			transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
		}
	}

	// �÷��̾ �þ� ������ ������ �� �̵��� ���߰�, 2�� �� �ٽ� �̵��� �簳
	public void StopCameraForSeconds(float seconds)
	{
		StartCoroutine(StopCameraCoroutine(seconds));
	}

	IEnumerator StopCameraCoroutine(float seconds)
	{
		isMoving = false; // ī�޶� �̵� ����
		yield return new WaitForSeconds(seconds); // ������ �ð� ���� ���
		isMoving = true; // ī�޶� �̵� �簳
	}
}
