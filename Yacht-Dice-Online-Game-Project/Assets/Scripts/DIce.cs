using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static DiceController;

public class Dice : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Rigidbody rb;

    public static Vector3 diceVelcoity;

    private bool thrown;

    // ���̽� ����
    public int score;

    // select �Ǿ����� Ȯ�� �ϴ� ����
    public bool isSelect;

    // �ֻ����� ���� �����̰� �ִ� üũ�ϴ� ����
    public bool isMoving;

    // 6���� ���̵� �ݶ��̴� 
    [SerializeField]
    private List<GameObject> sides = new List<GameObject>();

    // Return Zone�� ���� �� ������ �ε���
    public int returnZoneIndex;

    [Header("PV")]
    public PhotonView PV;

    private void Start()
    {
        PV = photonView;

        // 6���� �ݶ��̴� �ʱ�ȭ
        SetSidesColider(false);

        score = 0;
        isSelect = false;
        isMoving = false;

        thrown = false;

        returnZoneIndex = -1;

        if (!DC.isThrown)
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
        if(DC.Dices.Count != 0)
        {
            if (DC.Dices.Contains(this))
            {
                DC.Dices.Remove(this);
            }
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
        PV.RPC("SetScoreRPC", RpcTarget.All, _score);
    }

    [PunRPC]
    // socreRPC ����
    private void SetScoreRPC(int _score)
    {
        score = _score;
    }

    // �ֻ��� �浹 ���� 
    public void SetDiceKinematic(bool isActive)
    {
        PV.RPC("SetDiceKinematicRPC", RpcTarget.All, isActive);
    }

    [PunRPC]
    // �ֻ��� �浹 ���� RPC
    public void SetDiceKinematicRPC(bool isActive)
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = isActive;
    }

    // �ֻ��� ���� ���� Rot�� ��ȯ
    public Vector3 GetRot(int score)
    {
        Vector3 rot;

        switch(score)
        {
            case 1:
                rot = new Vector3(0f, 0f, 0f);
                break;
            case 2:
                rot = new Vector3(0f, 0f, 90f);
                break;
            case 3:
                rot = new Vector3(90f, 0f, 0f);
                break;
            case 4:
                rot = new Vector3(270f, 0f, 0f);
                break;
            case 5:
                rot = new Vector3(0f, 0f, 270f);
                break;
            case 6:
                rot = new Vector3(180f, 0f, 0f);
                break;
            default:
                rot = new Vector3(0f, 0f, 0f);
                Debug.Log("[ERROR] �´� score�� �����ϴ�!");
                break;
        }

        return rot;
    }
}
