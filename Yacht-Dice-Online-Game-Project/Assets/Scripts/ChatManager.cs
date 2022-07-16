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
     * ChatManager 스크립트
     *
     * Chatting System 감독
     * Emoticon System 감독
     * Button UI 애니메이션 감독
     * Game UI 감독
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

        // 이모티콘 변수 초기화
        emoticonContainer.SetActive(false);
        isEmoticonContainer = false;
        for (int i = 0; i < emoticonChatList.Count; i++) if (emoticonChatList[i]) emoticonChatList[i].SetActive(false);
    }

    #region  채팅

    // 채팅 변수 초기화
    public void ResetChat()
    {
        chatUI.SetActive(true);
        isChat = false;
    }

    // 채팅 초기화
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


    // 채팅 UI
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

    // 채팅 보내기
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + chatInput.text);
        chatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    public void ChatRPC(string msg)
    {
        bool isInput = false;

        // 비어있는 채팅 오브젝트를 찾고 msg 삽입
        for (int i = 0; i < chatText.Length; i++)
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }

        // 꽉차면 한칸씩 위로 올림
        if (!isInput)
        {
            for (int i = 1; i < chatText.Length; i++) chatText[i - 1].text = chatText[i].text;
            chatText[chatText.Length - 1].text = msg;
        }
    }

    #endregion

    #region 이모티콘

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

    // 이모티콘을 누르면 해당 플레이어의 이모챗 활성화
    public void OnClcikEmoticonChatButton(GameObject emoticon)
    {
        // 보낼 이모티콘 정보 찾기
        string emoticonMsg = emoticon.transform.GetChild(0).GetComponent<Text>().text;

        // 플레이어의 인덱스 구하기
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
        emoticonChatList[index].transform.GetChild(1).GetComponent<Text>().text = emoticonMsg + "ㄱ";

        yield return new WaitForSeconds(2f);

        emoticonChatList[index].SetActive(false);
    }

    #endregion
}
