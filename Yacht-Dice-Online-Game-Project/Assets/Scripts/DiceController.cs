using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static InGameNetWorkManager;
using static DiceSelectManager;

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

    [Header("Scripts")]

    public static DiceController DC;
    [SerializeField]
    private DiceSelectManager diceSelectManager;

    private void Awake()
    {
        DC = this;
    }

    private void Start()
    {
        isThrown = true;
        remainDiceCount = 0;
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

    // �ΰ��� ���̽� ����
    public void RerollYachtDices()
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

                // �ֻ��� ������
                Dices[i].RollDice();
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
        yield return new WaitForSeconds(8f);
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
            Debug.Log("��");
            IN.SetDice();
        }
        else
        {
            // �ֻ��� ����:
            // 1) �ֻ��� �� ������� ����
            // 2) ���� �ֻ����� ���� DiceSelect UI ����
            // 3) ������ DiceSelect UI ������Ʈ ���� ���� ������Ʈ
            // 4) �ֻ��� ������Ʈ Select Pos List ����
            // 5) �ֻ��� �浹 ���� (RPC)
            // 6) �ֻ��� �� �������� ���� �� DiceSelect Pos, Rot�� ���� ������Ʈ �̵�

            //diceSelectManager.PV.RPC("SetPlayerSelectDice", RpcTarget.AllBuffered, remainDiceCount);

            SortDices();
            UpdateRemainDiceCount();
            diceSelectManager.SetSelectButtonUI(remainDiceCount);
            diceSelectManager.UpdateSelectButtonScore();
            diceSelectManager.SetSelectPosList(remainDiceCount);
            diceSelectManager.PV.RPC("SetDiceKinematicRPC", RpcTarget.AllBuffered, true);
            diceSelectManager.SetAllDicesToBeSelectMode();

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
                if (Dices[i].transform.localPosition.y < 1.6f || Dices[i].transform.localPosition.y > 1.75f || Dices[i].score == 0) return true;
            }
        }
        return false;
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

    public void SetBottleInitPos()
    {
        // Init Bottle Anim���� �ʱ�ȭ
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);
    }

    public void SetBottlePlayingPos()
    {
        // set Bottle Anim���� �ʱ�ȭ
        diceBottle.GetComponent<Animator>().SetTrigger("set");
    }


    // ���̽� Bottle ����
    public void ShakingBottle()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", true);
    }

    // ���̽� Bottle ������
    public void ThrowBottle()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", false);
        StartCoroutine(AcitveDiceColiderCourutine());
    }

    // ���̽� Bottle �ٽ� ����(Reroll)
    public void ReBottlePlayingPos()
    {
        diceBottle.GetComponent<Animator>().SetTrigger("reroll");
    }

    #endregion

}
