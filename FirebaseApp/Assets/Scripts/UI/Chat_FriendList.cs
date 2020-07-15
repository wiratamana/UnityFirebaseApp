using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

/// <summary>
/// チャット画面。
/// </summary>
public class Chat_FriendList : MonoBehaviour
{
    private static Chat_FriendList instance;
    public static Chat_FriendList Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Chat_FriendList>();
            }

            return instance;
        }
    }

    public Chat_FriendList_UserInfo prefab;
    public Transform content;

    public UITweenFormTransition ToHome;

    private readonly List<Chat_FriendList_UserInfo> userInfos = new List<Chat_FriendList_UserInfo>();

    private void Start()
    {
        prefab.transition.animationGO.SetActive(false);
        Home.Instance.toChat.TransitionCompleted += InstantaiteUserInfo;
    }

    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        ToHome.TransitionCompleted += DestroyMessage;
        ToHome.Execute();
    }

    /// <summary>
    /// フレンド情報を生成する。
    /// </summary>
    private async void InstantaiteUserInfo()
    {
        ConnectingDialog.Connecting(true);
        var friendList = await FBSDK.FriendsManager.GetFriendListAsyncTask();
        var tasks = new List<Task<ChatRoom>>();

        foreach (var item in friendList)
        {
            tasks.Add(FBSDK.ChatManager.GetChatMetadataAsyncTask(item));
        }

        var result = await Task.WhenAll(tasks);

        userInfos.Clear();

        float delay = 0.0f;
        float stride = 0.01f;
        foreach (var item in result)
        {
            var info = Instantiate(prefab, content);
            info.SetValue(item);
            info.transition.ActivateAndPlayAnimation(delay);
            userInfos.Add(info);

            delay += stride;
        }

        ConnectingDialog.Success();
    }

    /// <summary>
    /// 生成したフレンド情報を破壊する。
    /// </summary>
    private void DestroyMessage()
    {
        ToHome.TransitionCompleted -= DestroyMessage;

        foreach (var item in userInfos)
        {
            Destroy(item);
        }
    }
}
