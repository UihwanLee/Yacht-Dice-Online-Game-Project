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
     * Photon NetWorkManager ��ũ��Ʈ
     * 
     * Title Network ����
     * Lobby Network ����
     * Room Network ����
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

    // ���� �ε���
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


    #region ��Ʈ��ũ ����
    private void Awake()
    {
        // ��Ʈ��ũ ���� ����
        NM = this;

        // ��Ʈ��ũ ����
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        // ���� ī�޶� ��ġ ����
        mainCamera.transform.position = new Vector3(2.52f, 16.1f, -3.5f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(69.19f, 0f, 0f));

        // UI ������Ʈ �ʱ�ȭ
        roomPanel.SetActive(true);
        for (int i = 0; i < roomPlayers.Count; i++)
        {
           if(roomPlayers[i]) roomPlayers[i].SetActive(false);
        }

        // UI �ʱ�ȭ
        enterLobbyUI.SetActive(false);
        createRoomUI.SetActive(false);

        // �г� �ʱ�ȭ 
        titlePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    private void Start()
    {
        PV = photonView;
    }
    #endregion


    #region ��������

    private void Update()
    {
        if(uiManager.GetLoadingUIAcive()) statusText.GetComponent<Text>().text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�κ� / " + PhotonNetwork.CountOfPlayers + "����";
    }

    public void Connect()
    {
        // ��ǲ���� ����ְų� 6���� �̻��� �� ��
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

        // �ε� ȭ�� ����
        uiManager.SetLoadingUI(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        // �ε�ȭ�� ��Ȱ��ȭ
        uiManager.SetLoadingUI(false);

        // Panel ����
        titlePanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);

        // �÷��̾� �г��� ����
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

    #region ��

    // �� ���� : �� ���� Input�� ����� �� �������� ����
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

    // �� ���� ���� �� ���� ���������� �� ����
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //roomInfoText.text = "";
        CreateRoom();
    }


    #endregion

    #region �渮��Ʈ ����

    // ����ư -2 , ����ư -1 , �� ����
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
        // �� �ο� ���� �� ä���� ������ �� ����.
        if(myList[multiple + num].PlayerCount != myList[multiple + num].MaxPlayers)
        {
            // ���� ���̸� ���� �Ұ�
            if(myList[multiple + num].IsOpen)
            {
                uiManager.SetLoadingUI(true);
                // �÷����� �г����� ���õ��� �ʾҴٸ� ���� �÷��̾�� Ȱ��ȭ
                if (PhotonNetwork.LocalPlayer.NickName == "") PhotonNetwork.LocalPlayer.NickName = "PL" + Random.Range(0, 1000);
                PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            }
            else
            {
                uiManager.SetNoticeUI("���� �������̶� �����Ҽ� �����ϴ�!");
            }
        }
        else
        {
            uiManager.SetNoticeUI("�� �ο���                        �� �� ���ֽ��ϴ�!");
        }
    }

    private void MyListRenewal()
    {
        // �ִ�������
        maxPage = (myList.Count % roomList.Length == 0) ? myList.Count / roomList.Length : myList.Count / roomList.Length + 1;

        // ����, ������ư
        previousButton.interactable = (currentPage <= 1) ? false : true;
        nextButton.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * roomList.Length;
        for (int i = 0; i < roomList.Length; i++)
        {
            if((multiple + i < myList.Count))
            {
                roomList[i].interactable = true;
                roomList[i].transform.GetChild(0).GetComponent<Text>().text = myList[multiple + i].Name;

                // ���� �� �� �ְų� ���� �÷��� ���̸� ������ ǥ��
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

    // �� ����Ʈ ������Ʈ
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

    #region ��

    // �� ����� �߻��ϴ� �Լ�
    public override void OnJoinedRoom()
    {
        RoomRenewal();

        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        // ä�� UI �ʱ�ȭ
        CM.ResetChat();
        CM.OnClckChatButton();

        // �÷��̾� ����
        MyPlayer = PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();


        StartCoroutine(SetRoomCoroutine());

    }

    // Player�� �� ���� �� Player ���� ��� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        CM.PV.RPC("ChatRPC", RpcTarget.AllBuffered, ("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>"));
    }

    // �÷��̾� �� ����
    public void TryPlayerBan(int index)
    {
        currentBanPlayerIndex = index;
        uiManager.SetConfirmUI("������ " + Players[index].GetPlayerNickName() + "(��)�� �����Ű�ڽ��ϱ�?", "����");
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

        // �ش� �÷��̾ ã�ư� ������ �����Ų��.
        if (MyPlayer.GetPlayerActor() == Players[banPlayerIndex].GetPlayerActor())
        {
            LeftRoom();
        }
    }

    // �� ������ �Լ�
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

    // Player�� �� ���� �� Player ���� ��� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        CM.PV.RPC("ChatRPC", RpcTarget.AllBuffered, ("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>"));
        PV.RPC("ResetRoomRPC", RpcTarget.AllBuffered, otherPlayer.NickName);
        CM.ResetChatText();
    }

    [PunRPC]
    // �� ���� �� �÷��̾� ���� �� �� �ʱ�ȭ
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

        // ���� �� ����
        SetRoomPlayer();

        // ���� ���� ����
        SetRoomPlayerMasterClient();
    }

    // �� ������
    void RoomRenewal()
    {
        listText.text = "�� �ο� ���: ";
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++)
        {
            listText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ",");
        }
        roomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "�� / " + PhotonNetwork.CurrentRoom.MaxPlayers + "�ִ�";
    }

    #endregion

    #region �÷��̾� ����

    // �÷��̾� ����Ʈ ����(�� ���ӱ���)
    public void SortPlayers()
    {
        Players.Sort((p1, p2) => p1.GetPlayerActor().CompareTo(p2.GetPlayerActor()));
    }

    // �÷��̾� ����Ʈ ����(�÷��̾� ���� ����)
    public void SortPlayersBySequence()
    {
        Players.Sort((p1, p2) => p1.GetPlayerSequence().CompareTo(p2.GetPlayerSequence()));
    }

    // �� ���� �ڷ�ƾ
    IEnumerator SetRoomCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        // �ε� ȭ�� ����
        uiManager.SetLoadingUI(false);

        // �÷��̾� ����
        SetPlayer();

        // �÷��̾� ������ ������ �߰�
        SetRandPlayerIcon();

        // �÷��̾� �� ��ġ
        SetRoomPlayer();

        // �÷��̾� ���� ��ġ
        SetRoomPlayerMasterClient();
    }

    // �÷��̾� �̸� ����
    public void SetPlayer()
    {
        MyPlayer.GetComponent<PhotonView>().RPC("SetPlayerRPC", RpcTarget.AllBuffered);
    }

    // ������ �÷��̾� ������ ����
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


    // �÷��̾� �� ��ġ
    public void SetRoomPlayer()
    {
        // Player UI ��ġ
        PV.RPC("SetRoomPlayerRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetRoomPlayerRPC()
    {
        for(int i=0; i< roomPlayers.Count; i++)
        {
            if(i < Players.Count)
            {
                // �÷��̾� ���� �Է�
                Debug.Log("�ڽ��� �� �ε���: " + i + " �÷��̾� �̸�: " + Players[i].GetPlayerNickName());
                roomPlayers[i].SetActive(true);
                roomPlayers[i].transform.GetChild(0).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
                roomPlayers[i].transform.GetChild(1).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
                roomPlayers[i].transform.GetChild(2).gameObject.SetActive(false);

                // �÷��̾� ���� ����
                Players[i].SetPlayerSequence(i);
                roomPlayers[i].transform.GetChild(5).gameObject.SetActive(true);
                roomPlayers[i].transform.GetChild(5).GetComponent<Text>().text = (Players[i].GetPlayerSequence() + 1).ToString() + "st";

                // �÷��̾ ���� �Ͻ�, �հ� �߰� && ���� �غ� UI ����
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

                // ������ �� ���� ���� ��� Ȱ��ȭ
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

                // �ִ� �ο��� �Ǻ��Ͽ� �������� ������ X�� �ٲܼ� �ֵ��� �Ѵ�.
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
        // �����̸� �հ� &&  ���� ���� ��ư Ȱ��ȭ / ������ �ƴϸ� �ٸ� ��� playerExit �߰� && ���� �غ� ��ư Ȱ��ȭ
        // �����̸� Game Start / ������ �ƴ� �� Game Ready 
        if (PhotonNetwork.IsMasterClient)
        {
            gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "���� ����";
        }
        else
        {
            // �÷��̾ ���� �غ� �������� �ƴ��� �Ǻ��Ͽ� ����
            if (MyPlayer.isReady) gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "�غ� ���";
            else gameStartButton.transform.GetChild(0).GetComponent<Text>().text = "���� �غ�";
        }
    }

    // Game Start / Exit Button ����
    public void SetStartExitButton(bool isChat)
    {
        gameStartButton.SetActive(isChat);
        exitRoomButton.SetActive(isChat);
    }

    #endregion


    #endregion

    #region �ΰ���

    // pos -180.5 16.45 -1.15
    // rot 81.464 0 0

    // ���� ���� : ��� �÷��̾ ���� �غ� �Ϸ� �����̸� ���ӽ���
    // ���� �غ� -> �غ� ��� / �غ� ��� -> ���� �غ�
    public void OnClickGameStartButton()
    {
        if(PhotonNetwork.IsMasterClient && gameStartButton.transform.GetChild(0).GetComponent<Text>().text == "���� ����")
        {
            int isAllReady = 0;
            foreach(var player in Players)
            {
                if(MyPlayer.GetPlayerNickName() != player.GetPlayerNickName())
                {
                    if (player.isReady) isAllReady++;
                }
            }

            // �÷��̾ ��� �غ� ��, ���� ���� / �ƴ� �� �˸� UI �ٿ��
            if (isAllReady == Players.Count - 1)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                RoomRenewal();
                PV.RPC("StartGameRPC", RpcTarget.AllBuffered);
            }
            else
            {
                uiManager.SetNoticeUI("��� �ο���              �غ� ���� �ʾҽ��ϴ�!");
            }
        }
        else
        {
            bool setReady = (MyPlayer.isReady) ? false : true;
            string Msg = (setReady) ? "�غ� ���" : "���� �غ�";
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
        statusText.GetComponent<Text>().text = "���� �غ� ��";

        yield return new WaitForSeconds(2f);

        statusText.GetComponent<Text>().text = "";
        uiManager.SetLoadingUI(false);

        // �ΰ��� �÷��̾� ����
        IN.SetSetAllInGamePlayer();
    }

    IEnumerator SetPlayerReadyCoroutine(bool _isReady, string Msg)
    {
        yield return new WaitForSeconds(0.25f);

        gameStartButton.transform.GetChild(0).GetComponent<Text>().text = Msg;

        PV.RPC("SetPlayerReady", RpcTarget.AllBuffered, MyPlayer.GetPlayerSequence(), _isReady);
    }

    // ���� �غ� ���� RPC
    [PunRPC]
    private void SetPlayerReady(int index, bool isReady)
    {
        roomPlayers[index].transform.GetChild(6).transform.GetChild(0).gameObject.SetActive(isReady);
        Players[index].SetReady(isReady);
    }


    #endregion

    #region ��ȯ �Լ�

    public bool GetActiveRoomPanel() { return roomPanel.activeSelf; }
    public bool GetAcitveInGamePanel() { return inGamePanel.activeSelf; }

    #endregion
}
