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

    // �� ����
    public bool isReady = false;

    // �ΰ��� ����
    [SerializeField]
    private int playerSequence; // �÷��̾� ����
    public int rerollCount; // ���� ���� Ƚ��
    [SerializeField]
    private List<int> normalScoreList = new List<int>(); // ��� ���ھ� ����Ʈ
    [SerializeField]
    private List<int> challengeSocreList = new List<int>(); // ç���� ���ھ� ����Ʈ
    public int bonusScore;
    public bool bonusScoreSuccess;
    public int totalScore;

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

        // �÷��̾� �ΰ��� ����
        rerollCount = 2;
        bonusScoreSuccess = false;

        // ���ھ� �ʱ�ȭ
        for (int i = 0; i < 6; i++) normalScoreList.Add(-1);
        for (int i = 0; i < 6; i++) challengeSocreList.Add(-1);
        bonusScore = 0;
        totalScore = 0;
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

    // �÷��̾� ���� ��ġ
    public void SetPlayerSequence(int sequence)
    {
        this.playerSequence = sequence;
    }


    // �÷��̾� Ready ����
    public void SetReady(bool _isReady)
    {
        PV.RPC("SetReadyRPC", RpcTarget.AllBuffered, _isReady);
    }

    [PunRPC]
    private void SetReadyRPC(bool _isReady)
    {
        isReady = _isReady;
    }

    // �÷��̾� ���� ���� : �����ؾ� �ϴ� �������� ������ �� �ֵ��� �Ѵ�.
    public void ResetInGameSetting()
    {
        PV.RPC("ResetInGameSettingRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ResetInGameSettingRPC()
    {
        // ���� Ƚ�� ä���
        this.rerollCount = 2;
    }

    // ���� ����
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

    // ���� Ȯ�� ��, ���ʽ�/���� ����Ͽ� �����Ѵ�.
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

        // ���ʽ� ���� ä��� 35�� �߰�
        if (bonus >= 63)
        {
            bonusScoreSuccess = true;
            totalScore += 35;
        }

        this.bonusScore = bonus;  this.totalScore = total;
    }

    // �÷��̾� ���� �ʱ�ȭ �� �ʱ�ȭ
    public void InitPlayerSetting()
    {
        // �÷��̾� �ΰ��� ����
        rerollCount = 2;

        // ���ھ� �ʱ�ȭ
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

    // ���� �������� ���� �Լ�
    public int GetPlayerActor() { return playerActor; }
    public string GetPlayerNickName() { return playerNickName.text; }
    public Sprite GetPlayerIcon() { return playerIcon.sprite; }
    public int GetPlayerIconIndex() { return playerIconIndex; }
    public int GetPlayerSequence() { return playerSequence; }

    public List<int> GetNormalScoreList() { return normalScoreList; }
    public List<int> GetChallangeScoreList() { return challengeSocreList; }
}
