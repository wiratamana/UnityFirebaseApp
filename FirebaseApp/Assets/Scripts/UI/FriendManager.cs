using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Utils.InvokeByUnity]
    public void OnClick_ToWaitingRequest()
    {
        toWaitingRequest.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_ToUserSearch()
    {
        toUserSearch.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_ToFriendsList()
    {
        toFriendsList.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_FromWaitingRequest()
    {
        fromWaitingRequest.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_FromUserSearch()
    {
        fromUserSearch.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_FromFriendsList()
    {
        fromFriendsList.Execute();
    }

    [Utils.InvokeByUnity]
    public void OnClick_Back()
    {
        toHome.Execute();
    }
}
