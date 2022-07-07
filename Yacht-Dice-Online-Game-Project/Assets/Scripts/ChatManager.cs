using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;

public class ChatManager : MonoBehaviourPunCallbacks
{

    /*
     * ChatManager ��ũ��Ʈ
     *
     * Chatting System ����
     * Emoticon System ����
     * 
    */

    [Header("Chat")]
    [SerializeField]
    private GameObject chatUI;
    [SerializeField]
    private GameObject chatTextUI;
    private bool isChat = false;
    [SerializeField]
    private Text[] chatText;
    [SerializeField]
    private InputField chatInput;

    [Header("Emoticon")]
    [SerializeField]
    private GameObject emoticonContainer;
    private bool isEmoticonContainer;

    public PhotonView PV;

    public static ChatManager CM;

    private void Awake()
    {
        CM = this;

        PV = photonView;

        isChat = false;

        chatUI.SetActive(false);

        // �̸�Ƽ�� ���� �ʱ�ȭ
        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;
    }

    #region  ä��

    // ä�� ���� �ʱ�ȭ
    public void ResetChat()
    {
        chatUI.SetActive(true);
        isChat = false;
    }

    // ä�� �ʱ�ȭ
    public void ResetChatText()
    {
        chatInput.text = "";
        for (int i = 0; i < chatText.Length; i++)
        {
            chatText[i].text = "";
        }
    }


    // ä�� UI
    public void OnClckChatButton()
    {
  
        if (NM.GetActiveRoomPanel())
        {
            NM.SetStartExitButton(!isChat);
        }
        else
        {
        }

        chatTextUI.SetActive(isChat);
        chatTextUI.transform.localPosition = (isChat) ? new Vector3(-120, 12f, 0f) : new Vector3(-950f, 12f, 0f);

        isChat = !isChat;
    }

    // ä�� ������
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    public void ChatRPC(string msg)
    {
        bool isInput = false;

        // ����ִ� ä�� ������Ʈ�� ã�� msg ����
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }

        // ������ ��ĭ�� ���� �ø�
        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

    #endregion

    #region �̸�Ƽ��

    public void OnClickEmoticonButton()
    {
        if (!isEmoticonContainer)
        {
            emoticonContainer.SetActive(true);
            emoticonContainer.GetComponent<Animator>().SetTrigger("on");
        }
        else
        {
            emoticonContainer.GetComponent<Animator>().SetTrigger("off");
            /*
            if(!emoticonContainer.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Anim_EmoticonsUnscroll"))
            {
                emoticonContainer.SetActive(false);
            }
            */
        }

        isEmoticonContainer = !isEmoticonContainer;
    }

    #endregion
}