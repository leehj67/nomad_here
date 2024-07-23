using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterSpawner : MonoBehaviourPun
{
	public GameObject monsterPrefab; // ���� ������
	private Vector2 spawnPos = new Vector2(-5.5f, 0.4f);

	void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			// Since it's a 2D game, ensure that the prefab is a 2D GameObject and that it has no unnecessary 3D rotation
			GameObject monster = PhotonNetwork.Instantiate(monsterPrefab.name, spawnPos, Quaternion.identity); // Quaternion.identity represents no rotation
			monster.transform.parent = this.transform; // ���͸� �������� �ڽ����� ����
		}
	}
}
