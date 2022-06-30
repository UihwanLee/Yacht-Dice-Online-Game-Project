using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    /* <Title Panel ���� ��ũ��Ʈ>
     * 
     * Title Panel Animation ����
     * Title Panel Effect ����
     * Title Panel Button ����
     * Main Camera ����
     * 
    */

    [SerializeField]
    private Animator titleAnimator;

    [SerializeField]
    private GameObject touchScreenUI;

    [SerializeField]
    private GameObject gameStartUI;

    [SerializeField]
    private GameObject severEnterUI;

    [SerializeField]
    private Camera mainCamera;

    private Vector3 mainCameraPosition;
    private Vector3 mainCameraRotation;

    [SerializeField]
    private FallingDice fallingDice;

    private Vector3 spawnPos = new Vector3(2f, 20f, 0f);
    private float timer = 1f;

    public void Awake()
    {
        Init();

        fallingDice.SetSpawnPos(spawnPos);
    }

    private void OnEnable()
    {
        // Title Panel ȣ�� �� Title ����
        titleAnimator.SetBool("isAppear", true);

        if (mainCamera != null)
        {
            Debug.Log("Camera ���� �Ϸ� " + mainCameraPosition + " / " + mainCameraRotation);
            mainCamera.transform.position = mainCameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }
    }

    private void Update()
    {
        // 15�� �������� ���̽� ��ȯ
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            fallingDice.SpawnYachtDices(12f);
            timer = 15f;
        }

    }

    // ���� ���� �� �ʱ�ȭ
    public void Init()
    {
        // ���� �޴� ȭ�� UI �ʱ�ȭ
        touchScreenUI.SetActive(true);
        gameStartUI.SetActive(false);
        severEnterUI.SetActive(false);

        // ī�޶� ��ġ ����
        mainCameraPosition = mainCamera.transform.position;// new Vector3(2.52f, 16.1f, -3.5f);
        mainCameraRotation = mainCamera.transform.rotation.eulerAngles; // new Vector3(69.19f, 0f, 0f);
    }
}
