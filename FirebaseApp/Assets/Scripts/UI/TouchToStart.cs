using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchToStart : MonoBehaviour
{
    public TextMeshProUGUI touchToStartMessage;
    public UITweenFormTransition register;
    public UITweenFormTransition toLoadAccount;
    public UITweenFormTransition fromRegisterToHome;
    public UITweenFormTransition fromTitleToHome;
    public TextMeshProUGUI UniqueIDText;

    [Utils.InvokeByUnity]
    public void OnClick_Login()
    {
        StopCoroutine(PingPong());
        touchToStartMessage.gameObject.SetActive(false);

        LoginProcess();
    }

    [Utils.InvokeByUnity]
    public void OnClick_LoadAccount()
    {
        LoadAccount();
    }

    private void LoadAccount()
    {
        toLoadAccount.Execute();
    }

    private async void LoginProcess()
    {
        ConnectingDialog.Connecting(true);

        if (string.IsNullOrEmpty(UserData.UserUniqueID))
        {
            var userUniqueID = await FBFunctions.CreateEmptyUser();
            if (string.IsNullOrEmpty(userUniqueID))
            {
                ConnectingDialog.Failed();
                return;
            }

            UserData.UserUniqueID = userUniqueID;

            GoToRegisterForm();
        }
        else
        {
            var data = await FBSDK.GetUserInfoDataAsyncTask(UserData.UserUniqueID);
            UserData.MyUserData = data;

            ConnectingDialog.Success();
            Home.Instance.SetValue(UserData.MyUserData);
            ToHome();
        }
    }

    private void GoToRegisterForm()
    {
        ConnectingDialog.Success();
        RegisterForm.AccountCreated += RegisterForm_AccountCreated;
        register.Execute();
    }

    private async void RegisterForm_AccountCreated(string username, Gender gender, System.DateTime birthday)
    {
        RegisterForm.AccountCreated -= RegisterForm_AccountCreated;

        ConnectingDialog.Connecting(true);
        var myUserData = new UserInfoData(UserData.UserUniqueID, username, birthday, gender, 0);
        var result = await FBSDK.UpdateUserInfoDataAsyncTask(myUserData);
        if (result == false)
        {
            ConnectingDialog.Failed();
            return;
        }

        ConnectingDialog.Success();

        UserData.MyUserData = myUserData;
        Home.Instance.SetValue(myUserData);
        ToHome();
    }

    private void ToHome()
    {
        if (FindObjectOfType<RegisterForm>().IsOnScreen)
        {
            fromRegisterToHome.Execute();
            return;
        }

        fromTitleToHome.Execute();
    }

    private void Start()
    {
        UserData.UserUniqueIDChanged += (uniqueID) => UniqueIDText.text = $"ID : {uniqueID}";
        StartCoroutine(PingPong());

        var uid = UserData.UserUniqueID;
        if (string.IsNullOrEmpty(uid))
        {
            UniqueIDText.text = $"ID : 未登録";
        }
        else
        {
            UniqueIDText.text = $"ID : {uid}";
        }       
    }

    public void ReactivateTouchToStart()
    {
        touchToStartMessage.gameObject.SetActive(true);
        StartCoroutine(PingPong());
    }

    private IEnumerator PingPong()
    {
        touchToStartMessage.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(1.0f);

        var t = 1.0f;
        float v;
        while (true)
        {
            v = Mathf.PingPong(t, 1.0f);
            t += Time.deltaTime;

            var c = touchToStartMessage.color;
            c.a = v;
            touchToStartMessage.color = c;

            yield return null;
        }
    }
}
