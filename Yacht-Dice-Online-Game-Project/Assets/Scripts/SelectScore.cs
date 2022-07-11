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

    public void OnPointerClick(PointerEventData eventData)
    {
        // ����ó��
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(true);
            scoreBoardManager.selectScoreUI.transform.localPosition = (isChallengeScore) ? new Vector3(this.transform.localPosition.x, -450f, 0f) : new Vector3(this.transform.localPosition.x, -280f, 0f);

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
