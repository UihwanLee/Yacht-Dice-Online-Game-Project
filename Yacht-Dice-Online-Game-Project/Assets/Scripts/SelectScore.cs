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

    public void OnPointerClick(PointerEventData eventData)
    {
        // 예외처리
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            // Select UI 이동
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(true);
            scoreBoardManager.selectScoreUI.transform.localPosition = (isChallengeScore) ? new Vector3(this.transform.localPosition.x, -450f, 0f) : new Vector3(this.transform.localPosition.x, -280f, 0f);

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
