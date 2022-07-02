using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    // ��Ʈ��ũ ����
    [Header("Phote Network")]
    [SerializeField]
    private PhotonView PV;

    // Player ����
    [Header("PlayerInfo")]
    [SerializeField]
    private Text playerNickName; // �÷��̾� �г���
    [SerializeField]
    private Image playerIcon; // �÷��̾� ������

    private void Awake()
    {
        // �÷��̾� ���� ����
        playerNickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        // playerIcon = PV.IsMine ? 
    }

    public void ShowNickName()
    {
        Debug.Log(playerNickName);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
