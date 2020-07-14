using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Utils.InvokeByUnityButton]
    public void OnClick_Logout()
    {
        FindObjectOfType<TouchToStart>().ReactivateTouchToStart();
        logoutTransition.Execute();
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_FriendManager()
    {
        searchUserTransition.Execute();
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_ToChat()
    {
        toChat.Execute();
    }
}
