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

    // ī�޶� �ʱ� ��ġ
    private Vector3 mainCameraInitPosition;
    private Vector3 mainCameraInitRotation;

    // ī�޶� �̵� ��ġ
    private Vector3 mainCameraMovePosition;
    private Vector3 mainCameraMoveRoatation;

    [SerializeField]
    private FallingDice fallingDice;


    // ���� ���� üũ ����
    private bool isMoved;
    private bool isMoving;

    // �̵� �ӵ�
    private float duration = 3f;


    // 2.52 / 4.65 / -13.53
    // -19 / 0 / 0

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        // Title Panel ȣ�� �� Title ����
        titleAnimator.SetTrigger("appear");

    }

    private void Update()
    {
        /*
        // 15�� �������� ���̽� ��ȯ
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0f)
        {
            fallingDice.SpawnYachtDices(12f);
            spawnTimer = 15f;
        }
        */
    }

    // ���� ���� �� �ʱ�ȭ
    private void Init()
    {
        // ���� �޴� ȭ�� UI �ʱ�ȭ
        touchScreenUI.SetActive(true);
        gameStartUI.SetActive(false);
        severEnterUI.SetActive(false);

        // ī�޶� ��ġ ����
        mainCameraInitPosition =  new Vector3(2.52f, 16.1f, -3.5f);
        mainCameraInitRotation =  new Vector3(69.19f, 0f, 0f);

        mainCameraMovePosition = new Vector3(2.52f, 4.65f, -13.53f);
        mainCameraMoveRoatation = new Vector3(-19f, 0f, 0f);

        // ���� üũ ���� �ʱ�ȭ
        isMoved = false;
        isMoving = false;
    }

    public void MovingCamera()
    {
        // ī�޶� �̵�
        StartCoroutine(movingMainCamera());
    }

    // Camera Move �ڷ�ƾ
    IEnumerator movingMainCamera()
    {
        if(isMoving)
        {
            yield break;
        }

        if (gameStartUI != null && severEnterUI != null)
        {
            if (isMoved) severEnterUI.SetActive(false);
            else gameStartUI.SetActive(false);
        }
        else
        {
            Debug.Log("No UI");
        }

        isMoving = true;

        // postion ����
        Vector3 currentPos = mainCamera.transform.position;
        Vector3 movePos = isMoved ? mainCameraInitPosition : mainCameraMovePosition;

        // rotation ����
        Quaternion currentRot = mainCamera.transform.rotation;
        Quaternion moveRot = isMoved ? Quaternion.Euler(mainCameraInitRotation) : Quaternion.Euler(mainCameraMoveRoatation);

        float timer = 0f;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            mainCamera.transform.position = Vector3.Lerp(currentPos, movePos, timer / duration);
            mainCamera.transform.rotation = Quaternion.Lerp(currentRot, moveRot, timer / duration);
            yield return null;
        }
        isMoving = false;

        isMoved = !isMoved;

        // �̵��� UI Ȱ��ȭ
        if (gameStartUI != null && severEnterUI != null)
        {
            if (isMoved) severEnterUI.SetActive(true);
            else gameStartUI.SetActive(true);
        }
    }
}
