using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
	public float speed = 5.0f; // Speed of the monster, adjustable in Unity's inspector
	private Vector2 moveDirection; // Current movement direction
	private float directionChangeInterval = 1.5f; // Interval to change direction
	private Rigidbody2D rb; // Reference to the Rigidbody component

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		StartCoroutine(ChangeDirectionRoutine());
	}

	void Update()
	{
		rb.velocity = moveDirection * speed; // Apply movement direction and speed
	}

	IEnumerator ChangeDirectionRoutine()
	{
		while (true)
		{
			moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // Set random new direction
			yield return new WaitForSeconds(directionChangeInterval); // Wait for the next direction change
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Wall") // Check if the collision is with a wall
		{
			StartCoroutine(AvoidWall(collision));
		}
	}

	IEnumerator AvoidWall(Collision2D collision)
	{
		Vector2 originalDirection = moveDirection;
		Vector2 backDirection = -originalDirection; // Calculate backward direction

		// Move backward briefly
		moveDirection = backDirection;
		yield return new WaitForSeconds(0.5f);

		// Try moving in a new direction, perpendicular to the original
		moveDirection = new Vector2(-originalDirection.y, originalDirection.x); // Rotate 90 degrees
		yield return new WaitForSeconds(1.0f);

		// Check if still colliding
		if (collision.collider != null)
		{
			StartCoroutine(AvoidWall(collision)); // If still colliding, repeat the process
		}
		else
		{
			StartCoroutine(ChangeDirectionRoutine()); // Otherwise, go back to normal behavior
		}
	}
}
