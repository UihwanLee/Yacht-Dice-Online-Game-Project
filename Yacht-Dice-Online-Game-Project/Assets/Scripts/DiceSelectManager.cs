using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class DiceSelectManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject diceSelcetUI;

    [Header("Variables")]
    // ReturnZone, SelectZone Pos, Rot 저장 변수
    private List<Vector3> returnZonePos = new List<Vector3>();
    private List<Vector3> selectZonePos_5Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_4Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_3Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_2Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_1Dice = new List<Vector3>();

    private Vector3 returnZoneRot;
    private Vector3 selectZoneRot;

    // ReturnZone, SelectZone UI 변수
    [SerializeField]
    private List<GameObject> returnZoneDice = new List<GameObject>();

    [SerializeField]
    private GameObject dice5Container;
    [SerializeField]
    private GameObject dice4Container;
    [SerializeField]
    private GameObject dice3Container;
    [SerializeField]
    private GameObject dice2Container;
    [SerializeField]
    private GameObject dice1Container;

    [SerializeField]
    private List<GameObject> dice5 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice4 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice3 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice2 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice1 = new List<GameObject>();


    // 선택된 SelectButtonUIList
    private List<GameObject> currentSelectZoneList = new List<GameObject>();

    // 선택된 SelectPosList
    private List<Vector3> currentSelectPosList = new List<Vector3>();

    // Select UI
    [Header("Select UI")]
    public GameObject selectZoneSelectUI;
    public GameObject returnZoneSelectUI;

    [Header("Scripts")]
    [SerializeField]
    private DiceController diceController;

    [Header("PV")]
    public PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        PV = photonView;

        // returnZonPos 초기화
        returnZonePos.Add(new Vector3(-182.41f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-181.45f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-180.49f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-179.53f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-178.57f, 3.5f, 3f));

        // selectZonePos_5Dice 초기화
        selectZonePos_5Dice.Add(new Vector3(-182.1f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-181.3f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-179.7f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-178.9f, 9.71f, 0.05f));

        // selectZonePos_4Dice 초기화
        selectZonePos_4Dice.Add(new Vector3(-181.7f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.9f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.1f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-179.3f, 9.71f, 0.05f));

        // selectZonePos_3Dice 초기화
        selectZonePos_3Dice.Add(new Vector3(-181.3f, 9.71f, 0.05f));
        selectZonePos_3Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));
        selectZonePos_3Dice.Add(new Vector3(-179.7f, 9.71f, 0.05f));

        // selectZonePos_2Dice 초기화
        selectZonePos_2Dice.Add(new Vector3(-180.9f, 9.71f, 0.05f));
        selectZonePos_2Dice.Add(new Vector3(-180.1f, 9.71f, 0.05f));

        // selectZonePos_1Dice 초기화
        selectZonePos_1Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));


        // returnZonRot 초기화
        returnZoneRot = new Vector3(-14f, 0f, 0f);

        // selectZoneRot 초기화
        selectZoneRot = new Vector3(-7.68f, 0f, 0f);

        diceSelcetUI.SetActive(true);
        // SelectZone, ReturnZone UI 초기화
        for (int i = 0; i < returnZoneDice.Count; i++) returnZoneDice[i].SetActive(false);
        dice5Container.SetActive(false);
        dice4Container.SetActive(false);
        dice3Container.SetActive(false);
        dice2Container.SetActive(false);
        dice1Container.SetActive(false);

        // Select UI 비활성화
        selectZoneSelectUI.SetActive(false);
        returnZoneSelectUI.SetActive(false);

    }

    // ReturnZone UI 활성화/비활성화
    public void SetReturnZoneAcitve(bool isActive)
    {
        for (int i = 0; i < returnZoneDice.Count; i++) returnZoneDice[i].SetActive(isActive);
    }

    // SelectZoneUI 업데이트
    public void UpdateSelectZone()
    {
        // 현재 남아있는 주사위 개수에 맞는 SelectZone 설정 및 점수 갱신
        SetSelectButtonUI(diceController.remainDiceCount);
        UpdateSelectButtonScore();
        // 현재 남아있는 주사위 개수에 맞는 SelectZonePos 설정
        SetSelectPosList(diceController.remainDiceCount);
    }

    // 돌릴 주사위 개수에 따라 SelectDiceUI 선택
    private void SetSelectButtonUI(int diceCount)
    {
        switch (diceCount)
        {
            case 0:
                dice1Container.SetActive(false);
                dice2Container.SetActive(false);
                dice3Container.SetActive(false);
                dice4Container.SetActive(false);
                dice5Container.SetActive(false);
                currentSelectZoneList = null;
                break;
            case 1:
                dice1Container.SetActive(true);
                dice2Container.SetActive(false);
                dice3Container.SetActive(false);
                dice4Container.SetActive(false);
                dice5Container.SetActive(false);
                currentSelectZoneList = dice1;
                break;
            case 2:
                dice2Container.SetActive(true);
                dice1Container.SetActive(false);
                dice3Container.SetActive(false);
                dice4Container.SetActive(false);
                dice5Container.SetActive(false);
                currentSelectZoneList = dice2;
                break;
            case 3:
                dice3Container.SetActive(true);
                dice1Container.SetActive(false);
                dice2Container.SetActive(false);
                dice4Container.SetActive(false);
                dice5Container.SetActive(false);
                currentSelectZoneList = dice3;
                break;
            case 4:
                dice4Container.SetActive(true);
                dice1Container.SetActive(false);
                dice2Container.SetActive(false);
                dice3Container.SetActive(false);
                dice5Container.SetActive(false);
                currentSelectZoneList = dice4;
                break;
            case 5:
                dice5Container.SetActive(true);
                dice1Container.SetActive(false);
                dice2Container.SetActive(false);
                dice3Container.SetActive(false);
                dice4Container.SetActive(false);
                currentSelectZoneList = dice5;
                break;
            default:
                Debug.Log("[ERROR]Dices가 비어있음!");
                break;

        }
    }

    // 선택한 DiceSelect UI 오브젝트 각각 점수 업데이트
    private void UpdateSelectButtonScore()
    {
        if (currentSelectZoneList == null) return;

        int index = 0;
        foreach(var dice in diceController.Dices)
        {
            if(!dice.isSelect)
            {
                currentSelectZoneList[index].GetComponent<SelectDice>().score = dice.score;
                index++;
            }
        }
    }

    // Select Pos List 선택
    private void SetSelectPosList(int diceCount)
    {
        switch (diceCount)
        {
            case 0:
                currentSelectZoneList = null;
                break;
            case 1:
                currentSelectPosList = selectZonePos_1Dice;
                break;
            case 2:
                currentSelectPosList = selectZonePos_2Dice;
                break;
            case 3:
                currentSelectPosList = selectZonePos_3Dice;
                break;
            case 4:
                currentSelectPosList = selectZonePos_4Dice;
                break;
            case 5:
                currentSelectPosList = selectZonePos_5Dice;
                break;
            default:
                Debug.Log("[ERROR]Dices가 비어있음!");
                break;

        }
    }

    // 주사위 눈 기준으로 정렬 후 DiceSelect Pos, Rot에 따라 오브젝트 이동
    public void SetAllDicesToBeSelectMode()
    {
        int index = 0;
        foreach(var dice in diceController.Dices)
        {
            if(!dice.isSelect)
            {
                StartCoroutine(TeleportDice(dice, index, 0.5f, true));
                index++;
            }
        }
    }

    #region SelectZone에서 Dice 선택 시


    public void SelectDiceUI(int index)
    {
        PV.RPC("SelectDiceUIRPC", RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    // 주사위가 선택되어 Return Zone으로 돌아가고 SelectZone을 정렬하는 함수
    // index : 선택된 주사위 인덱스
    public void SelectDiceUIRPC(int newindex)
    {
        UpdateSelectZone();
        if (currentSelectZoneList == null) { return; }
        SelectDice diceInfo = currentSelectZoneList[newindex].GetComponent<SelectDice>();
        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        Dice dice = TryFindDiceinSelectZone(dices, diceInfo.index);
        if (dice == null) return;

        int index = FindReturnZoneIndex(dice, diceInfo);

        if (index == -1) return;

        // 모든 조건이 괜찮을 시, 이동 시작
        StartCoroutine(TeleportDice(dice, index, 0.25f, false));

        // Return Zone에 모든 주사위가 모일 시 자동으로 점수 집계 및 진행

        // 남은 주사위 정렬
        SortSelectZoneDice();
    }

    // 주사위 매치 함수
    private Dice TryFindDiceinSelectZone(List<Dice> dices, int findIndex)
    {
        int currentIndex = 0;
        foreach(var dice in dices)
        {
            if(!dice.isSelect)
            {
                if (currentIndex == findIndex) return dice;
                currentIndex++;
            }
        }
        return null;
    }

    // Return Zone 매치 인덱스
    private int FindReturnZoneIndex(Dice dice, SelectDice diceInfo)
    {
        for(int i=0; i<returnZoneDice.Count; i++)
        {
            // 맞는 인덱스를 찾았을 시, 점수를 집어넣고 인덱스 반환
            if(returnZoneDice[i].GetComponent<SelectDice>().score == 0)
            {
                returnZoneDice[i].GetComponent<SelectDice>().score = diceInfo.score;
                diceController.remainDiceCount--; // 남아있는 주사위 개수 감소
                dice.returnZoneIndex = i;
                dice.isSelect = true; // isSelect 업데이트
                diceInfo.score = 0;
                return i;
            }
        }
        return -1;
    }

    // SelectZone Dice 정렬
    private void SortSelectZoneDice()
    {
        UpdateSelectZone();
        TeleportDiceinSelectZone();
    }

    // SelectZone에서는 바로 이동시킨다. 
    public void TeleportDiceinSelectZone()
    {
        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        int index = 0;
        foreach (var dice in dices)
        {
            if (!dice.isSelect)
            {
                dice.transform.localPosition = currentSelectPosList[index];
                dice.transform.rotation = Quaternion.Euler(dice.GetRot(dice.score) + selectZoneRot);
                index++;
            }
        }
    }

    #endregion

    #region ReturnZone에서 Dice 선택 시

    // 주사위가 선택되어 Select Zone으로 돌아가는 함수
    // Select Zone으로 이동한 후 Return Zone이 아닌 Select Zone만 정렬한다.
    // 따라서 Select Zone과 반대로 정렬 후 -> 이동
    // index : 선택된 주사위 인덱스

    public void ReturnDiceUI(int index)
    {
        PV.RPC("ReturnDiceUIRPC", RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    public void ReturnDiceUIRPC(int newindex)
    {
        UpdateSelectZone();
        if (returnZoneDice == null) { return; }
        SelectDice diceInfo = returnZoneDice[newindex].GetComponent<SelectDice>();

        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        Dice dice = TryFindDiceinReturnZone(dices, diceInfo.index, diceInfo);
        if (dice == null) return;

        // Select Zone UI 업데이트
        UpdateSelectZone();

        // 들어갈 인덱스를 찾는다.
        int index = FindSelectZoneIndex(dice, diceInfo);

        if (index == -1) return;

        dice.isMoving = false;

        // 이동 시작
        StartCoroutine(TeleportDice(dice, index, 0.25f, true));

        // 이동 후 정렬
        TeleportDiceinSelectZoneButReturnZoneIndex(index);
    }


    // Return Zone 주사위 매치 함수
    // Dice가 갖고 있는 returnZoneIndex를 이용하여 일치하는 주사위를 찾는다.
    private Dice TryFindDiceinReturnZone(List<Dice> dices, int findIndex, SelectDice diceinfo)
    {
        int currentIndex = 0;
        foreach (var dice in dices)
        {
            if (dice.isSelect)
            {
                if (dice.returnZoneIndex == findIndex)
                {
                    diceController.remainDiceCount++;

                    // Dice Select 정보 초기화
                    dice.isSelect = false;
                    dice.isMoving = true;
                    diceinfo.score = 0;

                    // dice returnZone  정보 초기화
                    dice.returnZoneIndex = -1;

                    return dice;
                }
                currentIndex++;
            }
        }
        return null;
    }

    // Select Zone 매치 인덱스
    // Select UI 업데이트 이후 찾아갈 인덱스를 찾는다.
    private int FindSelectZoneIndex(Dice movingDice, SelectDice diceInfo)
    {
        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        int index = 0;
        foreach (var dice in dices)
        {
            if (!dice.isSelect)
            {
                if (dice.isMoving) return index;
                index++;
            }
        }

        return -1;
    }

    // SelectZone에서는 바로 이동시킨다. : 단, Return Zone에서 올라갈 인덱스를 자리는 비워놓는다.
    // index : 비워두어야할 인덱스
    public void TeleportDiceinSelectZoneButReturnZoneIndex(int avoidIndex)
    {
        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        int index = 0;
        foreach (var dice in dices)
        {
            if (!dice.isSelect)
            {
                dice.transform.localPosition = currentSelectPosList[index];
                dice.transform.rotation = Quaternion.Euler(dice.GetRot(dice.score) + selectZoneRot);
                index++;
            }
        }
    }


    #endregion

    // dice : 이동할 주사위 오브젝트, isSelectZone : 이동시킬 Zone 유형, index : 이동 시킬 Zone index, duration : 이동 시간
    IEnumerator TeleportDice(Dice dice, int index, float duration, bool isSelectZone)
    {
        if (dice.isMoving)
        {
            yield break;
        }

        dice.isMoving = true;

        // postion 설정
        Vector3 currentPos = dice.transform.position;
        Vector3 movePos = (isSelectZone) ? currentSelectPosList[index] : returnZonePos[index];

        // rotation 설정 : 주사위 눈에 따라 Rot 값 이동
        Quaternion currentRot = dice.transform.rotation;
        Quaternion moveRot = Quaternion.Euler(dice.GetRot(dice.score) + (isSelectZone ? selectZoneRot : returnZoneRot));

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            dice.transform.position = Vector3.Lerp(currentPos, movePos, timer / duration);
            dice.transform.rotation = Quaternion.Lerp(currentRot, moveRot, timer / duration);
            yield return null;
        }


        dice.isMoving = false;
    }

    // 리롤 시, 기존에 남아있는 Return Zone의 주사위를 정렬한다.
    public void SortReturnZoneDice()
    {
        PV.RPC("SortReturnZoneDiceRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SortReturnZoneDiceRPC()
    {
        List<Dice> dices = new List<Dice>();
        dices = diceController.Dices;

        int index = 0;
        foreach (var dice in dices)
        {
            if(dice.isSelect)
            {
                returnZoneDice[index].GetComponent<SelectDice>().score = dice.score;
                dice.returnZoneIndex = index;
                dice.transform.localPosition = returnZonePos[index];
                index++;
            }
        }

        // 남은 ReturnZone SelectDice의 score는 0으로 초기화 해준다.
        for(int i=index; i<returnZoneDice.Count; i++)
        {
            returnZoneDice[i].GetComponent<SelectDice>().score = 0;
        }
    }

    // 플레이어 리롤턴 이 모두 끝날 시 : 남은 주사위들을 모두 ReturnZone에 보낸다.
    // Return Zone에 모든 주사위가 모일 시 점수를 꼭 적게 만든다.
    public void MoveAllDicesReturnZone()
    {
        Debug.Log("MoveAllDicesReturnZone 호출!");
        while (true)
       {
            if (currentSelectZoneList == null) break;
            SelectDice dice = currentSelectZoneList[0].GetComponent<SelectDice>();
            if (dice == null) break;
            SelectDiceUI(dice.index);
       }
    }

    public void ResetReturnZoneSelectUIScore()
    {
        PV.RPC("ResetReturnZoneSelectUIScoreRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // ReturnZone에 있는 모든 Select UI의 score를 0으로 초기화한다.
    private void ResetReturnZoneSelectUIScoreRPC()
    {
        for(int i=0; i<returnZoneDice.Count; i++)
        {
            returnZoneDice[i].GetComponent<SelectDice>().score = 0;
        }
    }

    #region Select UI 관련 함수

    public void SetSelectZoneSelectUI(bool isActive)
    {
        PV.RPC("SetSelectZoneSelectUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI 활성화/비활성화
    private void SetSelectZoneSelectUIRPC(bool isActive)
    {
        selectZoneSelectUI.SetActive(isActive);
    }

    public void SetReturnZoneSelectUI(bool isActive)
    {
        PV.RPC("SetReturnZoneSelectUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI 활성화/비활성화
    private void SetReturnZoneSelectUIRPC(bool isActive)
    {
        returnZoneSelectUI.SetActive(isActive);
    }

    #endregion

    #region SelectDice PunRPC 함수

    public void ToMovingOnSelectDiceUI(bool isSelectZone, float movePosX)
    {
        PV.RPC("ToMovingOnSelectDiceUIRPC", RpcTarget.AllBuffered, isSelectZone, movePosX);
    }

    [PunRPC]
    private void ToMovingOnSelectDiceUIRPC(bool isSelectZone, float movePosX)
    {
        if (isSelectZone) selectZoneSelectUI.transform.localPosition = new Vector3(movePosX, 0f, 0f);
        else returnZoneSelectUI.transform.localPosition = new Vector3(movePosX, 272f, 0f);
    }


    #endregion


}
