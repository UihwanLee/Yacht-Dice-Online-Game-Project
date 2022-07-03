using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InGameNetWorkManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /*
     * InGameNetWorkManager ��ũ��Ʈ
     *
     * Room Panel Network ����
     * InGame NetWork ����
     * 
    */

    [Header("TitlePanel")]
    [SerializeField]
    private GameObject titlePanel;

    [Header("LobbyPanel")]
    [SerializeField]
    private GameObject lobbyPanel;

    [Header("RoomPanel")]
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private List<GameObject> roomPlayers = new List<GameObject>();

    [Header("Effect")]
    [SerializeField]
    private DiceController diceController;

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();

    [Header("PV")]
    public PhotonView PV;

    private int roomSlotIndex;

    public static InGameNetWorkManager IN;

    private void Awake()
    {
        // �ν��Ͻ� �ʱ�ȭ
        IN = this;

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

    #region ī�޶� ����



    #endregion

    #region Room ����

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

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
