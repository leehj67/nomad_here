using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour
{
    public static PasswordPanel Instance;
    public TMP_InputField passwordInputField;
    public Button joinRoomButton;
    public GameObject panel; // 패널 전체를 나타내는 게임 오브젝트
    private RoomInfo currentRoomInfo;

    private void Awake()
    {
        Instance = this;
        joinRoomButton.onClick.AddListener(JoinRoom);
        panel.SetActive(false); // 패널을 비활성화 상태로 시작
    }

    public void OpenPanel(RoomInfo roomInfo)
    {
        currentRoomInfo = roomInfo;
        panel.SetActive(true); // 패널을 활성화
    }

    void JoinRoom()
    {
        if (currentRoomInfo != null)
        {
            string inputPassword = passwordInputField.text;
            string roomPassword = currentRoomInfo.CustomProperties["password"].ToString();
            if (inputPassword == roomPassword)
            {
                PhotonNetwork.JoinRoom(currentRoomInfo.Name);
                panel.SetActive(false); // 패널을 비활성화
            }
            else
            {
                Debug.Log("Incorrect password");
            }
        }
        else
        {
            Debug.LogError("currentRoomInfo is null");
        }
    }
}
