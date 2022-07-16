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
     * InGameNetWorkManager ��ũ��Ʈ
     *
     * Room Panel Network ����
     * InGame NetWork ����
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

    // DiceButton 3���� ������ư
    private string SET = "Set";
    private string ROLL = "Roll";
    private string REROLL = "Reroll";

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();
    // ���� �÷��̾� ���� : �� ������ ���� �÷��̾� �÷��̸� ������� �� �� �ְ��Ѵ�.
    public int currentPlayerSequence;
    public int currentRound;

    public List<Dice> newDices = new List<Dice>();

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    // �ִϸ��̼� ���� ����
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
    // NetworkManager ��ũ��Ʈ���� �ް� �� �÷��̾� ����
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
        // �ν��Ͻ� �ʱ�ȭ
        IN = this;

        InitGame();

        // ���� Reset �ʿ� ����
        InitGameSetting();
    }

    private void InitGame()
    {
        //  ������Ʈ �ʱ�ȭ
        inGamePanel.SetActive(true);
        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if (inGamePlayers[i]) inGamePlayers[i].SetActive(false);
        }
        DC.SetBottleInitPosByTransform();

        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;

        // �г� ����
        inGamePanel.SetActive(false);


        Players.Clear();
    }

    private void InitGameSetting()
    {
        currentPlayerSequence = 0;
        currentRound = 1;
        inGamePlayerIndex = 0;

        // rerollCountUI ��Ȱ��ȭ
        rerollCountUI.SetActive(false);

        // Animation UI ��Ȱ��ȭ
        challengeSuccess.SetActive(false);
        showRoundUI.SetActive(false);
        showCurrentPlayerSequenceUI.SetActive(false);
        failThrowDiceUI.SetActive(false);
        rerollCountDicreaseUI.SetActive(false);
        showPlayerRankingUI.SetActive(false);


        // restartORExitUI ��Ȱ��ȭ
        restartORExitUI.SetActive(false);

        // playerScoreBoard UI �ʱ�ȭ
        if(inGamePanel.activeSelf) scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);

        // ��ƼŬ ������Ʈ �ʱ�ȭ
        yachtSuccessFireWorks.SetActive(false);
    }


    private void Start()
    {
        PV = photonView;
    }

    #region Room ����

    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

    #region �ΰ��� ���� ����


    // �ΰ��� �÷��̾� ����
    public void SetSetAllInGamePlayer()
    {
        PV.RPC("SetAllInGamePlayerRPC", RpcTarget.AllBuffered);

        // ù��° �÷��̾� �ٷ� �÷���
        SetPlayerPlaying();
    }

    [PunRPC]
    public void SetAllInGamePlayerRPC()
    {

        // �������� �ֻ��� ȿ�� ����
        DC.FallingDice(false);

        // �г� ����
        roomPanel.SetActive(false);
        inGamePanel.SetActive(true);

        // ī�޶� ����
        mainCamera.transform.position = new Vector3(-180.5f, 16.45f, -1.15f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(81.464f, 0f, 0f));

        // �÷��̾� ���� ���
        NM.SortPlayersBySequence(); // �÷��̾� ������� �ٲ� �� ���� ���
        Players = NM.Players;
        MyPlayer = NM.MyPlayer;
       
        // �÷��̾� �����̳� ��ġ ����
        SetPlayerContainerPos(Players.Count);

        // �÷��̾� ���ھ� ���� �ʱ� ����
        scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);

        // �ΰ��� �÷��̾� ����
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

    // Player Container ������ ����
    private void SetPlayerContainerPos(int count)
    {
        // �÷��̾� ���� ���� ������ ��ġ�� �̵���Ų��.
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

    #region �ΰ��� �÷���

    // �÷��̾� ���� ���� ��, ���� �÷��̾� ����
    public void NextPlayer()
    {
        PV.RPC("SetCurrentPlayerSequenceRPC", RpcTarget.AllBuffered, currentPlayerSequence);
        DC.GetDices();
        DC.SetBottleInitPos();
        StartCoroutine(NextPlayerCoroutine());
    }

    // ���� �÷��̾� ���࿡ ���� �ʱ�ȭ ������ϴ� �������� ��� �ʱ�ȭ ���� �� �ֵ��� �Ѵ�.
    [PunRPC]
    private void SetCurrentPlayerSequenceRPC(int currentSeqiemce)
    {
        // isSelect �ʱ�ȭ
        DC.ResetDiceSelect();
        DC.ResetRemainDiceCount();
        // Dices ��� �Ѱ��ֱ�
        this.newDices = DC.Dices;

        // Dice ���� : Dice Bottle ��ġ �ʱ�ȭ, Dice ��ġ �ʱ�ȭ(�ֻ����� Dice Container ���� isKinetic���� ����
        DC.SetBottleInitPosByTransform();

        currentSeqiemce++;
        this.currentPlayerSequence = currentSeqiemce;
    }

    // �� �� ��� �� ����
    IEnumerator NextPlayerCoroutine()
    {
        SetInterctableRollButton(false);
        SetRerollCountUI(false);

        yield return new WaitForSeconds(3f);

        // ���� �ʱ�ȭ : ��� �ð� ���� �ִϸ��̼� Ʋ��

        // ScoreBoard ��Ȱ��ȭ
        scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);

        // ��� �÷��̾ ���� ��ġ�� ���带 ������Ű�� �ٽ� ù �÷��̾���� ����
        PV.RPC("CheckRound", RpcTarget.AllBuffered);

        if(currentRound<=12) SetPlayerPlaying();
    }

    // �÷��̾� �÷��� ����
    public void SetPlayerPlaying()
    {

        // �÷��̾� �ΰ��� ���� ����
        ResetPlayerInGameSetting();

        // �ִϸ��̼� �÷���
        PV.RPC("ShowInGameAnimationRPC", RpcTarget.AllBuffered);
        
        // ù��° �÷��̾� �Ұ��ϴ� �ִϸ��̼� ���� ù��° �÷��̾� ������ �����Ѵ�.
        PV.RPC("SetPlayerPlayingRPC", RpcTarget.AllBuffered);

        // �÷��̾� ���ھ� ���� ����
        scoreBoardManager.SetCurrentPlayerScoreBoard();
        scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);
    }
    
    [PunRPC]
    private void CheckRound()
    {
        if (currentPlayerSequence == Players.Count)
        {
            // ���� ���� ��, ù �÷��̾���� �ٽ� ����
            currentRound++;
            currentPlayerSequence = 0;

            // 12���尡 ������ ��, �÷��̾� ���� ���� -> ���� ��ǥ �� ���� ����
            if (currentRound > 12)
            {
                StartCoroutine(StartShowPlayersRankingCoroutine());
            }
        }
    }


    // �÷��̾� ��� ����
    IEnumerator StartShowPlayersRankingCoroutine()
    {
        scoreBoardManager.isOpenPlayersScoreBoard = false;
        scoreBoardManager.OnClickScoreBoardButton();

        yield return new WaitForSeconds(2f);

        // �÷��̾� ��� �����ִ� �ִϸ��̼� �߰�
        StartShowPlayersRankingAnim();

        yield return new WaitForSeconds(8f);

        // �ٽ� ���� or ������ UI Ȱ��ȭ
        restartORExitUI.SetActive(true);
    }

    // �÷��̾� �ΰ��� ���� ����
    private void ResetPlayerInGameSetting()
    {
        Players[currentPlayerSequence].ResetInGameSetting();
    }

    [PunRPC]
    // �÷��̾� ������� ����
    public void SetPlayerPlayingRPC()
    {
        // ���� �÷��̾� ������ �̵�
        MoveIconCurrentPlayer();

        // ��� �÷��̾� Roll Button interactable ��Ȱ��ȭ / ���� �÷��̾ Roll Button Interactable Ȱ��ȭ
        bool isCurrentPlayer = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? true : false;
        SetInterctableRollButton(isCurrentPlayer);

        // Roll -> Set ���� ����
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = SET;

    }

    private void MoveIconCurrentPlayer()
    {
        // �ַ� �÷��� �϶��� retrun
        if (Players.Count == 1) return;

        for(int i=0; i < Players.Count; i++)
        {
            if (i == currentPlayerSequence) inGamePlayers[i].transform.localPosition = new Vector3(inGamePlayers[i].transform.localPosition.x, 750f, 0f);
            else inGamePlayers[i].transform.localPosition = new Vector3(inGamePlayers[i].transform.localPosition.x, 790f, 0f);
        }

    }


    // �� ��ư Ȱ��ȭ/��Ȱ��ȭ
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
            rerollCountUI.transform.GetChild(1).GetComponent<Text>().text = Players[currentPlayerSequence].rerollCount + "ȸ";
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
            Debug.Log("Roll Burron ����!");
            // �÷��̾ ���̽� ó�� ���� ��� isSelect �ʱ�ȭ
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET)
            {
                DC.ResetDiceSelect();
            }

            // ���� UI ��Ȱ��ȭ
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            scoreBoardManager.ChangeSelectScore(-1, false);


            // ���� �� ��, ���� ���̽��� ���� �� �������� �ʰ� �Ѵ�.
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == REROLL)
            {
                DC.UpdateRemainDiceCount();
                if (DC.remainDiceCount == 0)
                {
                    Debug.Log("���� �ֻ����� ����");
                    return;
                }
                diceSelectManager.SortReturnZoneDice();

                // ���� Ƚ�� ����: ���� Ƚ���� ��� ���� ��, Reroll�� ���ϸ� ������ �����ؾ���
                Players[currentPlayerSequence].rerollCount--;
            }

            SetRerollCountUI(false);

            // ���ھ� ���� ��Ȱ��ȭ
            scoreBoardManager.SetActiveCurrentPlayerScoreBoard(false);

            // ���̽� �� ��ġ �̵�(Anim���� ����)
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET)
            {
                DC.SetBottlePlayingPos();
            }
            else
            {
                DC.ReBottlePlayingPos();
            }

            // ���̽� Bottle Set
            PV.RPC("SetDiceRPC", RpcTarget.AllBuffered);

            // ���̽� ��ȯ
            StartCoroutine(SpawnDiceInGameCorutine());
        }
        else Debug.Log("[ERROR]RollButton ���°� SET�̳� REROLL�� �ƴ�!");
    }
    

    [PunRPC]
    // ���̽� ���� RPC
    public void SetDiceRPC()
    {
        // Set -> Roll�� ����
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = ROLL;

        // ���̽� ��ȯ
        // pos -180.27 12.2 0.16
        DC.SetSpawnPos(new Vector3(-180.27f, 7.2f, 0.16f));
    }

    // ���̽� ��ȯ
    IEnumerator SpawnDiceInGameCorutine()
    {
        yield return new WaitForSeconds(1f);
        // ���̽� isKinematic �ʱ�ȭ
        PV.RPC("SetAllDiceKinematicRPC", RpcTarget.AllBuffered);
        // �̹� DicePrefab�� �����Ǿ� �ִ� ��� Reroll�� �۾�
        if (DC.Dices.Count == 0) DC.SpawnYachtDicesInGame(1);
        else
        {
            Debug.Log("01. �̹� �ֻ����� �����ؼ� �ٽõ���!");
            if(Players[currentPlayerSequence].rerollCount!=2) StartRerollCountDicreaseAnim();
            DC.RerollYachtDices();
        }
    }

    [PunRPC]
    private void SetAllDiceKinematicRPC()
    {
        DC.SetAllDiceKinematic(false);
    }

    // Dice Roll �õ�
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

    // Roll Button ������ ���� �� ��� Dice �� ����
    public void RollButtonPointerDown()
    {
        DC.ShakingBottle();
    }

    // Dice Roll �õ�
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

    // Roll Button ������ �� �� Dice Bottle ������
    public void RollButtonPointerUp()
    {
        DC.ThrowBottle();
    }

    // RollButton REROLL�� �ٲٱ�
    public void SetRollButtonReroll()
    {
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = REROLL;
    }


    #endregion

    #region �ִϸ��̼� UI

    [PunRPC]
    // ���� ���� �ִϸ��̼�
    private void ShowInGameAnimationRPC()
    {
        StartCoroutine(ShowInGameAnimationRPCCoroutine());
    }

    IEnumerator ShowInGameAnimationRPCCoroutine()
    {
        // ù��° �÷��̾��� ��� ���� ���� ����
        if (currentPlayerSequence == 0)
        {
            showRoundUI.SetActive(true);
            showRoundUI.transform.GetChild(2).GetComponent<Text>().text = currentRound.ToString();
            yield return new WaitForSeconds(4f);
            showRoundUI.SetActive(false);
        }

        // �÷��̾� ����
        showCurrentPlayerSequenceUI.SetActive(true);
        showCurrentPlayerSequenceUI.transform.GetChild(2).GetComponent<Image>().sprite = inGamePlayers[currentPlayerSequence].GetComponent<Image>().sprite;
        showCurrentPlayerSequenceUI.transform.GetChild(3).GetComponent<Image>().sprite = Players[currentPlayerSequence].GetPlayerIcon();
        showCurrentPlayerSequenceUI.transform.GetChild(4).GetComponent<Text>().text = (Players[currentPlayerSequence].GetPlayerNickName() + " Turn");

        yield return new WaitForSeconds(3f);

        showCurrentPlayerSequenceUI.SetActive(false);
    }

    // "��" �ִϸ��̼�
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

        // ���� �� ��� ���� Ƚ���� �پ��� ���� ����ó��
        Players[currentPlayerSequence].rerollCount++;
        failThrowDiceUI.SetActive(false);
    }

    // ���� Ƚ�� ���� �ִϸ��̼�
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

    // ��� ���尡 ������ �÷��̾� ��� �����ִ� �ִϸ��̼�
    private void StartShowPlayersRankingAnim()
    {
        showPlayerRankingUI.SetActive(true);

        // �÷��̾� ��� ����
        SortPlayersByTotalScore();

        // �÷��̾� ���� ���� UI ���� ����
        for(int i=0; i < playerRankingList.Count; i++)
        {
            if(i<Players.Count)
            {
                int index = Players.Count - i - 1;
                playerRankingList[i].SetActive(true);
                playerRankingList[i].transform.GetChild(1).GetComponent<Image>().sprite = inGamePlayers[Players[index].GetPlayerSequence()].GetComponent<Image>().sprite;
                playerRankingList[i].transform.GetChild(2).GetComponent<Image>().sprite = Players[index].GetPlayerIcon();
                playerRankingList[i].transform.GetChild(3).GetComponent<Text>().text = Players[index].GetPlayerNickName();
                playerRankingList[i].transform.GetChild(5).GetComponent<Text>().text = "���� : " + Players[index].totalScore + "��";
            }
            else
            {
                playerRankingList[i].SetActive(false);
            }
        }

        // �÷��̾� ���� ���� ������ �ִϸ��̼� �ߵ�
        showPlayerRankingUI.GetComponent<Animator>().SetTrigger(Players.Count.ToString());
    }

    // �÷��̾� ����Ʈ ����(�÷��̾� ���� ����)
    private void SortPlayersByTotalScore()
    {
        Players.Sort((p1, p2) => p1.totalScore.CompareTo(p2.totalScore));
    }

    #endregion

    #region ������ ������ ����ϴ� �Լ�



    // ���� �ٽ� ����
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
        currentRound--; // NextPlayer()���� ���� �����ϴ� ó�� ����

        NextPlayer();
    }

    // ���� ���� (���� ������)
    public void ExitGame()
    {
        bool isCurrentSequence = (MyPlayer.GetPlayerSequence() == currentPlayerSequence) ? true : false;
        PV.RPC("ResetInGamePlayersRPC", RpcTarget.AllBuffered, isCurrentSequence);
        StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator ExitGameCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        // �ε�ȭ�� �ѱ�
        uiManager.SetLoadingUI(true);

        // ���� �ʱ�ȭ
        InitGame();
        InitGameSetting();

        // Chat Button �ʱ�ȭ
        chatManager.ResetChatText();

        // �� ������
        NM.LeftRoom();

        // ī�޶� ���� �ʱ�ȭ
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

        // �÷��̾� �����̳� ��ġ ����
        SetPlayerContainerPos(Players.Count);

        // �÷��̾� ���ھ� ���� �ʱ� ����
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

        // ���� �÷��� ���̸� ���� �÷��̾� ����
        if (isCurrentSequence) NextPlayer();

    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // ��ȯ �Լ�
    public List<GameObject> GetInGamePlayerList() { return inGamePlayers; }
}
