using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour
{
	public float moveSpeed = 5f; // 카메라 이동 속도, 유니티 에디터에서 설정 가능
	private bool isMoving = true; // 카메라가 이동 중인지 확인하는 변수
	private Camera mainCamera;

	void Start()
	{
		mainCamera = Camera.main; // 메인 카메라를 참조
	}

	// Update는 매 프레임마다 호출됩니다.
	void Update()
	{
		if (isMoving)
		{
			// 카메라를 Y축 방향으로 하강
			transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
		}
	}

	// 플레이어가 시야 밖으로 나갔을 때 이동을 멈추고, 2초 후 다시 이동을 재개
	public void StopCameraForSeconds(float seconds)
	{
		StartCoroutine(StopCameraCoroutine(seconds));
	}

	IEnumerator StopCameraCoroutine(float seconds)
	{
		isMoving = false; // 카메라 이동 멈춤
		yield return new WaitForSeconds(seconds); // 지정된 시간 동안 대기
		isMoving = true; // 카메라 이동 재개
	}
}
