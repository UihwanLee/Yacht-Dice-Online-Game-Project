using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DiceController;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    public static Vector3 diceVelcoity;

    private bool thrown;

    // ���̽� ����
    public int score;

    // select �Ǿ����� Ȯ�� �ϴ� ����
    public bool isSelect;

    // 6���� ���̵� �ݶ��̴� 
    [SerializeField]
    private List<GameObject> sides = new List<GameObject>();



    private void Start()
    {
        // 6���� �ݶ��̴� �ʱ�ȭ
        SetSidesColider(false);

        score = 0;
        isSelect = false;

        thrown = false;

        if(!DC.isThrown)
        {
            // Dices List �߰�
            DC.Dices.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (thrown)
        {
            diceVelcoity = rb.velocity;
        }
        //RollDice();
    }

    public void OnDestroy()
    {
        if (DC.Dices.Contains(this))
        {
            DC.Dices.Remove(this);
            
        }
    }

    // ���̽� ������
    public void RollDice()
    {
        if (!thrown)
        {
            thrown = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        }
    }

    // ��ġ �̵�
    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    // ������Ʈ ����
    public void Destory(float time)
    {
        Destroy(this.gameObject, time);
    }

    // 6���� ���̵� �ݶ��̴� Ȱ��ȭ/��Ȱ��ȭ
    public void SetSidesColider(bool isAcitve)
    {
        for (int i = 0; i < sides.Count; i++) sides[i].SetActive(isAcitve);
    }

    // socre ����
    public void SetScore(int _score)
    {
        score = _score;
    }
}
