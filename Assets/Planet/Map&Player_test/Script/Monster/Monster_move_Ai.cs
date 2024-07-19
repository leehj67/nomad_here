using System.Collections;
using UnityEngine;

public class MonsterMoveAI : MonoBehaviour
{
	public float moveSpeed = 5.0f; // ������ �̵� �ӵ�, ����Ƽ �ν����Ϳ��� ���� ����
	private Rigidbody2D rb; // Rigidbody2D ������Ʈ ����
	private Vector2 movementDirection; // ������ ���� �̵� ����

	void Start()
	{
		rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ������Ʈ ��������
		rb.gravityScale = 0; // �߷� �������� 0���� �����Ͽ� ���� ����
		StartCoroutine(ChangeDirectionRoutine()); // ���� ���� �ڷ�ƾ ����
	}

	void Update()
	{
		MoveMonster(); // �� �����Ӹ��� ���� �̵�
	}

	private void MoveMonster()
	{
		rb.velocity = movementDirection * moveSpeed; // �̵� ����
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
}
