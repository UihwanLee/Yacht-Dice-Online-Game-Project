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
        // Title Panel 호출 시 Title 띄우기
        titleAnimator.SetBool("isAppear", true);

        if (mainCamera != null)
        {
            Debug.Log("Camera 리셋 완료 " + mainCameraPosition + " / " + mainCameraRotation);
            mainCamera.transform.position = mainCameraPosition;
            mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
        }
    }

    private void Update()
    {
        // 15초 간격으로 다이스 소환
        timer -= Time.deltaTime;
        if(timer <= 0f)
        {
            fallingDice.SpawnYachtDices(12f);
            timer = 15f;
        }

    }

    // 게임 시작 시 초기화
    public void Init()
    {
        // 메인 메뉴 화면 UI 초기화
        touchScreenUI.SetActive(true);
        gameStartUI.SetActive(false);
        severEnterUI.SetActive(false);

        // 카메라 위치 세팅
        mainCameraPosition = mainCamera.transform.position;// new Vector3(2.52f, 16.1f, -3.5f);
        mainCameraRotation = mainCamera.transform.rotation.eulerAngles; // new Vector3(69.19f, 0f, 0f);
    }
}
