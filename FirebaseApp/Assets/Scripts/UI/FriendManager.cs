using UnityEngine;

/// <summary>
/// フレンド管理
/// </summary>
public class FriendManager : MonoBehaviour
{
    private static FriendManager instance;
    public static FriendManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FriendManager>();
            }

            return instance;
        }
    }

    public UITweenFormTransition toWaitingRequest;
    public UITweenFormTransition toUserSearch;
    public UITweenFormTransition toFriendsList;

    public UITweenFormTransition fromWaitingRequest;
    public UITweenFormTransition fromUserSearch;
    public UITweenFormTransition fromFriendsList;

    public UITweenFormTransition toHome;

    /// <summary>
    /// 申請待ち画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_ToWaitingRequest()
    {
        toWaitingRequest.Execute();
    }

    /// <summary>
    /// フレンド検索画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_ToUserSearch()
    {
        toUserSearch.Execute();
    }

    /// <summary>
    /// フレンドリスト画面に遷移する。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_ToFriendsList()
    {
        toFriendsList.Execute();
    }

    /// <summary>
    /// 申請待ち画面からフレンド管理画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_FromWaitingRequest()
    {
        fromWaitingRequest.Execute();
    }

    /// <summary>
    /// フレンド検索画面からフレンド管理画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_FromUserSearch()
    {
        fromUserSearch.Execute();
    }

    /// <summary>
    /// フレンドリスト画面からフレンド管理画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_FromFriendsList()
    {
        fromFriendsList.Execute();
    }

    /// <summary>
    /// ホーム画面に戻る。
    /// </summary>
    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        toHome.Execute();
    }
}
