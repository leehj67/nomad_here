using System.Collections;
using UnityEngine;

public class Wallcheck : MonoBehaviour
{
	public bool Wallimpact = false;  // 벽과의 충돌 상태

	void Update()
	{
		if (Wallimpact) // Wallimpact 값이 참인 경우 메시지 출력
		{
			Debug.Log("벽과 충돌");
		}
	}

	// 벽과 지속적으로 충돌 중일 때 호출됩니다.
	void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			if (!Wallimpact) // Wallimpact 상태가 변경될 때만 메시지를 출력하도록 조건 추가
			{
				Debug.Log("벽과 충돌");
			}
			Wallimpact = true;  // 벽과 충돌 중이면 Wallimpact 값을 true로 설정
		}
	}

	// 벽과의 충돌이 끝났을 때 호출됩니다.
	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Wall"))
		{
			Wallimpact = false;  // 벽과의 충돌이 끝나면 Wallimpact 값을 false로 설정
			Debug.Log("벽과의 충돌 종료");  // 벽과의 충돌이 종료되었을 때 메시지 출력
		}
	}
}
