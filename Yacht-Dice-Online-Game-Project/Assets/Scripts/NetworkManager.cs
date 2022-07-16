using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static InGameNetWorkManager;
using static ChatManager;

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
    private GameObject enterLobbyUI;
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
    [SerializeField]
    private Text roomsCountText;
    [SerializeField]
    private GameObject createRoomUI;
    [SerializeField]
    private List<GameObject> maxPlayersCount = new List<GameObject>();

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

    // 방장 인덱스
    private int currentRoomMasterindex = 0;

    [Header("InGamePanel")]
    [SerializeField]
    private GameObject inGamePanel;

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("Scipts")]
    [SerializeField]
    private DiceController diceController;
    [SerializeField]
    private ChatManager chatManager;
    [SerializeField]
    private UIManager uiManager;

    [Header("Status")]
    [SerializeField]
    private GameObject statusText;

    [Header("PV")]
    public PhotonView PV;

    [Header("List")]
    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [Header("Player")]
    public string MyPlayerNickName;
    public PlayerController MyPlayer;
    public List<PlayerController> Players = new List<PlayerController>();
    public int currentBanPlayerIndex = -1;

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

        // UI 초기화
        enterLobbyUI.SetActive(false);
        createRoomUI.SetActive(false);

        // 패널 초기화 
        titlePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    private void Start()
    {
        PV = photonView;
    }
    #endregion


    #region 서버연결

    private void Update()
    {
        if(uiManager.GetLoadingUIAcive()) statusText.GetComponent<Text>().text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

    public void Connect()
    {
        // 인풋값이 비어있거나 6글자 이상일 시 컷
        if(nickNameInputField.text != "" && nickNameInputField.text.Length <= 5)
        {
            StartCoroutine(ConnectCoroutine());
        }
        else
        {
            nickNameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }

    IEnumerator ConnectCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        // 로딩 화면 전개
        uiManager.SetLoadingUI(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        // 로딩화면 비활성화
        uiManager.SetLoadingUI(false);

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
            if(Players.Count!=0) Players.Clear();
            chatManager.ResetChatText();
            isChat = false;
            inGamePanel.SetActive(false);
        }
        if(roomPanel)
        {
            if (Players.Count != 0) Players.Clear();
            chatManager.ResetChatText();
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
        StartCoroutine(CreatRoomCoroutine());
    }

    IEnumerator CreatRoomCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        uiManager.SetLoadingUI(true);

        int count = -1;
        for (int i = 0; i < maxPlayersCount.Count; i++) if (maxPlayersCount[i].transform.GetChild(1).gameObject.activeSelf) count = i;

        if (count != -1) PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = System.Convert.ToByte(count + 1) });

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
            TryJoinRoom(num);
        }
        MyListRenewal();
    }

    private void TryJoinRoom(int num)
    {
        // 방 인원 수가 다 채워져 있으면 못 들어간다.
        if(myList[multiple + num].PlayerCount != myList[multiple + num].MaxPlayers)
        {
            // 게임 중이면 접속 불가
            if(myList[multiple + num].IsOpen)
            {
                uiManager.SetLoadingUI(true);
                // 플레리어 닉네임이 세팅되지 않았다면 랜덤 플레이어로 활성화
                if (PhotonNetwork.LocalPlayer.NickName == "") PhotonNetwork.LocalPlayer.NickName = "PL" + Random.Range(0, 1000);
                PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            }
            else
            {
                uiManager.SetNoticeUI("방이 게임중이라 접속할수 없습니다!");
            }
        }
        else
        {
            uiManager.SetNoticeUI("방 인원이                        다 꽉 차있습니다!");
        }
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
            if((multiple + i < myList.Count))
            {
                roomList[i].interactable = true;
                roomList[i].transform.GetChild(0).GetComponent<Text>().text = myList[multiple + i].Name;

                // 방이 다 차 있거나 게임 플레이 중이면 빨간색 표시
                Color newColor = (myList[multiple + i].PlayerCount == myList[multiple + i].MaxPlayers || !myList[multiple + i].IsOpen) ? Color.red : Color.black;
                roomList[i].transform.GetChild(1).GetComponent<Text>().text = myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers;
                roomList[i].transform.GetChild(1).GetComponent<Text>().color = newColor;
                roomList[i].transform.GetChild(2).GetComponent<Text>().text = (myList[multiple + i].IsOpen) ? "" : "Game Playing";
            }
            else
            {
                roomList[i].interactable = false;
                roomList[i].transform.GetChild(0).GetComponent<Text>().text = "";
                roomList[i].transform.GetChild(1).GetComponent<Text>().text = "";
                roomList[i].transform.GetChild(2).GetComponent<Text>().text = "";
            }
        }

        roomsCountText.text = "Rooms " + currentPage.ToString() + "/" + (myList.Count / roomList.Length + 1).ToString();
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
        CM.ResetChat();
        CM.OnClckChatButton();

        // 플레이어 생성
        MyPlayer = PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();


        StartCoroutine(SetRoomCoroutine());

    }

    // Player가 방 입장 시 Player 변수 사용 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        CM.PV.RPC("ChatRPC", RpcTarget.AllBuffered, ("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>"));
    }

    // 플레이어 방 퇴장
    public void TryPlayerBan(int index)
    {
        currentBanPlayerIndex = index;
        uiManager.SetConfirmUI("정말로 " + Players[index].GetPlayerNickName() + "(님)을 퇴장시키겠습니까?", "퇴장");
    }

    public void PlayerBan()
    {
        PV.RPC("PlayerBanRPC", RpcTarget.AllBuffered, currentBanPlayerIndex);
    }

    [PunRPC]
    private void PlayerBanRPC(int banPlayerIndex)
    {
        if (banPlayerIndex == -1) return;


        Debug.Log(banPlayerIndex);

        // 해당 플레이어를 찾아가 강제로 강퇴시킨다.
        if (MyPlayer.GetPlayerActor() == Players[banPlayerIndex].GetPlayerActor())
        {
            LeftRoom();
        }
    }

    // 방 나가기 함수
    public void LeftRoom()
    {
        StartCoroutine(LeftRoomCoroutine());
    }

    IEnumerator LeftRoomCoroutine()
    {
        string nickname = MyPlayer.GetPlayerNickName();

        yield return new WaitForSeconds(0.25f);

        if (Players.Count != 0) Players.Clear();
        chatManager.ResetChatText();

        PhotonNetwork.LeaveRoom();

        uiManager.SetLoadingUI(true);
        roomPanel.SetActive(false);
        inGamePanel.SetActive(false);
        lobbyPanel.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    // Player가 방 나갈 시 Player 변수 사용 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        CM.PV.RPC("ChatRPC", RpcTarget.AllBuffered, ("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>"));
        PV.RPC("ResetRoomRPC", RpcTarget.AllBuffered, otherPlayer.NickName);
        CM.ResetChatText();
    }

    [PunRPC]
    // 방 나갈 시 플레이어 삭제 후 룸 초기화
    private void ResetRoomRPC(string playerNickName)
    {
        PlayerController playerDelete = null;
        foreach(var player in Players)
        {
            if(player.GetPlayerNickName() == playerNickName)
            {
                playerDelete = player;
            }
        }
        if(playerDelete != null) Players.Remove(playerDelete);

        // 남은 방 정리
        SetRoomPlayer();

        // 남은 세팅 정리
        SetRoomPlayerMasterClient();
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

    // 방 세팅 코루틴
    IEnumerator SetRoomCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        // 로딩 화면 제거
        uiManager.SetLoadingUI(false);

        // 플레이어 설정
        SetPlayer();

        // 플레이어 무작위 아이콘 추가
        SetRandPlayerIcon();

        // 플레이어 룸 배치
        SetRoomPlayer();

        // 플레이어 세팅 배치
        SetRoomPlayerMasterClient();
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
        // Player UI 배치
        PV.RPC("SetRoomPlayerRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetRoomPlayerRPC()
    {
        for(int i=0; i< roomPlayers.Count; i++)
        {
            if(i < Players.Count)
            {
                // 플레이어 정보 입력
                Debug.Log("자신이 들어갈 인덱스: " + i + " 플레이어 이름: " + Players[i].GetPlayerNickName());
                roomPlayers[i].SetActive(true);
                roomPlayers[i].transform.GetChild(0).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
                roomPlayers[i].transform.GetChild(1).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
                roomPlayers[i].transform.GetChild(2).gameObject.SetActive(false);

                // 플레이어 순서 설정
                Players[i].SetPlayerSequence(i);
                roomPlayers[i].transform.GetChild(5).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(5).GetComponent<Text>().text = (Players[i].GetPlayerSequence() + 1).ToString() + "st";

                // 플레이어가 방장 일시, 왕관 추가 && 게임 준비 UI 끄기
                if (i == currentRoomMasterindex)
                {
                    roomPlayers[i].transform.GetChild(2).gameObject.SetActive(true);
                    roomPlayers[i].transform.GetChild(6).gameObject.SetActive(false);
                }
                else
                {
                    roomPlayers[i].transform.GetChild(6).gameObject.SetActive(true);
                    bool isReady = (Players[i].isReady) ? true : false;
                    roomPlayers[i].transform.GetChild(6).transform.GetChild(0).gameObject.SetActive(isReady);
                }

                // 방장일 시 게임 퇴장 기능 활성화
                if (PhotonNetwork.IsMasterClient)
                { 
                    if (i != currentRoomMasterindex) roomPlayers[i].transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    roomPlayers[i].transform.GetChild(3).gameObject.SetActive(false);
                }
            }
            else
            {
                roomPlayers[i].SetActive(false);

                // 최대 인원을 판별하여 못들어오는 슬롯은 X로 바꿀수 있도록 한다.
                if(i > PhotonNetwork.CurrentRoom.MaxPlayers - 1)
                {
                    roomPlayers[i].SetActive(true);
                    roomPlayers[i].transform.GetChild(0).gameObject.SetActive(false);
                    roomPlayers[i].transform.GetChild(1).gameObject.SetActive(false);
                    roomPlayers[i].transform.GetChild(2).gameObject.SetActive(false);
                    roomPlayers[i].transform.GetChild(3).gameObject.SetActive(false);
                    roomPlayers[i].transform.GetChild(4).gameObject.SetActive(true);
                    roomPlayers[i].transform.GetChild(5).gameObject.SetActive(false);
                    roomPlayers[i].transform.GetChild(6).gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetRoomPlayerMasterClient()
    {
        // 방장이면 왕관 &&  게임 시작 버튼 활성화 / 방장이 아니면 다른 사람 playerExit 추가 && 게임 준비 버튼 활성화
        // 방장이면 Game Start / 방장이 아닐 시 Game Ready 
        if (PhotonNetwork.IsMasterClient)
        {
            gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "게임 시작";
        }
        else
        {
            // 플레이어가 게임 준비 상태인지 아닌지 판별하여 세팅
            if (MyPlayer.isReady) gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "준비 취소";
            else gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "게임 준비";
        }
    }

    // Game Start / Exit Button 세팅
    public void SetStartExitButton(bool isChat)
    {
        gameStartButton.SetActive(isChat);
        exitRoomButton.SetActive(isChat);
    }

    #endregion


    #endregion

    #region 인게임

    // pos -180.5 16.45 -1.15
    // rot 81.464 0 0

    // 게임 시작 : 모든 플레이어가 게임 준비 완료 상태이면 게임시작
    // 게임 준비 -> 준비 취소 / 준비 취소 -> 게임 준비
    public void OnClickGameStartButton()
    {
        if(PhotonNetwork.IsMasterClient && gameStartButton.transform.GetChild(0).GetComponent<Text>().text == "게임 시작")
        {
            int isAllReady = 0;
            foreach(var player in Players)
            {
                if(MyPlayer.GetPlayerNickName() != player.GetPlayerNickName())
                {
                    if (player.isReady) isAllReady++;
                }
            }

            // 플레이어가 모두 준비 시, 게임 시작 / 아닐 시 알림 UI 뛰우기
            if (isAllReady == Players.Count - 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                RoomRenewal();
                PV.RPC("StartGameRPC", RpcTarget.AllBuffered);
            }
            else
            {
                uiManager.SetNoticeUI("모든 인원이              준비가 되지 않았습니다!");
            }
        }
        else
        {
            bool setReady = (MyPlayer.isReady) ? false : true;
            string Msg = (setReady) ? "준비 취소" : "게임 준비";
            StartCoroutine(SetPlayerReadyCoroutine(setReady, Msg));
        }
    }

    [PunRPC]
    private void StartGameRPC()
    {
        StartCoroutine(StartGameRPCCoroutine());
    }

    IEnumerator StartGameRPCCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        uiManager.SetLoadingUI(true);
        statusText.GetComponent<Text>().text = "게임 준비 중";

        yield return new WaitForSeconds(2f);

        statusText.GetComponent<Text>().text = "";
        uiManager.SetLoadingUI(false);

        // 인게임 플레이어 세팅
        IN.SetSetAllInGamePlayer();
    }

    IEnumerator SetPlayerReadyCoroutine(bool _isReady, string Msg)
    {
        yield return new WaitForSeconds(0.25f);

        gameStartButton.transform.GetChild(0).GetComponent<Text>().text = Msg;

        PV.RPC("SetPlayerReady", RpcTarget.AllBuffered, MyPlayer.GetPlayerSequence(), _isReady);
    }

    // 게임 준비 세팅 RPC
    [PunRPC]
    private void SetPlayerReady(int index, bool isReady)
    {
        roomPlayers[index].transform.GetChild(6).transform.GetChild(0).gameObject.SetActive(isReady);
        Players[index].SetReady(isReady);
    }


    #endregion

    #region 반환 함수

    public bool GetActiveRoomPanel() { return roomPanel.activeSelf; }
    public bool GetAcitveInGamePanel() { return inGamePanel.activeSelf; }

    #endregion
}
