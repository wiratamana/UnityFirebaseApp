using UnityEngine;

/// <summary>
/// アニメーションとユーザー情報が入っているベースクラス
/// </summary>
public abstract class UserInfoBase : MonoBehaviour
{
    public UserInfoUnityUI userinfo;
    public UITweenHorizontalTransition transition;

    protected string playerUniqueID;

    /// <summary>
    /// 値を代入する。
    /// </summary>
    /// <param name="userInfoData">ユーザー情報</param>
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
