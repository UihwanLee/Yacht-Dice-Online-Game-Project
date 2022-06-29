using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheckZone : MonoBehaviour
{
    [SerializeField]
    Vector3 diceVelocity;

    private int diceNum = 0;

    private void FixedUpdate()
    {
        diceVelocity = DIce.diceVelcoity;
    }

    private void OnTriggerStay(Collider other)
    {
        if(diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            switch(other.gameObject.name)
            {
                case "Side1":
                    diceNum = 1;
                    break;
                case "Side2":
                    diceNum = 2;
                    break;
                case "Side3":
                    diceNum = 3;
                    break;
                case "Side4":
                    diceNum = 4;
                    break;
                case "Side5":
                    diceNum = 5;
                    break;
                case "Side6":
                    diceNum = 6;
                    break;
            }

            Debug.Log(diceNum);
        }
    }
}