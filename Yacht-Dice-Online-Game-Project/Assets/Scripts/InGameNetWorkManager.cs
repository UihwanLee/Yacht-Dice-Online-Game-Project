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

    [Header("Controller")]
    [SerializeField]
    private DiceController diceController;

    [Header("Objects")]
    [SerializeField]
    private GameObject diceBottle;

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();

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

        // �г� ����
        inGamePanel.SetActive(false);

        Players.Clear();
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
    }

    // Player Container ������ ����
    public void SetPlayerContainerPos(int count)
    {
        // �÷��̾� ���� ���� ������ ��ġ�� �̵���Ų��.
        switch(count)
        {
            case 1:
                playerContainer.transform.localPosition = new Vector3(-65f, 860f, 0);
                break;
            case 2:
                playerContainer.transform.localPosition = new Vector3(-180f, 860f, 0);
                break;
            case 3:
                playerContainer.transform.localPosition = new Vector3(-300f, 860f, 0);
                break;
            case 4:
                playerContainer.transform.localPosition = new Vector3(-440f, 860f, 0);
                break;
            default:
                Debug.Log("[ERROR]No match!");
                break;
        }
    }


    #endregion

    #region �ΰ��� �÷���

    // ���̽� ����
    public void SetDice()
    {
        // ���̽� �� ��ġ �̵�
        diceBottle.transform.localPosition = new Vector3(-180.27f, 6.07f, 0.16f);
    }


    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
