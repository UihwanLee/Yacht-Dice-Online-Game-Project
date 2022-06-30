using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private int diceValue;

    public static Vector3 diceVelcoity;

    private bool thrown;


    private void Start()
    {
        thrown = false;
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
}
