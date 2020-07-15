/// <summary>
/// フレンドリスト中のユーザー情報
/// </summary>
public class FriendManager_FriendList_UserInfo : UserInfoBase
{
    [Utils.InvokeByUnity]
    public void OnClick_Remove()
    {
        Remove();
    }

    /// <summary>
    /// フレンドを削除する。
    /// </summary>
    private async void Remove()
    {
        ConnectingDialog.Connecting(true);
        var result = await FBSDK.FriendsManager.RemoveFriendAysncTask(playerUniqueID);
        if (result == false)
        {
            ConnectingDialog.Failed();
            return;
        }

        ConnectingDialog.Success();
        transition.DestroyWithAnimation();
    }
}
