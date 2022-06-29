using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDice : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private Vector3 diceVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = rb.velocity;

        if(Input.GetKeyDown (KeyCode.Space))
        {
            float dirX = Random.Range(0, 500);
            float dirY = Random.Range(0, 500);
            float dirZ = Random.Range(0, 500);

            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * 500);
            rb.AddTorque(dirX, dirY, dirZ);
        }
    }
}
