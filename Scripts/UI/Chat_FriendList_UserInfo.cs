using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Chat_FriendList_UserInfo : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI lastMessage;
    public TextMeshProUGUI date;

    public UITweenFormTransition toChat;
    public UITweenHorizontalTransition transition;
    FBSDK.ChatManager.Metadata metadata;

    public void SetValue(FBSDK.ChatManager.Metadata metadata)
    {
        this.metadata = metadata;
        userName.text = metadata.Username;
        lastMessage.text = metadata.Date;
        date.text = metadata.Date;
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_ToChat()
    {
        ToChat();
    }

    private void ToChat()
    {
        toChat.Execute();
    }
}
