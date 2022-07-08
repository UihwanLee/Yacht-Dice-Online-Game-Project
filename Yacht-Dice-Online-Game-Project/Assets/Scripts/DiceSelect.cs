using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiceSelect : MonoBehaviour, IPointerClickHandler
{
    /*
    * Select 될 Dice 정보를 담은 스크립트
    */

    // 담고 있는 주사위 눈
    public int score;


    public void OnPointerClick(PointerEventData eventData)
    {
        int click = eventData.clickCount;

        if (click >= 1)
        {
            Debug.Log("한번 이상 클릭");

            // Select Anim 재생 : transform position으로 이동


        }
        else if (click >= 2) Debug.Log("두번 이상 클릭");
    }
}
