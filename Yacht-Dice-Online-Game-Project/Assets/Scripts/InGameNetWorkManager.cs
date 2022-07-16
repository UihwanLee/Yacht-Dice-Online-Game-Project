using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;
using static DiceController;

public class InGameNetWorkManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /*
     * InGameNetWorkManager 스크립트
     *
     * Room Panel Network 감독
     * InGame NetWork 감독
     * 
    */

    [Header("TitlePanel")]
    [SerializeField]
    private GameObject titlePanel;

    [Header("RoomPanel")]
    [SerializeField]
    private GameObject roomPanel;

    [Header("InGamePanel")]
    [SerializeField]
    private GameObject inGamePanel;
    [SerializeField]
    private GameObject playerContainer;
    [SerializeField]
    private List<GameObject> inGamePlayers = new List<GameObject>();
    private int inGamePlayerIndex;
    [SerializeField]
    private GameObject emoticonContainer;
    private bool isEmoticonContainer;
    [SerializeField]
    private GameObject rerollCountUI;
    [SerializeField]
    private GameObject restartORExitUI;
    public GameObject rollDiceButton;

    // DiceButton 3가지 설정버튼
    private string SET = "Set";
    private string ROLL = "Roll";
    private string REROLL = "Reroll";

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();
    // 현재 플레이어 순서 : 이 변수에 따라 플레이어 플레이를 순서대로 할 수 있게한다.
    public int currentPlayerSequence;
    public int currentRound;

    public List<Dice> newDices = new List<Dice>();

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    // 애니메이션 관련 변수
    [Header("Animation UI")]
    public GameObject challengeSuccess;
    [SerializeField]
    private GameObject showRoundUI;
    [SerializeField]
    private GameObject showCurrentPlayerSequenceUI;
    [SerializeField]
    private GameObject failThrowDiceUI;
    [SerializeField]
    private GameObject rerollCountDicreaseUI;
    [SerializeField]
    private GameObject showPlayerRankingUI;
    [SerializeField]
    private List<GameObject> playerRankingList = new List<GameObject>();


    [Header("Particle")]
    public GameObject yachtSuccessFireWorks;

    [Header("PV")]
    public PhotonView PV;

    [Header("Player")]
    public PlayerController MyPlayer;
    // NetworkManager 스크립트에서 받게 될 플레이어 정보
    public List<PlayerController> Players = new List<PlayerController>();

    [Header("Scirpts")]
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;
    [SerializeField]
    private DiceSelectManager diceSelectManager;
    [SerializeField]
    private ChatManager chatManager;
    [SerializeField]
    private UIManager uiManager;


    public static InGameNetWorkManager IN;

    private void Awake()
    {
        // 인스턴스 초기화
        IN = this;

        InitGame();

        // 게임 Reset 필요 변수
        InitGameSetting();
    }

    private void InitGame()
    {
        //  오브젝트 초기화
        inGamePanel.SetActive(true);
        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if (inGamePlayers[i]) inGamePlayers[i].SetActive(false);
        }
        DC.SetBottleInitPosByTransform();

        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;

        // 패널 정리
        inGamePanel.SetActive(false);


        Players.Clear();
    }

    private void InitGameSetting()
    {
        currentPlayerSequence = 0;
        currentRound = 1;
        inGamePlayerIndex = 0;

        // rerollCountUI 비활성화
        rerollCountUI.SetActive(false);

        // Animation UI 비활성화
        challengeSuccess.SetActive(false);
        showRoundUI.SetActive(false);
        showCurrentPlayerSequenceUI.SetActive(false);
        failThrowDiceUI.SetActive(false);
        rerollCountDicreaseUI.SetActive(false);
        showPlayerRankingUI.SetActive(false);


        // restartORExitUI 비활성화
        restartORExitUI.SetActive(false);

        // playerScoreBoard UI 초기화
        if(inGamePanel.activeSelf) scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);

        // 파티클 오브젝트 초기화
        yachtSuccessFireWorks.SetActive(false);
    }


    private void Start()
    {
        PV = photonView;
    }

    #region Room 설정

    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

    #region 인게임 세팅 설정


    // 인게임 플레이어 세팅
    public void SetSetAllInGamePlayer()
    {
        PV.RPC("SetAllInGamePlayerRPC", RpcTarget.AllBuffered);

        // 첫번째 플레이어 바로 플레이
        SetPlayerPlaying();
    }

    [PunRPC]
    public void SetAllInGamePlayerRPC()
    {

        // 떨어지는 주사위 효과 끄기
        DC.FallingDice(false);

        // 패널 정리
        roomPanel.SetActive(false);
        inGamePanel.SetActive(true);

        // 카메라 설정
        mainCamera.transform.position = new Vector3(-180.5f, 16.45f, -1.15f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(81.464f, 0f, 0f));

        // 플레이어 정보 얻기
        NM.SortPlayersBySequence(); // 플레이어 순서대로 바꾼 후 정보 얻기
        Players = NM.Players;
        MyPlayer = NM.MyPlayer;
       
        // 플레이어 컨테이너 위치 세팅
        SetPlayerContainerPos(Players.Count);

        // 플레이어 스코어 보드 초기 세팅
        scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);

        // 인게임 플레이아 세팅
        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if(i < Players.Count)
            {
                inGamePlayers[i].SetActive(true);
                inGamePlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
                inGamePlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
            }
            else
            {
                inGamePlayers[i].SetActive(false);
            }
        }
       
    }

    // Player Container 포지션 설정
    private void SetPlayerContainerPos(int count)
    {
        // 플레이어 수에 따라 적절한 위치로 이동시킨다.
        switch(count)
        {
            case 1:
                playerContainer.transform.localPosition = new Vector3(370f, 0f, 0f);
                break;
            case 2:
                playerContainer.transform.localPosition = new Vector3(246f, 0f, 0f);
                break;
            case 3:
                playerContainer.transform.localPosition = new Vector3(123f, 0f, 0f);
                break;
            case 4:
                playerContainer.transform.localPosition = new Vector3(0f, 0f, 0f);
                break;
            default:
                Debug.Log("[ERROR]No match!");
                break;
        }
    }

    #endregion

    #region 인게임 플레이

    // 플레이어 턴이 끝날 시, 다음 플레이어 진행
    public void NextPlayer()
    {
        PV.RPC("SetCurrentPlayerSequenceRPC", RpcTarget.AllBuffered, currentPlayerSequence);
        DC.GetDices();
        DC.SetBottleInitPos();
        StartCoroutine(NextPlayerCoroutine());
    }

    // 다음 플레이어 진행에 맞춰 초기화 해줘야하는 변수들을 모두 초기화 해줄 수 있도록 한다.
    [PunRPC]
    private void SetCurrentPlayerSequenceRPC(int currentSeqiemce)
    {
        // isSelect 초기화
        DC.ResetDiceSelect();
        DC.ResetRemainDiceCount();
        // Dices 모두 넘겨주기
        this.newDices = DC.Dices;

        // Dice 설정 : Dice Bottle 위치 초기화, Dice 위치 초기화(주사위는 Dice Container 위에 isKinetic으로 존재
        DC.SetBottleInitPosByTransform();

        currentSeqiemce++;
        this.currentPlayerSequence = currentSeqiemce;
    }

    // 몇 초 대기 후 시작
    IEnumerator NextPlayerCoroutine()
    {
        SetInterctableRollButton(false);
        SetRerollCountUI(false);

        yield return new WaitForSeconds(3f);

        // 세팅 초기화 : 대기 시간 동안 애니메이션 틀기

        // ScoreBoard 비활성화
        scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);

        // 모든 플레이어가 턴을 마치면 라운드를 증가시키고 다시 첫 플레이어부터 진행
        PV.RPC("CheckRound", RpcTarget.AllBuffered);

        if(currentRound<=12) SetPlayerPlaying();
    }

    // 플레이어 플레이 설정
    public void SetPlayerPlaying()
    {

        // 플레이어 인게임 세팅 리셋
        ResetPlayerInGameSetting();

        // 애니메이션 플레이
        PV.RPC("ShowInGameAnimationRPC", RpcTarget.AllBuffered);
        
        // 첫번째 플레이어 소개하는 애니메이션 이후 첫번째 플레이어 순서를 설정한다.
        PV.RPC("SetPlayerPlayingRPC", RpcTarget.AllBuffered);

        // 플레이어 스코어 보드 생성
        scoreBoardManager.SetCurrentPlayerScoreBoard();
        scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);
    }
    
    [PunRPC]
    private void CheckRound()
    {
        if (currentPlayerSequence == Players.Count)
        {
            // 라운드 증가 후, 첫 플레이어부터 다시 시작
            currentRound++;
            currentPlayerSequence = 0;

            // 12라운드가 끝났을 시, 플레이어 점수 집계 -> 순위 발표 후 게임 종료
            if (currentRound > 12)
            {
                StartCoroutine(StartShowPlayersRankingCoroutine());
            }
        }
    }


    // 플레이어 등수 공개
    IEnumerator StartShowPlayersRankingCoroutine()
    {
        scoreBoardManager.isOpenPlayersScoreBoard = false;
        scoreBoardManager.OnClickScoreBoardButton();

        yield return new WaitForSeconds(2f);

        // 플레이어 등수 보여주는 애니메이션 추가
        StartShowPlayersRankingAnim();

        yield return new WaitForSeconds(8f);

        // 다시 시작 or 나가는 UI 활성화
        restartORExitUI.SetActive(true);
    }

    // 플레이어 인게임 세팅 리셋
    private void ResetPlayerInGameSetting()
    {
        Players[currentPlayerSequence].ResetInGameSetting();
    }

    [PunRPC]
    // 플레이어 순서대로 진행
    public void SetPlayerPlayingRPC()
    {
        // 현재 플레이어 아이콘 이동
        MoveIconCurrentPlayer();

        // 모든 플레이어 Roll Button interactable 비활성화 / 현재 플레이어만 Roll Button Interactable 활성화
        bool isCurrentPlayer = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? true : false;
        SetInterctableRollButton(isCurrentPlayer);

        // Roll -> Set 으로 설정
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = SET;

    }

    private void MoveIconCurrentPlayer()
    {
        // 솔로 플레이 일때는 retrun
        if (Players.Count == 1) return;

        for(int i=0; i < Players.Count; i++)
        {
            if (i == currentPlayerSequence) inGamePlayers[i].transform.localPosition = new Vector3(inGamePlayers[i].transform.localPosition.x, 750f, 0f);
            else inGamePlayers[i].transform.localPosition = new Vector3(inGamePlayers[i].transform.localPosition.x, 790f, 0f);
        }

    }


    // 롤 버튼 활성화/비활성화
    private void SetInterctableRollButton(bool isInteractable)
    {
        rollDiceButton.GetComponent<Button>().interactable = isInteractable;
    }

    public void SetRerollCountUI(bool isActive)
    {
        PV.RPC("SetRerollCountUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    private void SetRerollCountUIRPC(bool isActive)
    {
        if(isActive)
        {
            rerollCountUI.SetActive(isActive);
            rerollCountUI.transform.GetChild(1).GetComponent<Text>().text = Players[currentPlayerSequence].rerollCount + "회";
        }
        else
        {
            rerollCountUI.SetActive(isActive);
        }
    }

    public void SetDice()
    {
        if(rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET || rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == REROLL)
        {
            Debug.Log("Roll Burron 누름!");
            // 플레이어가 다이스 처음 돌릴 경우 isSelect 초기화
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET)
            {
                DC.ResetDiceSelect();
            }

            // 각종 UI 비활성화
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            scoreBoardManager.ChangeSelectScore(-1, false);


            // 리롤 할 시, 돌릴 다이스가 없을 시 동작하지 않게 한다.
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == REROLL)
            {
                DC.UpdateRemainDiceCount();
                if (DC.remainDiceCount == 0)
                {
                    Debug.Log("돌릴 주사위가 없음");
                    return;
                }
                diceSelectManager.SortReturnZoneDice();

                // 리롤 횟수 감소: 리롤 횟수가 모두 차감 시, Reroll을 못하며 점수를 선택해야함
                Players[currentPlayerSequence].rerollCount--;
            }

            SetRerollCountUI(false);

            // 스코어 보드 비활성화
            scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);

            // 다이스 병 위치 이동(Anim으로 세팅)
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET)
            {
                DC.SetBottlePlayingPos();
            }
            else
            {
                DC.ReBottlePlayingPos();
            }

            // 다이스 Bottle Set
            PV.RPC("SetDiceRPC", RpcTarget.AllBuffered);

            // 다이스 소환
            StartCoroutine(SpawnDiceInGameCorutine());
        }
        else Debug.Log("[ERROR]RollButton 상태가 SET이나 REROLL이 아님!");
    }
    

    [PunRPC]
    // 다이스 설정 RPC
    public void SetDiceRPC()
    {
        // Set -> Roll로 설정
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = ROLL;

        // 다이스 소환
        // pos -180.27 12.2 0.16
        DC.SetSpawnPos(new Vector3(-180.27f, 7.2f, 0.16f));
    }

    // 다이스 소환
    IEnumerator SpawnDiceInGameCorutine()
    {
        yield return new WaitForSeconds(1f);
        // 다이스 isKinematic 초기화
        PV.RPC("SetAllDiceKinematicRPC", RpcTarget.AllBuffered);
        // 이미 DicePrefab이 생성되어 있는 경우 Reroll만 작업
        if (DC.Dices.Count == 0) DC.SpawnYachtDicesInGame(1);
        else
        {
            Debug.Log("01. 이미 주사위가 존재해서 다시돌림!");
            if(Players[currentPlayerSequence].rerollCount!=2) StartRerollCountDicreaseAnim();
            DC.RerollYachtDices();
        }
    }

    [PunRPC]
    private void SetAllDiceKinematicRPC()
    {
        DC.SetAllDiceKinematic(false);
    }

    // Dice Roll 시도
    public void TryRollButtonPointerDown()
    {
        if(rollDiceButton.activeSelf)
        {
            if(rollDiceButton.GetComponent<Button>().interactable)
            {
                if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == ROLL)
                {
                    RollButtonPointerDown();
                }
            }
        }
    }

    // Roll Button 누르고 있을 시 계속 Dice 병 흔들기
    public void RollButtonPointerDown()
    {
        DC.ShakingBottle();
    }

    // Dice Roll 시도
    public void TryRollButtonPointerUp()
    {
        if (rollDiceButton.activeSelf)
        {
            if (rollDiceButton.GetComponent<Button>().interactable)
            {
                if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == ROLL)
                {
                    RollButtonPointerUp();
                }
            }
        }
    }

    // Roll Button 누르고 땔 시 Dice Bottle 던지기
    public void RollButtonPointerUp()
    {
        DC.ThrowBottle();
    }

    // RollButton REROLL로 바꾸기
    public void SetRollButtonReroll()
    {
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = REROLL;
    }


    #endregion

    #region 애니메이션 UI

    [PunRPC]
    // 라운드 공개 애니메이션
    private void ShowInGameAnimationRPC()
    {
        StartCoroutine(ShowInGameAnimationRPCCoroutine());
    }

    IEnumerator ShowInGameAnimationRPCCoroutine()
    {
        // 첫번째 플레이어일 경우 현재 라운드 공개
        if (currentPlayerSequence == 0)
        {
            showRoundUI.SetActive(true);
            showRoundUI.transform.GetChild(2).GetComponent<Text>().text = currentRound.ToString();
            yield return new WaitForSeconds(4f);
            showRoundUI.SetActive(false);
        }

        // 플레이어 공개
        showCurrentPlayerSequenceUI.SetActive(true);
        showCurrentPlayerSequenceUI.transform.GetChild(2).GetComponent<Image>().sprite = inGamePlayers[currentPlayerSequence].GetComponent<Image>().sprite;
        showCurrentPlayerSequenceUI.transform.GetChild(3).GetComponent<Image>().sprite = Players[currentPlayerSequence].GetPlayerIcon();
        showCurrentPlayerSequenceUI.transform.GetChild(4).GetComponent<Text>().text = (Players[currentPlayerSequence].GetPlayerNickName() + " Turn");

        yield return new WaitForSeconds(3f);

        showCurrentPlayerSequenceUI.SetActive(false);
    }

    // "낙" 애니메이션
    public void StartFailThrowDiceAnim()
    {
        PV.RPC("StartFailThrowDiceAnimRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void StartFailThrowDiceAnimRPC()
    {
        SetDice();
        StartCoroutine(StartFailThrowDiceAnimRPCCoroutine());
    }

    IEnumerator StartFailThrowDiceAnimRPCCoroutine()
    {
        failThrowDiceUI.SetActive(true);
        yield return new WaitForSeconds(2.5f);

        // 낙이 될 경우 리롤 횟수가 줄어드는 버그 예외처리
        Players[currentPlayerSequence].rerollCount++;
        failThrowDiceUI.SetActive(false);
    }

    // 리롤 횟수 감소 애니메이션
    public void StartRerollCountDicreaseAnim()
    {
        PV.RPC("StartRerollCountDicreaseAnimRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void StartRerollCountDicreaseAnimRPC()
    {
        StartCoroutine(StartRerollCountDicreaseAnimRPCCoroutine());
    }

    IEnumerator StartRerollCountDicreaseAnimRPCCoroutine()
    {
        rerollCountDicreaseUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        rerollCountDicreaseUI.SetActive(false);
    }

    // 모든 라운드가 끝나고 플레이어 등수 보여주는 애니메이션
    private void StartShowPlayersRankingAnim()
    {
        showPlayerRankingUI.SetActive(true);

        // 플레이어 등수 정렬
        SortPlayersByTotalScore();

        // 플레이어 수에 따라 UI 개수 세팅
        for(int i=0; i < playerRankingList.Count; i++)
        {
            if(i<Players.Count)
            {
                int index = Players.Count - i - 1;
                playerRankingList[i].SetActive(true);
                playerRankingList[i].transform.GetChild(1).GetComponent<Image>().sprite = inGamePlayers[Players[index].GetPlayerSequence()].GetComponent<Image>().sprite;
                playerRankingList[i].transform.GetChild(2).GetComponent<Image>().sprite = Players[index].GetPlayerIcon();
                playerRankingList[i].transform.GetChild(3).GetComponent<Text>().text = Players[index].GetPlayerNickName();
                playerRankingList[i].transform.GetChild(5).GetComponent<Text>().text = "총점 : " + Players[index].totalScore + "점";
            }
            else
            {
                playerRankingList[i].SetActive(false);
            }
        }

        // 플레이어 수에 따라 적절한 애니메이션 발동
        showPlayerRankingUI.GetComponent<Animator>().SetTrigger(Players.Count.ToString());
    }

    // 플레이어 리스트 정렬(플레이어 총합 기준)
    private void SortPlayersByTotalScore()
    {
        Players.Sort((p1, p2) => p1.totalScore.CompareTo(p2.totalScore));
    }

    #endregion

    #region 게임이 끝나면 기능하는 함수



    // 게임 다시 시작
    public void RestartGame()
    {
        PV.RPC("RestartGameRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RestartGameRPC()
    {
        StartCoroutine(RestartGameRPCCoroutine());
    }

    IEnumerator RestartGameRPCCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        InitGameSetting();
        foreach (var player in Players)
        {
            player.InitPlayerSetting();
        }
        currentRound--; // NextPlayer()에서 라운드 증가하는 처리 제외

        NextPlayer();
    }

    // 게임 종료 (서버 나가기)
    public void ExitGame()
    {
        bool isCurrentSequence = (MyPlayer.GetPlayerSequence() == currentPlayerSequence) ? true : false;
        PV.RPC("ResetInGamePlayersRPC", RpcTarget.AllBuffered, isCurrentSequence);
        StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator ExitGameCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        // 로딩화면 켜기
        uiManager.SetLoadingUI(true);

        // 게임 초기화
        InitGame();
        InitGameSetting();

        // Chat Button 초기화
        chatManager.ResetChatText();

        // 방 나가기
        NM.LeftRoom();

        // 카메라 설정 초기화
        mainCamera.transform.position = new Vector3(2.52f, 4.65f, -13.53f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(-19f, 0f, 0f));
    }

    [PunRPC]
    private void ResetInGamePlayersRPC(bool isCurrentSequence)
    {
        StartCoroutine(ResetInGamePlayersRPCCoroutine(isCurrentSequence));
    }

    IEnumerator ResetInGamePlayersRPCCoroutine(bool isCurrentSequence)
    {
        yield return new WaitForSeconds(1f);

        // 플레이어 컨테이너 위치 세팅
        SetPlayerContainerPos(Players.Count);

        // 플레이어 스코어 보드 초기 세팅
        scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);

        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if (i < Players.Count)
            {
                inGamePlayers[i].SetActive(true);
                inGamePlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
                inGamePlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
            }
            else
            {
                inGamePlayers[i].SetActive(false);
            }
        }

        // 현재 플레이 중이면 다음 플레이어 실행
        if (isCurrentSequence) NextPlayer();

    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // 반환 함수
    public List<GameObject> GetInGamePlayerList() { return inGamePlayers; }
}
