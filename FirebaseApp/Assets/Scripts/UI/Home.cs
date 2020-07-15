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

    /// <summary>
    /// 値を代入する。
    /// </summary>
    public void SetValue(UserInfoData userInfoData)
    {
        userinfo.SetValue(userInfoData);
    }

    /// <summary>
    /// Touch to Start 画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Logout()
    {
        FindObjectOfType<TouchToStart>().ReactivateTouchToStart();
        logoutTransition.Execute();
    }

    /// <summary>
    /// フレンド管理に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_FriendManager()
    {
        searchUserTransition.Execute();
    }

    /// <summary>
    /// チャット画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_ToChat()
    {
        toChat.Execute();
    }
}
