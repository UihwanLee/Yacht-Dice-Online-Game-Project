using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static InGameNetWorkManager;

public class ScoreBoardManager : MonoBehaviourPunCallbacks
{
    [Header("PV")]
    public PhotonView PV;

    [Header("CurrentPlayerScoreBoard")]
    [SerializeField]
    private GameObject currentPlayerScoreBoard;
    [SerializeField]
    private List<GameObject> normalScoreList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> challengeScoreList = new List<GameObject>();
    [SerializeField]
    private GameObject bonus;
    [SerializeField]
    private GameObject totalScore;

    private List<int> currentNormalScores = new List<int>();
    private List<int> curretnChallengeScores = new List<int>();

    // �÷��̾ ���� ������ Score ��ư
    private SelectScore currentSelectScore;

    [Header("AllPlayersScoreBoard")]
    [SerializeField]
    private GameObject allPlayersScoreBoard;
    [SerializeField]
    private GameObject scoreInfo;
    [SerializeField]
    private GameObject playersScoreInfo;
    [SerializeField]
    private GameObject player1;
    [SerializeField]
    private GameObject player2;
    [SerializeField]
    private GameObject player3;
    [SerializeField]
    private GameObject player4;

    // ���ھ� ���� �������� �ݾҴ��� Ȯ�����ִ� ����
    private bool isOpenPlayersScoreBoard; 

    // Select UI
    [Header("Select UI")]
    public GameObject selectScoreUI;

    [Header("Scirpts")]
    [SerializeField]
    private DiceController diceController;
    [SerializeField]
    private ScoreLogic scoreLogic;

    private void Start()
    {
        PV = photonView;

        // ScoreBoard UI ��Ȱ��ȭ
        currentPlayerScoreBoard.SetActive(false);
        allPlayersScoreBoard.SetActive(false);

        // Select UI ��Ȱ��ȭ
        selectScoreUI.SetActive(false);

        // ���� ���õ� ���� ���� �ʱ�ȭ
        currentSelectScore = null;

        // bool ���� �ʱ�ȭ
        isOpenPlayersScoreBoard = false;
    }

    #region CurrentPlayerScoreBoard

    [PunRPC]
    // CurrentPlayerScoreBoard Ȱ��ȭ/��Ȱ��ȭ
    public void SetActiveCurrentPlayerScoreBoard(bool isActive)
    {
        currentPlayerScoreBoard.SetActive(isActive);
    }


    [PunRPC]
    // ���� �÷��̾��� ������ ������ ���ھ� ���� ����
    public void SetCurrentPlayerScoreBoardRPC()
    {
        // ���� �÷��̾� �ҷ�����
        PlayerController currentPlayer = IN.Players[IN.currentPlayerSequence];

        currentPlayerScoreBoard.SetActive(true);

        List<int> currentPlayerNormalSocreList = currentPlayer.GetNormalScoreList();
        List<int> currentPlayerChallengeScoreList = currentPlayer.GetChallangeScoreList();

        string score; Color color;

        // Normal Score Board ����
        for(int i=0; i<normalScoreList.Count; i++)
        {
            score = (currentPlayerNormalSocreList[i] == 0) ? "" : currentPlayerNormalSocreList[i].ToString();
            color = Color.black; color.a = (score == "") ? 0.3f : 1f; // ���İ� ����
            normalScoreList[i].transform.GetChild(2).GetComponent<Text>().text = score;
            normalScoreList[i].transform.GetChild(2).GetComponent<Text>().color = color;
        }

        // Challenge Score Board ����
        for (int i = 0; i < challengeScoreList.Count; i++)
        {
            score = (currentPlayerChallengeScoreList[i] == -1) ? "" : currentPlayerChallengeScoreList[i].ToString();
            color = Color.black; color.a = (score == "") ? 0.3f : 1f; // ���İ� ����
            challengeScoreList[i].transform.GetChild(2).GetComponent<Text>().text = score;
            challengeScoreList[i].transform.GetChild(2).GetComponent<Text>().color = color;
        }

        // Bonus  ����(63�� �̻��̸� 35������ ����)
        score = (currentPlayer.bonusScore >= 63) ? "35" : (currentPlayer.bonusScore.ToString() + "/63");
        color = (currentPlayer.bonusScore >= 63) ? Color.black : Color.white;
        bonus.transform.GetChild(2).GetComponent<Text>().text = score;
        bonus.transform.GetChild(2).GetComponent<Text>().color = color;

        // TotalScore  ����
        totalScore.transform.GetChild(2).GetComponent<Text>().text = currentPlayer.totalScore.ToString();

    }

