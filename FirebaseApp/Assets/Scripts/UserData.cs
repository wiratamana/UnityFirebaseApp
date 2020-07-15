/// <summary>
/// 端末内のユーザーデータ
/// </summary>
public static class UserData
{
    /// <summary>
    /// 端末内に保存されたユーザー ID です。
    /// </summary>
    public static string UserUniqueID
    {
        get => UnityEngine.PlayerPrefs.GetString(nameof(UserUniqueID), null);
        set
        {
            UnityEngine.PlayerPrefs.SetString(nameof(UserUniqueID), value);
            UserUniqueIDChanged?.Invoke(value);
        }
    }

    public static event System.Action<string> UserUniqueIDChanged;

    public static UserInfoData MyUserData;
}
