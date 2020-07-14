using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendManager_Search_Result : MonoBehaviour
{
    public UserInfoUnityUI userinfo;

    public UITweenFormTransition backToUserSearch;

    private string playerUniqueID;

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

    private void BackToUserSearch()
    {
        backToUserSearch.Execute();
    }

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
