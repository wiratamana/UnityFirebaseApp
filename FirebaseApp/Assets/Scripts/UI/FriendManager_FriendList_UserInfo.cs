using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FriendManager_FriendList_UserInfo : UserInfoBase
{
    [Utils.InvokeByUnity]
    public void OnClick_Remove()
    {
        Remove();
    }

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
