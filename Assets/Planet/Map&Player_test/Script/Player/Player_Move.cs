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

	// �ִϸ��̼� ���� ����ȭ ����
	private bool isMoving;
	private float dirX;
	private float dirY;

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
			isMoving = !Mathf.Approximately(h, 0f) || !Mathf.Approximately(v, 0f);
			dirX = h;
			dirY = v;

			UpdateAnimation();
		}
		else
		{
			// ��Ʈ��ũ���� ������ ��ġ �� ȸ�� ����
			transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * Speed);
			rigid.velocity = networkVelocity;
			UpdateAnimation();
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
		anim.SetBool("isMoving", isMoving);
		anim.SetFloat("DirX", dirX);
		anim.SetFloat("DirY", dirY);

		// �ִϸ��̼� ���� ������Ʈ
		if (Mathf.Abs(dirX) + Mathf.Abs(dirY) == 0)
		{
			anim.SetBool("Up", false);
			anim.SetBool("Down", false);
			anim.SetBool("Right", false);
			anim.SetBool("Left", false);
		}
		else
		{
			anim.SetBool("Up", dirY > 0);
			anim.SetBool("Down", dirY < 0);
			anim.SetBool("Right", dirX > 0);
			anim.SetBool("Left", dirX < 0);
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
			stream.SendNext(isMoving); // �ִϸ��̼� ���� �۽�
			stream.SendNext(dirX); // �ִϸ��̼� ���� �۽�
			stream.SendNext(dirY); // �ִϸ��̼� ���� �۽�
		}
		else
		{
			// ������ ����
			networkPosition = (Vector3)stream.ReceiveNext();
			networkRotation = (Quaternion)stream.ReceiveNext();
			networkVelocity = (Vector2)stream.ReceiveNext();
			isMoving = (bool)stream.ReceiveNext(); // �ִϸ��̼� ���� ����
			dirX = (float)stream.ReceiveNext(); // �ִϸ��̼� ���� ����
			dirY = (float)stream.ReceiveNext(); // �ִϸ��̼� ���� ����

			// ��Ʈ��ũ ���� �ð� ���
			float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
			networkPosition += (Vector3)networkVelocity * lag;
		}
	}
}
