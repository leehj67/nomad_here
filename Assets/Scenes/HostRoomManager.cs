using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HostRoomManager : MonoBehaviourPunCallbacks
{
    public Button startGameButton;
    public GameObject gameStateManagerPrefab;
    public GameObject eventManagerPrefab;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateSingleton(gameStateManagerPrefab, GameStateManager.Instance);
            InstantiateSingleton(eventManagerPrefab, EventManager.Instance);

            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    private void InstantiateSingleton(GameObject prefab, MonoBehaviour instance)
    {
        if (instance == null)
        {
            GameObject manager = PhotonNetwork.Instantiate(prefab.name, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(manager);
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    public override void OnLeftRoom()
    {
        startGameButton.gameObject.SetActive(false);
    }
}
