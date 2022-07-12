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
    // ReturnZone, SelectZone Pos, Rot ���� ����
    private List<Vector3> returnZonePos = new List<Vector3>();
    private List<Vector3> selectZonePos_5Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_4Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_3Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_2Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_1Dice = new List<Vector3>();

    private Vector3 returnZoneRot;
    private Vector3 selectZoneRot;

    // ReturnZone, SelectZone UI ����
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


    // ���õ� SelectButtonUIList
    private List<GameObject> currentSelectZoneList = new List<GameObject>();

    // ���õ� SelectPosList
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

        // returnZonPos �ʱ�ȭ
        returnZonePos.Add(new Vector3(-182.41f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-181.45f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-180.49f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-179.53f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-178.57f, 3.5f, 3f));

        // selectZonePos_5Dice �ʱ�ȭ
        selectZonePos_5Dice.Add(new Vector3(-182.1f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-181.3f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-179.7f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-178.9f, 9.71f, 0.05f));

        // selectZonePos_4Dice �ʱ�ȭ
        selectZonePos_4Dice.Add(new Vector3(-181.7f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.9f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.1f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-179.3f, 9.71f, 0.05f));

        // selectZonePos_3Dice �ʱ�ȭ
        selectZonePos_3Dice.Add(new Vector3(-181.3f, 9.71f, 0.05f));
        selectZonePos_3Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));
        selectZonePos_3Dice.Add(new Vector3(-179.7f, 9.71f, 0.05f));

        // selectZonePos_2Dice �ʱ�ȭ
        selectZonePos_2Dice.Add(new Vector3(-180.9f, 9.71f, 0.05f));
        selectZonePos_2Dice.Add(new Vector3(-180.1f, 9.71f, 0.05f));

        // selectZonePos_1Dice �ʱ�ȭ
        selectZonePos_1Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));


        // returnZonRot �ʱ�ȭ
        returnZoneRot = new Vector3(-14f, 0f, 0f);

        // selectZoneRot �ʱ�ȭ
        selectZoneRot = new Vector3(-7.68f, 0f, 0f);

        diceSelcetUI.SetActive(true);
        // SelectZone, ReturnZone UI �ʱ�ȭ
        for (int i = 0; i < returnZoneDice.Count; i++) returnZoneDice[i].SetActive(false);
        dice5Container.SetActive(false);
        dice4Container.SetActive(false);
        dice3Container.SetActive(false);
        dice2Container.SetActive(false);
        dice1Container.SetActive(false);

        // Select UI ��Ȱ��ȭ
        selectZoneSelectUI.SetActive(false);
        returnZoneSelectUI.SetActive(false);

    }

    // ReturnZone UI Ȱ��ȭ/��Ȱ��ȭ
    public void SetReturnZoneAcitve(bool isActive)
    {
        for (int i = 0; i < returnZoneDice.Count; i++) returnZoneDice[i].SetActive(isActive);
    }

    // SelectZoneUI ������Ʈ
    public void UpdateSelectZone()
    {
        // ���� �����ִ� �ֻ��� ������ �´� SelectZone ���� �� ���� ����
        SetSelectButtonUI(diceController.remainDiceCount);
        UpdateSelectButtonScore();
        // ���� �����ִ� �ֻ��� ������ �´� SelectZonePos ����
        SetSelectPosList(diceController.remainDiceCount);
    }

    // ���� �ֻ��� ������ ���� SelectDiceUI ����
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
                Debug.Log("[ERROR]Dices�� �������!");
                break;

        }
    }

    // ������ DiceSelect UI ������Ʈ ���� ���� ������Ʈ
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

    // Select Pos List ����
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
                Debug.Log("[ERROR]Dices�� �������!");
                break;

        }
    }

    // �ֻ��� �� �������� ���� �� DiceSelect Pos, Rot�� ���� ������Ʈ �̵�
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

    #region SelectZone���� Dice ���� ��


    public void SelectDiceUI(int index)
    {
        PV.RPC("SelectDiceUIRPC", RpcTarget.AllBuffered, index);
    }

    [PunRPC]
    // �ֻ����� ���õǾ� Return Zone���� ���ư��� SelectZone�� �����ϴ� �Լ�
    // index : ���õ� �ֻ��� �ε���
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

        // ��� ������ ������ ��, �̵� ����
        StartCoroutine(TeleportDice(dice, index, 0.25f, false));

        // Return Zone�� ��� �ֻ����� ���� �� �ڵ����� ���� ���� �� ����

        // ���� �ֻ��� ����
        SortSelectZoneDice();
    }

    // �ֻ��� ��ġ �Լ�
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

    // Return Zone ��ġ �ε���
    private int FindReturnZoneIndex(Dice dice, SelectDice diceInfo)
    {
        for(int i=0; i<returnZoneDice.Count; i++)
        {
            // �´� �ε����� ã���� ��, ������ ����ְ� �ε��� ��ȯ
            if(returnZoneDice[i].GetComponent<SelectDice>().score == 0)
            {
                returnZoneDice[i].GetComponent<SelectDice>().score = diceInfo.score;
                diceController.remainDiceCount--; // �����ִ� �ֻ��� ���� ����
                dice.returnZoneIndex = i;
                dice.isSelect = true; // isSelect ������Ʈ
                diceInfo.score = 0;
                return i;
            }
        }
        return -1;
    }

    // SelectZone Dice ����
    private void SortSelectZoneDice()
    {
        UpdateSelectZone();
        TeleportDiceinSelectZone();
    }

    // SelectZone������ �ٷ� �̵���Ų��. 
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

    #region ReturnZone���� Dice ���� ��

    // �ֻ����� ���õǾ� Select Zone���� ���ư��� �Լ�
    // Select Zone���� �̵��� �� Return Zone�� �ƴ� Select Zone�� �����Ѵ�.
    // ���� Select Zone�� �ݴ�� ���� �� -> �̵�
    // index : ���õ� �ֻ��� �ε���

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

        // Select Zone UI ������Ʈ
        UpdateSelectZone();

        // �� �ε����� ã�´�.
        int index = FindSelectZoneIndex(dice, diceInfo);

        if (index == -1) return;

        dice.isMoving = false;

        // �̵� ����
        StartCoroutine(TeleportDice(dice, index, 0.25f, true));

        // �̵� �� ����
        TeleportDiceinSelectZoneButReturnZoneIndex(index);
    }


    // Return Zone �ֻ��� ��ġ �Լ�
    // Dice�� ���� �ִ� returnZoneIndex�� �̿��Ͽ� ��ġ�ϴ� �ֻ����� ã�´�.
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

                    // Dice Select ���� �ʱ�ȭ
                    dice.isSelect = false;
                    dice.isMoving = true;
                    diceinfo.score = 0;

                    // dice returnZone  ���� �ʱ�ȭ
                    dice.returnZoneIndex = -1;

                    return dice;
                }
                currentIndex++;
            }
        }
        return null;
    }

    // Select Zone ��ġ �ε���
    // Select UI ������Ʈ ���� ã�ư� �ε����� ã�´�.
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

    // SelectZone������ �ٷ� �̵���Ų��. : ��, Return Zone���� �ö� �ε����� �ڸ��� ������´�.
    // index : ����ξ���� �ε���
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

    // dice : �̵��� �ֻ��� ������Ʈ, isSelectZone : �̵���ų Zone ����, index : �̵� ��ų Zone index, duration : �̵� �ð�
    IEnumerator TeleportDice(Dice dice, int index, float duration, bool isSelectZone)
    {
        if (dice.isMoving)
        {
            yield break;
        }

        dice.isMoving = true;

        // postion ����
        Vector3 currentPos = dice.transform.position;
        Vector3 movePos = (isSelectZone) ? currentSelectPosList[index] : returnZonePos[index];

        // rotation ���� : �ֻ��� ���� ���� Rot �� �̵�
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

    // ���� ��, ������ �����ִ� Return Zone�� �ֻ����� �����Ѵ�.
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

        // ���� ReturnZone SelectDice�� score�� 0���� �ʱ�ȭ ���ش�.
        for(int i=index; i<returnZoneDice.Count; i++)
        {
            returnZoneDice[i].GetComponent<SelectDice>().score = 0;
        }
    }

    // �÷��̾� ������ �� ��� ���� �� : ���� �ֻ������� ��� ReturnZone�� ������.
    // Return Zone�� ��� �ֻ����� ���� �� ������ �� ���� �����.
    public void MoveAllDicesReturnZone()
    {
        Debug.Log("MoveAllDicesReturnZone ȣ��!");
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
    // ReturnZone�� �ִ� ��� Select UI�� score�� 0���� �ʱ�ȭ�Ѵ�.
    private void ResetReturnZoneSelectUIScoreRPC()
    {
        for(int i=0; i<returnZoneDice.Count; i++)
        {
            returnZoneDice[i].GetComponent<SelectDice>().score = 0;
        }
    }

    #region Select UI ���� �Լ�

    public void SetSelectZoneSelectUI(bool isActive)
    {
        PV.RPC("SetSelectZoneSelectUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI Ȱ��ȭ/��Ȱ��ȭ
    private void SetSelectZoneSelectUIRPC(bool isActive)
    {
        selectZoneSelectUI.SetActive(isActive);
    }

    public void SetReturnZoneSelectUI(bool isActive)
    {
        PV.RPC("SetReturnZoneSelectUIRPC", RpcTarget.AllBuffered, isActive);
    }

    [PunRPC]
    // Select UI Ȱ��ȭ/��Ȱ��ȭ
    private void SetReturnZoneSelectUIRPC(bool isActive)
    {
        returnZoneSelectUI.SetActive(isActive);
    }

    #endregion

    #region SelectDice PunRPC �Լ�

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