    [PunRPC]
    // ���� ���ھ� ��Ȳ ������Ʈ
    public void UpdateCurrentPlayerScoreBoardRPC()
    {
        currentNormalScores = scoreLogic.CalculateNormalScoreByDiceObject(diceController.Dices);
        curretnChallengeScores = scoreLogic.CalculateChallengeByDiceObject(diceController.Dices);

        currentPlayerScoreBoard.SetActive(true);

        int index = 0;
        // Normal Score Board ������Ʈ : �̹� ���� score�� �������� �ʴ´�.
        foreach (var normalScore in normalScoreList)
        {
            if(normalScore.transform.GetChild(2).GetComponent<Text>().text == "" || normalScore.transform.GetChild(2).GetComponent<Text>().color.a == 0.3f)
            {
                normalScore.transform.GetChild(2).GetComponent<Text>().text = currentNormalScores[index].ToString();
                index++;
            }
        }


        index = 0;
        // Challenge Score Board ������Ʈ : �̹� ���� score�� �������� �ʴ´�.
        foreach (var challengeScore in challengeScoreList)
        {
            if (challengeScore.transform.GetChild(2).GetComponent<Text>().text == "" || challengeScore.transform.GetChild(2).GetComponent<Text>().color.a == 0.3f)
            {
                challengeScore.transform.GetChild(2).GetComponent<Text>().text = curretnChallengeScores[index].ToString();
                index++;
            }
        }
    }

    // ���� Ŭ�� ��, �̹� Ȯ���� �������� Ȯ���ϴ� �Լ�
    public bool CheckIsSelectedScore(int index, bool isChallenge)
    {
        if (isChallenge)
        {
            if (challengeScoreList[(index % 6)].transform.GetChild(2).GetComponent<Text>().color == Color.black) return true;
        }
        else
        {
            if (normalScoreList[(index)].transform.GetChild(2).GetComponent<Text>().color == Color.black) return true;
        }
        return false;
    }

    // Select Dice���� ���� ��ȸ ��û ��, ������ ��ȯ�� �� �ְ� �Ѵ�.
    public int GetCurrentPlayerScoreBoardScore(int index, bool isChallenge)
    {
        if (isChallenge) return int.Parse(challengeScoreList[(index % 6)].transform.GetChild(2).GetComponent<Text>().text);
        else return int.Parse(normalScoreList[(index)].transform.GetChild(2).GetComponent<Text>().text);
    }

