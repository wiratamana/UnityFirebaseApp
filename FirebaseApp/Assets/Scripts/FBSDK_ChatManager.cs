using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static partial class FBSDK
{
    public static partial class ChatManager
    {
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
#if FIRESTORE_TRANSACTION
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
#else
            try
            {                
                var chatRoomID = string.Empty;
                var exist = true;
                while (exist)
                {
                    chatRoomID = Tamana.Utils.GetRandomHexNumber(16).Insert(4, "-").Insert(9, "-").Insert(14, "-");
                    var room = await Chats.Document(chatRoomID).GetSnapshotAsync();
                    exist = room.Exists;
                }

                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();

                List<Dictionary<string, object>> getData(DocumentSnapshot snapshot, string uniqueID)
                {
                    var chatIDs = snapshot.GetValue<object>(UserData.FIELD_FRIEND_CHAT_ROOM_IDS) as List<Dictionary<string, object>>;
                    if (chatIDs == null)
                    {
                        chatIDs = new List<Dictionary<string, object>>();
                    }

                    var newData = new Dictionary<string, object>()
                    {
                        { nameof(UserData.UserUniqueID), uniqueID },
                        { UserData.FIELD_CHAT_ROOM_ID, chatRoomID }
                    };
                    chatIDs.Add(newData);

                    return chatIDs;
                }

                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Set(Chats.Document(chatRoomID), new Dictionary<string, object> { { UserData.FIELD_CHAT_OBJECTS, new List<object>() } });
                batch.Update(GetDocuments(playerUniqueID), UserData.FIELD_FRIEND_CHAT_ROOM_IDS, getData(friendSnapshot, userUniqueID));
                batch.Update(GetDocuments(userUniqueID), UserData.FIELD_FRIEND_CHAT_ROOM_IDS, getData(mySnapshot, playerUniqueID));

                await batch.CommitAsync();

                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
#endif

        }

        public static async Task<ChatRoom> GetChatMetadataAsyncTask(UserInfoData userInfoData)
        {
#if FIRESTORE_TRANSACTION
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
                        chatObjects.Add(new ChatObject(message, userUniqueID));
                    }

                    return new Metadata(chatRoomID, userInfoData.UserUniqueID, userInfoData.Username, chatObjects);
                }
                catch
                {
                    return Metadata.Null;
                }
            });
#else
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
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
                    return null;
                }

                var chatSnapshot = await Chats.Document(chatRoomID).GetSnapshotAsync();
                return new ChatRoom(userInfoData.UserUniqueID, chatSnapshot, userInfoData.Username);
            }
            catch (System.Exception e)
            {
                return null;
            }
#endif
        }

        public static async Task<Dictionary<string, string>> SendMessageAsyncTask(ChatObject chatObject, string chatRoomID)
        {
            try
            {
                var data = new Dictionary<string, object>
                {
                    { nameof(ChatObject.UserUniqueID), chatObject.UserUniqueID },
                    { nameof(ChatObject.Message), chatObject.Message },
                    { nameof(ChatRoom.ChatRoomID), chatRoomID }
                };

                return await FBFunctions.SendMessageAsyncTask(data);
            }
            catch
            {
                return null;
            }

        }
    }
}



