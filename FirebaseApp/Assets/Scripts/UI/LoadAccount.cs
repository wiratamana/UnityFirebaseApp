using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadAccount : MonoBehaviour
{
    public TMP_InputField userUniqueID;

    public UITweenFormTransition toTouchToStart;
    public UITweenFormTransition toLoadAccountResult;

    [Utils.InvokeByUnityButton]
    public void OnClick_Search()
    {
        ToLoadAccountResult();
    }

    [Utils.InvokeByUnityButton]
    public void OnClick_Back()
    {
        ToTouchToStart();
    }

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

    private void ToTouchToStart()
    {
        toTouchToStart.Execute();
    }
}
