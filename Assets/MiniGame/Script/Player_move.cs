using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_move : MonoBehaviour
{
	public float maxSpeed;
	public float RuntrapRight; // ���������� �о�� ������ �ӵ�
	public float RuntrapLeft;  // �������� �о�� ������ �ӵ�
	public float Dashspeed;    // �뽬 �ӵ�, �ܺο��� ���� ����
	public Button ButtonDash;  // �뽬 ��ư�� �����ϴ� ����, �ܺο��� �Ҵ�
	public Button ButtonSkill; // ��ų ��ư�� �����ϴ� ����, �ܺο��� �Ҵ�
	public GameObject projectilePrefab; // �߻�ü ������
	public Transform firePoint; // �߻�ü�� �߻�� ��ġ

	private Rigidbody2D rigid;
	private Animator animator;
	private bool facingRight = true;
	private bool isOnRightTrap = false;
	private bool isOnLeftTrap = false;
	private Camera mainCamera;
	private bool isOutOfView = false;
	private bool isDashing = false;
	private bool isSkillActive = false; // ��ų ��ư�� Ȱ��ȭ ����

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		mainCamera = Camera.main; // ���� ī�޶� ����
	}

	void Start()
	{
		// �뽬 ��ư Ŭ�� �� OnDashButtonClicked �޼��带 ȣ���ϵ��� ����
		ButtonDash.onClick.AddListener(OnDashButtonClicked);
		// ��ų ��ư Ŭ�� �� OnSkillButtonClicked �޼��带 ȣ���ϵ��� ����
		ButtonSkill.onClick.AddListener(OnSkillButtonClicked);
	}

	void FixedUpdate()
	{
		// Move By Key Control
		float h = Input.GetAxisRaw("Horizontal");

		// �÷��̾ � ���� ������ ���� ���� ���� ����Ű�� �̵� ����
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

		// �÷��̾��� �̵� �ӵ��� ���� ��������Ʈ ����
		if (rigid.velocity.x > 0 && !facingRight)
		{
			Flip();
		}
		else if (rigid.velocity.x < 0 && facingRight)
		{
			Flip();
		}

		// �̵� �ӵ��� ���� �ִϸ��̼� ����
		if (rigid.velocity.x != 0)
		{
			animator.SetBool("Move", true);
		}
		else
		{
			animator.SetBool("Move", false);
		}

		// �÷��̾ ī�޶��� �þ� ������ �������� Ȯ��
		CheckIfOutOfView();

		// ��ų Ȱ��ȭ ���ο� ���� ���Ÿ� ���� �߻�
		if (isSkillActive)
		{
			FireProjectile();
			isSkillActive = false; // ��ų ��� �� ��Ȱ��ȭ
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
		if (!isDashing) // �뽬 ���� �ƴ� ���� �뽬 ����
		{
			StartCoroutine(Dash());
		}
	}

	IEnumerator Dash()
	{
		isDashing = true; // �뽬 ����
		float dashDirection = facingRight ? 1 : -1; // ���� ���⿡ ���� �뽬 ���� ����
		rigid.velocity = new Vector2(dashDirection * Dashspeed, rigid.velocity.y); // �뽬 �ӵ� ����
		yield return new WaitForSeconds(1f); // 1�� ���� �뽬 ����
		isDashing = false; // �뽬 ����
	}

	void OnSkillButtonClicked()
	{
		isSkillActive = true; // ��ų ��ư Ŭ�� �� ��ų Ȱ��ȭ
	}

	void FireProjectile()
	{
		// �߻�ü ����
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

		// �߻�ü ���� ����
		if (!facingRight)
		{
			Vector3 theScale = projectile.transform.localScale;
			theScale.x *= -1;
			projectile.transform.localScale = theScale;

			// �߻�ü�� �������� �̵��ϵ��� �ӵ� ����
			projectile.GetComponent<Projectile>().speed *= -1;
		}
	}
}
