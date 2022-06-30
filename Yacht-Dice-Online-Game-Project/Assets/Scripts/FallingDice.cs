using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDice : MonoBehaviour
{
    /* Yacht Dice Falling Effect ��ũ��Ʈ
     * 
     *  �� ��ũ��Ʈ�� ��� �� ���
     *  - <Title Panel> �������� �ֻ��� ����Ʈ
     *  - <Game Play Panel> �ֻ��� �����̳� ���� �������� ����Ʈ
     * 
    */


    // ��ȯ�� ������ : ���̸� ���� / ��ġ�� ����
    [SerializeField]
    private Vector3 spawnPos;

    // ��ȯ�� �ֻ����� �Ÿ�
    [SerializeField]
    private float spawnDistance;

    // Yacht Dice ������
    [SerializeField]
    private GameObject dicePrefab;

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
}
