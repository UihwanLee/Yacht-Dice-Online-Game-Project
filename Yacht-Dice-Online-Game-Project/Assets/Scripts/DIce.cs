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

    // 다이스 점수
    public int score;

    // select 되었는지 확인 하는 변수
    public bool isSelect;

    // 주사위가 현재 움직이고 있는 체크하는 변수
    public bool isMoving;

    // 6개의 사이드 콜라이더 
    [SerializeField]
    private List<GameObject> sides = new List<GameObject>();

    // Return Zone에 있을 시 내포된 인덱스
    public int returnZoneIndex;

    [Header("PV")]
    public PhotonView PV;

    private void Start()
    {
        PV = photonView;

        // 6개의 콜라이더 초기화
        SetSidesColider(false);

        score = 0;
        isSelect = false;
        isMoving = false;

        thrown = false;

        returnZoneIndex = -1;

        if (!DC.isThrown)
        {
            // Dices List 추가
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

    // 다이스 굴리기
    public void RollDice()
    {
        if (!thrown)
        {
            thrown = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        }
    }

    // 위치 이동
    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    // 오브젝트 삭제
    public void Destory(float time)
    {
        Destroy(this.gameObject, time);
    }

    // 6개의 사이드 콜라이더 활성화/비활성화
    public void SetSidesColider(bool isAcitve)
    {
        for (int i = 0; i < sides.Count; i++) sides[i].SetActive(isAcitve);
    }

    // socre 세팅
    public void SetScore(int _score)
    {
        PV.RPC("SetScoreRPC", RpcTarget.All, _score);
    }

    [PunRPC]
    // socreRPC 세팅
    private void SetScoreRPC(int _score)
    {
        score = _score;
    }

    // 주사위 충돌 해제 
    public void SetDiceKinematic(bool isActive)
    {
        PV.RPC("SetDiceKinematicRPC", RpcTarget.All, isActive);
    }

    [PunRPC]
    // 주사위 충돌 해제 RPC
    public void SetDiceKinematicRPC(bool isActive)
    {
        this.gameObject.GetComponent<Rigidbody>().isKinematic = isActive;
    }

    // 주사위 눈에 따라 Rot값 반환
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
                Debug.Log("[ERROR] 맞는 score가 없습니다!");
                break;
        }

        return rot;
    }
}
