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

    [SerializeField] private UserInfoUnityUI userinfo;

    [SerializeField] private UITweenFormTransition logoutTransition;
    [SerializeField] private UITweenFormTransition searchUserTransition;

    [SerializeField] private UITweenFormTransition toChat;

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
