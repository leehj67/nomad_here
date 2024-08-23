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

	// Player_Skill 태그가 붙은 오브젝트와 충돌 시 몬스터 삭제
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player_Skill"))
		{
			Destroy(gameObject); // 몬스터 오브젝트 삭제
		}
	}
}
