using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static InGameNetWorkManager;

public class DiceController : MonoBehaviourPunCallbacks
{
    /* Yacht Dice Controller ��ũ��Ʈ
     * 
     *  �� ��ũ��Ʈ�� ��� �� ���
     *  - <Title Panel> �������� �ֻ��� ����Ʈ
     *  - <Game Play Panel> �ֻ��� �����̳� ���� �������� ����Ʈ
     * 
    */

    [Header("Dice")]
    // ��ȯ�� ������ : ���̸� ���� / ��ġ�� ����
    [SerializeField]
    private Vector3 spawnPos;

    // ��ȯ�� �ֻ����� �Ÿ�
    [SerializeField]
    private float spawnDistance;

    // Yacht Dice ������
    [SerializeField]
    private GameObject dicePrefab;

    // �ֻ��� ����Ʈ ���� ����
    public bool isThrown; 

    // private Vector3 spawnPos = new Vector3(2f, 20f, 0f);
    private float spawnTimer = 1f;

    // Dice ������ ����Ʈ
    public List<Dice> Dices = new List<Dice>();

    // ���� Dice ����
    public int remainDiceCount;

    [Header("Dice Bottle")]
    [SerializeField]
    private GameObject diceBottle;

    // ��鸲 ����
    [SerializeField]
    private float shakeAmt;

    [Header("PV")]
    public PhotonView PV;

    [Header("Scripts")]

    public static DiceController DC;
    [SerializeField]
    private DiceSelectManager diceSelectManager;
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;

    private void Awake()
    {
        DC = this;
    }

    private void Start()
    {
        isThrown = true;
        remainDiceCount = 5;

        PV = photonView;
    }

