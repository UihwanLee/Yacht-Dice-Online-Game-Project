using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

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
    private bool isThrown; 

    // private Vector3 spawnPos = new Vector3(2f, 20f, 0f);
    private float spawnTimer = 1f;

    [Header("Dice Bottle")]
    [SerializeField]
    private GameObject diceBottle;

    // 흔들림 세기
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

    // 다이스 위치 업데이트
    public void AllocateDiceAround(Dice[] dices)
    {
        for (int i = 0; i < dices.Length; i++)
        {
            float radian = (2f * Mathf.PI) / dices.Length;
            radian *= i;
            dices[i].Teleport(spawnPos + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));
        }
    }

    // 오브젝트 제거
    public void DestoryDice(GameObject _dice)
    {
        Destroy(_dice, 5f);
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
    }

    #endregion

}
