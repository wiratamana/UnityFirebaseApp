public static class UserData
{
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

    public const string FIELD_FRIEND_REQUEST = "FriendRequest";
    public const string FIELD_FRIEND_LIST = "FriendList";
    public const string FIELD_FRIEND_CHAT_ROOM_IDS = "FriendChatRoomIDs";
    public const string FIELD_CHAT_ROOM_ID = "ChatRoomID";
    public const string FIELD_CHAT_OBJECTS = "ChatObjects";
}
