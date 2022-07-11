using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;

public class SelectDice : MonoBehaviour, IPointerClickHandler
{
    /*
    * Select 될 Dice 정보를 담은 스크립트
    */


    // 담고 있는 주사위 눈
    public int score;
    // 내포하고 있는 인덱스
    public int index;

    // selectZone인지 returnZone인지 확인하는 변수
    public bool isSelectZone;

    // DiceSelectManager 스크립트
    [SerializeField]
    private DiceSelectManager diceSelectManager;
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;

    // 활성화 될때마다 score 초기화 : score가 0일 경우 오브젝트가 담겨있지 않는 뜻이기에 클릭해도 반응을 안하는 예외처리 가능
    private void OnEnable()
    {
        this.score = 0;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // 예외처리
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            diceSelectManager.ToClickOneSelectDice(this.index);
        }
        else if (click >= 2)
        {
            diceSelectManager.ToClickOneSelectDice(this.index);
        }
    }

    public bool TryClick()
    {
        // 현재 순서에 맞는 플레이어만 클릭 가능
        if (IN.Players[IN.currentPlayerSequence].GetPlayerNickName() != IN.MyPlayer.GetPlayerNickName()) return false;
        // 주사위가 존재하지 않을 경우 반응 없음
        else if (this.score == 0) return false;
        else return true;
    }

    #region 클릭 함수

    public void ClickOneSelectDice()
    {
        if (isSelectZone)
        {
            // Select UI 이동
            diceSelectManager.SetSelectZoneSelectUI(true);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            scoreBoardManager.ChangeSelectScore(-1, false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);
        }
        else
        {
            // ReturnZone 안에 주사위가 있을 경우만 활성화
            if (score != 0)
            {
                // Select UI 이동
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(true);
                scoreBoardManager.SetSelectScoreUI(false);
                scoreBoardManager.ChangeSelectScore(-1, false);
                diceSelectManager.returnZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 272f, 0f);
            }
        }
    }

    public void TwiceClickSelectDice()
    {
        if (isSelectZone)
        {
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.SelectDiceUI(this);
        }
        else
        {
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            scoreBoardManager.ChangeSelectScore(-1, false);
            diceSelectManager.ReturnDiceUI(this);
        }
    }

    #endregion
}
