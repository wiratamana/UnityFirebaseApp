using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// タッチして、ログインする
/// </summary>
public class TouchToStart : MonoBehaviour
{
    public TextMeshProUGUI touchToStartMessage;
    public UITweenFormTransition register;
    public UITweenFormTransition toLoadAccount;
    public UITweenFormTransition fromRegisterToHome;
    public UITweenFormTransition fromTitleToHome;
    public TextMeshProUGUI UniqueIDText;

    /// <summary>
    /// ログイン処理を走らせる。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Login()
    {
        StopCoroutine(PingPong());
        touchToStartMessage.gameObject.SetActive(false);

        LoginProcess();
    }

    /// <summary>
    /// アカウント連携画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_LoadAccount()
    {
        LoadAccount();
    }

    private void LoadAccount()
    {
        toLoadAccount.Execute();
    }

    /// <summary>
    /// ログインする。
    /// </summary>
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

    /// <summary>
    /// Touch to Start から ユーザー情報を記入するフォームへ移動。
    /// </summary>
    private void GoToRegisterForm()
    {
        ConnectingDialog.Success();
        RegisterForm.AccountCreated += OnAccountCreated;
        register.Execute();
    }

    /// <summary>
    /// ユーザー情報を記入後のコールバック。
    /// </summary>
    /// <param name="username">ユーザー名</param>
    /// <param name="gender">性別</param>
    /// <param name="birthday">誕生日</param>
    private async void OnAccountCreated(string username, Gender gender, System.DateTime birthday)
    {
        RegisterForm.AccountCreated -= OnAccountCreated;

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

    /// <summary>
    /// ログインに成功したときにホーム画面に移動。
    /// </summary>
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

    /// <summary>
    /// ホームからログインするときに、Touch to Start のアニメーションを再生する。
    /// </summary>
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
