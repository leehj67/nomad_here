using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemController : MonoBehaviourPun
{
	public GameObject itemPrefab; // 아이템 프리펩
	private Vector2 spawnPos = new Vector2(-5.5f, 0.4f);

	void Start()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			// Since it's a 2D game, ensure that the prefab is a 2D GameObject and that it has no unnecessary 3D rotation
			GameObject item = PhotonNetwork.Instantiate(itemPrefab.name, spawnPos, Quaternion.identity); // Quaternion.identity represents no rotation
			item.transform.parent = this.transform; // 아이템을 스포너의 자식으로 설정
		}
	}
}