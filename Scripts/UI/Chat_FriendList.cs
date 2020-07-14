using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

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

    private void Start()
    {
        Home.Instance.toChat.TransitionCompleted += InstantaiteUserInfo;
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_Back()
    {
        ToHome.Execute();
    }

    private async void InstantaiteUserInfo()
    {
        ConnectingDialog.Connecting(true);
        var friendList = await FBSDK.FriendsManager.GetFriendListAsyncTask();
        var tasks = new List<Task<FBSDK.ChatManager.Metadata>>();

        foreach (var item in friendList)
        {
            tasks.Add(FBSDK.ChatManager.GetChatMetadataAsyncTask(item));
        }

        var result = await Task.WhenAll(tasks);

        float delay = 0.0f;
        float stride = 0.01f;
        foreach (var item in result)
        {
            var info = Instantiate(prefab, content);
            info.SetValue(item);
            info.transition.ActivateAndPlayAnimation(delay);
            delay += stride;
            Debug.Log(item);
        }

        ConnectingDialog.Success();
    }
}
