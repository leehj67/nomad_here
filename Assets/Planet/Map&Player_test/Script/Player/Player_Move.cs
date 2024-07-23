using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player_Move : MonoBehaviourPun, IPunObservable
{
	public float Speed;
	public Joystick joystick; // 조이스틱 변수 추가
	Rigidbody2D rigid;
	Animator anim;
	float h;
	float v;
	bool isHorizonMove;

	// 네트워크 동기화 변수
	private Vector3 networkPosition;
	private Quaternion networkRotation;
	private Vector2 networkVelocity;
	private float lag;

	// 애니메이션 상태 동기화 변수
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
			// 키보드 입력 받기
			h = Input.GetAxisRaw("Horizontal") + joystick.Horizontal; // 조이스틱 입력 추가
			v = Input.GetAxisRaw("Vertical") + joystick.Vertical; // 조이스틱 입력 추가

			// 이동 방향 결정
			isHorizonMove = Mathf.Abs(h) > Mathf.Abs(v);

			// 이동 상태 및 방향 애니메이션 설정
			isMoving = !Mathf.Approximately(h, 0f) || !Mathf.Approximately(v, 0f);
			dirX = h;
			dirY = v;

			UpdateAnimation();
		}
		else
		{
			// 네트워크에서 수신한 위치 및 회전 적용
			transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * Speed);
			rigid.velocity = networkVelocity;
			UpdateAnimation();
		}
	}

	void FixedUpdate()
	{
		if (photonView.IsMine)
		{
			// 이동 수행
			Vector2 moveVec = new Vector2(h, v).normalized;
			rigid.velocity = moveVec * Speed;
		}
	}

	void UpdateAnimation()
	{
		anim.SetBool("isMoving", isMoving);
		anim.SetFloat("DirX", dirX);
		anim.SetFloat("DirY", dirY);

		// 애니메이션 상태 업데이트
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
				newPosition.x += 1; // X좌표를 1만큼 이동
				transform.position = newPosition;
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// 데이터 송신
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(rigid.velocity);
			stream.SendNext(isMoving); // 애니메이션 상태 송신
			stream.SendNext(dirX); // 애니메이션 상태 송신
			stream.SendNext(dirY); // 애니메이션 상태 송신
		}
		else
		{
			// 데이터 수신
			networkPosition = (Vector3)stream.ReceiveNext();
			networkRotation = (Quaternion)stream.ReceiveNext();
			networkVelocity = (Vector2)stream.ReceiveNext();
			isMoving = (bool)stream.ReceiveNext(); // 애니메이션 상태 수신
			dirX = (float)stream.ReceiveNext(); // 애니메이션 상태 수신
			dirY = (float)stream.ReceiveNext(); // 애니메이션 상태 수신

			// 네트워크 지연 시간 계산
			float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
			networkPosition += (Vector3)networkVelocity * lag;
		}
	}
}
