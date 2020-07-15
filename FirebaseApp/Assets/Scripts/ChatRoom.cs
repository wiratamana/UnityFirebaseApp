using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Firebase.Firestore;
using Tamana;

/// <summary>
/// チャット部屋
/// </summary>
public class ChatRoom
{
    public string ChatRoomID { get; }
    public string UserUniqueID { get; }
    public string Username { get; }
    public string LastMessage { get; private set; }
    public string DateUTC { get; private set; }
    public ReadOnlyCollection<ChatObject> ChatObjects => chatObjects.AsReadOnly();

    /// <summary>
    /// メッセージが送られたときからの経過時間。
    /// </summary>
    public string ElapsedTimeSinceMessageSentString
    {
        get
        {
            if (chatObjects.Count == 0)
            {
                return null;
            }

            return chatObjects[chatObjects.Count - 1].ElapsedTimeSinceMessageSentString;
        }
    }

    private readonly List<ChatObject> chatObjects = new List<ChatObject>();
    private readonly ListenerRegistration listenerRegistration;

    /// <summary>
    /// 新たなメッセージが受け取ったときのイベント
    /// </summary>
    public event Action<ReadOnlyCollection<ChatObject>> NewMessageReceived;

    /// <summary>
    /// チャット部屋を生成する。
    /// </summary>
    /// <param name="userUniqueID">相手のユーザーID</param>
    /// <param name="snapshot">Firestore のドキュメントスナップショット</param>
    /// <param name="username">相手のユーザー名</param>
    public ChatRoom(string userUniqueID, DocumentSnapshot snapshot, string username)
    {
        ChatRoomID = snapshot.Reference.Id;
        UserUniqueID = userUniqueID;
        Username = username;

        var dics = snapshot.ToDictionary();
        var chatObjectsRaw = snapshot.GetValue<List<Dictionary<string, string>>>(FBSDK.FIELD_CHAT_OBJECTS);
        if (chatObjectsRaw != null)
        {
            foreach (var item in chatObjectsRaw)
            {
                chatObjects.Add(new ChatObject(item as Dictionary<string, string>));
            }
        }

        listenerRegistration = snapshot.Reference.Listen(OnNewMessageReceived);
    }

    /// <summary>
    /// Firestore のドキュメントのリスナーを停止する。
    /// </summary>
    public void StopListener()
    {
        listenerRegistration.Stop();
    }

    /// <summary>
    /// 新たなメッセージが受け取ったときのコールバック。
    /// </summary>
    /// <param name="snapshot">Firestore からの更新されたスナップショット</param>
    private void OnNewMessageReceived(DocumentSnapshot snapshot)
    {
        var chatObjectsRaw = snapshot.GetValue<List<Dictionary<string, string>>>(FBSDK.FIELD_CHAT_OBJECTS);
        if (chatObjectsRaw == null || chatObjectsRaw.Count == 0)
        {
            return;
        }

        var newReceivedChatObject = new List<ChatObject>();
        var userUniqueID = UserData.UserUniqueID;
        foreach (var item in chatObjectsRaw)
        {
            // 私のメッセージは無視する。
            // ----------------------
            if (item.GetValueIfExist(nameof(ChatObject.UserUniqueID)) == userUniqueID)
            {
                continue;
            }

            var chatObject = new ChatObject(item);

            if (chatObjects.Count == 0)
            {
                newReceivedChatObject.Add(chatObject);
                continue;
            }

            else if (chatObjects.Contains(chatObject) || newReceivedChatObject.Contains(chatObject))
            {
                continue;
            }

            newReceivedChatObject.Add(chatObject);
        }

        chatObjects.AddRange(newReceivedChatObject);

        LastMessage = chatObjects.Count > 0 ? chatObjects[chatObjects.Count - 1].Message : null;
        DateUTC = chatObjects.Count > 0 ? chatObjects[chatObjects.Count - 1].ElapsedTimeSinceMessageSentString : null;

        NewMessageReceived?.Invoke(newReceivedChatObject.AsReadOnly());
    }

    public override string ToString()
    {
        return $"ChatRoomID : {ChatRoomID} | UserUniqueID : {UserUniqueID} | Username : {Username} | LastMessage : {LastMessage} | Date : {DateUTC} | Message.Count : {chatObjects.Count}";
    }
}