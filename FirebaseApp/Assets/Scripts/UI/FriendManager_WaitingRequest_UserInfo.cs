/// <summary>
/// 承認待ちのユーザー情報
/// </summary>
public class FriendManager_WaitingRequest_UserInfo : UserInfoBase
{
    /// <summary>
    /// フレンドを申請する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Accept()
    {
        Accept();
    }

    /// <summary>
    /// 申請したフレンドを削除する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Decline()
    {
        Decline();
    }

    /// <summary>
    /// 承認して、フレンドになる。
    /// </summary>
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

    /// <summary>
    /// 削除する。
    /// </summary>
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