    private void Update()
    {
        if(isThrown)
        {
            // 15�� �������� ���̽� ��ȯ
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnYachtDices(12f);
                spawnTimer = 15f;
            }
        }
    }

    #region Dice Manager

    public void GetDices()
    {
        PV.RPC("GetDicesRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void GetDicesRPC()
    {
        this.Dices = IN.newDices;
    }

    // �ֻ��� �������� ����Ʈ ���� / �ߴ�
    public void FallingDice(bool _isThrown)
    {
        isThrown = _isThrown;
    }

    // SpawnPos ����
    public void SetSpawnPos(Vector3 _spawnPos)
    {
        spawnPos = _spawnPos;
    }

    // ���̽� ��ȯ
    public void SpawnYachtDices(float time)
    {
        for(int i=0; i<5; i++)
        {
            var dice = Instantiate(dicePrefab, spawnPos, Quaternion.identity).GetComponent<Dice>();
            
            // ������ ��ġ�Ͽ� ��ȯ
            float radian = (3f * Mathf.PI) / 5;
            radian *= i;
            dice.Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));

            // �ֻ��� ������
            dice.RollDice();

            // ���� �ð� ���� ������Ʈ ����
            dice.Destory(time);
        }
    }

    // ������Ʈ ����
    public void DestoryDice(GameObject _dice)
    {
        Destroy(_dice, 5f);
    }

    // �ΰ��� ���̽� ��ȯ
    public void SpawnYachtDicesInGame(float _spawnDistance)
    {
        for (int i = 0; i < 5; i++)
        {
            var dice = PhotonNetwork.Instantiate("DicePrefab", spawnPos, Quaternion.identity).GetComponent<Dice>();

            // ������ ��ġ�Ͽ� ��ȯ
            float radian = (3f * Mathf.PI) / 5;
            radian *= i;
            dice.Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _spawnDistance));

            // �ֻ��� ������
            dice.RollDice();
        }
    }

    public void RerollYachtDices()
    {
        PV.RPC("RerollYachtDicesRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // �ΰ��� ���̽� ����
    private void RerollYachtDicesRPC()
    {
        // ���� �ֻ��� ���� ������Ʈ
        UpdateRemainDiceCount();

        for (int i=0; i<Dices.Count; i++)
        {
            // ���õ��� ���� �ֻ�����θ� ����
            if(!Dices[i].isSelect)
            {
                // ������ ��ġ�Ͽ� ��ȯ
                float radian = (3f * Mathf.PI) / remainDiceCount;
                radian *= i;
                float _spawnDistance = 1f;
                Dices[i].Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _spawnDistance));
            }
        }
    }

    // �ֻ��� �� ������� ����
    public void SortDices()
    {
        Dices.Sort((p1, p2) => p1.score.CompareTo(p2.score));
    }

    // �ֻ��� �ݶ��̴� �ѱ�(�ֻ��� �� Ȯ�� �Լ�)
    IEnumerator AcitveDiceColiderCourutine()
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < Dices.Count; i++)
        {
            if (!Dices[i].isSelect) Dices[i].SetSidesColider(true);
        }

        // Reroll�� ������
        IN.SetRollButtonReroll();

        yield return new WaitForSeconds(0.5f);
        // '��' �� �ֻ����� / �ֻ��� ���� ������ �ȵ� �ֻ��� ���� �� �ٽ� ������
        if (CheckDicePos())
        {
            IN.StartFailThrowDiceAnim();
        }
        else
        {
            // �ֻ��� ����:
            // 1) �ֻ��� �� ������� ����
            // 2) ReturnZone Ȱ��ȭ
            // 3) ���� �ֻ����� ���� DiceSelect UI ����
            // 4) ������ DiceSelect UI ������Ʈ ���� ���� ������Ʈ
            // 5) �ֻ��� ������Ʈ Select Pos List ����
            // 6) �ֻ��� �浹 ���� (RPC)
            // 7) �ֻ��� �� �������� ���� �� DiceSelect Pos, Rot�� ���� ������Ʈ �̵�

            // 1) �÷��̾� ���ھ� ���� ��������
            // 2) ���� �ֻ��� ���� ���� ���ھ� ���� ������Ʈ

            //diceSelectManager.PV.RPC("SetPlayerSelectDice", RpcTarget.AllBuffered, remainDiceCount);

            SortDices();
            UpdateRemainDiceCount();

            SetAllDiceKinematic(true);

            diceSelectManager.SetReturnZoneAcitve(true);
            diceSelectManager.UpdateSelectZone();
            diceSelectManager.SetAllDicesToBeSelectMode();

            scoreBoardManager.SetActiveCurrentPlayerScoreBoard(true);
            scoreBoardManager.UpdateCurrentPlayerScoreBoard();

            if (remainDiceCount == 5) diceSelectManager.ResetReturnZoneSelectUIScore();

            IN.SetRerollCountUI(true);

            // �÷��̾� ���� ī��Ʈ�� ������ ��, ���̻� ������ ���� ���ϸ� ������ �ٷ� �����ϰ� �Ѵ�.
            // �÷��� �ֻ������� ��� ReturnZone���� ���� 
            if (IN.Players[IN.currentPlayerSequence].rerollCount == 0)
            {
                yield return new WaitForSeconds(1f);
                diceSelectManager.MoveAllDicesReturnZone();
            }

        }
    }

    // �ֻ��� �浹 ����
    public void SetAllDiceKinematic(bool isActive)
    {
        foreach (var dice in Dices)
        {
            if (!dice.isSelect)
            {
                dice.SetDiceKinematic(isActive);
            }
        }
    }

    // '��'�� ������ �ִ� �ֻ��� Ȯ��
    public bool CheckDicePos()
    {
        for(int i=0; i<Dices.Count; i++)
        {
            // ���õ� �ֻ����� ����
            if(!Dices[i].isSelect)
            {
                if (Dices[i].transform.localPosition.y < 1.6f || Dices[i].transform.localPosition.y > 2f)
                {
                    PV.RPC("SendError", RpcTarget.AllBuffered, ("��: �Ÿ��� �ָ� ���������� �ֻ��� y��ǥ : " + (Dices[i].transform.localPosition.y).ToString()));
                    return true;
                }
                else if (Dices[i].score == 0)
                {
                    PV.RPC("SendError", RpcTarget.AllBuffered, ("��: �ֻ��� ������ ������� ����"));
                    return true;
                }
            }
        }
        return false;
    }

    [PunRPC]
    private void SendError(string errorMsg)
    {
        Debug.Log(errorMsg);
    }


    public void ResetRemainDiceCount()
    {
        remainDiceCount = 5;
    }

    // ���� �ֻ��� ���� ������Ʈ
    public void UpdateRemainDiceCount()
    {
        if(Dices.Count != 0)
        {
            int count = 0;
            for (int i = 0; i < Dices.Count; i++)
            {
                if (!Dices[i].isSelect)
                {
                    count++;
                }
            }
            remainDiceCount = count;
        }
    }

    // Dice isSelect �ʱ�ȭ (�� �÷��̾ ó�� ������ ����)
    public void ResetDiceSelect()
    {
        if (Dices.Count != 0)
        {
            for (int i = 0; i < Dices.Count; i++)
            {
                Dices[i].isSelect = false;
            }
        }
    }

    #endregion

    #region Dice Bottle Manager

    public void SetBottleInitPosByTransform()
    {
        // transform���� ��ġ �ʱ�ȭ
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);
    }

    public void SetBottleInitPos()
    {
        PV.RPC("SetBottleInitPosRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SetBottleInitPosRPC()
    {
        // Init Bottle Anim���� �ʱ�ȭ
        diceBottle.GetComponent<Animator>().SetTrigger("init");
    }

    public void SetBottlePlayingPos()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetTrigger("down");
        PV.RPC("SetBottlePlayingPosRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetBottlePlayingPosRPC()
    {
        // set Bottle Anim���� �ʱ�ȭ
        diceBottle.GetComponent<Animator>().SetTrigger("set");
    }


    // ���̽� Bottle ����
    public void ShakingBottle()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetBool("isDown", true);
        PV.RPC("ShakingBottleRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // ���̽� Bottle ����
    private void ShakingBottleRPC()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", true);
    }

    // ���̽� Bottle ������
    public void ThrowBottle()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetBool("isDown", false);
        PV.RPC("ThrowBottleRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // ���̽� Bottle ������
    private void ThrowBottleRPC()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", false);
        StartCoroutine(AcitveDiceColiderCourutine());
    }

    // ���̽� Bottle �ٽ� ����(Reroll)
    public void ReBottlePlayingPos()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetTrigger("down");
        PV.RPC("ReBottlePlayingPosRPC", RpcTarget.AllBuffered);
    }


    [PunRPC]
    // ���̽� Bottle �ٽ� ����(Reroll)
    private void ReBottlePlayingPosRPC()
    {
        diceBottle.GetComponent<Animator>().SetTrigger("reroll");
    }

    #endregion

}
