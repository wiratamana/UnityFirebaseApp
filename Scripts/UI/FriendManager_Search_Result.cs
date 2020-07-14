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

    [Utils.InvokeByUnityButton]
    public void OnClick_Back()
    {
        backToUserSearch.Execute();
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_SendFriendRequest()
    {
        SendRequestFriend();
    }

    private async void SendRequestFriend()
    {
        var result = await FBSDK.FriendsManager.SendFriendRequestAsyncTask(playerUniqueID);
        if (result == false)
        {
            Debug.Log("Failed");
        }
        else
        {
            Debug.Log("Success");
        }
    }
}
