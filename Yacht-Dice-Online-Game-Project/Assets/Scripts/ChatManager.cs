using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;
using static InGameNetWorkManager;

public class ChatManager : MonoBehaviourPunCallbacks
{

    /*
     * ChatManager ��ũ��Ʈ
     *
     * Chatting System ����
     * Emoticon System ����
     * Button UI �ִϸ��̼� ����
     * Game UI ����
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
    [SerializeField]
    private List<GameObject> emoticonChatList = new List<GameObject>();

    public PhotonView PV;

    public static ChatManager CM;

    private void Awake()
    {
        CM = this;

        PV = photonView;

        // Chat
        isChat = false;
        chatUI.SetActive(false);

        // �̸�Ƽ�� ���� �ʱ�ȭ
        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;
        for (int i = 0; i < emoticonChatList.Count; i++) if (emoticonChatList[i]) emoticonChatList[i].SetActive(false);
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

        chatUI.SetActive(false);
        isChat = false;
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
        }

        isEmoticonContainer = !isEmoticonContainer;
    }

    // �̸�Ƽ���� ������ �ش� �÷��̾��� �̸�ê Ȱ��ȭ
    public void OnClcikEmoticonChatButton(GameObject emoticon)
    {
        // ���� �̸�Ƽ�� ���� ã��
        string emoticonMsg = emoticon.transform.GetChild(0).GetComponent<Text>().text;

        // �÷��̾��� �ε��� ���ϱ�
        int index = IN.MyPlayer.GetPlayerSequence();

        PV.RPC("SendEmoticonchatRPC", RpcTarget.AllBuffered, emoticonMsg, index);

        emoticonContainer.GetComponent<Animator>().SetTrigger("off");
        isEmoticonContainer = !isEmoticonContainer;
    }

    [PunRPC]
    private void SendEmoticonchatRPC(string emoticonMsg, int index)
    {
        StartCoroutine(SendEmoticonCourutine(emoticonMsg, index));
    }

    IEnumerator SendEmoticonCourutine(string emoticonMsg, int index)
    {

        emoticonChatList[index].SetActive(true);
        emoticonChatList[index].transform.GetChild(1).GetComponent<Text>().text = emoticonMsg + "��";

        yield return new WaitForSeconds(2f);

        emoticonChatList[index].SetActive(false);
    }

    #endregion
}
