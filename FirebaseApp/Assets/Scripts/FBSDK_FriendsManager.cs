using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static partial class FBSDK
{
    public static class FriendsManager
    {
        public static async Task<bool> AcceptWaitingRequestAsyncTask(string playerUniqueID)
        {
            try
            {
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var friendRequestResult = mySnapshot.TryGetValue(UserData.FIELD_FRIEND_REQUEST, out List<object> friendRequest);
                friendRequest.Remove(playerUniqueID);

                var friendListResult = mySnapshot.TryGetValue(UserData.FIELD_FRIEND_LIST, out List<object> friendList);
                if (friendList == null)
                {
                    friendList = new List<object>();
                }
                friendList.Add(playerUniqueID);


                Data.Clear();
                Data.Add(UserData.FIELD_FRIEND_REQUEST, friendRequest);
                Data.Add(UserData.FIELD_FRIEND_LIST, friendList);

                await MyDocuments.UpdateAsync(Data);

                return true;
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
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var array = mySnapshot.GetValue<List<object>>(UserData.FIELD_FRIEND_REQUEST);
                array.Remove(playerUniqueID);
                await MyDocuments.UpdateAsync(UserData.FIELD_FRIEND_REQUEST, array);

                return true;
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
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var friendListResult = mySnapshot.TryGetValue(UserData.FIELD_FRIEND_LIST, out List<object> friendList);
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
                var mySnapshot = await MyDocuments.GetSnapshotAsync();

                var friendListResult = mySnapshot.TryGetValue(UserData.FIELD_FRIEND_LIST, out List<object> friendList);
                if (friendListResult == false || friendList == null || friendList.Count == 0)
                {
                    return false;
                }

                if (friendList.Exists(x => x.ToString() == playerUniqueID))
                {
                    friendList.Remove(playerUniqueID);
                    await MyDocuments.UpdateAsync(UserData.FIELD_FRIEND_LIST, friendList);
                    return true;
                }               

                return false;
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

        public static async Task<bool> SendRequestFriendAsyncTask(string userUniqueID)
        {
            try
            {
                var snapshot = await GetDocuments(userUniqueID).GetSnapshotAsync();
                var waitingRequestIDs = snapshot.GetValue<List<object>>(UserData.FIELD_FRIEND_REQUEST);
                if (waitingRequestIDs.Contains(userUniqueID))
                {
                    return true;
                }

                waitingRequestIDs.Add(userUniqueID);
                await GetDocuments(userUniqueID).UpdateAsync(UserData.FIELD_FRIEND_REQUEST, waitingRequestIDs);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
