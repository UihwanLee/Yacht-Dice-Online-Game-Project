using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;
using static InGameNetWorkManager;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    // ��Ʈ��ũ ����
    [Header("Phote Network")]
    [SerializeField]
    public PhotonView PV;

    // Player ����
    [Header("PlayerInfo")]
    [SerializeField]
    private int playerActor; // �÷��̾� Actor ��ȣ
    [SerializeField]
    private Text playerNickName; // �÷��̾� �г���
    [SerializeField]
    private Image playerIcon; // �÷��̾� ������
    [SerializeField]
    private int playerIconIndex; // �÷��̾� ������ �ε���

    private void Start()
    {
        // �÷��̾� ���� ����
        PV = photonView;
        playerActor = PV.Owner.ActorNumber;
        playerNickName.text = PV.Owner.NickName;
        playerIconIndex = -1;

        SetPlayerNickName();

        // Player List �߰�
        NM.RoomPlayers.Add(this);
        NM.SortPlayers();

        Debug.Log(this.GetPlayerNickName() + "���� �濡 �����Ͽ����ϴ�.");
    }

    private void Update()
    {
        if (PV.IsMine) return;
    }


    // �÷��̾� ���� ����
    public void SetPlayerNickName()
    {
        playerNickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
    }

    // �÷��̾� ����Ʈ ����
    public void OnDestroy()
    {
        if(NM.RoomPlayers.Contains(this))
        {
            NM.RoomPlayers.Remove(this);
            NM.SortPlayers();
        }
    }

    // �÷��̾� ������ ����
    [PunRPC]
    public void SetIconRPC(int _iconIndex)
    {
        playerIcon.sprite = IN.GetPlayerIconByIndex(_iconIndex);
        playerIconIndex = _iconIndex;
    }

    // �濡 �÷��̾� ��ġ
    [PunRPC]
    public void SetRoomRPC(string _playerNickName)
    {
        Sprite _playerIcon = playerIcon.sprite;
        IN.SetRoomPlayer(_playerIcon, _playerNickName);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // ���� �������� ���� �Լ�
    public int GetPlayerActor() { return playerActor; }
    public string GetPlayerNickName() { return playerNickName.text; }
    public int GetPlayerIconIndex() { return playerIconIndex; }
}
