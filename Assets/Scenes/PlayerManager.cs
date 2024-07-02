using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject playerUI; // 플레이어 UI 프리팹
    public Transform startPos; // 시작 위치

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab != null)
            {
                // 시작 위치에서 플레이어 생성
                Vector3 spawnPosition = startPos != null ? startPos.position : new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
                spawnPosition.z = -1; // Z 값을 -1로 설정
                GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

                // UI 생성
                if (playerUI != null)
                {
                    GameObject uiInstance = Instantiate(playerUI);
                    // UI 인스턴스에 플레이어를 할당합니다. 필요한 경우 PlayerPlanetMove 스크립트에 UI를 할당하는 메서드를 추가하세요.
                    PlayerPlanetMove playerMove = player.GetComponent<PlayerPlanetMove>();
                    if (playerMove != null)
                    {
                        playerMove.InitializeUI(uiInstance);
                    }
                }
            }
        }
    }
}
