using System.Collections;
using UnityEngine;
using Photon.Pun;

public class MonsterMoveAI : MonoBehaviourPun, IPunObservable
{
	public float moveSpeed = 5.0f; // ������ �̵� �ӵ�, ����Ƽ �ν����Ϳ��� ���� ����
	private Rigidbody2D rb; // Rigidbody2D ������Ʈ ����
	private Vector2 movementDirection; // ������ ���� �̵� ����
	private Vector2 networkPosition; // ��Ʈ��ũ�� ���� ����ȭ�� ��ġ
	private Vector2 networkMovementDirection; // ��Ʈ��ũ�� ���� ����ȭ�� �̵� ����

	void Start()
	{
		rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
		rb.gravityScale = 0; // �߷� �������� 0���� �����Ͽ� ���� ����

		if (photonView.IsMine)
		{
			StartCoroutine(ChangeDirectionRoutine()); // ���� ���� �ڷ�ƾ ����
		}
	}

	void Update()
	{
		if (photonView.IsMine)
		{
			MoveMonster(); // �� �����Ӹ��� ���� �̵�
		}
		else
		{
			SmoothMove(); // ��Ʈ��ũ ����ȭ ��ġ�� �ε巴�� �̵�
		}
	}

	private void MoveMonster()
	{
		rb.velocity = movementDirection * moveSpeed; // �̵� ����
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
			movementDirection = Random.insideUnitCircle.normalized; // ���� ����ȭ�� ���� ����
			float changeInterval = Random.Range(1.0f, 2.0f); // 1�ʿ��� 2�� ������ ���� ���͹�
			yield return new WaitForSeconds(changeInterval); // ���� ���͹���ŭ ��� �� �ٽ� ���� ����
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// ������ �۽�
			stream.SendNext(rb.position);
			stream.SendNext(movementDirection);
		}
		else
		{
			// ������ ����
			networkPosition = (Vector2)stream.ReceiveNext();
			networkMovementDirection = (Vector2)stream.ReceiveNext();
		}
	}
}
