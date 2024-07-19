using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
	public GameObject obstaclePrefab; // Consider renaming this to monsterPrefab for clarity
	private Vector2 spawnPos = new Vector2(-5.5f, 0.4f);

	void Start()
	{
		// Since it's a 2D game, ensure that the prefab is a 2D GameObject and that it has no unnecessary 3D rotation
		Instantiate(obstaclePrefab, spawnPos, Quaternion.identity); // Quaternion.identity represents no rotation
	}
}
