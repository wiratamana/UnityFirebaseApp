using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class FriendManager_Search : MonoBehaviour
{
    public TMP_InputField id;
    public UITweenFormTransition toUserDataAfterSearch;

    [Utils.InvokeByUnity]
    public void OnClick_Search()
    {
        GetUserInfoData();
    }

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
