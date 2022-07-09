using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DiceCheckZone : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Vector3 diceVelocity;

    private int diceNum = 0;


    private void FixedUpdate()
    {
        diceVelocity = Dice.diceVelcoity;
    }

    private void OnTriggerStay(Collider other)
    {
        if(diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            switch(other.gameObject.name)
            {
                case "Side1":
                    diceNum = 6;
                    break;
                case "Side2":
                    diceNum = 5;
                    break;
                case "Side3":
                    diceNum = 4;
                    break;
                case "Side4":
                    diceNum = 3;
                    break;
                case "Side5":
                    diceNum = 2;
                    break;
                case "Side6":
                    diceNum = 1;
                    break;
            }

            other.transform.parent.GetComponent<Dice>().SetScore(diceNum);
        }
    }
}
