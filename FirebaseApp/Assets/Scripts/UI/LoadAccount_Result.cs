using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAccount_Result : MonoBehaviour
{
    public UserInfoUnityUI userinfo;

    public UITweenFormTransition toUserSearch;
    public UITweenFormTransition toHome;

    private UserInfoData userInfoData;  

    public void SetValue(UserInfoData userInfoData)
    {
        this.userInfoData = userInfoData;
        userinfo.SetValue(userInfoData);
    }

    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        Back();
    }

    [Utils.InvokeByUnity]
    public void OnClick_LoadAccount()
    {
        LoadAccount();
    }

    private void Back()
    {
        toUserSearch.Execute();
    }

    private void LoadAccount()
    {
        UserData.UserUniqueID = userInfoData.UserUniqueID;
        UserData.MyUserData = userInfoData;
        FindObjectOfType<Home>().SetValue(userInfoData);
        toHome.Execute();
    }
}
