using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase.Firestore;

public static partial class FBSDK
{
    public static class ChatManager
    {
        private static CollectionReference Chats => FirebaseFirestore.DefaultInstance.Collection("chats");

        /// <summary>
        /// チャット部屋の ID を取得
        /// </summary>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば チャット部屋の ID を返す。途中でエラーなどが発生した場合に、null を返す。</returns>
        public static async Task<string> GetChatRoomIDsAsyncTask(string playerUniqueID)
        {
            var mySnapshot = await MyDocuments.GetSnapshotAsync();
            var chatRoomIDs = mySnapshot.GetValue<List<Dictionary<string, string>>>(FIELD_FRIEND_CHAT_ROOM_IDS);
            foreach (var item in chatRoomIDs)
            {
                if (item.ContainsKey(playerUniqueID))
                {
                    return item[playerUniqueID];
                }
            }

            return null;
        }

        /// <summary>
        /// 新たにチャット部屋を作成します。
        /// </summary>
        /// <param name="userUniqueID">私ののユーザーID</param>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<bool> AddNewChatRoomAsyncTask(string userUniqueID, string playerUniqueID)
        {
            try
            {
                // チャット部屋の ID を生成する。
                // ---------------------------
                var chatRoomID = string.Empty;
                var exist = true;
                while (exist)
                {
                    chatRoomID = Tamana.Utils.GetRandomHexNumber(16).Insert(4, "-").Insert(9, "-").Insert(14, "-");
                    var room = await Chats.Document(chatRoomID).GetSnapshotAsync();
                    exist = room.Exists;
                }

                // 生成された チャット部屋の ID を私と相手に配布準備をする。
                // ---------------------------------------------------
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();

                List<Dictionary<string, object>> getData(DocumentSnapshot snapshot, string uniqueID)
                {
                    var chatIDs = snapshot.GetValue<object>(FIELD_FRIEND_CHAT_ROOM_IDS) as List<Dictionary<string, object>>;
                    if (chatIDs == null)
                    {
                        chatIDs = new List<Dictionary<string, object>>();
                    }

                    var newData = new Dictionary<string, object>()
                    {
                        { nameof(UserData.UserUniqueID), uniqueID },
                        { FIELD_CHAT_ROOM_ID, chatRoomID }
                    };
                    chatIDs.Add(newData);

                    return chatIDs;
                }

                // チャット部屋を作成することとチャット部屋の ID を私と相手に配布する。
                // -------------------------------------------------------------
                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Set(Chats.Document(chatRoomID), new Dictionary<string, object> { { FIELD_CHAT_OBJECTS, new List<object>() } });
                batch.Update(GetDocuments(playerUniqueID), FIELD_FRIEND_CHAT_ROOM_IDS, getData(friendSnapshot, userUniqueID));
                batch.Update(GetDocuments(userUniqueID), FIELD_FRIEND_CHAT_ROOM_IDS, getData(mySnapshot, playerUniqueID));

                await batch.CommitAsync();

                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 私とチャットフレンドとのチャット情報を取得する。
        /// </summary>
        /// <param name="userInfoData">相手のユーザー情報</param>
        /// <returns>処理が最後まで行けば <see cref="ChatRoom"/> を返す。途中でエラーなどが発生した場合に、<see cref="null"/> を返す。</returns>
        public static async Task<ChatRoom> GetChatMetadataAsyncTask(UserInfoData userInfoData)
        {
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var chatRoomIDs = mySnapshot.GetValue<List<Dictionary<string, string>>>(FIELD_FRIEND_CHAT_ROOM_IDS);

                var chatRoomID = string.Empty;
                foreach (var item in chatRoomIDs)
                {
                    if (item[nameof(UserData.UserUniqueID)] == userInfoData.UserUniqueID)
                    {
                        chatRoomID = item[FIELD_CHAT_ROOM_ID];
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
        }

        /// <summary>
        /// Cloud Function を通して、相手にメッセージを送る。
        /// </summary>
        /// <param name="chatObject">チャット情報</param>
        /// <param name="chatRoomID">チャット部屋 ID</param>
        /// <returns>ID 付きの <see cref="ChatObject"/> を返す</returns>
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