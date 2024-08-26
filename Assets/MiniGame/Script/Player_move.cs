using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Player_move : MonoBehaviour
{
	public float maxSpeed;
	public float RuntrapRight;
	public float RuntrapLeft;
	public float Dashspeed;
	public Button ButtonDash;
	public Button ButtonSkill;
	public GameObject projectilePrefab;
	public Transform firePoint;
	public Slider healthBarSlider;
	public SceneAsset gameOverScene;
	public SceneAsset gameClearScene; // ���� Ŭ���� ��, ��Ӵٿ����� ����
	public GameObject gameOverUI; // ���� ���� �� ��Ÿ�� UI ������Ʈ
	public GameObject gameClearUI; // ���� Ŭ���� �� ��Ÿ�� UI ������Ʈ

	private string gameOverSceneName;
	private string gameClearSceneName; // ���� Ŭ���� �� �̸�

	public int maxHealth = 100;
	private int currentHealth;

	private Rigidbody2D rigid;
	private Animator animator;
	private bool facingRight = true;
	private bool isOnRightTrap = false;
	private bool isOnLeftTrap = false;
	private Camera mainCamera;
	private bool isOutOfView = false;
	private bool isDashing = false;
	private bool isSkillActive = false;
	private bool isTakingDamageFromTrap = false;
	private bool isInvincible = false;
	private bool isGameOver = false; // ���� ���� ���� Ȯ��

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		mainCamera = Camera.main;
		currentHealth = maxHealth;
		UpdateHealthBar();

		if (gameOverScene != null)
		{
			gameOverSceneName = gameOverScene.name;
		}

		if (gameClearScene != null)
		{
			gameClearSceneName = gameClearScene.name;
		}

		// ���� ���� �� Ŭ���� UI �ʱ�ȭ
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(false);
		}

		if (gameClearUI != null)
		{
			gameClearUI.SetActive(false);
		}
	}

	void Start()
	{
		ButtonDash.onClick.AddListener(OnDashButtonClicked);
		ButtonSkill.onClick.AddListener(OnSkillButtonClicked);
	}

	void FixedUpdate()
	{
		if (isGameOver) return; // ���� ���� �� �Է� ����

		float h = Input.GetAxisRaw("Horizontal");

		if (!isOnRightTrap && !isOnLeftTrap && !isDashing)
		{
			if (h != 0)
			{
				rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

				if (rigid.velocity.x > maxSpeed)
					rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
				else if (rigid.velocity.x < maxSpeed * (-1))
					rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
			}
			else
			{
				rigid.velocity = new Vector2(0, rigid.velocity.y);
			}
		}

		if (rigid.velocity.x > 0 && !facingRight)
		{
			Flip();
		}
		else if (rigid.velocity.x < 0 && facingRight)
		{
			Flip();
		}

		if (rigid.velocity.x != 0)
		{
			animator.SetBool("Move", true);
		}
		else
		{
			animator.SetBool("Move", false);
		}

		CheckIfOutOfView();

		if (isSkillActive)
		{
			FireProjectile();
			isSkillActive = false;
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
		if (collision.CompareTag("Enemy") && !isInvincible)
		{
			TakeDamage(10);
		}
		if (collision.CompareTag("Monster_Skill") && !isInvincible)
		{
			TakeDamage(5);
		}
		if (collision.CompareTag("damage trap"))
		{
			isTakingDamageFromTrap = true;
			if (!isInvincible)
			{
				StartCoroutine(TakeDamageOverTime(10, 1f));
			}
		}
		if (collision.CompareTag("End"))
		{
			// "End" �±׸� ���� ������Ʈ�� �浹 �� ���� Ŭ���� ó��
			StartCoroutine(HandleGameClear());
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
		if (collision.CompareTag("damage trap"))
		{
			isTakingDamageFromTrap = false;
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
				TakeDamage(20);
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
		if (!isDashing)
		{
			StartCoroutine(Dash());
		}
	}

	IEnumerator Dash()
	{
		isDashing = true;
		isInvincible = true;
		float dashDirection = facingRight ? 1 : -1;
		rigid.velocity = new Vector2(dashDirection * Dashspeed, rigid.velocity.y);
		yield return new WaitForSeconds(0.25f);
		isInvincible = false;
		isDashing = false;
	}

	void OnSkillButtonClicked()
	{
		if (!isSkillActive)
		{
			isSkillActive = true;
		}
	}

	void FireProjectile()
	{
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

		if (!facingRight)
		{
			Vector3 theScale = projectile.transform.localScale;
			theScale.x *= -1;
			projectile.transform.localScale = theScale;

			projectile.GetComponent<Projectile>().speed *= -1;
		}
	}

	void TakeDamage(int damage)
	{
		currentHealth -= damage;
		if (currentHealth <= 1)
		{
			currentHealth = 0;
			StartCoroutine(HandleGameOver()); // ���� ���� ó��
		}
		UpdateHealthBar();
	}

	IEnumerator HandleGameOver()
	{
		isGameOver = true;
		if (gameOverUI != null)
		{
			gameOverUI.SetActive(true); // ���� ���� UI ǥ��
		}
		yield return new WaitForSeconds(5f); // 5�� ���
		SceneManager.LoadScene(gameOverSceneName); // ���� ���� ������ ��ȯ
	}

	IEnumerator HandleGameClear()
	{
		isGameOver = true;
		if (gameClearUI != null)
		{
			gameClearUI.SetActive(true); // ���� Ŭ���� UI ǥ��
		}
		yield return new WaitForSeconds(5f); // 5�� ���
		SceneManager.LoadScene(gameClearSceneName); // ���� Ŭ���� ������ ��ȯ
	}

	void UpdateHealthBar()
	{
		healthBarSlider.value = (float)currentHealth / maxHealth;
	}

	IEnumerator TakeDamageOverTime(int damage, float interval)
	{
		while (isTakingDamageFromTrap)
		{
			TakeDamage(damage);
			yield return new WaitForSeconds(interval);
		}
	}
}
