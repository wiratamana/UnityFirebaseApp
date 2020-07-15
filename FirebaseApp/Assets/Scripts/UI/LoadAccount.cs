using UnityEngine;
using TMPro;

/// <summary>
/// アカウント連携用。
/// </summary>
public class LoadAccount : MonoBehaviour
{
    public TMP_InputField userUniqueID;

    public UITweenFormTransition toTouchToStart;
    public UITweenFormTransition toLoadAccountResult;

    /// <summary>
    /// アカウントを検索する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Search()
    {
        ToLoadAccountResult();
    }

    /// <summary>
    /// Touch to Start 画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        ToTouchToStart();
    }

    /// <summary>
    /// 記入してもらったユーザーIDを使ってユーザー検索する。ユーザーが見つかったら連携する確認画面に移動。
    /// </summary>
    private async void ToLoadAccountResult()
    {
        ConnectingDialog.Connecting(true);
        var id = Utils.GetUserUniqueIDFromInput(userUniqueID.text);

        if (string.IsNullOrEmpty(id))
        {
            ConnectingDialog.Failed();
            return;
        }

        var userInfoData = await FBSDK.GetUserInfoDataAsyncTask(id);

        if (userInfoData.IsNull)
        {
            ConnectingDialog.Failed();
        }
        else
        {
            ConnectingDialog.Success();

            userUniqueID.text = null;
            FindObjectOfType<LoadAccount_Result>().SetValue(userInfoData);
            toLoadAccountResult.Execute();
        }
    }

    /// <summary>
    /// Touch to Start 画面に戻る。
    /// </summary>
    private void ToTouchToStart()
    {
        toTouchToStart.Execute();
    }
}
