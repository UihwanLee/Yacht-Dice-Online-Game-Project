using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;

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

        // ���� ScoreBoardManager ��ũ��Ʈ�� ���ؼ� �ҷ�����

        if (click == 1)
        {
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(false);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(true);
            scoreBoardManager.selectScoreUI.transform.localPosition = (isChallengeScore) ? new Vector3(this.transform.localPosition.x, -450f, 0f) : new Vector3(this.transform.localPosition.x, -280f, 0f);

            // ���� ��ȭ : (ScoreBoardManager)�� �̿��Ͽ� index�� ���� ������ ��ȭ��Ų��.
            scoreBoardManager.ChangeColorSelectScore(this.index);

        }
        else if (click >= 2)
        {
            // Select UI �̵�
            diceSelectManager.SetSelectZoneSelectUI(true);
            diceSelectManager.SetReturnZoneSelectUI(false);
            scoreBoardManager.SetSelectScoreUI(false);
            diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);

            // ������ Ȯ���� �Ǹ� �ش� �÷��̾�� ���� ���� ��, ���� �÷��̾� ����
        }
    }

    private bool TryClick()
    {
        // ���� ������ �´� �÷��̾ Ŭ�� ����
        if (IN.Players[IN.currentPlayerSequence].GetPlayerNickName() != IN.MyPlayer.GetPlayerNickName()) return false;
        else return true;
    }
}
