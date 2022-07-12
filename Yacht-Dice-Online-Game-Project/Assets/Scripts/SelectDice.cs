using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;

public class SelectDice : MonoBehaviour, IPointerClickHandler
{
    /*
    * Select �� Dice ������ ���� ��ũ��Ʈ
    */


    // ��� �ִ� �ֻ��� ��
    public int score;
    // �����ϰ� �ִ� �ε���
    public int index;

    // selectZone���� returnZone���� Ȯ���ϴ� ����
    public bool isSelectZone;

    // DiceSelectManager ��ũ��Ʈ
    [SerializeField]
    private DiceSelectManager diceSelectManager;
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;

    // ����� ����
    public float m_DoubleClickSecond = 0.25f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    // Ȱ��ȭ �ɶ����� score �ʱ�ȭ : score�� 0�� ��� ������Ʈ�� ������� �ʴ� ���̱⿡ Ŭ���ص� ������ ���ϴ� ����ó�� ����
    private void OnEnable()
    {
        this.score = 0;
    }

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

        if (isSelectZone)
        {
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(true);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            scoreBoardManager.ChangeSelectScore(-1, false);
            diceSelectManager.ToMovingOnSelectDiceUI(isSelectZone, this.transform.localPosition.x);
        }
        else
        {
            // ReturnZone �ȿ� �ֻ����� ���� ��츸 Ȱ��ȭ
            if (score != 0)
            {
                // Select UI �̵�
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(true);
                scoreBoardManager.SetSelectScoreUI(false);
                scoreBoardManager.ChangeSelectScore(-1, false);
                diceSelectManager.ToMovingOnSelectDiceUI(isSelectZone, this.transform.localPosition.x);
            }
        }

        if (!m_IsOneClick)
        {
            m_Timer = Time.time;
            m_IsOneClick = true;
        }
        else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
        {
            m_IsOneClick = false;
            //�Ʒ��� ����Ŭ������ ó���ϰ���� �̺�Ʈ �ۼ�

            if (isSelectZone)
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                scoreBoardManager.SetSelectScoreUI(false);
                diceSelectManager.SelectDiceUI(this.index); // RPC
            }
            else
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                scoreBoardManager.SetSelectScoreUI(false);
                scoreBoardManager.ChangeSelectScore(-1, false);
                diceSelectManager.ReturnDiceUI(this.index); // RPC
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        /*
        // ����ó��
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            if (isSelectZone)
            {
                // Select UI �̵�
                diceSelectManager.SetSelectZoneSelectUI(true);
                diceSelectManager.SetReturnZoneSelectUI(false);
                scoreBoardManager.SetSelectScoreUI(false);
                scoreBoardManager.ChangeSelectScore(-1, false);
                diceSelectManager.ToMovingOnSelectDiceUI(isSelectZone, this.transform.localPosition.x);
                //diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);
            }
            else
            {
                // ReturnZone �ȿ� �ֻ����� ���� ��츸 Ȱ��ȭ
                if (score != 0)
                {
                    // Select UI �̵�
                    diceSelectManager.SetSelectZoneSelectUI(false);
                    diceSelectManager.SetReturnZoneSelectUI(true);
                    scoreBoardManager.SetSelectScoreUI(false);
                    scoreBoardManager.ChangeSelectScore(-1, false);
                    diceSelectManager.ToMovingOnSelectDiceUI(isSelectZone, this.transform.localPosition.x);
                    //diceSelectManager.returnZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 272f, 0f);
                }
            }
        }
        else if (click >= 2)
        {
            if (isSelectZone)
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                scoreBoardManager.SetSelectScoreUI(false);
                diceSelectManager.SelectDiceUI(this.index); // RPC
            }
            else
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                scoreBoardManager.SetSelectScoreUI(false);
                scoreBoardManager.ChangeSelectScore(-1, false);
                diceSelectManager.ReturnDiceUI(this.index); // RPC
            }
        }
        */
    }


    public bool TryClick()
    {
        // ���� ������ �´� �÷��̾ Ŭ�� ����
        if (IN.Players[IN.currentPlayerSequence].GetPlayerNickName() != IN.MyPlayer.GetPlayerNickName()) return false;
        // �ֻ����� �������� ���� ��� ���� ����
        else if (this.score == 0) return false;
        else return true;
    }
}
