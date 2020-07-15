using UnityEngine;

/// <summary>
/// フレンド検索したの結果。
/// </summary>
public class FriendManager_Search_Result : MonoBehaviour
{
    public UserInfoUnityUI userinfo;

    public UITweenFormTransition backToUserSearch;

    private string playerUniqueID;

    /// <summary>
    /// 値を代入する。
    /// </summary>
    /// <param name="userInfoData">ユーザー情報</param>
    public void SetValue(UserInfoData userInfoData)
    {
        playerUniqueID = userInfoData.UserUniqueID;

        userinfo.SetValue(userInfoData);
    }

    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        BackToUserSearch();
    }

    [Utils.InvokeByUnity]
    public void OnClick_SendFriendRequest()
    {
        SendRequestFriend();
    }

    /// <summary>
    /// ユーザー検索画面に戻る。
    /// </summary>
    private void BackToUserSearch()
    {
        backToUserSearch.Execute();
    }

    /// <summary>
    /// フレンド申請する。
    /// </summary>
    private async void SendRequestFriend()
    {
        ConnectingDialog.Connecting(true);
        var result = await FBSDK.FriendsManager.SendFriendRequestAsyncTask(playerUniqueID);
        if (result == false)
        {
            ConnectingDialog.Failed();
        }
        else
        {
            ConnectingDialog.Success();
            BackToUserSearch();
        }
    }
}
