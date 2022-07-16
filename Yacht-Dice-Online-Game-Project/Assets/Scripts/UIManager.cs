using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class UIManager : MonoBehaviourPunCallbacks
{
    /*
     * UIManager ��ũ��Ʈ
     *
     * Button UI �ִϸ��̼� ����
     * Game UI ����
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

    // �ε� �� �ѱ�/����
    public void SetLoadingUI(bool isActvie)
    {
        loadingUI.SetActive(isActvie);
    }

    // �ε� ȭ Ȱ��ȭ �Ǻ� ����
    public bool GetLoadingUIAcive() { return loadingUI.activeSelf; }


    // Notice UI ����

    public void SetNoticeUI(string noticeMsg)
    {
        noticeUI.SetActive(true);
        noticeUI.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = noticeMsg;
    }

    #endregion

}
