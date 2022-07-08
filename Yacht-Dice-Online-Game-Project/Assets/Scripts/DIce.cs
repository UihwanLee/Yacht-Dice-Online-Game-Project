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

    // 다이스 점수
    public int score;

    // select 되었는지 확인 하는 변수
    public bool isSelect;

    // 6개의 사이드 콜라이더 
    [SerializeField]
    private List<GameObject> sides = new List<GameObject>();



    private void Start()
    {
        // 6개의 콜라이더 초기화
        SetSidesColider(false);

        score = 0;
        isSelect = false;

        thrown = false;

        if(!DC.isThrown)
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
        if (DC.Dices.Contains(this))
        {
            DC.Dices.Remove(this);
            
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
        score = _score;
    }
}
