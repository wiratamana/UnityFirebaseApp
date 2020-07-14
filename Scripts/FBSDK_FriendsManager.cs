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
    public static class FriendsManager
    {
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
        /// transaction, myMetadata, friendMetadata (can null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="playerUniqueID"></param>
        /// <param name="cb"></param>
        /// <returns></returns>
        private static async Task<T> RunFriendTransactionAsyncTask<T>(string playerUniqueID, Func<Transaction, Metadata, Metadata, T> cb)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var userUniqueID = UserData.UserUniqueID;
            DocumentReference myDoc = MyDocuments;
            DocumentReference friendDoc = string.IsNullOrEmpty(playerUniqueID) ? null : GetDocuments(playerUniqueID);

            return await db.RunTransactionAsync(async transaction =>
            {
                try
                {
                    DocumentSnapshot mySnapshot = await transaction.GetSnapshotAsync(myDoc);
                    DocumentSnapshot friendSnapshot = friendDoc == null ? null : await transaction.GetSnapshotAsync(friendDoc);

                    List<string> myselfReq = mySnapshot.GetValue<List<string>>(UserData.FIELD_FRIEND_REQUEST);
                    List<string> myselfList = mySnapshot.GetValue<List<string>>(UserData.FIELD_FRIEND_LIST);

                    List<string> friendList = friendSnapshot?.GetValue<List<string>>(UserData.FIELD_FRIEND_LIST);
                    List<string> friendReq = friendSnapshot?.GetValue<List<string>>(UserData.FIELD_FRIEND_REQUEST);

                    var myMetadata = new Metadata(myDoc, userUniqueID, myselfList, myselfReq);
                    var friendMetadata = friendDoc == null ? Metadata.Null : new Metadata(friendDoc, playerUniqueID, friendList, friendReq);

                    return cb == null ? default : cb.Invoke(transaction, myMetadata, friendMetadata);
                }
                catch
                {
                    return default;
                }                
            });
        }


        public static async Task<bool> AcceptWaitingRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                return await RunFriendTransactionAsyncTask(playerUniqueID, (transaction, myMetadata, friendMetadata) =>
                {
                    // me : remove req add list
                    myMetadata.friendRequest.RemoveIfContains(playerUniqueID);
                    myMetadata.friendList.AddIfNotContains(playerUniqueID);

                    lock (Data)
                    {
                        Data.Clear();
                        Data.Add(UserData.FIELD_FRIEND_REQUEST, myMetadata.friendRequest);
                        Data.Add(UserData.FIELD_FRIEND_LIST, myMetadata.friendList);
                    }

                    transaction.Update(myMetadata.documentReference, Data);

                    // friend : add list only 
                    friendMetadata.friendList.AddIfNotContains(myMetadata.uniqueID);

                    lock (Data)
                    {
                        Data.Clear();
                        Data.Add(UserData.FIELD_FRIEND_LIST, friendMetadata.friendList);
                    }

                    transaction.Update(friendMetadata.documentReference, Data);

                    return true;
                });
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> RemoveWaitingRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                var db = FirebaseFirestore.DefaultInstance;
                var myDoc = MyDocuments;

                return await db.RunTransactionAsync(async transaction => {

                    var mySnapshot = await transaction.GetSnapshotAsync(myDoc);
                    var array = mySnapshot.GetValue<List<string>>(UserData.FIELD_FRIEND_REQUEST);
                    array.RemoveIfContains(playerUniqueID);
                    await MyDocuments.UpdateAsync(UserData.FIELD_FRIEND_REQUEST, array);
                    return true;

                });
            }
            catch
            {
                return false;
            }
        }


        public static async Task<ReadOnlyCollection<UserInfoData>> GetFriendListAsyncTask()
        {
            try
            {
                var db = FirebaseFirestore.DefaultInstance;
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var friendListResult = mySnapshot.TryGetValue(UserData.FIELD_FRIEND_LIST, out List<string> friendList);
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

        public static async Task<bool> RemoveFriendAysncTask(string playerUniqueID)
        {
            try
            {
                return await RunFriendTransactionAsyncTask(playerUniqueID, (transaction, myMetadata, friendMetadata) => 
                {
                    myMetadata.friendList.RemoveIfContains(friendMetadata.uniqueID);
                    transaction.Update(myMetadata.documentReference, UserData.FIELD_FRIEND_LIST, myMetadata.friendList);

                    friendMetadata.friendList.RemoveIfContains(myMetadata.uniqueID);
                    transaction.Update(friendMetadata.documentReference, UserData.FIELD_FRIEND_LIST, friendMetadata.friendList);

                    return true;
                });
            }
            catch
            {
                return false;
            }
        }

        public static async Task<ReadOnlyCollection<UserInfoData>> GetWaitingRequestAsyncTask()
        {
            try
            {
                var snapshot = await MyDocuments.GetSnapshotAsync();

                var waitingRequestIDs = snapshot.GetValue<List<object>>(UserData.FIELD_FRIEND_REQUEST);
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

        public static async Task<bool> SendFriendRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                return await RunFriendTransactionAsyncTask(playerUniqueID, (transaction, myMetadata, friendMetadata) =>
                {
                    friendMetadata.friendRequest.AddIfNotContains(myMetadata.uniqueID);
                    transaction.Update(friendMetadata.documentReference, UserData.FIELD_FRIEND_REQUEST, friendMetadata.friendRequest);

                    return true;
                });
            }
            catch
            {
                return false;
            }
        }
    }
}
