using System.Collections.ObjectModel;
using UnityEngine;

using TMPro;

/// <summary>
/// チャット画面のフレンド情報
/// </summary>
public class Chat_FriendList_UserInfo : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI lastMessage;
    public TextMeshProUGUI date;

    public UITweenFormTransition toChat;
    public UITweenHorizontalTransition transition;

    private ChatRoom metadata;

    /// <summary>
    /// 値を代入する。
    /// </summary>
    /// <param name="metadata">チャット部屋</param>
    public void SetValue(ChatRoom metadata)
    {
        this.metadata = metadata;
        userName.text = metadata?.Username;
        lastMessage.text = metadata?.LastMessage;
        date.text = metadata?.ElapsedTimeSinceMessageSentString;

        metadata.NewMessageReceived += UpdateLatestMessage;
    }

    /// <summary>
    /// チャットを送る画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_ToChat()
    {
        ToChat();
    }

    /// <summary>
    /// チャットを送る画面に遷移する。
    /// </summary>
    private void ToChat()
    {
        Chat_Messaging.Instance.SetValue(metadata);
        toChat.Execute();
    }

    /// <summary>
    /// 最後に送信したメッセージ内容を更新する。
    /// </summary>
    /// <param name="chatObjects">チャット情報</param>
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
