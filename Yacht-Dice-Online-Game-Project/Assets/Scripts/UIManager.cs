using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPun, IPunObservable
{
    /*
     * UI Manager 스크립트
     * 
     * Title Panel UI 감독
     * Lobby Panel UI 감독
     * Room Panel UI 감독
     * 
    */

    [Header("TitlePanel")]
    [SerializeField]
    private GameObject titlePanel;
    [SerializeField]
    private InputField nickNameInputField;

    [Header("LobbyPanel")]
    [SerializeField]
    private GameObject lobbyPanel;
    [SerializeField]
    private InputField roomInput;
    [SerializeField]
    private Text welcomeText;
    [SerializeField]
    private Text lobbyInfoText;
    [SerializeField]
    private Button[] roomList;
    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private Button nextButton;

    [Header("RoomPanel")]
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private Text listText;
    [SerializeField]
    private Text roomInfoText;
    [SerializeField]
    private List<GameObject> roomPlayers = new List<GameObject>();
    [SerializeField]
    private Text[] chatText;
    [SerializeField]
    private InputField chatInput;

    [Header("Effect")]
    [SerializeField]
    private DiceController diceController;

    [Header("PlayerIcon")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();

    [Header("PV")]
    public PhotonView PV;

    private int roomSlotIndex;

    public static UIManager UM;

    private void Awake()
    {
        // 인스턴스 초기화
        UM = this;

        // 패널 초기화 
        titlePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        // UI 오브젝트 초기화
        for (int i = 0; i < roomPlayers.Count; i++)
        {
            roomPlayers[i].SetActive(false);
        }
    }

    private void Start()
    {
        PV = photonView;

        roomSlotIndex = 0;
    }

    // RoomPlayerList에 플레이어 배치
    public void SetRoomPlayer(Sprite playerIcon, string playerNickName)
    {
        // 플레이어 정보 입력
        Debug.Log("자신이 들어갈 인덱스: " + roomSlotIndex + " 플레이어 이름: " + playerNickName);
        roomPlayers[roomSlotIndex].SetActive(true);
        roomPlayers[roomSlotIndex].transform.GetChild(0).GetComponent<Image>().sprite = playerIcon;
        roomPlayers[roomSlotIndex++].transform.GetChild(1).GetComponent<Text>().text = playerNickName;
    }


    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
