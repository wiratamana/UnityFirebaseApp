using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

using TMPro;

public class Chat_FriendList_UserInfo : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI lastMessage;
    public TextMeshProUGUI date;

    public UITweenFormTransition toChat;
    public UITweenHorizontalTransition transition;

    private ChatRoom metadata;

    public void SetValue(ChatRoom metadata)
    {
        this.metadata = metadata;
        userName.text = metadata?.Username;
        lastMessage.text = metadata?.LastMessage;
        date.text = metadata?.ElapsedTimeSinceMessageSentString;

        metadata.NewMessageReceived += UpdateLatestMessage;
    }

    [Utils.InvokeByUnity]
    public void OnClick_ToChat()
    {
        ToChat();
    }

    private void ToChat()
    {
        Chat_Messaging.Instance.SetValue(metadata);
        toChat.Execute();
    }

    private void UpdateLatestMessage(ReadOnlyCollection<ChatObject> chatObjects)
    {
        if (chatObjects.Count == 0)
        {
            return;
        }

        lastMessage.text = chatObjects[chatObjects.Count - 1].Message;
        date.text = chatObjects[chatObjects.Count - 1].ElapsedTimeSinceMessageSentString;
    }

    private void OnDestroy()
    {
        if (metadata != null)
        {
            metadata.NewMessageReceived -= UpdateLatestMessage;
        }
        
        Destroy(gameObject);
    }
}
