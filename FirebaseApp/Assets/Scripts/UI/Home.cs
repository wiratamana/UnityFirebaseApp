using UnityEngine;

/// <summary>
/// ホーム画面
/// </summary>
public class Home : MonoBehaviour
{
    private static Home instance;
    public static Home Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Home>();
            }

            return instance;
        }
    }

    public UserInfoUnityUI userinfo;

    public UITweenFormTransition logoutTransition;
    public UITweenFormTransition searchUserTransition;

    public UITweenFormTransition toChat;

    public void SetValue(UserInfoData userInfoData)
    {
        userinfo.SetValue(userInfoData);
    }

    [Utils.InvokeByUnity]
    public void OnClick_Logout()
    {
        FindObjectOfType<TouchToStart>().ReactivateTouchToStart();
        logoutTransition.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_FriendManager()
    {
        searchUserTransition.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_ToChat()
    {
        toChat.Execute();
    }
}
