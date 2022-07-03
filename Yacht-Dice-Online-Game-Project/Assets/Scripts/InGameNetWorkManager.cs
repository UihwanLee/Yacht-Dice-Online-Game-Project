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

    [Header("TitlePanel")]
    [SerializeField]
    private GameObject titlePanel;

    [Header("LobbyPanel")]
    [SerializeField]
    private GameObject lobbyPanel;

    [Header("RoomPanel")]
    [SerializeField]
    private GameObject roomPanel;
    [SerializeField]
    private List<GameObject> roomPlayers = new List<GameObject>();

    [Header("Effect")]
    [SerializeField]
    private DiceController diceController;

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();

    [Header("PV")]
    public PhotonView PV;

    private int roomSlotIndex;

    public static InGameNetWorkManager IN;

    private void Awake()
    {
        // 인스턴스 초기화
        IN = this;

        // UI 오브젝트 초기화
        for (int i = 0; i < roomPlayers.Count; i++)
        {
            roomPlayers[i].SetActive(false);
        }
    }

    private void Start()
    {
        PV = photonView;

        roomSlotIndex = 0;
    }

    #region 카메라 설정



    #endregion

    #region Room 설정

    // RoomPlayerList에 플레이어 배치
    public void SetRoomPlayer(Sprite playerIcon, string playerNickName)
    {
        // 플레이어 정보 입력
        Debug.Log("자신이 들어갈 인덱스: " + roomSlotIndex + " 플레이어 이름: " + playerNickName);
        roomPlayers[roomSlotIndex].SetActive(true);
        roomPlayers[roomSlotIndex].transform.GetChild(0).GetComponent<Image>().sprite = playerIcon;
        roomPlayers[roomSlotIndex++].transform.GetChild(1).GetComponent<Text>().text = playerNickName;
    }


    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
