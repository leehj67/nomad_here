using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
	public float Speed;
	Rigidbody2D rigid;
	Animator anim;
	float h;
	float v;
	bool isHorizonMove;
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();

		// 중력의 영향을 받지 않도록 설정
		rigid.gravityScale = 0;

		// X축과 Y축의 회전을 고정
		rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
	}
	void Update()
	{
		h = Input.GetAxisRaw("Horizontal");
		v = Input.GetAxisRaw("Vertical");
		bool hDown = Input.GetButtonDown("Horizontal");
		bool vDown = Input.GetButtonDown("Vertical");
		bool hUp = Input.GetButtonUp("Horizontal");
		bool vUp = Input.GetButtonUp("Vertical");
		if (hDown)
			isHorizonMove = true;
		else if (vDown)
			isHorizonMove = false;
		else if (hUp || vUp)
		{
			if (Mathf.Abs(h) > 0)
				isHorizonMove = true;
			else if (Mathf.Abs(v) > 0)
				isHorizonMove = false;
		}
		if (Mathf.Abs(h) + Mathf.Abs(v) == 0)
			anim.SetBool("isMoving", false);
		else
			anim.SetBool("isMoving", true);

		anim.SetFloat("DirX", h);
		anim.SetFloat("DirY", v);

		UpdateAnimation();
	}
	void FixedUpdate()
	{
		Vector2 moveVec = new Vector2(h, v).normalized;
		rigid.velocity = moveVec * Speed;
	}
	void UpdateAnimation()
	{
		if (Mathf.Abs(h) + Mathf.Abs(v) == 0)
		{
			anim.SetBool("Up", false);
			anim.SetBool("Down", false);
			anim.SetBool("Right", false);
			anim.SetBool("Left", false);
		}
		else
		{
			if (v > 0)
			{
				anim.SetBool("Up", true);
				anim.SetBool("Down", false);
				anim.SetBool("Right", false);
				anim.SetBool("Left", false);
			}
			else if (v < 0)
			{
				anim.SetBool("Up", false);
				anim.SetBool("Down", true);
				anim.SetBool("Right", false);
				anim.SetBool("Left", false);
			}
			else if (h > 0)
			{
				anim.SetBool("Up", false);
				anim.SetBool("Down", false);
				anim.SetBool("Right", true);
				anim.SetBool("Left", false);
			}
			else if (h < 0)
			{
				anim.SetBool("Up", false);
				anim.SetBool("Down", false);
				anim.SetBool("Right", false);
				anim.SetBool("Left", true);
			}
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
				newPosition.x += 1; // X좌표를 +1 이동
				transform.position = newPosition;
			}
		}
	}
}
