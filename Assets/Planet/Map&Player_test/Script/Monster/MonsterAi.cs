using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAi : MonoBehaviour
{
	public float speed = 5.0f;
	private Rigidbody2D rb;
	private Vector2 movementDirection;
	private Vector2 nextMovementPoint;
	private float changeDirectionTime = 0;
	public Transform player;
	public float detectionRange = 10.0f;
	private bool isChasingPlayer = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		rb.constraints = RigidbodyConstraints2D.FreezeRotation;
		ChangeDirection();
	}

	void Update()
	{
		float distanceToPlayer = Vector2.Distance(rb.position, player.position);
		if (distanceToPlayer <= detectionRange)
		{
			isChasingPlayer = true;
		}
		else
		{
			isChasingPlayer = false;
		}

		if (!isChasingPlayer && Time.time >= changeDirectionTime)
		{
			ChangeDirection();
		}
	}

	void FixedUpdate()
	{
		if (isChasingPlayer)
		{
			ChasePlayer();
		}
		else
		{
			rb.MovePosition(Vector2.MoveTowards(rb.position, nextMovementPoint, speed * Time.fixedDeltaTime));
		}
	}

	void ChangeDirection()
	{
		changeDirectionTime = Time.time + Random.Range(1f, 5f);
		float angle = Random.Range(0, 2 * Mathf.PI);
		movementDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
		nextMovementPoint = rb.position + movementDirection * 10;
	}

	void ChasePlayer()
	{
		nextMovementPoint = player.position;
		rb.MovePosition(Vector2.MoveTowards(rb.position, nextMovementPoint, speed * Time.fixedDeltaTime));
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "wall")
		{
			// Rotate the current movement direction by 90 degrees
			movementDirection = new Vector2(-movementDirection.y, movementDirection.x); // Rotate 90 degrees clockwise
			nextMovementPoint = rb.position + movementDirection * 10;
		}
	}
}
