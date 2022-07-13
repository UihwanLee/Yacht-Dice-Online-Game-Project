using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;
using static DiceController;

public class SelectScore : MonoBehaviour, IPointerClickHandler
{
    [Header("variable")]
    // 내포하고 있는 인덱스
    public int index;
    // 담고 있는 점수
    public int score;

    // 도전 과제 점수인지 아닌지 체크
    [SerializeField]
    private bool isChallengeScore; 

    // 스크립트
    [Header("Scripts")]
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;
    [SerializeField]
    private DiceSelectManager diceSelectManager;

    // 모바일 변수
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    private void Update()
    {
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {
            m_IsOneClick = false;
        }
    }

    public void OnClick()
    {
        if (!TryClick()) return;

        // Select UI 이동
        diceSelectManager.SetSelectZoneSelectUI(false);
        diceSelectManager.SetReturnZoneSelectUI(false);
        scoreBoardManager.SetSelectScoreUI(true);
        scoreBoardManager.MovingSelectUITransform(isChallengeScore, this.transform.localPosition.x); // RPC

        // 색깔 변화 : (ScoreBoardManager)를 이용하여 index를 통해 색깔을 변화시킨다.
        scoreBoardManager.ChangeSelectScore(this.index, false);

        if (!m_IsOneClick)
        {
            m_Timer = Time.time;
            m_IsOneClick = true;
        }
        else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
        {
            m_IsOneClick = false;
            //아래에 더블클릭에서 처리하고싶은 이벤트 작성

            // Select UI 이동
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);

            // 점수 ScoreBoardManager 스크립트를 통해서 불러오기
            this.score = scoreBoardManager.GetCurrentPlayerScoreBoardScore(this.index, this.isChallengeScore);

            // 플레이어가 보너스 점수 아직 달성 못한 상태에서 보너스 63점 이상 달성 할 시 보너스+35 애니메이션 재생
            if (!isChallengeScore && !IN.Players[IN.currentPlayerSequence].bonusScoreSuccess && (IN.Players[IN.currentPlayerSequence].bonusScore + this.score) >= 63)
            {
                string bonusMsg = "Bonus +35";
                scoreBoardManager.ShowChallengeSuccess(bonusMsg);
            }

            // 점수가 확정이 되며 해당 플레이어에게 정보 전달 후, 다음 플레이어 개시
            scoreBoardManager.ChangeSelectScore(this.index, true);
            IN.Players[IN.currentPlayerSequence].SetScore(this.score, this.index, this.isChallengeScore);

            // 모든 주사위가 ReturnZone에 있지 않을 시, 모든 주사위를 ReturnZone으로 보내기
                if (DC.remainDiceCount != 0) diceSelectManager.MoveAllDicesReturnZone();

            // 다음 플레이어/라운드 진행
            IN.NextPlayer();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        // 예외처리
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            // Select UI 이동
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(true);
            scoreBoardManager.MovingSelectUITransform(isChallengeScore, this.transform.localPosition.x); // RPC

            // 색깔 변화 : (ScoreBoardManager)를 이용하여 index를 통해 색깔을 변화시킨다.
            scoreBoardManager.ChangeSelectScore(this.index, false);

        }
        else if (click >= 2)
        {
            // Select UI 이동
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);

            // 점수 ScoreBoardManager 스크립트를 통해서 불러오기
            this.score = scoreBoardManager.GetCurrentPlayerScoreBoardScore(this.index, this.isChallengeScore);

            // 점수가 확정이 되며 해당 플레이어에게 정보 전달 후, 다음 플레이어 개시
            scoreBoardManager.ChangeSelectScore(this.index, true);
            IN.Players[IN.currentPlayerSequence].SetScore(this.score, this.index, this.isChallengeScore);

            // 모든 주사위가 ReturnZone에 있지 않을 시, 모든 주사위를 ReturnZone으로 보내기
            if(DC.remainDiceCount!=0) diceSelectManager.MoveAllDicesReturnZone();

            // 다음 플레이어/라운드 진행
            IN.NextPlayer();
        }
        */
    }

    private bool TryClick()
    {
        // 현재 순서에 맞는 플레이어만 클릭 가능
        if (IN.Players[IN.currentPlayerSequence].GetPlayerNickName() != IN.MyPlayer.GetPlayerNickName()) return false;
        // 이미 확정된 점수이면 클릭 불가능
        else if (scoreBoardManager.CheckIsSelectedScore(this.index, this.isChallengeScore)) return false;
        else return true;
    }
}
