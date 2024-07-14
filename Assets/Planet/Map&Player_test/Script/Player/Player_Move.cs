using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
	public float Speed;
	public VariableJoystick joy;
	Rigidbody2D rigid;
	Animator anim;
	float h;
	float v;
	bool isHorizonMove;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		rigid.gravityScale = 0;
		rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void Update()
	{
		// �Է� ���� �ֽ� ���·� ������Ʈ
		h = joy.Horizontal;
		v = joy.Vertical;

		// ���̽�ƽ �Է��� ���� �� Ű���� �Է� �ޱ�
		if (Mathf.Approximately(h, 0f)) h = Input.GetAxisRaw("Horizontal");
		if (Mathf.Approximately(v, 0f)) v = Input.GetAxisRaw("Vertical");

		// �̵� ���� ����
		isHorizonMove = Mathf.Abs(h) > Mathf.Abs(v);

		// �̵� ���� �� ���� �ִϸ��̼� ����
		anim.SetBool("isMoving", !Mathf.Approximately(h, 0f) || !Mathf.Approximately(v, 0f));
		anim.SetFloat("DirX", h);
		anim.SetFloat("DirY", v);

		UpdateAnimation();
	}

	void FixedUpdate()
	{
		// �̵� ����
		Vector2 moveVec = new Vector2(h, v).normalized;
		rigid.velocity = moveVec * Speed;
	}

	void UpdateAnimation()
	{
		// �ִϸ��̼� ���� ������Ʈ
		if (Mathf.Abs(h) + Mathf.Abs(v) == 0)
		{
			anim.SetBool("Up", false);
			anim.SetBool("Down", false);
			anim.SetBool("Right", false);
			anim.SetBool("Left", false);
		}
		else
		{
			anim.SetBool("Up", v > 0);
			anim.SetBool("Down", v < 0);
			anim.SetBool("Right", h > 0);
			anim.SetBool("Left", h < 0);
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Door_01"))
		{
			GameObject door02 = GameObject.FindGameObjectWithTag("Door_02");
			if (door02 != null)
			{
				Vector3 newPosition = door02.transform.position;
				newPosition.x += 1; // X��ǥ�� 1��ŭ �̵�
				transform.position = newPosition;
			}
		}
	}
}
