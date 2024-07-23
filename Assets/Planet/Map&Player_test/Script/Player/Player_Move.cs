using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Move : MonoBehaviourPun, IPunObservable
{
	public float Speed;
	public Joystick joystick; // ���̽�ƽ ���� �߰�
	Rigidbody2D rigid;
	Animator anim;
	float h;
	float v;
	bool isHorizonMove;

	// ��Ʈ��ũ ����ȭ ����
	private Vector3 networkPosition;
	private Quaternion networkRotation;
	private Vector2 networkVelocity;
	private float lag;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		rigid.gravityScale = 0;
		rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	void Update()
	{
		if (photonView.IsMine)
		{
			// Ű���� �Է� �ޱ�
			h = Input.GetAxisRaw("Horizontal") + joystick.Horizontal; // ���̽�ƽ �Է� �߰�
			v = Input.GetAxisRaw("Vertical") + joystick.Vertical; // ���̽�ƽ �Է� �߰�

			// �̵� ���� ����
			isHorizonMove = Mathf.Abs(h) > Mathf.Abs(v);

			// �̵� ���� �� ���� �ִϸ��̼� ����
			anim.SetBool("isMoving", !Mathf.Approximately(h, 0f) || !Mathf.Approximately(v, 0f));
			anim.SetFloat("DirX", h);
			anim.SetFloat("DirY", v);

			UpdateAnimation();
		}
		else
		{
			// ��Ʈ��ũ���� ������ ��ġ �� ȸ�� ����
			transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * Speed);
			rigid.velocity = networkVelocity;
		}
	}

	void FixedUpdate()
	{
		if (photonView.IsMine)
		{
			// �̵� ����
			Vector2 moveVec = new Vector2(h, v).normalized;
			rigid.velocity = moveVec * Speed;
		}
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
		if (collision.CompareTag("Door_01") && photonView.IsMine)
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

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// ������ �۽�
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(rigid.velocity);
		}
		else
		{
			// ������ ����
			networkPosition = (Vector3)stream.ReceiveNext();
			networkRotation = (Quaternion)stream.ReceiveNext();
			networkVelocity = (Vector2)stream.ReceiveNext();

			// ��Ʈ��ũ ���� �ð� ���
			float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
			networkPosition += (Vector3)networkVelocity * lag;
		}
	}
}
