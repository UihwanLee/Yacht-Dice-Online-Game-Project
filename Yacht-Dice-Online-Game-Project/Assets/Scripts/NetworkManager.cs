using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static InGameNetWorkManager;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    /*
     * Photon NetWorkManager 스크립트
     * 
     * Title Network 감독
     * Lobby Network 감독
     * Room Network 감독
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
    private GameObject chatUI;
    private bool isChat = false;
    [SerializeField]
    private Text[] chatText;
    [SerializeField]
    private InputField chatInput;
    [SerializeField]
    private GameObject gameStartButton;
    [SerializeField]
    private GameObject exitRoomButton;
    [SerializeField]
    private List<GameObject> roomPlayers = new List<GameObject>();
    private int roomSlotIndex;

    [Header("InGamePanel")]
    [SerializeField]
    private GameObject inGamePanel;

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("Effect")]
    [SerializeField]
    private DiceController diceController;

    [Header("ETC")]
    [SerializeField]
    private Text statusText;
    public PhotonView PV;

    [Header("List")]
    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [Header("Player")]
    public string MyPlayerNickName;
    public PlayerController MyPlayer;
    public List<PlayerController> Players = new List<PlayerController>();

    public static NetworkManager NM;


    #region 네트워크 설정
    private void Awake()
    {
        // 네트워크 변수 설정
        NM = this;

        // 네트워크 설정
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // 메인 카메라 위치 설정
        mainCamera.transform.position = new Vector3(2.52f, 16.1f, -3.5f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(69.19f, 0f, 0f));

        // UI 오브젝트 초기화
        roomPanel.SetActive(true);
        for (int i = 0; i < roomPlayers.Count; i++)
        {
           if(roomPlayers[i]) roomPlayers[i].SetActive(false);
        }

        // 패널 초기화 
        titlePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    private void Start()
    {
        PV = photonView;

        roomSlotIndex = 0;
    }
    #endregion


    #region 서버연결

    private void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

    public void Connect()
    {
        if(nickNameInputField.text != "")
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            nickNameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        // Panel 정리
        titlePanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);

        // 플레이어 닉네임 설정
        PhotonNetwork.LocalPlayer.NickName = nickNameInputField.text;
        MyPlayerNickName = nickNameInputField.text;
        nickNameInputField.text = "";

        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (inGamePanel)
        {
            inGamePanel.SetActive(false);
        }
        if(roomPanel)
        {
            isChat = false;
            ResetRoom();
            roomPanel.SetActive(false);
        }
        if(lobbyPanel)
        {
            lobbyPanel.SetActive(false);
        }
        if(titlePanel)
        {
            titlePanel.SetActive(true);
        }
    }
    #endregion

    #region Lobby Panel NetWorkManager

    #region 방

    // 방 생성 : 방 제목 Input이 비었을 시 랜덤으로 생성
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = 4 });
    }

    // 방 생성 실패 시 랜덤 방제목으로 방 생성
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //roomInfoText.text = "";
        CreateRoom();
    }


    #endregion

    #region 방리스트 갱신

    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else
        {
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }
        MyListRenewal();
    }

    private void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % roomList.Length == 0) ? myList.Count / roomList.Length : myList.Count / roomList.Length + 1;

        // 이전, 다음버튼
        previousButton.interactable = (currentPage <= 1) ? false : true;
        nextButton.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * roomList.Length;
        for (int i = 0; i < roomList.Length; i++)
        {
            roomList[i].interactable = (multiple + i < myList.Count) ? true : false;
            roomList[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            roomList[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    // 룸 리스트 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion



    #endregion

    #region Room Panel NetWorkManager

    #region 방

    // 방 입장시 발생하는 함수
    public override void OnJoinedRoom()
    {
        RoomRenewal();

        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        // 채팅 UI 초기화
        OnClckChatUI();
        ResetChat();

        // 플레이어 생성
        MyPlayer = PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();


        // 플레이어 무작위 아이콘 추가
        SetRandPlayerIcon();

        // 플레이어 설정
        SetPlayer();

        // 플레이어 룸 배치
        SetRoomPlayer();

    }

    // Player가 방 입장 시 Player 변수 사용 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    // Player가 방 나갈 시 Player 변수 사용 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
        roomPanel.SetActive(false);
    }

    // 방 리뉴얼
    void RoomRenewal()
    {
        listText.text = "방 인원 목록: ";
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            listText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ",");
        }
        roomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    #endregion

    #region 플레이어 설정

    // 플레이어 리스트 정렬(방 접속기준)
    public void SortPlayers()
    {
        Players.Sort((p1, p2) => p1.GetPlayerActor().CompareTo(p2.GetPlayerActor()));
    }

    // 플레이어 리스트 정렬(플레이어 순서 기준)
    public void SortPlayersBySequence()
    {
        Players.Sort((p1, p2) => p1.GetPlayerSequence().CompareTo(p2.GetPlayerSequence()));
    }

    // 플레이어 이름 설정
    public void SetPlayer()
    {
        MyPlayer.GetComponent<PhotonView>().RPC("SetPlayerRPC", RpcTarget.AllBuffered);
    }

    // 무작위 플레이어 아이콘 설정
    public void SetRandPlayerIcon()
    {
        List<int> PlayerIcons = new List<int>();

        for (int i = 0; i < Players.Count; i++)
        {
            PlayerIcons.Add(Players[i].GetPlayerIconIndex());
        }

        while (true)
        {
            int randIndex = Random.Range(0, 8);

            if (!PlayerIcons.Contains(randIndex))
            {
                MyPlayer.GetComponent<PhotonView>().RPC("SetIconRPC", RpcTarget.AllBuffered, randIndex);
                break;
            }
        }
    }


    // 플레이어 방 배치
    public void SetRoomPlayer()
    {
        string playerNickName = MyPlayer.GetComponent<PhotonView>().Owner.NickName;
        MyPlayer.GetComponent<PhotonView>().RPC("SetRoomRPC", RpcTarget.AllBuffered);
    }

    // RoomPlayerList에 플레이어 배치
    public void SetRoomPlayerByRPC(Sprite playerIcon, string playerNickName)
    {
        // 플레이어 정보 입력
        Debug.Log("자신이 들어갈 인덱스: " + roomSlotIndex + " 플레이어 이름: " + playerNickName);
        roomPlayers[roomSlotIndex].SetActive(true);
        roomPlayers[roomSlotIndex].transform.GetChild(0).GetComponent<Image>().sprite = playerIcon;
        roomPlayers[roomSlotIndex++].transform.GetChild(1).GetComponent<Text>().text = playerNickName;

        //플레이어 순서 입력
    }

    public int GetRoomSlotIndex()
    {
        return roomSlotIndex;
    }

    // Room Slot Index 리셋
    public void ResetRoom()
    {
        roomSlotIndex = 0;
    }

    #endregion


    #region 채팅

    // 채팅 UI
    public void OnClckChatUI()
    {
        if (isChat)
        {
            chatUI.transform.localPosition = new Vector3(-120, 12f, 0f);
        }
        else
        {
            chatUI.transform.localPosition = new Vector3(-950f, 12f, 0f);
        }

        gameStartButton.SetActive(!isChat);
        exitRoomButton.SetActive(!isChat);

        isChat = !isChat;
    }

    // 채팅 초기화
    public void ResetChat()
    {
        chatInput.text = "";
        for (int i = 0; i < chatText.Length; i++)
        {
            chatText[i].text = "";
        }
    }

    // 채팅 보내기
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    public void ChatRPC(string msg)
    {
        bool isInput = false;

        // 비어있는 채팅 오브젝트를 찾고 msg 삽입
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }

        // 꽉차면 한칸씩 위로 올림
        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

    #endregion

    #endregion

    #region 인게임

    // pos -180.5 16.45 -1.15
    // rot 81.464 0 0

    public void OnClickGameStartButton()
    {

        // 인게임 플레이어 세팅
        IN.PV.RPC("SetAllInGamePlayerRPC", RpcTarget.All);
    }


    #endregion
}
