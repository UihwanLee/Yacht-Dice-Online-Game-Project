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
    // 네트워크 변수
    [Header("Phote Network")]
    [SerializeField]
    public PhotonView PV;

    // Player 정보
    [Header("PlayerInfo")]
    [SerializeField]
    private int playerActor; // 플레이어 Actor 번호
    [SerializeField]
    private Text playerNickName; // 플레이어 닉네임
    [SerializeField]
    private Image playerIcon; // 플레이어 아이콘
    [SerializeField]
    private int playerIconIndex; // 플레이어 아이콘 인덱스

    // 인게임 정보
    [SerializeField]
    private int playerSequence; // 플레이어 순서

    private void Start()
    {
        // 플레이어 변수 설정
        PV = photonView;
        playerActor = PV.Owner.ActorNumber;
        playerNickName.text = PV.Owner.NickName;
        playerIconIndex = -1;

        // Player List 추가
        NM.Players.Add(this);
        NM.SortPlayers();

        Debug.Log(this.GetPlayerNickName() + "님이 방에 입장하였습니다.");
    }

    private void Update()
    {
        if (PV.IsMine) return;
    }

    // 플레이어 리스트 삭제
    public void OnDestroy()
    {
        if (NM.Players.Contains(this))
        {
            NM.MyPlayerNickName = playerNickName.text;
            NM.Players.Remove(this);
            NM.SortPlayers();
        }
    }


    // 플레이어 설정
    [PunRPC]
    public void SetPlayerRPC()
    {
        // 플레이어 닉네임 설정
        playerNickName.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
    }

    // 플레이어 아이콘 설정
    [PunRPC]
    public void SetIconRPC(int _iconIndex)
    {
        playerIcon.sprite = IN.GetPlayerIconByIndex(_iconIndex);
        playerIconIndex = _iconIndex;
    }


    // 방에 플레이어 배치
    [PunRPC]
    public void SetRoomRPC()
    {
        Sprite _playerIcon = playerIcon.sprite;
        string _playerNickName = playerNickName.text;
        NM.SetRoomPlayerByRPC(_playerIcon, _playerNickName);
    }

    // Roll Button interactable 활성화 : 개인 화면에만 보임
    public void SetRollButtonActive(bool isInteract)
    {
        IN.SetInGameRollButton(isInteract);
    }




    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // 변수 가져오는 리턴 함수
    public int GetPlayerActor() { return playerActor; }
    public string GetPlayerNickName() { return playerNickName.text; }
    public Sprite GetPlayerIcon() { return playerIcon.sprite; }
    public int GetPlayerIconIndex() { return playerIconIndex; }
    public int GetPlayerSequence() { return playerSequence; }
}
