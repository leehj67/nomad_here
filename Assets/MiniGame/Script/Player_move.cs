using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_move : MonoBehaviour
{
	public float maxSpeed;
	public float RuntrapRight; // 오른쪽으로 밀어내는 함정의 속도
	public float RuntrapLeft;  // 왼쪽으로 밀어내는 함정의 속도
	public float Dashspeed;    // 대쉬 속도, 외부에서 설정 가능
	public Button ButtonDash;  // 대쉬 버튼을 참조하는 변수, 외부에서 할당
	public Button ButtonSkill; // 스킬 버튼을 참조하는 변수, 외부에서 할당
	public GameObject projectilePrefab; // 발사체 프리팹
	public Transform firePoint; // 발사체가 발사될 위치

	private Rigidbody2D rigid;
	private Animator animator;
	private bool facingRight = true;
	private bool isOnRightTrap = false;
	private bool isOnLeftTrap = false;
	private Camera mainCamera;
	private bool isOutOfView = false;
	private bool isDashing = false;
	private bool isSkillActive = false; // 스킬 버튼의 활성화 상태

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		mainCamera = Camera.main; // 메인 카메라를 참조
	}

	void Start()
	{
		// 대쉬 버튼 클릭 시 OnDashButtonClicked 메서드를 호출하도록 설정
		ButtonDash.onClick.AddListener(OnDashButtonClicked);
		// 스킬 버튼 클릭 시 OnSkillButtonClicked 메서드를 호출하도록 설정
		ButtonSkill.onClick.AddListener(OnSkillButtonClicked);
	}

	void FixedUpdate()
	{
		// Move By Key Control
		float h = Input.GetAxisRaw("Horizontal");

		// 플레이어가 어떤 함정 위에도 있지 않을 때만 방향키로 이동 제어
		if (!isOnRightTrap && !isOnLeftTrap && !isDashing)
		{
			if (h != 0)
			{
				rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

				if (rigid.velocity.x > maxSpeed) // Right Max Speed
					rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
				else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
					rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
			}
			else
			{
				rigid.velocity = new Vector2(0, rigid.velocity.y);
			}
		}

		// 플레이어의 이동 속도에 따라 스프라이트 반전
		if (rigid.velocity.x > 0 && !facingRight)
		{
			Flip();
		}
		else if (rigid.velocity.x < 0 && facingRight)
		{
			Flip();
		}

		// 이동 속도에 따른 애니메이션 제어
		if (rigid.velocity.x != 0)
		{
			animator.SetBool("Move", true);
		}
		else
		{
			animator.SetBool("Move", false);
		}

		// 플레이어가 카메라의 시야 밖으로 나갔는지 확인
		CheckIfOutOfView();

		// 스킬 활성화 여부에 따라 원거리 공격 발사
		if (isSkillActive)
		{
			FireProjectile();
			isSkillActive = false; // 스킬 사용 후 비활성화
		}
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("RunRight"))
		{
			isOnRightTrap = true;
		}
		if (collision.CompareTag("RunLeft"))
		{
			isOnLeftTrap = true;
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("RunRight"))
		{
			isOnRightTrap = false;
		}
		if (collision.CompareTag("RunLeft"))
		{
			isOnLeftTrap = false;
		}
	}

	void Update()
	{
		if (isOnRightTrap)
		{
			rigid.velocity = new Vector2(RuntrapRight, rigid.velocity.y);
		}
		if (isOnLeftTrap)
		{
			rigid.velocity = new Vector2(-RuntrapLeft, rigid.velocity.y);
		}
	}

	void CheckIfOutOfView()
	{
		Vector3 playerPos = transform.position;
		Vector3 viewPos = mainCamera.WorldToViewportPoint(playerPos);

		if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
		{
			if (!isOutOfView)
			{
				isOutOfView = true;
				MovePlayerToCenter();
				mainCamera.GetComponent<Camera_Move>().StopCameraForSeconds(2f);
			}
		}
		else
		{
			isOutOfView = false;
		}
	}

	void MovePlayerToCenter()
	{
		Vector3 cameraCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane));
		transform.position = new Vector3(cameraCenter.x, cameraCenter.y, transform.position.z);
	}

	void OnDashButtonClicked()
	{
		if (!isDashing) // 대쉬 중이 아닐 때만 대쉬 실행
		{
			StartCoroutine(Dash());
		}
	}

	IEnumerator Dash()
	{
		isDashing = true; // 대쉬 시작
		float dashDirection = facingRight ? 1 : -1; // 현재 방향에 따른 대쉬 방향 설정
		rigid.velocity = new Vector2(dashDirection * Dashspeed, rigid.velocity.y); // 대쉬 속도 설정
		yield return new WaitForSeconds(1f); // 1초 동안 대쉬 유지
		isDashing = false; // 대쉬 종료
	}

	void OnSkillButtonClicked()
	{
		isSkillActive = true; // 스킬 버튼 클릭 시 스킬 활성화
	}

	void FireProjectile()
	{
		// 발사체 생성
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

		// 발사체 방향 설정
		if (!facingRight)
		{
			Vector3 theScale = projectile.transform.localScale;
			theScale.x *= -1;
			projectile.transform.localScale = theScale;

			// 발사체가 왼쪽으로 이동하도록 속도 조정
			projectile.GetComponent<Projectile>().speed *= -1;
		}
	}
}
