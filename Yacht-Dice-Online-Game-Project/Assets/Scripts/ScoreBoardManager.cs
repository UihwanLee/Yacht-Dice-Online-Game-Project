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

    // 스코어 보드 열었는지 닫았는지 확인해주는 변수
    public bool isOpenPlayersScoreBoard; 

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

        // ScoreBoard UI 비활성화
        currentPlayerScoreBoard.SetActive(false);
        allPlayersScoreBoard.SetActive(false);

        // Select UI 비활성화
        selectScoreUI.SetActive(false);

        // 현재 선택된 점수 정보 초기화
        currentSelectScore = null;

        // bool 변수 초기화
        isOpenPlayersScoreBoard = false;
    }

    #region CurrentPlayerScoreBoard

    public void SetActiveCurrentPlayerScoreBoard(bool isActive)
    {
        PV.RPC("SetActiveCurrentPlayerScoreBoardRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // CurrentPlayerScoreBoard 활성화/비활성화
    private void SetActiveCurrentPlayerScoreBoardRPC(bool isActive)
    {
        currentPlayerScoreBoard.SetActive(isActive);
    }

    public void SetCurrentPlayerScoreBoard()
    {
        PV.RPC("SetCurrentPlayerScoreBoardRPC", RpcTarget.AllBuffered);
    }


    [PunRPC]
    // 현재 플레이어의 정보를 가져와 스코어 보드 세팅
    private void SetCurrentPlayerScoreBoardRPC()
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
            score = (currentPlayerNormalSocreList[i] == -1) ? "" : currentPlayerNormalSocreList[i].ToString();
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

    public void UpdateCurrentPlayerScoreBoard()
    {
        PV.RPC("UpdateCurrentPlayerScoreBoardRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // 현재 스코어 상황 업데이트
    private void UpdateCurrentPlayerScoreBoardRPC()
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
            }
            index++;
        }


        index = 0; string success = "";
        // Challenge Score Board 업데이트 : 이미 적힌 score는 참조하지 않는다.
        foreach (var challengeScore in challengeScoreList)
        {
            if (challengeScore.transform.GetChild(2).GetComponent<Text>().text == "" || challengeScore.transform.GetChild(2).GetComponent<Text>().color.a == 0.3f)
            {
                challengeScore.transform.GetChild(2).GetComponent<Text>().text = curretnChallengeScores[index].ToString();

                // Challenge Success 성공 여부 저장 : 순서대로 우선순위 저장
                if(scoreLogic.GetChallengeSuccess(index, curretnChallengeScores[index]) != "")  success = scoreLogic.GetChallengeSuccess(index, curretnChallengeScores[index]);
            }
            index++;
        }

        //  Challenge Success 성공 시 애니메이션 재생
        if (success != "") ShowChallengeSuccess(success);
    }

    // 도전 과제 달성 시 애니메이션 추가
    public void ShowChallengeSuccess(string success)
    {
        StartCoroutine(ShowChallengeSuccessCoroutine(success));
    }

    IEnumerator ShowChallengeSuccessCoroutine(string success)
    {
        IN.challengeSuccess.SetActive(true);
        IN.challengeSuccess.GetComponent<Text>().text = success;

        // 야추 달성하면 야추 이펙트 파티클 재생!
        if(success == scoreLogic.YACHTText)
        {
            IN.yachtSuccessFireWorks.SetActive(true);
        }

        yield return new WaitForSeconds(2.5f);

        IN.challengeSuccess.SetActive(false);

        yield return new WaitForSeconds(4f);

        IN.yachtSuccessFireWorks.SetActive(false);
    }

    // 점수 클릭 시, 이미 확정된 점수인지 확인하는 함수
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

    // Select Dice에서 점수 조회 요청 시, 점수를 반환할 수 있게 한다.
    public int GetCurrentPlayerScoreBoardScore(int index, bool isChallenge)
    {
        if (isChallenge) return int.Parse(challengeScoreList[(index % 6)].transform.GetChild(2).GetComponent<Text>().text);
        else return int.Parse(normalScoreList[(index)].transform.GetChild(2).GetComponent<Text>().text);
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

    #endregion

    #region AllPlayersScoreBoard

    // 스코어 보드 버튼 눌렀을 시
    public void OnClickScoreBoardButton()
    {
        StartCoroutine(OnClickScoreBoardButtonCoroutine());
    }

    IEnumerator OnClickScoreBoardButtonCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        isOpenPlayersScoreBoard = !isOpenPlayersScoreBoard;

        allPlayersScoreBoard.SetActive(isOpenPlayersScoreBoard);
        SetScoreBoardPlayersScore();
    }

    // 플레이어 수에 맞춰 AllPlayerScoreBoard 초기화
    public void InitAllPlayerScoreBoard(int playerCount)
    {
        allPlayersScoreBoard.SetActive(true);
        InitPlayerScoreBoardActive(playerCount);
        InitAllPlayerScoreBoardPos(playerCount);
        allPlayersScoreBoard.SetActive(false);
        isOpenPlayersScoreBoard = false;
    }

    /// <summary>
    /// PlayersScoreBoard 포지션 설정
    /// </summary>
    /// <param name="count"></param>
    private void InitPlayerScoreBoardActive(int count)
    {
        // 플레이어 수에 따라 playerScoreBoard 활성화/비활성화
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

    /// <summary>
    /// PlayersScoreBoard 포지션 설정
    /// </summary>
    /// <param name="count"></param>
    private void InitAllPlayerScoreBoardPos(int count)
    {
        // 플레이어 수에 따라 적절한 위치로 이동시킨다.
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

    /// <summary>
    /// 플레이어가 스코어보드를 눌렀을 때 플레이어 스코어보드 정보 불러오는 함수
    /// </summary>
    // 플레이어 점수 정보는 플레이어 각각의 PlayerController 스크립트에 저장되어 있으므르 
    // PlayerController 스크립트에서 불러올 수 있도록 한다.
    // 버튼을 누르때마다 호출하면 비효율적이므로 한 플레이어가 턴이 끝날때마다 갱신할 수 있도록 한다.(선택)
    public void SetScoreBoardPlayersScore()
    {
        List<GameObject> playerScoreBoardList = new List<GameObject>();
        playerScoreBoardList.Add(player1); playerScoreBoardList.Add(player2); playerScoreBoardList.Add(player3); playerScoreBoardList.Add(player4);

        for(int i=0; i<IN.Players.Count; i++)
        {
            SetScoreBoardPlayerScore(i, playerScoreBoardList[i]);
        }
    }

    private void SetScoreBoardPlayerScore(int index, GameObject playerScoreBoard)
    {
        // 변수 할당
        Text roundInfo = scoreInfo.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        Image playerIcon = playerScoreBoard.transform.GetChild(0).transform.GetChild(1).GetComponent<Image>();
        List<Transform> normalScore = new List<Transform>(); List<Transform> challengeScore = new List<Transform>();
        for(int i=0; i<6; i++) 
        { 
            normalScore.Add(playerScoreBoard.transform.GetChild(1).transform.GetChild(i)); 
            challengeScore.Add(playerScoreBoard.transform.GetChild(2).transform.GetChild(i)); 
        }

        List<int> playerNormalSocreList = IN.Players[index].GetNormalScoreList(); 
        List<int> playerChallengeSocreList = IN.Players[index].GetChallangeScoreList();
        if (playerNormalSocreList == null || playerChallengeSocreList == null) return; // 예외처리

        Transform bonus = playerScoreBoard.transform.GetChild(3).transform.GetChild(0);
        Transform bonusGet = playerScoreBoard.transform.GetChild(3).transform.GetChild(1);
        Transform total = playerScoreBoard.transform.GetChild(4).transform.GetChild(0);


        // 라운드 정보 할당
        roundInfo.text = IN.currentRound.ToString();

        // 플레이어 정보 할당
        playerIcon.sprite = IN.Players[index].GetPlayerIcon();

        // 점수 할당

        string score;


        // 일반 점수 할당
        for (int i=0; i<playerNormalSocreList.Count; i++)
        {
            score = (playerNormalSocreList[i] == -1) ? "" : playerNormalSocreList[i].ToString();
            normalScore[i].transform.GetChild(2).GetComponent<Text>().text = score;
            normalScore[i].transform.GetChild(2).GetComponent<Text>().color = Color.black;

            // 자기 자신의 스코어 보드면 배경 노란색 처리
            if (IN.MyPlayer.GetPlayerSequence() == index) normalScore[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        }

        // 도전 점수 할당
        for (int i = 0; i < playerChallengeSocreList.Count; i++)
        {
            score = (playerChallengeSocreList[i] == -1) ? "" : playerChallengeSocreList[i].ToString();
            challengeScore[i].transform.GetChild(2).GetComponent<Text>().text = score;
            challengeScore[i].transform.GetChild(2).GetComponent<Text>().color = Color.black;

            // 자기 자신의 스코어 보드면 배경 노란색 처리
            if (IN.MyPlayer.GetPlayerSequence() == index) challengeScore[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        }

        // Bonus  세팅(63점 이상이면 35점으로 갱신)
        score = IN.Players[index].bonusScore.ToString();
        bonus.transform.GetChild(2).GetComponent<Text>().text = (score + "/63");
        bonusGet.transform.GetChild(2).GetComponent<Text>().text = (int.Parse(score) >= 63) ? "35" : "";

        // Total 세팅
        if (IN.MyPlayer.GetPlayerSequence() == index) total.transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
        total.transform.GetChild(2).GetComponent<Text>().text = IN.Players[index].totalScore.ToString();
    }

    #endregion

    #region 점수 선택

    // Select Score 점수 세팅
    public void SetAllSelectScore()
    {
        // Normal Score Board : SelectScore 업데이트 
        foreach (var normalScore in normalScoreList) normalScore.GetComponent<SelectDice>().score = int.Parse(normalScore.transform.GetChild(2).GetComponent<Text>().text);
        // Challenge Score Board : SelectScore 업데이트
        foreach (var challengeScore in challengeScoreList) challengeScore.GetComponent<SelectDice>().score = int.Parse(challengeScore.transform.GetChild(2).GetComponent<Text>().text);

    }

    // Selcet UI Transform 이동
    public void MovingSelectUITransform(bool isChallenge, float movePosX)
    {
        PV.RPC("MovingSelectUITransformRPC", RpcTarget.AllBuffered, isChallenge, movePosX);
    }

    [PunRPC]
    private void MovingSelectUITransformRPC(bool isChallenge, float movePosX)
    {
        this.selectScoreUI.transform.localPosition = (isChallenge) ? new Vector3(movePosX, -450f, 0f) : new Vector3(movePosX, -280f, 0f);
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