    // Select Score UI Ȱ��ȭ/��Ȱ��ȭ
    public void SetSelectScoreUI(bool isActive)
    {
        PV.RPC("SetSelectScoreUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI Ȱ��ȭ/��Ȱ��ȭ
    private void SetSelectScoreUIRPC(bool isActive)
    {
        selectScoreUI.SetActive(isActive);
    }

    #endregion

    #region AllPlayersScoreBoard

    // ���ھ� ���� ��ư ������ ��
    public void OnClickScoreBoardButton()
    {
        isOpenPlayersScoreBoard = !isOpenPlayersScoreBoard;

        allPlayersScoreBoard.SetActive(isOpenPlayersScoreBoard);
    }

    // �÷��̾� ���� ���� AllPlayerScoreBoard �ʱ�ȭ
    public void InitAllPlayerScoreBoard(int playerCount)
    {
        allPlayersScoreBoard.SetActive(true);
        InitPlayerScoreBoardActive(playerCount);
        InitAllPlayerScoreBoardPos(playerCount);
        allPlayersScoreBoard.SetActive(false);
    }

    // PlayersScoreBoard ������ ����
    private void InitPlayerScoreBoardActive(int count)
    {
        // �÷��̾� ���� ���� playerScoreBoard Ȱ��ȭ/��Ȱ��ȭ
        switch (count)
        {
            case 1:
                player1.SetActive(true); player2.SetActive(false); player3.SetActive(false); player4.SetActive(false);
                break;
            case 2:
                player1.SetActive(true); player2.SetActive(true); player3.SetActive(false); player4.SetActive(false);
                break;
            case 3:
                player1.SetActive(true); player2.SetActive(true); player3.SetActive(true); player4.SetActive(false);
                break;
            case 4:
                player1.SetActive(true); player2.SetActive(true); player3.SetActive(true); player4.SetActive(true);
                break;
            default:
                Debug.Log("[ERROR]No match!");
                break;
        }
    }

    // PlayersScoreBoard ������ ����
    private void InitAllPlayerScoreBoardPos(int count)
    {
        // �÷��̾� ���� ���� ������ ��ġ�� �̵���Ų��.
        switch (count)
        {
            case 1:
                scoreInfo.transform.localPosition = new Vector3(220f, -15f, 0f);
                playersScoreInfo.transform.localPosition = new Vector3(200f, 0f, 0f);
                break;
            case 2:
                scoreInfo.transform.localPosition = new Vector3(170f, -15f, 0f);
                playersScoreInfo.transform.localPosition = new Vector3(150f, 0f, 0f);
                break;
            case 3:
                scoreInfo.transform.localPosition = new Vector3(80f, -15f, 0f);
                playersScoreInfo.transform.localPosition = new Vector3(60f, 0f, 0f);
                break;
            case 4:
                scoreInfo.transform.localPosition = new Vector3(20f, -15f, 0f);
                playersScoreInfo.transform.localPosition = new Vector3(0f, 0f, 0f);
                break;
            default:
                Debug.Log("[ERROR]No match!");
                break;
        }
    }

    #endregion

    #region ���� ����

    // Select Score ���� ����
    public void SetAllSelectScore()
    {
        // Normal Score Board : SelectScore ������Ʈ 
        foreach (var normalScore in normalScoreList) normalScore.GetComponent<SelectDice>().score = int.Parse(normalScore.transform.GetChild(2).GetComponent<Text>().text);
        // Challenge Score Board : SelectScore ������Ʈ
        foreach (var challengeScore in challengeScoreList) challengeScore.GetComponent<SelectDice>().score = int.Parse(challengeScore.transform.GetChild(2).GetComponent<Text>().text);

    }

    // �÷��̾ ������ SelectScore ������Ʈ
    public void ChangeSelectScore(int index, bool isSelect)
    {
        PV.RPC("ChangeSelectScoreRPC", RpcTarget.All, index, isSelect);
    }

    [PunRPC]
    // RPC�� SelectScore ��ũ��Ʈ�� ���� �� �����Ƿ� index�� ã�� �� �ֵ��� �Ѵ�.
    private void ChangeSelectScoreRPC(int index, bool isSelect)
    {
        // ��� SelectUI Color �ʱ�ȭ
        foreach (var normalScore in normalScoreList) normalScore.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        foreach (var challengeScore in challengeScoreList) challengeScore.transform.GetChild(0).GetComponent<Image>().color = Color.white;


        // index ���� ���� �´� Select Score ���� ��ȭ : index = -1 �� �� ����(Ŭ�� ��: ��� / ���� ��: ��Ȳ)
        if (index == -1) return;

        if(isSelect)
        {
            if (index < 6)
            {
                normalScoreList[index].transform.GetChild(0).GetComponent<Image>().color = new Color(0.9960784f, 0.3137255f, 0f);
                normalScoreList[index].transform.GetChild(2).GetComponent<Text>().color = Color.black;

            }
            else
            {
                challengeScoreList[(index % 6)].transform.GetChild(0).GetComponent<Image>().color = new Color(0.9960784f, 0.3137255f, 0f);
                challengeScoreList[(index % 6)].transform.GetChild(2).GetComponent<Text>().color = Color.black;
            }

        }
        else
        {
            if (index < 6) normalScoreList[index].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
            else challengeScoreList[(index % 6)].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        }
    
    }

    #endregion

}
