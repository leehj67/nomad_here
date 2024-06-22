using UnityEngine;

namespace MyGameNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public Vector3 PlayerStartPosition { get; private set; }
        public int[] partHealth = new int[4]; // 각 파츠의 건강 상태 (최대 4개의 파츠)

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
    }
}
