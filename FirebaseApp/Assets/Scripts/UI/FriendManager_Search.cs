﻿using UnityEngine;
using TMPro;

/// <summary>
/// ユーザー検索
/// </summary>
public class FriendManager_Search : MonoBehaviour
{
    public TMP_InputField id;
    public UITweenFormTransition toUserDataAfterSearch;

    /// <summary>
    /// フレンドを検索する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Search()
    {
        GetUserInfoData();
    }

    /// <summary>
    /// 入力してもらった ID でユーザー情報を検索する。
    /// </summary>
    private async void GetUserInfoData()
    {
        var id = Utils.GetUserUniqueIDFromInput(this.id.text);

        if (string.IsNullOrEmpty(id))
        {
            ConnectingDialog.Failed();
            return;
        }        

        ConnectingDialog.Connecting(true);
        var userInfoData = await FBSDK.GetUserInfoDataAsyncTask(id);

        if (userInfoData.IsNull)
        {
            ConnectingDialog.Failed();
        }
        else
        {
            ConnectingDialog.Success();

            this.id.text = null;
            FindObjectOfType<FriendManager_Search_Result>().SetValue(userInfoData);
            toUserDataAfterSearch.Execute();
        }
    }
}
