using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static InGameNetWorkManager;

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

    [Header("PV")]
    public PhotonView PV;

    [Header("Scripts")]

    public static DiceController DC;
    [SerializeField]
    private DiceSelectManager diceSelectManager;
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;

    private void Awake()
    {
        DC = this;
    }

    private void Start()
    {
        isThrown = true;
        remainDiceCount = 5;

        PV = photonView;
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

    public void GetDices()
    {
        PV.RPC("GetDicesRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void GetDicesRPC()
    {
        this.Dices = IN.newDices;
    }

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

    public void RerollYachtDices()
    {
        PV.RPC("RerollYachtDicesRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // 인게임 다이스 리롤
    private void RerollYachtDicesRPC()
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
        yield return new WaitForSeconds(5f);
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
            IN.StartFailThrowDiceAnim();
        }
        else
        {
            // 주사위 선택:
            // 1) 주사위 눈 순서대로 정렬
            // 2) ReturnZone 활성화
            // 3) 돌릴 주사위에 따라 DiceSelect UI 선택
            // 4) 선택한 DiceSelect UI 오브젝트 각각 점수 업데이트
            // 5) 주사위 오브젝트 Select Pos List 선택
            // 6) 주사위 충돌 해제 (RPC)
            // 7) 주사위 눈 기준으로 정렬 후 DiceSelect Pos, Rot에 따라 오브젝트 이동

            // 1) 플레이어 스코어 보드 가져오기
            // 2) 나온 주사위 눈에 따라 스코어 보드 업데이트

            //diceSelectManager.PV.RPC("SetPlayerSelectDice", RpcTarget.AllBuffered, remainDiceCount);

            SortDices();
            UpdateRemainDiceCount();

            SetAllDiceKinematic(true);

            diceSelectManager.SetReturnZoneAcitve(true);
            diceSelectManager.UpdateSelectZone();
            diceSelectManager.SetAllDicesToBeSelectMode();

            scoreBoardManager.SetActiveCurrentPlayerScoreBoard(true);
            scoreBoardManager.UpdateCurrentPlayerScoreBoard();

            if (remainDiceCount == 5) diceSelectManager.ResetReturnZoneSelectUIScore();

            IN.SetRerollCountUI(true);

            // 플레이어 리롤 카운트가 끝났을 시, 더이상 리롤을 하지 못하며 점수를 바로 선택하게 한다.
            // 올려진 주사위들은 모두 ReturnZone으로 보냄 
            if (IN.Players[IN.currentPlayerSequence].rerollCount == 0)
            {
                yield return new WaitForSeconds(1f);
                diceSelectManager.MoveAllDicesReturnZone();
            }

        }
    }

    // 주사위 충돌 해제
    public void SetAllDiceKinematic(bool isActive)
    {
        foreach (var dice in Dices)
        {
            if (!dice.isSelect)
            {
                dice.SetDiceKinematic(isActive);
            }
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
                if (Dices[i].transform.localPosition.y < 1.6f || Dices[i].transform.localPosition.y > 2f)
                {
                    PV.RPC("SendError", RpcTarget.AllBuffered, ("낙: 거리가 멀리 떨어져있음 주사위 y좌표 : " + (Dices[i].transform.localPosition.y).ToString()));
                    return true;
                }
                else if (Dices[i].score == 0)
                {
                    PV.RPC("SendError", RpcTarget.AllBuffered, ("낙: 주사위 점수가 집계되지 않음"));
                    return true;
                }
            }
        }
        return false;
    }

    [PunRPC]
    private void SendError(string errorMsg)
    {
        Debug.Log(errorMsg);
    }


    public void ResetRemainDiceCount()
    {
        remainDiceCount = 5;
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

    public void SetBottleInitPosByTransform()
    {
        // transform으로 위치 초기화
        diceBottle.transform.localPosition = new Vector3(-178.39f, 1.51f, 5.8f);
    }

    public void SetBottleInitPos()
    {
        PV.RPC("SetBottleInitPosRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SetBottleInitPosRPC()
    {
        // Init Bottle Anim으로 초기화
        diceBottle.GetComponent<Animator>().SetTrigger("init");
    }

    public void SetBottlePlayingPos()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetTrigger("down");
        PV.RPC("SetBottlePlayingPosRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetBottlePlayingPosRPC()
    {
        // set Bottle Anim으로 초기화
        diceBottle.GetComponent<Animator>().SetTrigger("set");
    }


    // 다이스 Bottle 흔들기
    public void ShakingBottle()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetBool("isDown", true);
        PV.RPC("ShakingBottleRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // 다이스 Bottle 흔들기
    private void ShakingBottleRPC()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", true);
    }

    // 다이스 Bottle 던지기
    public void ThrowBottle()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetBool("isDown", false);
        PV.RPC("ThrowBottleRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    // 다이스 Bottle 던지기
    private void ThrowBottleRPC()
    {
        diceBottle.GetComponent<Animator>().SetBool("isShake", false);
        StartCoroutine(AcitveDiceColiderCourutine());
    }

    // 다이스 Bottle 다시 세팅(Reroll)
    public void ReBottlePlayingPos()
    {
        IN.rollDiceButton.GetComponent<Animator>().SetTrigger("down");
        PV.RPC("ReBottlePlayingPosRPC", RpcTarget.AllBuffered);
    }


    [PunRPC]
    // 다이스 Bottle 다시 세팅(Reroll)
    private void ReBottlePlayingPosRPC()
    {
        diceBottle.GetComponent<Animator>().SetTrigger("reroll");
    }

    #endregion

}
