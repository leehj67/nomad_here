using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item_type_01 : MonoBehaviourPun, IPunObservable
{
	void Start()
	{
		// 0.5초마다 충돌 상태 확인
		StartCoroutine(CheckForPlayerPickup());
	}

	IEnumerator CheckForPlayerPickup()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);

			// 현재 오브젝트의 콜라이더와 충돌한 콜라이더들을 저장할 배열
			Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0);

			foreach (Collider2D collider in colliders)
			{
				if (collider.CompareTag("Player_itempickup"))
				{
					// 아이템 오브젝트 삭제
					PhotonNetwork.Destroy(gameObject);
					break;
				}
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			// 데이터 송신
			stream.SendNext(transform.position);
		}
		else
		{
			// 데이터 수신
			transform.position = (Vector3)stream.ReceiveNext();
		}
	}
}
