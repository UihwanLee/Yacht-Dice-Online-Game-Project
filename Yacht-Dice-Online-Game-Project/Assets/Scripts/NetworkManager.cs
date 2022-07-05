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

        // �г� �ʱ�ȭ 
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


    #region ��������

    private void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString();
        lobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�κ� / " + PhotonNetwork.CountOfPlayers + "����";
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

    #region ��

    // �� ���� : �� ���� Input�� ����� �� �������� ����
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomInput.text == "" ? "Room" + Random.Range(0, 100) : roomInput.text, new RoomOptions { MaxPlayers = 4 });
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
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }
        MyListRenewal();
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
            roomList[i].interactable = (multiple + i < myList.Count) ? true : false;
            roomList[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            roomList[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
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
        OnClckChatUI();
        ResetChat();

        // �÷��̾� ����
        MyPlayer = PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();


        // �÷��̾� ������ ������ �߰�
        SetRandPlayerIcon();

        // �÷��̾� ����
        SetPlayer();

        // �÷��̾� �� ��ġ
        SetRoomPlayer();

    }

    // Player�� �� ���� �� Player ���� ��� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
    }

    // Player�� �� ���� �� Player ���� ��� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "���� �����ϼ̽��ϴ�</color>");
        roomPanel.SetActive(false);
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
        string playerNickName = MyPlayer.GetComponent<PhotonView>().Owner.NickName;
        MyPlayer.GetComponent<PhotonView>().RPC("SetRoomRPC", RpcTarget.AllBuffered);
    }

    // RoomPlayerList�� �÷��̾� ��ġ
    public void SetRoomPlayerByRPC(Sprite playerIcon, string playerNickName)
    {
        // �÷��̾� ���� �Է�
        Debug.Log("�ڽ��� �� �ε���: " + roomSlotIndex + " �÷��̾� �̸�: " + playerNickName);
        roomPlayers[roomSlotIndex].SetActive(true);
        roomPlayers[roomSlotIndex].transform.GetChild(0).GetComponent<Image>().sprite = playerIcon;
        roomPlayers[roomSlotIndex++].transform.GetChild(1).GetComponent<Text>().text = playerNickName;

        //�÷��̾� ���� �Է�
    }

    public int GetRoomSlotIndex()
    {
        return roomSlotIndex;
    }

    // Room Slot Index ����
    public void ResetRoom()
    {
        roomSlotIndex = 0;
    }

    #endregion


    #region ä��

    // ä�� UI
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

    // ä�� �ʱ�ȭ
    public void ResetChat()
    {
        chatInput.text = "";
        for (int i = 0; i < chatText.Length; i++)
        {
            chatText[i].text = "";
        }
    }

    // ä�� ������
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    public void ChatRPC(string msg)
    {
        bool isInput = false;

        // ����ִ� ä�� ������Ʈ�� ã�� msg ����
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }

        // ������ ��ĭ�� ���� �ø�
        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

    #endregion

    #endregion

    #region �ΰ���

    // pos -180.5 16.45 -1.15
    // rot 81.464 0 0

    public void OnClickGameStartButton()
    {

        // �ΰ��� �÷��̾� ����
        IN.PV.RPC("SetAllInGamePlayerRPC", RpcTarget.All);
    }


    #endregion
}
