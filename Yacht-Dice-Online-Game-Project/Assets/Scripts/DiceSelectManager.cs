using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject diceSelcetUI;

    // ReturnZone, SelectZone Pos, Rot 저장 변수
    private List<Vector3> returnZonePos = new List<Vector3>();
    private List<Vector3> selectZonePos_5Dice = new List<Vector3>();
    private List<Vector3> selectZonePos_4Dice = new List<Vector3>();

    private Vector3 returnZoneRot;
    private Vector3 selectZoneRot;

    // ReturnZone, SelectZone UI 변수
    [SerializeField]
    private List<GameObject> returnZoneDice = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice5 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice4 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice3 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice2 = new List<GameObject>();
    [SerializeField]
    private List<GameObject> dice1 = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //diceSelcetUI.SetActive(false);

        // returnZonPos 초기화
        returnZonePos.Add(new Vector3(-182.41f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-181.45f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-180.49f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-179.53f, 3.5f, 3f));
        returnZonePos.Add(new Vector3(-178.57f, 3.5f, 3f));

        // selectZonePos_5Dice 초기화
        selectZonePos_5Dice.Add(new Vector3(-182.1f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-181.3f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-180.5f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-179.7f, 9.71f, 0.05f));
        selectZonePos_5Dice.Add(new Vector3(-178.9f, 9.71f, 0.05f));

        // selectZonePos_4Dice 초기화
        selectZonePos_4Dice.Add(new Vector3(-181.7f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.9f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-180.1f, 9.71f, 0.05f));
        selectZonePos_4Dice.Add(new Vector3(-179.3f, 9.71f, 0.05f));

        // returnZonRot 초기화
        returnZoneRot = new Vector3(-14f, 0f, 0f);

        // selectZoneRot 초기화
        selectZoneRot = new Vector3(-7.68f, 0f, 0f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
