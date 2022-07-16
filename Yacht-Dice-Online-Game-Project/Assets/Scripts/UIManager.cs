using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{
    /*
     * UIManager 스크립트
     *
     * Button UI 애니메이션 감독
     * Game UI 감독
     * 
    */


    [Header("UI")]
    [SerializeField]
    private GameObject loadingUI;
    [SerializeField]
    private GameObject noticeUI;

    // Start is called before the first frame update
    private void Start()
    {
        loadingUI.SetActive(false);
        noticeUI.SetActive(false);
    }

    #region OnClickButton

    public void OnClickButton(GameObject button)
    {
        button.GetComponent<Animator>().SetTrigger("click");
    }

    public void Open(GameObject openUI)
    {
        StartCoroutine(OpenCoroutine(openUI));
    }

    IEnumerator OpenCoroutine(GameObject openUI)
    {
        yield return new WaitForSeconds(0.25f);

        openUI.SetActive(true);
    }

    public void Close(GameObject closeUI)
    {
        StartCoroutine(CloseCoroutine(closeUI));
    }

    IEnumerator CloseCoroutine(GameObject closeUI)
    {
        yield return new WaitForSeconds(0.25f);

        closeUI.SetActive(false);
    }

    #endregion

    #region GameUI

    // 로딩 바 켜기/끄기
    public void SetLoadingUI(bool isActvie)
    {
        loadingUI.SetActive(isActvie);
    }

    // 로딩 화 활성화 판별 변수
    public bool GetLoadingUIAcive() { return loadingUI.activeSelf; }


    // Notice UI 관리

    public void SetNoticeUI(string noticeMsg)
    {
        noticeUI.SetActive(true);
        noticeUI.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = noticeMsg;
    }

    #endregion

}
