using System.Collections;
using UnityEngine;
using Photon.Pun;

public class MonsterMoveAI : MonoBehaviourPun, IPunObservable
{
	public float moveSpeed = 5.0f; // 몬스터의 이동 속도, 유니티 인스펙터에서 조정 가능
	private Rigidbody2D rb; // Rigidbody2D 컴포넌트 참조
	private Vector2 movementDirection; // 몬스터의 현재 이동 방향
	private Vector2 networkPosition; // 네트워크를 통해 동기화할 위치
	private Vector2 networkMovementDirection; // 네트워크를 통해 동기화할 이동 방향

	void Start()
	{
		rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
		rb.gravityScale = 0; // 중력 스케일을 0으로 설정하여 낙하 방지

		if (photonView.IsMine)
		{
			StartCoroutine(ChangeDirectionRoutine()); // 방향 변경 코루틴 시작
		}
	}

	void Update()
	{
		if (photonView.IsMine)
		{
			MoveMonster(); // 매 프레임마다 몬스터 이동
		}
		else
		{
			SmoothMove(); // 네트워크 동기화 위치로 부드럽게 이동
		}
	}

	private void MoveMonster()
	{
		rb.velocity = movementDirection * moveSpeed; // 이동 적용
	}

	private void SmoothMove()
	{
		rb.position = Vector2.MoveTowards(rb.position, networkPosition, moveSpeed * Time.deltaTime);
		rb.velocity = networkMovementDirection * moveSpeed;
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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// 데이터 송신
			stream.SendNext(rb.position);
			stream.SendNext(movementDirection);
		}
		else
		{
			// 데이터 수신
			networkPosition = (Vector2)stream.ReceiveNext();
			networkMovementDirection = (Vector2)stream.ReceiveNext();
		}
	}
}
