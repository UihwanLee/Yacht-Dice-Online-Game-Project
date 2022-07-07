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

    // �ΰ��� ����
    [SerializeField]
    private int playerSequence; // �÷��̾� ����

    private void Start()
    {
        // �÷��̾� ���� ����
        PV = photonView;
        playerActor = PV.Owner.ActorNumber;
        playerNickName.text = PV.Owner.NickName;
        playerIconIndex = -1;

        // Player List �߰�
        NM.Players.Add(this);
        NM.SortPlayers();

        Debug.Log(this.GetPlayerNickName() + "���� �濡 �����Ͽ����ϴ�.");
    }

    private void Update()
    {
        if (PV.IsMine) return;
    }

    // �÷��̾� ����Ʈ ����
    public void OnDestroy()
    {
        if (NM.Players.Contains(this))
        {
            NM.MyPlayerNickName = playerNickName.text;
            NM.Players.Remove(this);
            NM.SortPlayers();
        }
    }


    // �÷��̾� ����
    [PunRPC]
    public void SetPlayerRPC()
    {
        // �÷��̾� �г��� ����
        playerNickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
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
    public void SetRoomRPC()
    {
        Sprite _playerIcon = playerIcon.sprite;
        string _playerNickName = playerNickName.text;
        NM.SetRoomPlayerByRPC(_playerIcon, _playerNickName);
    }

    // Roll Button interactable Ȱ��ȭ : ���� ȭ�鿡�� ����
    public void SetRollButtonActive(bool isInteract)
    {
        IN.SetInGameRollButton(isInteract);
    }




    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // ���� �������� ���� �Լ�
    public int GetPlayerActor() { return playerActor; }
    public string GetPlayerNickName() { return playerNickName.text; }
    public Sprite GetPlayerIcon() { return playerIcon.sprite; }
    public int GetPlayerIconIndex() { return playerIconIndex; }
    public int GetPlayerSequence() { return playerSequence; }
}
