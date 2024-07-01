using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace MyGameNamespace
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }
        public Vector3 PlayerStartPosition { get; private set; }
        public int[] partHealth = new int[4]; // 각 파츠의 건강 상태 (최대 4개의 파츠)

        [System.Serializable]
        public class PlanetInfo
        {
            public string Risk { get; private set; }
            public int Asset { get; private set; }

            public PlanetInfo(string risk, int asset)
            {
                Risk = risk;
                Asset = asset;
            }
        }

        public PlanetInfo[] planetsInfo; // 각 행성의 정보를 저장하는 배열
        public int SelectedPlanetIndex { get; set; } // 선택된 행성 인덱스를 저장하는 변수
        public string[] planetSceneNames; // 각 행성 씬의 이름을 저장하는 배열
        public GameObject playerPrefab; // 플레이어 프리팹
        public Transform[] spawnPoints; // 스폰 포인트 배열

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
                InitializePartHealth(); // 초기 건강 상태 설정
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializePartHealth()
        {
            for (int i = 0; i < partHealth.Length; i++)
            {
                partHealth[i] = 100; // 초기 건강 상태는 100
            }
        }

        public void SetPlayerStartPosition(Vector3 position)
        {
            PlayerStartPosition = position;
        }

        // 파츠의 상태를 업데이트하는 메서드
        public void UpdatePartHealth(int partNumber, int health)
        {
            if (partNumber >= 0 && partNumber < partHealth.Length)
            {
                partHealth[partNumber] = health;
            }
        }

        // 파츠의 현재 상태를 반환하는 메서드
        public int GetPartHealth(int partNumber)
        {
            if (partNumber >= 0 && partNumber < partHealth.Length)
            {
                return partHealth[partNumber];
            }
            return -1; // 잘못된 파츠 번호의 경우
        }

        // 행성 정보를 설정하는 메서드
        public void SetPlanetInfo(int index, string risk, int asset)
        {
            if (planetsInfo == null)
            {
                planetsInfo = new PlanetInfo[6]; // 행성 수에 맞게 배열 초기화
            }
            planetsInfo[index] = new PlanetInfo(risk, asset);
        }

        // 행성 정보를 반환하는 메서드
        public PlanetInfo GetPlanetInfo(int index)
        {
            if (planetsInfo != null && index >= 0 && index < planetsInfo.Length)
            {
                return planetsInfo[index];
            }
            return null; // 잘못된 인덱스의 경우
        }

        // 행성 씬으로 전환하는 메서드
        public void LoadPlanetScene(int index)
        {
            if (planetSceneNames != null && index >= 0 && index < planetSceneNames.Length)
            {
                SceneManager.LoadScene(planetSceneNames[index]);
            }
            else
            {
                Debug.LogError("Invalid planet index or scene name is missing.");
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
                Vector3 spawnPosition = spawnPoints[spawnIndex].position;
                GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

                Camera.main.GetComponent<CameraFollow>().SetTarget(player.transform);
                player.GetComponent<PlayerControllerMain>().InitializeUI();
            }
        }
    }
}
