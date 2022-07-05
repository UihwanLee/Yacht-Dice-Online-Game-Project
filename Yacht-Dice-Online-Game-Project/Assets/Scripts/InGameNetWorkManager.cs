using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

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

    [Header("Effect")]
    [SerializeField]
    private DiceController diceController;

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
    }

    private void Start()
    {
        PV = photonView;
    }

    public void SetInGamePlayer(List<PlayerController> _Players)
    {
        if (_Players != null) Players = _Players;
        else Debug.Log("There is no Players List!");
    }

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

        // �÷��̾� ��ư ����
    }

    #endregion

    #region Room ����


    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
