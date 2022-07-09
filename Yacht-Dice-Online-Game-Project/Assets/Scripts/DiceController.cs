using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static InGameNetWorkManager;
using static DiceSelectManager;

public class DiceController : MonoBehaviourPunCallbacks
{
    /* Yacht Dice Controller 스크립트
     * 
     *  본 스크립트가 사용 될 경로
     *  - <Title Panel> 떨어지는 주사위 이펙트
     *  - <Game Play Panel> 주사위 컨테이너 위에 떨어지는 이펙트
     * 
    */

    [Header("Dice")]
    // 소환될 포지션 : 높이만 동일 / 위치는 랜덤
    [SerializeField]
    private Vector3 spawnPos;

    // 소환될 주사위들 거리
    [SerializeField]
    private float spawnDistance;

    // Yacht Dice 프리팹
    [SerializeField]
    private GameObject dicePrefab;

    // 주사위 이펙트 관리 변수
    public bool isThrown; 

    // private Vector3 spawnPos = new Vector3(2f, 20f, 0f);
    private float spawnTimer = 1f;

    // Dice 관리할 리스트
    public List<Dice> Dices = new List<Dice>();

    // 돌릴 Dice 개수
    public int remainDiceCount;

    [Header("Dice Bottle")]
    [SerializeField]
    private GameObject diceBottle;

    // 흔들림 세기
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
            // 15초 간격으로 다이스 소환
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                SpawnYachtDices(12f);
                spawnTimer = 15f;
            }
        }
    }

    #region Dice Manager

    // 주사위 떨어지는 이펙트 실행 / 중단
    public void FallingDice(bool _isThrown)
    {
        isThrown = _isThrown;
    }

    // SpawnPos 설정
    public void SetSpawnPos(Vector3 _spawnPos)
    {
        spawnPos = _spawnPos;
    }

    // 다이스 소환
    public void SpawnYachtDices(float time)
    {
        for(int i=0; i<5; i++)
        {
            var dice = Instantiate(dicePrefab, spawnPos, Quaternion.identity).GetComponent<Dice>();
            
            // 원으로 배치하여 소환
            float radian = (3f * Mathf.PI) / 5;
            radian *= i;
            dice.Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));

            // 주사위 굴리기
            dice.RollDice();

            // 일정 시간 이후 오브젝트 삭제
            dice.Destory(time);
        }
    }

    // 오브젝트 제거
    public void DestoryDice(GameObject _dice)
    {
        Destroy(_dice, 5f);
    }

    // 인게임 다이스 소환
    public void SpawnYachtDicesInGame(float _spawnDistance)
    {
        for (int i = 0; i < 5; i++)
        {
            var dice = PhotonNetwork.Instantiate("DicePrefab", spawnPos, Quaternion.identity).GetComponent<Dice>();

            // 원으로 배치하여 소환
            float radian = (3f * Mathf.PI) / 5;
            radian *= i;
            dice.Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _spawnDistance));

            // 주사위 굴리기
            dice.RollDice();
        }
    }

    // 인게임 다이스 리롤
    public void RerollYachtDices()
    {
        // 돌릴 주사위 개수 업데이트
        UpdateRemainDiceCount();


        for (int i=0; i<Dices.Count; i++)
        {
            // 선택되지 않은 주사위들로만 리롤
            if(!Dices[i].isSelect)
            {
                // 원으로 배치하여 소환
                float radian = (3f * Mathf.PI) / remainDiceCount;
                radian *= i;
                float _spawnDistance = 1f;
                Dices[i].Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * _spawnDistance));

                // 주사위 굴리기
                Dices[i].RollDice();
            }
        }
    }

    // 주사위 눈 순서대로 정렬
    public void SortDices()
    {
        Dices.Sort((p1, p2) => p1.score.CompareTo(p2.score));
    }

    // 주사위 콜라이더 켜기(주사위 눈 확인 함수)
    IEnumerator AcitveDiceColiderCourutine()
    {
        yield return new WaitForSeconds(8f);
        for (int i = 0; i < Dices.Count; i++)
        {
            if (!Dices[i].isSelect) Dices[i].SetSidesColider(true);
        }

        // Reroll로 돌리기
        IN.SetRollButtonReroll();

        yield return new WaitForSeconds(0.5f);
        // '낙' 된 주사위가 / 주사위 눈이 배정이 안된 주사위 있을 시 다시 던지기
        if (CheckDicePos())
        {
            Debug.Log("낙");
            IN.SetDice();
        }
        else
        {
            // 주사위 선택:
            // 1) 주사위 눈 순서대로 정렬
            // 2) 돌릴 주사위에 따라 DiceSelect UI 선택
            // 3) 선택한 DiceSelect UI 오브젝트 각각 점수 업데이트
            // 4) 주사위 오브젝트 Select Pos List 선택
            // 5) 주사위 충돌 해제 (RPC)
            // 6) 주사위 눈 기준으로 정렬 후 DiceSelect Pos, Rot에 따라 오브젝트 이동

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

    // '낙'이 판정이 있는 주사위 확인
    public bool CheckDicePos()
    {
        for(int i=0; i<Dices.Count; i++)
        {
            // 선택된 주사위는 제외
            if(!Dices[i].isSelect)
            {
                if (Dices[i].transform.localPosition.y < 1.6f || Dices[i].transform.localPosition.y > 1.75f || Dices[i].score == 0) return true;
            }
        }
        return false;
    }

    // 돌릴 주사위 개수 업데이트
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

    // Dice isSelect 초기화 (한 플레이어가 처음 돌릴때 적용)
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
        // Init Bottle Anim으로 초기화
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);
    }

    public void SetBottlePlayingPos()
    {
        // set Bottle Anim으로 초기화
        diceBottle.GetComponent<Animator>().SetTrigger("set");
    }


    // 다이스 Bottle 흔들기
    public void ShakingBottle()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", true);
    }

    // 다이스 Bottle 던지기
    public void ThrowBottle()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", false);
        StartCoroutine(AcitveDiceColiderCourutine());
    }

    // 다이스 Bottle 다시 세팅(Reroll)
    public void ReBottlePlayingPos()
    {
        diceBottle.GetComponent<Animator>().SetTrigger("reroll");
    }

    #endregion

}
