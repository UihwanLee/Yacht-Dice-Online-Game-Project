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

    // 룸 정보
    public bool isReady = false;

    // 인게임 정보
    [SerializeField]
    private int playerSequence; // 플레이어 순서
    public int rerollCount; // 리롤 남은 횟수
    [SerializeField]
    private List<int> normalScoreList = new List<int>(); // 노멀 스코어 리스트
    [SerializeField]
    private List<int> challengeSocreList = new List<int>(); // 챌린지 스코어 리스트
    public int bonusScore;
    public bool bonusScoreSuccess;
    public int totalScore;

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

        // 플레이어 인게임 변수
        rerollCount = 2;
        bonusScoreSuccess = false;

        // 스코어 초기화
        for (int i = 0; i < 6; i++) normalScoreList.Add(-1);
        for (int i = 0; i < 6; i++) challengeSocreList.Add(-1);
        bonusScore = 0;
        totalScore = 0;
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

    // 플레이어 순서 배치
    public void SetPlayerSequence(int sequence)
    {
        this.playerSequence = sequence;
    }


    // 플레이어 Ready 세팅
    public void SetReady(bool _isReady)
    {
        PV.RPC("SetReadyRPC", RpcTarget.AllBuffered, _isReady);
    }

    [PunRPC]
    private void SetReadyRPC(bool _isReady)
    {
        isReady = _isReady;
    }

    // 플레이어 세팅 리셋 : 리셋해야 하는 변수들을 리셋할 수 있도록 한다.
    public void ResetInGameSetting()
    {
        PV.RPC("ResetInGameSettingRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ResetInGameSettingRPC()
    {
        // 리롤 횟수 채우기
        this.rerollCount = 2;
    }

    // 점수 세팅
    public void SetScore(int score, int index, bool isChallenge)
    {
        PV.RPC("SetScoreRPC", RpcTarget.AllBuffered, score, index, isChallenge);
    }

    [PunRPC]
    private void SetScoreRPC(int score, int index, bool isChallenge)
    {
        if (!isChallenge) normalScoreList[index] = score;
        else challengeSocreList[(index % 6)] = score;

        CaculatePlayerScore();
    }

    // 점수 확정 후, 보너스/총합 계산하여 저장한다.
    private void CaculatePlayerScore()
    {
        int bonus = 0; int total = 0;
        foreach(var score in normalScoreList) 
        { 
            bonus += (score == -1) ? 0 : score;
            total += (score == -1) ? 0 : score;
        }
        foreach (var score in challengeSocreList)
        {
            total += (score==-1) ? 0 : score;
        }

        // 보너스 점수 채우면 35점 추가
        if (bonus >= 63)
        {
            bonusScoreSuccess = true;
            totalScore += 35;
        }

        this.bonusScore = bonus;  this.totalScore = total;
    }

    // 플레이어 점수 초기화 및 초기화
    public void InitPlayerSetting()
    {
        // 플레이어 인게임 변수
        rerollCount = 2;

        // 스코어 초기화
        normalScoreList.Clear(); challengeSocreList.Clear();
        for (int i = 0; i < 6; i++) normalScoreList.Add(-1);
        for (int i = 0; i < 6; i++) challengeSocreList.Add(-1);
        bonusScore = 0;
        bonusScoreSuccess = false;
        totalScore = 0;
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

    public List<int> GetNormalScoreList() { return normalScoreList; }
    public List<int> GetChallangeScoreList() { return challengeSocreList; }
}
