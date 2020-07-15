using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 承認待ち画面。
/// </summary>
public class FriendManager_WaitingRequest : MonoBehaviour
{
    public FriendManager_WaitingRequest_UserInfo prefab;
    public Transform parent;

    private List<FriendManager_WaitingRequest_UserInfo> userInfos = new List<FriendManager_WaitingRequest_UserInfo>();

    private void Awake()
    {
        FriendManager.Instance.toWaitingRequest.TransitionCompleted += ShowWaitingRequest;
        FriendManager.Instance.fromWaitingRequest.TransitionCompleted += DestroyAllPrefabs;
    }

    /// <summary>
    /// 承認待ちのユーザー情報を生成する。
    /// </summary>
    public async void ShowWaitingRequest()
    {
        ConnectingDialog.Connecting(true);

        var waitingRequestList = await FBSDK.FriendsManager.GetWaitingRequestAsyncTask();

        if (waitingRequestList == null)
        {
            ConnectingDialog.Failed();
            return;
        }

        float delay = 0.0f;
        float stride = 0.1f;
        for (int i = 0; i < waitingRequestList.Count; i++)
        {
            var userInfo = Instantiate(prefab);
            userInfo.transform.SetParent(parent);
            userInfo.transition.ActivateAndPlayAnimation(delay);
            userInfo.SetValue(waitingRequestList[i]);
            delay += stride;

            userInfos.Add(userInfo);
        }

        ConnectingDialog.Success();
    }

    /// <summary>
    /// 生成した、ユーザー情報を全部削除する。
    /// </summary>
    public void DestroyAllPrefabs()
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
            catch { }
            
        }
    }
}
