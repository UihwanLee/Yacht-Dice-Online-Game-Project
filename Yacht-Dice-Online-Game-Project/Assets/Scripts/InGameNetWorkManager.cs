using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;

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

    [Header("Controller")]
    [SerializeField]
    private DiceController diceController;

    [Header("Objects")]
    [SerializeField]
    private GameObject diceBottle;

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();
    // ���� �÷��̾� ���� : �� ������ ���� �÷��̾� �÷��̸� ������� �� �� �ְ��Ѵ�.
    private int currentPlayerSequence; 

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("PV")]
    public PhotonView PV;

    [Header("Player")]
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
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);

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



    [PunRPC]
    public void SetAllInGamePlayerRPC()
    {

        // �������� �ֻ��� ȿ�� ����
        diceController.FallingDice(false);

        // �г� ����
        roomPanel.SetActive(false);
        inGamePanel.SetActive(true);

        // ī�޶� ����
        mainCamera.transform.position = new Vector3(-180.5f, 16.45f, -1.15f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(81.464f, 0f, 0f));

        // �÷��̾� ���� ���
        Players = NM.Players;

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

        // ù��° �÷��̾� �Ұ��ϴ� �ִϸ��̼� ���� ù��° �÷��̾� ������ �����Ѵ�.
        SetPlayerPlaying();
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

    // �÷��̾� ���� ����
    public void SetPlayerPlaying()
    {
        // ���� �÷��̾� ������ ����(localPosition�� �ƴ� position���� ����)
        Vector3 movePos = inGamePlayers[currentPlayerSequence].transform.localPosition;
        movePos.y = 740f;
        inGamePlayers[currentPlayerSequence].transform.localPosition = movePos;
    }

    // ���̽� ����
    public void SetDice()
    {
        // ���̽� �� ��ġ �̵�
        diceBottle.transform.localPosition = new Vector3(-180.27f, 6.07f, 0.16f);
    }


    #endregion

    #region �̸�Ƽ��

    public void OnClickEmoticonButton()
    {
        if(!isEmoticonContainer)
        {
            emoticonContainer.SetActive(true);
            emoticonContainer.GetComponent<Animator>().SetTrigger("on");
        }
        else
        {
            emoticonContainer.GetComponent<Animator>().SetTrigger("off");
            /*
            if(!emoticonContainer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Anim_EmoticonsUnscroll"))
            {
                emoticonContainer.SetActive(false);
            }
            */
        }

        isEmoticonContainer = !isEmoticonContainer;
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
