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
    private GameObject rollDiceButton;
    [SerializeField]
    private GameObject rerollCountUI;

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
    [SerializeField]
    private GameObject showRoundUI;
    [SerializeField]
    private GameObject showCurrentPlayerSequenceUI;
    [SerializeField]
    private GameObject failThrowDiceUI;
    [SerializeField]
    private GameObject rerollCountDicreaseUI;

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


    public static InGameNetWorkManager IN;

    private void Awake()
    {
        // �ν��Ͻ� �ʱ�ȭ
        IN = this;

        //  ������Ʈ �ʱ�ȭ
        inGamePanel.SetActive(true);
        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if (inGamePlayers[i]) inGamePlayers[i].SetActive(false);
        }
        DC.SetBottleInitPosByTransform();

        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;

        // rerollCountUI ��Ȱ��ȭ
        rerollCountUI.SetActive(false);

        // Animation UI ��Ȱ��ȭ
        showRoundUI.SetActive(false);
        showCurrentPlayerSequenceUI.SetActive(false);
        failThrowDiceUI.SetActive(false);
        rerollCountDicreaseUI.SetActive(false);

        // �г� ����
        inGamePanel.SetActive(false);


        Players.Clear();
        currentPlayerSequence = 0;
        currentRound = 1;
    }

    private void Start()
    {
        PV = photonView;

        inGamePlayerIndex = 0;
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
        PV.RPC("SetAllInGamePlayerRPC", RpcTarget.All);

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
        Players = NM.Players;
        MyPlayer = NM.MyPlayer;

        // �÷��̾� �ΰ��� ����
        InitPlayersInGameSetting();
       
        // �÷��̾� �����̳� ��ġ ����
        SetPlayerContainerPos(Players.Count);

        // �÷��̾� ���ھ� ���� �ʱ� ����
        scoreBoardManager.InitAllPlayerScoreBoard(Players.Count);
       
        // �ΰ��� �÷��̾� ����
        for (int i = 0; i < Players.Count; i++)
        {
            inGamePlayers[i].SetActive(true);
            inGamePlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
            inGamePlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
        }
       
    }

    // �÷��̾� �ΰ��� ���� �ʱ�ȭ
    private void InitPlayersInGameSetting()
    {
        int index = 0;
        foreach(var player in Players)
        {
            player.SetPlayerSequence(index);
            index++;
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
        PV.RPC("SetCurrentPlayerSequenceRPC", RpcTarget.All, currentPlayerSequence);
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

        SetPlayerPlaying();
    }

    // �÷��̾� �÷��� ����
    public void SetPlayerPlaying()
    {
        // ��� �÷��̾ ���� ��ġ�� ���带 ������Ű�� �ٽ� ù �÷��̾���� ����
        PV.RPC("CheckRound", RpcTarget.All);

        // �÷��̾� �ΰ��� ���� ����
        ResetPlayerInGameSetting();

        // �ִϸ��̼� �÷���
        PV.RPC("ShowInGameAnimationRPC", RpcTarget.All);
        
        // ù��° �÷��̾� �Ұ��ϴ� �ִϸ��̼� ���� ù��° �÷��̾� ������ �����Ѵ�.
        PV.RPC("SetPlayerPlayingRPC", RpcTarget.All);

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
                Debug.Log("���� ����!");
            }
        }
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
        // ���� �÷��̾�� ������ ����
        Vector3 movePos = inGamePlayers[MyPlayer.GetPlayerSequence()].transform.localPosition;
        movePos.y = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? 690f : 740f;
        inGamePlayers[MyPlayer.GetPlayerSequence()].transform.localPosition = movePos;
        Debug.Log(MyPlayer.GetPlayerSequence() + " / " + movePos.y);

        // ��� �÷��̾� Roll Button interactable ��Ȱ��ȭ / ���� �÷��̾ Roll Button Interactable Ȱ��ȭ
        bool isCurrentPlayer = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? true : false;
        SetInterctableRollButton(isCurrentPlayer);

        // Roll -> Set ���� ����
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = SET;

    }


    // �� ��ư Ȱ��ȭ/��Ȱ��ȭ
    private void SetInterctableRollButton(bool isInteractable)
    {
        rollDiceButton.GetComponent<Button>().interactable = isInteractable;
    }

    public void SetRerollCountUI(bool isActive)
    {
        PV.RPC("SetRerollCountUIRPC", RpcTarget.All, isActive);
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
            PV.RPC("SetDiceRPC", RpcTarget.All);

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
        PV.RPC("SetAllDiceKinematicRPC", RpcTarget.All);
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

        yield return new WaitForSeconds(4f);

        showCurrentPlayerSequenceUI.SetActive(false);
    }

    // "��" �ִϸ��̼�
    public void StartFailThrowDiceAnim()
    {
        PV.RPC("StartFailThrowDiceAnimRPC", RpcTarget.All);
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
        PV.RPC("StartRerollCountDicreaseAnimRPC", RpcTarget.All);
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

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // ��ȯ �Լ�
    public List<GameObject> GetInGamePlayerList() { return inGamePlayers; }
}
