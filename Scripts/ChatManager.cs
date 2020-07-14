using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tamana;


public static partial class FBSDK
{
    public static class ChatManager
    {
        public readonly struct Metadata
        {
            public readonly string ChatRoomID;
            public readonly string UserUniqueID;
            public readonly string Username;
            public readonly string LastMessage;
            public readonly string Date;

            private readonly List<ChatObject> chatObjects;
            public ReadOnlyCollection<ChatObject> ChatObjects => chatObjects.AsReadOnly();

            public Metadata(string chatRoomID, string userUniqueID, string name, IEnumerable<ChatObject> chatObjects)
            {
                UserUniqueID = userUniqueID;
                Username = name;
                ChatRoomID = chatRoomID;

                this.chatObjects = new List<ChatObject>();
                if (chatObjects != null)
                {
                    this.chatObjects.AddRange(chatObjects);
                }

                LastMessage = this.chatObjects.Count > 0 ? this.chatObjects[this.chatObjects.Count - 1].Message : null;
                Date = this.chatObjects.Count > 0 ? this.chatObjects[this.chatObjects.Count - 1].Date : null;
            }

            public bool IsNull => string.IsNullOrEmpty(UserUniqueID);
            public static Metadata Null => new Metadata(null, null, null, null);

            public override string ToString()
            {
                if (IsNull)
                {
                    return "null";
                }
                return $"ChatRoomID : {ChatRoomID} | UserUniqueID : {UserUniqueID} | Username : {Username} | LastMessage : {LastMessage} | Date : {Date} | Message.Count : {chatObjects.Count}";
            }
        }

        public readonly struct ChatObject
        {
            public readonly string UserUniqueID;
            public readonly string Message;
            public readonly string Date;

            public ChatObject(string message, string userUniqueID, string date)
            {
                Message = message;
                UserUniqueID = userUniqueID;
                Date = date;
            }

            public override string ToString()
            {
                return $"UserUniqueID : {UserUniqueID} | Message : {Message} | Date : {Date}";
            }
        }

        private static CollectionReference Chats => FirebaseFirestore.DefaultInstance.Collection("chats");

        public static async Task<string> GetChatRoomIDsAsyncTask(string userUniqueID)
        {
            var mySnapshot = await MyDocuments.GetSnapshotAsync();
            var chatRoomIDs = mySnapshot.GetValue<List<Dictionary<string, string>>>(UserData.FIELD_FRIEND_CHAT_ROOM_IDS);
            foreach (var item in chatRoomIDs)
            {
                if (item.ContainsKey(userUniqueID))
                {
                    return item[userUniqueID];
                }
            }

            return null;
        }

        public static async Task<bool> AddNewChatRoomAsyncTask(string userUniqueID, string playerUniqueID)
        {
            var db = FirebaseFirestore.DefaultInstance;

            return await db.RunTransactionAsync(async transaction =>
            {
                try
                {
                    var mySnapshot = await transaction.GetSnapshotAsync(GetDocuments(userUniqueID));
                    var friendSnapshot = await transaction.GetSnapshotAsync(GetDocuments(playerUniqueID));

                    var myChatIDs = mySnapshot.GetValue<List<Dictionary<string, object>>>(UserData.FIELD_FRIEND_CHAT_ROOM_IDS);
                    var friendChatIDs = friendSnapshot.GetValue<List<Dictionary<string, object>>>(UserData.FIELD_FRIEND_CHAT_ROOM_IDS);

                    var chatRoomID = Tamana.Utils.GetRandomHexNumber(16).Insert(4, "-").Insert(9, "-").Insert(14, "-");
                    var chatRoom = await Chats.Document(chatRoomID).GetSnapshotAsync();
                    while (chatRoom.Exists)
                    {
                        chatRoomID = Tamana.Utils.GetRandomHexNumber(16).Insert(4, "-").Insert(9, "-").Insert(14, "-");
                        chatRoom = await Chats.Document(chatRoomID).GetSnapshotAsync();
                    }

                    lock (Data)
                    {
                        Data.Clear();
                        Data.Add(UserData.FIELD_CHAT_OBJECTS, null);
                    }
                    await Chats.Document(chatRoomID).SetAsync(Data);

                    lock (Data)
                    {
                        Data.Clear();
                        Data.Add(nameof(UserData.UserUniqueID), userUniqueID);
                        Data.Add(UserData.FIELD_CHAT_ROOM_ID, chatRoomID);
                    }

                    friendChatIDs.Add(Data);
                    transaction.Update(GetDocuments(playerUniqueID), UserData.FIELD_FRIEND_CHAT_ROOM_IDS, friendChatIDs);

                    lock (Data)
                    {
                        Data.Clear();
                        Data.Add(nameof(UserData.UserUniqueID), playerUniqueID);
                        Data.Add(UserData.FIELD_CHAT_ROOM_ID, chatRoomID);
                    }

                    myChatIDs.Add(Data);
                    transaction.Update(GetDocuments(userUniqueID), UserData.FIELD_FRIEND_CHAT_ROOM_IDS, myChatIDs);


                    return true;
                }
                catch
                {
                    return false;
                }               
            });
        }

        public static async Task<Metadata> GetChatMetadataAsyncTask(UserInfoData userInfoData)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var myDoc = MyDocuments;

            return await db.RunTransactionAsync(async transaction =>
            {
                try
                {
                    var mySnapshot = await transaction.GetSnapshotAsync(myDoc);
                    var chatRoomIDs = mySnapshot.GetValue<List<Dictionary<string, string>>>(UserData.FIELD_FRIEND_CHAT_ROOM_IDS);

                    var chatRoomID = string.Empty;
                    foreach (var item in chatRoomIDs)
                    {
                        if (item[nameof(UserData.UserUniqueID)] == userInfoData.UserUniqueID)
                        {
                            chatRoomID = item[UserData.FIELD_CHAT_ROOM_ID];
                        }
                    }

                    if (string.IsNullOrEmpty(chatRoomID))
                    {
                        return Metadata.Null;
                    }

                    var chatRoomSnapshot = await transaction.GetSnapshotAsync(Chats.Document(chatRoomID));
                    var chatObjectsRaw = chatRoomSnapshot.GetValue<List<Dictionary<string, string>>>(UserData.FIELD_CHAT_OBJECTS);
                    if (chatObjectsRaw == null || chatObjectsRaw.Count == 0)
                    {
                        return new Metadata(chatRoomID, userInfoData.UserUniqueID, userInfoData.Username, null);
                    }

                    var chatObjects = new List<ChatObject>();
                    foreach (var item in chatObjectsRaw)
                    {
                        var message = item[nameof(ChatObject.Message)];
                        var userUniqueID = item[nameof(ChatObject.UserUniqueID)];
                        var date = item[nameof(ChatObject.Date)];
                        chatObjects.Add(new ChatObject(message, userUniqueID, date));
                    }

                    return new Metadata(chatRoomID, userInfoData.UserUniqueID, userInfoData.Username, chatObjects);
                }
                catch
                {
                    return Metadata.Null;
                }
            });
        }
    }
}


