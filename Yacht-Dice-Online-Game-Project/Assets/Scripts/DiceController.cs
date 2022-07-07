using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    private bool isThrown; 

    // private Vector3 spawnPos = new Vector3(2f, 20f, 0f);
    private float spawnTimer = 1f;

    [Header("Dice Bottle")]
    [SerializeField]
    private GameObject diceBottle;

    // ��鸲 ����
    [SerializeField]
    private float shakeAmt;

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

    // ���̽� ��ġ ������Ʈ
    public void AllocateDiceAround(Dice[] dices)
    {
        for (int i = 0; i < dices.Length; i++)
        {
            float radian = (2f * Mathf.PI) / dices.Length;
            radian *= i;
            dices[i].Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));
        }
    }

    // ������Ʈ ����
    public void DestoryDice(GameObject _dice)
    {
        Destroy(_dice, 5f);
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
    }

    #endregion

}
