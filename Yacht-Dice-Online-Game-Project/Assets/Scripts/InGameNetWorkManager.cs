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

    // DiceButton 3���� ������ư
    private string SET = "Set";
    private string ROLL = "Roll";
    private string REROLL = "Reroll";

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();
    // ���� �÷��̾� ���� : �� ������ ���� �÷��̾� �÷��̸� ������� �� �� �ְ��Ѵ�.
    public int currentPlayerSequence; 

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("PV")]
    public PhotonView PV;

    [Header("Player")]
    public PlayerController MyPlayer;
    // NetworkManager ��ũ��Ʈ���� �ް� �� �÷��̾� ����
    public List<PlayerController> Players = new List<PlayerController>(); 


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
        DC.SetBottleInitPos();

        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;

        // �г� ����
        inGamePanel.SetActive(false);

        Players.Clear();
        currentPlayerSequence = 0;
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

        // �÷��̾� �����̳� ��ġ ����
        SetPlayerContainerPos(Players.Count);

        // �ΰ��� �÷��̾� ����
        for (int i = 0; i < Players.Count; i++)
        {
            Debug.Log(i + "��° �÷��̾� ��ġ! �÷��̾� �г��� : " + Players[i].GetPlayerNickName());
            inGamePlayers[i].SetActive(true);
            inGamePlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
            inGamePlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
        }

    }

    // Player Container ������ ����
    public void SetPlayerContainerPos(int count)
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

    // �÷��̾� �÷��� ����
    public void SetPlayerPlaying()
    {
        // ù��° �÷��̾� �Ұ��ϴ� �ִϸ��̼� ���� ù��° �÷��̾� ������ �����Ѵ�.
        PV.RPC("SetPlayerPlayingRPC", RpcTarget.All);
    }

    [PunRPC]
    // �÷��̾� ���� ����
    public void SetPlayerPlayingRPC()
    {
        // ���� �÷��̾� ������ ����(localPosition�� �ƴ� position���� ����)
        Vector3 movePos = inGamePlayers[currentPlayerSequence].transform.localPosition;
        movePos.y = 740f;
        inGamePlayers[currentPlayerSequence].transform.localPosition = movePos;

        // ��� �÷��̾� Roll Button interactable ��Ȱ��ȭ / ���� �÷��̾ Roll Button Interactable Ȱ��ȭ
        bool isCurrentPlayer = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? true : false;
        rollDiceButton.GetComponent<Button>().interactable = isCurrentPlayer;

        // Roll -> Set ���� ����
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = SET;

    }

    public void SetDice()
    {
        if(rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET || rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == REROLL)
        {
            // �÷��̾ ���̽� ó�� ���� ��� isSelect �ʱ�ȭ
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET) DC.ResetDiceSelect();

            // ���̽� Bottle Set
            PV.RPC("SetDiceRPC", RpcTarget.All);

            // ���̽� ��ȯ
            StartCoroutine(SpawnDiceInGameCorutine());
        }
    }
    

    [PunRPC]
    // ���̽� ���� RPC
    public void SetDiceRPC()
    {
        // ���̽� �� ��ġ �̵�(Anim���� ����)
        if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET) DC.SetBottlePlayingPos();
        else DC.ReBottlePlayingPos();

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
        // �̹� DicePrefab�� �����Ǿ� �ִ� ��� Reroll�� �۾�
        if (DC.Dices.Count == 0) DC.SpawnYachtDicesInGame(1);
        else
        {
            Debug.Log("���� or �̹� ������");
            DC.RerollYachtDices();
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
