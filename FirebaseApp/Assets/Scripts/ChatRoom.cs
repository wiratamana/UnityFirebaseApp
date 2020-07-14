using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Tamana;


public class ChatRoom
{
    public string ChatRoomID { get; }
    public string UserUniqueID { get; }
    public string Username { get; }
    public string LastMessage { get; private set; }
    public string DateUTC { get; private set; }
    public ReadOnlyCollection<ChatObject> ChatObjects => chatObjects.AsReadOnly();

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

    public event Action<ReadOnlyCollection<ChatObject>> NewMessageReceived;

    public ChatRoom(string userUniqueID, DocumentSnapshot snapshot, string name)
    {
        ChatRoomID = snapshot.Reference.Id;
        UserUniqueID = userUniqueID;
        Username = name;

        var dics = snapshot.ToDictionary();
        var chatObjectsRaw = snapshot.GetValue<List<Dictionary<string, string>>>(UserData.FIELD_CHAT_OBJECTS);
        if (chatObjectsRaw != null)
        {
            foreach (var item in chatObjectsRaw)
            {
                chatObjects.Add(new ChatObject(item as Dictionary<string, string>));
            }
        }

        listenerRegistration = snapshot.Reference.Listen(OnNewMessageReceived);
    }

    public void StopListener()
    {
        listenerRegistration.Stop();
    }

    private void OnNewMessageReceived(DocumentSnapshot snapshot)
    {
        var chatObjectsRaw = snapshot.GetValue<List<Dictionary<string, string>>>(UserData.FIELD_CHAT_OBJECTS);
        if (chatObjectsRaw == null || chatObjectsRaw.Count == 0)
        {
            return;
        }

        var newReceivedChatObject = new List<ChatObject>();
        var userUniqueID = UserData.UserUniqueID;
        foreach (var item in chatObjectsRaw)
        {
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