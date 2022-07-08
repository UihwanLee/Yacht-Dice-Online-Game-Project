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

    [Header("Dice Bottle")]
    [SerializeField]
    private GameObject diceBottle;

    // ��鸲 ����
    [SerializeField]
    private float shakeAmt;

    public static DiceController DC;

    private void Awake()
    {
        DC = this;
    }

    private void Start()
    {
        isThrown = true;
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
        // ���� �ֻ��� ī���� ���
        int count = 0;
        for (int i = 0; i < Dices.Count; i++) { if (!Dices[i].isSelect) count++; }


        for (int i=0; i<Dices.Count; i++)
        {
            // ���õ��� ���� �ֻ�����θ� ����
            if(!Dices[i].isSelect)
            {
                // ������ ��ġ�Ͽ� ��ȯ
                float radian = (3f * Mathf.PI) / count;
                radian *= i;
                float _spawnDistance = 1f;
                Dices[i].Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _spawnDistance));

                // �ֻ��� ������
                Dices[i].RollDice();
            }
        }
    }

    // ������Ʈ ����
    public void DestoryDice(GameObject _dice)
    {
        Destroy(_dice, 5f);
    }

    // �ֻ��� �� ������� ����
    public void SortDices()
    {
        Dices.Sort((p1, p2) => p1.score.CompareTo(p2.score));
    }

    // �ֻ��� �ݶ��̴� �ѱ�(�ֻ��� �� Ȯ�� �Լ�)
    IEnumerator AcitveDiceColiderCourutine()
    {
        yield return new WaitForSeconds(15f);
        for (int i = 0; i < Dices.Count; i++)
        {
            if (!Dices[i].isSelect) Dices[i].SetSidesColider(true);
        }

        // Reroll�� ������
        IN.SetRollButtonReroll();

        yield return new WaitForSeconds(1f);
        // '��' �� �ֻ����� / �ֻ��� ���� ������ �ȵ� �ֻ��� ���� �� �ٽ� ������
        if (CheckDicePos())
        {
            Debug.Log("��");
            IN.SetDice();
        }
        else
        {
            // �ֻ��� ����: DiceSelectManager���� �ֻ��� ���� �Ѱ��ֱ�
        }
    }

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
