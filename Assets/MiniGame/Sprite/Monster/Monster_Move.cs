using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Move : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	// Player_Skill �±װ� ���� ������Ʈ�� �浹 �� ���� ����
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player_Skill"))
		{
			Destroy(gameObject); // ���� ������Ʈ ����
		}
	}
}
