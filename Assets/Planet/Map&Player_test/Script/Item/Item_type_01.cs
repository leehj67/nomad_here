using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item_type_01 : MonoBehaviourPun, IPunObservable
{
	void Start()
	{
		// 0.5�ʸ��� �浹 ���� Ȯ��
		StartCoroutine(CheckForPlayerPickup());
	}

	IEnumerator CheckForPlayerPickup()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);

			// ���� ������Ʈ�� �ݶ��̴��� �浹�� �ݶ��̴����� ������ �迭
			Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0);

			foreach (Collider2D collider in colliders)
			{
				if (collider.CompareTag("Player_itempickup"))
				{
					// ������ ������Ʈ ����
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
			// ������ �۽�
			stream.SendNext(transform.position);
		}
		else
		{
			// ������ ����
			transform.position = (Vector3)stream.ReceiveNext();
		}
	}
}
