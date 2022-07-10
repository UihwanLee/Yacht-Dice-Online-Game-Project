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

    // 플레이어가 현재 선택한 Score 버튼
    private SelectScore currentSelectScore;

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

        currentPlayerScoreBoard.SetActive(false);

        // Select UI 비활성화
        selectScoreUI.SetActive(false);

        // 현재 선택된 점수 정보 초기화
        currentSelectScore = null;
    }

    [PunRPC]
    // CurrentPlayerScoreBoard 활성화/비활성화
    public void SetActiveCurrentPlayerScoreBoard(bool isActive)
    {
        currentPlayerScoreBoard.SetActive(isActive);
    }


    [PunRPC]
    // 현재 플레이어의 정보를 가져와 스코어 보드 세팅
    public void SetCurrentPlayerScoreBoardRPC()
    {
        // 현재 플레이어 불러오기
        PlayerController currentPlayer = IN.Players[IN.currentPlayerSequence];

        currentPlayerScoreBoard.SetActive(true);

        List<int> currentPlayerNormalSocreList = currentPlayer.GetNormalScoreList();
        List<int> currentPlayerChallengeScoreList = currentPlayer.GetChallangeScoreList();

        string score; Color color;

        // Normal Score Board 세팅
        for(int i=0; i<normalScoreList.Count; i++)
        {
            score = (currentPlayerNormalSocreList[i] == 0) ? "" : currentPlayerNormalSocreList[i].ToString();
            color = Color.black; color.a = (score == "") ? 0.3f : 1f; // 알파값 설정
            normalScoreList[i].transform.GetChild(2).GetComponent<Text>().text = score;
            normalScoreList[i].transform.GetChild(2).GetComponent<Text>().color = color;
        }

        // Challenge Score Board 세팅
        for (int i = 0; i < challengeScoreList.Count; i++)
        {
            score = (currentPlayerChallengeScoreList[i] == -1) ? "" : currentPlayerChallengeScoreList[i].ToString();
            color = Color.black; color.a = (score == "") ? 0.3f : 1f; // 알파값 설정
            challengeScoreList[i].transform.GetChild(2).GetComponent<Text>().text = score;
            challengeScoreList[i].transform.GetChild(2).GetComponent<Text>().color = color;
        }

        // Bonus  세팅(63점 이상이면 35점으로 갱신)
        score = (currentPlayer.bonusScore >= 63) ? "35" : (currentPlayer.bonusScore.ToString() + "/63");
        color = (currentPlayer.bonusScore >= 63) ? Color.black : Color.white;
        bonus.transform.GetChild(2).GetComponent<Text>().text = score;
        bonus.transform.GetChild(2).GetComponent<Text>().color = color;

        // TotalScore  세팅
        totalScore.transform.GetChild(2).GetComponent<Text>().text = currentPlayer.totalScore.ToString();

    }

    [PunRPC]
    // 현재 스코어 상황 업데이트
    public void UpdateCurrentPlayerScoreBoardRPC()
    {
        currentNormalScores = scoreLogic.CalculateNormalScoreByDiceObject(diceController.Dices);
        curretnChallengeScores = scoreLogic.CalculateChallengeByDiceObject(diceController.Dices);

        currentPlayerScoreBoard.SetActive(true);

        int index = 0;
        // Normal Score Board 업데이트 : 이미 적힌 score는 참조하지 않는다.
        foreach (var normalScore in normalScoreList)
        {
            if(normalScore.transform.GetChild(2).GetComponent<Text>().text == "" || normalScore.transform.GetChild(2).GetComponent<Text>().color.a == 0.3f)
            {
                normalScore.transform.GetChild(2).GetComponent<Text>().text = currentNormalScores[index].ToString();
                index++;
            }
        }


        index = 0;
        // Challenge Score Board 업데이트 : 이미 적힌 score는 참조하지 않는다.
        foreach (var challengeScore in challengeScoreList)
        {
            if (challengeScore.transform.GetChild(2).GetComponent<Text>().text == "" || challengeScore.transform.GetChild(2).GetComponent<Text>().color.a == 0.3f)
            {
                challengeScore.transform.GetChild(2).GetComponent<Text>().text = curretnChallengeScores[index].ToString();
                index++;
            }
        }
    }

    // Select Score UI 활성화/비활성화
    public void SetSelectScoreUI(bool isActive)
    {
        PV.RPC("SetSelectScoreUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI 활성화/비활성화
    private void SetSelectScoreUIRPC(bool isActive)
    {
        selectScoreUI.SetActive(isActive);
    }

    #region 점수 선택

    // Select Score 점수 세팅
    public void SetAllSelectScore()
    {
        // Normal Score Board : SelectScore 업데이트 
        foreach (var normalScore in normalScoreList) normalScore.GetComponent<SelectDice>().score = int.Parse(normalScore.transform.GetChild(2).GetComponent<Text>().text);
        // Challenge Score Board : SelectScore 업데이트
        foreach (var challengeScore in challengeScoreList) challengeScore.GetComponent<SelectDice>().score = int.Parse(challengeScore.transform.GetChild(2).GetComponent<Text>().text);

    }

    // 플레이어가 선택한 SelectScore 업데이트
    public void ChangeSelectScore(int index, bool isSelect)
    {
        PV.RPC("ChangeSelectScoreRPC", RpcTarget.AllBuffered, index, isSelect);
    }

    [PunRPC]
    // RPC로 SelectScore 스크립트를 받을 수 없으므로 index로 찾을 수 있도록 한다.
    private void ChangeSelectScoreRPC(int index, bool isSelect)
    {
        // 모든 SelectUI Color 초기화
        foreach (var normalScore in normalScoreList) normalScore.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        foreach (var challengeScore in challengeScoreList) challengeScore.transform.GetChild(0).GetComponent<Image>().color = Color.white;


        // index 값에 따라 맞는 Select Score 색깔 변화 : index = -1 일 시 제외(클릭 시: 노랑 / 선택 시: 주황)
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
