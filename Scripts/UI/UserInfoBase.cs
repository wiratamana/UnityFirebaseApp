using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInfoBase : MonoBehaviour
{
    public UserInfoUnityUI userinfo;
    public UITweenHorizontalTransition transition;

    protected string playerUniqueID;

    public void SetValue(UserInfoData userInfoData)
    {
        playerUniqueID = userInfoData.UserUniqueID;
        userinfo.SetValue(userInfoData);
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
