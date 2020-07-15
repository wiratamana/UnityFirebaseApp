using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Firebase.Firestore;

public static partial class FBSDK
{
    public const string FIELD_FRIEND_REQUEST = "FriendRequest";
    public const string FIELD_FRIEND_LIST = "FriendList";
    public const string FIELD_FRIEND_CHAT_ROOM_IDS = "FriendChatRoomIDs";
    public const string FIELD_CHAT_ROOM_ID = "ChatRoomID";
    public const string FIELD_CHAT_OBJECTS = "ChatObjects";

    private static CollectionReference Users
    {
        get
        {
            var db = FirebaseFirestore.DefaultInstance;
            return db.Collection("users");
        }
    }

    private static DocumentReference MyDocuments => Users.Document(UserData.UserUniqueID);
    private static DocumentReference GetDocuments(string userUniqueID) => Users.Document(userUniqueID);

    /// <summary>
    /// ユーザー情報を取得する。
    /// </summary>
    /// <param name="userUniqueID">相手の ID</param>
    /// <returns>処理が最後まで行けば <see cref="UserInfoData"/> を返す。途中でエラーなどが発生した場合に、<see cref="UserInfoData.Null"/> を返す。</returns>
    public static async Task<UserInfoData> GetUserInfoDataAsyncTask(string userUniqueID)
    {
        try
        {
            var snapshot = await GetDocuments(userUniqueID).GetSnapshotAsync();

            var username = snapshot.GetValue<string>(nameof(UserInfoData.Username));
            var gender = (Gender)Enum.Parse(typeof(Gender), snapshot.GetValue<string>(nameof(UserInfoData.Gender)));
            var birthday = DateTime.Parse(snapshot.GetValue<string>(nameof(UserInfoData.Birthday)));
            var score = snapshot.GetValue<long>(nameof(UserInfoData.Score));

            return new UserInfoData(userUniqueID, username, birthday, gender, score);
        }
        catch
        {
            return UserInfoData.Null;
        }
    }

    /// <summary>
    /// ユーザー情報を更新する。
    /// </summary>
    /// <param name="userUniqueID">相手の ID</param>
    /// <returns>処理が最後まで行けば <see cref="true"/> を返す。途中でエラーなどが発生した場合に、<see cref="false"/> を返す。</returns>
    public static async Task<bool> UpdateUserInfoDataAsyncTask(UserInfoData userInfoData)
    {
        try
        {
            var docRef = GetDocuments(userInfoData.UserUniqueID);

            var Data = new Dictionary<string, object>
            {
                { nameof(UserInfoData.Username), userInfoData.Username },
                { nameof(UserInfoData.Gender), userInfoData.Gender.ToString() },
                { nameof(UserInfoData.Birthday), userInfoData.Birthday.ToShortDateString() },
                { nameof(UserInfoData.Score), userInfoData.Score }
            };

            await docRef.UpdateAsync(Data);
            return true;
        }
        catch
        {
            return false;
        }       
    }
}
