using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceSelect : MonoBehaviour, IPointerClickHandler
{
    /*
    * Select �� Dice ������ ���� ��ũ��Ʈ
    */

    // ��� �ִ� �ֻ��� ��
    public int score;


    public void OnPointerClick(PointerEventData eventData)
    {
        int click = eventData.clickCount;

        if (click >= 1)
        {
            Debug.Log("�ѹ� �̻� Ŭ��");

            // Select Anim ��� : transform position���� �̵�


        }
        else if (click >= 2) Debug.Log("�ι� �̻� Ŭ��");
    }
}
