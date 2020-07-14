using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendManager_WaitingRequest_UserInfo : UserInfoBase
{
    [Utils.InvokeByUnity]
    public void OnClick_Accept()
    {
        Accept();
    }

    [Utils.InvokeByUnity]
    public void OnClick_Decline()
    {
        Decline();
    }

    private async void Accept()
    {
        ConnectingDialog.Connecting(true);
        var result = await FBSDK.FriendsManager.AcceptWaitingRequestAsyncTask(playerUniqueID);
        if (result == false)
        {
            ConnectingDialog.Failed();
            return;
        }

        result = await FBSDK.ChatManager.AddNewChatRoomAsyncTask(UserData.UserUniqueID, playerUniqueID);
        if (result == false)
        {
            ConnectingDialog.Failed();
            return;
        }

        ConnectingDialog.Success();
        transition.DestroyWithAnimation();
    }

    private async void Decline()
    {
        ConnectingDialog.Connecting(true);
        var result = await FBSDK.FriendsManager.RemoveWaitingRequestAsyncTask(playerUniqueID);
        if (result == false)
        {
            ConnectingDialog.Failed();
            return;
        }

        ConnectingDialog.Success();
        transition.DestroyWithAnimation();
    }
}
