using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    // 네트워크 변수
    [Header("Phote Network")]
    [SerializeField]
    private PhotonView PV;

    // Player 정보
    [Header("PlayerInfo")]
    [SerializeField]
    private Text playerNickName; // 플레이어 닉네임
    [SerializeField]
    private Image playerIcon; // 플레이어 아이콘

    private void Awake()
    {
        // 플레이어 변수 설정
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
