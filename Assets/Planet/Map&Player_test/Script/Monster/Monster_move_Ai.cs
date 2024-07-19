using System.Collections;
using UnityEngine;

public class MonsterMoveAI : MonoBehaviour
{
	public float moveSpeed = 5.0f; // 몬스터의 이동 속도, 유니티 인스펙터에서 조정 가능
	private Rigidbody2D rb; // Rigidbody2D 컴포넌트 참조
	private Vector2 movementDirection; // 몬스터의 현재 이동 방향

	void Start()
	{
		rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
		rb.gravityScale = 0; // 중력 스케일을 0으로 설정하여 낙하 방지
		StartCoroutine(ChangeDirectionRoutine()); // 방향 변경 코루틴 시작
	}

	void Update()
	{
		MoveMonster(); // 매 프레임마다 몬스터 이동
	}

	private void MoveMonster()
	{
		rb.velocity = movementDirection * moveSpeed; // 이동 적용
	}

	private IEnumerator ChangeDirectionRoutine()
	{
		while (true)
		{
			movementDirection = Random.insideUnitCircle.normalized; // 랜덤 정규화된 방향 선택
			float changeInterval = Random.Range(1.0f, 2.0f); // 1초에서 2초 사이의 랜덤 인터벌
			yield return new WaitForSeconds(changeInterval); // 랜덤 인터벌만큼 대기 후 다시 방향 변경
		}
	}
}
