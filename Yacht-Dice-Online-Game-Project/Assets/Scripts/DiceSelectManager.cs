using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static DiceController;

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

    }

    
    // 돌릴 주사위 개수에 따라 SelectDiceUI 선택
    public void SetSelectButtonUI(int diceCount)
    {
        switch(diceCount)
        {
            case 1:
                dice1Container.SetActive(true);
                currentSelectZoneList = dice1;
                break;
            case 2:
                dice2Container.SetActive(true);
                currentSelectZoneList = dice2;
                break;
            case 3:
                dice3Container.SetActive(true);
                currentSelectZoneList = dice3;
                break;
            case 4:
                dice4Container.SetActive(true);
                currentSelectZoneList = dice4;
                break;
            case 5:
                dice5Container.SetActive(true);
                currentSelectZoneList = dice5;
                break;
            default:
                Debug.Log("[ERROR]Dices가 비어있음!");
                break;

        }
    }

    // 선택한 DiceSelect UI 오브젝트 각각 점수 업데이트
    public void UpdateSelectButtonScore()
    {
        int index = 0;
        foreach(var dice in DC.Dices)
        {
            if(!dice.isSelect)
            {
                currentSelectZoneList[index].GetComponent<DiceSelect>().score = dice.score;
                index++;
            }
        }
    }

    // Select Pos List 선택
    public void SetSelectPosList(int diceCount)
    {
        switch (diceCount)
        {
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

    [PunRPC]
    // 주사위 눈 기준으로 정렬 후 DiceSelect Pos, Rot에 따라 오브젝트 이동
    public void SetAllDicesToBeSelectMode()
    {
        int index = 0;
        foreach(var dice in DC.Dices)
        {
            if(!dice.isSelect)
            {
                Debug.Log(dice.score + "번호 이동중!");
                StartCoroutine(TeleportDice(dice, index, 0.5f));
                index++;
            }
        }
    }

    IEnumerator TeleportDice(Dice dice, int index,float duration)
    {
        if (dice.isMoving)
        {
            yield break;
        }

        dice.isMoving = true;
        //dice.GetComponent<MeshCollider>().convex = false;

        // postion 설정
        Vector3 currentPos = dice.transform.position;
        Vector3 movePos = currentSelectPosList[index];

        // rotation 설정 : 주사위 눈에 따라 Rot 값 이동
        Quaternion currentRot = dice.transform.rotation;
        Quaternion moveRot = Quaternion.Euler(dice.GetRot(dice.score) + selectZoneRot);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            dice.transform.position = Vector3.Lerp(currentPos, movePos, timer / duration);
            dice.transform.rotation = Quaternion.Lerp(currentRot, moveRot, timer / duration);
            yield return null;
        }

        //dice.GetComponent<MeshCollider>().convex = true;

        dice.isMoving = false;
    }
}
