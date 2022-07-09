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

    // DiceButton 3가지 설정버튼
    private string SET = "Set";
    private string ROLL = "Roll";
    private string REROLL = "Reroll";

    [Header("Player")]
    [SerializeField]
    private List<Sprite> playerIcons = new List<Sprite>();
    // 현재 플레이어 순서 : 이 변수에 따라 플레이어 플레이를 순서대로 할 수 있게한다.
    public int currentPlayerSequence; 

    [Header("Main Camera")]
    [SerializeField]
    private Camera mainCamera;

    [Header("PV")]
    public PhotonView PV;

    [Header("Player")]
    public PlayerController MyPlayer;
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
        DC.SetBottleInitPos();

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


    // 인게임 플레이어 세팅
    public void SetSetAllInGamePlayer()
    {
        PV.RPC("SetAllInGamePlayerRPC", RpcTarget.All);

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
        Players = NM.Players;
        MyPlayer = NM.MyPlayer;

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

    // 플레이어 플레이 설정
    public void SetPlayerPlaying()
    {
        // 첫번째 플레이어 소개하는 애니메이션 이후 첫번째 플레이어 순서를 설정한다.
        PV.RPC("SetPlayerPlayingRPC", RpcTarget.All);
    }

    [PunRPC]
    // 플레이어 순서 설정
    public void SetPlayerPlayingRPC()
    {
        // 현재 플레이어 아이콘 수정(localPosition이 아닌 position으로 수정)
        Vector3 movePos = inGamePlayers[currentPlayerSequence].transform.localPosition;
        movePos.y = 740f;
        inGamePlayers[currentPlayerSequence].transform.localPosition = movePos;

        // 모든 플레이어 Roll Button interactable 비활성화 / 현재 플레이어만 Roll Button Interactable 활성화
        bool isCurrentPlayer = (Players[currentPlayerSequence].GetPlayerNickName() == MyPlayer.GetPlayerNickName()) ? true : false;
        rollDiceButton.GetComponent<Button>().interactable = isCurrentPlayer;

        // Roll -> Set 으로 설정
        rollDiceButton.transform.GetChild(0).GetComponent<Text>().text = SET;

    }

    public void SetDice()
    {
        if(rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET || rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == REROLL)
        {
            // 플레이어가 다이스 처음 돌릴 경우 isSelect 초기화
            if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET) DC.ResetDiceSelect();

            // 다이스 Bottle Set
            PV.RPC("SetDiceRPC", RpcTarget.All);

            // 다이스 소환
            StartCoroutine(SpawnDiceInGameCorutine());
        }
    }
    

    [PunRPC]
    // 다이스 설정 RPC
    public void SetDiceRPC()
    {
        // 다이스 병 위치 이동(Anim으로 세팅)
        if (rollDiceButton.transform.GetChild(0).GetComponent<Text>().text == SET) DC.SetBottlePlayingPos();
        else DC.ReBottlePlayingPos();

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
        // 이미 DicePrefab이 생성되어 있는 경우 Reroll만 작업
        if (DC.Dices.Count == 0) DC.SpawnYachtDicesInGame(1);
        else
        {
            Debug.Log("리롤 or 이미 생성됨");
            DC.RerollYachtDices();
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
