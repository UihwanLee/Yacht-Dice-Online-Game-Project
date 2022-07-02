using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    /* <Title Panel 관리 스크립트>
     * 
     * Title Panel Animation 관리
     * Title Panel Effect 관리
     * Title Panel Button 관리
     * Main Camera 관리
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

    // 카메라 초기 위치
    private Vector3 mainCameraInitPosition;
    private Vector3 mainCameraInitRotation;

    // 카메라 이동 위치
    private Vector3 mainCameraMovePosition;
    private Vector3 mainCameraMoveRoatation;

    [SerializeField]
    private FallingDice fallingDice;


    // 게임 시작 체크 변수
    private bool isMoved;
    private bool isMoving;

    // 이동 속도
    private float duration = 3f;


    // 2.52 / 4.65 / -13.53
    // -19 / 0 / 0

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        // Title Panel 호출 시 Title 띄우기
        titleAnimator.SetTrigger("appear");

    }

    private void Update()
    {
        /*
        // 15초 간격으로 다이스 소환
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0f)
        {
            fallingDice.SpawnYachtDices(12f);
            spawnTimer = 15f;
        }
        */
    }

    // 게임 시작 시 초기화
    private void Init()
    {
        // 메인 메뉴 화면 UI 초기화
        touchScreenUI.SetActive(true);
        gameStartUI.SetActive(false);
        severEnterUI.SetActive(false);

        // 카메라 위치 세팅
        mainCameraInitPosition =  new Vector3(2.52f, 16.1f, -3.5f);
        mainCameraInitRotation =  new Vector3(69.19f, 0f, 0f);

        mainCameraMovePosition = new Vector3(2.52f, 4.65f, -13.53f);
        mainCameraMoveRoatation = new Vector3(-19f, 0f, 0f);

        // 시작 체크 변수 초기화
        isMoved = false;
        isMoving = false;
    }

    public void MovingCamera()
    {
        // 카메라 이동
        StartCoroutine(movingMainCamera());
    }

    // Camera Move 코루틴
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

        // postion 설정
        Vector3 currentPos = mainCamera.transform.position;
        Vector3 movePos = isMoved ? mainCameraInitPosition : mainCameraMovePosition;

        // rotation 설정
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

        // 이동이 UI 활성화
        if (gameStartUI != null && severEnterUI != null)
        {
            if (isMoved) severEnterUI.SetActive(true);
            else gameStartUI.SetActive(true);
        }
    }
}
