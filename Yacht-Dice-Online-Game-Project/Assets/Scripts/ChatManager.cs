using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using static NetworkManager;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject chatUI;
    [SerializeField]
    private GameObject chatTextUI;
    private bool isChat = false;
    [SerializeField]
    private Text[] chatText;
    [SerializeField]
    private InputField chatInput;

    public PhotonView PV;

    public static ChatManager CM;

    private void Awake()
    {
        CM = this;

        PV = photonView;

        isChat = false;

        chatUI.SetActive(false);
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
}
