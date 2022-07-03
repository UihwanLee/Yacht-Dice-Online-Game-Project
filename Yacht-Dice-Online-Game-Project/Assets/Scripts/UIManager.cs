using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPun, IPunObservable
{
    /*
     * UI Manager ��ũ��Ʈ
     * 
     * Title Panel UI ����
     * Lobby Panel UI ����
     * Room Panel UI ����
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
        // �ν��Ͻ� �ʱ�ȭ
        UM = this;

        // �г� �ʱ�ȭ 
        titlePanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);

        // UI ������Ʈ �ʱ�ȭ
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

    // RoomPlayerList�� �÷��̾� ��ġ
    public void SetRoomPlayer(Sprite playerIcon, string playerNickName)
    {
        // �÷��̾� ���� �Է�
        Debug.Log("�ڽ��� �� �ε���: " + roomSlotIndex + " �÷��̾� �̸�: " + playerNickName);
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
