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

    [Header("Scirpts")]
    [SerializeField]
    private DiceController diceController;
    [SerializeField]
    private ScoreLogic scoreLogic;

    private void Start()
    {
        PV = photonView;

        currentPlayerScoreBoard.SetActive(false);
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

        Debug.Log("스코어 보드 업데이트!");
    }
}
