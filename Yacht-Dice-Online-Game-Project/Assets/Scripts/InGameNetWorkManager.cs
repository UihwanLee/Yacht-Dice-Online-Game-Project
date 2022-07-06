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
    // 현재 플레이어 순서 : 이 변수에 따라 플레이어 플레이를 순서대로 할 수 있게한다.
    private int currentPlayerSequence; 

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

        //  오브젝트 초기화
        inGamePanel.SetActive(true);
        for (int i = 0; i < inGamePlayers.Count; i++)
        {
            if (inGamePlayers[i]) inGamePlayers[i].SetActive(false);
        }
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);

        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;

        // 패널 정리
        inGamePanel.SetActive(false);

        Players.Clear();
        currentPlayerSequence = 0;
    }

    private void Start()
    {
        PV = photonView;

        inGamePlayerIndex = 0;
    }

    #region Room 설정

    public Sprite GetPlayerIconByIndex(int index)
    {
        return playerIcons[index];
    }

    #endregion

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

        // 플레이어 정보 얻기
        Players = NM.Players;

        // 플레이어 컨테이너 위치 세팅
        SetPlayerContainerPos(Players.Count);

        // 인게임 플레이아 세팅
        for (int i = 0; i < Players.Count; i++)
        {
            Debug.Log(i + "번째 플레이어 배치! 플레이어 닉네임 : " + Players[i].GetPlayerNickName());
            inGamePlayers[i].SetActive(true);
            inGamePlayers[i].transform.GetChild(0).GetComponent<Image>().sprite = Players[i].GetPlayerIcon();
            inGamePlayers[i].transform.GetChild(1).GetComponent<Text>().text = Players[i].GetPlayerNickName();
        }

        // 첫번째 플레이어 소개하는 애니메이션 이후 첫번째 플레이어 순서를 설정한다.
        SetPlayerPlaying();
    }

    // Player Container 포지션 설정
    public void SetPlayerContainerPos(int count)
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

    // 플레이어 순서 설정
    public void SetPlayerPlaying()
    {
        // 현재 플레이어 아이콘 수정(localPosition이 아닌 position으로 수정)
        Vector3 movePos = inGamePlayers[currentPlayerSequence].transform.localPosition;
        movePos.y = 740f;
        inGamePlayers[currentPlayerSequence].transform.localPosition = movePos;
    }

    // 다이스 설정
    public void SetDice()
    {
        // 다이스 병 위치 이동
        diceBottle.transform.localPosition = new Vector3(-180.27f, 6.07f, 0.16f);
    }


    #endregion

    #region 이모티콘

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
