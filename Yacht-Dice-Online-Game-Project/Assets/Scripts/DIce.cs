using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIce : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private Vector3 initPosition;
    [SerializeField]
    private int diceValue;

    public static Vector3 diceVelcoity;

    private bool thrown;

 
    private void Start()
    {
        thrown = false;
    }

    // Update is called once per frame
    void Update()
    {
        diceVelcoity = rb.velocity;
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
        */
        RollDice();
    }

    void RollDice()
    {
        if(!thrown)
        {
            thrown = true;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        }
    }
}
