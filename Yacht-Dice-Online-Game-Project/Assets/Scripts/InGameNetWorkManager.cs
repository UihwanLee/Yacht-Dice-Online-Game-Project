using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class InGameNetWorkManager : MonoBehaviourPunCallbacks, IPunObservable
{
    /*
     * InGameNetWorkManager 스크립트
     *
     * Room Panel Network 감독
     * InGame NetWork 감독
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
    // NetworkManager 스크립트에서 받게 될 플레이어 정보
    public List<PlayerController> Players = new List<PlayerController>(); 

    public static InGameNetWorkManager IN;

    private void Awake()
    {
        // 인스턴스 초기화
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

    #region 인게임 세팅 설정

    [PunRPC]
    public void SetAllInGamePlayerRPC()
    {
        // 떨어지는 주사위 효과 끄기
        diceController.FallingDice(false);

        // 패널 정리
        roomPanel.SetActive(false);
        inGamePanel.SetActive(true);

        // 카메라 설정
        mainCamera.transform.position = new Vector3(-180.5f, 16.45f, -1.15f);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(81.464f, 0f, 0f));

        // 플레이어 버튼 세팅
    }

    #endregion

    #region Room 설정


    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
