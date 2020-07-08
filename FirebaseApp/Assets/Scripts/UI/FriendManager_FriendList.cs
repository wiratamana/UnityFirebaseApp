using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FriendManager_FriendList : MonoBehaviour
{
    public FriendManager_FriendList_UserInfo prefab;
    public Transform contentParent;

    private List<FriendManager_FriendList_UserInfo> userInfos = new List<FriendManager_FriendList_UserInfo>();

    private void Start()
    {
        FriendManager.Instance.toFriendsList.TransitionCompleted += InstantiateFriendsInfo;
        FriendManager.Instance.fromFriendsList.TransitionCompleted += DestroyFriendsInfo;
    }

    private async void InstantiateFriendsInfo()
    {
        ConnectingDialog.Connecting(true);
        var friendList = await FBSDK.FriendsManager.GetFriendListAsyncTask();

        if (friendList == null)
        {
            ConnectingDialog.Failed();
            return;
        }

        float delay = 0.0f;
        float stride = 0.1f;
        foreach (var item in friendList)
        {
            var userInfo = Instantiate(prefab);
            userInfo.transform.SetParent(contentParent);
            userInfo.ActivateAndPlayAnimation(delay);
            userInfo.SetValue(item);
            delay += stride;

            userInfos.Add(userInfo);
        }
        ConnectingDialog.Success();
    }

    private void DestroyFriendsInfo()
    {
        if (userInfos.Count == 0)
        {
            return;
        }

        foreach (var item in userInfos)
        {
            try
            {
                Destroy(item);
            }
            catch
            {
                
            }            
        }

        userInfos.Clear();
    }
}
