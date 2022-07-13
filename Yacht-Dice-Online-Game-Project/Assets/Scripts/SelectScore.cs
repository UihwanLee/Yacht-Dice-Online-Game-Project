using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;
using static DiceController;

public class SelectScore : MonoBehaviour, IPointerClickHandler
{
    [Header("variable")]
    // �����ϰ� �ִ� �ε���
    public int index;
    // ��� �ִ� ����
    public int score;

    // ���� ���� �������� �ƴ��� üũ
    [SerializeField]
    private bool isChallengeScore; 

    // ��ũ��Ʈ
    [Header("Scripts")]
    [SerializeField]
    private ScoreBoardManager scoreBoardManager;
    [SerializeField]
    private DiceSelectManager diceSelectManager;

    // ����� ����
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

        // Select UI �̵�
        diceSelectManager.SetSelectZoneSelectUI(false);
        diceSelectManager.SetReturnZoneSelectUI(false);
        scoreBoardManager.SetSelectScoreUI(true);
        scoreBoardManager.MovingSelectUITransform(isChallengeScore, this.transform.localPosition.x); // RPC

        // ���� ��ȭ : (ScoreBoardManager)�� �̿��Ͽ� index�� ���� ������ ��ȭ��Ų��.
        scoreBoardManager.ChangeSelectScore(this.index, false);

        if (!m_IsOneClick)
        {
            m_Timer = Time.time;
            m_IsOneClick = true;
        }
        else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
        {
            m_IsOneClick = false;
            //�Ʒ��� ����Ŭ������ ó���ϰ���� �̺�Ʈ �ۼ�

            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);

            // ���� ScoreBoardManager ��ũ��Ʈ�� ���ؼ� �ҷ�����
            this.score = scoreBoardManager.GetCurrentPlayerScoreBoardScore(this.index, this.isChallengeScore);

            // �÷��̾ ���ʽ� ���� ���� �޼� ���� ���¿��� ���ʽ� 63�� �̻� �޼� �� �� ���ʽ�+35 �ִϸ��̼� ���
            if (!isChallengeScore && !IN.Players[IN.currentPlayerSequence].bonusScoreSuccess && (IN.Players[IN.currentPlayerSequence].bonusScore + this.score) >= 63)
            {
                string bonusMsg = "Bonus +35";
                scoreBoardManager.ShowChallengeSuccess(bonusMsg);
            }

            // ������ Ȯ���� �Ǹ� �ش� �÷��̾�� ���� ���� ��, ���� �÷��̾� ����
            scoreBoardManager.ChangeSelectScore(this.index, true);
            IN.Players[IN.currentPlayerSequence].SetScore(this.score, this.index, this.isChallengeScore);

            // ��� �ֻ����� ReturnZone�� ���� ���� ��, ��� �ֻ����� ReturnZone���� ������
                if (DC.remainDiceCount != 0) diceSelectManager.MoveAllDicesReturnZone();

            // ���� �÷��̾�/���� ����
            IN.NextPlayer();
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
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(true);
            scoreBoardManager.MovingSelectUITransform(isChallengeScore, this.transform.localPosition.x); // RPC

            // ���� ��ȭ : (ScoreBoardManager)�� �̿��Ͽ� index�� ���� ������ ��ȭ��Ų��.
            scoreBoardManager.ChangeSelectScore(this.index, false);

        }
        else if (click >= 2)
        {
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);

            // ���� ScoreBoardManager ��ũ��Ʈ�� ���ؼ� �ҷ�����
            this.score = scoreBoardManager.GetCurrentPlayerScoreBoardScore(this.index, this.isChallengeScore);

            // ������ Ȯ���� �Ǹ� �ش� �÷��̾�� ���� ���� ��, ���� �÷��̾� ����
            scoreBoardManager.ChangeSelectScore(this.index, true);
            IN.Players[IN.currentPlayerSequence].SetScore(this.score, this.index, this.isChallengeScore);

            // ��� �ֻ����� ReturnZone�� ���� ���� ��, ��� �ֻ����� ReturnZone���� ������
            if(DC.remainDiceCount!=0) diceSelectManager.MoveAllDicesReturnZone();

            // ���� �÷��̾�/���� ����
            IN.NextPlayer();
        }
        */
    }

    private bool TryClick()
    {
        // ���� ������ �´� �÷��̾ Ŭ�� ����
        if (IN.Players[IN.currentPlayerSequence].GetPlayerNickName() != IN.MyPlayer.GetPlayerNickName()) return false;
        // �̹� Ȯ���� �����̸� Ŭ�� �Ұ���
        else if (scoreBoardManager.CheckIsSelectedScore(this.index, this.isChallengeScore)) return false;
        else return true;
    }
}
