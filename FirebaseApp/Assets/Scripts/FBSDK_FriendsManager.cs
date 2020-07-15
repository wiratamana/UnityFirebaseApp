using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Firebase.Firestore;
using Tamana;

/// <summary>
/// Firestore にかかわるメソッドの集まり。
/// </summary>
public static partial class FBSDK
{
    /// <summary>
    /// Firestore の フレンド管理にかかわるメソッドの集まり。
    /// </summary>
    public static class FriendsManager
    {
        /// <summary>
        /// Firestore から取得したデータです。
        /// </summary>
        private readonly struct Metadata
        {
            public readonly DocumentReference documentReference;
            public readonly string uniqueID;
            public readonly List<string> friendList;
            public readonly List<string> friendRequest;

            public Metadata(DocumentReference documentReference, string uniqueID, List<string> friendList, List<string> friendRequest)
            {
                this.documentReference = documentReference;
                this.uniqueID = uniqueID;
                this.friendList = friendList;
                this.friendRequest = friendRequest;
            }

            public bool IsNull => documentReference == null;
            public static Metadata Null => new Metadata(null, null, null, null);
        }

        /// <summary>
        /// 承認待ちユーザーをフレンド登録します。
        /// </summary>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<bool> AcceptWaitingRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();

                var myRequest = mySnapshot.GetValue<List<string>>(FIELD_FRIEND_REQUEST);
                var myFriendList = mySnapshot.GetValue<List<string>>(FIELD_FRIEND_LIST);
                var friendFriendList = friendSnapshot.GetValue<List<string>>(FIELD_FRIEND_LIST);

                myRequest.RemoveIfContains(playerUniqueID);
                myFriendList.AddIfNotContains(playerUniqueID);
                friendFriendList.AddIfNotContains(UserData.UserUniqueID);

                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Update(MyDocuments, FIELD_FRIEND_REQUEST, myRequest);
                batch.Update(MyDocuments, FIELD_FRIEND_LIST, myFriendList);
                batch.Update(GetDocuments(playerUniqueID), FIELD_FRIEND_LIST, friendFriendList);
                await batch.CommitAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 承認待ちユーザーを削除する。
        /// </summary>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<bool> RemoveWaitingRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();

                var myRequest = mySnapshot.GetValue<List<string>>(FIELD_FRIEND_REQUEST);

                myRequest.RemoveIfContains(playerUniqueID);

                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Update(MyDocuments, FIELD_FRIEND_REQUEST, myRequest);
                await batch.CommitAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// フレンド情報を取得します。
        /// </summary>
        /// <returns>フレンド情報</returns>
        public static async Task<ReadOnlyCollection<UserInfoData>> GetFriendListAsyncTask()
        {
            try
            {
                var db = FirebaseFirestore.DefaultInstance;
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var friendListResult = mySnapshot.TryGetValue(FIELD_FRIEND_LIST, out List<string> friendList);
                if (friendListResult == false || friendList == null)
                {
                    return null;
                }

                List<Task<UserInfoData>> tasks = new List<Task<UserInfoData>>();
                foreach (var item in friendList)
                {
                    tasks.Add(GetUserInfoDataAsyncTask(item.ToString()));
                }

                var result = await Task.WhenAll(tasks);
                return new List<UserInfoData>(result).AsReadOnly();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// フレンドを削除する。
        /// </summary>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<bool> RemoveFriendAysncTask(string playerUniqueID)
        {
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();

                var myFriendList = mySnapshot.GetValue<List<string>>(FIELD_FRIEND_LIST);
                var friendFriendList = friendSnapshot.GetValue<List<string>>(FIELD_FRIEND_LIST);

                myFriendList.RemoveIfContains(playerUniqueID);
                friendFriendList.RemoveIfContains(UserData.UserUniqueID);

                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Update(MyDocuments, FIELD_FRIEND_LIST, myFriendList);
                batch.Update(GetDocuments(playerUniqueID), FIELD_FRIEND_LIST, friendFriendList);
                await batch.CommitAsync();

                return true;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 承認待ちユーザー情報を取得する。
        /// </summary>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<ReadOnlyCollection<UserInfoData>> GetWaitingRequestAsyncTask()
        {
            try
            {
                var snapshot = await MyDocuments.GetSnapshotAsync();

                var waitingRequestIDs = snapshot.GetValue<List<object>>(FIELD_FRIEND_REQUEST);
                var tasks = new List<Task<UserInfoData>>();
                foreach (var item in waitingRequestIDs)
                {
                    tasks.Add(GetUserInfoDataAsyncTask(item.ToString()));
                }

                var results = new List<UserInfoData>(await Task.WhenAll(tasks));
                return results.AsReadOnly();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 相手にフレンドを申請する。
        /// </summary>
        /// <param name="playerUniqueID">相手のユーザーID</param>
        /// <returns>処理が最後まで行けば true を返す。途中でエラーなどが発生した場合に、false を返す。</returns>
        public static async Task<bool> SendFriendRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                var friendSnapshot = await GetDocuments(playerUniqueID).GetSnapshotAsync();
                var friendFriendRequest = friendSnapshot.GetValue<List<string>>(FIELD_FRIEND_REQUEST);
                friendFriendRequest.AddIfNotContains(UserData.UserUniqueID);

                var batch = FirebaseFirestore.DefaultInstance.StartBatch();
                batch.Update(GetDocuments(playerUniqueID), FIELD_FRIEND_REQUEST, friendFriendRequest);
                await batch.CommitAsync();

                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
