using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static InGameNetWorkManager;

public class DiceSelect : MonoBehaviour, IPointerClickHandler
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

    // Ȱ��ȭ �ɶ����� score �ʱ�ȭ : score�� 0�� ��� ������Ʈ�� ������� �ʴ� ���̱⿡ Ŭ���ص� ������ ���ϴ� ����ó�� ����
    private void OnEnable()
    {
        this.score = 0;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // ����ó��
        if (!TryClick()) return;

        int click = eventData.clickCount;

        if (click == 1)
        {
            Debug.Log("�ѹ� Ŭ��");

            if (isSelectZone)
            {
                // Select UI �̵�
                diceSelectManager.SetSelectZoneSelectUI(true);
                diceSelectManager.SetReturnZoneSelectUI(false);
                diceSelectManager.selectZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 0f, 0f);
            }
            else
            {
                // ReturnZone �ȿ� �ֻ����� ���� ��츸 Ȱ��ȭ
                if(score != 0)
                {
                    // Select UI �̵�
                    diceSelectManager.SetSelectZoneSelectUI(false);
                    diceSelectManager.SetReturnZoneSelectUI(true);
                    diceSelectManager.returnZoneSelectUI.transform.localPosition = new Vector3(this.transform.localPosition.x, 272f, 0f);
                }
            }
        }
        else if (click >= 2)
        {
            Debug.Log("�ι� �̻� Ŭ��");
            if (isSelectZone)
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                diceSelectManager.SelectDice(this);
            }
            else
            {
                diceSelectManager.SetSelectZoneSelectUI(false);
                diceSelectManager.SetReturnZoneSelectUI(false);
                diceSelectManager.ReturnDice(this);
            }
        }
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
