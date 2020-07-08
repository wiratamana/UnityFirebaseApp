using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Firebase.Firestore;
using Firebase.Extensions;

public static partial class FBSDK
{
    private static Dictionary<string, object> Data = new Dictionary<string, object>();

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

    public static async Task<bool> UpdateUserInfoDataAsyncTask(UserInfoData userInfoData)
    {
        try
        {
            var docRef = GetDocuments(userInfoData.UserUniqueID);

            Data.Clear();
            Data.Add(nameof(UserInfoData.Username), userInfoData.Username);
            Data.Add(nameof(UserInfoData.Gender), userInfoData.Gender.ToString());
            Data.Add(nameof(UserInfoData.Birthday), userInfoData.Birthday.ToShortDateString());
            Data.Add(nameof(UserInfoData.Score), userInfoData.Score);

            await docRef.UpdateAsync(Data);
            return true;
        }
        catch
        {
            return false;
        }       
    }
}
